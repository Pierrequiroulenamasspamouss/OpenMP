namespace Kampai.UI.View
{
	public class SellPanelMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.SellPanelView>
	{
		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject(global::Kampai.Game.SocialServices.FACEBOOK)]
		public global::Kampai.Game.ISocialService facebookService { get; set; }

		[Inject]
		public global::Kampai.UI.View.OpenCreateNewSalePanelSignal openSalePanelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseCreateNewSalePanelSignal closeCreateNewSaleSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RefreshSlotsSignal refreshSlotsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceOpenSalePanelSignal openSalePanel { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceCloseSalePanelSignal closeSalePanelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceCloseAllSalePanels closeAllSalePanels { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateMarketplaceSaleOrderSignal updateSaleOrderSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.EnableStorageBuildingItemDescriptionSignal enableItemDescriptionSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SocialLoginSuccessSignal loginSuccess { get; set; }

		[Inject]
		public global::Kampai.Game.IMarketplaceService marketplaceService { get; set; }

		public override void OnRegister()
		{
			base.view.Init();
			base.view.OnOpenPanelSignal.AddListener(OnOpen);
			loginSuccess.AddListener(RefreshSlotsAfterLogin);
			openSalePanel.AddListener(Open);
			closeAllSalePanels.AddListener(CloseAllViews);
			openSalePanelSignal.AddListener(OpenCreateNewSalePanel);
			closeCreateNewSaleSignal.AddListener(CloseCreateNewSalePanel);
			refreshSlotsSignal.AddListener(PurchaseSlot);
			base.view.ArrowButtonView.ClickedSignal.AddListener(Close);
		}

		public override void OnRemove()
		{
			base.view.OnOpenPanelSignal.RemoveListener(OnOpen);
			loginSuccess.RemoveListener(RefreshSlotsAfterLogin);
			openSalePanel.RemoveListener(Open);
			closeAllSalePanels.RemoveListener(CloseAllViews);
			openSalePanelSignal.RemoveListener(OpenCreateNewSalePanel);
			closeCreateNewSaleSignal.RemoveListener(CloseCreateNewSalePanel);
			refreshSlotsSignal.RemoveListener(PurchaseSlot);
			base.view.ArrowButtonView.ClickedSignal.RemoveListener(Close);
		}

		private void OnOpen()
		{
			base.uiAddedSignal.Dispatch(GetViewGameObject(), Close);
			enableItemDescriptionSignal.Dispatch(true);
		}

		private void Open(bool isInstant)
		{
			if (facebookService.isLoggedIn || marketplaceService.DebugFacebook)
			{
				updateSaleOrderSignal.Dispatch();
			}
			enableItemDescriptionSignal.Dispatch(true);
			RefreshSlots();
			base.view.ScrollView.SetupScrollView();
			base.view.SetOpen(true);
			soundFXSignal.Dispatch("Play_shop_pane_in_01");
		}

		private void RefreshSlots()
		{
			base.view.ScrollView.ClearItems();
			global::System.Collections.Generic.List<global::Kampai.Game.MarketplaceSaleSlot> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.MarketplaceSaleSlot>();
			instancesByType.Sort();
			if (instancesByType != null)
			{
				base.view.ScrollView.AddList(instancesByType, CreateSlotView, (global::Kampai.Game.MarketplaceSaleSlot slot) => !base.view.HasSlot(slot) && marketplaceService.IsSlotVisible(slot), false);
			}
		}

		private global::System.Collections.IEnumerator UpdateScrollViewAfterLogin()
		{
			yield return null;
			base.view.ScrollView.SetupScrollView();
		}

		private void RefreshSlotsAfterLogin(global::Kampai.Game.ISocialService socialService)
		{
			StartCoroutine(UpdateScrollViewAfterLogin());
		}

		private void PurchaseSlot(bool scroll)
		{
			RefreshSlots();
			if (scroll)
			{
				base.view.ScrollView.SetupScrollView(-1f, global::Kampai.UI.View.KampaiScrollView.MoveDirection.End);
			}
		}

		private global::Kampai.UI.View.StorageBuildingSaleSlotView CreateSlotView(int index, global::Kampai.Game.MarketplaceSaleSlot slot)
		{
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("cmp_StorageBuildingSaleSlot") as global::UnityEngine.GameObject;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
			global::Kampai.UI.View.StorageBuildingSaleSlotView component = gameObject.GetComponent<global::Kampai.UI.View.StorageBuildingSaleSlotView>();
			component.slotId = slot.ID;
			return component;
		}

		private void OpenCreateNewSalePanel(int slotIndex)
		{
			if (base.view.isOpen)
			{
				base.view.FadeOutItems();
				base.view.FadeAnimation(true);
			}
		}

		private void CloseCreateNewSalePanel()
		{
			if (base.view.isOpen)
			{
				base.view.FadeAnimation(false);
			}
			base.view.FadeInItems();
		}

		private void CloseAllViews()
		{
			base.view.SetOpen(false);
			base.view.FadeInItems();
			closeCreateNewSaleSignal.Dispatch();
			closeSalePanelSignal.Dispatch();
			enableItemDescriptionSignal.Dispatch(false);
			soundFXSignal.Dispatch("Play_shop_pane_out_01");
			base.uiRemovedSignal.Dispatch(base.view.gameObject);
		}

		protected override void Close()
		{
			if (base.view.isCreateNewSalePanelOpened)
			{
				soundFXSignal.Dispatch("Play_marketplace_backFromSale_01");
				base.view.FadeAnimation(false);
				closeCreateNewSaleSignal.Dispatch();
				base.uiAddedSignal.Dispatch(GetViewGameObject(), Close);
			}
			else
			{
				CloseAllViews();
			}
		}
	}
}
