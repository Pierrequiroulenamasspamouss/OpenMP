namespace Kampai.UI.View
{
	public class CraftingQueueView : global::Kampai.Util.KampaiView, global::UnityEngine.EventSystems.IPointerExitHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerEnterHandler
	{
		public global::UnityEngine.RectTransform inProgressPanel;

		public global::UnityEngine.RectTransform availablePanel;

		public global::UnityEngine.RectTransform lockedPanel;

		public global::UnityEngine.RectTransform clockPanel;

		public global::UnityEngine.GameObject ClockIcon;

		public global::UnityEngine.GameObject PartyIcon;

		public global::UnityEngine.Transform ClockGroupForPulsing;

		public global::Kampai.UI.View.KampaiImage inProgressImage;

		public global::UnityEngine.UI.Text inProgressTime;

		public global::UnityEngine.UI.Text inProgressCost;

		public ScrollableButtonView inProgressRush;

		public ScrollableButtonView inProgressHarvest;

		public global::Kampai.UI.View.KampaiImage availableImage;

		public global::UnityEngine.UI.Text availableText;

		public global::UnityEngine.UI.Text lockedCost;

		public ScrollableButtonView lockedPurchase;

		internal bool isLocked;

		internal global::Kampai.Game.IngredientsItemDefinition itemDef;

		internal bool inProduction;

		internal bool harvestReady;

		private bool isPartying;

		internal bool isCorrectBuffType;

		private global::Kampai.Game.ITimeEventService timeEventService;

		private global::Kampai.Main.ILocalizationService localizationService;

		private global::Kampai.Game.IPlayerService playerService;

		protected global::Kampai.Util.IKampaiLogger logger;

		private global::Kampai.Game.BuildingChangeStateSignal changeStateSignal;

		internal global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData> onPointerEnterSignal = new global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData>();

		internal global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData> onPointerExitSignal = new global::strange.extensions.signal.impl.Signal<global::UnityEngine.EventSystems.PointerEventData>();

		public global::Kampai.Game.CraftingBuilding building { get; set; }

		public int index { get; set; }

		public int purchaseCost { get; set; }

		public int rushCost { get; set; }

		public void Init(global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.IPlayerService playerService, global::Kampai.Game.ITimeEventService timeEventService, global::Kampai.Main.ILocalizationService localizationService, global::Kampai.Game.BuildingChangeStateSignal changeStateSignal)
		{
			this.timeEventService = timeEventService;
			this.localizationService = localizationService;
			this.playerService = playerService;
			this.changeStateSignal = changeStateSignal;
			logger = global::Elevation.Logging.LogManager.GetClassLogger("CraftingQueueView") as global::Kampai.Util.IKampaiLogger;
			lockedPurchase.EnableDoubleConfirm();
			SetPartyState(false);
			if (index >= building.RecipeInQueue.Count)
			{
				return;
			}
			itemDef = definitionService.Get<global::Kampai.Game.IngredientsItemDefinition>(building.RecipeInQueue[index]);
			if (index == 0)
			{
				inProduction = true;
				inProgressPanel.gameObject.SetActive(true);
				inProgressRush.ResetTapState();
				inProgressRush.EnableDoubleConfirm();
				availablePanel.gameObject.SetActive(false);
				lockedPanel.gameObject.SetActive(false);
				inProgressImage.sprite = UIUtils.LoadSpriteFromPath(itemDef.Image);
				inProgressImage.maskSprite = UIUtils.LoadSpriteFromPath(itemDef.Mask);
				Update();
			}
			else
			{
				inProgressPanel.gameObject.SetActive(false);
				availablePanel.gameObject.SetActive(true);
				lockedPanel.gameObject.SetActive(false);
				if (itemDef != null)
				{
					availableText.gameObject.SetActive(false);
					availableImage.gameObject.SetActive(true);
					availableImage.sprite = UIUtils.LoadSpriteFromPath(itemDef.Image);
					availableImage.maskSprite = UIUtils.LoadSpriteFromPath(itemDef.Mask);
				}
			}
		}

		public void Update()
		{
			if (index == 0 && inProduction)
			{
				int timeRemaining = timeEventService.GetTimeRemaining(building.ID);
				bool flag = playerService.GetMinionPartyInstance().IsBuffHappening && isCorrectBuffType;
				if (isPartying != flag)
				{
					SetPartyState(flag);
				}
				if (timeRemaining <= 0)
				{
					// DIAGNOSTIC LOGGING
					if (rushCost > 0 || (inProgressTime != null && inProgressTime.text != "00:00:00"))
					{
						if (logger != null)
						{
							logger.Warning("[TIMER_BUG] Time <= 0 but UI not swapped. ID: {0}, TimeRemaining: {1}, LastRushCost: {2}, Building: {3}", building.ID, timeRemaining, rushCost, building.Definition.LocalizedKey);
						}
					}

					rushCost = 0;
					inProgressTime.text = "00:00:00";
					inProgressCost.text = "0";

					inProduction = false;
					harvestReady = true;
					SwapToHarvest();
				}
				else
				{
					inProgressTime.text = UIUtils.FormatTime(timeRemaining, localizationService);
					rushCost = timeEventService.CalculateRushCostForTimer(timeRemaining, global::Kampai.Game.RushActionType.CRAFTING);
					inProgressCost.text = rushCost.ToString();
				}
			}
		}

		private void SwapToHarvest()
		{
			changeStateSignal.Dispatch(building.ID, (building.RecipeInQueue.Count <= 1) ? global::Kampai.Game.BuildingState.Harvestable : global::Kampai.Game.BuildingState.HarvestableAndWorking);
			inProgressRush.gameObject.SetActive(false);
			clockPanel.gameObject.SetActive(false);
			inProgressHarvest.gameObject.SetActive(true);
		}

		public void OnPointerEnter(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			onPointerEnterSignal.Dispatch(eventData);
		}

		public void OnPointerExit(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			onPointerExitSignal.Dispatch(eventData);
		}

		internal void SetPartyState(bool isPartying)
		{
			this.isPartying = isPartying;
			ClockIcon.SetActive(!isPartying);
			PartyIcon.SetActive(isPartying);
			if (isPartying)
			{
				global::UnityEngine.Vector3 originalScale;
				global::Kampai.Util.TweenUtil.Throb(ClockGroupForPulsing, 1.1f, 0.2f, out originalScale);
				UIUtils.FlashingColor(inProgressTime, 0);
				return;
			}
			Go.killAllTweensWithTarget(ClockGroupForPulsing);
			Go.killAllTweensWithTarget(inProgressTime);
			inProgressTime.color = global::UnityEngine.Color.white;
			ClockGroupForPulsing.localScale = global::UnityEngine.Vector3.one;
		}
	}
}
