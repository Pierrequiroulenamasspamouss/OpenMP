namespace Kampai.Game
{
	public class SpecialEventItem : global::Kampai.Game.Item
	{
		public bool HasEnded { get; set; }

		public SpecialEventItem(global::Kampai.Game.SpecialEventItemDefinition def)
			: base(def)
		{
		}

		protected override bool DeserializeProperty(string propertyName, global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (string.Equals(propertyName, "HasEnded", global::System.StringComparison.OrdinalIgnoreCase))
			{
				if (reader.TokenType == global::Newtonsoft.Json.JsonToken.PropertyName)
				{
					reader.Read();
				}
				HasEnded = global::System.Convert.ToBoolean(reader.Value);
				return true;
			}
			return base.DeserializeProperty(propertyName, reader, converters);
		}

		public override void Serialize(global::Newtonsoft.Json.JsonWriter writer)
		{
			writer.WriteStartObject();
			SerializeProperties(writer);
			writer.WriteEndObject();
		}

		protected override void SerializeProperties(global::Newtonsoft.Json.JsonWriter writer)
		{
			base.SerializeProperties(writer);
			writer.WritePropertyName("HasEnded");
			writer.WriteValue(HasEnded);
		}
	}
}
