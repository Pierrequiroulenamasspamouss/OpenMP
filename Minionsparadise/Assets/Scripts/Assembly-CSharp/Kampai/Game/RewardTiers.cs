namespace Kampai.Game
{
	public class RewardTiers : global::Kampai.Util.IFastJSONDeserializable
	{
		public global::Kampai.Game.OnTheGlassDailyRewardTier Tier1 { get; set; }

		public global::Kampai.Game.OnTheGlassDailyRewardTier Tier2 { get; set; }

		public global::Kampai.Game.OnTheGlassDailyRewardTier Tier3 { get; set; }

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
			case "TIER1":
				reader.Read();
				Tier1 = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.OnTheGlassDailyRewardTier>(reader, converters);
				break;
			case "TIER2":
				reader.Read();
				Tier2 = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.OnTheGlassDailyRewardTier>(reader, converters);
				break;
			case "TIER3":
				reader.Read();
				Tier3 = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.OnTheGlassDailyRewardTier>(reader, converters);
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
