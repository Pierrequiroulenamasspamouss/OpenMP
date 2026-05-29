namespace Kampai.UI.View
{
	public static class StoreButtonBuilder
	{
		private static readonly string[] Textures = new string[4] { "icn_currency_Grind_fill", "icn_currency_Grind_mask", "icn_currency_premium_fill", "icn_currency_premium_mask" };

		public static global::Kampai.UI.View.StoreButtonView Build(global::Kampai.Game.Definition definition, global::Kampai.Game.Transaction.TransactionDefinition transaction, global::Kampai.Game.StoreItemDefinition storeItemDefinition, global::UnityEngine.Transform i_parent, global::Kampai.Main.ILocalizationService localService, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Util.IKampaiLogger logger, global::Kampai.Game.IPlayerService playerService)
		{
			if (definition == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.EX_NULL_ARG);
			}
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("cmp_SubMenuItem") as global::UnityEngine.GameObject;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
			global::Kampai.UI.View.StoreButtonView component = gameObject.GetComponent<global::Kampai.UI.View.StoreButtonView>();
			component.init(playerService);
			component.ItemName.text = localService.GetStringUpper(definition.LocalizedKey);
			component.definition = definition;
			component.transactionDef = transaction;
			component.storeItemDefinition = storeItemDefinition;
			global::Kampai.Game.DisplayableDefinition displayableDefinition = definition as global::Kampai.Game.DisplayableDefinition;
			if (displayableDefinition != null)
			{
				component.ItemDescription.text = localService.GetString(displayableDefinition.Description);
				component.UpdatePartyPointText(localService);
				if (string.IsNullOrEmpty(displayableDefinition.Mask))
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Your Building Definition: {0} doesn' have a mask image defined for the building icon: {1}", displayableDefinition.ID, displayableDefinition.Image);
					displayableDefinition.Mask = "btn_Circle01_mask";
				}
				string key = ((component.storeItemDefinition.Type != global::Kampai.Game.StoreItemType.MasterPlanLeftOvers) ? "UnlockAt" : "UnlockForMPLeaveBehind");
				component.UnlockedAtLevel.text = localService.GetString(key, definitionService.GetLevelItemUnlocksAt(displayableDefinition.ID));
			}
			global::UnityEngine.RectTransform rectTransform = gameObject.transform as global::UnityEngine.RectTransform;
			rectTransform.SetParent(i_parent, false);
			rectTransform.SetAsFirstSibling();
			rectTransform.localPosition = new global::UnityEngine.Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
			rectTransform.localScale = global::UnityEngine.Vector3.one;
			gameObject.SetActive(false);
			return component;
		}

		public static bool DetermineUnlock(global::Kampai.UI.View.StoreButtonView view, global::Kampai.Game.IPlayerService playerService, global::System.Collections.Generic.Dictionary<int, int> countMap, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Util.IKampaiLogger logger, global::Kampai.Game.ITimeService timeService, global::Kampai.Main.ILocalizationService localeService, global::Kampai.Game.IMasterPlanService masterPlanService, global::Kampai.UI.IBuildMenuService buildMenuService)
		{
			if (view.definition == null || view.definition.Disabled)
			{
				view.SetShouldBerendered(false);
				return false;
			}
			bool result = false;
			int iD = view.definition.ID;
			global::Kampai.Game.DisplayableDefinition displayableDefinition = view.definition as global::Kampai.Game.DisplayableDefinition;
			global::Kampai.Game.StoreItemDefinition storeItemDefinition = view.storeItemDefinition;
			int num = playerService.GetUnlockedQuantityOfID(iD);
			bool flag = false; // Force unlocked
			if (string.IsNullOrEmpty(displayableDefinition.Image))
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Your Building Definition: {0} doesn' have a image defined", displayableDefinition.ID);
				displayableDefinition.Image = "btn_Circle01_mask";
			}
			if (((flag && num == 0) || IsMasterPlanItemForIncompletePlan(storeItemDefinition, iD, definitionService, masterPlanService)))
			{
				ItemLocked(view);
			}
			else
			{
				if (!buildMenuService.ShouldRenderStoreDef(storeItemDefinition))
				{
					view.SetShouldBerendered(false);
					return false;
				}
				view.SetShouldBerendered(true);
				global::UnityEngine.Sprite sprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Image);
				global::UnityEngine.Sprite sprite2 = UIUtils.LoadSpriteFromPath(displayableDefinition.Mask);
				if (sprite != null && sprite2 != null)
				{
					global::Kampai.UI.View.KampaiImage itemIcon = view.ItemIcon;
					itemIcon.sprite = sprite;
					itemIcon.maskSprite = sprite2;
					view.DragSpritePath = displayableDefinition.Image;
					view.DragMaskPath = displayableDefinition.Mask;
					view.DragAnimationController = "asm_BuildStore_StoreDragHint";
				}
				CheckBadge(view);
				result = CheckLocked(view);
				result = (num != 0 && result) || (view.CurrentCapacity > 0 && (view.CurrentCapacity < num || num < 0) && !result);
				if (storeItemDefinition.SpecialEventID > 0)
				{
					global::Kampai.Game.SpecialEventItemDefinition definition;
					bool flag2 = definitionService.TryGet<global::Kampai.Game.SpecialEventItemDefinition>(storeItemDefinition.SpecialEventID, out definition);
					if ((flag2 && !definition.IsActive) || !flag2)
					{
						num = playerService.GetCountByDefinitionId(iD);
					}
				}
				if (!storeItemDefinition.IsOnSale(global::UnityEngine.Application.platform, timeService, localeService, logger))
				{
					num = playerService.GetCountByDefinitionId(iD);
				}
				int buildingCount = UpdateBuildingCount(view, iD, num, countMap);
				int incrementalCost = GetIncrementalCost(view, buildingCount, definitionService, playerService);
				CheckForTransactions(view, incrementalCost);
				view.DisplayOrHideUnlockedCostIcons();
			}
			return result;
		}

		private static int UpdateBuildingCount(global::Kampai.UI.View.StoreButtonView view, int itemDefintionId, int capacity, global::System.Collections.Generic.Dictionary<int, int> countMap)
		{
			view.SetCapacity(capacity);
			int num = 0;
			if (countMap.ContainsKey(itemDefintionId))
			{
				num = countMap[itemDefintionId];
				view.SetBuildingCount(num);
			}
			return num;
		}

		private static void CheckForTransactions(global::Kampai.UI.View.StoreButtonView view, int totalIncrementalCost)
		{
			if (view.transactionDef != null)
			{
				if (global::Kampai.Game.Transaction.TransactionUtil.IsOnlyPremiumInputs(view.transactionDef))
				{
					view.Cost.text = UIUtils.FormatLargeNumber(global::Kampai.Game.Transaction.TransactionUtil.SumOutputsForStaticItem(view.transactionDef, global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID, true) + totalIncrementalCost);
					view.MoneyIcon.sprite = UIUtils.LoadSpriteFromPath(Textures[2]);
					view.MoneyIcon.maskSprite = UIUtils.LoadSpriteFromPath(Textures[3]);
				}
				else
				{
					view.Cost.text = UIUtils.FormatLargeNumber(global::Kampai.Game.Transaction.TransactionUtil.SumOutputsForStaticItem(view.transactionDef, global::Kampai.Game.StaticItem.GRIND_CURRENCY_ID, true) + totalIncrementalCost);
					view.MoneyIcon.sprite = UIUtils.LoadSpriteFromPath(Textures[0]);
					view.MoneyIcon.maskSprite = UIUtils.LoadSpriteFromPath(Textures[1]);
					view.DisableDoubleConfirm();
				}
			}
			else
			{
				view.Cost.text = "0";
				view.MoneyIcon.sprite = UIUtils.LoadSpriteFromPath(Textures[0]);
				view.MoneyIcon.maskSprite = UIUtils.LoadSpriteFromPath(Textures[1]);
			}
		}

		private static bool CheckLocked(global::Kampai.UI.View.StoreButtonView view)
		{
			return view.ChangeStateToUnlocked();
		}

		private static void CheckBadge(global::Kampai.UI.View.StoreButtonView view)
		{
			view.ItemBadge.HideNew();
		}

		private static void ItemLocked(global::Kampai.UI.View.StoreButtonView view)
		{
			view.ChangeStateToLocked();
		}

		private static int GetIncrementalCost(global::Kampai.UI.View.StoreButtonView view, int buildingCount, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.IPlayerService playerService)
		{
			int incrementalCost = definitionService.GetIncrementalCost(view.definition);
			int num = 0;
			if (incrementalCost > 0)
			{
				num = playerService.GetInventoryCountByDefinitionID(view.definition.ID);
			}
			return (buildingCount + num) * incrementalCost;
		}

		private static bool IsMasterPlanItemForIncompletePlan(global::Kampai.Game.StoreItemDefinition storeItemDef, int itemDefinitionId, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.IMasterPlanService masterPlanService)
		{
			if (storeItemDef.Type != global::Kampai.Game.StoreItemType.MasterPlanLeftOvers)
			{
				return false;
			}
			global::System.Collections.Generic.List<global::Kampai.Game.MasterPlanDefinition> all = definitionService.GetAll<global::Kampai.Game.MasterPlanDefinition>();
			foreach (global::Kampai.Game.MasterPlanDefinition item in all)
			{
				if (itemDefinitionId == item.LeavebehindBuildingDefID)
				{
					if (!masterPlanService.HasReceivedInitialRewardFromPlanDefinition(item))
					{
						return true;
					}
					break;
				}
			}
			return false;
		}
	}
}
