namespace Kampai.UI.View
{
	public class BuyMarketplacePanelView : global::Kampai.UI.View.PopupMenuView
	{
		internal enum RefreshButtonState
		{
			None = 0,
			RefreshReady = 1,
			RefreshPending = 2,
			StopSpinning = 3
		}

		private global::System.Collections.IEnumerator premiumButtonDisableCoroutine;

		private global::System.Collections.IEnumerator adPanelDisableCoroutine;

		public global::Kampai.UI.View.ButtonView ArrowButtonView;

		public global::Kampai.UI.View.DoubleConfirmButtonView RefreshPremiumButtonView;

		public global::Kampai.UI.View.DoubleConfirmButtonView RefreshPremiumButtonViewAdPanel;

		public global::Kampai.UI.View.ButtonView RefreshButtonView;

		public global::Kampai.UI.View.ButtonView StopButtonView;

		public global::Kampai.UI.View.ButtonView AdVideoButtonView;

		public global::UnityEngine.GameObject PanelRefreshPendingWithAd;

		public global::UnityEngine.UI.Text RefreshCostText;

		public global::UnityEngine.UI.Text RefreshCostTextAdPanel;

		public global::UnityEngine.UI.Text RefreshTitleText;

		public global::UnityEngine.UI.Text RefreshTitleTextAdPanel;

		public global::Kampai.UI.View.KampaiScrollView ScrollView;

		private global::Kampai.Main.ILocalizationService localService;

		internal bool AdButtonEnabled;

		internal global::strange.extensions.signal.impl.Signal<bool> OnOpenSignal = new global::strange.extensions.signal.impl.Signal<bool>();

		internal global::strange.extensions.signal.impl.Signal OnCloseSignal = new global::strange.extensions.signal.impl.Signal();

		internal global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState refreshButtonState;

		public bool IsOpen { get; set; }

		protected override void Awake()
		{
			global::Kampai.Util.KampaiView.BubbleToContextOnAwake(this, ref currentContext, true);
		}

		public void Init(global::Kampai.Main.ILocalizationService localizationService)
		{
			base.Init();
			refreshButtonState = global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.None;
			localService = localizationService;
			StopButtonView.PlaySoundOnClick = false;
			RefreshPremiumButtonView.PlaySoundOnClick = false;
			RefreshPremiumButtonViewAdPanel.PlaySoundOnClick = false;
			RefreshPremiumButtonView.EnableDoubleConfirm();
			RefreshPremiumButtonViewAdPanel.EnableDoubleConfirm();
			SetRefreshCost(0);
		}

		public bool HasSlot(global::Kampai.Game.MarketplaceBuyItem slot)
		{
			foreach (global::UnityEngine.MonoBehaviour item in ScrollView)
			{
				global::Kampai.UI.View.BuyMarketplaceSlotView buyMarketplaceSlotView = item as global::Kampai.UI.View.BuyMarketplaceSlotView;
				if (buyMarketplaceSlotView == null || slot.ID != buyMarketplaceSlotView.slotId)
				{
					continue;
				}
				return true;
			}
			return false;
		}

		public void EnableRewardedAdRushButton(bool enable)
		{
			if (AdButtonEnabled != enable)
			{
				AdButtonEnabled = enable;
				if (refreshButtonState == global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending)
				{
					EnableRefreshPendingWithAdPanel(enable);
				}
			}
		}

		private void EnableRefreshPendingWithAdPanel(bool enable)
		{
			if (enable)
			{
				PanelRefreshPendingWithAd.SetActive(enable);
				if (adPanelDisableCoroutine != null)
				{
					StopCoroutine(adPanelDisableCoroutine);
					adPanelDisableCoroutine = null;
				}
			}
			else if (adPanelDisableCoroutine == null)
			{
				adPanelDisableCoroutine = DisableAdPanelOnEndOfFrame();
				StartCoroutine(adPanelDisableCoroutine);
			}
		}

		internal void SetupRefreshButtonState(global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState state, int timeRemaining = 0)
		{
			if (state != refreshButtonState && state != global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending)
			{
				EnableRefreshPendingWithAdPanel(false);
			}
			switch (state)
			{
			case global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshReady:
				SetRefreshTitleText(localService.GetString("BuyPanelRefreshUserPrompt"));
				if (state != refreshButtonState)
				{
					RefreshButtonView.gameObject.SetActive(true);
					if (premiumButtonDisableCoroutine == null)
					{
						premiumButtonDisableCoroutine = DisablePremiumButtonOnEndOfFrame();
						StartCoroutine(premiumButtonDisableCoroutine);
					}
					StopButtonView.gameObject.SetActive(false);
				}
				break;
			case global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.RefreshPending:
			{
				global::System.TimeSpan timeSpan = global::System.TimeSpan.FromSeconds(timeRemaining);
				SetRefreshTitleText(string.Format("{0} {1}", localService.GetString("BuyPanelRefreshTitle"), UIUtils.FormatTime(timeSpan.TotalSeconds, localService)));
				if (state != refreshButtonState)
				{
					if (premiumButtonDisableCoroutine != null)
					{
						StopCoroutine(premiumButtonDisableCoroutine);
						premiumButtonDisableCoroutine = null;
					}
					SetActivePremium(true);
					RefreshButtonView.gameObject.SetActive(false);
					StopButtonView.gameObject.SetActive(false);
					EnableRefreshPendingWithAdPanel(AdButtonEnabled);
				}
				break;
			}
			case global::Kampai.UI.View.BuyMarketplacePanelView.RefreshButtonState.StopSpinning:
				SetRefreshTitleText(localService.GetString("BuyPanelStopSpinningUserPrompt"));
				if (state != refreshButtonState)
				{
					RefreshPremiumButtonView.ResetAnim();
					RefreshPremiumButtonViewAdPanel.ResetAnim();
					StopButtonView.gameObject.SetActive(true);
					RefreshButtonView.gameObject.SetActive(false);
					if (premiumButtonDisableCoroutine == null)
					{
						premiumButtonDisableCoroutine = DisablePremiumButtonOnEndOfFrame();
						StartCoroutine(premiumButtonDisableCoroutine);
					}
				}
				break;
			}
			refreshButtonState = state;
		}

		internal void SetActivePremium(bool active)
		{
			RefreshPremiumButtonView.gameObject.SetActive(active);
			RefreshPremiumButtonViewAdPanel.gameObject.SetActive(active);
		}

		internal void SetRefreshTitleText(string text)
		{
			if (RefreshTitleText != null)
			{
				RefreshTitleText.text = text;
				RefreshTitleTextAdPanel.text = text;
			}
		}

		internal void SetRefreshCost(int cost)
		{
			string text = cost.ToString();
			RefreshCostText.text = text;
			RefreshCostTextAdPanel.text = text;
		}

		internal global::System.Collections.IEnumerator DisablePremiumButtonOnEndOfFrame()
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			SetActivePremium(false);
			premiumButtonDisableCoroutine = null;
		}

		internal global::System.Collections.IEnumerator DisableAdPanelOnEndOfFrame()
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			PanelRefreshPendingWithAd.SetActive(false);
			adPanelDisableCoroutine = null;
		}

		internal void SetOpen(bool show, bool dispatchSignals = true, bool isInstant = false)
		{
			if (show)
			{
				if (isInstant)
				{
					float lastFrame = 1f;
					int defaultLayer = -1;
					OpenInstantly(defaultLayer, lastFrame);
				}
				else
				{
					Open();
				}
				if (dispatchSignals)
				{
					OnOpenSignal.Dispatch(isInstant);
				}
			}
			else
			{
				Close();
				if (dispatchSignals)
				{
					OnCloseSignal.Dispatch();
				}
			}
			IsOpen = show;
		}
	}
}
