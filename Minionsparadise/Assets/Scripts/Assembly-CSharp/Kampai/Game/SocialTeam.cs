namespace Kampai.Game
{
	[global::Kampai.Util.RequiresJsonConverter]
	public class SocialTeam : global::Kampai.Util.IFastJSONDeserializable
	{
		public long ID { get; set; }

		public int SocialEventId { get; set; }

		public global::Kampai.Game.TimedSocialEventDefinition Definition { get; protected set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.UserIdentity> Members { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.SocialOrderProgress> OrderProgress { get; set; }

		public SocialTeam(global::Kampai.Game.TimedSocialEventDefinition def)
		{
			Definition = def;
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
			case "ID":
				reader.Read();
				ID = global::System.Convert.ToInt64(reader.Value);
				break;
			case "SOCIALEVENTID":
				reader.Read();
				SocialEventId = global::System.Convert.ToInt32(reader.Value);
				break;
			case "MEMBERS":
				reader.Read();
				Members = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadUserIdentity, Members);
				break;
			case "ORDERPROGRESS":
				reader.Read();
				OrderProgress = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadSocialOrderProgress, OrderProgress);
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
