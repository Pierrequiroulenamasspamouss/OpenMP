namespace Kampai.Game
{
	public class PlayerService : global::Kampai.Game.IPlayerService
	{
		protected global::Kampai.Game.Player player;

		private object mutex = new object();

		private global::System.Collections.Generic.HashSet<string> segments;

		protected global::Kampai.Game.TransactionEngine _engine;

		protected global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("PlayerService") as global::Kampai.Util.IKampaiLogger;

		private string swrveGroup;

		protected global::Kampai.Game.TransactionEngine engine
		{
			get
			{
				if (_engine == null)
				{
					_engine = new global::Kampai.Game.TransactionEngine(logger, definitionService, randService, this);
				}
				return _engine;
			}
		}

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Common.IRandomService randService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.PostTransactionSignal postTransactionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.InsufficientInputsSignal insufficientInputsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisplayRandomDropIconSignal randomDropSignal { get; set; }

		[Inject]
		public global::Kampai.Game.OpenStorageBuildingSignal openStorageBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal changeState { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Game.ITelemetryUtil telemetryUtil { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistanceService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }

		[Inject]
		public global::Kampai.Game.ShowCraftingBuildingMenuSignal showCraftingBuildingMenuSignal { get; set; }

		[Inject]
		public global::Kampai.Game.FTUELevelChangedSignal ftueLevelChangedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SendBuildingToInventorySignal sendBuildingToInventorySignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPartyService partyService { get; set; }

		public long ID
		{
			get
			{
				return player.ID;
			}
			set
			{
				player.ID = value;
			}
		}

		public global::Kampai.Game.Player LastSave { get; set; }

		public bool PlayerDataIsLoaded { get; set; }

		public int LevelUpUTC
		{
			get
			{
				return player.lastLevelUpTime;
			}
			set
			{
				player.lastLevelUpTime = value;
			}
		}

		public int LastGameStartUTC
		{
			get
			{
				return player.lastGameStartTime;
			}
			set
			{
				player.lastGameStartTime = value;
			}
		}

		public int FirstGameStartUTC
		{
			get
			{
				return player.firstGameStartTime;
			}
			set
			{
				player.firstGameStartTime = value;
			}
		}

		public int LastPlayedUTC
		{
			get
			{
				return player.lastPlayedTime;
			}
			set
			{
				player.lastPlayedTime = value;
			}
		}

		public int TimeZoneOffset
		{
			get
			{
				return player.timezoneOffset;
			}
			set
			{
				player.timezoneOffset = value;
			}
		}

		public string SWRVEGroup
		{
			get
			{
				return (!string.IsNullOrEmpty(swrveGroup)) ? swrveGroup : "anyVariant";
			}
			set
			{
				swrveGroup = value;
			}
		}

		public int GameplaySecondsSinceLevelUp
		{
			get
			{
				return player.totalGameplayDurationSinceLastLevelUp;
			}
			set
			{
				player.totalGameplayDurationSinceLastLevelUp = value;
			}
		}

		public int AccumulatedGameplayDuration
		{
			get
			{
				return player.totalAccumulatedGameplayDuration;
			}
			set
			{
				player.totalAccumulatedGameplayDuration = value;
			}
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Player.HelpTipTrackingItem> helpTipsTrackingData
		{
			get
			{
				return player.helpTipsTrackingData;
			}
		}

		public PlayerService()
		{
		}

		public PlayerService(global::Kampai.Game.Player player)
			: this()
		{
			this.player = player;
		}

		public uint GetQuantity(global::Kampai.Game.StaticItem def)
		{
			return player.GetQuantity(def);
		}

		public int GetCountByDefinitionId(int defId)
		{
			return player.GetCountByDefinitionId(defId);
		}

		public uint GetQuantityByDefinitionId(int defId)
		{
			return player.GetQuantityByDefinitionId(defId);
		}

		public uint GetTotalCountByDefinitionId(int defId)
		{
			return player.GetTotalCountByDefinitionId(defId);
		}

		public uint GetQuantityByInstanceId(int instanceId)
		{
			return player.GetQuantityByInstanceId(instanceId);
		}

		public void AlterQuantity(global::Kampai.Game.StaticItem def, int amount)
		{
			player.AlterQuantity(def, amount);
		}

		public void SetQuantity(global::Kampai.Game.StaticItem def, int amount)
		{
			if (amount >= 0)
			{
				player.SetQuantityByStaticItem(def, (uint)amount);
			}
			else
			{
				logger.Fatal(global::Kampai.Util.FatalCode.PS_NEGATIVE_VALUE, amount);
			}
		}

		public global::System.Collections.Generic.ICollection<int> GetAnimatingBuildingIDs()
		{
			return player.FindAllAnimatingBuildingIDs();
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Item> GetSellableItems()
		{
			uint totalStorableQuantity = 0u;
			uint totalSellableQuantity = 0u;
			return player.GetSellableItems(out totalStorableQuantity, out totalSellableQuantity);
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Item> GetSellableItems(out uint totalStorableQuantity, out uint totalSellableQuantity)
		{
			return player.GetSellableItems(out totalStorableQuantity, out totalSellableQuantity);
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Instance> GetInstancesByDefinition<T>() where T : global::Kampai.Game.Definition
		{
			return player.GetInstancesByDefinition<T>();
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Instance> GetInstancesByDefinitionID(int defId)
		{
			return player.GetInstancesByDefinitionID(defId);
		}

		public void GetInstancesByType<T>(ref global::System.Collections.Generic.List<T> list) where T : class, global::Kampai.Game.Instance
		{
			player.GetInstancesByType(ref list);
		}

		public global::System.Collections.Generic.List<T> GetInstancesByType<T>() where T : class, global::Kampai.Game.Instance
		{
			global::System.Collections.Generic.List<T> result = new global::System.Collections.Generic.List<T>();
			player.GetInstancesByType(ref result);
			return result;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Item> GetItemsByDefinition<T>() where T : global::Kampai.Game.Definition
		{
			return player.GetItemsByDefinition<T>();
		}

		public global::System.Collections.Generic.List<int> GetUnavailableBuildingIDSForItem(int itemID)
		{
			return GetUnavailableBuildingIDSForItem(definitionService.Get<global::Kampai.Game.IngredientsItemDefinition>(itemID));
		}

		private global::System.Collections.Generic.List<int> GetUnavailableBuildingIDSForItem(global::Kampai.Game.IngredientsItemDefinition item)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			if (item != null)
			{
				int buildingDefintionIDFromItemDefintionID = definitionService.GetBuildingDefintionIDFromItemDefintionID(item.ID);
				if (GetFirstInstanceByDefinitionId<global::Kampai.Game.Building>(buildingDefintionIDFromItemDefintionID) == null)
				{
					list.Add(buildingDefintionIDFromItemDefintionID);
				}
				global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(item.TransactionId).Inputs;
				foreach (global::Kampai.Util.QuantityItem item2 in inputs)
				{
					buildingDefintionIDFromItemDefintionID = definitionService.GetBuildingDefintionIDFromItemDefintionID(item2.ID);
					if (GetFirstInstanceByDefinitionId<global::Kampai.Game.Building>(buildingDefintionIDFromItemDefintionID) == null)
					{
						list.Add(buildingDefintionIDFromItemDefintionID);
						break;
					}
					list.AddRange(GetUnavailableBuildingIDSForItem(item2.ID));
				}
			}
			return list;
		}

		public bool HasPurchasedBuildingAssociatedWithItem(global::Kampai.Game.IngredientsItemDefinition item)
		{
			int buildingDefintionIDFromItemDefintionID = definitionService.GetBuildingDefintionIDFromItemDefintionID(item.ID);
			if (buildingDefintionIDFromItemDefintionID == 0)
			{
				logger.Error("Can't find building for itemDefinitionID {0}", item.ID);
				return false;
			}
			return GetFirstInstanceByDefinitionId<global::Kampai.Game.Building>(buildingDefintionIDFromItemDefintionID) != null;
		}

		public bool ItemUsesBuildingsInList(int itemID, global::System.Collections.Generic.List<int> buildingIDs)
		{
			return ItemUsesBuildingsInList(definitionService.Get<global::Kampai.Game.IngredientsItemDefinition>(itemID), buildingIDs);
		}

		public bool ItemUsesBuildingsInList(global::Kampai.Game.IngredientsItemDefinition item, global::System.Collections.Generic.List<int> buildingIDs)
		{
			bool result = false;
			if (item != null)
			{
				int buildingDefintionIDFromItemDefintionID = definitionService.GetBuildingDefintionIDFromItemDefintionID(item.ID);
				if (buildingIDs.Contains(buildingDefintionIDFromItemDefintionID))
				{
					result = true;
				}
				else
				{
					global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(item.TransactionId).Inputs;
					foreach (global::Kampai.Util.QuantityItem item2 in inputs)
					{
						buildingDefintionIDFromItemDefintionID = definitionService.GetBuildingDefintionIDFromItemDefintionID(item2.ID);
						if (buildingIDs.Contains(buildingDefintionIDFromItemDefintionID) || ItemUsesBuildingsInList(item2.ID, buildingIDs))
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		public global::System.Collections.Generic.ICollection<global::Kampai.Game.Item> GetStorableItems(out uint itemCount)
		{
			return player.FindStorableItems(out itemCount);
		}

		public global::System.Collections.Generic.Dictionary<int, int> GetBuildingOnBoardCountMap()
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Building> instancesByType = player.GetInstancesByType<global::Kampai.Game.Building>();
			global::System.Collections.Generic.Dictionary<int, int> dictionary = new global::System.Collections.Generic.Dictionary<int, int>();
			int num = 0;
			foreach (global::Kampai.Game.Building item in instancesByType)
			{
				if (item.State != global::Kampai.Game.BuildingState.Inventory)
				{
					num = item.Definition.ID;
					if (dictionary.ContainsKey(num))
					{
						global::System.Collections.Generic.Dictionary<int, int> dictionary3;
						global::System.Collections.Generic.Dictionary<int, int> dictionary2 = (dictionary3 = dictionary);
						int key2;
						int key = (key2 = num);
						key2 = dictionary3[key2];
						dictionary2[key] = key2 + 1;
					}
					else
					{
						dictionary.Add(num, 1);
					}
				}
			}
			return dictionary;
		}

		public T GetByInstanceId<T>(int id) where T : class, global::Kampai.Game.Instance
		{
			return player.GetByInstanceId<T>(id);
		}

		public T GetFirstInstanceByDefinitionId<T>(int definitionId) where T : class, global::Kampai.Game.Instance
		{
			return player.GetFirstInstanceByDefinitionId<T>(definitionId);
		}

		public I GetFirstInstanceByDefintion<I, D>() where I : class, global::Kampai.Game.Instance where D : global::Kampai.Game.Definition
		{
			return player.GetFirstInstanceByDefintion<I, D>();
		}

		public int GetUnlockedQuantityOfID(int defId)
		{
			return player.GetUnlockedAmountFromID(defId);
		}

		public int GetPurchasedQuanityByUpsellID(int defId)
		{
			return player.GetAmountPurchased(defId);
		}

		public global::System.Collections.Generic.IDictionary<int, int> GetAllUpsellsPurchased()
		{
			return player.GetUpsellsPurchased();
		}

		public void AddUpsellToPurchased(int defID)
		{
			player.AddPurchasedUpsells(defID);
		}

		public void ClearPurchasedUpsells(int defID)
		{
			player.ClearPurchasedUpsells(defID);
		}

		public global::System.Collections.Generic.IList<T> GetUnlockedDefsByType<T>() where T : global::Kampai.Game.Definition
		{
			return player.FindAllUnLockedByType<T>();
		}

		public bool IsMinionPartyUnlocked()
		{
			return GetUnlockedQuantityOfID(80000) > 0;
		}

		public bool HasPurchasedMinigamePack()
		{
			return GetPurchasedQuanityByUpsellID(50002) > 0;
		}

		private bool RunTransaction(global::Kampai.Game.Transaction.TransactionDefinition transaction, out global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs, global::Kampai.Game.TransactionArg arg = null)
		{
			outputs = null;
			if (transaction != null)
			{
				return engine.Perform(player, transaction, out outputs, arg);
			}
			logger.Fatal(global::Kampai.Util.FatalCode.PS_NULL_TRANSACTION, "Null transaction");
			return false;
		}

		public global::Kampai.Game.Player LoadPlayerData(string serialized)
		{
			global::Kampai.Game.Player player = null;
			lock (mutex)
			{
				try
				{
					global::Kampai.Game.PlayerVersion playerVersion = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.PlayerVersion>(serialized);
					logger.Info("Player Version is {0}", playerVersion.Version);
					player = playerVersion.CreatePlayer(serialized, definitionService, localPersistanceService, partyService, logger);
					if (player == null)
					{
						throw new global::Kampai.Util.FatalException(global::Kampai.Util.FatalCode.PS_NULL_PLAYER, "PlayerService.LoadPlayerData(): null player");
					}
				}
				catch (global::Newtonsoft.Json.JsonSerializationException e)
				{
					HandleJsonParseException(serialized, e);
				}
				catch (global::Newtonsoft.Json.JsonReaderException e2)
				{
					HandleJsonParseException(serialized, e2);
				}
			}
			return player;
		}

		private void HandleJsonParseException(string json, global::System.Exception e)
		{
			logger.Error("HandleJsonParseException(): player json: {0}", json ?? "null");
			throw new global::Kampai.Util.FatalException(global::Kampai.Util.FatalCode.PS_JSON_PARSE_ERR, 5, e, "Json Parse Err: {0}", e);
		}

		public void Deserialize(string serialized, bool isRetry = false)
		{
			player = LoadPlayerData(serialized);
			LastSave = LoadPlayerData(serialized);
		}

		public byte[] SavePlayerData(global::Kampai.Game.Player playerData)
		{
			byte[] result = null;
			lock (mutex)
			{
				try
				{
					global::Kampai.Game.PlayerVersion playerVersion = new global::Kampai.Game.PlayerVersion();
					playerData.country = localizationService.GetCountry();
					result = playerVersion.Serialize(playerData, definitionService, logger);
				}
				catch (global::Newtonsoft.Json.JsonSerializationException ex)
				{
					logger.Fatal(global::Kampai.Util.FatalCode.PS_JSON_SERIALIZE_ERR, "Json Err: {0}", ex.ToString());
				}
			}
			return result;
		}

		public byte[] Serialize()
		{
			return SavePlayerData(player);
		}

		public bool IsPlayerInitialized()
		{
			return player != null;
		}

		public void Add(global::Kampai.Game.Instance i)
		{
			int iD = i.ID;
			if (iD != 0)
			{
				global::Kampai.Game.Instance byInstanceId = player.GetByInstanceId<global::Kampai.Game.Instance>(iD);
				if (byInstanceId != null)
				{
					logger.Fatal(global::Kampai.Util.FatalCode.PS_ITEM_ALREADY_ADDED, "Item {0} already exists in player inventory.", iD);
					return;
				}
			}
			player.AssignNextInstanceId(i);
			player.Add(i);
		}

		public void AssignNextInstanceId(global::Kampai.Util.Identifiable i)
		{
			player.AssignNextInstanceId(i);
		}

		public void Remove(global::Kampai.Game.Instance i)
		{
			if (i.ID != 0)
			{
				player.Remove(i);
			}
		}

		public global::System.Collections.Generic.ICollection<T> GetByDefinitionId<T>(int id) where T : global::Kampai.Game.Instance
		{
			return player.GetByDefinitionId<T>(id);
		}

		public global::Kampai.Game.Transaction.WeightedInstance GetWeightedInstance(int defId, global::Kampai.Game.Transaction.WeightedDefinition wd = null)
		{
			return player.GetWeightedInstance(defId, wd);
		}

		public bool CanAffordExchange(int grindNeeded)
		{
			return player.GetQuantityByDefinitionId(1) >= PremiumCostForGrind(grindNeeded);
		}

		public int PremiumCostForGrind(int grindNeeded)
		{
			return engine.RequiredPremiumForGrind(grindNeeded);
		}

		public void ExchangePremiumForGrind(int grindNeeded, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback)
		{
			int num = engine.RequiredPremiumForGrind(grindNeeded);
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
			transactionDefinition.ID = int.MaxValue;
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> list = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> list2 = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			grindNeeded = engine.PremiumToGrind(num);
			list.Add(new global::Kampai.Util.QuantityItem(1, (uint)num));
			list2.Add(new global::Kampai.Util.QuantityItem(0, (uint)grindNeeded));
			transactionDefinition.Inputs = list;
			transactionDefinition.Outputs = list2;
			ProcessItemPurchase(num, list2, true, callback, true);
		}

		public int CalculateRushCost(global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items)
		{
			return engine.CalculateRushCost(player, items);
		}

		public void ProcessSlotPurchase(int slotCost, bool showStoreOnFail, int slotNumber, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, int instanceId)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(slotCost);
			string source = "SlotPurchase" + slotNumber;
			ProcessPremiumTransaction(list, null, showStoreOnFail, callback, source, instanceId, true);
		}

		public void ProcessSaleCancel(int cost, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(cost);
			string source = "DeleteSale";
			ProcessPremiumTransaction(list, null, true, callback, source, 314, false, true);
		}

		public void ProcessRefreshMarket(int cost, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback)
		{
			global::System.Collections.Generic.IList<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(cost);
			ProcessPremiumTransaction(list, null, showStoreOnFail, callback, "RefreshMarket", 3117, true);
		}

		public void ProcessOrderFill(int orderCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(orderCost);
			string source = "OrderCompletion";
			ProcessPremiumTransaction(list, items, showStoreOnFail, callback, source, 3022, false, true);
		}

		public void ProcessItemPurchase(int itemCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, bool byPassStorageCheck = false)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(itemCost);
			ProcessPremiumTransaction(list, items, showStoreOnFail, callback, "ItemPurchase", 0, true, byPassStorageCheck);
		}

		public void ProcessItemPurchase(global::System.Collections.Generic.IList<int> itemCosts, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, bool byPassStorageCheck = false)
		{
			ProcessPremiumTransaction(itemCosts, items, showStoreOnFail, callback, "ItemPurchase", 0, true, byPassStorageCheck);
		}

		public void ProcessRush(int rushCost, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, int instanceId)
		{
			global::System.Collections.Generic.IList<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(rushCost);
			ProcessPremiumTransaction(list, null, showStoreOnFail, callback, "Rush", instanceId, true);
		}

		public void ProcessRush(int rushCost, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback)
		{
			ProcessRush(rushCost, null, showStoreOnFail, callback);
		}

		public void ProcessRush(int rushCost, bool showStoreOnFail, string source, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback)
		{
			ProcessRush(rushCost, null, showStoreOnFail, source, callback);
		}

		public void ProcessRush(int rushCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, bool byPassStorageCheck = false)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(rushCost);
			ProcessPremiumTransaction(list, items, showStoreOnFail, callback, "Rush", 0, true, byPassStorageCheck);
		}

		public void ProcessRush(int rushCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, string source, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, bool byPassStorageCheck = false)
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			list.Add(rushCost);
			ProcessPremiumTransaction(list, items, showStoreOnFail, callback, source, 0, true, byPassStorageCheck);
		}

		public void ProcessRush(global::System.Collections.Generic.IList<int> itemCostList, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, bool byPassStorageCheck = false)
		{
			ProcessPremiumTransaction(itemCostList, items, showStoreOnFail, callback, "Rush", 0, true, byPassStorageCheck);
		}

		public void ProcessPremiumTransaction(global::System.Collections.Generic.IList<int> rushCosts, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, bool showStoreOnFail, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, string source, int instanceId, bool reportItems, bool byPassStorageCheck = false)
		{
			if (!byPassStorageCheck && !CheckStorageCapacity(items, global::Kampai.Game.TransactionTarget.NO_VISUAL, null))
			{
				return;
			}
			int num = 0;
			foreach (int rushCost in rushCosts)
			{
				num += rushCost;
			}
			if (player.GetQuantity(global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID) >= num)
			{
				player.AlterQuantity(global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID, -num);
				if (items != null)
				{
					engine.AddOutputs(player, items);
				}
				if (items != null && reportItems)
				{
					for (int i = 0; i < items.Count; i++)
					{
						int num2 = num;
						if (rushCosts.Count - 1 >= i)
						{
							num2 = rushCosts[i];
						}
						if (items[i].ID == 0)
						{
							global::Kampai.Game.Transaction.TransactionUpdateData transactionUpdateData = createPremiumPurchaseTransactionData(items[i], num2, source, instanceId);
							transactionUpdateData.IsFromPremiumSource = true;
							transactionUpdateData.IsNotForPlayerTraining = true;
							postTransactionSignal.Dispatch(transactionUpdateData);
						}
						else
						{
							global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem> list = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
							list.Add(items[i]);
							SetAndSendUpdateData(source, instanceId, true, list, num2);
						}
					}
				}
				else
				{
					SetAndSendUpdateData(source, instanceId, false, items, num);
				}
				Success(callback, null, true, num, items, null);
			}
			else if (showStoreOnFail)
			{
				SendRushTelemetry(source, instanceId, num, items);
				insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(null, true, num, items, null, callback), false);
			}
			else
			{
				Fail(callback, null, true, num, items, null);
			}
		}

		private void SendRushTelemetry(string source, int instanceId, int rushCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items)
		{
			global::Kampai.Game.Transaction.TransactionUpdateData update = CreatePremiumPurchaseTransactionData(source, instanceId, true, items, rushCost);
			string highLevel = string.Empty;
			string specific = string.Empty;
			string type = string.Empty;
			string other = string.Empty;
			telemetryUtil.DetermineTaxonomy(update, true, out highLevel, out specific, out type, out other);
			string sourceName = telemetryUtil.GetSourceName(update);
			global::Kampai.Game.ItemDefinition itemDefinition = definitionService.Get<global::Kampai.Game.ItemDefinition>(1);
			telemetryService.Send_Telemetry_EVT_PINCH_PROMPT(sourceName, itemDefinition.LocalizedKey, rushCost, highLevel, specific, type, "null");
		}

		private void SetAndSendUpdateData(string source, int instanceId, bool isPremium, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, int totalRushCost)
		{
			global::Kampai.Game.Transaction.TransactionUpdateData param = CreatePremiumPurchaseTransactionData(source, instanceId, isPremium, items, totalRushCost);
			postTransactionSignal.Dispatch(param);
		}

		private global::Kampai.Game.Transaction.TransactionUpdateData CreatePremiumPurchaseTransactionData(string source, int instanceId, bool isPremium, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> items, int totalRushCost)
		{
			global::Kampai.Game.Transaction.TransactionUpdateData transactionUpdateData = new global::Kampai.Game.Transaction.TransactionUpdateData();
			transactionUpdateData.Type = global::Kampai.Game.Transaction.UpdateType.OTHER;
			transactionUpdateData.Source = source;
			transactionUpdateData.InstanceId = instanceId;
			transactionUpdateData.IsFromPremiumSource = isPremium;
			transactionUpdateData.IsNotForPlayerTraining = true;
			transactionUpdateData.Outputs = items;
			transactionUpdateData.AddInput(1, totalRushCost);
			return transactionUpdateData;
		}

		private global::Kampai.Game.Transaction.TransactionUpdateData createPremiumPurchaseTransactionData(global::Kampai.Util.QuantityItem output, int itemCost, string source, int instanceId)
		{
			global::Kampai.Game.Transaction.TransactionUpdateData transactionUpdateData = new global::Kampai.Game.Transaction.TransactionUpdateData();
			transactionUpdateData.Type = global::Kampai.Game.Transaction.UpdateType.OTHER;
			transactionUpdateData.Source = source;
			transactionUpdateData.InstanceId = instanceId;
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> list = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			list.Add(output);
			transactionUpdateData.Outputs = list;
			transactionUpdateData.AddInput(1, itemCost);
			return transactionUpdateData;
		}

		public bool IsMissingItemFromTransaction(global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition)
		{
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs = transactionDefinition.Inputs;
			if (inputs != null)
			{
				int count = inputs.Count;
				for (int i = 0; i < count; i++)
				{
					uint quantityByDefinitionId = GetQuantityByDefinitionId(inputs[i].ID);
					int num = (int)(inputs[i].Quantity - quantityByDefinitionId);
					if (num > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> GetMissingItemListFromTransaction(global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition)
		{
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> list = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs = transactionDefinition.Inputs;
			if (inputs != null)
			{
				int count = inputs.Count;
				for (int i = 0; i < count; i++)
				{
					global::Kampai.Util.QuantityItem quantityItem = null;
					uint quantityByDefinitionId = GetQuantityByDefinitionId(inputs[i].ID);
					int num = (int)(inputs[i].Quantity - quantityByDefinitionId);
					if (num > 0)
					{
						quantityItem = new global::Kampai.Util.QuantityItem(inputs[i].ID, (uint)num);
						list.Add(quantityItem);
					}
				}
			}
			return list;
		}

		private void SetTransactionTime(ref global::Kampai.Game.TransactionArg arg)
		{
			if (arg == null)
			{
				arg = new global::Kampai.Game.TransactionArg();
			}
			arg.TransactionUTCTime = timeService.CurrentTime();
		}

		public global::Kampai.Game.MinionParty GetMinionPartyInstance()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.MinionParty> instancesByType = GetInstancesByType<global::Kampai.Game.MinionParty>();
			if (instancesByType.Count == 0)
			{
				global::Kampai.Game.MinionPartyDefinition definition = definitionService.Get<global::Kampai.Game.MinionPartyDefinition>(80000);
				global::Kampai.Game.MinionParty minionParty = new global::Kampai.Game.MinionParty(definition);
				Add(minionParty);
				return minionParty;
			}
			return instancesByType[0];
		}

		public void AddXP(int xp)
		{
			AlterQuantity(global::Kampai.Game.StaticItem.XP_ID, xp);
			if (IsMinionPartyUnlocked())
			{
				UpdateMinionPartyPointValues();
			}
		}

		public void UpdateMinionPartyPointValues()
		{
			global::Kampai.Game.MinionParty minionPartyInstance = GetMinionPartyInstance();
			if (minionPartyInstance != null)
			{
				int quantity = (int)GetQuantity(global::Kampai.Game.StaticItem.LEVEL_PARTY_INDEX_ID);
				int quantity2 = (int)GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID);
				minionPartyInstance.CurrentPartyIndex = quantity;
				minionPartyInstance.TotalLevelPartiesCount = partyService.GetTotalParties(quantity2);
				minionPartyInstance.CurrentPartyPoints = GetQuantity(global::Kampai.Game.StaticItem.XP_ID);
				minionPartyInstance.CurrentPartyPointsRequired = partyService.GetTotalPartyPoints(quantity2, quantity);
			}
		}

		public bool VerifyTransaction(int transactionId)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactiondef = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionId);
			return VerifyTransaction(transactiondef);
		}

		public bool VerifyTransaction(global::Kampai.Game.Transaction.TransactionDefinition transactiondef)
		{
			return engine.ValidateInputs(player, transactiondef);
		}

		public void StartTransaction(int transactionId, global::Kampai.Game.TransactionTarget target, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, global::Kampai.Game.TransactionArg arg = null, int startTime = 0, int index = 0)
		{
			global::Kampai.Game.Transaction.TransactionDefinition td = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionId);
			StartTransaction(td, target, callback, arg, startTime, index);
		}

		public void StartTransaction(global::Kampai.Game.Transaction.TransactionDefinition td, global::Kampai.Game.TransactionTarget target, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, global::Kampai.Game.TransactionArg arg = null, int startTime = 0, int index = 0)
		{
			SetTransactionTime(ref arg);
			switch (target)
			{
			case global::Kampai.Game.TransactionTarget.INGREDIENT:
			case global::Kampai.Game.TransactionTarget.REPAIR_BRIDGE:
				if (!VerifyTransaction(td))
				{
					insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, true, 0, td.Inputs, null, callback, global::Kampai.Game.TransactionTarget.NO_VISUAL, arg), false);
					return;
				}
				break;
			case global::Kampai.Game.TransactionTarget.BLACKMARKETBOARD:
			{
				if (!VerifyTransaction(td))
				{
					insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, true, 0, td.Inputs, null, callback, global::Kampai.Game.TransactionTarget.NO_VISUAL, arg), false);
					return;
				}
				global::Kampai.Game.OrderBoard byInstanceId2 = GetByInstanceId<global::Kampai.Game.OrderBoard>(arg.InstanceId);
				foreach (global::Kampai.Game.OrderBoardTicket ticket in byInstanceId2.tickets)
				{
					if (ticket.BoardIndex == index)
					{
						ticket.StartTime = startTime;
						break;
					}
				}
				break;
			}
			case global::Kampai.Game.TransactionTarget.CLEAR_DEBRIS:
			{
				if (!VerifyTransaction(td))
				{
					insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, true, 0, td.Inputs, null, callback, target, arg), false);
					return;
				}
				global::Kampai.Game.DebrisBuilding byInstanceId = GetByInstanceId<global::Kampai.Game.DebrisBuilding>(arg.InstanceId);
				byInstanceId.PaidInputCostToClear = true;
				break;
			}
			}
			if (!engine.SubtractInputs(player, td))
			{
				insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, false, 0, null, null, callback, global::Kampai.Game.TransactionTarget.NO_VISUAL, arg), false);
				return;
			}
			SendTransactionUpdate(td, arg, global::Kampai.Game.Transaction.UpdateType.TRANSACTION_START, null, target);
			Success(callback, td, false, 0, null, null);
		}

		public bool FinishTransaction(int transactionId, global::Kampai.Game.TransactionTarget target, global::Kampai.Game.TransactionArg arg = null)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs;
			return FinishTransaction(transactionId, target, out outputs, arg);
		}

		public bool FinishTransaction(global::Kampai.Game.Transaction.TransactionDefinition td, global::Kampai.Game.TransactionTarget target, global::Kampai.Game.TransactionArg arg = null)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs;
			return FinishTransaction(td, target, out outputs, arg);
		}

		public bool FinishTransaction(int transactionId, global::Kampai.Game.TransactionTarget target, out global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs, global::Kampai.Game.TransactionArg arg = null)
		{
			global::Kampai.Game.Transaction.TransactionDefinition td = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionId);
			return FinishTransaction(td, target, out outputs, arg);
		}

		public bool FinishTransaction(global::Kampai.Game.Transaction.TransactionDefinition td, global::Kampai.Game.TransactionTarget target, out global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs, global::Kampai.Game.TransactionArg arg = null)
		{
			SetTransactionTime(ref arg);
			bool flag = false;
			outputs = null;
			if (!CheckStorageCapacity(td.Outputs, target, arg))
			{
				return false;
			}
			if (engine.AddOutputs(player, td, out outputs, arg))
			{
				SendTransactionUpdate(td, arg, global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FINISH, outputs, target);
				flag = true;
			}
			else
			{
				flag = false;
			}
			CheckRandomDrop(target, arg);
			return flag;
		}

		private int RandomDropIncrement(global::Kampai.Game.TransactionTarget target, global::Kampai.Game.TransactionArg arg = null)
		{
			int result = 0;
			switch (target)
			{
			case global::Kampai.Game.TransactionTarget.HARVEST:
				result = 1;
				break;
			case global::Kampai.Game.TransactionTarget.TASK_COMPLETE:
			case global::Kampai.Game.TransactionTarget.TASK_COMPLETE_INGREDIENT:
			{
				if (arg == null)
				{
					logger.LogNullArgument();
					break;
				}
				global::Kampai.Game.TaskTransactionArgument taskTransactionArgument = arg.Get<global::Kampai.Game.TaskTransactionArgument>();
				if (taskTransactionArgument == null)
				{
					logger.LogNullArgument();
				}
				else
				{
					result = taskTransactionArgument.DropStep;
				}
				break;
			}
			}
			return result;
		}

		private bool IsLastItemInStack(int instanceId)
		{
			global::Kampai.Game.Building byInstanceId = GetByInstanceId<global::Kampai.Game.Building>(instanceId);
			global::Kampai.Game.ResourceBuilding resourceBuilding = byInstanceId as global::Kampai.Game.ResourceBuilding;
			if (resourceBuilding != null)
			{
				return resourceBuilding.AvailableHarvest == 1;
			}
			global::Kampai.Game.CraftingBuilding craftingBuilding = byInstanceId as global::Kampai.Game.CraftingBuilding;
			if (craftingBuilding != null)
			{
				return craftingBuilding.CompletedCrafts.Count == 1;
			}
			return true;
		}

		private void CheckRandomDrop(global::Kampai.Game.TransactionTarget target, global::Kampai.Game.TransactionArg arg = null)
		{
			int num = RandomDropIncrement(target, arg);
			if (num <= 0)
			{
				return;
			}
			int instanceId = ((arg != null) ? arg.InstanceId : 0);
			AlterQuantity(global::Kampai.Game.StaticItem.ACTIONS_SINCE_LAST_DROP, num);
			global::Kampai.Game.DropLevelBandDefinition dropLevelBandDefinition = definitionService.Get<global::Kampai.Game.DropLevelBandDefinition>(88889);
			int value = (int)(GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID) - 1);
			value = global::UnityEngine.Mathf.Clamp(value, 0, dropLevelBandDefinition.HarvestsPerDrop.Count - 1);
			if (GetQuantity(global::Kampai.Game.StaticItem.ACTIONS_SINCE_LAST_DROP) >= dropLevelBandDefinition.HarvestsPerDrop[value] && (target != global::Kampai.Game.TransactionTarget.HARVEST || IsLastItemInStack(instanceId)))
			{
				player.SetQuantityByStaticItem(global::Kampai.Game.StaticItem.ACTIONS_SINCE_LAST_DROP, 0u);
				global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback = delegate(global::Kampai.Game.PendingCurrencyTransaction pct)
				{
					randomDropSignal.Dispatch(global::Kampai.Util.Tuple.Create(pct.GetOutputs()[0].Definition.ID, instanceId));
				};
				RunEntireTransaction(5037, global::Kampai.Game.TransactionTarget.NO_VISUAL, callback, arg);
			}
		}

		public void RunEntireTransaction(int transactionId, global::Kampai.Game.TransactionTarget target, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, global::Kampai.Game.TransactionArg arg = null)
		{
			global::Kampai.Game.Transaction.TransactionDefinition td = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionId);
			RunEntireTransaction(td, target, callback, arg);
		}

		public void RunEntireTransaction(global::Kampai.Game.Transaction.TransactionDefinition td, global::Kampai.Game.TransactionTarget target, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, global::Kampai.Game.TransactionArg arg = null)
		{
			SetTransactionTime(ref arg);
			bool isRush = false;
			global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs = null;
			switch (target)
			{
			case global::Kampai.Game.TransactionTarget.CURRENCY:
				if (RunTransaction(td, out outputs, arg))
				{
					SendTransactionUpdate(td, arg, global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FULL, outputs, target);
					CheckRandomDrop(target, arg);
					Success(callback, td, false, 0, null, outputs);
				}
				else
				{
					Fail(callback, td, false, 0, null, null);
					logger.Fatal(global::Kampai.Util.FatalCode.PS_UNABLE_TO_RUN_PENDING_TRANSACTION);
				}
				return;
			case global::Kampai.Game.TransactionTarget.STORAGEBUILDING:
				if (RunTransaction(td, out outputs, arg))
				{
					GetByInstanceId<global::Kampai.Game.StorageBuilding>(arg.InstanceId).CurrentStorageBuildingLevel++;
					SendTransactionUpdate(td, arg, global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FULL, outputs, target);
					CheckRandomDrop(target, arg);
					Success(callback, td, false, 0, null, outputs);
				}
				else
				{
					insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, false, 0, null, null, callback, global::Kampai.Game.TransactionTarget.NO_VISUAL, arg), false);
				}
				return;
			case global::Kampai.Game.TransactionTarget.INGREDIENT:
			case global::Kampai.Game.TransactionTarget.TASK_COMPLETE_INGREDIENT:
				isRush = true;
				break;
			case global::Kampai.Game.TransactionTarget.MARKETPLACE:
				if (!CheckStorageCapacity(td.Outputs, target, arg, true))
				{
					Fail(callback, td, true, 0, null, null, global::Kampai.Game.CurrencyTransactionFailReason.STORAGE);
				}
				else if (VerifyTransaction(td))
				{
					RunTransaction(td, out outputs, arg);
					SendTransactionUpdate(td, arg, global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FULL, outputs, target);
					CheckRandomDrop(target, arg);
					Success(callback, td, isRush, 0, null, outputs);
				}
				else
				{
					insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, isRush, 0, null, null, callback, global::Kampai.Game.TransactionTarget.NO_VISUAL, arg), false);
				}
				return;
			}
			if (VerifyTransaction(td))
			{
				RunTransaction(td, out outputs, arg);
				SendTransactionUpdate(td, arg, global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FULL, outputs, target);
				CheckRandomDrop(target, arg);
				Success(callback, td, isRush, 0, null, outputs);
			}
			else
			{
				insufficientInputsSignal.Dispatch(new global::Kampai.Game.PendingCurrencyTransaction(td, isRush, 0, null, null, callback, global::Kampai.Game.TransactionTarget.NO_VISUAL, arg), false);
			}
		}

		private void SendTransactionUpdate(global::Kampai.Game.Transaction.TransactionDefinition td, global::Kampai.Game.TransactionArg arg, global::Kampai.Game.Transaction.UpdateType type, global::System.Collections.Generic.IList<global::Kampai.Game.Instance> newItems, global::Kampai.Game.TransactionTarget target)
		{
			global::Kampai.Game.Transaction.TransactionUpdateData transactionUpdateData = new global::Kampai.Game.Transaction.TransactionUpdateData();
			transactionUpdateData.Type = type;
			transactionUpdateData.TransactionId = td.ID;
			transactionUpdateData.InstanceId = ((arg != null) ? arg.InstanceId : 0);
			transactionUpdateData.startPosition = ((arg != null) ? arg.StartPosition : global::UnityEngine.Vector3.zero);
			transactionUpdateData.fromGlass = arg != null && arg.fromGlass;
			transactionUpdateData.Source = ((arg != null) ? arg.Source : null);
			transactionUpdateData.NewItems = newItems;
			transactionUpdateData.IsFromPremiumSource = arg.IsFromPremiumSource;
			transactionUpdateData.Target = target;
			transactionUpdateData.IsNotForPlayerTraining = arg.IsFromQuestSource != 0;
			transactionUpdateData.craftableXPEarned = ((arg != null) ? arg.CraftableXPEarned : 0);
			if (arg.IsFromQuestSource == 1)
			{
				transactionUpdateData.Source = "QuestStep";
			}
			else if (arg.IsFromQuestSource == 2)
			{
				transactionUpdateData.Source = "TSMQuestStep";
			}
			else if (arg.IsFromQuestSource == 3)
			{
				transactionUpdateData.Source = "MasterPlanQuestStep";
			}
			switch (type)
			{
			case global::Kampai.Game.Transaction.UpdateType.TRANSACTION_START:
				transactionUpdateData.Inputs = td.Inputs;
				break;
			case global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FINISH:
				transactionUpdateData.Outputs = td.Outputs;
				break;
			case global::Kampai.Game.Transaction.UpdateType.TRANSACTION_FULL:
				transactionUpdateData.Inputs = td.Inputs;
				transactionUpdateData.Outputs = td.Outputs;
				break;
			}
			postTransactionSignal.Dispatch(transactionUpdateData);
		}

		public void StopTask(int minionId)
		{
			global::Kampai.Game.Minion byInstanceId = GetByInstanceId<global::Kampai.Game.Minion>(minionId);
			if (byInstanceId == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.PS_NO_SUCH_MINION, minionId);
			}
			global::Kampai.Game.TaskableBuilding byInstanceId2 = GetByInstanceId<global::Kampai.Game.TaskableBuilding>(byInstanceId.BuildingID);
			if (byInstanceId2 == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.PS_NO_SUCH_INSTANCE_TASKING, byInstanceId.BuildingID);
			}
			byInstanceId.BuildingID = -1;
			byInstanceId.UTCTaskStartTime = -1;
			byInstanceId.State = global::Kampai.Game.MinionState.Idle;
			byInstanceId.PartyTimeReduction = 0;
			byInstanceId2.RemoveMinion(minionId, timeService.CurrentTime());
			if (byInstanceId2.GetMinionsInBuilding() == 0)
			{
				changeState.Dispatch(byInstanceId2.ID, global::Kampai.Game.BuildingState.Idle);
			}
			byInstanceId2.StateStartTime = 0;
		}

		public void BuyCraftingSlot(int buildingID)
		{
			GetByInstanceId<global::Kampai.Game.CraftingBuilding>(buildingID).Slots++;
		}

		public void UpdateCraftingQueue(int buildingID, int itemDefId)
		{
			global::Kampai.Game.CraftingBuilding byInstanceId = GetByInstanceId<global::Kampai.Game.CraftingBuilding>(buildingID);
			byInstanceId.RecipeInQueue.Add(itemDefId);
		}

		public bool VerifyPlayerHasRequiredInputs(global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs)
		{
			bool result = true;
			foreach (global::Kampai.Util.QuantityItem input in inputs)
			{
				if (GetQuantityByDefinitionId(input.ID) < input.Quantity)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public void PurchaseSlotForBuilding(int buildingID, int level)
		{
			global::Kampai.Game.ResourceBuilding byInstanceId = GetByInstanceId<global::Kampai.Game.ResourceBuilding>(buildingID);
			int num = byInstanceId.BuildingNumber - 1;
			if (num < 0)
			{
				num = GetInstancesByDefinitionID(byInstanceId.Definition.ID).Count;
			}
			int count = byInstanceId.Definition.SlotUnlocks.Count;
			num = ((num <= count - 1) ? num : (count - 1));
			if (byInstanceId.MinionSlotsOwned < byInstanceId.Definition.SlotUnlocks[num].SlotUnlockLevels.Count && byInstanceId.Definition.SlotUnlocks[num].SlotUnlockLevels[byInstanceId.MinionSlotsOwned] == level)
			{
				byInstanceId.IncrementMinionSlotsOwned();
			}
		}

		public int GetMinionCount()
		{
			return GetInstancesByType<global::Kampai.Game.Minion>().Count;
		}

		private bool CheckStorageCapacity(global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> outputs, global::Kampai.Game.TransactionTarget target, global::Kampai.Game.TransactionArg arg, bool allowDrops = false)
		{
			if (target == global::Kampai.Game.TransactionTarget.AUTOMATIC)
			{
				return true;
			}
			uint num = 0u;
			global::Kampai.Game.CraftingBuilding craftingBuilding = null;
			bool flag = false;
			if (arg != null && arg.InstanceId != 0)
			{
				craftingBuilding = GetByInstanceId<global::Kampai.Game.CraftingBuilding>(arg.InstanceId);
				if (craftingBuilding != null)
				{
					flag = true;
				}
			}
			if (outputs != null)
			{
				foreach (global::Kampai.Util.QuantityItem output in outputs)
				{
					int iD = output.ID;
					if (iD != 0 && iD != 2 && iD != 1 && iD != 3)
					{
						global::Kampai.Game.ItemDefinition definition;
						definitionService.TryGet<global::Kampai.Game.ItemDefinition>(output.ID, out definition);
						if (definition != null && (allowDrops || !(definition is global::Kampai.Game.DropItemDefinition)) && !(definition is global::Kampai.Game.CostumeItemDefinition) && !(definition is global::Kampai.Game.PartyFavorAnimationItemDefinition))
						{
							num += output.Quantity;
						}
					}
				}
			}
			if (num == 0)
			{
				return true;
			}
			global::Kampai.Game.StorageBuilding storageBuilding = null;
			using (global::System.Collections.Generic.IEnumerator<global::Kampai.Game.StorageBuilding> enumerator2 = player.GetByDefinitionId<global::Kampai.Game.StorageBuilding>(3018).GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					global::Kampai.Game.StorageBuilding current2 = enumerator2.Current;
					storageBuilding = current2;
				}
			}
			if (storageBuilding == null)
			{
				return true;
			}
			uint currentStorageCapacity = GetCurrentStorageCapacity();
			uint itemCount = GetStorageCount();
			player.FindStorableItems(out itemCount);
			uint num2 = itemCount + num;
			if (num2 > currentStorageCapacity)
			{
				telemetryService.Send_Telemetry_EVT_STORAGE_LIMIT_HIT((int)currentStorageCapacity);
				if (target.Equals(global::Kampai.Game.TransactionTarget.HARVEST) && flag && craftingBuilding != null)
				{
					showCraftingBuildingMenuSignal.Dispatch(craftingBuilding);
				}
				else if (!target.Equals(global::Kampai.Game.TransactionTarget.MARKETPLACE))
				{
					openStorageBuildingSignal.Dispatch(storageBuilding, true);
				}
				return false;
			}
			return true;
		}

		public void CreateAndRunCustomTransaction(int defID, int quantity, global::Kampai.Game.TransactionTarget target, global::Kampai.Game.TransactionArg args = null)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
			global::Kampai.Util.QuantityItem item = new global::Kampai.Util.QuantityItem(defID, (uint)quantity);
			transactionDefinition.Inputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			transactionDefinition.Outputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			transactionDefinition.Outputs.Add(item);
			transactionDefinition.ID = int.MaxValue;
			RunEntireTransaction(transactionDefinition, target, null, args);
		}

		public int GetInvestmentTimeForTransaction(int transactionID)
		{
			int num = 0;
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(transactionID);
			if (transactionDefinition == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.PS_NO_TRANSACTION, transactionID);
				return 0;
			}
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs = transactionDefinition.Inputs;
			if (inputs != null)
			{
				foreach (global::Kampai.Util.QuantityItem item in inputs)
				{
					global::Kampai.Game.IngredientsItemDefinition ingredientsItemDefinition = definitionService.Get<global::Kampai.Game.IngredientsItemDefinition>(item.ID);
					int quantity = (int)item.Quantity;
					num += IngredientsItemUtil.GetHarvestTimeFromIngredientDefinition(ingredientsItemDefinition, definitionService) * quantity;
					int tier = ingredientsItemDefinition.Tier;
					if (tier > 0)
					{
						num += GetInvestmentTimeForTransaction(ingredientsItemDefinition.TransactionId);
					}
				}
			}
			return num;
		}

		public global::Kampai.Game.KampaiPendingTransaction GetPendingTransaction(string externalIdentifier)
		{
			return player.GetPendingTransaction(externalIdentifier);
		}

		public bool PlayerAlreadyHasPlatformStoreTransactionID(string identifier)
		{
			return player.HasPlatformStoreTransactionID(identifier);
		}

		public void AddPlatformStoreTransactionID(string identifier)
		{
			player.AddPlatformStoreTransactionID(identifier);
		}

		public void QueuePendingTransaction(global::Kampai.Game.KampaiPendingTransaction pendingTransaction)
		{
			logger.Debug("QUEUE: {0}", pendingTransaction);
			if (player.GetPendingTransaction(pendingTransaction.ExternalIdentifier) == null)
			{
				player.QueuePendingTransaction(pendingTransaction);
			}
			else
			{
				logger.Fatal(global::Kampai.Util.FatalCode.PS_DUPLICATE_PENDING_TRANSACTION);
			}
		}

		public global::Kampai.Game.KampaiPendingTransaction ProcessPendingTransaction(string externalIdentifier, bool isFromPremium, global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback = null)
		{
			logger.Debug("PROCESS: {0}", externalIdentifier);
			global::Kampai.Game.KampaiPendingTransaction pendingTransaction = player.GetPendingTransaction(externalIdentifier);
			if (pendingTransaction != null)
			{
				global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = pendingTransaction.TransactionInstance.ToDefinition();
				global::Kampai.Game.TransactionTarget target = global::Kampai.Game.TransactionTarget.CURRENCY;
				if (pendingTransaction.StoreItemDefinitionId == 50002)
				{
					target = global::Kampai.Game.TransactionTarget.LAND_EXPANSION;
				}
				int grindOutputForTransaction = global::Kampai.Game.Transaction.TransactionUtil.GetGrindOutputForTransaction(transactionDefinition);
				int premiumOutputForTransaction = global::Kampai.Game.Transaction.TransactionUtil.GetPremiumOutputForTransaction(transactionDefinition);
				global::Kampai.Game.TransactionArg transactionArg = new global::Kampai.Game.TransactionArg();
				if (grindOutputForTransaction > 0 || premiumOutputForTransaction > 0)
				{
					transactionArg.Source = "STORE";
					transactionArg.IsFromPremiumSource = isFromPremium;
				}
				RunEntireTransaction(transactionDefinition, target, callback, transactionArg);
				player.RemovePendingTransaction(pendingTransaction);
				return pendingTransaction;
			}
			return null;
		}

		public global::Kampai.Game.KampaiPendingTransaction CancelPendingTransaction(string externalIdentifier)
		{
			global::Kampai.Game.KampaiPendingTransaction pendingTransaction = player.GetPendingTransaction(externalIdentifier);
			if (pendingTransaction != null)
			{
				player.RemovePendingTransaction(pendingTransaction);
			}
			return pendingTransaction;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.KampaiPendingTransaction> GetPendingTransactions()
		{
			return player.GetPendingTransactions();
		}

		private void Success(global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, global::Kampai.Game.Transaction.TransactionDefinition pendingTransaction, bool isRush, int rushCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> rushOutputs, global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs)
		{
			if (callback != null)
			{
				global::Kampai.Game.PendingCurrencyTransaction pendingCurrencyTransaction = new global::Kampai.Game.PendingCurrencyTransaction(pendingTransaction, isRush, rushCost, rushOutputs, outputs);
				pendingCurrencyTransaction.Success = true;
				callback(pendingCurrencyTransaction);
			}
		}

		private void Fail(global::System.Action<global::Kampai.Game.PendingCurrencyTransaction> callback, global::Kampai.Game.Transaction.TransactionDefinition pendingTransaction, bool isRush, int rushCost, global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> rushOutputs, global::System.Collections.Generic.IList<global::Kampai.Game.Instance> outputs, global::Kampai.Game.CurrencyTransactionFailReason failReason = global::Kampai.Game.CurrencyTransactionFailReason.NONE)
		{
			if (callback != null)
			{
				global::Kampai.Game.PendingCurrencyTransaction pendingCurrencyTransaction = new global::Kampai.Game.PendingCurrencyTransaction(pendingTransaction, isRush, rushCost, rushOutputs, outputs);
				pendingCurrencyTransaction.FailReason = failReason;
				callback(pendingCurrencyTransaction);
			}
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Building> GetBuildingsWithoutState(global::Kampai.Game.BuildingState excludedState)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Building> result = new global::System.Collections.Generic.List<global::Kampai.Game.Building>();
			player.GetInstancesByType(ref result, (global::Kampai.Game.Building building) => building.State != excludedState);
			return result;
		}

		public void RemoveTrigger(global::Kampai.Game.Trigger.TriggerInstance triggerInstance)
		{
			logger.Info("Removing trigger instance id {0}", triggerInstance.ID);
			player.RemoveTrigger(triggerInstance.ID);
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> GetTriggers()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerInstance> triggers = player.GetTriggers();
			if (triggers == null || triggers.Count == 0)
			{
				return new global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerInstance>();
			}
			triggers.Sort();
			return triggers;
		}

		public global::Kampai.Game.Trigger.TriggerInstance AddTrigger(global::Kampai.Game.Trigger.TriggerDefinition triggerDefinition)
		{
			if (triggerDefinition == null)
			{
				logger.Error("Can't add null trigger definition");
				return null;
			}
			global::Kampai.Game.Trigger.TriggerInstance triggerInstance = triggerDefinition.Build();
			player.Add(triggerInstance);
			return triggerInstance;
		}

		public bool HasTrigger(int triggerId)
		{
			return player.GetTriggerByDefinitionId(triggerId) != null;
		}

		public global::Kampai.Game.Trigger.TriggerInstance GetTriggerByDefinitionId(int defId)
		{
			return player.GetTriggerByDefinitionId(defId);
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Building> GetBuildingsWithState(global::Kampai.Game.BuildingState state)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Building> result = new global::System.Collections.Generic.List<global::Kampai.Game.Building>();
			player.GetInstancesByType(ref result, (global::Kampai.Game.Building building) => building.State == state);
			return result;
		}

		public void AddLandExpansion(global::Kampai.Game.LandExpansionConfig expansionConfig)
		{
			player.AddLandExpansion(expansionConfig);
		}

		public bool IsExpansionPurchased(int expansionId)
		{
			return player.IsExpansionPurchased(expansionId);
		}

		public int GetPurchasedExpansionCount()
		{
			return player.GetPurchasedExpansionCount();
		}

		public void QueueVillain(global::Kampai.Game.Prestige villainPrestige)
		{
			player.QueueVillain(villainPrestige);
		}

		public int PopVillain()
		{
			return player.PopVillain();
		}

		public void SetTargetExpansion(int id)
		{
			player.targetExpansionID = id;
		}

		public int GetTargetExpansion()
		{
			return player.targetExpansionID;
		}

		public void ClearTargetExpansion()
		{
			player.targetExpansionID = 0;
		}

		public bool HasTargetExpansion()
		{
			return player.targetExpansionID != 0;
		}

		public bool HasStorageBuilding()
		{
			global::Kampai.Game.StorageBuilding firstInstanceByDefintion = player.GetFirstInstanceByDefintion<global::Kampai.Game.StorageBuilding, global::Kampai.Game.StorageBuildingDefinition>();
			global::Kampai.Game.BuildingState state = firstInstanceByDefintion.State;
			return state != global::Kampai.Game.BuildingState.Broken && state != global::Kampai.Game.BuildingState.Inaccessible && state != global::Kampai.Game.BuildingState.Disabled;
		}

		public uint GetStorageCount()
		{
			uint itemCount = 0u;
			player.FindStorableItems(out itemCount);
			return itemCount;
		}

		public bool isStorageFull()
		{
			if (GetStorageCount() >= GetCurrentStorageCapacity())
			{
				return true;
			}
			return false;
		}

		public uint GetAvailableStorageCapacity()
		{
			return GetCurrentStorageCapacity() - GetStorageCount();
		}

		public uint GetCurrentStorageCapacity()
		{
			global::Kampai.Game.StorageBuilding firstInstanceByDefintion = player.GetFirstInstanceByDefintion<global::Kampai.Game.StorageBuilding, global::Kampai.Game.StorageBuildingDefinition>();
			if (firstInstanceByDefintion == null)
			{
				return 0u;
			}
			uint quantity = player.GetQuantity(global::Kampai.Game.StaticItem.STORAGE_ADDITIONAL_CAPACITY_ID);
			uint storageCapacity = firstInstanceByDefintion.Definition.StorageUpgrades[firstInstanceByDefintion.CurrentStorageBuildingLevel].StorageCapacity;
			return storageCapacity + quantity;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Minion> GetMinions(bool has, params global::Kampai.Game.MinionState[] states)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> list = new global::System.Collections.Generic.List<global::Kampai.Game.Minion>();
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			for (int i = 0; i < instancesByType.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < states.Length; j++)
				{
					if (instancesByType[i].State == states[j])
					{
						flag = true;
						break;
					}
				}
				if ((flag && has) || (!flag && !has))
				{
					list.Add(instancesByType[i]);
				}
			}
			return list;
		}

		public int GetMinionCountByLevel(int level)
		{
			int num = 0;
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.Level == level && !item.HasPrestige)
				{
					num++;
				}
			}
			return num;
		}

		public int GetMinionCountAtOrAboveLevel(int level)
		{
			int num = 0;
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.Level >= level && !item.HasPrestige)
				{
					num++;
				}
			}
			return num;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Minion> GetMinionsByLevel(int level)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> list = new global::System.Collections.Generic.List<global::Kampai.Game.Minion>();
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.Level == level && !item.HasPrestige)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Minion> GetMinionsSortedByLevel()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			instancesByType.Sort((global::Kampai.Game.Minion x, global::Kampai.Game.Minion y) => y.Level.CompareTo(x.Level));
			return instancesByType;
		}

		public int GetHighestUntaskedMinionLevel()
		{
			int num = 0;
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.State == global::Kampai.Game.MinionState.Idle && item.Level > num)
				{
					num = item.Level;
				}
			}
			return num;
		}

		public int GetHighestMinionForLeisure(int requiredMinionCount)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> list = new global::System.Collections.Generic.List<global::Kampai.Game.Minion>();
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.State == global::Kampai.Game.MinionState.Idle)
				{
					list.Add(item);
				}
			}
			if (list.Count < requiredMinionCount)
			{
				return 0;
			}
			list.Sort((global::Kampai.Game.Minion x, global::Kampai.Game.Minion y) => x.Level.CompareTo(y.Level));
			return list[requiredMinionCount - 1].Level;
		}

		public global::Kampai.Game.Minion GetUntaskedMinionWithHighestLevel()
		{
			global::Kampai.Game.MinionBenefitLevelBandDefintion minionBenefitLevelBandDefintion = definitionService.Get<global::Kampai.Game.MinionBenefitLevelBandDefintion>(89898);
			int num = minionBenefitLevelBandDefintion.minionBenefitLevelBands.Count - 1;
			global::Kampai.Game.Minion result = null;
			int num2 = -1;
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.State == global::Kampai.Game.MinionState.Idle && item.Level > num2)
				{
					result = item;
					if (item.Level == num)
					{
						break;
					}
					num2 = item.Level;
				}
			}
			return result;
		}

		public void LevelupMinion(int instanceID)
		{
			player.GetByInstanceId<global::Kampai.Game.Minion>(instanceID).Level++;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Minion> GetIdleMinions()
		{
			return GetMinions(true, global::Kampai.Game.MinionState.Idle, global::Kampai.Game.MinionState.Selectable, global::Kampai.Game.MinionState.Uninitialized);
		}

		public void IncreaseCompletedOrders()
		{
			player.completedOrders++;
		}

		public void IncreaseCompletedQuests()
		{
			player.completedQuestsTotal++;
		}

		public int GetHighestFtueCompleted()
		{
			return player.HighestFtueLevel;
		}

		public void SetHighestFtueCompleted(int newLevel)
		{
			int highestFtueLevel = player.HighestFtueLevel;
			if (newLevel > highestFtueLevel)
			{
				player.HighestFtueLevel = newLevel;
				ftueLevelChangedSignal.Dispatch();
			}
			else if (newLevel < highestFtueLevel)
			{
				logger.Warning("New FTUE level lower than current {0} -> {1}", highestFtueLevel, newLevel);
			}
		}

		public int GetInventoryCountByDefinitionID(int defId)
		{
			int capacity = GetUnlockedQuantityOfID(defId);
			int boardCount = 0;
			GetBuildingOnBoardCountMap().TryGetValue(defId, out boardCount);
			int maxAllowedInventory = global::System.Math.Max(0, capacity - boardCount);

			global::System.Collections.Generic.ICollection<global::Kampai.Game.Instance> byDefinitionId = GetByDefinitionId<global::Kampai.Game.Instance>(defId);
			global::System.Collections.Generic.List<global::Kampai.Game.Instance> inventoryItems = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
			if (byDefinitionId.Count != 0)
			{
				foreach (global::Kampai.Game.Instance item in byDefinitionId)
				{
					global::Kampai.Game.Building building = item as global::Kampai.Game.Building;
					if (building != null && building.State == global::Kampai.Game.BuildingState.Inventory)
					{
						inventoryItems.Add(building);
					}
				}
			}

			if (capacity >= 0 && inventoryItems.Count > maxAllowedInventory)
			{
				int excessCount = inventoryItems.Count - maxAllowedInventory;
				for (int i = 0; i < excessCount; i++)
				{
					Remove(inventoryItems[i]);
				}
				return maxAllowedInventory;
			}

			return inventoryItems.Count;
		}

		public bool CheckIfBuildingIsCapped(int defID)
		{
			int unlockedQuantityOfID = GetUnlockedQuantityOfID(defID);
			if (unlockedQuantityOfID < 0)
			{
				return false;
			}
			int num = 0;
			global::System.Collections.Generic.ICollection<global::Kampai.Game.Building> byDefinitionId = GetByDefinitionId<global::Kampai.Game.Building>(defID);
			if (byDefinitionId.Count != 0)
			{
				foreach (global::Kampai.Game.Building item in byDefinitionId)
				{
					if (item.State != global::Kampai.Game.BuildingState.Inventory)
					{
						num++;
					}
				}
			}
			return num >= unlockedQuantityOfID;
		}

		public global::Kampai.Game.SocialClaimRewardItem.ClaimState GetSocialClaimReward(int eventID)
		{
			if (player == null)
			{
				logger.Warning("Failed to get claim reward state for event {0}", eventID);
				return global::Kampai.Game.SocialClaimRewardItem.ClaimState.UNKNOWN;
			}
			global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState> socialClaimRewards = player.GetSocialClaimRewards();
			if (socialClaimRewards.ContainsKey(eventID))
			{
				return socialClaimRewards[eventID];
			}
			return global::Kampai.Game.SocialClaimRewardItem.ClaimState.UNKNOWN;
		}

		public void AddSocialClaimReward(int eventID, global::Kampai.Game.SocialClaimRewardItem.ClaimState claimState)
		{
			if (player == null)
			{
				logger.Warning("Failed to update claim reward state for event {0}", eventID);
			}
			else
			{
				player.AddSocialClaimRewards(eventID, claimState);
			}
		}

		public void CleanupSocialClaimReward(global::System.Collections.Generic.List<int> recentEventIDs)
		{
			if (player == null)
			{
				logger.Warning("Failed to clean up claim reward state for past events");
			}
			else
			{
				player.CleanupSocialClaimReward(recentEventIDs);
			}
		}

		public void TrackMTXPurchase(string SKU)
		{
			player.AddMTXPurchaseTracking(SKU);
		}

		public global::System.Collections.Generic.IList<string> GetMTXPurchaseTracking()
		{
			return player.GetMTXPurchaseTracking();
		}

		public int MTXPurchaseCount(string sku)
		{
			return player.MTXPurchaseCount(sku);
		}

		public global::Kampai.Game.Player.SanityCheckFailureReason DeepScan(global::Kampai.Game.Player prevSave)
		{
			return player.ValidateSaveData(prevSave);
		}

		public void addPendingRemption(global::Kampai.Game.Mtx.ReceiptValidationResult validationResult)
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Debug, "Adding redemption validation result, SKU:" + validationResult.sku);
			player.addPendingRedemption(validationResult);
		}

		public global::Kampai.Game.Mtx.ReceiptValidationResult popPendingRedemption()
		{
			return player.popPendingRedemption();
		}

		public global::Kampai.Game.Mtx.ReceiptValidationResult topPendingRedemption()
		{
			return player.topPendingRedemption();
		}

		public void AddSocialInvitationSeen(long invitationId)
		{
			player.addSocialInvitationSeen(invitationId);
		}

		public bool SeenSocialInvitation(long invitationId)
		{
			return player.seenSocialInvitiation(invitationId);
		}

		public int getAndIncrementRequestCounter()
		{
			return player.getAndIncrementRequestCounter();
		}

		public void NotifyBuildMenuNewBuilding(int buildingID)
		{
			sendBuildingToInventorySignal.Dispatch(buildingID);
		}

		public void GrantInputs(global::Kampai.Game.Transaction.TransactionDefinition transaction)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
			transactionDefinition.ID = int.MaxValue;
			transactionDefinition.Outputs = transaction.Inputs;
			RunEntireTransaction(transactionDefinition, global::Kampai.Game.TransactionTarget.NO_VISUAL, null);
		}

		public void TrackHelpTipShown(int tipDefinitionId, int time)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Player.HelpTipTrackingItem> list = player.helpTipsTrackingData;
			if (list == null)
			{
				list = new global::System.Collections.Generic.List<global::Kampai.Game.Player.HelpTipTrackingItem>(1);
				player.helpTipsTrackingData = list;
			}
			for (int i = 0; i < list.Count; i++)
			{
				global::Kampai.Game.Player.HelpTipTrackingItem value = list[i];
				if (value.tipDifinitionId == tipDefinitionId)
				{
					value.showsCount++;
					value.lastShownTime = time;
					list[i] = value;
					return;
				}
			}
			list.Add(new global::Kampai.Game.Player.HelpTipTrackingItem(tipDefinitionId, time));
		}

		public void IngestPlayerMeta(PlayerMetaData meta)
		{
			if (meta == null)
			{
				return;
			}
			SetQuantity(global::Kampai.Game.StaticItem.TOTAL_USD, meta.USD);
			if (!string.IsNullOrEmpty(meta.Segments))
			{
				segments = new global::System.Collections.Generic.HashSet<string>();
				string[] array = meta.Segments.Split(',');
				foreach (string text in array)
				{
					string text2 = text.Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						segments.Add(text2.ToLower());
					}
				}
			}
			else
			{
				segments = null;
			}
			string churn = meta.Churn;
			if (string.IsNullOrEmpty(churn))
			{
				return;
			}
			float result = 0f;
			if (float.TryParse(churn, out result))
			{
				if (result < 0f)
				{
					result = 0f;
				}
				if (result > 1f)
				{
					result = 1f;
				}
				float num = Churn();
				float num2 = result - num + 1f;
				int amount = (int)(result * 10000f);
				int amount2 = (int)(num2 * 10000f);
				SetQuantity(global::Kampai.Game.StaticItem.CHURN_ID, amount);
				SetQuantity(global::Kampai.Game.StaticItem.CHURN_DELTA_ID, amount2);
			}
			else
			{
				logger.Error("Invalid churn {0}", churn);
			}
		}

		public bool IsInSegment(string segmentName)
		{
			if (segments != null && !string.IsNullOrEmpty(segmentName))
			{
				return segments.Contains(segmentName.ToLower());
			}
			return false;
		}

		public float Churn()
		{
			uint quantity = GetQuantity(global::Kampai.Game.StaticItem.CHURN_ID);
			return (float)quantity / 10000f;
		}
	}
}
