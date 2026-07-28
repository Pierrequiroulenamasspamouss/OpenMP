namespace Kampai.Game
{
	public class Definitions : global::Kampai.Util.IBinarySerializable, global::Kampai.Util.IFastJSONDeserializable
	{
		public global::System.Collections.Generic.IList<global::Kampai.Game.CompositeBuildingPieceDefinition> compositeBuildingPieceDefinitions;

		public virtual int TypeCode
		{
			get
			{
				return 1190;
			}
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.DefinitionGroup> definitionGroups { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.Transaction.TransactionDefinition> transactions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.Transaction.WeightedDefinition> weightedDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.BuildingDefinition> buildingDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PlotDefinition> plotDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.ItemDefinition> itemDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.MinionDefinition> minionDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.StoreItemDefinition> storeItemDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.CurrencyItemDefinition> currencyItemDefinitions { get; set; }

		public global::Kampai.Game.MarketplaceDefinition marketplaceDefinition { get; set; }

		public global::Kampai.Game.MinionPartyDefinition minionPartyDefinition { get; set; }

		public global::System.Collections.Generic.IList<string> environmentDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PurchasedLandExpansionDefinition> purchasedExpansionDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.LandExpansionDefinition> expansionDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.DebrisDefinition> debrisDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.AspirationalBuildingDefinition> aspirationalBuildingDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.LandExpansionConfig> expansionConfigs { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.CommonLandExpansionDefinition> commonExpansionDefinitions { get; set; }

		public global::Kampai.Game.GachaConfig gachaConfig { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.MinionAnimationDefinition> MinionAnimationDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.QuestDefinition> quests { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.QuestResourceDefinition> questResources { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.NotificationDefinition> notificationDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.RewardCollectionDefinition> collectionDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PrestigeDefinition> prestigeDefinitions { get; set; }

		public global::Kampai.Game.LevelUpDefinition levelUpDefinition { get; set; }

		public global::Kampai.Game.LevelFunTable levelFunTable { get; set; }

		public global::Kampai.Game.LevelXPTable levelXPTable { get; set; }

		public global::Kampai.Game.TaskDefinition tasks { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> timedSocialEventDefinitions { get; set; }

		public global::Kampai.Game.PlayerVersion player { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.RushTimeBandDefinition> rushDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.FootprintDefinition> footprintDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.QuestChainDefinition> questChains { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.NamedCharacterDefinition> namedCharacterDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.StickerDefinition> stickerDefinitions { get; set; }

		public global::Kampai.Game.DropLevelBandDefinition randomDropLevelBandDefinition { get; set; }

		public global::Kampai.Game.WayFinderDefinition wayFinderDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.FlyOverDefinition> flyOverDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Splash.LoadinTipBucketDefinition> loadInTipBucketDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Splash.LoadInTipDefinition> loadInTipDefinitions { get; set; }

		public global::Kampai.Game.CameraDefinition cameraSettingsDefinition { get; set; }

		public global::Kampai.Game.SocialSettingsDefinition socialSettingsDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.SalePackDefinition> salePackDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.CurrencyStorePackDefinition> currencyStorePackDefinitions { get; set; }

		public global::Kampai.Game.NotificationSystemDefinition notificationSystemDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PlayerTrainingDefinition> playerTrainingDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PlayerTrainingCardDefinition> playerTrainingCardDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PlayerTrainingCategoryDefinition> playerTrainingCategoryDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.AchievementDefinition> achievementDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.BuffDefinition> buffDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.CustomCameraPositionDefinition> customCameraPositionDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PartyFavorAnimationDefinition> partyFavorAnimationDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.GuestOfHonorDefinition> guestOfHonorDefinitions { get; set; }

		public global::Kampai.Game.CurrencyStoreDefinition currencyStoreDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.UIAnimationDefinition> uiAnimationDefinitions { get; set; }

		public global::Kampai.Game.MinionBenefitLevelBandDefintion minionBenefitDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PopulationBenefitDefinition> populationBenefitDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerDefinition> triggerDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerRewardDefinition> triggerRewardDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.HelpTipDefinition> helpTipDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.VillainLairDefinition> villainLairDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.MasterPlanDefinition> masterPlanDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.MasterPlanComponentDefinition> masterPlanComponentDefinitions { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.MasterPlanOnboardDefinition> onboardDefinitions { get; set; }

		public global::Kampai.Game.DynamicMasterPlanDefinition dynamicMasterPlanDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.PendingRewardDefinition> pendingRewardDefinitions { get; set; }

		public global::Kampai.Game.RewardedAdvertisementDefinition rewardedAdvertisementDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.LegalDocumentDefinition> legalDocumentDefinitions { get; set; }

		public global::Kampai.Game.PetsXPromoDefinition petsXPromoDefinition { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Main.HindsightCampaignDefinition> hindsightCampaignDefinitions { get; set; }

		public virtual void Write(global::System.IO.BinaryWriter writer)
		{
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, definitionGroups);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, transactions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, weightedDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, buildingDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, plotDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, itemDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, minionDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, storeItemDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, currencyItemDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, marketplaceDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, minionPartyDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, global::Kampai.Util.BinarySerializationUtil.WriteString, environmentDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, purchasedExpansionDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, expansionDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, debrisDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, aspirationalBuildingDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, expansionConfigs);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, commonExpansionDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteGachaConfig(writer, gachaConfig);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, MinionAnimationDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, quests);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, questResources);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, notificationDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, collectionDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, prestigeDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, levelUpDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, levelFunTable);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, levelXPTable);
			global::Kampai.Util.BinarySerializationUtil.WriteTaskDefinition(writer, tasks);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, timedSocialEventDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WritePlayerVersion(writer, player);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, rushDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, footprintDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, questChains);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, namedCharacterDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, stickerDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, randomDropLevelBandDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, wayFinderDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, flyOverDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, loadInTipBucketDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, loadInTipDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, cameraSettingsDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, socialSettingsDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, salePackDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, currencyStorePackDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, notificationSystemDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, playerTrainingDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, playerTrainingCardDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, playerTrainingCategoryDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, achievementDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, buffDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, customCameraPositionDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, partyFavorAnimationDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, guestOfHonorDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, currencyStoreDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, uiAnimationDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, minionBenefitDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, populationBenefitDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, triggerDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, triggerRewardDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, helpTipDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, villainLairDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, masterPlanDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, masterPlanComponentDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, onboardDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, dynamicMasterPlanDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, pendingRewardDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, rewardedAdvertisementDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, legalDocumentDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, petsXPromoDefinition);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, hindsightCampaignDefinitions);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, compositeBuildingPieceDefinitions);
		}

		public virtual void Read(global::System.IO.BinaryReader reader)
		{
			definitionGroups = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, definitionGroups);
			transactions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, transactions);
			weightedDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, weightedDefinitions);
			buildingDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, buildingDefinitions);
			plotDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, plotDefinitions);
			itemDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, itemDefinitions);
			minionDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, minionDefinitions);
			storeItemDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, storeItemDefinitions);
			currencyItemDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, currencyItemDefinitions);
			marketplaceDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.MarketplaceDefinition>(reader);
			minionPartyDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.MinionPartyDefinition>(reader);
			environmentDefinition = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, global::Kampai.Util.BinarySerializationUtil.ReadString, environmentDefinition);
			purchasedExpansionDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, purchasedExpansionDefinitions);
			expansionDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, expansionDefinitions);
			debrisDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, debrisDefinitions);
			aspirationalBuildingDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, aspirationalBuildingDefinitions);
			expansionConfigs = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, expansionConfigs);
			commonExpansionDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, commonExpansionDefinitions);
			gachaConfig = global::Kampai.Util.BinarySerializationUtil.ReadGachaConfig(reader);
			MinionAnimationDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, MinionAnimationDefinitions);
			quests = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, quests);
			questResources = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, questResources);
			notificationDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, notificationDefinitions);
			collectionDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, collectionDefinitions);
			prestigeDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, prestigeDefinitions);
			levelUpDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.LevelUpDefinition>(reader);
			levelFunTable = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.LevelFunTable>(reader);
			levelXPTable = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.LevelXPTable>(reader);
			tasks = global::Kampai.Util.BinarySerializationUtil.ReadTaskDefinition(reader);
			timedSocialEventDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, timedSocialEventDefinitions);
			player = global::Kampai.Util.BinarySerializationUtil.ReadPlayerVersion(reader);
			rushDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, rushDefinitions);
			footprintDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, footprintDefinitions);
			questChains = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, questChains);
			namedCharacterDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, namedCharacterDefinitions);
			stickerDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, stickerDefinitions);
			randomDropLevelBandDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.DropLevelBandDefinition>(reader);
			wayFinderDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.WayFinderDefinition>(reader);
			flyOverDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, flyOverDefinitions);
			loadInTipBucketDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, loadInTipBucketDefinitions);
			loadInTipDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, loadInTipDefinitions);
			cameraSettingsDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.CameraDefinition>(reader);
			socialSettingsDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.SocialSettingsDefinition>(reader);
			salePackDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, salePackDefinitions);
			currencyStorePackDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, currencyStorePackDefinitions);
			notificationSystemDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.NotificationSystemDefinition>(reader);
			playerTrainingDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, playerTrainingDefinitions);
			playerTrainingCardDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, playerTrainingCardDefinitions);
			playerTrainingCategoryDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, playerTrainingCategoryDefinitions);
			achievementDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, achievementDefinitions);
			buffDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, buffDefinitions);
			customCameraPositionDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, customCameraPositionDefinitions);
			partyFavorAnimationDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, partyFavorAnimationDefinitions);
			guestOfHonorDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, guestOfHonorDefinitions);
			currencyStoreDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.CurrencyStoreDefinition>(reader);
			uiAnimationDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, uiAnimationDefinitions);
			minionBenefitDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.MinionBenefitLevelBandDefintion>(reader);
			populationBenefitDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, populationBenefitDefinitions);
			triggerDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, triggerDefinitions);
			triggerRewardDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, triggerRewardDefinitions);
			helpTipDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, helpTipDefinitions);
			villainLairDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, villainLairDefinitions);
			masterPlanDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, masterPlanDefinitions);
			masterPlanComponentDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, masterPlanComponentDefinitions);
			onboardDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, onboardDefinitions);
			dynamicMasterPlanDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.DynamicMasterPlanDefinition>(reader);
			pendingRewardDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, pendingRewardDefinitions);
			rewardedAdvertisementDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.RewardedAdvertisementDefinition>(reader);
			legalDocumentDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, legalDocumentDefinitions);
			petsXPromoDefinition = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.PetsXPromoDefinition>(reader);
			hindsightCampaignDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, hindsightCampaignDefinitions);
			compositeBuildingPieceDefinitions = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, compositeBuildingPieceDefinitions);
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
			case "DEFINITIONGROUPS":
				reader.Read();
				definitionGroups = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, definitionGroups);
				break;
			case "TRANSACTIONS":
				reader.Read();
				transactions = ((converters.transactionDefinitionConverter == null) ? global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, transactions) : global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.transactionDefinitionConverter, transactions));
				break;
			case "WEIGHTEDDEFINITIONS":
				reader.Read();
				weightedDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, weightedDefinitions);
				break;
			case "BUILDINGDEFINITIONS":
				reader.Read();
				buildingDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.buildingDefinitionConverter, buildingDefinitions);
				break;
			case "PLOTDEFINITIONS":
				reader.Read();
				plotDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.plotDefinitionConverter, plotDefinitions);
				break;
			case "ITEMDEFINITIONS":
				reader.Read();
				itemDefinitions = ((converters.itemDefinitionConverter == null) ? global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, itemDefinitions) : global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.itemDefinitionConverter, itemDefinitions));
				break;
			case "MINIONDEFINITIONS":
				reader.Read();
				minionDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, minionDefinitions);
				break;
			case "STOREITEMDEFINITIONS":
				reader.Read();
				storeItemDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, storeItemDefinitions);
				break;
			case "CURRENCYITEMDEFINITIONS":
				reader.Read();
				currencyItemDefinitions = ((converters.currencyItemDefinitionConverter == null) ? global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, currencyItemDefinitions) : global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.currencyItemDefinitionConverter, currencyItemDefinitions));
				break;
			case "MARKETPLACEDEFINITION":
				reader.Read();
				marketplaceDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.MarketplaceDefinition>(reader, converters);
				break;
			case "MINIONPARTYDEFINITION":
				reader.Read();
				minionPartyDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.MinionPartyDefinition>(reader, converters);
				break;
			case "ENVIRONMENTDEFINITION":
				reader.Read();
				environmentDefinition = global::Kampai.Util.ReaderUtil.PopulateList<string>(reader, converters, global::Kampai.Util.ReaderUtil.ReadString, environmentDefinition);
				break;
			case "PURCHASEDEXPANSIONDEFINITIONS":
				reader.Read();
				purchasedExpansionDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, purchasedExpansionDefinitions);
				break;
			case "EXPANSIONDEFINITIONS":
				reader.Read();
				expansionDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, expansionDefinitions);
				break;
			case "DEBRISDEFINITIONS":
				reader.Read();
				debrisDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, debrisDefinitions);
				break;
			case "ASPIRATIONALBUILDINGDEFINITIONS":
				reader.Read();
				aspirationalBuildingDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, aspirationalBuildingDefinitions);
				break;
			case "EXPANSIONCONFIGS":
				reader.Read();
				expansionConfigs = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, expansionConfigs);
				break;
			case "COMMONEXPANSIONDEFINITIONS":
				reader.Read();
				commonExpansionDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, commonExpansionDefinitions);
				break;
			case "GACHACONFIG":
				reader.Read();
				gachaConfig = global::Kampai.Util.ReaderUtil.ReadGachaConfig(reader, converters);
				break;
			case "MINIONANIMATIONDEFINITIONS":
				reader.Read();
				MinionAnimationDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, MinionAnimationDefinitions);
				break;
			case "QUESTS":
				reader.Read();
				quests = ((converters.questDefinitionConverter == null) ? global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, quests) : global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.questDefinitionConverter, quests));
				break;
			case "QUESTRESOURCES":
				reader.Read();
				questResources = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, questResources);
				break;
			case "NOTIFICATIONDEFINITIONS":
				reader.Read();
				notificationDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, notificationDefinitions);
				break;
			case "COLLECTIONDEFINITIONS":
				reader.Read();
				collectionDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, collectionDefinitions);
				break;
			case "PRESTIGEDEFINITIONS":
				reader.Read();
				prestigeDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, prestigeDefinitions);
				break;
			case "LEVELUPDEFINITION":
				reader.Read();
				levelUpDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.LevelUpDefinition>(reader, converters);
				break;
			case "LEVELFUNTABLE":
				reader.Read();
				levelFunTable = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.LevelFunTable>(reader, converters);
				break;
			case "LEVELXPTABLE":
				reader.Read();
				levelXPTable = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.LevelXPTable>(reader, converters);
				break;
			case "TASKS":
				reader.Read();
				tasks = global::Kampai.Util.ReaderUtil.ReadTaskDefinition(reader, converters);
				break;
			case "TIMEDSOCIALEVENTDEFINITIONS":
				reader.Read();
				timedSocialEventDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, timedSocialEventDefinitions);
				break;
			case "PLAYER":
				reader.Read();
				player = ((converters.playerVersionConverter == null) ? global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.PlayerVersion>(reader, converters) : converters.playerVersionConverter.ReadJson(reader, converters));
				break;
			case "RUSHDEFINITIONS":
				reader.Read();
				rushDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, rushDefinitions);
				break;
			case "FOOTPRINTDEFINITIONS":
				reader.Read();
				footprintDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, footprintDefinitions);
				break;
			case "QUESTCHAINS":
				reader.Read();
				questChains = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, questChains);
				break;
			case "NAMEDCHARACTERDEFINITIONS":
				reader.Read();
				namedCharacterDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.namedCharacterDefinitionConverter, namedCharacterDefinitions);
				break;
			case "STICKERDEFINITIONS":
				reader.Read();
				stickerDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, stickerDefinitions);
				break;
			case "RANDOMDROPLEVELBANDDEFINITION":
				reader.Read();
				randomDropLevelBandDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.DropLevelBandDefinition>(reader, converters);
				break;
			case "WAYFINDERDEFINITION":
				reader.Read();
				wayFinderDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.WayFinderDefinition>(reader, converters);
				break;
			case "FLYOVERDEFINITIONS":
				reader.Read();
				flyOverDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, flyOverDefinitions);
				break;
			case "LOADINTIPBUCKETDEFINITIONS":
				reader.Read();
				loadInTipBucketDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, loadInTipBucketDefinitions);
				break;
			case "LOADINTIPDEFINITIONS":
				reader.Read();
				loadInTipDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, loadInTipDefinitions);
				break;
			case "CAMERASETTINGSDEFINITION":
				reader.Read();
				cameraSettingsDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.CameraDefinition>(reader, converters);
				break;
			case "SOCIALSETTINGSDEFINITION":
				reader.Read();
				socialSettingsDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.SocialSettingsDefinition>(reader, converters);
				break;
			case "SALEPACKDEFINITIONS":
				reader.Read();
				salePackDefinitions = ((converters.salePackDefinitionConverter == null) ? global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, salePackDefinitions) : global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.salePackDefinitionConverter, salePackDefinitions));
				break;
			case "CURRENCYSTOREPACKDEFINITIONS":
				reader.Read();
				currencyStorePackDefinitions = ((converters.currencyStorePackDefinitionConverter == null) ? global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, currencyStorePackDefinitions) : global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.currencyStorePackDefinitionConverter, currencyStorePackDefinitions));
				break;
			case "NOTIFICATIONSYSTEMDEFINITION":
				reader.Read();
				notificationSystemDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.NotificationSystemDefinition>(reader, converters);
				break;
			case "PLAYERTRAININGDEFINITIONS":
				reader.Read();
				playerTrainingDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, playerTrainingDefinitions);
				break;
			case "PLAYERTRAININGCARDDEFINITIONS":
				reader.Read();
				playerTrainingCardDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, playerTrainingCardDefinitions);
				break;
			case "PLAYERTRAININGCATEGORYDEFINITIONS":
				reader.Read();
				playerTrainingCategoryDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, playerTrainingCategoryDefinitions);
				break;
			case "ACHIEVEMENTDEFINITIONS":
				reader.Read();
				achievementDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, achievementDefinitions);
				break;
			case "BUFFDEFINITIONS":
				reader.Read();
				buffDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, buffDefinitions);
				break;
			case "CUSTOMCAMERAPOSITIONDEFINITIONS":
				reader.Read();
				customCameraPositionDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, customCameraPositionDefinitions);
				break;
			case "PARTYFAVORANIMATIONDEFINITIONS":
				reader.Read();
				partyFavorAnimationDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, partyFavorAnimationDefinitions);
				break;
			case "GUESTOFHONORDEFINITIONS":
				reader.Read();
				guestOfHonorDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, guestOfHonorDefinitions);
				break;
			case "CURRENCYSTOREDEFINITION":
				reader.Read();
				currencyStoreDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.CurrencyStoreDefinition>(reader, converters);
				break;
			case "UIANIMATIONDEFINITIONS":
				reader.Read();
				uiAnimationDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, uiAnimationDefinitions);
				break;
			case "MINIONBENEFITDEFINITION":
				reader.Read();
				minionBenefitDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.MinionBenefitLevelBandDefintion>(reader, converters);
				break;
			case "POPULATIONBENEFITDEFINITIONS":
				reader.Read();
				populationBenefitDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, populationBenefitDefinitions);
				break;
			case "TRIGGERDEFINITIONS":
				reader.Read();
				triggerDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.triggerDefinitionConverter, triggerDefinitions);
				break;
			case "TRIGGERREWARDDEFINITIONS":
				reader.Read();
				triggerRewardDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.triggerRewardDefinitionConverter, triggerRewardDefinitions);
				break;
			case "HELPTIPDEFINITIONS":
				reader.Read();
				helpTipDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, helpTipDefinitions);
				break;
			case "VILLAINLAIRDEFINITIONS":
				reader.Read();
				villainLairDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, villainLairDefinitions);
				break;
			case "MASTERPLANDEFINITIONS":
				reader.Read();
				masterPlanDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, masterPlanDefinitions);
				break;
			case "MASTERPLANCOMPONENTDEFINITIONS":
				reader.Read();
				masterPlanComponentDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, masterPlanComponentDefinitions);
				break;
			case "ONBOARDDEFINITIONS":
				reader.Read();
				onboardDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, onboardDefinitions);
				break;
			case "DYNAMICMASTERPLANDEFINITION":
				reader.Read();
				dynamicMasterPlanDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.DynamicMasterPlanDefinition>(reader, converters);
				break;
			case "PENDINGREWARDDEFINITIONS":
				reader.Read();
				pendingRewardDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, pendingRewardDefinitions);
				break;
			case "REWARDEDADVERTISEMENTDEFINITION":
				reader.Read();
				rewardedAdvertisementDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.RewardedAdvertisementDefinition>(reader, converters);
				break;
			case "LEGALDOCUMENTDEFINITIONS":
				reader.Read();
				legalDocumentDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, legalDocumentDefinitions);
				break;
			case "PETSXPROMODEFINITION":
				reader.Read();
				petsXPromoDefinition = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.PetsXPromoDefinition>(reader, converters);
				break;
			case "HINDSIGHTCAMPAIGNDEFINITIONS":
				reader.Read();
				hindsightCampaignDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, hindsightCampaignDefinitions);
				break;
			case "COMPOSITEBUILDINGPIECEDEFINITIONS":
				reader.Read();
				compositeBuildingPieceDefinitions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, compositeBuildingPieceDefinitions);
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
