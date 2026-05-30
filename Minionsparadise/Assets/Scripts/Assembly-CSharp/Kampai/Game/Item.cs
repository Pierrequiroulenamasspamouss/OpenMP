namespace Kampai.Game
{
	public class Item : global::Kampai.Game.Instance<global::Kampai.Game.ItemDefinition>
	{
		private uint quantity;

		public uint Quantity
		{
			get
			{
				return quantity;
			}
			set
			{
				quantity = value;
				if (Definition != null && Definition.ID == 1 && quantity > 999999)
				{
					quantity = 999999;
				}
			}
		}

		public Item(global::Kampai.Game.ItemDefinition def)
			: base(def)
		{
		}

		protected override bool DeserializeProperty(string propertyName, global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			switch (propertyName)
			{
			case "QUANTITY":
				reader.Read();
				Quantity = global::System.Convert.ToUInt32(reader.Value);
				return true;
			default:
				return base.DeserializeProperty(propertyName, reader, converters);
			}
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
			writer.WritePropertyName("Quantity");
			writer.WriteValue(Quantity);
		}
	}
}
