namespace Kampai.Game
{
	public class NamedCharacterDefinitionFastConverter : global::Kampai.Util.FastJsonCreationConverter<global::Kampai.Game.NamedCharacterDefinition>
	{
		private global::Kampai.Game.NamedCharacterType type;

		public override global::Kampai.Game.NamedCharacterDefinition ReadJson(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::Newtonsoft.Json.Linq.JObject jObject = global::Newtonsoft.Json.Linq.JObject.Load(reader);
			global::Newtonsoft.Json.Linq.JProperty jProperty = jObject.Property("type");
			if (jProperty != null)
			{
				string value = jProperty.Value.ToString();
				type = (global::Kampai.Game.NamedCharacterType)(int)global::System.Enum.Parse(typeof(global::Kampai.Game.NamedCharacterType), value);
			}
			reader = jObject.CreateReader();
			return base.ReadJson(reader, converters);
		}

		public override global::Kampai.Game.NamedCharacterDefinition Create()
		{
			switch (type)
			{
			case global::Kampai.Game.NamedCharacterType.BOB:
				return new global::Kampai.Game.BobCharacterDefinition();
			case global::Kampai.Game.NamedCharacterType.VILLAIN:
				return new global::Kampai.Game.VillainDefinition();
			case global::Kampai.Game.NamedCharacterType.PHIL:
				return new global::Kampai.Game.PhilCharacterDefinition();
			case global::Kampai.Game.NamedCharacterType.STUART:
				return new global::Kampai.Game.StuartCharacterDefinition();
			case global::Kampai.Game.NamedCharacterType.KEVIN:
				return new global::Kampai.Game.KevinCharacterDefinition();
			case global::Kampai.Game.NamedCharacterType.TSM:
				return new global::Kampai.Game.TSMCharacterDefinition();
			case global::Kampai.Game.NamedCharacterType.SPECIAL_EVENT:
				return new global::Kampai.Game.SpecialEventCharacterDefinition();
			default:
				return new global::Kampai.Game.BobCharacterDefinition();
			}
		}
	}
}
