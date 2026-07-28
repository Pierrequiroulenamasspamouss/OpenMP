namespace Kampai.Util
{
	public class QuantityItem : global::System.IEquatable<global::Kampai.Util.QuantityItem>, global::Kampai.Util.IBinarySerializable, global::Kampai.Util.IFastJSONDeserializable, global::Kampai.Util.Identifiable
	{
		public virtual int TypeCode
		{
			get
			{
				return 1008;
			}
		}

		public int ID { get; set; }

		public uint Quantity { get; set; }

		public QuantityItem()
		{
		}

		public QuantityItem(int id)
		{
			ID = id;
		}

		public QuantityItem(int id, uint quantity)
		{
			ID = id;
			Quantity = quantity;
		}

		public virtual void Write(global::System.IO.BinaryWriter writer)
		{
			writer.Write(ID);
			writer.Write(Quantity);
		}

		public virtual void Read(global::System.IO.BinaryReader reader)
		{
			ID = reader.ReadInt32();
			Quantity = reader.ReadUInt32();
		}

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
			case "QUANTITY":
				reader.Read();
				Quantity = global::System.Convert.ToUInt32(reader.Value);
				break;
			default:
				return false;
			case "ID":
				reader.Read();
				ID = global::System.Convert.ToInt32(reader.Value);
				break;
			}
			return true;
		}

		public static global::Kampai.Util.QuantityItem Build(global::System.Collections.Generic.IDictionary<string, object> src, global::Kampai.Util.QuantityItem qi = null)
		{
			if (src != null)
			{
				if (qi == null)
				{
					qi = new global::Kampai.Util.QuantityItem();
				}
				qi.ID = global::System.Convert.ToInt32(src["id"]);
				qi.Quantity = global::System.Convert.ToUInt32(src["quantity"]);
				return qi;
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("ID: {0}, Quantity: {1}", ID, Quantity);
		}

		public string ToString(global::Kampai.Game.IDefinitionService definitionService)
		{
			global::Kampai.Game.Definition definition;
			return (definitionService.TryGet<global::Kampai.Game.Definition>(ID, out definition) && !string.IsNullOrEmpty(definition.LocalizedKey)) ? string.Format("{0}: {1}", definition.LocalizedKey, Quantity) : ToString();
		}

		public override bool Equals(object other)
		{
			return Equals(other as global::Kampai.Util.QuantityItem);
		}

		public bool Equals(global::Kampai.Util.QuantityItem other)
		{
			return other != null && ID == other.ID && Quantity == other.Quantity;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + ID.GetHashCode();
			return num * 23 + Quantity.GetHashCode();
		}
	}
}
