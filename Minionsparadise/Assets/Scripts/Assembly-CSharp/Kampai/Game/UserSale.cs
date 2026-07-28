namespace Kampai.Game
{
	public class UserSale : global::Kampai.Util.IFastJSONDeserializable
	{
		public long SaleId { get; set; }

		public string SaleDefinition { get; set; }

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
			case "SALEDEFINITION":
				reader.Read();
				SaleDefinition = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "SALEID":
				reader.Read();
				SaleId = global::System.Convert.ToInt64(reader.Value);
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
