namespace Kampai.Game
{
	public class PlayerData : global::Kampai.Util.IFastJSONDeserializable, global::Kampai.Util.IFastJSONSerializable
	{
		public long ID;

		public int version;

		public int nextId;

		public global::System.Collections.Generic.IList<int> villainQueue;

		[global::Kampai.Util.Serializer("PlayerData.SerializeInventory")]
		public global::System.Collections.Generic.IList<global::Kampai.Game.Instance> inventory;

		public global::System.Collections.Generic.IList<global::Kampai.Game.KampaiPendingTransaction> pendingTransactions;

		public global::System.Collections.Generic.IList<global::Kampai.Game.UnlockedItem> unlocks;

		public global::System.Collections.Generic.IList<global::Kampai.Game.TrackedSale> purchasedSales;

		[global::Kampai.Util.Serializer("PlayerData.SerializeTriggers")]
		public global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggers;

		public int lastLevelUpTime;

		public int lastGameStartTime;

		public int firstGameStartTime;

		public int lastPlayedTime;

		public int totalGameplayDurationSinceLastLevelUp;

		public int totalAccumulatedGameplayDuration;

		public int targetExpansionID;

		public int timezoneOffset;

		public string country;

		public int completedOrders;

		public int highestFtueLevel;

		public global::System.Collections.Generic.IList<global::Kampai.Game.SocialClaimRewardItem> socialRewards;

		public global::System.Collections.Generic.IList<string> mtxPurchaseTracking;

		public int completedQuestsTotal;

		public int currentItemCount;

		public global::System.Collections.Generic.IList<string> PlatformStoreTransactionIDs;

		public global::System.Collections.Generic.List<global::Kampai.Game.Player.HelpTipTrackingItem> helpTipsTrackingData;

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
			case "VERSION":
				reader.Read();
				version = global::System.Convert.ToInt32(reader.Value);
				break;
			case "NEXTID":
				reader.Read();
				nextId = global::System.Convert.ToInt32(reader.Value);
				break;
			case "VILLAINQUEUE":
				reader.Read();
				villainQueue = global::Kampai.Util.ReaderUtil.PopulateListInt32(reader, villainQueue);
				break;
			case "INVENTORY":
				reader.Read();
				inventory = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.instanceConverter, inventory);
				break;
			case "PENDINGTRANSACTIONS":
				reader.Read();
				pendingTransactions = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadKampaiPendingTransaction, pendingTransactions);
				break;
			case "UNLOCKS":
				reader.Read();
				unlocks = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadUnlockedItem, unlocks);
				break;
			case "PURCHASEDSALES":
				reader.Read();
				purchasedSales = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadTrackedSale, purchasedSales);
				break;
			case "TRIGGERS":
				reader.Read();
				triggers = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, converters.triggerInstanceConverter, triggers);
				break;
			case "LASTLEVELUPTIME":
				reader.Read();
				lastLevelUpTime = global::System.Convert.ToInt32(reader.Value);
				break;
			case "LASTGAMESTARTTIME":
				reader.Read();
				lastGameStartTime = global::System.Convert.ToInt32(reader.Value);
				break;
			case "FIRSTGAMESTARTTIME":
				reader.Read();
				firstGameStartTime = global::System.Convert.ToInt32(reader.Value);
				break;
			case "LASTPLAYEDTIME":
				reader.Read();
				lastPlayedTime = global::System.Convert.ToInt32(reader.Value);
				break;
			case "TOTALGAMEPLAYDURATIONSINCELASTLEVELUP":
				reader.Read();
				totalGameplayDurationSinceLastLevelUp = global::System.Convert.ToInt32(reader.Value);
				break;
			case "TOTALACCUMULATEDGAMEPLAYDURATION":
				reader.Read();
				totalAccumulatedGameplayDuration = global::System.Convert.ToInt32(reader.Value);
				break;
			case "TARGETEXPANSIONID":
				reader.Read();
				targetExpansionID = global::System.Convert.ToInt32(reader.Value);
				break;
			case "TIMEZONEOFFSET":
				reader.Read();
				timezoneOffset = global::System.Convert.ToInt32(reader.Value);
				break;
			case "COUNTRY":
				reader.Read();
				country = global::Kampai.Util.ReaderUtil.ReadString(reader, converters);
				break;
			case "COMPLETEDORDERS":
				reader.Read();
				completedOrders = global::System.Convert.ToInt32(reader.Value);
				break;
			case "HIGHESTFTUELEVEL":
				reader.Read();
				highestFtueLevel = global::System.Convert.ToInt32(reader.Value);
				break;
			case "SOCIALREWARDS":
				reader.Read();
				socialRewards = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadSocialClaimRewardItem, socialRewards);
				break;
			case "MTXPURCHASETRACKING":
				reader.Read();
				mtxPurchaseTracking = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadString, mtxPurchaseTracking);
				break;
			case "COMPLETEDQUESTSTOTAL":
				reader.Read();
				completedQuestsTotal = global::System.Convert.ToInt32(reader.Value);
				break;
			case "CURRENTITEMCOUNT":
				reader.Read();
				currentItemCount = global::System.Convert.ToInt32(reader.Value);
				break;
			case "PLATFORMSTORETRANSACTIONIDS":
				reader.Read();
				PlatformStoreTransactionIDs = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadString, PlatformStoreTransactionIDs);
				break;
			case "HELPTIPSTRACKINGDATA":
				reader.Read();
				helpTipsTrackingData = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadHelpTipTrackingItem, helpTipsTrackingData);
				break;
			default:
				return false;
			}
			return true;
		}

		public virtual void Serialize(global::Newtonsoft.Json.JsonWriter writer)
		{
			writer.WriteStartObject();
			SerializeProperties(writer);
			writer.WriteEndObject();
		}

		protected virtual void SerializeProperties(global::Newtonsoft.Json.JsonWriter writer)
		{
			writer.WritePropertyName("ID");
			writer.WriteValue(ID);
			writer.WritePropertyName("version");
			writer.WriteValue(version);
			writer.WritePropertyName("nextId");
			writer.WriteValue(nextId);
			if (villainQueue != null)
			{
				writer.WritePropertyName("villainQueue");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<int> enumerator = villainQueue.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						int current = enumerator.Current;
						writer.WriteValue(current);
					}
				}
				finally
				{
					enumerator.Dispose();
				}
				writer.WriteEndArray();
			}
			if (inventory != null)
			{
				writer.WritePropertyName("inventory");
				SerializeInventory(writer, inventory);
			}
			if (pendingTransactions != null)
			{
				writer.WritePropertyName("pendingTransactions");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<global::Kampai.Game.KampaiPendingTransaction> enumerator2 = pendingTransactions.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						global::Kampai.Game.KampaiPendingTransaction current2 = enumerator2.Current;
						writer.WriteStartObject();
						if (current2.ExternalIdentifier != null)
						{
							writer.WritePropertyName("ExternalIdentifier");
							writer.WriteValue(current2.ExternalIdentifier);
						}
						if (current2.Transaction != null)
						{
							writer.WritePropertyName("Transaction");
							global::Kampai.Game.KampaiPendingTransaction.SerializeDefinition(writer, current2.Transaction);
						}
						if (current2.TransactionInstance != null)
						{
							writer.WritePropertyName("TransactionInstance");
							writer.WriteStartObject();
							writer.WritePropertyName("ID");
							writer.WriteValue(current2.TransactionInstance.ID);
							if (current2.TransactionInstance.Inputs != null)
							{
								writer.WritePropertyName("Inputs");
								writer.WriteStartArray();
								global::System.Collections.Generic.IEnumerator<global::Kampai.Util.QuantityItem> enumerator3 = current2.TransactionInstance.Inputs.GetEnumerator();
								try
								{
									while (enumerator3.MoveNext())
									{
										global::Kampai.Util.QuantityItem current3 = enumerator3.Current;
										writer.WriteStartObject();
										writer.WritePropertyName("ID");
										writer.WriteValue(current3.ID);
										writer.WritePropertyName("Quantity");
										writer.WriteValue(current3.Quantity);
										writer.WriteEndObject();
									}
								}
								finally
								{
									enumerator3.Dispose();
								}
								writer.WriteEndArray();
							}
							if (current2.TransactionInstance.Outputs != null)
							{
								writer.WritePropertyName("Outputs");
								writer.WriteStartArray();
								global::System.Collections.Generic.IEnumerator<global::Kampai.Util.QuantityItem> enumerator4 = current2.TransactionInstance.Outputs.GetEnumerator();
								try
								{
									while (enumerator4.MoveNext())
									{
										global::Kampai.Util.QuantityItem current4 = enumerator4.Current;
										writer.WriteStartObject();
										writer.WritePropertyName("ID");
										writer.WriteValue(current4.ID);
										writer.WritePropertyName("Quantity");
										writer.WriteValue(current4.Quantity);
										writer.WriteEndObject();
									}
								}
								finally
								{
									enumerator4.Dispose();
								}
								writer.WriteEndArray();
							}
							writer.WriteEndObject();
						}
						writer.WritePropertyName("StoreItemDefinitionId");
						writer.WriteValue(current2.StoreItemDefinitionId);
						writer.WritePropertyName("UTCTimeCreated");
						writer.WriteValue(current2.UTCTimeCreated);
						writer.WriteEndObject();
					}
				}
				finally
				{
					enumerator2.Dispose();
				}
				writer.WriteEndArray();
			}
			if (unlocks != null)
			{
				writer.WritePropertyName("unlocks");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<global::Kampai.Game.UnlockedItem> enumerator5 = unlocks.GetEnumerator();
				try
				{
					while (enumerator5.MoveNext())
					{
						global::Kampai.Game.UnlockedItem current5 = enumerator5.Current;
						writer.WriteStartObject();
						writer.WritePropertyName("defID");
						writer.WriteValue(current5.defID);
						writer.WritePropertyName("quantity");
						writer.WriteValue(current5.quantity);
						writer.WriteEndObject();
					}
				}
				finally
				{
					enumerator5.Dispose();
				}
				writer.WriteEndArray();
			}
			if (purchasedSales != null)
			{
				writer.WritePropertyName("purchasedSales");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<global::Kampai.Game.TrackedSale> enumerator6 = purchasedSales.GetEnumerator();
				try
				{
					while (enumerator6.MoveNext())
					{
						global::Kampai.Game.TrackedSale current6 = enumerator6.Current;
						writer.WriteStartObject();
						writer.WritePropertyName("defID");
						writer.WriteValue(current6.defID);
						writer.WritePropertyName("numberPurchased");
						writer.WriteValue(current6.numberPurchased);
						writer.WriteEndObject();
					}
				}
				finally
				{
					enumerator6.Dispose();
				}
				writer.WriteEndArray();
			}
			if (triggers != null)
			{
				writer.WritePropertyName("triggers");
				SerializeTriggers(writer, triggers);
			}
			writer.WritePropertyName("lastLevelUpTime");
			writer.WriteValue(lastLevelUpTime);
			writer.WritePropertyName("lastGameStartTime");
			writer.WriteValue(lastGameStartTime);
			writer.WritePropertyName("firstGameStartTime");
			writer.WriteValue(firstGameStartTime);
			writer.WritePropertyName("lastPlayedTime");
			writer.WriteValue(lastPlayedTime);
			writer.WritePropertyName("totalGameplayDurationSinceLastLevelUp");
			writer.WriteValue(totalGameplayDurationSinceLastLevelUp);
			writer.WritePropertyName("totalAccumulatedGameplayDuration");
			writer.WriteValue(totalAccumulatedGameplayDuration);
			writer.WritePropertyName("targetExpansionID");
			writer.WriteValue(targetExpansionID);
			writer.WritePropertyName("timezoneOffset");
			writer.WriteValue(timezoneOffset);
			if (country != null)
			{
				writer.WritePropertyName("country");
				writer.WriteValue(country);
			}
			writer.WritePropertyName("completedOrders");
			writer.WriteValue(completedOrders);
			writer.WritePropertyName("highestFtueLevel");
			writer.WriteValue(highestFtueLevel);
			if (socialRewards != null)
			{
				writer.WritePropertyName("socialRewards");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<global::Kampai.Game.SocialClaimRewardItem> enumerator7 = socialRewards.GetEnumerator();
				try
				{
					while (enumerator7.MoveNext())
					{
						global::Kampai.Game.SocialClaimRewardItem current7 = enumerator7.Current;
						writer.WriteStartObject();
						writer.WritePropertyName("eventID");
						writer.WriteValue(current7.eventID);
						writer.WritePropertyName("claimState");
						writer.WriteValue((int)current7.claimState);
						writer.WriteEndObject();
					}
				}
				finally
				{
					enumerator7.Dispose();
				}
				writer.WriteEndArray();
			}
			if (mtxPurchaseTracking != null)
			{
				writer.WritePropertyName("mtxPurchaseTracking");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<string> enumerator8 = mtxPurchaseTracking.GetEnumerator();
				try
				{
					while (enumerator8.MoveNext())
					{
						string current8 = enumerator8.Current;
						writer.WriteValue(current8);
					}
				}
				finally
				{
					enumerator8.Dispose();
				}
				writer.WriteEndArray();
			}
			writer.WritePropertyName("completedQuestsTotal");
			writer.WriteValue(completedQuestsTotal);
			writer.WritePropertyName("currentItemCount");
			writer.WriteValue(currentItemCount);
			if (PlatformStoreTransactionIDs != null)
			{
				writer.WritePropertyName("PlatformStoreTransactionIDs");
				writer.WriteStartArray();
				global::System.Collections.Generic.IEnumerator<string> enumerator9 = PlatformStoreTransactionIDs.GetEnumerator();
				try
				{
					while (enumerator9.MoveNext())
					{
						string current9 = enumerator9.Current;
						writer.WriteValue(current9);
					}
				}
				finally
				{
					enumerator9.Dispose();
				}
				writer.WriteEndArray();
			}
			if (helpTipsTrackingData == null)
			{
				return;
			}
			writer.WritePropertyName("helpTipsTrackingData");
			writer.WriteStartArray();
			global::System.Collections.Generic.List<global::Kampai.Game.Player.HelpTipTrackingItem>.Enumerator enumerator10 = helpTipsTrackingData.GetEnumerator();
			try
			{
				while (enumerator10.MoveNext())
				{
					global::Kampai.Game.Player.HelpTipTrackingItem current10 = enumerator10.Current;
					writer.WriteStartObject();
					writer.WritePropertyName("tipDifinitionId");
					writer.WriteValue(current10.tipDifinitionId);
					writer.WritePropertyName("showsCount");
					writer.WriteValue(current10.showsCount);
					writer.WritePropertyName("lastShownTime");
					writer.WriteValue(current10.lastShownTime);
					writer.WriteEndObject();
				}
			}
			finally
			{
				enumerator10.Dispose();
			}
			writer.WriteEndArray();
		}

		public static void SerializeInventory(global::Newtonsoft.Json.JsonWriter writer, global::System.Collections.Generic.IList<global::Kampai.Game.Instance> items)
		{
			writer.WriteStartArray();
			for (int i = 0; i < items.Count; i++)
			{
				items[i].Serialize(writer);
			}
			writer.WriteEndArray();
		}

		public static void SerializeTriggers(global::Newtonsoft.Json.JsonWriter writer, global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggers)
		{
			if (triggers != null)
			{
				writer.WriteStartArray();
				for (int i = 0; i < triggers.Count; i++)
				{
					triggers[i].Serialize(writer);
				}
				writer.WriteEndArray();
			}
		}
	}
}
