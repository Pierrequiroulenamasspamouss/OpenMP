namespace Kampai.Game
{
	[global::Kampai.Util.RequiresJsonConverter]
	public class PlayerVersion : global::Kampai.Util.IFastJSONDeserializable
	{
		private const int currentVersion = 15;

		private global::Kampai.Game.IPlayerSerializer CurrentSerializer = new global::Kampai.Game.PlayerSerializerV15();

		public int Version { get; set; }

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
			case "VERSION":
				reader.Read();
				Version = global::System.Convert.ToInt32(reader.Value);
				return true;
			default:
				return false;
			}
		}

		public global::Kampai.Game.Player CreatePlayer(string json, global::Kampai.Game.IDefinitionService definitionService, ILocalPersistanceService localPersistanceService, global::Kampai.Game.IPartyService partyService, global::Kampai.Util.IKampaiLogger logger)
		{
			global::Kampai.Game.Player player = CurrentSerializer.Deserialize(json, definitionService, localPersistanceService, partyService, logger);
			if (player.Version != 15)
			{
				player.Version = 15;
			}
			return player;
		}

		public byte[] Serialize(global::Kampai.Game.Player player, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Util.IKampaiLogger logger)
		{
			return CurrentSerializer.Serialize(player, definitionService, logger);
		}
	}
}
