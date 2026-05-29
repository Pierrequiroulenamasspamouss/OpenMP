namespace Kampai.UI.View
{
	public class BuyMarketplacePanelMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.BuyMarketplacePanelView>
	{
		private int refreshTimeSeconds;

		private global::Kampai.Game.MarketplaceRefreshTimer refreshTimer;

		private readonly global::Kampai.Game.AdPlacementName adPlacementName = global::Kampai.Game.AdPlacementName.MARKETPLACE;

		private global::Kampai.Game.AdPlacementInstance adPlacementInstance;

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject(global::Kampai.Game.SocialServices.FACEBOOK)]
		public global::Kampai.Game.ISocialService facebookService { get; set; }

		[Inject]
		public global::Kampai.Game.RefreshSaleItemsSuccessSignal refreshSuccessSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RushRefreshTimerSuccessSignal rushSuccessSignal { get; set; }

		[Inject]
		public global::Kampai.Game.HaltSlotMachine haltSlotMachine { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceOpenBuyPanelSignal openBuyPanel { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceCloseBuyPanelSignal closeBuyPanel { get; set; }

		[Inject]
		public global::Kampai.Common.ICoppaService coppaService { get; set; }

		[Inject]
		public global::Kampai.Game.SocialLoginSuccessSignal loginSuccess { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveStorageBuildingItemDescriptionSignal removeItemDescriptionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IMarketplaceService marketPlaceService { get; set; }

		[Inject]
		public global::Kampai.Game.IRewardedAdService rewardedAdService { get; set; }

		[Inject]
		public global::Kampai.Game.RewardedAdRewardSignal rewardedAdRewardSignal { get; set; }

		[Inject]
		public global::Kampai.Game.AdPlacementActivityStateChangedSignal adPlacementActivityStateChangedSignal { get; set; }

		public override void OnRegister()
		{
			base.view.Init(localService);
			global::Kampai.Game.MarketplaceRefreshTimerDefinition marketplaceRefreshTimerDefinition = definitionService.Get<global::Kampai.Game.MarketplaceRefreshTimerDefinition>(1000008093);
			refreshTimeSeconds = marketplaceRefreshTimerDefinition.RefreshTimeSeconds;
			openBuyPanel.AddListener(OpenPanel);
			base.view.OnOpenSignal.AddListener(OpenPanel);
			base.view.OnCloseSignal.AddListener(Close);
			base.view.ArrowButtonView.ClickedSignal.AddListener(Close);
			base.view.RefreshButtonView.ClickedSignal.AddListener(OnRefreshButtonClick);
			base.view.RefreshPremiumButtonView.ClickedSignal.AddListener(OnRefreshButtonClick);
			base.view.RefreshPremiumButtonViewAdPanel.ClickedSignal.AddListener(OnRefreshButtonClick);
			base.view.AdVideoButtonView.ClickedSignal.AddListener(OnAdButtonButtonClick);
			base.view.StopButtonView.ClickedSignal.AddListener(OnRefreshButtonClick);
			refreshSuccessSignal.AddListener(LoadScrollViewItems);
			refreshSuccessSignal.AddListener(UpdateRefreshTime);
			rushSuccessSignal.AddListener(UpdateRefreshTime);
			haltSlotMachine.AddListener(CompleteSlotMachine);
			loginSuccess.AddListener(OnLoginSuccess);
			rewardedAdRewardSignal.AddListener(OnRewardedAdReward);
			adPlacementActivityStateChangedSignal.AddListener(OnAdPlacementActivityStateChanged);
			base.view.SetRefreshCost(marketplaceRefreshTimerDefinition.RushCost);
			UpdateAdButton();
		}

		public override void OnRemove()
		{
			openBuyPanel.RemoveListener(OpenPanel);
			base.view.OnOpenSignal.RemoveListener(OpenPanel);
			base.view.OnCloseSignal.RemoveListener(Close);
			base.view.ArrowButtonView.ClickedSignal.RemoveListener(Close);
			base.view.RefreshButtonView.ClickedSignal.RemoveListener(OnRefreshButtonClick);
			base.view.RefreshPremiumButtonView.ClickedSignal.RemoveListener(OnRefreshButtonClick);
			base.view.RefreshPremiumButtonViewAdPanel.ClickedSignal.RemoveListener(OnRefreshButtonClick);
			base.view.AdVideoButtonView.ClickedSignal.RemoveListener(OnAdButtonButtonClick);
			base.view.StopButtonView.ClickedSignal.RemoveListener(OnRefreshButtonClick);
			refreshSuccessSignal.RemoveListener(LoadScrollViewItems);
			refreshSuccessSignal.RemoveListener(UpdateRefreshTime);
			rushSuccessSignal.RemoveListener(UpdateRefreshTime);
			haltSlotMachine.RemoveListener(CompleteSlotMachine);
			loginSuccess.RemoveListener(OnLoginSuccess);
			rewardedAdRewardSignal.RemoveListener(OnRewardedAdReward);
			adPlacementActivityStateChangedSignal.RemoveListener(OnAdPlacementActivityStateChanged);
			StopAllCoroutines();
		}

		private void OpenPanel(bool isInstant)
		{
			base.view.SetOpen(true, false, isInstant);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.StartMarketplaceRefreshTimerSignal>().Dispatch(false);
			UpdateRefreshTime();
			telemetryService.Send_Telemtry_EVT_MARKETPLACE_VIEWED("VIEW");
			InvokeRepeating("UpdateRefreshTime", 0.001f, 1f);
			LoadScrollViewItems();
			base.uiAddedSignal.Dispatch(base.view.gameObject, Close);
			removeItemDescriptionSignal.Dispatch();
			soundFXSignal.Dispatch("Play_shop_pane_in_01");
		}

		protected override void Close()
		{
			CancelInvoke("UpdateRefreshTime");
			if (base.view.refreshButtonState == global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.StopSpinning)
			{
				StopSlotSpinning();
			}
			else
			{
				UpdateButtonSpinningState();
			}
			base.view.SetOpen(false, false);
			closeBuyPanel.Dispatch();
			base.uiRemovedSignal.Dispatch(GetViewGameObject());
			soundFXSignal.Dispatch("Play_shop_pane_out_01");
		}

		internal void CompleteSlotMachine()
		{
			StopAllCoroutines();
			StopSlotSpinning();
			soundFXSignal.Dispatch("Play_marketplace_slotEnd_01");
			base.view.RefreshPremiumButtonView.ResetAnim();
			base.view.RefreshPremiumButtonViewAdPanel.ResetAnim();
		}

		internal void StopSlotSpinning()
		{
			foreach (global::Kampai.UI.View.BuyMarketplaceSlotView itemView in base.view.ScrollView.ItemViewList)
			{
				itemView.StopSlotMachine();
			}
			UpdateButtonSpinningState();
		}

		internal void UpdateButtonSpinningState()
		{
			base.view.ScrollView.EnableScrolling(false, true);
			if (base.view.refreshButtonState != global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.StopSpinning)
			{
				return;
			}
			base.view.SetupRefreshButtonState(global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.StartMarketplaceRefreshTimerSignal>().Dispatch(true);
			UpdateRefreshTime();
			foreach (global::Kampai.UI.View.BuyMarketplaceSlotView itemView in base.view.ScrollView.ItemViewList)
			{
				if (itemView.BuyButtonView.isActiveAndEnabled && itemView.BuyButtonView.animator != null)
				{
					itemView.BuyButtonView.animator.enabled = true;
				}
			}
		}

		private void OnAdButtonButtonClick()
		{
			if (adPlacementInstance != null)
			{
				rewardedAdService.ShowRewardedVideo(adPlacementInstance);
			}
		}

		private void UpdateAdButton()
		{
			bool flag = rewardedAdService.IsPlacementActive(adPlacementName);
			global::Kampai.Game.AdPlacementInstance placementInstance = rewardedAdService.GetPlacementInstance(adPlacementName);
			bool enable = flag && placementInstance != null;
			base.view.EnableRewardedAdRushButton(enable);
			adPlacementInstance = placementInstance;
		}

		private void OnRewardedAdReward(global::Kampai.Game.AdPlacementInstance placement)
		{
			if (placement.Equals(adPlacementInstance))
			{
				if (base.view.refreshButtonState == global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending)
				{
					RushForPremium(true);
					rewardedAdService.RewardPlayer(null, placement);
					telemetryService.Send_Telemetry_EVT_AD_INTERACTION(placement.Definition.Name.ToString(), "Rush", placement.RewardPerPeriodCount);
				}
				adPlacementInstance = null;
				UpdateAdButton();
			}
		}

		private void OnAdPlacementActivityStateChanged(global::Kampai.Game.AdPlacementInstance placement, bool enabled)
		{
			UpdateAdButton();
		}

		private void OnRefreshButtonClick()
		{
			global::strange.extensions.injector.api.ICrossContextInjectionBinder injectionBinder = gameContext.injectionBinder;
			switch (base.view.refreshButtonState)
			{
			case global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshReady:
			{
				if (refreshTimer == null)
				{
					refreshTimer = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.MarketplaceRefreshTimer>(1000008093);
				}
				if (refreshTimer != null)
				{
					refreshTimer.UTCStartTime = timeService.CurrentTime() + 3;
				}
				RefreshSaleItemsSignalArgs refreshSaleItemsSignalArgs = new RefreshSaleItemsSignalArgs();
				refreshSaleItemsSignalArgs.RefreshItems = true;
				refreshSaleItemsSignalArgs.StopSpinning = false;
				RefreshSaleItemsSignalArgs type = refreshSaleItemsSignalArgs;
				injectionBinder.GetInstance<global::Kampai.Game.RefreshSaleItemsSignal>().Dispatch(type);
				base.view.SetupRefreshButtonState(global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.StopSpinning);
				break;
			}
			case global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending:
				if (base.view.RefreshPremiumButtonView.isDoubleConfirmed() || base.view.RefreshPremiumButtonViewAdPanel.isDoubleConfirmed())
				{
					soundFXSignal.Dispatch("Play_button_premium_01");
					RushForPremium();
					base.view.RefreshPremiumButtonView.ResetTapState();
					base.view.RefreshPremiumButtonViewAdPanel.ResetTapState();
				}
				break;
			case global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.StopSpinning:
			{
				CompleteSlotMachine();
				RefreshSaleItemsSignalArgs refreshSaleItemsSignalArgs = new RefreshSaleItemsSignalArgs();
				refreshSaleItemsSignalArgs.RefreshItems = false;
				refreshSaleItemsSignalArgs.StopSpinning = true;
				RefreshSaleItemsSignalArgs type = refreshSaleItemsSignalArgs;
				injectionBinder.GetInstance<global::Kampai.Game.RefreshSaleItemsSignal>().Dispatch(type);
				break;
			}
			}
		}

		private void RushForPremium(bool rewardedForAdImpression = false)
		{
			RefreshSaleItemsSignalArgs refreshSaleItemsSignalArgs = new RefreshSaleItemsSignalArgs();
			refreshSaleItemsSignalArgs.RefreshItems = false;
			refreshSaleItemsSignalArgs.StopSpinning = false;
			RefreshSaleItemsSignalArgs refreshSaleItemsSignalArgs2 = refreshSaleItemsSignalArgs;
			if (rewardedForAdImpression)
			{
				refreshSaleItemsSignalArgs2.RushCost = 0;
			}
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.RefreshSaleItemsSignal>().Dispatch(refreshSaleItemsSignalArgs2);
		}

		private void UpdateRefreshTime()
		{
			if (base.view.refreshButtonState != global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.StopSpinning)
			{
				int num = 0;
				if (refreshTimer == null)
				{
					refreshTimer = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.MarketplaceRefreshTimer>(1000008093);
				}
				if (refreshTimer != null)
				{
					num = refreshTimer.UTCStartTime + refreshTimeSeconds - timeService.CurrentTime();
				}
				if (num <= 0)
				{
					num = 0;
					base.view.SetupRefreshButtonState(global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshReady);
				}
				else
				{
					base.view.SetupRefreshButtonState(global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending, num);
				}
			}
		}

		private void LoadScrollViewItems()
		{
			bool isCOPPAGated = coppaService.Restricted();
			global::System.Collections.Generic.List<global::Kampai.Game.MarketplaceBuyItem> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.MarketplaceBuyItem>();
			if (base.view.ScrollView.ItemViewList.Count == instancesByType.Count)
			{
				bool flag = false;
				foreach (global::Kampai.UI.View.BuyMarketplaceSlotView itemView in base.view.ScrollView.ItemViewList)
				{
					if (itemView.SetupBuyItem(localService, definitionService, facebookService, isCOPPAGated, instancesByType[itemView.slotIndex], soundFXSignal, marketPlaceService))
					{
						flag = true;
					}
				}
				if (flag)
				{
					StopAllCoroutines();
					base.view.ScrollView.TweenToPosition(new global::UnityEngine.Vector2(0f, 1f), 0f);
					base.view.ScrollView.EnableScrolling(false, false);
					StartCoroutine("EnableScrollView");
				}
			}
			else
			{
				base.view.ScrollView.ClearItems();
				if (instancesByType != null)
				{
					base.view.ScrollView.AddList(instancesByType, CreateMarketplaceItem);
				}
			}
		}

		internal global::System.Collections.IEnumerator EnableScrollView()
		{
			yield return new global::UnityEngine.WaitForSeconds(3.7f);
			UpdateButtonSpinningState();
		}

		private global::Kampai.UI.View.BuyMarketplaceSlotView CreateMarketplaceItem(int index, global::Kampai.Game.MarketplaceBuyItem item)
		{
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("cmp_StorageBuildingBuySlot") as global::UnityEngine.GameObject;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
			global::Kampai.UI.View.BuyMarketplaceSlotView component = gameObject.GetComponent<global::Kampai.UI.View.BuyMarketplaceSlotView>();
			component.slotIndex = index;
			component.slotId = item.ID;
			component.BuyItem = item;
			return component;
		}

		private void OnLoginSuccess(global::Kampai.Game.ISocialService socialService)
		{
			if (socialService.type == global::Kampai.Game.SocialServices.FACEBOOK)
			{
				LoadScrollViewItems();
			}
		}
	}
}
