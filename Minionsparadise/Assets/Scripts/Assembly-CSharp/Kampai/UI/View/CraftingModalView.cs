namespace Kampai.UI.View
{
	public class CraftingModalView : global::Kampai.UI.View.PopupMenuView
	{
		public global::UnityEngine.UI.Text title;

		public global::UnityEngine.UI.Text partyBoostText;

		public global::Kampai.UI.View.ButtonView backArrow;

		public global::Kampai.UI.View.ButtonView forwardArrow;

		public global::System.Collections.Generic.List<global::UnityEngine.RectTransform> recipeLocations;

		public global::System.Collections.Generic.List<global::UnityEngine.RectTransform> oneOffLocations;

		public global::UnityEngine.RectTransform queueScrollView;

		public global::UnityEngine.GameObject PartyPanel;

		public global::Kampai.UI.View.ButtonView freeRush;

		public global::Kampai.UI.View.HelpTipButtonView helpTipButtonView;

		public global::UnityEngine.RectTransform helpPopoupParent;

		private global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.Game.IPlayerService playerService;

		private global::Kampai.Game.IQuestService questService;

		internal global::Kampai.Game.CraftingBuilding building;

		internal global::Kampai.Game.View.CraftableBuildingObject buildingObject;

		private global::System.Collections.Generic.List<global::UnityEngine.GameObject> recipeIcons = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

		private global::System.Collections.Generic.List<global::Kampai.UI.View.CraftingQueueView> queueIcons = new global::System.Collections.Generic.List<global::Kampai.UI.View.CraftingQueueView>();

		private global::System.Collections.Generic.List<int> availableOneOffs = new global::System.Collections.Generic.List<int>();

		internal bool highlightItem;

		internal int higlightItemId;

		private global::UnityEngine.GameObject dragTarget;

		private bool isProductionBuff;

		private float queueWidth;

		private float queuePadding;

		private global::Kampai.UI.View.CraftingRecipeView dragTutorialView;

		private int dragTutorialId;

		private global::UnityEngine.GameObject crvCopy;

		private GoTween tutorialPosition;

		private GoTween tutorialFade;

		private GoTweenChain tutorialChain;

		internal void Init(global::Kampai.Game.IPlayerService playerService, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.IQuestService questService, global::Kampai.Game.CraftingBuilding building, global::Kampai.Game.View.CraftableBuildingObject buildingObject)
		{
			base.Init();
			this.playerService = playerService;
			this.definitionService = definitionService;
			this.questService = questService;
			this.building = building;
			this.buildingObject = buildingObject;
			helpTipButtonView.rectTransform = helpPopoupParent;
			StoreOneOffCraftables();
			PopulateRecipeIcons(global::Kampai.UI.View.HighlightType.DRAG);
			PopulateQueueIcons();
			base.Open();
		}

		internal void SetTitle(string localizedString)
		{
			title.text = localizedString;
		}

		internal void SetPartyInfo(float boost, string boostString, bool isOn = true)
		{
			partyBoostText.text = boostString;
			isProductionBuff = (int)(boost * 100f) != 100;
			PartyPanel.SetActive(isOn && isProductionBuff);
			SetChildrenAsPartying();
		}

		public void SetChildrenAsPartying()
		{
			foreach (global::Kampai.UI.View.CraftingQueueView queueIcon in queueIcons)
			{
				queueIcon.isCorrectBuffType = isProductionBuff;
			}
		}

		internal void RePopulateModal(global::Kampai.Game.CraftingBuilding building, global::Kampai.Game.View.CraftableBuildingObject buildingObject, global::Kampai.UI.View.HighlightType type)
		{
			this.building = building;
			if (this.buildingObject != null)
			{
				this.buildingObject.DisableHighLightBuilding();
			}
			this.buildingObject = buildingObject;
			CleanupRecipeIcons();
			StoreOneOffCraftables();
			PopulateRecipeIcons(type);
			RefreshQueue();
		}

		internal void RefreshQueue()
		{
			CleanupQueueIcons();
			PopulateQueueIcons();
		}

		private void StoreOneOffCraftables()
		{
			availableOneOffs.Clear();
			for (int i = 0; i < building.Definition.RecipeDefinitions.Count; i++)
			{
				int itemID = building.Definition.RecipeDefinitions[i].ItemID;
				global::Kampai.Game.DynamicIngredientsDefinition definition;
				if (definitionService.TryGet<global::Kampai.Game.DynamicIngredientsDefinition>(itemID, out definition) && IsDynamicRecipeAvailable(building, definition))
				{
					availableOneOffs.Add(itemID);
				}
			}
		}

		private global::UnityEngine.Transform GetOneOffLocation(int count)
		{
			if (availableOneOffs.Count == 1)
			{
				return oneOffLocations[0];
			}
			if (availableOneOffs.Count == 2)
			{
				global::UnityEngine.Vector2 anchoredPosition = oneOffLocations[count].anchoredPosition;
				oneOffLocations[count].anchoredPosition = new global::UnityEngine.Vector2(anchoredPosition.x, anchoredPosition.y + 60f);
				return oneOffLocations[count];
			}
			return oneOffLocations[count - 1];
		}

		private void PopulateRecipeIcons(global::Kampai.UI.View.HighlightType type)
		{
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>("cmp_CraftingRecipe");
			int num = 0;
			int num2 = 0;
			global::System.Collections.Generic.List<global::Kampai.Game.RecipeDefinition> list = building.Definition.RecipeDefinitions as global::System.Collections.Generic.List<global::Kampai.Game.RecipeDefinition>;
			list.Sort(delegate(global::Kampai.Game.RecipeDefinition r1, global::Kampai.Game.RecipeDefinition r2)
			{
				global::Kampai.Game.DynamicIngredientsDefinition definition2;
				if (definitionService.TryGet<global::Kampai.Game.DynamicIngredientsDefinition>(r1.ItemID, out definition2))
				{
					return 1;
				}
				if (definitionService.TryGet<global::Kampai.Game.DynamicIngredientsDefinition>(r2.ItemID, out definition2))
				{
					return -1;
				}
				int levelItemUnlocksAt = definitionService.GetLevelItemUnlocksAt(r1.ItemID);
				int levelItemUnlocksAt2 = definitionService.GetLevelItemUnlocksAt(r2.ItemID);
				int num4 = levelItemUnlocksAt.CompareTo(levelItemUnlocksAt2);
				return (num4 != 0) ? num4 : r1.ItemID.CompareTo(r2.ItemID);
			});
			for (int num3 = 0; num3 < list.Count; num3++)
			{
				bool flag = false;
				int itemID = list[num3].ItemID;
				global::Kampai.Game.DynamicIngredientsDefinition definition;
				global::UnityEngine.GameObject gameObject;
				if (definitionService.TryGet<global::Kampai.Game.DynamicIngredientsDefinition>(itemID, out definition))
				{
					if (!availableOneOffs.Contains(itemID))
					{
						continue;
					}
					flag = true;
					num++;
					gameObject = global::UnityEngine.Object.Instantiate(original);
					global::UnityEngine.Transform oneOffLocation = GetOneOffLocation(num);
					gameObject.transform.SetParent(oneOffLocation, false);
				}
				else
				{
					gameObject = global::UnityEngine.Object.Instantiate(original);
					gameObject.transform.SetParent(recipeLocations[num2], false);
					num2++;
				}
				global::Kampai.UI.View.CraftingRecipeView component = gameObject.GetComponent<global::Kampai.UI.View.CraftingRecipeView>();
				component.IsValidDragAreaSignal.AddListener(HighlightBuildingObject);
				if (flag)
				{
					component.groupQuantity.gameObject.SetActive(false);
				}
				if (highlightItem && higlightItemId == itemID)
				{
					switch (type)
					{
					case global::Kampai.UI.View.HighlightType.THROB:
						component.isHighlighted = true;
						break;
					case global::Kampai.UI.View.HighlightType.DRAG:
						dragTutorialView = component;
						dragTutorialId = itemID;
						break;
					default:
						logger.Error("Unknown highlight type {0}", type);
						break;
					}
				}
				component.recipeID = itemID;
				component.instanceID = building.ID;
				recipeIcons.Add(gameObject);
			}
		}

		public override void FinishedOpening()
		{
			ShowDragTutorial();
		}

		public void ShowDragTutorial()
		{
			if (dragTutorialView != null)
			{
				EnableDragTutorial(dragTutorialId, dragTutorialView);
				dragTutorialView = null;
			}
		}

		public void CleanupTweens()
		{
			global::Kampai.Util.TweenUtil.Cleanup(ref tutorialFade);
			global::Kampai.Util.TweenUtil.Cleanup(ref tutorialPosition);
			global::Kampai.Util.TweenUtil.Cleanup(ref tutorialChain);
			if (crvCopy != null)
			{
				global::UnityEngine.Object.Destroy(crvCopy);
				crvCopy = null;
			}
		}

		private void EnableDragTutorial(int itemId, global::Kampai.UI.View.CraftingRecipeView crv)
		{
			CleanupTweens();
			crvCopy = CreateItemCopy(itemId, crv);
			tutorialPosition = Go.to(crvCopy.transform, 2f, new GoTweenConfig().position(DragTarget().position).setIterations(-1, GoLoopType.RestartFromBeginning));
			tutorialFade = new GoTween(crvCopy.GetComponent<global::Kampai.UI.View.KampaiImage>(), 1f, new GoTweenConfig().colorProp("color", new global::UnityEngine.Color(1f, 1f, 1f, 0f)));
			tutorialChain = new GoTweenChain(new GoTweenCollectionConfig().setIterations(-1, GoLoopType.RestartFromBeginning));
			tutorialChain.append(tutorialFade).appendDelay(1f);
			tutorialChain.autoRemoveOnComplete = true;
			tutorialChain.play();
		}

		private global::UnityEngine.GameObject CreateItemCopy(int itemId, global::Kampai.UI.View.CraftingRecipeView crv)
		{
			global::Kampai.Game.IngredientsItemDefinition ingredientsItemDefinition = definitionService.Get<global::Kampai.Game.IngredientsItemDefinition>(itemId);
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("hint");
			gameObject.transform.SetParent(crv.unlockedImage.transform.parent, false);
			gameObject.layer = 5;
			global::Kampai.UI.View.KampaiImage kampaiImage = gameObject.AddComponent<global::Kampai.UI.View.KampaiImage>();
			kampaiImage.sprite = UIUtils.LoadSpriteFromPath(ingredientsItemDefinition.Image);
			kampaiImage.maskSprite = UIUtils.LoadSpriteFromPath(ingredientsItemDefinition.Mask);
			kampaiImage.material = crv.unlockedImage.material;
			kampaiImage.preserveAspect = true;
			global::UnityEngine.RectTransform component = gameObject.GetComponent<global::UnityEngine.RectTransform>();
			global::UnityEngine.RectTransform component2 = crv.unlockedImage.gameObject.GetComponent<global::UnityEngine.RectTransform>();
			global::Kampai.Util.RectUtil.Copy(component2, component);
			return gameObject;
		}

		private void HighlightBuildingObject(bool isValidArea, bool canCraftRecipe)
		{
			if (isValidArea)
			{
				buildingObject.EnableHighLightBuilding(canCraftRecipe);
			}
			else
			{
				buildingObject.DisableHighLightBuilding();
			}
		}

		private void CleanupRecipeIcons()
		{
			CleanupTweens();
			foreach (global::UnityEngine.GameObject recipeIcon in recipeIcons)
			{
				global::UnityEngine.Object.Destroy(recipeIcon);
			}
		}

		private void PopulateQueueIcons()
		{
			if (building == null)
			{
				return;
			}
			int num = building.Slots + 1;
			if (num > building.Definition.MaxQueueSlots)
			{
				num = building.Definition.MaxQueueSlots;
			}
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load("cmp_CraftQueue") as global::UnityEngine.GameObject;
			queueWidth = (gameObject.transform as global::UnityEngine.RectTransform).sizeDelta.x;
			queuePadding = queueWidth / 5f;
			for (int i = 0; i < num; i++)
			{
				global::UnityEngine.GameObject gameObject2 = SetupQueueView(i, gameObject);
				global::UnityEngine.RectTransform rectTransform = gameObject2.transform as global::UnityEngine.RectTransform;
				if (i == 0)
				{
					rectTransform.offsetMin = new global::UnityEngine.Vector2(queueWidth * (float)i, 0f);
					rectTransform.offsetMax = new global::UnityEngine.Vector2(queueWidth * (float)(i + 1), 0f);
				}
				else
				{
					rectTransform.offsetMin = new global::UnityEngine.Vector2(queueWidth * (float)i + queuePadding, 0f);
					rectTransform.offsetMax = new global::UnityEngine.Vector2(queueWidth * (float)(i + 1) + queuePadding, 0f);
				}
			}
			int num2 = 3 * (int)queueWidth;
			int num3 = num * (int)queueWidth + (int)queuePadding;
			int num4 = 0;
			if (num3 > num2)
			{
				num4 = (num3 - num2) / 2;
			}
			queueScrollView.sizeDelta = new global::UnityEngine.Vector2(num3, 0f);
			queueScrollView.localPosition = new global::UnityEngine.Vector2(num4, queueScrollView.localPosition.y);
		}

		private global::UnityEngine.GameObject SetupQueueView(int index, global::UnityEngine.GameObject prefab)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(prefab);
			gameObject.transform.SetParent(queueScrollView, false);
			global::UnityEngine.RectTransform rectTransform = gameObject.transform as global::UnityEngine.RectTransform;
			rectTransform.localPosition = new global::UnityEngine.Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
			rectTransform.localScale = global::UnityEngine.Vector3.one;
			global::Kampai.UI.View.CraftingQueueView component = gameObject.GetComponent<global::Kampai.UI.View.CraftingQueueView>();
			component.index = index;
			component.building = building;
			if (index < building.Slots)
			{
				if (index > 0)
				{
					rectTransform.localScale *= 0.8f;
				}
				else
				{
					rectTransform.pivot = global::UnityEngine.Vector2.one / 2f;
				}
				component.inProgressPanel.gameObject.SetActive(false);
				component.availablePanel.gameObject.SetActive(true);
				component.lockedPanel.gameObject.SetActive(false);
			}
			else
			{
				rectTransform.localScale *= 0.8f;
				component.isLocked = true;
				component.availablePanel.gameObject.SetActive(false);
				component.inProgressPanel.gameObject.SetActive(false);
				component.lockedPanel.gameObject.SetActive(true);
				component.purchaseCost = building.getNextIncrementalCost();
				component.lockedCost.text = component.purchaseCost.ToString();
			}
			queueIcons.Add(component);
			return gameObject;
		}

		internal global::Kampai.UI.View.CraftingQueueView GetFirstQueueItem()
		{
			return (queueIcons.Count <= 0) ? null : queueIcons[0];
		}

		private void CleanupQueueIcons()
		{
			foreach (global::Kampai.UI.View.CraftingQueueView queueIcon in queueIcons)
			{
				if (queueIcon != null && queueIcon.gameObject != null)
				{
					global::UnityEngine.Object.Destroy(queueIcon.gameObject);
				}
			}
			queueIcons.Clear();
		}

		internal void UpdateQueuePosition()
		{
		}

		internal void SetArrowButtonState(bool enable)
		{
			backArrow.GetComponent<global::UnityEngine.UI.Button>().interactable = enable;
			forwardArrow.GetComponent<global::UnityEngine.UI.Button>().interactable = enable;
		}

		private bool IsDynamicRecipeAvailable(global::Kampai.Game.CraftingBuilding fromBuilding, global::Kampai.Game.DynamicIngredientsDefinition definition)
		{
			int iD = definition.ID;
			int num = 0;
			num = questService.IsOneOffCraftableDisplayable(definition.QuestDefinitionUnlockId, iD);
			if (num == 0)
			{
				return false;
			}
			int quantityByDefinitionId = (int)playerService.GetQuantityByDefinitionId(iD);
			if (quantityByDefinitionId >= num)
			{
				return false;
			}
			int num2 = 0;
			global::System.Collections.Generic.ICollection<global::Kampai.Game.CraftingBuilding> byDefinitionId = playerService.GetByDefinitionId<global::Kampai.Game.CraftingBuilding>(fromBuilding.Definition.ID);
			foreach (global::Kampai.Game.CraftingBuilding item in byDefinitionId)
			{
				foreach (int item2 in item.RecipeInQueue)
				{
					if (item2 == iD)
					{
						num2++;
					}
				}
				foreach (int completedCraft in item.CompletedCrafts)
				{
					if (completedCraft == iD)
					{
						num2++;
					}
				}
			}
			if (quantityByDefinitionId + num2 >= num)
			{
				return false;
			}
			return true;
		}

		internal void ResetDoubleTap(int viewId)
		{
			foreach (global::Kampai.UI.View.CraftingQueueView queueIcon in queueIcons)
			{
				if (queueIcon.index != viewId)
				{
					if (queueIcon.inProduction && queueIcon.inProgressRush != null)
					{
						queueIcon.inProgressRush.ResetTapState();
						queueIcon.inProgressRush.ResetAnim();
					}
					else if (queueIcon.isLocked && queueIcon.lockedPurchase != null)
					{
						queueIcon.lockedPurchase.ResetTapState();
						queueIcon.lockedPurchase.ResetAnim();
					}
				}
			}
		}

		private global::UnityEngine.Transform DragTarget()
		{
			if (dragTarget == null)
			{
				dragTarget = base.gameObject.FindChild("drag_hint");
				if (dragTarget == null)
				{
					logger.Error("No drag target found");
				}
			}
			return dragTarget.transform;
		}

		public override void Close(bool instant = false)
		{
			base.Close(instant);
			CleanupTweens();
		}

		public void EnableRewardedAdRushButton(bool enable)
		{
			freeRush.gameObject.SetActive(enable);
		}
	}
}
