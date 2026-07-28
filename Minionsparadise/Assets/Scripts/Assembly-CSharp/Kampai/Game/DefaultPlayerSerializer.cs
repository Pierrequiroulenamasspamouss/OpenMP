namespace Kampai.Game
{
	internal abstract class DefaultPlayerSerializer : global::Kampai.Game.IPlayerSerializer
	{
		public abstract int Version { get; }

		protected virtual global::Kampai.Game.PlayerData GeneratePlayerData(string serialized, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.IPartyService partyService, global::Kampai.Util.IKampaiLogger logger)
		{
			global::UnityEngine.Debug.LogFormat("[DEBUG] DefaultPlayerSerializer.GeneratePlayerData: deserializing player json len={0}", serialized != null ? serialized.Length : 0);
			global::Kampai.Game.PlayerData playerData = null;
			try
			{
				JsonConverters jsonConverters = new JsonConverters();
				jsonConverters.instanceConverter = new global::Kampai.Game.InventoryFastConverter(definitionService, logger);
				jsonConverters.transactionDefinitionConverter = new global::Kampai.Game.TransactionDefinitionFastConverter(definitionService);
				jsonConverters.questDefinitionConverter = new global::Kampai.Game.QuestDefinitionFastConverter();
				jsonConverters.triggerInstanceConverter = new global::Kampai.Game.Trigger.TriggerInstanceFastConverter(definitionService);
				playerData = global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.PlayerData>(serialized, jsonConverters);
				if (playerData == null)
				{
					logger.Error("PlayerDataV1: player is null after deserialization, creating fallback."); playerData = new global::Kampai.Game.PlayerData(); playerData.inventory = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
				}
			}
			catch (global::Newtonsoft.Json.JsonSerializationException e)
			{
				HandleJsonParseException(serialized, e, logger);
			}
			catch (global::Newtonsoft.Json.JsonReaderException e2)
			{
				HandleJsonParseException(serialized, e2, logger);
			}
			return playerData;
		}

		private void HandleJsonParseException(string json, global::System.Exception e, global::Kampai.Util.IKampaiLogger logger)
		{
			logger.Error("HandleJsonParseException(): player json: {0}", json ?? "null");
			global::UnityEngine.Debug.LogErrorFormat("[DEBUG] DefaultPlayerSerializer.HandleJsonParseException: {0}\n{1}", e.Message, e.StackTrace);
			logger.Error("HandleJsonParseException(): player json parse error: {0}", e);
		}

		public virtual global::Kampai.Game.Player Deserialize(string json, global::Kampai.Game.IDefinitionService definitionService, ILocalPersistanceService localPersistanceService, global::Kampai.Game.IPartyService partyService, global::Kampai.Util.IKampaiLogger logger)
		{
			global::Kampai.Game.PlayerData playerData = GeneratePlayerData(json, definitionService, partyService, logger);
			global::Kampai.Game.Player player = new global::Kampai.Game.Player(definitionService, logger);
			player.ID = playerData.ID;
			player.Version = playerData.version;
			player.nextId = playerData.nextId;
			player.lastLevelUpTime = playerData.lastLevelUpTime;
			player.lastGameStartTime = playerData.lastGameStartTime;
			player.firstGameStartTime = playerData.firstGameStartTime;
			player.lastPlayedTime = playerData.lastPlayedTime;
			player.totalGameplayDurationSinceLastLevelUp = playerData.totalGameplayDurationSinceLastLevelUp;
			player.totalAccumulatedGameplayDuration = playerData.totalAccumulatedGameplayDuration;
			player.targetExpansionID = playerData.targetExpansionID;
			player.HighestFtueLevel = playerData.highestFtueLevel;
			player.timezoneOffset = playerData.timezoneOffset;
			player.country = playerData.country;
			player.completedOrders = playerData.completedOrders;
			player.completedQuestsTotal = playerData.completedQuestsTotal;
			if (playerData.inventory == null)
			{
				logger.Warning("Player inventory is null, initializing empty inventory."); playerData.inventory = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
			}
			if (playerData.helpTipsTrackingData != null)
			{
				player.helpTipsTrackingData = playerData.helpTipsTrackingData;
			}
			foreach (global::Kampai.Game.Instance item in playerData.inventory)
			{
				player.Add(item);
			}
			if (playerData.pendingTransactions != null)
			{
				player.AddPendingTransactions(playerData.pendingTransactions);
			}
			if (playerData.unlocks != null && playerData.unlocks.Count > 0)
			{
				foreach (global::Kampai.Game.UnlockedItem unlock in playerData.unlocks)
				{
					player.AddUnlock(unlock.defID, unlock.quantity);
				}
			}
			if (playerData.purchasedSales != null && playerData.purchasedSales.Count > 0)
			{
				foreach (global::Kampai.Game.TrackedSale purchasedSale in playerData.purchasedSales)
				{
					player.AddPurchasedUpsell(purchasedSale.defID, purchasedSale.numberPurchased);
				}
			}
			DeserializeSocialTeam(playerData, player);
			DeserializeVillians(playerData, player);
			DeserializeMTXPurchaseTracking(playerData, player);
			DeserializeTriggers(playerData, player);
			DeserializeSales(definitionService, player);
			if (playerData.PlatformStoreTransactionIDs != null && playerData.PlatformStoreTransactionIDs.Count > 0)
			{
				foreach (string platformStoreTransactionID in playerData.PlatformStoreTransactionIDs)
				{
					player.AddPlatformStoreTransactionID(platformStoreTransactionID);
				}
			}
			return player;
		}

		private static void DeserializeSocialTeam(global::Kampai.Game.PlayerData pd, global::Kampai.Game.Player p)
		{
			if (pd.socialRewards == null || pd.socialRewards.Count <= 0)
			{
				return;
			}
			foreach (global::Kampai.Game.SocialClaimRewardItem socialReward in pd.socialRewards)
			{
				p.AddSocialClaimRewards(socialReward.eventID, socialReward.claimState);
			}
		}

		private static void DeserializeMTXPurchaseTracking(global::Kampai.Game.PlayerData pd, global::Kampai.Game.Player p)
		{
			if (pd.mtxPurchaseTracking == null || pd.mtxPurchaseTracking.Count <= 0)
			{
				return;
			}
			foreach (string item in pd.mtxPurchaseTracking)
			{
				p.AddMTXPurchaseTracking(item);
			}
		}

		private static void DeserializeVillians(global::Kampai.Game.PlayerData pd, global::Kampai.Game.Player p)
		{
			if (pd.villainQueue == null || pd.villainQueue.Count <= 0)
			{
				return;
			}
			foreach (int item in pd.villainQueue)
			{
				p.AddVillainQueue(item);
			}
		}

		private static void DeserializeSales(global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.Player player)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Sale> instancesByType = player.GetInstancesByType<global::Kampai.Game.Sale>();
			foreach (global::Kampai.Game.Sale item in instancesByType)
			{
				if (item.Definition.Type.Equals(global::Kampai.Game.SalePackType.Upsell))
				{
					if (!definitionService.Has(item.Definition.ID))
					{
						definitionService.Add(item.Definition);
					}
					if (!definitionService.Has(item.Definition.TransactionDefinition.ID))
					{
						definitionService.Add(item.Definition.TransactionDefinition.ToDefinition());
					}
				}
			}
		}

		private static void DeserializeTriggers(global::Kampai.Game.PlayerData playerData, global::Kampai.Game.Player player)
		{
			if (playerData.triggers == null)
			{
				return;
			}
			for (int i = 0; i < playerData.triggers.Count; i++)
			{
				if (playerData.triggers[i] != null)
				{
					player.Add(playerData.triggers[i]);
				}
			}
		}

		public virtual byte[] Serialize(global::Kampai.Game.Player player, global::Kampai.Game.IDefinitionService defintitionService, global::Kampai.Util.IKampaiLogger logger)
		{
			global::Kampai.Game.PlayerData playerData = new global::Kampai.Game.PlayerData();
			playerData.ID = player.ID;
			playerData.version = player.Version;
			playerData.nextId = player.NextId;
			playerData.inventory = new global::System.Collections.Generic.List<global::Kampai.Game.Instance>();
			playerData.unlocks = new global::System.Collections.Generic.List<global::Kampai.Game.UnlockedItem>();
			playerData.purchasedSales = new global::System.Collections.Generic.List<global::Kampai.Game.TrackedSale>();
			playerData.villainQueue = new global::System.Collections.Generic.List<int>();
			playerData.lastLevelUpTime = player.lastLevelUpTime;
			playerData.lastGameStartTime = player.lastGameStartTime;
			playerData.firstGameStartTime = player.firstGameStartTime;
			playerData.lastPlayedTime = player.lastPlayedTime;
			playerData.totalGameplayDurationSinceLastLevelUp = player.totalGameplayDurationSinceLastLevelUp;
			playerData.totalAccumulatedGameplayDuration = player.totalAccumulatedGameplayDuration;
			playerData.targetExpansionID = player.targetExpansionID;
			playerData.highestFtueLevel = player.HighestFtueLevel;
			playerData.mtxPurchaseTracking = new global::System.Collections.Generic.List<string>();
			playerData.timezoneOffset = player.timezoneOffset;
			playerData.socialRewards = new global::System.Collections.Generic.List<global::Kampai.Game.SocialClaimRewardItem>();
			playerData.completedQuestsTotal = player.completedQuestsTotal;
			playerData.currentItemCount = player.GetStorableItemCount();
			playerData.country = player.country;
			playerData.completedOrders = player.completedOrders;
			playerData.PlatformStoreTransactionIDs = new global::System.Collections.Generic.List<string>();
			playerData.helpTipsTrackingData = player.helpTipsTrackingData;
			SerializeCollectionsPart1(playerData, player);
			return global::Kampai.Util.FastJSONSerializer.SerializeUTF8(playerData);
		}

		private void SerializeCollectionsPart1(global::Kampai.Game.PlayerData playerData, global::Kampai.Game.Player player)
		{
			foreach (global::Kampai.Game.Instance item in player.GetInstancesByDefinition())
			{
				playerData.inventory.Add(item);
			}
			global::System.Collections.Generic.IList<global::Kampai.Game.KampaiPendingTransaction> pendingTransactions = player.GetPendingTransactions();
			if (pendingTransactions != null && pendingTransactions.Count > 0)
			{
				playerData.pendingTransactions = pendingTransactions;
			}
			global::System.Collections.Generic.IDictionary<int, int> unlockedItems = player.GetUnlockedItems();
			if (unlockedItems != null && unlockedItems.Count > 0)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<int, int> item2 in unlockedItems)
				{
					playerData.unlocks.Add(new global::Kampai.Game.UnlockedItem(item2.Key, item2.Value));
				}
			}
			global::System.Collections.Generic.IDictionary<int, int> upsellsPurchased = player.GetUpsellsPurchased();
			if (upsellsPurchased != null && upsellsPurchased.Count > 0)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<int, int> item3 in upsellsPurchased)
				{
					playerData.purchasedSales.Add(new global::Kampai.Game.TrackedSale(item3.Key, item3.Value));
				}
			}
			SerializeCollectionsPart2(playerData, player);
		}

		private void SerializeCollectionsPart2(global::Kampai.Game.PlayerData playerData, global::Kampai.Game.Player player)
		{
			global::System.Collections.Generic.IList<int> villainQueue = player.GetVillainQueue();
			if (villainQueue != null && villainQueue.Count > 0)
			{
				foreach (int item in villainQueue)
				{
					playerData.villainQueue.Add(item);
				}
			}
			global::System.Collections.Generic.IList<string> platformStoreTransactionIDs = player.GetPlatformStoreTransactionIDs();
			if (platformStoreTransactionIDs != null && platformStoreTransactionIDs.Count > 0)
			{
				foreach (string item2 in platformStoreTransactionIDs)
				{
					playerData.PlatformStoreTransactionIDs.Add(item2);
				}
			}
			global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState> socialClaimRewards = player.GetSocialClaimRewards();
			if (socialClaimRewards != null && socialClaimRewards.Count > 0)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.SocialClaimRewardItem.ClaimState> item3 in socialClaimRewards)
				{
					playerData.socialRewards.Add(new global::Kampai.Game.SocialClaimRewardItem(item3.Key, item3.Value));
				}
			}
			global::System.Collections.Generic.IList<string> mTXPurchaseTracking = player.GetMTXPurchaseTracking();
			if (mTXPurchaseTracking != null && mTXPurchaseTracking.Count > 0)
			{
				foreach (string item4 in mTXPurchaseTracking)
				{
					playerData.mtxPurchaseTracking.Add(item4);
				}
			}
			SerializeTriggers(playerData, player);
		}

		private static void SerializeTriggers(global::Kampai.Game.PlayerData playerData, global::Kampai.Game.Player player)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggers = player.GetTriggers();
			if (triggers != null && triggers.Count != 0)
			{
				playerData.triggers = new global::System.Collections.Generic.List<global::Kampai.Game.Trigger.TriggerInstance>();
				for (int i = 0; i < triggers.Count; i++)
				{
					playerData.triggers.Add(triggers[i]);
				}
			}
		}
	}
}
