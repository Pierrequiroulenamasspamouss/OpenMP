namespace Kampai.Game
{
	public class ConfigurationsService : global::Kampai.Game.IConfigurationsService
	{
		private enum ConfigLoadingState
		{
			WAIT_DEFAULT_CONFIG = 0,
			WAIT_DEFAULT_CONFIG_OVERRIDE_KNOWN = 1,
			WAIT_OVERRIDEN_CONFIG = 2,
			OVERRIDEN_CONFIG_LOADED = 3
		}

		private const string DEFAULT_DEVICE_CONFIG_NAME = "anyDeviceType";

		private const string KILLSWITCH_PP_OVERRIDE_LEAD = "KS-";

		private global::Kampai.Game.ConfigurationDefinition config;

		private bool init = true;

		private int tries;

		private int tryCap = 5;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("ConfigurationsService") as global::Kampai.Util.IKampaiLogger;

		[Inject("game.server.environment")]
		public string ServerEnv { get; set; }

		[Inject]
		public global::Kampai.Game.ConfigurationsLoadedSignal configurationsLoadedSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IClientVersion clientVersion { get; set; }

		[Inject]
		public global::Kampai.Util.FPSUtil fpsUtil { get; set; }

		[Inject]
		public global::Kampai.Game.KillSwitchChangedSignal killSwitchChangedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public global::Kampai.Game.LoadConfigurationSignal loadConfigurationSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowOfflinePopupSignal showOfflinePopupSignal { get; set; }

		[Inject]
		public IResourceService resourceService { get; set; }

		private global::System.Collections.Generic.List<global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.KillSwitch, bool>> killswitchOverrides { get; set; }

		public global::Kampai.Game.ConfigurationDefinition GetConfigurations()
		{
			return config;
		}

		public int GetTries()
		{
			return tries;
		}

		public int GetTryCap()
		{
			return tryCap;
		}

		public void setInitonCallback(bool init)
		{
			this.init = init;
		}

		public void GetConfigurationCallback(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			global::Kampai.Util.TimeProfiler.EndSection("retrieve config");
			if (response.Success)
			{
				logger.Info("ConfigurationsService.GetConfigurationCallback: Attempting to deserialize configuration definition...");
				string body = response.Body;
				
				// Cache config locally for Offline Mode fallback
				global::Kampai.Util.OfflineModeUtility.SaveLocal(global::Kampai.Util.OfflineModeUtility.ConfigCachePath, body);

				global::Kampai.Game.ConfigurationDefinition configurationDefinition = null;
				try
				{
					global::Kampai.Util.TimeProfiler.StartSection("read config");
					configurationDefinition = LoadConfig(body);
				}
				catch (global::Newtonsoft.Json.JsonSerializationException e)
				{
					configurationDefinition = TryAgainForConfigAfterException(e);
				}
				catch (global::Newtonsoft.Json.JsonReaderException e2)
				{
					configurationDefinition = TryAgainForConfigAfterException(e2);
				}
				finally
				{
					global::Kampai.Util.TimeProfiler.EndSection("read config");
				}
				if (configurationDefinition != null)
				{
					tries = 0;
					if (global::Kampai.Util.GameConstants.StaticConfig.DEBUG_ENABLED)
					{
						TryLoadKillswitchOverrides();
					}
					logger.Info("ConfigurationDefinition is not null, carry on...");
					if (config == null)
					{
						config = new global::Kampai.Game.ConfigurationDefinition();
					}
					if (config.killSwitches != configurationDefinition.killSwitches)
					{
						config = configurationDefinition;
						killSwitchChangedSignal.Dispatch();
					}
					config = configurationDefinition;
					logger.SetAllowedLevel(configurationDefinition.logLevel);
					fpsUtil.SetFpsHeartbeat(configurationDefinition.fpsHeartbeat);
					global::Kampai.Util.HttpRequestConfig.SetConfig(config);
					configurationsLoadedSignal.Dispatch(init);
				}
			}
			else if (!response.Request.IsAborted())
			{
				if (userSessionService != null && userSessionService.IsOffline)
				{
					logger.Warning("Network error (code {0}) on config request while in offline mode. Ignoring.", response.Code);
					return;
				}
				if (!IgnoreError())
				{
					showOfflinePopupSignal.Dispatch(true);
					return;
				}
				logger.Warning("Network error (code {0}) on non initial configuration request.", response.Code);
			}
		}

		private global::Kampai.Game.ConfigurationDefinition LoadConfig(string json)
		{
			string strippedJson = json.Trim();
			// Check if it already has the wrapper, ignoring whitespace/newlines
			bool hasWrapper = strippedJson.StartsWith("{\"allConfigs\"") || 
							 strippedJson.StartsWith("{\n\"allConfigs\"") || 
							 strippedJson.StartsWith("{\r\n\"allConfigs\"") ||
							 strippedJson.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").StartsWith("{\"allConfigs\"");
			
			if (!hasWrapper)
			{
				json = "{\"allConfigs\": " + json + "}";
			}
			global::Kampai.Util.KampaiStringReader kampaiStringReader = new global::Kampai.Util.KampaiStringReader(json);
			global::Newtonsoft.Json.JsonTextReader jsonTextReader = new global::Newtonsoft.Json.JsonTextReader(kampaiStringReader);
			MoveReadingPositionToFirstConfig(jsonTextReader);
			string deviceType = GetDeviceType();
			global::Kampai.Game.ConfigurationDefinition configurationDefinition = null;
			global::Kampai.Game.ConfigurationsService.ConfigLoadingState configLoadingState = global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_DEFAULT_CONFIG;
			int position = -1;
			while (jsonTextReader.Read())
			{
				switch (jsonTextReader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					string text = (string)jsonTextReader.Value;
					switch (text)
					{
					case "anyDeviceType":
						jsonTextReader.Read();
						configurationDefinition = new global::Kampai.Game.ConfigurationDefinition();
						configurationDefinition.Deserialize(jsonTextReader);
						switch (configLoadingState)
						{
						case global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_DEFAULT_CONFIG:
							configLoadingState = global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_OVERRIDEN_CONFIG;
							break;
						case global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_DEFAULT_CONFIG_OVERRIDE_KNOWN:
						{
							kampaiStringReader.SetPosition(position);
							global::Newtonsoft.Json.JsonTextReader reader = new global::Newtonsoft.Json.JsonTextReader(kampaiStringReader);
							configurationDefinition.DeserializeOverride(reader);
							configLoadingState = global::Kampai.Game.ConfigurationsService.ConfigLoadingState.OVERRIDEN_CONFIG_LOADED;
							logger.Debug("LoadConfig: config loaded. Config is special for device {0}", deviceType);
							return configurationDefinition;
						}
						default:
							throw new global::Newtonsoft.Json.JsonSerializationException(string.Format("Unexpected config loading state {0} for device: {1}. Json reader state: {2}. {3}", configLoadingState, deviceType, jsonTextReader.TokenType, global::Kampai.Util.ReaderUtil.GetPositionInSource(jsonTextReader)));
						}
						break;
					default:
						if (text == deviceType)
						{
							switch (configLoadingState)
							{
							case global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_OVERRIDEN_CONFIG:
								jsonTextReader.Read();
								configurationDefinition.Deserialize(jsonTextReader);
								configLoadingState = global::Kampai.Game.ConfigurationsService.ConfigLoadingState.OVERRIDEN_CONFIG_LOADED;
								logger.Debug("LoadConfig: config loaded. Config is special for device {0}", deviceType);
								return configurationDefinition;
							case global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_DEFAULT_CONFIG:
								position = kampaiStringReader.GetPosition();
								jsonTextReader.Skip();
								configLoadingState = global::Kampai.Game.ConfigurationsService.ConfigLoadingState.WAIT_DEFAULT_CONFIG_OVERRIDE_KNOWN;
								break;
							default:
								throw new global::Newtonsoft.Json.JsonSerializationException(string.Format("Unexpected config loading state {0} for device: {1}. Json reader state: {2}. {3}", configLoadingState, deviceType, jsonTextReader.TokenType, global::Kampai.Util.ReaderUtil.GetPositionInSource(jsonTextReader)));
							}
						}
						else
						{
							jsonTextReader.Skip();
						}
						break;
					}
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					logger.Debug("LoadConfig: default config loaded.");
					return configurationDefinition ?? new global::Kampai.Game.ConfigurationDefinition();
				default:
					throw new global::Newtonsoft.Json.JsonSerializationException(string.Format("Unexpected token when deserializing object: {0}. {1}", jsonTextReader.TokenType, global::Kampai.Util.ReaderUtil.GetPositionInSource(jsonTextReader)));
				case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			throw new global::Newtonsoft.Json.JsonSerializationException("Unexpected end when deserializing object.");
		}

		private void MoveReadingPositionToFirstConfig(global::Newtonsoft.Json.JsonReader reader)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			global::Kampai.Util.ReaderUtil.EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			reader.Read();
			if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName || !"allConfigs".Equals(reader.Value))
			{
				throw new global::Newtonsoft.Json.JsonSerializationException("unexpected config format. First field allConfig is missing.");
			}
			reader.Read();
			global::Kampai.Util.ReaderUtil.EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
		}

		private bool ValidConfigurationExists()
		{
			return config != null;
		}

		private bool IgnoreError()
		{
			return !init && ValidConfigurationExists();
		}

		private global::Kampai.Game.ConfigurationDefinition TryAgainForConfigAfterException(global::System.Exception e)
		{
			if (!init && ValidConfigurationExists())
			{
				logger.Warning("Json error in ConfigurationService on non initial request: using the old configuration.");
				return config;
			}
			if (tries < tryCap)
			{
				logger.Info("Json serialization error in ConfigurationService: # of tries for a new service: ", tries);
				tries++;
				loadConfigurationSignal.Dispatch(init);
			}
			else
			{
				if (config != null)
				{
					logger.Error("Json serialization error in ConfigurationService: using the old configuration.");
					return config;
				}
				logger.Error("Error: {0}", e.Message);
				logger.Fatal(global::Kampai.Util.FatalCode.CONFIG_JSON_FAIL);
			}
			return null;
		}

		public string GetConfigURL()
		{
			string configVariant = GetConfigVariant();
			string clientPlatform = clientVersion.GetClientPlatform();
			string text = clientVersion.GetClientVersion();
			return string.Format(global::Kampai.Util.GameConstants.Server.CDN_URL + "/configs/{0}/{1}/{2}/{3}/config", ServerEnv.ToLower(), text, clientPlatform, configVariant);
		}

		public string GetDeviceType()
		{
			return global::Kampai.Util.Native.GetDeviceHardwareModel();
		}

		public string GetConfigVariant()
		{
			return (!global::Kampai.Util.ABTestModel.abtestEnabled) ? "anyVariant" : global::Kampai.Util.ABTestModel.configurationVariant;
		}

		public string GetDefinitionVariants()
		{
			if (global::Kampai.Util.ABTestModel.abtestEnabled && global::Kampai.Util.ABTestModel.definitionURL != null)
			{
				return global::Kampai.Util.ABTestModel.definitionVariants;
			}
			return string.Empty;
		}

		public void TryLoadKillswitchOverrides()
		{
			string empty = string.Empty;
			foreach (int value in global::System.Enum.GetValues(typeof(global::Kampai.Game.KillSwitch)))
			{
				empty = global::UnityEngine.PlayerPrefs.GetString("KS-" + (global::Kampai.Game.KillSwitch)value, null);
				if (!string.IsNullOrEmpty(empty))
				{
					OverrideKillswitch((global::Kampai.Game.KillSwitch)value, global::System.Convert.ToBoolean(empty));
				}
			}
		}

		public bool isKillSwitchOn(global::Kampai.Game.KillSwitch killswitchType)
		{
			if (userSessionService.IsOffline)
			{
				// Disable online features when offline
				if (killswitchType == global::Kampai.Game.KillSwitch.MARKETPLACESERVER ||
					killswitchType == global::Kampai.Game.KillSwitch.FACEBOOK ||
					killswitchType == global::Kampai.Game.KillSwitch.SYNERGY ||
					killswitchType == global::Kampai.Game.KillSwitch.SWRVE ||
					killswitchType == global::Kampai.Game.KillSwitch.LOGGLY)
				{
					return true;
				}
			}

			if (killswitchOverrides != null)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.KillSwitch, bool> killswitchOverride in killswitchOverrides)
				{
					if (killswitchOverride.Key == killswitchType)
					{
						return killswitchOverride.Value;
					}
				}
			}
			return (config.killSwitches != null && config.killSwitches.Contains(killswitchType)) ? true : false;
		}

		public void OverrideKillswitch(global::Kampai.Game.KillSwitch killswitchType, bool killswitchValue)
		{
			if (killswitchOverrides == null)
			{
				killswitchOverrides = new global::System.Collections.Generic.List<global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.KillSwitch, bool>>();
			}
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.KillSwitch, bool> killswitchOverride in killswitchOverrides)
			{
				if (killswitchOverride.Key == killswitchType)
				{
					killswitchOverrides.Remove(killswitchOverride);
					break;
				}
			}
			killswitchOverrides.Add(new global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.KillSwitch, bool>(killswitchType, killswitchValue));
			global::UnityEngine.PlayerPrefs.SetString("KS-" + killswitchType, killswitchValue.ToString());
		}

		public void ClearKillswitchOverride(global::Kampai.Game.KillSwitch killswitchType)
		{
			if (killswitchOverrides == null)
			{
				return;
			}
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.KillSwitch, bool> killswitchOverride in killswitchOverrides)
			{
				if (killswitchOverride.Key == killswitchType)
				{
					killswitchOverrides.Remove(killswitchOverride);
					global::UnityEngine.PlayerPrefs.DeleteKey("KS-" + killswitchType);
					break;
				}
			}
		}

		public void ClearAllKillswitchOverrides()
		{
			if (killswitchOverrides != null)
			{
				for (int num = killswitchOverrides.Count - 1; num >= 0; num--)
				{
					global::UnityEngine.PlayerPrefs.DeleteKey("KS-" + killswitchOverrides[num].Key);
					killswitchOverrides.RemoveAt(num);
				}
			}
		}

		public void LoadLocalConfiguration()
		{
			string json = global::Kampai.Util.OfflineModeUtility.LoadLocal(global::Kampai.Util.OfflineModeUtility.ConfigCachePath);
			if (string.IsNullOrEmpty(json))
			{
				logger.Info("[OfflineMode] No cached config found at {0}, trying built-in resource...", global::Kampai.Util.OfflineModeUtility.ConfigCachePath);
				// Try to load from Resources if cached is missing
				json = resourceService.LoadText("config_server");
				if (string.IsNullOrEmpty(json)) json = resourceService.LoadText("config_server.json");
				if (string.IsNullOrEmpty(json)) json = resourceService.LoadText("config_server_local.json");
				
				if (string.IsNullOrEmpty(json))
				{
					string rawPath = global::System.IO.Path.Combine(global::UnityEngine.Application.dataPath, "Resources/config_server.json");
					if (global::System.IO.File.Exists(rawPath)) json = global::System.IO.File.ReadAllText(rawPath);
				}
				
				if (!string.IsNullOrEmpty(json))
				{
					logger.Info("[OfflineMode] Successfully loaded config from resources, caching to {0}", global::Kampai.Util.OfflineModeUtility.ConfigCachePath);
					global::Kampai.Util.OfflineModeUtility.SaveLocal(global::Kampai.Util.OfflineModeUtility.ConfigCachePath, json);
				}
			}

			if (!string.IsNullOrEmpty(json))
			{
				logger.Info("[OfflineMode] Loading configurations from local source...");
				global::Kampai.Game.ConfigurationDefinition configurationDefinition = LoadConfig(json);
				if (configurationDefinition != null)
				{
					config = configurationDefinition;
					configurationsLoadedSignal.Dispatch(init);
				}
			}
			else
			{
				logger.Error("[OfflineMode] Failed to load any configuration source!");
				showOfflinePopupSignal.Dispatch(true);
			}
		}
	}
}
