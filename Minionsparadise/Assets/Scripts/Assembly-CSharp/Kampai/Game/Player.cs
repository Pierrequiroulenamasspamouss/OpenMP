namespace Kampai.Game
{
	public class Player
	{
		public enum SanityCheckFailureReason
		{
			Passed = 0,
			OrderBoardTicketCount = 1,
			OrderBoardTickerInputsOutputs = 2,
			OrderBoardMissing = 3,
			TSMOutputs = 4,
			BuildingCount = 5,
			RequiredBuilding = 6,
			MignetteMissing = 7,
			MinionCount = 8,
			CostumedMinion = 9,
			NamedMinion = 10,
			LandExpansionEmpty = 11,
			PurchasedLandExpansionMissing = 12,
			BuildingMissingLocation = 13
		}

		public struct HelpTipTrackingItem
		{
			public int tipDifinitionId;

			public int showsCount;

			public int lastShownTime;

			public HelpTipTrackingItem(int defId, int time)
			{
				tipDifinitionId = defId;
				showsCount = 1;
				lastShownTime = time;
			}
		}

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Instance> byInstanceId = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Instance>();

		protected global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Kampai.Game.Instance>> byDefinitionId = new global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Kampai.Game.Instance>>();

		protected global::System.Collections.Generic.List<global::Kampai.Game.KampaiPendingTransaction> pendingTransactions = new global::System.Collections.Generic.List<global::Kampai.Game.KampaiPendingTransaction>();

		protected global::System.Collections.Generic.IDictionary<int, int> unlocks = new global::System.Collections.Generic.Dictionary<int, int>();

		protected global::System.Collections.Generic.IDictionary<int, int> purchasedUpsells = new global::System.Collections.Generic.Dictionary<int, int>();

		protected global::System.Collections.Generic.IList<int> villainQueue = new global::System.Collections.Generic.List<int>();

		protected global::System.Collections.Generic.IList<string> PlatformStoreTransactionIDs = new global::System.Collections.Generic.List<string>();

		protected global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState> socialRewards = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState>();

		protected global::System.Collections.Generic.IList<string> mtxPurchaseTracking = new global::System.Collections.Generic.List<string>();

		protected global::System.Collections.Generic.IList<string> completedQuests = new global::System.Collections.Generic.List<string>();

		protected global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.Trigger.TriggerInstance> triggers = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Trigger.TriggerInstance>();

		protected global::System.Collections.Generic.IList<global::Kampai.Game.Mtx.ReceiptValidationResult> pendingRedemptions = new global::System.Collections.Generic.List<global::Kampai.Game.Mtx.ReceiptValidationResult>();

		public int nextId = 1000;

		public int Version { get; set; }

		public global::Kampai.Util.IKampaiLogger logger { get; set; }

		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		public long ID { get; set; }

		public int HighestFtueLevel { get; set; }

		public int NextId
		{
			get
			{
				return nextId;
			}
		}

		public int lastLevelUpTime { get; set; }

		public int lastGameStartTime { get; set; }

		public int firstGameStartTime { get; set; }

		public int lastPlayedTime { get; set; }

		public int totalGameplayDurationSinceLastLevelUp { get; set; }

		public int totalAccumulatedGameplayDuration { get; set; }

		public int targetExpansionID { get; set; }

		public int timezoneOffset { get; set; }

		public string country { get; set; }

		public int completedOrders { get; set; }

		public int completedQuestsTotal { get; set; }

		public global::System.Collections.Generic.IList<long> socialTeamInvitationsSeen { get; set; }

		public int requestCounter { get; set; }

		public global::System.Collections.Generic.List<global::Kampai.Game.Player.HelpTipTrackingItem> helpTipsTrackingData { get; set; }

		public Player(global::Kampai.Game.IDefinitionService defService, global::Kampai.Util.IKampaiLogger log)
		{
			definitionService = defService;
			logger = log;
		}

		public uint GetQuantity(global::Kampai.Game.StaticItem def)
		{
			return GetQuantityByDefinitionId((int)def);
		}

		public uint GetQuantityByDefinitionId(int defId)
		{
			global::Kampai.Game.Item itemIfExistsByDefId = GetItemIfExistsByDefId(defId);
			return (itemIfExistsByDefId != null) ? itemIfExistsByDefId.Quantity : 0u;
		}

		public int GetCountByDefinitionId(int defId)
		{
			if (byDefinitionId.ContainsKey(defId))
			{
				return byDefinitionId[defId].Count;
			}
			return 0;
		}

		public uint GetTotalCountByDefinitionId(int defId)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Instance> value;
			if (!byDefinitionId.TryGetValue(defId, out value))
			{
				return 0u;
			}
			uint num = 0u;
			for (int i = 0; i < value.Count; i++)
			{
				global::Kampai.Game.Instance instance = value[i];
				global::Kampai.Game.Item item = instance as global::Kampai.Game.Item;
				num += ((item == null) ? 1 : item.Quantity);
			}
			return num;
		}

		public uint GetQuantityByInstanceId(int instanceId)
		{
			global::Kampai.Game.Item itemIfExistsByInstanceId = GetItemIfExistsByInstanceId(instanceId);
			return (itemIfExistsByInstanceId != null) ? itemIfExistsByInstanceId.Quantity : 0u;
		}

		public global::System.Collections.Generic.ICollection<int> FindAllAnimatingBuildingIDs()
		{
			global::System.Collections.Generic.LinkedList<int> linkedList = new global::System.Collections.Generic.LinkedList<int>();
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				if (value.Definition is global::Kampai.Game.AnimatingBuildingDefinition)
				{
					linkedList.AddLast(((global::Kampai.Game.Building)value).ID);
				}
			}
			return linkedList;
		}

		public global::System.Collections.Generic.ICollection<global::Kampai.Game.Item> FindStorableItems(out uint itemCount)
		{
			global::System.Collections.Generic.LinkedList<global::Kampai.Game.Item> linkedList = new global::System.Collections.Generic.LinkedList<global::Kampai.Game.Item>();
			itemCount = 0u;
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				global::Kampai.Game.Item item = value as global::Kampai.Game.Item;
				if (item != null && item.Definition.Storable && item.Quantity != 0)
				{
					itemCount += item.Quantity;
					linkedList.AddLast(item);
				}
			}
			return linkedList;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Item> GetSellableItems(out uint totalStorableQuantity, out uint totalSellableQuantity)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Item> list = new global::System.Collections.Generic.List<global::Kampai.Game.Item>();
			totalStorableQuantity = 0u;
			totalSellableQuantity = 0u;
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				global::Kampai.Game.Item item = value as global::Kampai.Game.Item;
				if (item == null)
				{
					continue;
				}
				global::Kampai.Game.ItemDefinition definition = item.Definition;
				if (item.Quantity != 0 && (definition.Storable || definition.SellableForced))
				{
					if (definition.Storable)
					{
						totalStorableQuantity += item.Quantity;
					}
					totalSellableQuantity += item.Quantity;
					list.Add(item);
				}
			}
			return list;
		}

		public global::System.Collections.Generic.IList<T> FindAllUnLockedByType<T>() where T : global::Kampai.Game.Definition
		{
			global::System.Collections.Generic.IList<T> list = new global::System.Collections.Generic.List<T>();
			foreach (global::System.Collections.Generic.KeyValuePair<int, int> unlock in unlocks)
			{
				T definition = (T)null;
				if (definitionService.TryGet<T>(unlock.Key, out definition))
				{
					list.Add(definition);
				}
			}
			return list;
		}

		public global::System.Collections.Generic.IList<T> GetByDefinitionId<T>(int id) where T : global::Kampai.Game.Instance
		{
			if (byDefinitionId.ContainsKey(id))
			{
				global::System.Collections.Generic.List<T> list = new global::System.Collections.Generic.List<T>();
				{
					foreach (global::Kampai.Game.Instance item in byDefinitionId[id])
					{
						if (item is T)
						{
							list.Add((T)item);
						}
					}
					return list;
				}
			}
			return new global::System.Collections.Generic.List<T>();
		}

		public T GetFirstInstanceByDefinitionId<T>(int definitionId) where T : global::Kampai.Game.Instance
		{
			if (byDefinitionId.ContainsKey(definitionId) && byDefinitionId[definitionId].Count > 0)
			{
				return (T)byDefinitionId[definitionId][0];
			}
			return default(T);
		}

		public I GetFirstInstanceByDefintion<I, D>() where I : class, global::Kampai.Game.Instance where D : global::Kampai.Game.Definition
		{
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				D val = value.Definition as D;
				if (val != null)
				{
					I val2 = value as I;
					if (val2 != null)
					{
						return val2;
					}
				}
			}
			return (I)null;
		}

		public T GetByInstanceId<T>(int id) where T : class, global::Kampai.Game.Instance
		{
			global::Kampai.Game.Instance value;
			if (byInstanceId.TryGetValue(id, out value))
			{
				return value as T;
			}
			return (T)null;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Instance> GetInstancesByDefinition<T>() where T : global::Kampai.Game.Definition
		{
			global::System.Type typeFromHandle = typeof(T);
			global::System.Collections.Generic.List<global::Kampai.Game.Instance> list = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				global::System.Type type = value.Definition.GetType();
				if (typeFromHandle.IsAssignableFrom(type))
				{
					list.Add(value);
				}
			}
			return list;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Instance> GetInstancesByDefinitionID(int defId)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Instance> list = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				if (value.Definition.ID == defId)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public void GetInstancesByType<T>(ref global::System.Collections.Generic.List<T> result, global::System.Predicate<T> condition = null) where T : class, global::Kampai.Game.Instance
		{
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Instance>.Enumerator enumerator = byInstanceId.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					global::Kampai.Game.Instance value = enumerator.Current.Value;
					T val = value as T;
					if (val != null && (condition == null || condition(val)))
					{
						result.Add(val);
					}
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		public global::System.Collections.Generic.List<T> GetInstancesByType<T>(global::System.Predicate<T> condition = null) where T : class, global::Kampai.Game.Instance
		{
			global::System.Collections.Generic.List<T> result = new global::System.Collections.Generic.List<T>();
			GetInstancesByType(ref result, condition);
			return result;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Item> GetItemsByDefinition<T>() where T : global::Kampai.Game.Definition
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Item> list = new global::System.Collections.Generic.List<global::Kampai.Game.Item>();
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				T val = value.Definition as T;
				if (val != null)
				{
					global::Kampai.Game.Item item = value as global::Kampai.Game.Item;
					if (item != null)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.Instance> GetInstancesByDefinition()
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Instance> list = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
			foreach (global::Kampai.Game.Instance value in byInstanceId.Values)
			{
				list.Add(value);
			}
			return list;
		}

		public global::Kampai.Game.Trigger.TriggerInstance GetTriggerByDefinitionId(int defId)
		{
			object result;
			if (triggers.ContainsKey(defId))
			{
				global::Kampai.Game.Trigger.TriggerInstance triggerInstance = triggers[defId];
				result = triggerInstance;
			}
			else
			{
				result = null;
			}
			return (global::Kampai.Game.Trigger.TriggerInstance)result;
		}

		public global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerInstance> GetTriggers()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerInstance> list = new global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerInstance>();
			foreach (global::Kampai.Game.Trigger.TriggerInstance value in triggers.Values)
			{
				list.Add(value);
			}
			return list;
		}

		protected bool AssertNewTriggerId(int instanceId)
		{
			if (instanceId < 0 || triggers.ContainsKey(instanceId))
			{
				return false;
			}
			return true;
		}

		public void Add(global::Kampai.Game.Trigger.TriggerInstance i)
		{
			if (i == null)
			{
				logger.Error("Unable to add null trigger to triggers list.");
			}
			else if (i.Definition == null)
			{
				logger.Warning("Illegal Trigger (null definition).");
			}
			else if (AssertNewTriggerId(i.ID))
			{
				AddNewTrigger(i);
			}
		}

		public void RemoveTrigger(int triggerId)
		{
			if (triggers.ContainsKey(triggerId))
			{
				triggers.Remove(triggerId);
			}
		}

		protected void AddNewTrigger(global::Kampai.Game.Trigger.TriggerInstance trigger)
		{
			if (trigger != null)
			{
				triggers.Add(trigger.ID, trigger);
			}
		}

		public global::Kampai.Game.Transaction.WeightedInstance GetWeightedInstance(int defId, global::Kampai.Game.Transaction.WeightedDefinition wd = null)
		{
			global::Kampai.Game.Transaction.WeightedInstance weightedInstance;
			if (!byDefinitionId.ContainsKey(defId))
			{
				if (wd == null)
				{
					wd = definitionService.Get<global::Kampai.Game.Transaction.WeightedDefinition>(defId);
				}
				weightedInstance = new global::Kampai.Game.Transaction.WeightedInstance(wd);
				AssignNextInstanceId(weightedInstance);
				byInstanceId.Add(weightedInstance.ID, weightedInstance);
				global::System.Collections.Generic.List<global::Kampai.Game.Instance> list = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
				list.Add(weightedInstance);
				byDefinitionId.Add(defId, list);
			}
			else
			{
				weightedInstance = byDefinitionId[defId][0] as global::Kampai.Game.Transaction.WeightedInstance;
			}
			return weightedInstance;
		}

		public void Add(global::Kampai.Game.Instance i)
		{
			if (i != null)
			{
				if (i.Definition != null)
				{
					if (AssertNewId(i.ID))
					{
						AddNewInventoryItem(i);
					}
				}
				else
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Illegal instance (null definition).");
				}
			}
			else
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Unable to add null instance to inventory.");
			}
		}

		public void SetQuantityByStaticItem(global::Kampai.Game.StaticItem def, uint newQuantity)
		{
			SetQuantityByDefinitionId((int)def, newQuantity);
		}

		public void SetQuantityByDefinitionId(int defId, uint newQuantity)
		{
			global::Kampai.Game.Item orCreateItemByDefinition = GetOrCreateItemByDefinition(defId);
			orCreateItemByDefinition.Quantity = newQuantity;
		}

		public void SetQuantityByInstanceId(int instanceId, uint newQuantity)
		{
			global::Kampai.Game.Item itemIfExistsByInstanceId = GetItemIfExistsByInstanceId(instanceId);
			if (itemIfExistsByInstanceId == null)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Cannot set instance ID for quantity item if it does not exist: {0}", instanceId);
			}
			else
			{
				itemIfExistsByInstanceId.Quantity = newQuantity;
			}
		}

		public void AlterQuantity(global::Kampai.Game.StaticItem def, int amount = 1)
		{
			AlterQuantity((int)def, amount);
		}

		public void AlterQuantity(int definitionId, int amount = 1)
		{
			global::Kampai.Game.Item item = GetItemIfExistsByDefId(definitionId);
			if (item == null)
			{
				if (amount <= 0)
				{
					if (amount < 0)
					{
						logger.Error("An item cannot be negative {0}", definitionId);
					}
					return;
				}
				item = GetOrCreateItemByDefinition(definitionId);
			}
			AlterQuantityByInstanceId(item.ID, amount);
		}

		public void AlterQuantityByInstanceId(int instanceId, int amount = 1)
		{
			global::Kampai.Game.Item itemIfExistsByInstanceId = GetItemIfExistsByInstanceId(instanceId);
			if (itemIfExistsByInstanceId == null)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Cannot decrement inventory item which does not exist: {0}", instanceId);
			}
			else
			{
				AlterQuantity(itemIfExistsByInstanceId, amount);
			}
		}

		protected global::Kampai.Game.Item GetOrCreateItemByDefinition(int defId)
		{
			global::Kampai.Game.Item item = GetItemIfExistsByDefId(defId);
			if (item == null)
			{
				global::Kampai.Game.ItemDefinition def = definitionService.Get<global::Kampai.Game.ItemDefinition>(defId);
				item = new global::Kampai.Game.Item(def);
				AssignNextInstanceId(item);
				AddNewInventoryItem(item);
			}
			return item;
		}

		public global::Kampai.Game.Instance AlterQuantityByDefId(int defId, int amount = 1)
		{
			return AlterQuantity(GetOrCreateItemByDefinition(defId), amount);
		}

		protected global::Kampai.Game.Instance AlterQuantity(global::Kampai.Game.Item i, int amount)
		{
			if (i != null)
			{
				int num = (int)i.Quantity + amount;
				if (num > 0)
				{
					i.Quantity = (uint)num;
				}
				else if (num == 0)
				{
					RemoveInventoryItem(i);
				}
				else if (num < 0)
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Cannot decrement inventory item below zero: {0}", i.ID);
				}
			}
			return i;
		}

		public void Remove(global::Kampai.Game.Instance i)
		{
			RemoveInventoryItem(i);
		}

		public void AssignNextInstanceId(global::Kampai.Util.Identifiable i)
		{
			if (i != null)
			{
				i.ID = GetNextInstanceId();
			}
		}

		protected int GetNextInstanceId()
		{
			int result = nextId;
			while (byInstanceId.ContainsKey(++nextId))
			{
			}
			return result;
		}

		protected bool AssertNewId(int instanceId)
		{
			if (byInstanceId.ContainsKey(instanceId))
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Cannot add new instance ID because it already exists: {0}", instanceId);
				return false;
			}
			return true;
		}

		protected void AddNewInventoryItem(global::Kampai.Game.Instance i)
		{
			if (i != null)
			{
				int iD = i.Definition.ID;
				byInstanceId.Add(i.ID, i);
				if (!byDefinitionId.ContainsKey(iD))
				{
					byDefinitionId.Add(iD, new global::System.Collections.Generic.List<global::Kampai.Game.Instance>());
				}
				byDefinitionId[iD].Add(i);
			}
		}

		protected void RemoveInventoryItem(global::Kampai.Game.Instance i)
		{
			if (i != null)
			{
				int iD = i.Definition.ID;
				byInstanceId.Remove(i.ID);
				byDefinitionId[iD].Remove(i);
				if (byDefinitionId[iD].Count == 0)
				{
					byDefinitionId.Remove(iD);
				}
			}
		}

		protected global::Kampai.Game.Item GetItemIfExistsByInstanceId(int instanceId)
		{
			if (byInstanceId.ContainsKey(instanceId))
			{
				return byInstanceId[instanceId] as global::Kampai.Game.Item;
			}
			return null;
		}

		protected global::Kampai.Game.Item GetItemIfExistsByDefId(int defId)
		{
			if (byDefinitionId.ContainsKey(defId))
			{
				if (byDefinitionId[defId].Count == 1)
				{
					return byDefinitionId[defId][0] as global::Kampai.Game.Item;
				}
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Ambiguous query (multiple instances with def {0})", defId);
			}
			return null;
		}

		public global::Kampai.Game.KampaiPendingTransaction GetPendingTransaction(string externalIdentifier)
		{
			foreach (global::Kampai.Game.KampaiPendingTransaction pendingTransaction in pendingTransactions)
			{
				if (pendingTransaction.ExternalIdentifier.Trim().ToLower().Equals(externalIdentifier.Trim().ToLower()))
				{
					return pendingTransaction;
				}
			}
			return null;
		}

		public void QueuePendingTransaction(global::Kampai.Game.KampaiPendingTransaction kpt)
		{
			pendingTransactions.Add(kpt);
		}

		public void addPendingRedemption(global::Kampai.Game.Mtx.ReceiptValidationResult notification)
		{
			if (pendingRedemptions == null)
			{
				pendingRedemptions = new global::System.Collections.Generic.List<global::Kampai.Game.Mtx.ReceiptValidationResult>();
			}
			foreach (global::Kampai.Game.Mtx.ReceiptValidationResult pendingRedemption in pendingRedemptions)
			{
				if (pendingRedemption.nimbleTransactionId == notification.nimbleTransactionId)
				{
					return;
				}
			}
			pendingRedemptions.Add(notification);
		}

		public global::Kampai.Game.Mtx.ReceiptValidationResult popPendingRedemption()
		{
			if (pendingRedemptions == null || pendingRedemptions.Count == 0)
			{
				return null;
			}
			global::Kampai.Game.Mtx.ReceiptValidationResult result = pendingRedemptions[0];
			pendingRedemptions.RemoveAt(0);
			return result;
		}

		public global::Kampai.Game.Mtx.ReceiptValidationResult topPendingRedemption()
		{
			if (pendingRedemptions == null || pendingRedemptions.Count == 0)
			{
				return null;
			}
			return pendingRedemptions[0];
		}

		public void RemovePendingTransaction(global::Kampai.Game.KampaiPendingTransaction kpt)
		{
			pendingTransactions.Remove(kpt);
		}

		public void AddUnlockedItems(global::Kampai.Util.QuantityItem item)
		{
			global::Kampai.Game.UnlockDefinition unlockDefinition = definitionService.Get<global::Kampai.Game.UnlockDefinition>(item.ID);
			int quantity = (int)item.Quantity;
			int referencedDefinitionID = unlockDefinition.ReferencedDefinitionID;
			int unlockedQuantity = unlockDefinition.UnlockedQuantity;
			int num = unlockedQuantity * quantity;
			if (unlocks.ContainsKey(referencedDefinitionID))
			{
				if (unlockDefinition.Delta)
				{
					global::System.Collections.Generic.IDictionary<int, int> dictionary2;
					global::System.Collections.Generic.IDictionary<int, int> dictionary = (dictionary2 = unlocks);
					int key2;
					int key = (key2 = referencedDefinitionID);
					key2 = dictionary2[key2];
					dictionary[key] = key2 + num;
				}
				else
				{
					unlocks[referencedDefinitionID] = global::System.Math.Max(num, unlocks[referencedDefinitionID]);
				}
			}
			else
			{
				unlocks.Add(referencedDefinitionID, num);
			}
		}

		public void AddPurchasedUpsells(int saleDefID)
		{
			int num = 1;
			int value;
			if (purchasedUpsells.TryGetValue(saleDefID, out value))
			{
				purchasedUpsells[saleDefID] = value + num;
			}
			else
			{
				purchasedUpsells.Add(saleDefID, num);
			}
		}

		public void ClearPurchasedUpsells(int saleDefID)
		{
			if (purchasedUpsells.ContainsKey(saleDefID))
			{
				purchasedUpsells[saleDefID] = 0;
			}
		}

		public void QueueVillain(global::Kampai.Game.Prestige villainPrestige)
		{
			villainQueue.Add(villainPrestige.ID);
		}

		public int PopVillain()
		{
			if (villainQueue.Count == 0)
			{
				return -1;
			}
			int result = villainQueue[0];
			villainQueue.RemoveAt(0);
			return result;
		}

		public int GetUnlockedAmountFromID(int definitionID)
		{
			if (unlocks.Count == 0)
			{
				return -1;
			}
			if (unlocks.ContainsKey(definitionID))
			{
				return unlocks[definitionID];
			}
			return 0;
		}

		public global::System.Collections.Generic.IDictionary<int, int> GetUnlockedItems()
		{
			return unlocks;
		}

		public int GetAmountPurchased(int definitionID)
		{
			int value = 0;
			if (purchasedUpsells.Count == 0 || purchasedUpsells == null)
			{
				return 0;
			}
			if (purchasedUpsells.TryGetValue(definitionID, out value))
			{
				return value;
			}
			return 0;
		}

		public global::System.Collections.Generic.IDictionary<int, int> GetUpsellsPurchased()
		{
			return purchasedUpsells;
		}

		public global::System.Collections.Generic.IList<global::Kampai.Game.KampaiPendingTransaction> GetPendingTransactions()
		{
			return pendingTransactions;
		}

		public int GetPurchasedExpansionCount()
		{
			global::Kampai.Game.PurchasedLandExpansion purchasedLandExpansion = GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			if (purchasedLandExpansion != null)
			{
				return purchasedLandExpansion.PurchasedExpansionsCount();
			}
			return 0;
		}

		public void AddLandExpansion(global::Kampai.Game.LandExpansionConfig expansionConfig)
		{
			global::Kampai.Game.PurchasedLandExpansion purchasedLandExpansion = GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			int expansionId = expansionConfig.expansionId;
			if (purchasedLandExpansion == null)
			{
				return;
			}
			if (!purchasedLandExpansion.HasPurchased(expansionId))
			{
				purchasedLandExpansion.PurchasedExpansions.Add(expansionId);
				if (purchasedLandExpansion.IsAdjacentExpansion(expansionId))
				{
					purchasedLandExpansion.AdjacentExpansions.Remove(expansionId);
				}
			}
			foreach (int adjacentExpansionId in expansionConfig.adjacentExpansionIds)
			{
				if (!purchasedLandExpansion.HasPurchased(adjacentExpansionId) && !purchasedLandExpansion.IsAdjacentExpansion(adjacentExpansionId))
				{
					purchasedLandExpansion.AdjacentExpansions.Add(adjacentExpansionId);
				}
			}
		}

		public bool IsExpansionPurchased(int expansionId)
		{
			global::Kampai.Game.PurchasedLandExpansion purchasedLandExpansion = GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			if (purchasedLandExpansion != null)
			{
				return purchasedLandExpansion.HasPurchased(expansionId);
			}
			return false;
		}

		public void AddPendingTransactions(global::System.Collections.Generic.IList<global::Kampai.Game.KampaiPendingTransaction> pendingTransactions)
		{
			this.pendingTransactions.AddRange(pendingTransactions);
		}

		public void AddUnlock(int id, int quantity)
		{
			unlocks[id] = quantity;
		}

		public void AddPurchasedUpsell(int id, int quantity)
		{
			purchasedUpsells[id] = quantity;
		}

		public void AddVillainQueue(int characterDefinitionId)
		{
			villainQueue.Add(characterDefinitionId);
		}

		public bool HasPlatformStoreTransactionID(string identifier)
		{
			return PlatformStoreTransactionIDs.Contains(identifier);
		}

		public void AddPlatformStoreTransactionID(string id)
		{
			PlatformStoreTransactionIDs.Add(id);
		}

		public global::System.Collections.Generic.IList<int> GetVillainQueue()
		{
			return villainQueue;
		}

		public global::System.Collections.Generic.IList<string> GetPlatformStoreTransactionIDs()
		{
			return PlatformStoreTransactionIDs;
		}

		public global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState> GetSocialClaimRewards()
		{
			return socialRewards;
		}

		public void AddSocialClaimRewards(int eventID, global::Kampai.Game.SocialClaimRewardItem.ClaimState claimState)
		{
			socialRewards[eventID] = claimState;
		}

		public void CleanupSocialClaimReward(global::System.Collections.Generic.List<int> recentEventIDs)
		{
			global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState> dictionary = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState>();
			foreach (int recentEventID in recentEventIDs)
			{
				if (socialRewards.ContainsKey(recentEventID))
				{
					dictionary[recentEventID] = socialRewards[recentEventID];
				}
			}
			socialRewards = dictionary;
		}

		public global::System.Collections.Generic.IList<string> GetMTXPurchaseTracking()
		{
			return mtxPurchaseTracking;
		}

		public void AddMTXPurchaseTracking(string SKU)
		{
			mtxPurchaseTracking.Add(SKU);
		}

		public int MTXPurchaseCount(string sku)
		{
			if (string.IsNullOrEmpty(sku))
			{
				return mtxPurchaseTracking.Count;
			}
			int num = 0;
			sku = sku.ToLower();
			foreach (string item in mtxPurchaseTracking)
			{
				if (item.ToLower().Equals(sku))
				{
					num++;
				}
			}
			return num;
		}

		public int GetStorableItemCount()
		{
			uint itemCount = 0u;
			FindStorableItems(out itemCount);
			return (int)(((int)itemCount >= 0) ? itemCount : 0);
		}

		public global::Kampai.Game.Player.SanityCheckFailureReason ValidateSaveData(global::Kampai.Game.Player prevSave)
		{
			global::Kampai.Game.Player.SanityCheckFailureReason sanityCheckFailureReason = global::Kampai.Game.Player.SanityCheckFailureReason.Passed;
			sanityCheckFailureReason = ValidateOrderBoardTickets(prevSave);
			if (sanityCheckFailureReason != global::Kampai.Game.Player.SanityCheckFailureReason.Passed)
			{
				return sanityCheckFailureReason;
			}
			sanityCheckFailureReason = ValidateTSM();
			if (sanityCheckFailureReason != global::Kampai.Game.Player.SanityCheckFailureReason.Passed)
			{
				return sanityCheckFailureReason;
			}
			sanityCheckFailureReason = ValidateBuildings(prevSave);
			if (sanityCheckFailureReason != global::Kampai.Game.Player.SanityCheckFailureReason.Passed)
			{
				return sanityCheckFailureReason;
			}
			sanityCheckFailureReason = ValidateMinions(prevSave);
			if (sanityCheckFailureReason != global::Kampai.Game.Player.SanityCheckFailureReason.Passed)
			{
				return sanityCheckFailureReason;
			}
			sanityCheckFailureReason = ValidateLandExpansions(prevSave);
			if (sanityCheckFailureReason != global::Kampai.Game.Player.SanityCheckFailureReason.Passed)
			{
				return sanityCheckFailureReason;
			}
			return sanityCheckFailureReason;
		}

		private global::Kampai.Game.Player.SanityCheckFailureReason ValidateOrderBoardTickets(global::Kampai.Game.Player prevSave)
		{
			global::Kampai.Game.OrderBoard firstInstanceByDefinitionId = GetFirstInstanceByDefinitionId<global::Kampai.Game.OrderBoard>(3022);
			if (firstInstanceByDefinitionId != null && firstInstanceByDefinitionId.tickets != null)
			{
				foreach (global::Kampai.Game.OrderBoardTicket ticket in firstInstanceByDefinitionId.tickets)
				{
					if (ticket.TransactionInst != null && ticket.TransactionInst.Inputs != null && ticket.TransactionInst.Outputs != null && (ticket.TransactionInst.Inputs.Count == 0 || ticket.TransactionInst.Outputs.Count == 0))
					{
						return global::Kampai.Game.Player.SanityCheckFailureReason.OrderBoardTickerInputsOutputs;
					}
				}
				if (prevSave != null)
				{
					global::Kampai.Game.OrderBoard firstInstanceByDefinitionId2 = prevSave.GetFirstInstanceByDefinitionId<global::Kampai.Game.OrderBoard>(3022);
					if (firstInstanceByDefinitionId2 != null && firstInstanceByDefinitionId2.tickets != null && firstInstanceByDefinitionId2.tickets.Count > firstInstanceByDefinitionId.tickets.Count)
					{
						return global::Kampai.Game.Player.SanityCheckFailureReason.OrderBoardTicketCount;
					}
				}
				return global::Kampai.Game.Player.SanityCheckFailureReason.Passed;
			}
			return global::Kampai.Game.Player.SanityCheckFailureReason.OrderBoardMissing;
		}

		private global::Kampai.Game.Player.SanityCheckFailureReason ValidateTSM()
		{
			global::Kampai.Game.TSMCharacter firstInstanceByDefinitionId = GetFirstInstanceByDefinitionId<global::Kampai.Game.TSMCharacter>(70008);
			if (firstInstanceByDefinitionId != null)
			{
				global::Kampai.Game.Quest firstInstanceByDefinitionId2 = GetFirstInstanceByDefinitionId<global::Kampai.Game.Quest>(77777);
				if (firstInstanceByDefinitionId2 != null)
				{
					global::Kampai.Game.Transaction.TransactionDefinition reward = firstInstanceByDefinitionId2.GetActiveDefinition().GetReward(null);
					if (reward == null || reward.Outputs.Count == 0)
					{
						return global::Kampai.Game.Player.SanityCheckFailureReason.TSMOutputs;
					}
				}
			}
			return global::Kampai.Game.Player.SanityCheckFailureReason.Passed;
		}

		private global::Kampai.Game.Player.SanityCheckFailureReason ValidateBuildings(global::Kampai.Game.Player prevSave)
		{
			if (prevSave != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition = GetInstancesByDefinition<global::Kampai.Game.BuildingDefinition>();
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition2 = GetInstancesByDefinition<global::Kampai.Game.DebrisBuildingDefinition>();
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition3 = prevSave.GetInstancesByDefinition<global::Kampai.Game.BuildingDefinition>();
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition4 = prevSave.GetInstancesByDefinition<global::Kampai.Game.DebrisBuildingDefinition>();
				global::System.Collections.Generic.List<global::Kampai.Game.Instance> instancesByDefinition5 = GetInstancesByDefinition<global::Kampai.Game.MasterPlanComponentBuildingDefinition>();
				global::System.Collections.Generic.List<global::Kampai.Game.Instance> instancesByDefinition6 = prevSave.GetInstancesByDefinition<global::Kampai.Game.MasterPlanComponentBuildingDefinition>();
				int num = instancesByDefinition.Count - instancesByDefinition2.Count - instancesByDefinition5.Count;
				int num2 = instancesByDefinition3.Count - instancesByDefinition4.Count - instancesByDefinition6.Count;
				if (num2 > num)
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.BuildingCount;
				}
			}
			global::Kampai.Game.StaticItem[] array = new global::Kampai.Game.StaticItem[8]
			{
				global::Kampai.Game.StaticItem.TIKI_BAR_BUILDING_ID_DEF,
				global::Kampai.Game.StaticItem.STAGE_BUILDING_DEF_ID,
				global::Kampai.Game.StaticItem.FOUNTAIN_BUILDING_DEF_ID,
				global::Kampai.Game.StaticItem.WELCOME_BOOTH_BUILDING_ID_DEF,
				global::Kampai.Game.StaticItem.CABANA_ONE_BUILDING_ID_DEF,
				global::Kampai.Game.StaticItem.CABANA_TWO_BUILDING_ID_DEF,
				global::Kampai.Game.StaticItem.CABANA_THREE_BUILDING_ID_DEF,
				global::Kampai.Game.StaticItem.COMPOSITE_TOTEM_POLE_ID_DEF
			};
			global::Kampai.Game.StaticItem[] array2 = array;
			foreach (global::Kampai.Game.StaticItem definitionId in array2)
			{
				global::Kampai.Game.Building firstInstanceByDefinitionId = GetFirstInstanceByDefinitionId<global::Kampai.Game.Building>((int)definitionId);
				if (firstInstanceByDefinitionId == null)
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.RequiredBuilding;
				}
			}
			global::Kampai.Game.StaticItem[] array3 = new global::Kampai.Game.StaticItem[5]
			{
				global::Kampai.Game.StaticItem.MIGNETTE_ALLIGATOR_DEF_ID,
				global::Kampai.Game.StaticItem.MIGNETTE_BALLOON_DEF_ID,
				global::Kampai.Game.StaticItem.MIGNETTE_BUTTERFLY_DEF_ID,
				global::Kampai.Game.StaticItem.MIGNETTE_MINION_HANDS_DEF_ID,
				global::Kampai.Game.StaticItem.MIGNETTE_WATER_SLIDE_DEF_ID
			};
			global::Kampai.Game.StaticItem[] array4 = array3;
			foreach (global::Kampai.Game.StaticItem definitionId2 in array4)
			{
				global::Kampai.Game.Building firstInstanceByDefinitionId2 = GetFirstInstanceByDefinitionId<global::Kampai.Game.Building>((int)definitionId2);
				global::Kampai.Game.Building firstInstanceByDefinitionId3 = prevSave.GetFirstInstanceByDefinitionId<global::Kampai.Game.Building>((int)definitionId2);
				if (firstInstanceByDefinitionId2 == null && firstInstanceByDefinitionId3 != null)
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.MignetteMissing;
				}
			}
			foreach (global::Kampai.Game.Building item in GetInstancesByType<global::Kampai.Game.Building>())
			{
				if (item.State != global::Kampai.Game.BuildingState.Inventory && item.Location == null)
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.BuildingMissingLocation;
				}
			}
			return global::Kampai.Game.Player.SanityCheckFailureReason.Passed;
		}

		private global::Kampai.Game.Player.SanityCheckFailureReason ValidateMinions(global::Kampai.Game.Player prevSave)
		{
			if (prevSave != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition = GetInstancesByDefinition<global::Kampai.Game.MinionDefinition>();
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition2 = prevSave.GetInstancesByDefinition<global::Kampai.Game.MinionDefinition>();
				if (instancesByDefinition2.Count > instancesByDefinition.Count)
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.MinionCount;
				}
				global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
				foreach (global::Kampai.Game.Minion item in instancesByDefinition)
				{
					if (item.HasPrestige)
					{
						list.Add(item.ID);
					}
				}
				foreach (global::Kampai.Game.Minion item2 in instancesByDefinition2)
				{
					if (item2.HasPrestige && !list.Contains(item2.ID))
					{
						return global::Kampai.Game.Player.SanityCheckFailureReason.CostumedMinion;
					}
				}
			}
			global::Kampai.Game.StaticItem[] array = new global::Kampai.Game.StaticItem[4]
			{
				global::Kampai.Game.StaticItem.PHIL_CHARACTER_DEF_ID,
				global::Kampai.Game.StaticItem.STUART_CHARACTER_DEF_ID,
				global::Kampai.Game.StaticItem.BOB_CHARACTER_DEF_ID,
				global::Kampai.Game.StaticItem.KEVIN_CHARACTER_DEF_ID
			};
			global::Kampai.Game.StaticItem[] array2 = array;
			foreach (global::Kampai.Game.StaticItem staticItem in array2)
			{
				global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId = GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>((int)staticItem);
				if (firstInstanceByDefinitionId == null && staticItem == global::Kampai.Game.StaticItem.PHIL_CHARACTER_DEF_ID)
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.NamedMinion;
				}
				if (prevSave != null)
				{
					global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId2 = prevSave.GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>((int)staticItem);
					if (firstInstanceByDefinitionId2 != null && firstInstanceByDefinitionId == null)
					{
						return global::Kampai.Game.Player.SanityCheckFailureReason.NamedMinion;
					}
				}
			}
			return global::Kampai.Game.Player.SanityCheckFailureReason.Passed;
		}

		private global::Kampai.Game.Player.SanityCheckFailureReason ValidateLandExpansions(global::Kampai.Game.Player prevSave)
		{
			global::Kampai.Game.PurchasedLandExpansion purchasedLandExpansion = GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			global::Kampai.Game.PurchasedLandExpansion purchasedLandExpansion2 = prevSave.GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			if (purchasedLandExpansion.PurchasedExpansions.Count == 0 && purchasedLandExpansion.AdjacentExpansions.Count == 0)
			{
				return global::Kampai.Game.Player.SanityCheckFailureReason.LandExpansionEmpty;
			}
			foreach (int purchasedExpansion in purchasedLandExpansion2.PurchasedExpansions)
			{
				if (!purchasedLandExpansion.PurchasedExpansions.Contains(purchasedExpansion))
				{
					return global::Kampai.Game.Player.SanityCheckFailureReason.PurchasedLandExpansionMissing;
				}
			}
			return global::Kampai.Game.Player.SanityCheckFailureReason.Passed;
		}

		public void addSocialInvitationSeen(long socialInvitationId)
		{
			if (socialTeamInvitationsSeen == null)
			{
				socialTeamInvitationsSeen = new global::System.Collections.Generic.List<long>();
			}
			socialTeamInvitationsSeen.Add(socialInvitationId);
		}

		public bool seenSocialInvitiation(long socialInvitationId)
		{
			if (socialTeamInvitationsSeen == null)
			{
				return false;
			}
			return socialTeamInvitationsSeen.Contains(socialInvitationId);
		}

		public int getAndIncrementRequestCounter()
		{
			return ++requestCounter;
		}
	}
}
