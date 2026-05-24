public class LoadDefinitionsCommand : global::strange.extensions.command.impl.Command
{
	public class LoadDefinitionsData
	{
		public string Path { get; set; }

		public string Json { get; set; }
	}

	public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadDefinitionsCommand") as global::Kampai.Util.IKampaiLogger;

	[Inject]
	public bool hotSwap { get; set; }

	[Inject]
	public LoadDefinitionsCommand.LoadDefinitionsData defData { get; set; }

	[Inject]
	public global::Kampai.Game.IDefinitionService definitionService { get; set; }

	[Inject]
	public global::Kampai.Game.DefinitionsChangedSignal definitionsChangedSignal { get; set; }

	[Inject]
	public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

	[Inject]
	public global::Kampai.Game.IDLCService dlcService { get; set; }

	[Inject]
	public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

	[Inject]
	public global::Kampai.Util.IInvokerService invokerService { get; set; }

	[Inject]
	public global::Kampai.Game.IPlayerService playerService { get; set; }

	[Inject]
	public global::Kampai.Splash.SplashProgressUpdateSignal splashProgressUpdateSignal { get; set; }

	public override void Execute()
	{
		global::Kampai.Util.StartupTimer.LogCheckpoint("LoadDefinitionsCommand.Execute");
		logger.EventStart("LoadDefinitionsCommand.Execute");
		string jsonString = defData.Json;
		if (jsonString != null)
		{
			logger.Debug("LoadDefinitions: Starting json deserialization from string");
			routineRunner.StartAsyncConditionTask(() => DeserializeDefinitionsFromJsonString(jsonString), OnDeserializationSuccess);
		}
		else
		{
			string jsonPath = defData.Path;
			if (string.IsNullOrEmpty(jsonPath))
			{
				throw new global::System.ArgumentException("LoadDefinitionsCommand: neither json content nor path to file is specified");
			}
			bool needsJsonParse = true;
			string binaryDefinitionsPath = global::Kampai.Game.DefinitionService.GetBinaryDefinitionsPath();
#if !UNITY_WEBPLAYER
			if (global::System.IO.File.Exists(binaryDefinitionsPath) && IsBinaryCacheValid(jsonPath))
			{
				logger.Debug("LoadDefinitions: Starting binary deserialization (cache valid)");
				routineRunner.StartAsyncConditionTask(delegate
				{
					return DeserializeDefinitionsFromBinaryFile(binaryDefinitionsPath);
				}, delegate
				{
					logger.Info("LoadDefinitions: Binary cache loaded successfully");
					OnDeserializationSuccess();
				});
				needsJsonParse = false;
			}
			else
			{
				if (global::System.IO.File.Exists(binaryDefinitionsPath))
				{
					logger.Info("LoadDefinitions: Binary cache invalidated (JSON changed), deleting stale cache");
					global::Kampai.Game.DefinitionService.DeleteBinarySerialization();
				}
			}
#else
			if (false)
			{
			}
#endif
			if (needsJsonParse)
			{
				logger.Debug("LoadDefinitions: Starting json deserialization");
				routineRunner.StartAsyncConditionTask(delegate
				{
					bool success = DeserializeDefinitionsFromJsonFile(jsonPath);
					if (success)
					{
						SaveJsonHash(jsonPath);
					}
					else
					{
						RemoveCachedDefinitions(jsonPath);
					}
					return success;
				}, OnDeserializationSuccess);
			}
		}
		logger.EventStop("LoadDefinitionsCommand.Execute");
	}

	private static string GetHashFilePath()
	{
		return global::System.IO.Path.Combine(global::Kampai.Util.GameConstants.PERSISTENT_DATA_PATH, "definitions_hash.txt");
	}

	private bool IsBinaryCacheValid(string jsonPath)
	{
#if !UNITY_WEBPLAYER
		try
		{
			string hashFilePath = GetHashFilePath();
			if (!global::System.IO.File.Exists(hashFilePath))
			{
				logger.Debug("LoadDefinitions: No hash file found, cache invalid");
				return false;
			}
			if (!global::System.IO.File.Exists(jsonPath))
			{
				logger.Debug("LoadDefinitions: JSON file not found at '{0}', assuming cache is valid", jsonPath);
				return true;
			}

			string storedHash = global::System.IO.File.ReadAllText(hashFilePath).Trim();
			string currentHash = ComputeFileHash(jsonPath);
			bool valid = string.Equals(storedHash, currentHash, global::System.StringComparison.OrdinalIgnoreCase);
			if (!valid)
			{
				logger.Debug("LoadDefinitions: Hash mismatch. Stored='{0}', Current='{1}'", storedHash, currentHash);
			}
			return valid;
		}
		catch (global::System.Exception ex)
		{
			logger.Warning("LoadDefinitions: Error checking binary cache validity: {0}", ex.Message);
			return false;
		}
#else
		return false;
#endif
	}

	private void SaveJsonHash(string jsonPath)
	{
#if !UNITY_WEBPLAYER
		try
		{
			if (!global::System.IO.File.Exists(jsonPath)) return;
			string hash = ComputeFileHash(jsonPath);
			global::System.IO.File.WriteAllText(GetHashFilePath(), hash);
			logger.Debug("LoadDefinitions: Saved JSON hash '{0}'", hash);
		}
		catch (global::System.Exception ex)
		{
			logger.Warning("LoadDefinitions: Failed to save JSON hash: {0}", ex.Message);
		}
#endif
	}

	private static string ComputeFileHash(string filePath)
	{
		using (var md5 = global::System.Security.Cryptography.MD5.Create())
		{
			using (var stream = global::System.IO.File.OpenRead(filePath))
			{
				byte[] hashBytes = md5.ComputeHash(stream);
				var sb = new global::System.Text.StringBuilder(32);
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("x2"));
				}
				return sb.ToString();
			}
		}
	}

	private void RemoveCachedDefinitions(string path)
	{
#if !UNITY_WEBPLAYER
		try
		{
			global::System.IO.File.Delete(path);
		}
		catch (global::System.Exception)
		{
		}
#endif
	}

	private void OnDeserializationSuccess()
	{
		global::Kampai.Util.StartupTimer.LogCheckpoint("LoadDefinitionsCommand.OnDeserializationSuccess (Definitions Loaded)");
		this.telemetryService.Send_Telemetry_EVT_USER_GAME_LOAD_FUNNEL("80 - Loaded Definitions", playerService.SWRVEGroup, dlcService.GetDownloadQualityLevel());
		logger.Debug("LoadDefinitions: Deserialized successfully");
		global::Kampai.Common.TelemetryService telemetryService = this.telemetryService as global::Kampai.Common.TelemetryService;
		if (telemetryService != null)
		{
			telemetryService.SetDefinitionServiceReference(definitionService);
		}
		definitionsChangedSignal.Dispatch(hotSwap);
		splashProgressUpdateSignal.Dispatch(35, 10f);
	}

	private bool DeserializeDefinitionsFromBinaryFile(string path)
	{
#if !UNITY_WEBPLAYER
		try
		{
			using (global::System.IO.BinaryReader binaryReader = new global::System.IO.BinaryReader(new global::System.IO.FileStream(path, global::System.IO.FileMode.Open, global::System.IO.FileAccess.Read)))
			{
				definitionService.DeserializeBinary(binaryReader);
			}
			return true;
		}
		catch (global::System.Exception ex)
		{
			logger.Error("DeserializeDefinitionsFromBinaryFile: can't deserialize from binary file. Reason: {0}", ex);
			return false;
		}
#else
		return false;
#endif
	}

	private bool DeserializeDefinitionsFromJsonString(string jsonString)
	{
		using (global::System.IO.StringReader textReader = new global::System.IO.StringReader(jsonString))
		{
			return DeserializeDefinitionsFromJson(textReader);
		}
	}

	private bool DeserializeDefinitionsFromJsonFile(string jsonPath)
	{
#if !UNITY_WEBPLAYER
		try
		{
			string jsonText = global::System.IO.File.ReadAllText(jsonPath);
			jsonText = jsonText.TrimEnd();
			if (!jsonText.EndsWith("}"))
			{
				logger.Warning("FORCED JSON LOADING: Automatically appending missing closing brace on Android.");
				jsonText += "}";
			}
			return DeserializeDefinitionsFromJsonString(jsonText);
		}
		catch (global::System.Exception e)
		{
			HandleDefinitionFileOpenError(e);
			return false;
		}
#else
		return false;
#endif
	}

	private bool HandleDefinitionFileOpenError(global::System.Exception e)
	{
		logger.Error("Definition file open error: {0}", e);
		int reasonCode = 0;
		if (e is global::System.IO.FileNotFoundException)
		{
			reasonCode = 1;
		}
		else if (e is global::System.IO.IOException)
		{
			reasonCode = 2;
		}
		invokerService.Add(delegate
		{
			logger.FatalNoThrow(global::Kampai.Util.FatalCode.DS_UNABLE_TO_LOAD, reasonCode, "Reason: {0}", e);
		});
		return false;
	}

	private bool DeserializeDefinitionsFromJson(global::System.IO.TextReader textReader)
	{
		try
		{
			definitionService.DeserializeJson(textReader);
			return true;
		}
		catch (global::Kampai.Util.FatalException ex)
		{
			global::Kampai.Util.FatalException ex2 = ex;
			global::Kampai.Util.FatalException e = ex2;
			global::UnityEngine.Debug.LogErrorFormat("LoadDefinitionsCommand: FatalException: {0}\n{1}", e.Message, e.StackTrace);
			logger.Error("Can't deserialize: {0}", e);
			invokerService.Add(delegate
			{
				logger.FatalNoThrow(e.FatalCode, e.ReferencedId, "Message: {0}, Reason: {1}", e.Message, e.InnerException ?? e);
			});
		}
		catch (global::System.Exception ex3)
		{
			global::System.Exception ex4 = ex3;
			global::System.Exception e2 = ex4;
			global::UnityEngine.Debug.LogErrorFormat("LoadDefinitionsCommand: System.Exception: {0}\n{1}", e2.Message, e2.StackTrace);
			logger.Error("Can't deserialize: {0}", e2);
			invokerService.Add(delegate
			{
				logger.FatalNoThrow(global::Kampai.Util.FatalCode.DS_PARSE_ERROR, 0, "Reason: {0}", e2);
			});
		}
		return false;
	}
}
