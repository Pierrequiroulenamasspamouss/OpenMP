namespace Kampai.UI.View
{
	public class StoreButtonView : global::Kampai.UI.View.DoubleConfirmButtonView, global::UnityEngine.EventSystems.IDragHandler, global::UnityEngine.EventSystems.IEventSystemHandler
	{
		public global::UnityEngine.UI.Text ItemName;

		public global::UnityEngine.UI.Text ItemDescription;

		public global::UnityEngine.UI.Text ItemPartyPoints;

		public global::UnityEngine.UI.Text Capacity;

		public global::UnityEngine.UI.Text Cost;

		public global::UnityEngine.UI.Image CostBacking;

		public global::Kampai.UI.View.StoreBadgeView ItemBadge;

		public global::Kampai.UI.View.KampaiImage ItemIcon;

		public string DragSpritePath;

		public string DragMaskPath;

		public string DragAnimationController;

		public global::Kampai.UI.View.KampaiImage MoneyIcon;

		public global::UnityEngine.UI.Text UnlockedAtLevel;

		public global::UnityEngine.GameObject Locked;

		public global::UnityEngine.GameObject Unlocked;

		public global::UnityEngine.GameObject Highlighted;

		public float PaddingInPixels;

		public global::UnityEngine.Transform BackingImageRect;

		public global::Kampai.UI.View.KampaiImage Arrow;

		public global::UnityEngine.Color LockedTopBackingColor;

		public global::UnityEngine.Color CapacityReachedBackingColor;

		public new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Definition> ClickedSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Definition>();

		public global::strange.extensions.signal.impl.Signal BlockedSignal = new global::strange.extensions.signal.impl.Signal();

		public global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData, global::Kampai.Game.Definition, global::Kampai.Game.Transaction.TransactionDefinition, bool> pointerDownSignal = new global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData, global::Kampai.Game.Definition, global::Kampai.Game.Transaction.TransactionDefinition, bool>();

		public global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData, global::Kampai.Game.Definition, global::Kampai.Game.Transaction.TransactionDefinition, int> pointerDragSignal = new global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData, global::Kampai.Game.Definition, global::Kampai.Game.Transaction.TransactionDefinition, int>();

		public global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData, global::Kampai.Game.Definition, global::Kampai.Game.Transaction.TransactionDefinition> pointerUpSignal = new global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData, global::Kampai.Game.Definition, global::Kampai.Game.Transaction.TransactionDefinition>();

		private global::Kampai.Game.IPlayerService playerService;

		private global::UnityEngine.Vector3 originalScale;

		private global::UnityEngine.Vector3 starOriginalScale;

		private int currentBadgeCount;

		private bool shouldBeRendered;

		private int currentBuildingCount;

		private global::UnityEngine.Color defaultTopLeftBackingColor;

		private global::Kampai.UI.View.KampaiButton myButton;

		private bool isInCapacityReachedState;

		private bool isFunRewarding;

		private global::UnityEngine.GameObject dragPromptItem;

		public global::Kampai.Game.Definition definition { get; set; }

		public global::Kampai.Game.Transaction.TransactionDefinition transactionDef { get; set; }

		public global::Kampai.Game.StoreItemDefinition storeItemDefinition { get; set; }

		public int CurrentCapacity { get; private set; }

		public void init(global::Kampai.Game.IPlayerService plService)
		{
			playerService = plService;
			originalScale = Highlighted.transform.localScale;
			starOriginalScale = global::UnityEngine.Vector3.one;
			myButton = GetComponent<global::Kampai.UI.View.KampaiButton>();
			global::UnityEngine.UI.Image component = BackingImageRect.GetChild(0).GetComponent<global::UnityEngine.UI.Image>();
			if ((bool)component)
			{
				defaultTopLeftBackingColor = component.color;
			}
			else
			{
				defaultTopLeftBackingColor = global::UnityEngine.Color.gray;
			}
		}

		public void OnEnable()
		{
			ResetTapState();
		}

		public override void OnClickEvent()
		{
			ClickedSignal.Dispatch(definition);
		}

		public override void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!isDoubleConfirmed())
			{
				base.OnClickEvent();
			}
			bool flag = isDoubleConfirmed();
			pointerDownSignal.Dispatch(eventData, definition, transactionDef, flag && (currentBuildingCount < CurrentCapacity || CurrentCapacity < 0));
			if (flag)
			{
				ResetTapState();
			}
		}

		public void OnDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			pointerDragSignal.Dispatch(eventData, definition, transactionDef, currentBadgeCount);
		}

		public override void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			pointerUpSignal.Dispatch(eventData, definition, transactionDef);
		}

		internal void SetNewUnlockState(bool isNewThingUnlocked)
		{
			if (isNewThingUnlocked)
			{
				ItemBadge.SetNewUnlockCounter(1, false);
				global::Kampai.Util.TweenUtil.Throb(ItemBadge.transform, 0.85f, 0.5f, out starOriginalScale);
			}
			else
			{
				ItemBadge.HideNew();
				Go.killAllTweensWithTarget(ItemBadge.transform);
				ItemBadge.transform.localScale = starOriginalScale;
			}
		}

		internal void UpdatePartyPointText(global::Kampai.Main.ILocalizationService localizationService)
		{
			global::Kampai.Game.DecorationBuildingDefinition decorationBuildingDefinition = definition as global::Kampai.Game.DecorationBuildingDefinition;
			global::Kampai.Game.LeisureBuildingDefintiion leisureBuildingDefintiion = definition as global::Kampai.Game.LeisureBuildingDefintiion;
			if (decorationBuildingDefinition != null)
			{
				ItemPartyPoints.text = localizationService.GetString("DecorationProduction*", decorationBuildingDefinition.XPReward);
			}
			else if (leisureBuildingDefintiion != null)
			{
				ItemPartyPoints.text = string.Format(localizationService.GetString("LeisureProductionBuildMenu*", leisureBuildingDefintiion.PartyPointsReward), UIUtils.FormatTime(leisureBuildingDefintiion.LeisureTimeDuration, localizationService));
			}
			if (decorationBuildingDefinition != null || leisureBuildingDefintiion != null)
			{
				global::UnityEngine.RectTransform rectTransform = ItemDescription.gameObject.transform as global::UnityEngine.RectTransform;
				rectTransform.offsetMin = new global::UnityEngine.Vector2(rectTransform.offsetMin.x, 22.4f);
				ItemPartyPoints.gameObject.SetActive(ItemDescription.gameObject.activeSelf);
				isFunRewarding = true;
			}
			else
			{
				global::UnityEngine.RectTransform rectTransform2 = ItemDescription.gameObject.transform as global::UnityEngine.RectTransform;
				rectTransform2.offsetMin = new global::UnityEngine.Vector2(rectTransform2.offsetMin.x, -5f);
				ItemPartyPoints.gameObject.SetActive(false);
			}
		}

		internal void ChangeBuildingCount(bool isAdding)
		{
			if (CurrentCapacity >= 0)
			{
				Capacity.text = string.Format("{0}/{1}", (!isAdding) ? (--currentBuildingCount) : (++currentBuildingCount), CurrentCapacity);
				UpdateIconColors();
			}
		}

		internal void SetBuildingCount(int buildingCount)
		{
			if (CurrentCapacity >= 0)
			{
				currentBuildingCount = buildingCount;
				Capacity.text = string.Format("{0}/{1}", currentBuildingCount, CurrentCapacity);
				UpdateIconColors();
			}
		}

		internal void AdjustIncrementalCost()
		{
			global::Kampai.Game.BuildingDefinition buildingDefinition = definition as global::Kampai.Game.BuildingDefinition;
			if (buildingDefinition != null && buildingDefinition.IncrementalCost > 0)
			{
				global::System.Collections.Generic.ICollection<global::Kampai.Game.Building> byDefinitionId = playerService.GetByDefinitionId<global::Kampai.Game.Building>(buildingDefinition.ID);
				global::Kampai.Game.StaticItem staticItem = (global::Kampai.Game.Transaction.TransactionUtil.IsOnlyPremiumInputs(transactionDef) ? global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID : global::Kampai.Game.StaticItem.GRIND_CURRENCY_ID);
				int number = global::Kampai.Game.Transaction.TransactionUtil.SumOutputsForStaticItem(transactionDef, staticItem, true) + byDefinitionId.Count * buildingDefinition.IncrementalCost;
				Cost.text = UIUtils.FormatLargeNumber(number);
			}
		}

		internal void SetCapacity(int capacity)
		{
			CurrentCapacity = capacity;
			if (capacity < 0)
			{
				Capacity.gameObject.SetActive(false);
			}
			else
			{
				Capacity.text = string.Format("{0}/{1}", currentBuildingCount, CurrentCapacity);
			}
			UpdateIconColors();
		}

		private void UpdateIconColors()
		{
			if (currentBuildingCount >= CurrentCapacity && CurrentCapacity >= 0)
			{
				if (!isInCapacityReachedState)
				{
					ChangeStateToCapacityReached();
				}
				Capacity.color = global::Kampai.Util.GameConstants.UI.UI_BLACK;
				ItemIcon.Desaturate = 1f;
			}
			else
			{
				if (isInCapacityReachedState)
				{
					ChangeStateOutOfCapacityReached();
				}
				Capacity.color = global::Kampai.Util.GameConstants.UI.UI_TEXT_LIGHT_BLUE;
				ItemIcon.Desaturate = 0f;
			}
		}

		public void DisplayOrHideUnlockedCostIcons()
		{
			if (currentBuildingCount >= CurrentCapacity && CurrentCapacity >= 0)
			{
				DisplayCost(false);
				return;
			}
			int inventoryCountByDefinitionID = playerService.GetInventoryCountByDefinitionID(definition.ID);
			if (inventoryCountByDefinitionID > 0)
			{
				DisplayCost(false);
			}
			else
			{
				DisplayCost(true);
			}
		}

		internal void SetBadge(int badgeCount)
		{
			currentBadgeCount = badgeCount;
			ItemBadge.SetInventoryCount(badgeCount);
		}

		internal void SetHighlight(bool isHighlighted, global::Kampai.UI.View.HighlightType type = global::Kampai.UI.View.HighlightType.DRAG)
		{
			if (isHighlighted)
			{
				if (type == global::Kampai.UI.View.HighlightType.DRAG)
				{
					EnableDragTutorial(isHighlighted);
				}
				else
				{
					global::Kampai.Util.TweenUtil.Throb(Highlighted.transform, 0.85f, 0.5f, out originalScale);
				}
			}
			else
			{
				Go.killAllTweensWithTarget(Highlighted.transform);
				Highlighted.transform.localScale = originalScale;
				EnableDragTutorial(false);
			}
		}

		internal bool ChangeStateToUnlocked()
		{
			bool result = false;
			if (Locked.activeSelf)
			{
				SetNewUnlockState(true);
				Locked.SetActive(false);
				ItemIcon.Desaturate = 0f;
				result = true;
			}
			Unlocked.SetActive(true);
			if (!isInCapacityReachedState)
			{
				ChangeButtonBackingColor(defaultTopLeftBackingColor);
			}
			myButton.interactable = true;
			SetButtonTeased(false);
			return result;
		}

		internal bool IsUnlocked()
		{
			return Unlocked.activeSelf;
		}

		internal void ChangeStateToCapacityReached()
		{
			ChangeButtonBackingColor(CapacityReachedBackingColor);
			SetBuildingDraggable(false);
			DisplayCost(false);
			isInCapacityReachedState = true;
		}

		internal void ChangeStateOutOfCapacityReached()
		{
			ChangeButtonBackingColor(defaultTopLeftBackingColor);
			SetBuildingDraggable(true);
			isInCapacityReachedState = false;
		}

		internal void SetBuildingDraggable(bool set)
		{
			Arrow.enabled = set;
			myButton.interactable = set;
		}

		internal void DisplayCost(bool display)
		{
			Cost.enabled = display;
			CostBacking.enabled = display;
			MoneyIcon.enabled = display;
		}

		internal void ChangeStateToLocked()
		{
			Unlocked.SetActive(false);
			Locked.SetActive(true);
			ItemIcon.Desaturate = 1f;
			ChangeButtonBackingColor(LockedTopBackingColor);
			myButton.interactable = false;
			SetButtonTeased(false);
		}

		private void ChangeButtonBackingColor(global::UnityEngine.Color topLeftBackingColor)
		{
			for (int i = 0; i < BackingImageRect.childCount; i++)
			{
				global::UnityEngine.Transform child = BackingImageRect.GetChild(i);
				if (child != null)
				{
					child.GetComponent<global::UnityEngine.UI.Image>().color = topLeftBackingColor;
				}
			}
			BackingImageRect.GetComponent<global::UnityEngine.UI.Image>().color = topLeftBackingColor;
		}

		internal void SetButtonTeased(bool isTeased)
		{
			if (ItemIcon.gameObject.activeSelf != isTeased)
			{
				return;
			}
			ItemIcon.gameObject.SetActive(!isTeased);
			ItemName.gameObject.SetActive(!isTeased);
			ItemDescription.gameObject.SetActive(!isTeased);
			ItemPartyPoints.gameObject.SetActive(!isTeased && isFunRewarding);
			for (int i = 0; i < BackingImageRect.childCount; i++)
			{
				global::UnityEngine.Transform child = BackingImageRect.GetChild(i);
				if (child != null)
				{
					child.gameObject.SetActive(!isTeased);
				}
			}
		}

		public void SetShouldBerendered(bool value)
		{
			shouldBeRendered = value;
			base.gameObject.SetActive(value);
		}

		public bool ShouldBeRendered()
		{
			return shouldBeRendered;
		}

		private void EnableDragTutorial(bool enabled)
		{
			if (enabled)
			{
				if (dragPromptItem == null)
				{
					dragPromptItem = new global::UnityEngine.GameObject("drag");
					dragPromptItem.transform.SetParent(base.gameObject.transform, false);
					dragPromptItem.layer = 5;
					global::Kampai.UI.View.KampaiImage kampaiImage = dragPromptItem.AddComponent<global::Kampai.UI.View.KampaiImage>();
					kampaiImage.sprite = UIUtils.LoadSpriteFromPath(DragSpritePath);
					kampaiImage.maskSprite = UIUtils.LoadSpriteFromPath(DragMaskPath);
					kampaiImage.material = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Material>("CircleIconAlphaMaskMat");
					global::UnityEngine.Animator animator = dragPromptItem.AddComponent<global::UnityEngine.Animator>();
					animator.runtimeAnimatorController = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(DragAnimationController);
					animator.cullingMode = global::UnityEngine.AnimatorCullingMode.AlwaysAnimate;
					animator.applyRootMotion = false;
					global::UnityEngine.RectTransform component = dragPromptItem.GetComponent<global::UnityEngine.RectTransform>();
					global::UnityEngine.RectTransform component2 = ItemIcon.gameObject.GetComponent<global::UnityEngine.RectTransform>();
					global::Kampai.Util.RectUtil.Copy(component2, component);
				}
				dragPromptItem.SetActive(true);
			}
			else if (dragPromptItem != null)
			{
				dragPromptItem.SetActive(false);
				global::UnityEngine.Object.Destroy(dragPromptItem);
				dragPromptItem = null;
			}
		}
	}
}
