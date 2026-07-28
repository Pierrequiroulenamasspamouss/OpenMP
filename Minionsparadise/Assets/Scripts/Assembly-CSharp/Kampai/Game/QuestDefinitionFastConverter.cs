namespace Kampai.Game
{
	public class QuestDefinitionFastConverter : global::Kampai.Util.FastJsonCreationConverter<global::Kampai.Game.QuestDefinition>
	{
		private global::Kampai.Game.QuestType questType;

		public override global::Kampai.Game.QuestDefinition ReadJson(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			questType = global::Kampai.Game.QuestType.Default;
			if (reader.TokenType != global::Newtonsoft.Json.JsonToken.Null)
			{
				global::Newtonsoft.Json.Linq.JObject jObject = global::Newtonsoft.Json.Linq.JObject.Load(reader);
				global::Newtonsoft.Json.Linq.JProperty jProperty = jObject.Property("type");
				if (jProperty != null)
				{
					string value = jProperty.Value.ToString();
					questType = (global::Kampai.Game.QuestType)(int)global::System.Enum.Parse(typeof(global::Kampai.Game.QuestType), value);
				}
				else if (jObject.Property("serverStartTimeUTC") != null && jObject.Property("serverStopTimeUTC") != null)
				{
					questType = global::Kampai.Game.QuestType.LimitedQuest;
				}
				reader = jObject.CreateReader();
			}
			return base.ReadJson(reader, converters);
		}

		public override global::Kampai.Game.QuestDefinition Create()
		{
			switch (questType)
			{
			case global::Kampai.Game.QuestType.Default:
				return new global::Kampai.Game.QuestDefinition();
			case global::Kampai.Game.QuestType.TimedQuest:
				return new global::Kampai.Game.TimedQuestDefinition();
			case global::Kampai.Game.QuestType.LimitedQuest:
				return new global::Kampai.Game.LimitedQuestDefinition();
			case global::Kampai.Game.QuestType.DynamicQuest:
				return new global::Kampai.Game.DynamicQuestDefinition();
			default:
				return new global::Kampai.Game.QuestDefinition();
			}
		}
	}
}
