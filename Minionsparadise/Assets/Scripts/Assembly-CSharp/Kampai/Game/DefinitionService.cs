namespace Kampai.Game
{
	public class DefinitionService : global::Kampai.Game.IDefinitionService
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DefinitionService") as global::Kampai.Util.IKampaiLogger;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Definition> AllDefinitions;

		protected global::System.Collections.Generic.IList<string> environmentDefinition;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.WeightedDefinition> gachaDefinitionsByNumMinions;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.WeightedDefinition> partyDefinitionsByNumMinions;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Trigger.TriggerRewardDefinition> triggerRewardsDefinitions;

		protected global::System.Collections.Generic.Dictionary<int, int> levelUnlockLookUpTable;

		protected global::Kampai.Game.TaskDefinition taskDefinition;

		protected global::System.Collections.Generic.IList<global::Kampai.Game.RushTimeBandDefinition> rushDefinitions;

		protected global::System.Collections.Generic.IList<global::Kampai.Game.AchievementDefinition> achievementDefinitions;

		protected global::System.Collections.Generic.IList<global::Kampai.Game.LandExpansionConfig> expansionConfigs;

		protected global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerDefinition> triggerDefinitions;

		protected global::System.Collections.Generic.IList<global::Kampai.Game.CurrencyStoreCategoryDefinition> currencyStoreCategoryDefinitions;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.TransactionDefinition> packTransactionMap;

		protected global::Kampai.Util.TargetPerformance targetPerformance = global::Kampai.Util.TargetPerformance.HIGH;

		protected global::System.Collections.Generic.Dictionary<int, int> itemTransactionTable;

		protected string initialPlayer;

		protected bool validateDefinitions = true;

		protected string binaryDefinitionsPath;

		private global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Collections.IList> definitionsTypeMap;

		public DefinitionService()
		{
			AllDefinitions = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Definition>();
			binaryDefinitionsPath = GetBinaryDefinitionsPath();
		}

		public void DeserializeJson(global::System.IO.TextReader textReader, bool validateDefinitions = true)
		{
			if (textReader != null)
			{
				global::System.Diagnostics.Stopwatch stopwatch = global::System.Diagnostics.Stopwatch.StartNew();
				global::Kampai.Game.Definitions definitions = null;
				try
				{
					using (global::Newtonsoft.Json.JsonTextReader reader = new global::Newtonsoft.Json.JsonTextReader(textReader))
					{
						// IMPORTANT: definitions.json uses a custom deserialization logic.
						// We must use a JsonConverters object, not just an array.
						JsonConverters converters = GetJsonDefinitionsFastConverters();
						definitions = new global::Kampai.Game.Definitions();
						definitions.Deserialize(reader, converters);
					}
				}
				catch (global::System.Exception ex)
				{
					global::UnityEngine.Debug.LogErrorFormat("DefinitionService: Exception during JSON deserialization: {0}\n{1}", ex.Message, ex.StackTrace);
					logger.Error("DefinitionService: Exception during JSON deserialization: {0}", ex);
					logger.Error("DefinitionService: JSON parse error ignored."); return;
				}

				if (definitions == null)
				{
					logger.Error("DefinitionService: DeserializeJson produced NULL definitions!");
					return;
				}

				LoadDefinitions(definitions, validateDefinitions, "DefinitionService.DeserializeJson()");
				
				if (validateDefinitions)
				{
					SerializeBinary(definitions);
				}
				stopwatch.Stop();
			}
			else
			{
				logger.Error("DefinitionService.Deserialize(): empty json"); return;
			}
		}

		public JsonConverters GetJsonDefinitionsFastConverters()
		{
			JsonConverters jsonConverters = new JsonConverters();
			jsonConverters.buildingDefinitionConverter = new global::Kampai.Game.BuildingDefinitionFastConverter();
			jsonConverters.questDefinitionConverter = new global::Kampai.Game.QuestDefinitionFastConverter();
			jsonConverters.itemDefinitionConverter = new global::Kampai.Game.ItemDefinitionFastConverter();
			jsonConverters.currencyItemDefinitionConverter = new global::Kampai.Game.CurrencyItemFastConverter();
			jsonConverters.playerVersionConverter = new global::Kampai.Game.PlayerDefinitionFastConverter(this);
			jsonConverters.plotDefinitionConverter = new global::Kampai.Game.PlotDefinitionFastConverter(logger);
			jsonConverters.namedCharacterDefinitionConverter = new global::Kampai.Game.NamedCharacterDefinitionFastConverter();
			jsonConverters.salePackDefinitionConverter = new global::Kampai.Game.SalePackConverter();
			jsonConverters.currencyStorePackDefinitionConverter = new global::Kampai.Game.CurrencyStorePackConverter();
			jsonConverters.triggerDefinitionConverter = new global::Kampai.Game.Trigger.TriggerDefinitionFastConverter(logger);
			jsonConverters.triggerConditionDefinitionConverter = new global::Kampai.Game.Trigger.TriggerConditionDefinitionFastConverter(logger);
			jsonConverters.triggerRewardDefinitionConverter = new global::Kampai.Game.Trigger.TriggerRewardDefinitionFastConverter(logger);
			jsonConverters.adPlacementDefinitionConverter = new global::Kampai.Game.AdPlacementDefinitionFastConverter();
			return jsonConverters;
		}

		public global::Newtonsoft.Json.JsonConverter[] GetJsonDefinitionsConverters()
		{
			return new global::Newtonsoft.Json.JsonConverter[11]
			{
				new global::Kampai.Game.BuildingDefinitionConverter(),
				new global::Kampai.Game.QuestDefinitionConverter(),
				new global::Kampai.Game.ItemDefinitionConverter(),
				new global::Kampai.Game.CurrencyItemConverter(),
				new global::Kampai.Game.PlayerDefinitionConverter(this),
				new global::Kampai.Game.PlotDefinitionConverter(logger),
				new global::Kampai.Game.NamedCharacterDefinitionConverter(),
				new global::Kampai.Game.Trigger.TriggerDefinitionConverter(logger),
				new global::Kampai.Game.Trigger.TriggerConditionDefinitionConverter(logger),
				new global::Kampai.Game.Trigger.TriggerRewardDefinitionConverter(logger),
				new global::Kampai.Game.AdPlacementDefinitionConverter()
			};
		}

		public static string GetBinaryDefinitionsPath()
		{
			return global::System.IO.Path.Combine(global::Kampai.Util.GameConstants.PERSISTENT_DATA_PATH, "definitions.dat");
		}

		public static void DeleteBinarySerialization()
		{
			string path = GetBinaryDefinitionsPath();
#if !UNITY_WEBPLAYER
			if (global::System.IO.File.Exists(path))
			{
				global::System.IO.File.Delete(path);
			}
#endif
		}

		private void SerializeBinary(global::Kampai.Game.Definitions definitions)
		{
			try
			{
#if !UNITY_WEBPLAYER
				using (global::System.IO.BinaryWriter writer = new global::System.IO.BinaryWriter(global::System.IO.File.OpenWrite(binaryDefinitionsPath)))
				{
					global::Kampai.Util.BinarySerializationUtil.WriteString(writer, initialPlayer);
					global::Kampai.Util.BinarySerializationUtil.WriteObject(writer, definitions);
				}
#endif
			}
			catch (global::System.Exception ex)
			{
				logger.Error("DL: DefinitionService.SerializeBinary: unable to serizalize definitions to binary file {0}. Reason: {1}", binaryDefinitionsPath, ex);
				DeleteBinarySerialization();
			}
		}

		public void DeserializeBinary(global::System.IO.BinaryReader binaryReader, bool validateDefinitions = false)
		{
			global::Kampai.Game.Definitions definitions = null;
			initialPlayer = global::Kampai.Util.BinarySerializationUtil.ReadString(binaryReader);
			definitions = global::Kampai.Util.BinarySerializationUtil.ReadObject<global::Kampai.Game.Definitions>(binaryReader);
			LoadDefinitions(definitions, validateDefinitions, "DeserializeBinary");
		}

		private void LoadDefinitions(global::Kampai.Game.Definitions definitions, bool validateDefinitions, string callerTag)
		{
			if (definitions == null)
			{
				logger.Error("LoadDefinitions(): definitions are null, caller: {0}", callerTag); return;
			}
			this.validateDefinitions = validateDefinitions;
			AllDefinitions = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Definition>(2500);
			definitionsTypeMap = new global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Collections.IList>();
			levelUnlockLookUpTable = null;
			itemTransactionTable = null;
			
			MarkDefinitions(definitions);
			MarkMoreDefinitions(definitions);
			MarkMarketplaceDefinitions(definitions);
			MarkMinionPartyDefinitions(definitions);
			MarkNamedDefinitions(definitions.namedCharacterDefinitions);
			MarkSalePackTransactionDefinitions(definitions.salePackDefinitions);
			MarkRewardedAdvertisementDefinition(definitions.rewardedAdvertisementDefinition);
			MarkCurrencyStorePackDefinitions(definitions.currencyStorePackDefinitions);
			MarkMasterPlanDefinitions(definitions);
			MarkXPromoDefinitions(definitions);
			MarkHindsightDefinitions(definitions);
			
			AssembleGachaAnimationDefinitions();
			AddLevelUpDefinition(definitions);
			AddMinionBenefitDefinition(definitions);
			AddNotificationSystemDefinition(definitions);
			AddLevelXPTable(definitions);
			AddLevelFunTable(definitions);
			AddDropLevelBandDefinition(definitions);
			
			taskDefinition = GetTaskDefintion(definitions);
			rushDefinitions = GetRushDefinitions(definitions);
			achievementDefinitions = GetAchievementDefinitions(definitions);
			triggerRewardsDefinitions = GetTriggerRewardDefinitions(definitions);
			triggerDefinitions = GetTriggerDefinitions(definitions);
			currencyStoreCategoryDefinitions = GetCurrencyStoreCategoryDefinitions(definitions);
			
			if (AllDefinitions.Count == 0) {
			}
		}

		public void DeserializeEnvironmentDefinition(global::System.IO.TextReader textReader)
		{
			global::Kampai.Game.Definitions definitions = null;
			using (global::Newtonsoft.Json.JsonTextReader reader = new global::Newtonsoft.Json.JsonTextReader(textReader))
			{
				definitions = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.Definitions>(reader);
			}
			environmentDefinition = definitions.environmentDefinition;
		}

		private void MarkDefinitions(global::Kampai.Game.Definitions definitions)
		{
			MarkDefinitionsAsUsed(definitions.weightedDefinitions);
			MarkDefinitionsAsUsed(definitions.transactions);
			MarkDefinitionsAsUsed(definitions.buildingDefinitions);
			MarkDefinitionsAsUsed(definitions.plotDefinitions);
			MarkDefinitionsAsUsed(definitions.itemDefinitions);
			MarkDefinitionsAsUsed(definitions.minionDefinitions);
			MarkDefinitionsAsUsed(definitions.currencyItemDefinitions);
			MarkDefinitionsAsUsed(definitions.storeItemDefinitions);
			MarkDefinitionsAsUsed(definitions.purchasedExpansionDefinitions);
			MarkDefinitionsAsUsed(definitions.commonExpansionDefinitions);
			MarkDefinitionsAsUsed(definitions.expansionDefinitions);
			MarkDefinitionsAsUsed(definitions.debrisDefinitions);
			MarkDefinitionsAsUsed(definitions.aspirationalBuildingDefinitions);
			MarkDefinitionsAsUsed(definitions.footprintDefinitions);
			MarkDefinitionsAsUsed(definitions.playerTrainingDefinitions);
			MarkDefinitionsAsUsed(definitions.playerTrainingCardDefinitions);
			MarkDefinitionsAsUsed(definitions.playerTrainingCategoryDefinitions);
			MarkDefinitionsAsUsed(definitions.buffDefinitions);
			MarkDefinitionsAsUsed(definitions.guestOfHonorDefinitions);
			MarkDefinitionsAsUsed(definitions.customCameraPositionDefinitions);
			MarkDefinitionsAsUsed(definitions.helpTipDefinitions);
			MarkDefinitionsAsUsed(definitions.populationBenefitDefinitions);
			MarkDefinitionsAsUsed(definitions.villainLairDefinitions);
		}

		private void MarkMoreDefinitions(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.gachaConfig != null) {
				MarkDefinitionsAsUsed(definitions.gachaConfig.GatchaAnimationDefinitions);
				MarkDefinitionsAsUsed(definitions.gachaConfig.DistributionTables);
			}
			MarkDefinitionsAsUsed(definitions.expansionConfigs);
			MarkDefinitionsAsUsed(definitions.MinionAnimationDefinitions);
			MarkDefinitionsAsUsed(definitions.quests);
			MarkDefinitionsAsUsed(definitions.questResources);
			MarkDefinitionsAsUsed(definitions.notificationDefinitions);
			MarkDefinitionsAsUsed(definitions.collectionDefinitions);
			MarkDefinitionsAsUsed(definitions.prestigeDefinitions);
			MarkDefinitionsAsUsed(definitions.definitionGroups);
			MarkDefinitionsAsUsed(definitions.timedSocialEventDefinitions);
			MarkDefinitionsAsUsed(definitions.compositeBuildingPieceDefinitions);
			MarkDefinitionsAsUsed(definitions.questChains);
			MarkDefinitionsAsUsed(definitions.stickerDefinitions);
			MarkDefinitionsAsUsed(definitions.achievementDefinitions);
			MarkDefinitionAsUsed(definitions.wayFinderDefinition);
			MarkDefinitionsAsUsed(definitions.flyOverDefinitions);
			MarkDefinitionsAsUsed(definitions.loadInTipBucketDefinitions);
			MarkDefinitionsAsUsed(definitions.loadInTipDefinitions);
			MarkDefinitionAsUsed(definitions.cameraSettingsDefinition);
			MarkDefinitionAsUsed(definitions.socialSettingsDefinition);
			MarkDefinitionsAsUsed(definitions.uiAnimationDefinitions);
			MarkDefinitionsAsUsed(definitions.pendingRewardDefinitions);
			MarkDefinitionsAsUsed(definitions.legalDocumentDefinitions);
		}

		private void MarkMarketplaceDefinitions(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.marketplaceDefinition == null)
			{
				logger.Error("MarkMarketplaceDefinitions: marketplaceDefinition is NULL! Marketplace will not function.");
				return;
			}
			MarkDefinitionAsUsed(definitions.marketplaceDefinition);
			MarkDefinitionsAsUsed(definitions.marketplaceDefinition.itemDefinitions);
			MarkDefinitionsAsUsed(definitions.marketplaceDefinition.slotDefinitions);
			MarkDefinitionAsUsed(definitions.marketplaceDefinition.refreshTimerDefinition);
		}

		private void MarkMinionPartyDefinitions(global::Kampai.Game.Definitions definitions)
		{
			MarkDefinitionAsUsed(definitions.minionPartyDefinition);
			MarkDefinitionsAsUsed(definitions.partyFavorAnimationDefinitions);
		}

		private void MarkMasterPlanDefinitions(global::Kampai.Game.Definitions definitions)
		{
			MarkDefinitionsAsUsed(definitions.masterPlanDefinitions);
			MarkDefinitionsAsUsed(definitions.masterPlanComponentDefinitions);
			MarkDefinitionsAsUsed(definitions.onboardDefinitions);
			MarkDefinitionAsUsed(definitions.dynamicMasterPlanDefinition);
		}

		private void MarkXPromoDefinitions(global::Kampai.Game.Definitions definitions)
		{
			MarkDefinitionAsUsed(definitions.petsXPromoDefinition);
		}

		private void MarkHindsightDefinitions(global::Kampai.Game.Definitions definitions)
		{
			MarkDefinitionsAsUsed(definitions.hindsightCampaignDefinitions);
		}

		private void MarkNamedDefinitions(global::System.Collections.Generic.IList<global::Kampai.Game.NamedCharacterDefinition> namedCharacterDefinitions)
		{
			if (namedCharacterDefinitions == null)
			{
				return;
			}
			MarkDefinitionsAsUsed(namedCharacterDefinitions);
			foreach (global::Kampai.Game.NamedCharacterDefinition namedCharacterDefinition in namedCharacterDefinitions)
			{
				global::Kampai.Game.FrolicCharacterDefinition frolicCharacterDefinition = namedCharacterDefinition as global::Kampai.Game.FrolicCharacterDefinition;
				if (frolicCharacterDefinition != null && frolicCharacterDefinition.WanderWeightedDeck != null)
				{
					MarkDefinitionAsUsed(frolicCharacterDefinition.WanderWeightedDeck);
				}
			}
		}

		private void MarkSalePackTransactionDefinitions(global::System.Collections.Generic.IList<global::Kampai.Game.SalePackDefinition> salePackDefinitions)
		{
			if (salePackDefinitions == null)
			{
				return;
			}
			packTransactionMap = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.TransactionDefinition>();
			MarkDefinitionsAsUsed(salePackDefinitions);
			for (int i = 0; i < salePackDefinitions.Count; i++)
			{
				global::Kampai.Game.SalePackDefinition salePackDefinition = salePackDefinitions[i];
				if (salePackDefinition.TransactionDefinition != null && salePackDefinition.TransactionDefinition.ID != 0)
				{
					global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = salePackDefinition.TransactionDefinition.ToDefinition();
					packTransactionMap.Add(salePackDefinition.TransactionDefinition.ID, transactionDefinition);
					MarkDefinitionAsUsed(transactionDefinition);
				}
			}
		}

		private void MarkRewardedAdvertisementDefinition(global::Kampai.Game.RewardedAdvertisementDefinition rewardedAdvertisementDefinition)
		{
			if (rewardedAdvertisementDefinition == null)
			{
				return;
			}
			MarkDefinitionAsUsed(rewardedAdvertisementDefinition);
			if (rewardedAdvertisementDefinition.PlacementDefinitions != null)
			{
				global::System.Collections.Generic.List<global::Kampai.Game.AdPlacementDefinition> placementDefinitions = rewardedAdvertisementDefinition.PlacementDefinitions;
				for (int i = 0; i < placementDefinitions.Count; i++)
				{
					global::Kampai.Game.AdPlacementDefinition d = placementDefinitions[i];
					MarkDefinitionAsUsed(d);
				}
			}
		}

		private void MarkCurrencyStorePackDefinitions(global::System.Collections.Generic.IList<global::Kampai.Game.CurrencyStorePackDefinition> currencyStorePackDefinitions)
		{
			if (currencyStorePackDefinitions == null)
			{
				return;
			}
			packTransactionMap = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.TransactionDefinition>();
			MarkDefinitionsAsUsed(currencyStorePackDefinitions);
			for (int i = 0; i < currencyStorePackDefinitions.Count; i++)
			{
				global::Kampai.Game.CurrencyStorePackDefinition currencyStorePackDefinition = currencyStorePackDefinitions[i];
				if (currencyStorePackDefinition.TransactionDefinition != null && currencyStorePackDefinition.TransactionDefinition.ID != 0)
				{
					global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = currencyStorePackDefinition.TransactionDefinition.ToDefinition();
					packTransactionMap.Add(currencyStorePackDefinition.TransactionDefinition.ID, transactionDefinition);
					MarkDefinitionAsUsed(transactionDefinition);
				}
			}
		}

		public bool Has(int id)
		{
			return AllDefinitions.ContainsKey(id);
		}

		public bool Has<T>(int id) where T : global::Kampai.Game.Definition
		{
			global::Kampai.Game.Definition definition = null;
			if (Has(id))
			{
				definition = AllDefinitions[id] as T;
			}
			return definition != null;
		}

		public global::Kampai.Game.Definition Get(int id)
		{
			if (Has(id))
			{
				return AllDefinitions[id];
			}
			logger.FatalNoThrow(global::Kampai.Util.FatalCode.DS_NO_ITEM_DEF, id);
			return null;
		}

		public T Get<T>(int id) where T : global::Kampai.Game.Definition
		{
			T val = (T)null;
			if (Has(id))
			{
				val = AllDefinitions[id] as T;
			}
			if (val == null)
			{
				logger.Error("Get<{1}>: Definition ID {0} not found in DefinitionService!", id, typeof(T).Name);
			}
			return val;
		}

		public T Get<T>(global::Kampai.Game.StaticItem staticItem) where T : global::Kampai.Game.Definition
		{
			return Get<T>((int)staticItem);
		}

		public bool TryGet<T>(int id, out T definition) where T : global::Kampai.Game.Definition
		{
			definition = (T)null;
			if (Has(id))
			{
				definition = AllDefinitions[id] as T;
			}
			return definition != null;
		}

		public global::System.Collections.Generic.List<T> GetAll<T>() where T : global::Kampai.Game.Definition
		{
			global::System.Collections.IList value;
			if (definitionsTypeMap.TryGetValue(typeof(T), out value))
			{
				return value as global::System.Collections.Generic.List<T>;
			}
			global::System.Collections.Generic.List<T> list = new global::System.Collections.Generic.List<T>();
			foreach (global::Kampai.Game.Definition value2 in AllDefinitions.Values)
			{
				T val = value2 as T;
				if (val != null)
				{
					list.Add(val);
				}
			}
			definitionsTypeMap.Add(typeof(T), list);
			return list;
		}

		public T Get<T>() where T : global::Kampai.Game.Definition
		{
			foreach (global::Kampai.Game.Definition value in AllDefinitions.Values)
			{
				T val = value as T;
				if (val != null)
				{
					return val;
				}
			}
			return (T)null;
		}

		public global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Definition> GetAllDefinitions()
		{
			return AllDefinitions;
		}

		public global::System.Collections.Generic.IList<string> GetEnvironemtDefinition()
		{
			return environmentDefinition;
		}

		public void ReclaimEnfironmentDefinitions()
		{
			environmentDefinition = null;
		}

		public global::Kampai.Game.Transaction.WeightedDefinition GetGachaWeightsForNumMinions(int numMinions, bool party)
		{
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.WeightedDefinition> dictionary = ((!party) ? gachaDefinitionsByNumMinions : partyDefinitionsByNumMinions);
			if (dictionary.ContainsKey(numMinions))
			{
				return dictionary[numMinions];
			}
			return dictionary[4];
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedDefinition> GetAllGachaDefinitions()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedDefinition> list = new global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedDefinition>();
			list.AddRange(gachaDefinitionsByNumMinions.Values);
			list.AddRange(partyDefinitionsByNumMinions.Values);
			return list;
		}

		public int GetHarvestItemDefinitionIdFromTransactionId(int transactionId)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionId);
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> outputs = transactionDefinition.Outputs;
			if (outputs != null && outputs.Count > 0 && outputs[0] != null)
			{
				return outputs[0].ID;
			}
			logger.Fatal(global::Kampai.Util.FatalCode.PS_NO_TRANSACTION, transactionId);
			return -1;
		}

		public string GetHarvestIconFromTransactionID(int transactionId)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionId);
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> outputs = transactionDefinition.Outputs;
			if (outputs != null && outputs.Count > 0 && outputs[0] != null)
			{
				global::Kampai.Game.ItemDefinition itemDefinition = Get<global::Kampai.Game.ItemDefinition>(outputs[0].ID);
				return itemDefinition.Image;
			}
			logger.Fatal(global::Kampai.Util.FatalCode.PS_NO_TRANSACTION, transactionId);
			return string.Empty;
		}

		public int ExtractQuantityFromTransaction(int transactionID, int definitionID)
		{
			int result = 0;
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionID);
			if (transactionDefinition.Outputs != null)
			{
				result = global::Kampai.Game.Transaction.TransactionUtil.ExtractQuantityFromTransaction(transactionDefinition, definitionID);
			}
			return result;
		}

		public int GetBuildingDefintionIDFromItemDefintionID(int itemDefinitionID)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.VillainLairDefinition> all = GetAll<global::Kampai.Game.VillainLairDefinition>();
			foreach (global::Kampai.Game.VillainLairDefinition item in all)
			{
				if (item != null && item.ResourceItemID == itemDefinitionID)
				{
					return item.ResourceBuildingDefID;
				}
			}
			global::System.Collections.Generic.IList<global::Kampai.Game.AnimatingBuildingDefinition> all2 = GetAll<global::Kampai.Game.AnimatingBuildingDefinition>();
			foreach (global::Kampai.Game.AnimatingBuildingDefinition item2 in all2)
			{
				global::Kampai.Game.ResourceBuildingDefinition resourceBuildingDefinition = item2 as global::Kampai.Game.ResourceBuildingDefinition;
				if (resourceBuildingDefinition != null && resourceBuildingDefinition.ItemId == itemDefinitionID)
				{
					return resourceBuildingDefinition.ID;
				}
				global::Kampai.Game.CraftingBuildingDefinition craftingBuildingDefinition = item2 as global::Kampai.Game.CraftingBuildingDefinition;
				if (craftingBuildingDefinition == null)
				{
					continue;
				}
				foreach (global::Kampai.Game.RecipeDefinition recipeDefinition in craftingBuildingDefinition.RecipeDefinitions)
				{
					if (recipeDefinition.ItemID == itemDefinitionID)
					{
						return craftingBuildingDefinition.ID;
					}
				}
			}
			return 0;
		}

		public string GetBuildingFootprint(int ID)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.FootprintDefinition> all = GetAll<global::Kampai.Game.FootprintDefinition>();
			foreach (global::Kampai.Game.FootprintDefinition item in all)
			{
				if (item.ID == ID)
				{
					return item.Footprint;
				}
			}
			return string.Empty;
		}

		public int GetIncrementalCost(global::Kampai.Game.Definition definition)
		{
			global::Kampai.Game.BuildingDefinition buildingDefinition = definition as global::Kampai.Game.BuildingDefinition;
			return (buildingDefinition != null) ? buildingDefinition.IncrementalCost : 0;
		}

		public global::Kampai.Game.BridgeDefinition GetBridgeDefinition(int itemDefinitionID)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.BridgeDefinition> all = GetAll<global::Kampai.Game.BridgeDefinition>();
			foreach (global::Kampai.Game.BridgeDefinition item in all)
			{
				if (item.ID == itemDefinitionID)
				{
					return item;
				}
			}
			return null;
		}

		public bool HasUnlockItemInTransactionOutput(int transactionID)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionID);
			if (transactionDefinition.Outputs != null)
			{
				foreach (global::Kampai.Util.QuantityItem output in transactionDefinition.Outputs)
				{
					global::Kampai.Game.Definition definition = Get<global::Kampai.Game.Definition>(output.ID);
					if (definition is global::Kampai.Game.UnlockDefinition)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetLevelItemUnlocksAt(int definitionID)
		{
			if (levelUnlockLookUpTable == null)
			{
				levelUnlockLookUpTable = new global::System.Collections.Generic.Dictionary<int, int>();
				global::Kampai.Game.LevelUpDefinition levelUpDefinition = Get<global::Kampai.Game.LevelUpDefinition>(88888);
				for (int i = 0; i < levelUpDefinition.transactionList.Count; i++)
				{
					if (levelUpDefinition.transactionList[i] == 0)
					{
						continue;
					}
					global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = Get<global::Kampai.Game.Transaction.TransactionDefinition>(levelUpDefinition.transactionList[i]);
					if (transactionDefinition == null || transactionDefinition.Outputs == null)
					{
						continue;
					}
					foreach (global::Kampai.Util.QuantityItem output in transactionDefinition.Outputs)
					{
						global::Kampai.Game.UnlockDefinition definition = null;
						if (TryGet<global::Kampai.Game.UnlockDefinition>(output.ID, out definition) && !levelUnlockLookUpTable.ContainsKey(definition.ReferencedDefinitionID))
						{
							levelUnlockLookUpTable.Add(definition.ReferencedDefinitionID, i);
						}
					}
				}
			}
			if (!levelUnlockLookUpTable.ContainsKey(definitionID))
			{
				return 0;
			}
			return levelUnlockLookUpTable[definitionID];
		}

		public global::Kampai.Game.TaskLevelBandDefinition GetTaskLevelBandForLevel(int level)
		{
			if (taskDefinition != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.TaskLevelBandDefinition> levelBands = taskDefinition.levelBands;
				if (levelBands != null)
				{
					global::Kampai.Game.TaskLevelBandDefinition result = levelBands[0];
					{
						foreach (global::Kampai.Game.TaskLevelBandDefinition item in levelBands)
						{
							if (item.MinLevel <= level)
							{
								result = item;
							}
						}
						return result;
					}
				}
			}
			return null;
		}

		public global::Kampai.Game.RushTimeBandDefinition GetRushTimeBandForTime(int timeRemainingInSeconds)
		{
			if (rushDefinitions != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.RushTimeBandDefinition> list = rushDefinitions;
				if (list != null)
				{
					foreach (global::Kampai.Game.RushTimeBandDefinition item in list)
					{
						if (timeRemainingInSeconds <= item.TimeRemainingInSeconds)
						{
							return item;
						}
					}
					return list[list.Count - 1];
				}
			}
			return null;
		}

		public global::Kampai.Game.AchievementDefinition GetAchievementDefinitionFromDefinitionID(int defID)
		{
			if (achievementDefinitions != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.AchievementDefinition> list = achievementDefinitions;
				if (list != null)
				{
					foreach (global::Kampai.Game.AchievementDefinition item in list)
					{
						if (item.DefinitionID == defID)
						{
							return item;
						}
					}
				}
			}
			return null;
		}

		public int getItemTransactionID(int id)
		{
			if (itemTransactionTable == null)
			{
				itemTransactionTable = new global::System.Collections.Generic.Dictionary<int, int>();
				global::System.Collections.Generic.IList<global::Kampai.Game.StoreItemDefinition> all = GetAll<global::Kampai.Game.StoreItemDefinition>();
				foreach (global::Kampai.Game.StoreItemDefinition item in all)
				{
					itemTransactionTable[item.ReferencedDefID] = item.TransactionID;
				}
			}
			if (!itemTransactionTable.ContainsKey(id))
			{
				logger.Warning("getItemTransactionID: No transaction for id {0}", id);
				return 0;
			}
			return itemTransactionTable[id];
		}

		public global::Kampai.Game.Transaction.TransactionDefinition GetPackTransaction(int transactionId)
		{
			if (packTransactionMap == null || !packTransactionMap.ContainsKey(transactionId))
			{
				return null;
			}
			return packTransactionMap[transactionId];
		}

		public global::Kampai.Game.PackDefinition GetPackDefinitionFromTransactionId(int transactionId)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.PackDefinition> all = GetAll<global::Kampai.Game.PackDefinition>();
			if (all == null)
			{
				return null;
			}
			for (int i = 0; i < all.Count; i++)
			{
				global::Kampai.Game.PackDefinition packDefinition = all[i];
				if (packDefinition.TransactionDefinition.ID == transactionId)
				{
					return packDefinition;
				}
			}
			return null;
		}

		private global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Trigger.TriggerRewardDefinition> GetTriggerRewardDefinitions(global::Kampai.Game.Definitions definitions)
		{
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Trigger.TriggerRewardDefinition> dictionary = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Trigger.TriggerRewardDefinition>();
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerRewardDefinition> triggerRewardDefinitions = definitions.triggerRewardDefinitions;
			MarkDefinitionsAsUsed(triggerRewardDefinitions);
			if (triggerRewardDefinitions == null)
			{
				return dictionary;
			}
			for (int i = 0; i < definitions.triggerRewardDefinitions.Count; i++)
			{
				if (!dictionary.ContainsKey(triggerRewardDefinitions[i].ID))
				{
					dictionary.Add(triggerRewardDefinitions[i].ID, triggerRewardDefinitions[i]);
				}
			}
			return dictionary;
		}

		private global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerDefinition> GetTriggerDefinitions(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.triggerDefinitions == null)
			{
				return null;
			}
			MarkDefinitionsAsUsed(definitions.triggerDefinitions);
			global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerDefinition> list = new global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerDefinition>(definitions.triggerDefinitions);
			global::Kampai.Game.Trigger.TriggerDefinition triggerDefinition = null;
			for (int i = 0; i < list.Count; i++)
			{
				triggerDefinition = list[i];
				if (triggerDefinition.reward == null || triggerDefinition.reward.Count == 0)
				{
					logger.Error("Trigger Definition doesn't contain any reward ids {0}", triggerDefinition);
					continue;
				}
				for (int j = 0; j < triggerDefinition.reward.Count; j++)
				{
					int key = triggerDefinition.reward[j];
					if (triggerRewardsDefinitions.ContainsKey(key))
					{
						global::Kampai.Game.Trigger.TriggerRewardDefinition item = triggerRewardsDefinitions[key];
						triggerDefinition.rewards.Add(item);
					}
				}
			}
			list.Sort();
			return list;
		}

		private global::System.Collections.Generic.IList<global::Kampai.Game.CurrencyStoreCategoryDefinition> GetCurrencyStoreCategoryDefinitions(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.currencyStoreDefinition == null)
			{
				return null;
			}
			MarkDefinitionAsUsed(definitions.currencyStoreDefinition);
			MarkDefinitionsAsUsed(definitions.currencyStoreDefinition.CategoryDefinitions);
			global::System.Collections.Generic.List<global::Kampai.Game.CurrencyStoreCategoryDefinition> list = new global::System.Collections.Generic.List<global::Kampai.Game.CurrencyStoreCategoryDefinition>(definitions.currencyStoreDefinition.CategoryDefinitions);
			list.Sort();
			return list;
		}

		private void AssembleGachaAnimationDefinitions()
		{
			gachaDefinitionsByNumMinions = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.WeightedDefinition>();
			partyDefinitionsByNumMinions = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Transaction.WeightedDefinition>();
			foreach (global::Kampai.Game.GachaWeightedDefinition item in GetAll<global::Kampai.Game.GachaWeightedDefinition>())
			{
				global::Kampai.Game.Transaction.WeightedDefinition weightedDefinition = item.WeightedDefinition;
				MarkDefinitionAsUsed(weightedDefinition);
				int minions = item.Minions;
				bool partyOnly = item.PartyOnly;
				if ((!partyOnly && gachaDefinitionsByNumMinions.ContainsKey(minions)) || (partyOnly && partyDefinitionsByNumMinions.ContainsKey(minions)))
				{
					logger.Warning("DefinitionService: Duplicate gacha for minions {0}", minions); continue;
				}
				global::System.Collections.Generic.IList<global::Kampai.Game.Transaction.WeightedQuantityItem> entities = weightedDefinition.Entities;
				foreach (global::Kampai.Game.Transaction.WeightedQuantityItem item2 in entities)
				{
					int iD = item2.ID;
					if (iD > 0)
					{
						global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = Get<global::Kampai.Game.GachaAnimationDefinition>(iD);
						if (gachaAnimationDefinition == null)
						{
							logger.Warning("DefinitionService: Gacha animation relation does not exist for ID {0}", iD); continue;
						}
						if (gachaAnimationDefinition.Minions > 0 && gachaAnimationDefinition.Minions != minions)
						{
							logger.Warning("DefinitionService: Minion count mismatch for gacha animation ID {0}", item2.ID); continue;
						}
						if (gachaAnimationDefinition.MinPerformance > targetPerformance)
						{
							item2.Weight = 0u;
						}
					}
				}
				if (partyOnly)
				{
					partyDefinitionsByNumMinions.Add(minions, weightedDefinition);
				}
				else
				{
					gachaDefinitionsByNumMinions.Add(minions, weightedDefinition);
				}
			}
		}

		private void AddMinionBenefitDefinition(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.minionBenefitDefinition != null) AllDefinitions[89898] = definitions.minionBenefitDefinition;
		}

		private void AddLevelUpDefinition(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.levelUpDefinition != null) AllDefinitions[88888] = definitions.levelUpDefinition;
		}

		private void AddNotificationSystemDefinition(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.notificationSystemDefinition != null) AllDefinitions[66666] = definitions.notificationSystemDefinition;
		}

		private void AddDropLevelBandDefinition(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.randomDropLevelBandDefinition != null) AllDefinitions[88889] = definitions.randomDropLevelBandDefinition;
		}

		private void AddLevelXPTable(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.levelXPTable != null) AllDefinitions[99999] = definitions.levelXPTable;
		}

		private void AddLevelFunTable(global::Kampai.Game.Definitions definitions)
		{
			if (definitions.levelFunTable != null) AllDefinitions[1000009681] = definitions.levelFunTable;
		}

		private void MarkDefinitionAsUsed(global::Kampai.Game.Definition d)
		{
			if (d == null)
			{
				return;
			}
			int iD = d.ID;
			if (validateDefinitions && AllDefinitions.ContainsKey(iD))
			{
				logger.Warning("DefinitionService.MarkDefinitionAsUsed(): duplicate defId = {0}", iD);
			}
			AllDefinitions[d.ID] = d;
		}

		private global::System.Collections.Generic.IEnumerable<T> MarkDefinitionsAsUsed<T>(global::System.Collections.Generic.IEnumerable<T> used) where T : global::Kampai.Game.Definition
		{
			if (used != null)
			{
				int total = 0;
				int registered = 0;
				foreach (T item in used)
				{
					total++;
					if (item != null && !item.Disabled)
					{
						MarkDefinitionAsUsed(item);
						registered++;
					}
				}
			}
			return used;
		}

		private global::Kampai.Game.TaskDefinition GetTaskDefintion(global::Kampai.Game.Definitions defs)
		{
			global::Kampai.Game.TaskDefinition tasks = defs.tasks;
			if (tasks != null)
			{
				global::System.Collections.Generic.List<global::Kampai.Game.TaskLevelBandDefinition> list = new global::System.Collections.Generic.List<global::Kampai.Game.TaskLevelBandDefinition>(tasks.levelBands);
				list.Sort((global::Kampai.Game.TaskLevelBandDefinition p1, global::Kampai.Game.TaskLevelBandDefinition p2) => p1.MinLevel - p2.MinLevel);
				tasks.levelBands = list;
			}
			return tasks;
		}

		private global::System.Collections.Generic.IList<global::Kampai.Game.RushTimeBandDefinition> GetRushDefinitions(global::Kampai.Game.Definitions defs)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.RushTimeBandDefinition> list = defs.rushDefinitions;
			if (list != null)
			{
				global::System.Collections.Generic.List<global::Kampai.Game.RushTimeBandDefinition> list2 = new global::System.Collections.Generic.List<global::Kampai.Game.RushTimeBandDefinition>(list);
				global::Kampai.Util.RushUtil.SortByTime(list2);
				list = list2;
			}
			return list;
		}

		private global::System.Collections.Generic.IList<global::Kampai.Game.AchievementDefinition> GetAchievementDefinitions(global::Kampai.Game.Definitions defs)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.AchievementDefinition> list = defs.achievementDefinitions;
			if (list != null)
			{
				return list;
			}
			return null;
		}

		public global::Kampai.Game.SalePackType getSKUSalePackType(string ExternalIdentifier)
		{
			foreach (global::Kampai.Game.SalePackDefinition item in GetAll<global::Kampai.Game.SalePackDefinition>())
			{
				if (global::Kampai.Util.ItemUtil.CompareSKU(item.SKU, ExternalIdentifier))
				{
					return item.Type;
				}
			}
			return global::Kampai.Game.SalePackType.StarterPack;
		}

		public int GetPartyFavorDefinitionIDByItemID(int itemID)
		{
			foreach (global::Kampai.Game.PartyFavorAnimationDefinition item in GetAll<global::Kampai.Game.PartyFavorAnimationDefinition>())
			{
				if (item.ItemID == itemID)
				{
					return item.ID;
				}
			}
			return -1;
		}

		public string GetLegalURL(global::Kampai.Util.LegalDocuments.LegalType type, string language)
		{
			string result = string.Empty;
			foreach (global::Kampai.Game.LegalDocumentDefinition item in GetAll<global::Kampai.Game.LegalDocumentDefinition>())
			{
				if (item.type != type)
				{
					continue;
				}
				foreach (global::Kampai.Game.LegalDocumentURL url in item.urls)
				{
					if (url.language == "en")
					{
						result = url.url;
					}
					if (url.language == language)
					{
						return url.url;
					}
				}
			}
			return result;
		}

		public void SetInitialPlayer(string playerJson)
		{
			initialPlayer = playerJson;
		}

		public string GetInitialPlayer()
		{
			return initialPlayer;
		}

		public void Add(global::Kampai.Game.Definition definition)
		{
			AllDefinitions.Add(definition.ID, definition);
			definitionsTypeMap.Clear();
		}

		public void Remove(global::Kampai.Game.Definition definition)
		{
			AllDefinitions.Remove(definition.ID);
			definitionsTypeMap.Clear();
		}

		public void SetPerformanceQualityLevel(global::Kampai.Util.TargetPerformance targetPerformance)
		{
			this.targetPerformance = targetPerformance;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerDefinition> GetTriggerDefinitions()
		{
			return triggerDefinitions;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.CurrencyStoreCategoryDefinition> GetCurrencyStoreCategoryDefinitions()
		{
			return currencyStoreCategoryDefinitions;
		}
	}
}
