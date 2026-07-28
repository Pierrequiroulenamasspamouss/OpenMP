namespace Kampai.UI
{
	public class BuildMenuService : global::Kampai.UI.IBuildMenuService
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("BuildMenuService") as global::Kampai.Util.IKampaiLogger;

		private global::Kampai.UI.BuildMenuLocalState localState;

		private global::System.Collections.Generic.Dictionary<int, int> storeItemDefinitionMap;

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistanceService { get; set; }

		[Inject]
		public global::Kampai.Game.IMasterPlanService masterPlanService { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetBadgeForStoreTabSignal setBadgeForTabSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetNewUnlockForStoreTabSignal setNewUnlockForTabSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetNewUnlockForBuildMenuSignal setNewUnlockForBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetInventoryCountForBuildMenuSignal setInventoryCountForBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localeService { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdateUIButtonsSignal updateStoreButtonsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.IncreaseInventoryCountForBuildMenuSignal increaseInventoryCountSignal { get; set; }

		[PostConstruct]
		public void PostConstruct()
		{
			LoadPersist();
		}

		public void RetoreBuidMenuState(global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> buttonViews)
		{
			UpdateNewUnlockList(buttonViews);
			if (localState.UncheckedInventoryItemOnTabs.Count <= 0)
			{
				return;
			}
			int num = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.IDictionary<int, bool>> uncheckedInventoryItemOnTab in localState.UncheckedInventoryItemOnTabs)
			{
				int num2 = 0;
				foreach (global::System.Collections.Generic.KeyValuePair<int, bool> item in uncheckedInventoryItemOnTab.Value)
				{
					if (!item.Value && playerService.GetUnlockedQuantityOfID(item.Key) > 0)
					{
						num2++;
					}
				}
				if (num2 > 0)
				{
					setBadgeForTabSignal.Dispatch(uncheckedInventoryItemOnTab.Key, num2);
					num += num2;
				}
			}
			if (num > 0)
			{
				setInventoryCountForBuildMenuSignal.Dispatch(num);
			}
		}

		public void SetStoreUnlockChecked()
		{
			global::System.Collections.Generic.List<global::Kampai.Util.Tuple<global::Kampai.Game.StoreItemType, int>> list = new global::System.Collections.Generic.List<global::Kampai.Util.Tuple<global::Kampai.Game.StoreItemType, int>>();
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.IDictionary<int, bool>> uncheckedInventoryItemOnTab in localState.UncheckedInventoryItemOnTabs)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<int, bool> item in uncheckedInventoryItemOnTab.Value)
				{
					list.Add(new global::Kampai.Util.Tuple<global::Kampai.Game.StoreItemType, int>(uncheckedInventoryItemOnTab.Key, item.Key));
				}
			}
			foreach (global::Kampai.Util.Tuple<global::Kampai.Game.StoreItemType, int> item2 in list)
			{
				localState.UncheckedInventoryItemOnTabs[item2.Item1][item2.Item2] = true;
			}
			PersistLocalState();
		}

		public void AddNewUnlockedItem(global::Kampai.Game.StoreItemType type, int buildingDefinitionID)
		{
			if (localState.NewUnlockedItemOnTabs.ContainsKey(type))
			{
				if (!localState.NewUnlockedItemOnTabs[type].Contains(buildingDefinitionID))
				{
					localState.NewUnlockedItemOnTabs[type].Add(buildingDefinitionID);
				}
				else
				{
					logger.Warning("New unlock list already contains this item {0}", buildingDefinitionID);
				}
			}
			else
			{
				global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
				list.Add(buildingDefinitionID);
				localState.NewUnlockedItemOnTabs.Add(type, list);
			}
			PersistLocalState();
		}

		public bool RemoveNewUnlockedItem(global::Kampai.Game.StoreItemType type, int buildingDefinitionID)
		{
			bool result = false;
			if (localState.NewUnlockedItemOnTabs.ContainsKey(type) && localState.NewUnlockedItemOnTabs[type].Contains(buildingDefinitionID))
			{
				localState.NewUnlockedItemOnTabs[type].Remove(buildingDefinitionID);
				if (localState.NewUnlockedItemOnTabs[type].Count == 0)
				{
					localState.NewUnlockedItemOnTabs.Remove(type);
					result = true;
					if (localState.UncheckedTabs.Contains(type))
					{
						setNewUnlockForTabSignal.Dispatch(type, 0);
						localState.UncheckedTabs.Remove(type);
					}
				}
				PersistLocalState();
			}
			return result;
		}

		public void ClearAllNewUnlockItems()
		{
			localState.UncheckedTabs.Clear();
			localState.NewUnlockedItemOnTabs.Clear();
		}

		public bool ShouldRenderStoreDef(global::Kampai.Game.StoreItemDefinition storeDef)
		{
			if (storeDef == null || storeDef.Disabled)
			{
				return false;
			}
			if (storeDef.SpecialEventID > 0 || storeDef.ReferencedDefID >= 4201 && storeDef.ReferencedDefID <= 4216)
			{
				global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] ShouldRenderStoreDef: StoreDef ID={0}, RefDefID={1}, SpecialEventID={2}, Disabled={3}", storeDef.ID, storeDef.ReferencedDefID, storeDef.SpecialEventID, storeDef.Disabled);
			}
			return true;
		}

		public bool ShowingAChild(global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> children, bool notifyShouldBeRendered = true)
		{
			if (children == null)
			{
				return false;
			}
			bool flag = false;
			foreach (global::Kampai.UI.View.StoreButtonView child in children)
			{
				if (child.storeItemDefinition.OnlyShowIfInInventory || child.storeItemDefinition.OnlyShowIfOwned || child.storeItemDefinition.OnlyShowIfUnlocked || child.storeItemDefinition.SpecialEventID > 0)
				{
					bool flag2 = ShouldRenderStoreDef(child.storeItemDefinition);
					flag = flag || flag2;
					if (notifyShouldBeRendered)
					{
						child.SetShouldBerendered(flag2);
					}
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		public void UpdateNewUnlockList(global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> buttonViews, bool updateBuildMenuButton = true, bool updateBadge = true)
		{
			global::System.Collections.Generic.Dictionary<int, int> buildingOnBoardCountMap = playerService.GetBuildingOnBoardCountMap();
			int num = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> buttonView in buttonViews)
			{
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				global::Kampai.Game.StoreItemType key = buttonView.Key;
				if (!ShowingAChild(buttonView.Value, false) && key == global::Kampai.Game.StoreItemType.SpecialEvent)
				{
					continue;
				}
				foreach (global::Kampai.UI.View.StoreButtonView item in buttonView.Value)
				{
					if (key == global::Kampai.Game.StoreItemType.Featured)
					{
						continue;
					}
					num3++;
					if (global::Kampai.UI.View.StoreButtonBuilder.DetermineUnlock(item, playerService, buildingOnBoardCountMap, definitionService, logger, timeService, localeService, masterPlanService, this))
					{
						if (!localState.UncheckedTabs.Contains(key))
						{
							localState.UncheckedTabs.Add(key);
						}
						AddNewUnlockedItem(key, item.definition.ID);
						num4++;
						num++;
						item.SetNewUnlockState(true);
					}
					else if (localState.NewUnlockedItemOnTabs.ContainsKey(key) && localState.NewUnlockedItemOnTabs[key].Contains(item.definition.ID))
					{
						item.SetNewUnlockState(true);
						num4++;
						num++;
					}
					item.SetShouldBerendered(true);
					bool flag = item.IsUnlocked();
					if (flag)
					{
						num2++;
					}
					if (!flag)
					{
						item.ItemIcon.gameObject.SetActive(false);
					}
				}
				if (updateBadge)
				{
					setNewUnlockForTabSignal.Dispatch(buttonView.Key, num4);
				}
			}
			if (updateBadge && updateBuildMenuButton && num > 0)
			{
				setNewUnlockForBuildMenuSignal.Dispatch(num);
			}
		}

		public void AddUncheckedInventoryItem(global::Kampai.Game.StoreItemType type, int buildingDefinitionID)
		{
			if (localState.UncheckedInventoryItemOnTabs.ContainsKey(type))
			{
				if (!localState.UncheckedInventoryItemOnTabs[type].ContainsKey(buildingDefinitionID))
				{
					localState.UncheckedInventoryItemOnTabs[type][buildingDefinitionID] = false;
				}
				else
				{
					logger.Warning("Unchecked list already contains this item {0}", buildingDefinitionID);
				}
			}
			else
			{
				localState.UncheckedInventoryItemOnTabs[type] = new global::System.Collections.Generic.Dictionary<int, bool>();
				localState.UncheckedInventoryItemOnTabs[type][buildingDefinitionID] = false;
			}
			int count = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<int, bool> item in localState.UncheckedInventoryItemOnTabs[type])
			{
				if (!item.Value && playerService.GetUnlockedQuantityOfID(item.Key) > 0)
				{
					count++;
				}
			}
			setBadgeForTabSignal.Dispatch(type, count);
			PersistLocalState();
		}

		public void RemoveUncheckedInventoryItem(global::Kampai.Game.StoreItemType type, int buildingDefinitionID)
		{
			if (localState.UncheckedInventoryItemOnTabs.ContainsKey(type))
			{
				if (localState.UncheckedInventoryItemOnTabs[type].ContainsKey(buildingDefinitionID))
				{
					localState.UncheckedInventoryItemOnTabs[type].Remove(buildingDefinitionID);
					int count = 0;
					foreach (global::System.Collections.Generic.KeyValuePair<int, bool> item in localState.UncheckedInventoryItemOnTabs[type])
					{
						if (!item.Value && playerService.GetUnlockedQuantityOfID(item.Key) > 0)
						{
							count++;
						}
					}
					if (localState.UncheckedInventoryItemOnTabs[type].Count == 0)
					{
						localState.UncheckedInventoryItemOnTabs.Remove(type);
					}
					setBadgeForTabSignal.Dispatch(type, count);
				}
				else
				{
					logger.Warning("Unchecked list doesn't contain this item {0}", buildingDefinitionID);
				}
				int num = 0;
				foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.IDictionary<int, bool>> uncheckedInventoryItemOnTab in localState.UncheckedInventoryItemOnTabs)
				{
					foreach (global::System.Collections.Generic.KeyValuePair<int, bool> item in uncheckedInventoryItemOnTab.Value)
					{
						if (!item.Value && playerService.GetUnlockedQuantityOfID(item.Key) > 0)
						{
							num++;
						}
					}
				}
				setInventoryCountForBuildMenuSignal.Dispatch(num);
				PersistLocalState();
			}
			else
			{
				logger.Warning("Unchecked list doesn't contain this type {0}", type);
			}
		}

		public void ClearTab(global::Kampai.Game.StoreItemType type)
		{
			localState.NewUnlockedItemOnTabs.Remove(type);
			if (localState.UncheckedInventoryItemOnTabs.ContainsKey(type))
			{
				localState.UncheckedInventoryItemOnTabs.Remove(type);
			}
			if (localState.UncheckedTabs.Contains(type))
			{
				localState.UncheckedTabs.Remove(type);
			}
			PersistLocalState();
		}

		public int GetStoreItemDefinitionIDFromBuildingID(int buildingID)
		{
			if (storeItemDefinitionMap == null)
			{
				storeItemDefinitionMap = new global::System.Collections.Generic.Dictionary<int, int>();
				global::System.Collections.Generic.IList<global::Kampai.Game.StoreItemDefinition> all = definitionService.GetAll<global::Kampai.Game.StoreItemDefinition>();
				foreach (global::Kampai.Game.StoreItemDefinition item in all)
				{
					if (item.ReferencedDefID != 0)
					{
						storeItemDefinitionMap[item.ReferencedDefID] = item.ID;
					}
				}
			}
			if (storeItemDefinitionMap.ContainsKey(buildingID))
			{
				return storeItemDefinitionMap[buildingID];
			}
			return 0;
		}

		public void CompleteBuildMenuUpdate(BuildingType.BuildingTypeIdentifier buildingDefType, int buildingDefinitionID)
		{
			switch (buildingDefType)
			{
			case BuildingType.BuildingTypeIdentifier.RESOURCE:
				AddUncheckedInventoryItem(global::Kampai.Game.StoreItemType.BaseResource, buildingDefinitionID);
				increaseInventoryCountSignal.Dispatch();
				break;
			case BuildingType.BuildingTypeIdentifier.LEISURE:
				AddUncheckedInventoryItem(global::Kampai.Game.StoreItemType.Leisure, buildingDefinitionID);
				increaseInventoryCountSignal.Dispatch();
				break;
			case BuildingType.BuildingTypeIdentifier.CRAFTING:
				AddUncheckedInventoryItem(global::Kampai.Game.StoreItemType.Crafting, buildingDefinitionID);
				increaseInventoryCountSignal.Dispatch();
				break;
			case BuildingType.BuildingTypeIdentifier.DECORATION:
				AddUncheckedInventoryItem(global::Kampai.Game.StoreItemType.Decoration, buildingDefinitionID);
				increaseInventoryCountSignal.Dispatch();
				break;
			case BuildingType.BuildingTypeIdentifier.MASTER_LEFTOVER:
				AddUncheckedInventoryItem(global::Kampai.Game.StoreItemType.MasterPlanLeftOvers, buildingDefinitionID);
				updateStoreButtonsSignal.Dispatch(false);
				increaseInventoryCountSignal.Dispatch();
				break;
			}
		}

		private void PersistLocalState()
		{
			if (localState != null)
			{
				try
				{
					string data = global::Newtonsoft.Json.JsonConvert.SerializeObject(localState);
					localPersistanceService.PutDataPlayer("BuildMenuLocalSave", data);
					return;
				}
				catch (global::Newtonsoft.Json.JsonSerializationException ex)
				{
					logger.Error("PersistLocalState(): Json Parse Err: {0}", ex);
					return;
				}
			}
			localPersistanceService.DeleteKeyPlayer("BuildMenuLocalSave");
		}

		private void LoadPersist()
		{
			if (localPersistanceService.HasKeyPlayer("BuildMenuLocalSave"))
			{
				string dataPlayer = localPersistanceService.GetDataPlayer("BuildMenuLocalSave");
				if (dataPlayer != null)
				{
					try
					{
						localState = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.UI.BuildMenuLocalState>(dataPlayer);
					}
					catch (global::Newtonsoft.Json.JsonSerializationException e)
					{
						HandleJsonException(e);
					}
					catch (global::Newtonsoft.Json.JsonReaderException e2)
					{
						HandleJsonException(e2);
					}
				}
			}
			if (localState == null)
			{
				localState = new global::Kampai.UI.BuildMenuLocalState();
			}
		}

		private void HandleJsonException(global::System.Exception e)
		{
			logger.Error("BuildMenuService.LoadFromPersistence(): Json Parse Err: {0}", e);
		}
	}
}
