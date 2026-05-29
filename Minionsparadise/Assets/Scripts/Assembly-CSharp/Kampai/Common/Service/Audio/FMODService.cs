namespace Kampai.Common.Service.Audio
{
	public class FMODService : global::Kampai.Common.Service.Audio.IFMODService
	{
		private struct PendingBank
		{
			public global::FMOD.Studio.Bank Bank;

			public string Name;

			public override string ToString()
			{
				return Name ?? "uninitialized";
			}
		}

		private sealed class InitializeSystem_Iterator : global::System.IDisposable, global::System.Collections.Generic.IEnumerator<object>, global::System.Collections.IEnumerator
		{
			internal int _PC;
			internal object _current;
			internal global::Kampai.Common.Service.Audio.FMODService _this;

			object global::System.Collections.Generic.IEnumerator<object>.Current { get { return _current; } }
			object global::System.Collections.IEnumerator.Current { get { return _current; } }

			public bool MoveNext()
			{
				uint num = (uint)_PC;
				_PC = -1;
				switch (num)
				{
				case 0u:
					_this.logger.Info("FMODService: InitializeSystem: Step 0");
					_this.logger.EventStart("FMODService.InitializeSystem");
					global::Kampai.Util.TimeProfiler.StartSection("fmod");
					global::Kampai.Util.TimeProfiler.StartSection("maps");
					_this.LoadEventMapsFromRawFiles();
					global::Kampai.Util.TimeProfiler.EndSection("maps");
					global::Kampai.Util.TimeProfiler.StartSection("banks load start");
					_this.LoadAllBanksFromFilesystem();
					global::Kampai.Util.TimeProfiler.EndSection("banks load start");
					global::Kampai.Util.TimeProfiler.StartSection("streaming");
					_current = _this.LoadStreamingAudioBanks();
					_PC = 1;
					return true;
				case 1u:
					_this.logger.Info("FMODService: InitializeSystem: Step 1 (Finalizing)");
					global::Kampai.Util.TimeProfiler.EndSection("streaming");
					global::Kampai.Util.TimeProfiler.EndSection("fmod");
					_this.logger.EventStop("FMODService.InitializeSystem");
					_PC = -1;
					break;
				}
				return false;
			}

			public void Dispose() { _PC = -1; }
			public void Reset() { throw new global::System.NotSupportedException(); }
		}

		private const string TAG = "FMODService";
		private readonly global::Kampai.Common.IManifestService _manifestService;
		private readonly global::System.Collections.Generic.Dictionary<string, string> _nameIdMap = new global::System.Collections.Generic.Dictionary<string, string>();
		private global::System.Collections.Generic.Queue<global::Kampai.Common.Service.Audio.FMODService.PendingBank> pendingBanks = new global::System.Collections.Generic.Queue<global::Kampai.Common.Service.Audio.FMODService.PendingBank>();
		private global::System.Diagnostics.Stopwatch allBanksAsyncSW;
		private bool isProcessingBanks;
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("FMODService") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Main.ILocalContentService localContentService { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		public FMODService(global::Kampai.Common.IManifestService manifestService)
		{
			_manifestService = manifestService;
		}

		public global::System.Collections.IEnumerator InitializeSystem()
		{
			global::Kampai.Common.Service.Audio.FMODService.InitializeSystem_Iterator iterator = new global::Kampai.Common.Service.Audio.FMODService.InitializeSystem_Iterator();
			iterator._this = this;
			return iterator;
		}

		public string GetGuid(string eventName)
		{
			if (_nameIdMap.ContainsKey(eventName))
			{
				return _nameIdMap[eventName];
			}
			logger.Error("eventName '{0}' was not found in the dictionary.", eventName);
			return null;
		}

		private void LoadAllBanksFromFilesystem()
		{
			logger.Info("FMODService: Scanning for .bank files in DLC paths.");
			allBanksAsyncSW = global::System.Diagnostics.Stopwatch.StartNew();

			string[] searchPaths = { global::Kampai.Util.GameConstants.DLC_PATH, global::Kampai.Util.GameConstants.PRE_INSTALLED_DLC_PATH };
			foreach (string path in searchPaths)
			{
				if (global::System.IO.Directory.Exists(path))
				{
					string[] files = global::System.IO.Directory.GetFiles(path, "*.bank", global::System.IO.SearchOption.AllDirectories);
					foreach (string file in files)
					{
						logger.Info("FMODService: scheduled bank load from file: {0}", file);
						LoadLocalBankAsync(file);
					}
				}
			}
		}

		private string GetEventMapFilePath()
		{
			string resourceFolder = global::Kampai.Util.GameConstants.PRE_INSTALLED_FMOD_PATH;
			string path = global::System.IO.Path.Combine(global::UnityEngine.Application.persistentDataPath, resourceFolder + "Raw_FMOD_GlobalMap.json");
			if (global::System.IO.File.Exists(path)) return path;

#if UNITY_ANDROID && !UNITY_EDITOR
			path = global::System.IO.Path.Combine(global::UnityEngine.Application.persistentDataPath, "Raw_FMOD_GlobalMap.json");
			if (global::System.IO.File.Exists(path)) return path;
#endif
			return path;
		}

		private void LoadEventMapsFromRawFiles()
		{
			global::Kampai.Common.Service.Audio.FmodGlobalEventMap fmodGlobalEventMap = null;
			try
			{
				string eventMapFilePath = GetEventMapFilePath();
				logger.Info("Loading FmodGlobalEventMap from: {0}", eventMapFilePath);
				if (global::System.IO.File.Exists(eventMapFilePath))
				{
					using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(eventMapFilePath))
					{
						global::Newtonsoft.Json.JsonTextReader reader = new global::Newtonsoft.Json.JsonTextReader(streamReader);
						fmodGlobalEventMap = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Common.Service.Audio.FmodGlobalEventMap>(reader);
					}
				}
				
				if (fmodGlobalEventMap == null)
				{
					string resourceFolder = global::Kampai.Util.GameConstants.PRE_INSTALLED_FMOD_PATH;
					string resourcePath = resourceFolder + "Raw_FMOD_GlobalMap";
					logger.Info("FmodGlobalEventMap not found at {0}, trying Resources fallback at {1}...", eventMapFilePath, resourcePath);
					global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>(resourcePath);
					if (textAsset == null)
					{
						// Legacy fallback
						textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>("content/Raw_FMOD_GlobalMap");
					}
					
					if (textAsset != null)
					{
						fmodGlobalEventMap = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Common.Service.Audio.FmodGlobalEventMap>(textAsset.text);
						logger.Info("FmodGlobalEventMap loaded from Resources.");
					}
					else
					{
						logger.Error("FmodGlobalEventMap file not found in Resources either (checked {0} and 'content/Raw_FMOD_GlobalMap')", resourcePath);
					}
				}
			}
			catch (global::System.Exception ex)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.FMOD_EVENT_MAP_ERROR, "FmodGlobalEventMap load error: {0}", ex);
			}

			if (fmodGlobalEventMap == null || fmodGlobalEventMap.maps == null)
			{
				logger.Error("FmodGlobalEventMap is null or has no maps. Skipping map processing.");
				return;
			}
			foreach (var map in fmodGlobalEventMap.maps)
			{
				foreach (var item in map.Value)
				{
					if (!_nameIdMap.ContainsKey(item.Key))
					{
						_nameIdMap.Add(item.Key, item.Value);
					}
				}
			}
		}

		public void StartAsyncBankLoadingProcessing()
		{
			routineRunner.StartCoroutine(ProcessAsyncBankLoading());
		}

		private global::System.Collections.IEnumerator ProcessAsyncBankLoading()
		{
			isProcessingBanks = true;
			float startTime = global::UnityEngine.Time.realtimeSinceStartup;
			const float timeout = 30f; // 30 seconds timeout
			try
			{
				while (pendingBanks.Count > 0)
				{
					if (global::UnityEngine.Time.realtimeSinceStartup - startTime > timeout)
					{
						logger.Error("FMODService: Timeout waiting for banks to load! Stuck banks: {0}", pendingBanks.Count);
						foreach (var pb in pendingBanks)
						{
							logger.Error("  - Stuck bank: {0}", pb.Name);
						}
						pendingBanks.Clear();
						break;
					}
					int queueCount = pendingBanks.Count;
					while (queueCount-- != 0)
					{
						global::Kampai.Common.Service.Audio.FMODService.PendingBank pb = pendingBanks.Dequeue();
						global::FMOD.Studio.LOADING_STATE bankState;
						if (pb.Bank.getLoadingState(out bankState) == global::FMOD.RESULT.OK)
						{
							switch (bankState)
							{
							case global::FMOD.Studio.LOADING_STATE.LOADING:
								pendingBanks.Enqueue(pb);
								break;
							case global::FMOD.Studio.LOADING_STATE.ERROR:
								logger.Error("FMODService: error bank state {0}, bank {1}", bankState, pb);
								pb.Bank.unload();
								break;
							}
						}
					}
					if (pendingBanks.Count == 0)
					{
						break;
					}
					yield return null;
				}
			}
			finally
			{
				isProcessingBanks = false;
				logger.Debug("FMODService: Loading process finished. Elapsed: {0}s", global::UnityEngine.Time.realtimeSinceStartup - startTime);
			}
		}

		public bool BanksLoadingInProgress()
		{
			if (pendingBanks.Count > 0 && !isProcessingBanks)
			{
				logger.Warning("FMODService: banks pending but no coroutine running! Restarting...");
				StartAsyncBankLoadingProcessing();
			}
			return pendingBanks.Count != 0;
		}

		private string ExtractBankToFile(global::UnityEngine.TextAsset asset)
		{
			if (asset == null) return null;
			try
			{
				string bankDir = global::System.IO.Path.Combine(global::UnityEngine.Application.persistentDataPath, "FMOD");
				if (!global::System.IO.Directory.Exists(bankDir))
				{
					global::System.IO.Directory.CreateDirectory(bankDir);
				}
				string bankPath = global::System.IO.Path.Combine(bankDir, asset.name);
				if (!bankPath.EndsWith(".bank")) bankPath += ".bank";
				
				// Optional: Only write if file doesn't exist or is different size
				if (!global::System.IO.File.Exists(bankPath) || new global::System.IO.FileInfo(bankPath).Length != asset.bytes.Length)
				{
					logger.Debug("FMODService: Extracting bank {0} to {1}", asset.name, bankPath);
					global::System.IO.File.WriteAllBytes(bankPath, asset.bytes);
				}
				return bankPath;
			}
			catch (global::System.Exception ex)
			{
				logger.Error("FMODService: Failed to extract bank {0}: {1}", asset.name, ex.Message);
				return null;
			}
		}

		private global::FMOD.Studio.Bank LoadLocalBankAsync(string bankFile)
		{
			global::FMOD.Studio.Bank bank = default(global::FMOD.Studio.Bank);
			global::FMOD.Studio.System system = FMOD_StudioSystem.instance.System;
			if (!system.isValid())
			{
				logger.Error("FMODService: Cannot load bank {0} because FMOD System is null", bankFile);
				return default(global::FMOD.Studio.Bank);
			}
			global::FMOD.RESULT result = system.loadBankFile(bankFile, global::FMOD.Studio.LOAD_BANK_FLAGS.NONBLOCKING, out bank);
			if (result == global::FMOD.RESULT.OK)
			{
				bool wasEmpty = pendingBanks.Count == 0;
				pendingBanks.Enqueue(new PendingBank { Bank = bank, Name = bankFile });
				if (wasEmpty)
				{
					StartAsyncBankLoadingProcessing();
				}
			}
			else
			{
				logger.Error("FMODService: loadBankFile failed for {0} with error {1}", bankFile, result);
			}
			return bank;
		}

		private string GetStreamingBankPath(string bank)
		{
			string resourceFolder = global::Kampai.Util.GameConstants.PRE_INSTALLED_FMOD_PATH;
			string resourcePath = resourceFolder + bank;
			global::UnityEngine.TextAsset asset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>(resourcePath);
			if (asset != null)
			{
				return ExtractBankToFile(asset);
			}
			
			// Fallback to legacy path
			return global::Kampai.Util.GameConstants.PRE_INSTALLED_DLC_PATH + bank + ".bank";
		}

		private global::System.Collections.IEnumerator LoadStreamingAudioBanks()
		{
			logger.Debug("FMODService: Loading Streaming Audio Banks from Resources");
			string resourceFolder = global::Kampai.Util.GameConstants.PRE_INSTALLED_FMOD_PATH;
			if (resourceFolder.EndsWith("/") || resourceFolder.EndsWith("\\"))
			{
				resourceFolder = resourceFolder.Substring(0, resourceFolder.Length - 1);
			}

			global::UnityEngine.TextAsset[] bankAssets = global::UnityEngine.Resources.LoadAll<global::UnityEngine.TextAsset>(resourceFolder);
			if (bankAssets != null && bankAssets.Length > 0)
			{
				logger.Debug("FMODService: Found {0} banks in Resources/{1}", bankAssets.Length, resourceFolder);
				foreach (var asset in bankAssets)
				{
					if (asset.name.Contains("GlobalMap")) continue; // Skip map
					
					string bankPath = ExtractBankToFile(asset);
					if (!string.IsNullOrEmpty(bankPath))
					{
						LoadLocalBankAsync(bankPath);
					}
					// Optional: resources can be large, maybe unload after getting bytes?
					// Wait, we need it in memory for ExtractBankToFile.
				}
			}
			else
			{
				logger.Warning("FMODService: No banks found in Resources/{0}", resourceFolder);
			}
			
			// Fallback to localContentService list (for things that might not be in LoadAll)
			if (localContentService != null)
			{
				var streamingBanks = localContentService.GetStreamingAudioBanks();
				if (streamingBanks != null)
				{
					foreach (string bankName in streamingBanks)
					{
						string path = GetStreamingBankPath(bankName);
						if (!string.IsNullOrEmpty(path) && global::System.IO.File.Exists(path)) 
						{
							LoadLocalBankAsync(path);
						}
					}
				}
			}
			yield break;
		}

		private string GetRawAssetPathByOriginalName(string assetFileName)
		{
			string assetFileLocation = _manifestService.GetAssetFileLocation(assetFileName);
			return global::System.IO.Path.Combine(assetFileLocation, assetFileName);
		}
	}
}
