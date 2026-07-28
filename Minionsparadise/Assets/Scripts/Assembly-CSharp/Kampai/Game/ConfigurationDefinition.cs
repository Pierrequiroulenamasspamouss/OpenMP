namespace Kampai.Game
{
	public class ConfigurationDefinition : global::Kampai.Util.IFastJSONDeserializable
	{
		public enum RateAppAfterEvent
		{
			UnknownEvent = 0,
			LevelUp = 1,
			VillainCutscene = 2,
			XPPayout = 3
		}

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public bool serverPushNotifications { get; set; }

		public bool AprilsFool { get; set; }

		public bool promoPetsEnabled { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public float minimumVersion { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		[global::Kampai.Util.Deserializer("ReaderUtil.ReadRateAppTriggerConfig")]
		public global::System.Collections.Generic.Dictionary<global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent, bool> rateAppAfter { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public bool? night { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int maxRPS { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public global::System.Collections.Generic.List<global::Kampai.Game.KillSwitch> killSwitches { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int msHeartbeat { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int fpsHeartbeat { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int logLevel { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int healthMetricPercentage { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int nudgeUpgradePercentage { get; set; }

		[global::Kampai.Util.Deserializer("ReaderUtil.ReadDictionaryString")]
		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public global::System.Collections.Generic.Dictionary<string, string> dlcManifests { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public global::System.Collections.Generic.Dictionary<string, global::Kampai.Game.FeatureAccess> featureAccess { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public global::Kampai.Util.TargetPerformance targetPerformance { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public string definitionId { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public string definitions { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public bool isAllowed { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public bool isNudgeAllowed { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public string videoUri { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int httpRequestTimeout { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int httpRequestReadWriteTimeout { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int autoSaveIntervalUnlinkedAccount { get; set; }

		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public int autoSaveIntervalLinkedAccount { get; set; }

		[global::Kampai.Util.Deserializer("ReaderUtil.ReadNestedDictionary")]
		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public global::System.Collections.Generic.Dictionary<string, object> loggingConfig { get; set; }

		public virtual object Deserialize(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			global::Kampai.Util.ReaderUtil.EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					string propertyName = ((string)reader.Value).ToUpper();
					if (!DeserializeProperty(propertyName, reader, converters))
					{
						reader.Skip();
					}
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					return this;
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return this;
		}

		protected virtual bool DeserializeProperty(string propertyName, global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			switch (propertyName)
			{
			case "SERVERPUSHNOTIFICATIONS":
				reader.Read();
				serverPushNotifications = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "APRILSFOOL":
				reader.Read();
				AprilsFool = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "PROMOPETSENABLED":
				reader.Read();
				promoPetsEnabled = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "MINIMUMVERSION":
				reader.Read();
				minimumVersion = global::System.Convert.ToSingle(reader.Value);
				break;
			case "RATEAPPAFTER":
				reader.Read();
				rateAppAfter = global::Kampai.Util.ReaderUtil.ReadRateAppTriggerConfig(reader, converters);
				break;
			case "NIGHT":
				reader.Read();
				night = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "MAXRPS":
				reader.Read();
				maxRPS = global::System.Convert.ToInt32(reader.Value);
				break;
			case "KILLSWITCHES":
				reader.Read();
				killSwitches = global::Kampai.Util.ReaderUtil.PopulateList<global::Kampai.Game.KillSwitch>(reader, converters, global::Kampai.Util.ReaderUtil.ReadKillSwitch, killSwitches);
				break;
			case "MSHEARTBEAT":
				reader.Read();
				msHeartbeat = global::System.Convert.ToInt32(reader.Value);
				break;
			case "FPSHEARTBEAT":
				reader.Read();
				fpsHeartbeat = global::System.Convert.ToInt32(reader.Value);
				break;
			case "LOGLEVEL":
				reader.Read();
				logLevel = global::System.Convert.ToInt32(reader.Value);
				break;
			case "HEALTHMETRICPERCENTAGE":
				reader.Read();
				healthMetricPercentage = global::System.Convert.ToInt32(reader.Value);
				break;
			case "NUDGEUPGRADEPERCENTAGE":
				reader.Read();
				nudgeUpgradePercentage = global::System.Convert.ToInt32(reader.Value);
				break;
			case "DLCMANIFESTS":
				reader.Read();
				dlcManifests = global::Kampai.Util.ReaderUtil.ReadDictionaryString(reader, converters);
				break;
			case "FEATUREACCESS":
				reader.Read();
				featureAccess = global::Kampai.Util.ReaderUtil.ReadDictionary<global::Kampai.Game.FeatureAccess>(reader, converters);
				break;
			case "TARGETPERFORMANCE":
				reader.Read();
				targetPerformance = global::Kampai.Util.ReaderUtil.ReadEnum<global::Kampai.Util.TargetPerformance>(reader);
				break;
			case "DEFINITIONID":
				reader.Read();
				definitionId = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "DEFINITIONS":
				reader.Read();
				definitions = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "ISALLOWED":
				reader.Read();
				isAllowed = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "ISNUDGEALLOWED":
				reader.Read();
				isNudgeAllowed = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "VIDEOURI":
				reader.Read();
				videoUri = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "HTTPREQUESTTIMEOUT":
				reader.Read();
				httpRequestTimeout = global::System.Convert.ToInt32(reader.Value);
				break;
			case "HTTPREQUESTREADWRITETIMEOUT":
				reader.Read();
				httpRequestReadWriteTimeout = global::System.Convert.ToInt32(reader.Value);
				break;
			case "AUTOSAVEINTERVALUNLINKEDACCOUNT":
				reader.Read();
				autoSaveIntervalUnlinkedAccount = global::System.Convert.ToInt32(reader.Value);
				break;
			case "AUTOSAVEINTERVALLINKEDACCOUNT":
				reader.Read();
				autoSaveIntervalLinkedAccount = global::System.Convert.ToInt32(reader.Value);
				break;
			case "LOGGINGCONFIG":
				reader.Read();
				loggingConfig = global::Kampai.Util.ReaderUtil.ReadNestedDictionary(reader, converters);
				break;
			default:
				return false;
			}
			return true;
		}

		public virtual object DeserializeOverride(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			global::Kampai.Util.ReaderUtil.EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					string propertyName = ((string)reader.Value).ToUpper();
					if (!DeserializePropertyOverride(propertyName, reader, converters))
					{
						reader.Skip();
					}
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					return this;
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return this;
		}

		protected virtual bool DeserializePropertyOverride(string propertyName, global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			switch (propertyName)
			{
			case "SERVERPUSHNOTIFICATIONS":
				reader.Read();
				serverPushNotifications = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "APRILSFOOL":
				reader.Read();
								AprilsFool = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "PROMOPETSENABLED":
				reader.Read();
				promoPetsEnabled = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "MINIMUMVERSION":
				reader.Read();
				minimumVersion = global::System.Convert.ToSingle(reader.Value);
				break;
			case "RATEAPPAFTER":
				reader.Read();
				rateAppAfter = global::Kampai.Util.ReaderUtil.ReadRateAppTriggerConfig(reader, converters);
				break;
			case "NIGHT":
				reader.Read();
				night = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "MAXRPS":
				reader.Read();
				maxRPS = global::System.Convert.ToInt32(reader.Value);
				break;
			case "KILLSWITCHES":
				reader.Read();
				killSwitches = global::Kampai.Util.ReaderUtil.PopulateList<global::Kampai.Game.KillSwitch>(reader, converters, global::Kampai.Util.ReaderUtil.ReadKillSwitch);
				break;
			case "MSHEARTBEAT":
				reader.Read();
				msHeartbeat = global::System.Convert.ToInt32(reader.Value);
				break;
			case "FPSHEARTBEAT":
				reader.Read();
				fpsHeartbeat = global::System.Convert.ToInt32(reader.Value);
				break;
			case "LOGLEVEL":
				reader.Read();
				logLevel = global::System.Convert.ToInt32(reader.Value);
				break;
			case "HEALTHMETRICPERCENTAGE":
				reader.Read();
				healthMetricPercentage = global::System.Convert.ToInt32(reader.Value);
				break;
			case "NUDGEUPGRADEPERCENTAGE":
				reader.Read();
				nudgeUpgradePercentage = global::System.Convert.ToInt32(reader.Value);
				break;
			case "DLCMANIFESTS":
				reader.Read();
				dlcManifests = global::Kampai.Util.ReaderUtil.ReadDictionaryString(reader, converters);
				break;
			case "FEATUREACCESS":
				reader.Read();
				featureAccess = global::Kampai.Util.ReaderUtil.ReadDictionary<global::Kampai.Game.FeatureAccess>(reader, converters);
				break;
			case "TARGETPERFORMANCE":
				reader.Read();
				targetPerformance = global::Kampai.Util.ReaderUtil.ReadEnum<global::Kampai.Util.TargetPerformance>(reader);
				break;
			case "DEFINITIONID":
				reader.Read();
				definitionId = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "DEFINITIONS":
				reader.Read();
				definitions = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "ISALLOWED":
				reader.Read();
				isAllowed = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "ISNUDGEALLOWED":
				reader.Read();
				isNudgeAllowed = global::System.Convert.ToBoolean(reader.Value);
				break;
			case "VIDEOURI":
				reader.Read();
				videoUri = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "HTTPREQUESTTIMEOUT":
				reader.Read();
				httpRequestTimeout = global::System.Convert.ToInt32(reader.Value);
				break;
			case "HTTPREQUESTREADWRITETIMEOUT":
				reader.Read();
				httpRequestReadWriteTimeout = global::System.Convert.ToInt32(reader.Value);
				break;
			case "AUTOSAVEINTERVALUNLINKEDACCOUNT":
				reader.Read();
				autoSaveIntervalUnlinkedAccount = global::System.Convert.ToInt32(reader.Value);
				break;
			case "AUTOSAVEINTERVALLINKEDACCOUNT":
				reader.Read();
				autoSaveIntervalLinkedAccount = global::System.Convert.ToInt32(reader.Value);
				break;
			case "LOGGINGCONFIG":
				reader.Read();
				loggingConfig = global::Kampai.Util.ReaderUtil.ReadNestedDictionary(reader, converters);
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
