namespace Kampai.Game
{
	public class OnTheGlassDailyRewardTier : global::Kampai.Util.IFastJSONDeserializable
	{
		public int Weight { get; set; }

		public global::Kampai.Game.Transaction.WeightedDefinition PredefinedRewards { get; set; }

		public int CraftableRewardMinTier { get; set; }

		public int CraftableRewardMaxQuantity { get; set; }

		public int CraftableRewardWeight { get; set; }

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
			case "WEIGHT":
				reader.Read();
				Weight = global::System.Convert.ToInt32(reader.Value);
				break;
			case "PREDEFINEDREWARDS":
				reader.Read();
				PredefinedRewards = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.Transaction.WeightedDefinition>(reader, converters);
				break;
			case "CRAFTABLEREWARDMINTIER":
				reader.Read();
				CraftableRewardMinTier = global::System.Convert.ToInt32(reader.Value);
				break;
			case "CRAFTABLEREWARDMAXQUANTITY":
				reader.Read();
				CraftableRewardMaxQuantity = global::System.Convert.ToInt32(reader.Value);
				break;
			case "CRAFTABLEREWARDWEIGHT":
				reader.Read();
				CraftableRewardWeight = global::System.Convert.ToInt32(reader.Value);
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
