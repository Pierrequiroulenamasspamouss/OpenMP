namespace Kampai.Game
{
	public class AdPlacementDefinitionFastConverter : global::Kampai.Util.FastJsonCreationConverter<global::Kampai.Game.AdPlacementDefinition>
	{
		private global::Kampai.Game.RewardedAdType rewardType;

		public override global::Kampai.Game.AdPlacementDefinition ReadJson(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
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
				rewardType = (global::Kampai.Game.RewardedAdType)(int)global::System.Enum.Parse(typeof(global::Kampai.Game.RewardedAdType), value);
			}
			reader = jObject.CreateReader();
			return base.ReadJson(reader, converters);
		}

		public override global::Kampai.Game.AdPlacementDefinition Create()
		{
			switch (rewardType)
			{
			case global::Kampai.Game.RewardedAdType.OnTheGlassDailyReward:
				return new global::Kampai.Game.OnTheGlassDailyRewardDefinition();
			case global::Kampai.Game.RewardedAdType.CraftingRushReward:
				return new global::Kampai.Game.CraftingRushRewardDefinition();
			case global::Kampai.Game.RewardedAdType.MarketplaceRefreshRushReward:
				return new global::Kampai.Game.MarketplaceRefreshRushRewardDefinition();
			case global::Kampai.Game.RewardedAdType.MissingResourcesReward:
				return new global::Kampai.Game.MissingResourcesRewardDefinition();
			case global::Kampai.Game.RewardedAdType.Offerwall:
				return new global::Kampai.Game.OfferwallPlacementDefinition();
			case global::Kampai.Game.RewardedAdType.OrderboardFillOrderReward:
				return new global::Kampai.Game.OrderboardFillOrderRewardDefinition();
			case global::Kampai.Game.RewardedAdType.Quest2xReward:
				return new global::Kampai.Game.Quest2xRewardDefinition();
			default:
				return new global::Kampai.Game.AdPlacementDefinition();
			}
		}
	}
}
