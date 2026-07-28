namespace Kampai.Game
{
	public class ConfigurationDefinitionsDictionary : global::Kampai.Util.IFastJSONDeserializable
	{
		[global::Newtonsoft.Json.JsonProperty(NullValueHandling = global::Newtonsoft.Json.NullValueHandling.Ignore)]
		public global::System.Collections.Generic.Dictionary<string, global::Kampai.Game.ConfigurationDefinition> allConfigs { get; set; }

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
			case "ALLCONFIGS":
				reader.Read();
				allConfigs = global::Kampai.Util.ReaderUtil.ReadDictionary<global::Kampai.Game.ConfigurationDefinition>(reader, converters);
				return true;
			default:
				return false;
			}
		}
	}
}
