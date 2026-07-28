namespace Kampai.Game
{
	public abstract class Definition : global::Kampai.Util.IBinarySerializable, global::Kampai.Util.IFastJSONDeserializable, global::Kampai.Util.Identifiable
	{
		public virtual int TypeCode
		{
			get
			{
				return 1000;
			}
		}

		public string LocalizedKey { get; set; }

		public virtual int ID { get; set; }

		public bool Disabled { get; set; }

		public virtual void Write(global::System.IO.BinaryWriter writer)
		{
			global::Kampai.Util.BinarySerializationUtil.WriteString(writer, LocalizedKey);
			writer.Write(ID);
			writer.Write(Disabled);
		}

		public virtual void Read(global::System.IO.BinaryReader reader)
		{
			LocalizedKey = global::Kampai.Util.BinarySerializationUtil.ReadString(reader);
			ID = reader.ReadInt32();
			Disabled = reader.ReadBoolean();
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
			case "LOCALIZEDKEY":
				reader.Read();
				LocalizedKey = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "ID":
				reader.Read();
				ID = global::System.Convert.ToInt32(reader.Value);
				break;
			case "DISABLED":
				reader.Read();
				Disabled = global::System.Convert.ToBoolean(reader.Value);
				break;
			default:
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return string.Format("Defintion TYPE={0} ID={1} KEY={2}", GetType().Name, ID, LocalizedKey);
		}
	}
}
