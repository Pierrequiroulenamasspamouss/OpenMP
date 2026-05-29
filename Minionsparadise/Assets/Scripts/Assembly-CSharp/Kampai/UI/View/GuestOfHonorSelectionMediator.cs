namespace Kampai.UI.View
{
	public class GuestOfHonorSelectionMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.GuestOfHonorSelectionView>
	{
		private float scrollPosition;

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideSkrimSignal hideSkrim { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IPrestigeService prestigeService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IGuestOfHonorService guestOfHonorService { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetPremiumCurrencySignal setPremiumCurrencySignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal globalSFXSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.GOHCardClickedSignal gohCardClickedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowMinionPartySkipButtonSignal showSkipButtonSignal { get; set; }

		[Inject]
		public global::Kampai.Main.MoveAudioListenerSignal toggleCharacterAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CancelPurchaseSignal cancelPurchaseSignal { get; set; }

		public override void OnRegister()
		{
			Kampai.Input.InputCompat.MultiTouchEnabled = false;
			base.view.OnMenuClose.AddListener(OnMenuClose);
			base.view.rushGOHCooldown_Callback.AddListener(RushGoHCooldown);
			gohCardClickedSignal.AddListener(CharacterClicked);
			cancelPurchaseSignal.AddListener(CurrencyClosed);
		}

		public override void OnRemove()
		{
			Kampai.Input.InputCompat.MultiTouchEnabled = true;
			toggleCharacterAudioSignal.Dispatch(true, null);
			base.view.OnMenuClose.RemoveListener(OnMenuClose);
			base.view.rushGOHCooldown_Callback.RemoveListener(RushGoHCooldown);
			gohCardClickedSignal.RemoveListener(CharacterClicked);
			cancelPurchaseSignal.RemoveListener(CurrencyClosed);
		}

		private void CharacterClicked(int cardIndex, bool avail)
		{
			soundFXSignal.Dispatch("Play_button_click_01");
			if (base.view.currentCharacterIndex != cardIndex)
			{
				base.view.currentCharacterIndex = cardIndex;
				base.view.SetStartButtonUnlocked(avail);
			}
		}

		public override void Initialize(global::Kampai.UI.View.GUIArguments args)
		{
			Init();
			soundFXSignal.Dispatch("Play_main_menu_open_01");
		}

		private void Init()
		{
			base.closeAllOtherMenuSignal.Dispatch(base.gameObject);
			base.view.Init(prestigeService, definitionService, playerService, guestOfHonorService);
			base.view.startButton.onClick.AddListener(Proceed);
		}

		private void Proceed()
		{
			soundFXSignal.Dispatch("Play_main_menu_close_01");
			Kampai.Input.InputCompat.MultiTouchEnabled = true;
			base.view.Close();
			StartMinionParty();
		}

		private void StartMinionParty()
		{
			guestOfHonorService.SelectGuestOfHonor(base.view.GetCharacterPrestigeDefID(base.view.currentCharacterIndex));
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.StartMinionPartyIntroSignal>().Dispatch();
		}

		private void OnMenuClose()
		{
			guiService.Execute(global::Kampai.UI.View.GUIOperation.Unload, "screen_GuestOfHonorSelection");
			hideSkrim.Dispatch("StartPartySkirm");
			showSkipButtonSignal.Dispatch(true);
		}

		protected override void OnCloseAllMenu(global::UnityEngine.GameObject exception)
		{
		}

		protected override void Close()
		{
		}

		private void RushGoHCooldown()
		{
			global::Kampai.Game.Prestige prestige = prestigeService.GetPrestige(base.view.GetCharacterPrestigeDefID(base.view.currentCharacterIndex), false);
			if (playerService.GetQuantity(global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID) < guestOfHonorService.GetRushCostForPartyCoolDown(prestige.ID))
			{
				scrollPosition = base.view.GetHorizontalScrollPosition();
				base.gameObject.SetActive(false);
				base.view.Hide();
			}
			playerService.ProcessRush(guestOfHonorService.GetRushCostForPartyCoolDown(prestige.ID), true, RushTransactionCallback, prestige.ID);
		}

		private void RushTransactionCallback(global::Kampai.Game.PendingCurrencyTransaction pct)
		{
			if (pct.Success)
			{
				globalSFXSignal.Dispatch("Play_button_premium_01");
				global::Kampai.Game.Prestige prestige = prestigeService.GetPrestige(base.view.GetCharacterPrestigeDefID(base.view.currentCharacterIndex), false);
				guestOfHonorService.RushPartyCooldownForPrestige(prestige.ID);
				base.view.RushCurrentCharacterCooldown();
				setPremiumCurrencySignal.Dispatch();
			}
			CurrencyClosed(false);
		}

		private void CurrencyClosed(bool value)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
				Init();
				base.view.SetHorizontalScrollPosition(scrollPosition);
			}
		}
	}
}
