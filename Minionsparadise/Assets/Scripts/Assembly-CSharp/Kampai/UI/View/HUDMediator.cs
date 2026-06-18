namespace Kampai.UI.View
{
	public class HUDMediator : global::strange.extensions.mediation.impl.EventMediator
	{
		private global::Kampai.Util.MutableBoxed<bool> currentPeekToken;

		private bool characterLoopIsOn;

		[Inject]
		public global::Kampai.UI.View.HUDView view { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseHUDSignal closeSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeAllOtherMenusSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetGrindCurrencySignal setGrindCurrencySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetPremiumCurrencySignal setPremiumCurrencySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetStorageCapacitySignal setStorageSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal playSFXSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowMTXStoreSignal showMTXStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowHUDSignal showHUDSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSettingsButtonSignal showSettingsButtonSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowPetsButtonSignal showPetsButtonSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowPetsXPromoSignal showPetsXPromoSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.PeekHUDSignal peekHUDSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.TogglePopupForHUDSignal togglePopupSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetHUDButtonsVisibleSignal setHUDButtonsVisibleSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ICurrencyService currencyService { get; set; }

		[Inject]
		public global::Kampai.UI.ICurrencyStoreService currencyStoreService { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIAddedSignal uiAddedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIRemovedSignal uiRemovedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.XPFTUEHighlightSignal ftueHighlightSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowAllWayFindersSignal showAllWayFindersSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideAllWayFindersSignal hideAllWayFindersSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowAllResourceIconsSignal showAllResourceIconsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideAllResourceIconsSignal hideAllResourceIconsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ToggleAllFloatingTextSignal toggleAllFloatingTextSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HUDChangedSiblingIndexSignal hudChangingSiblingIndexSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.FirePremiumVFXSignal firePremiumSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.FireGrindVFXSignal fireGrindSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideDelayHUDSignal hideDelayHUDSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IQuestScriptService questScriptService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CurrencyDialogClosedSignal currencyDialogClosedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.OpenStorageBuildingSignal OpenStorageBuildingSignal { get; set; }

		[Inject]
		public RushDialogPurchaseHelper rushDialogPurchaseHelper { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetStorageMenuEnabledSignal setStorageMenuEnabledSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateSaleBadgeSignal updateSaleBadgeSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.QuestDialogDismissedSignal questDialogDismissed { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowOfflinePopupSignal showOfflinePopupSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PauseSoundSignal pauseSoundSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BeginCharacterLoopAnimationSignal beginCharacterLoop { get; set; }

		[Inject]
		public global::Kampai.Game.CharacterIntroCompleteSignal endCharacterLoop { get; set; }

		[Inject]
		public global::Kampai.UI.View.EnableVillainLairHudSignal enableVillainHudSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ExitVillainLairSignal exitVillainLairSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisplayCurrencyStoreSignal displayCurrencyStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetBuildMenuEnabledSignal setBuildMenuEnabledSignal { get; set; }

		[Inject]
		public global::Kampai.Game.VillainLairModel villainLairModel { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisplayDisco3DElements displayDisco3DElements { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIModel uiModel { get; set; }

		[Inject]
		public global::Kampai.Common.PickControllerModel pickControllerModel { get; set; }

		[Inject]
		public global::Kampai.Game.AwardLevelSignal awardLevelSignal { get; set; }

		[Inject]
		public global::Kampai.Main.LanguageChangedSignal languageChangedSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		public override void OnRegister()
		{
			view.Init(hudChangingSiblingIndexSignal);
			languageChangedSignal.AddListener(OnLanguageChanged);
			setStorageSignal.AddListener(SetStorage);
			closeAllOtherMenusSignal.AddListener(CloseAllMenu);
			closeSignal.AddListener(CloseMenu);
			view.MenuMoved.AddListener(MenuMoved);
			view.StorageButton.ClickedSignal.AddListener(OpenStorage);
			view.StorageExpandButton.ClickedSignal.AddListener(ExpandStorage);
			view.PetsButton.ClickedSignal.AddListener(ShowPetsXPromo);
			showHUDSignal.AddListener(ToggleHUD);
			showSettingsButtonSignal.AddListener(ToggleSettings);
			peekHUDSignal.AddListener(PeekHUD);
			hideDelayHUDSignal.AddListener(SetupHide);
			setHUDButtonsVisibleSignal.AddListener(SetButtonsVisible);
			togglePopupSignal.AddListener(TogglePopup);
			ftueHighlightSignal.AddListener(ShowFTUEXP);
			firePremiumSignal.AddListener(PlayPremiumVFX);
			fireGrindSignal.AddListener(PlayGrindVFX);
			rushDialogPurchaseHelper.actionSuccessfulSignal.AddListener(OnExpandStorageSuccess);
			currencyDialogClosedSignal.AddListener(OnCurrencyDone);
			setStorageMenuEnabledSignal.AddListener(SetStorageButtonVisible);
			updateSaleBadgeSignal.AddListener(UpdateSaleBadgeCount);
			questDialogDismissed.AddListener(QuestDialogDismissed);
			showOfflinePopupSignal.AddListener(OnPauseSound);
			beginCharacterLoop.AddListener(OnCharacterLoopBegin);
			endCharacterLoop.AddListener(OnCharacterLoopEnd);
			awardLevelSignal.AddListener(OnLevelUp);
			enableVillainHudSignal.AddListener(OnEnableVillainHud);
			view.ExitLairButton.ClickedSignal.AddListener(OnExitLairButtonClicked);
			showPetsButtonSignal.AddListener(HandlePetsButtonRequest);
			RegisterCurrencySignals();
			CheckStorageExpansionLimitReached();
			UpdateSaleBadgeCount();
			CheckShowPetsXPromo();

			// Initial updates
			SetGrindCurrency();
			SetPremiumCurrency();
			SetStorage();
		}

		public override void OnRemove()
		{
			languageChangedSignal.RemoveListener(OnLanguageChanged);
			setStorageSignal.RemoveListener(SetStorage);
			closeAllOtherMenusSignal.RemoveListener(CloseAllMenu);
			closeSignal.RemoveListener(CloseMenu);
			view.MenuMoved.RemoveListener(MenuMoved);
			view.StorageButton.ClickedSignal.RemoveListener(OpenStorage);
			view.StorageExpandButton.ClickedSignal.RemoveListener(ExpandStorage);
			rushDialogPurchaseHelper.actionSuccessfulSignal.RemoveListener(OnExpandStorageSuccess);
			showHUDSignal.RemoveListener(ToggleHUD);
			showSettingsButtonSignal.RemoveListener(ToggleSettings);
			peekHUDSignal.RemoveListener(PeekHUD);
			hideDelayHUDSignal.RemoveListener(SetupHide);
			setHUDButtonsVisibleSignal.RemoveListener(SetButtonsVisible);
			ftueHighlightSignal.RemoveListener(ShowFTUEXP);
			togglePopupSignal.RemoveListener(TogglePopup);
			firePremiumSignal.RemoveListener(PlayPremiumVFX);
			fireGrindSignal.RemoveListener(PlayGrindVFX);
			currencyDialogClosedSignal.RemoveListener(OnCurrencyDone);
			updateSaleBadgeSignal.RemoveListener(UpdateSaleBadgeCount);
			questDialogDismissed.RemoveListener(QuestDialogDismissed);
			showOfflinePopupSignal.RemoveListener(OnPauseSound);
			beginCharacterLoop.RemoveListener(OnCharacterLoopBegin);
			endCharacterLoop.RemoveListener(OnCharacterLoopEnd);
			RemoveVillainLairSignals();
			RemoveCurrencySignals();
			rushDialogPurchaseHelper.Cleanup();
		}

		private void OnLanguageChanged()
		{
			SetGrindCurrency();
			SetPremiumCurrency();
			SetStorage();
			UpdateSaleBadgeCount();
		}

		private void OnPauseSound(bool isPause)
		{
			pauseSoundSignal.Dispatch(isPause);
		}

		private void RegisterCurrencySignals()
		{
			setGrindCurrencySignal.AddListener(SetGrindCurrency);
			setPremiumCurrencySignal.AddListener(SetPremiumCurrency);
			showMTXStoreSignal.AddListener(ShowStore);
			view.PremiumMenuButton.ClickedSignal.AddListener(OnPremiumButtonClicked);
			view.PremiumIconButton.ClickedSignal.AddListener(OnPremiumButtonClicked);
			view.PremiumTextButton.ClickedSignal.AddListener(OnPremiumButtonClicked);
			view.GrindMenuButton.ClickedSignal.AddListener(OnGrindButtonClicked);
			view.GrindIconButton.ClickedSignal.AddListener(OnGrindButtonClicked);
			view.GrindTextButton.ClickedSignal.AddListener(OnGrindButtonClicked);
			view.StoreMenuButton.ClickedSignal.AddListener(OnStoreButtonClicked);
				view.BackgroundButton.ClickedSignal.AddListener(CloseMenuAndCurrency);
		}

		private void RemoveCurrencySignals()
		{
			setGrindCurrencySignal.RemoveListener(SetGrindCurrency);
			setPremiumCurrencySignal.RemoveListener(SetPremiumCurrency);
			showMTXStoreSignal.RemoveListener(ShowStore);
			view.PremiumMenuButton.ClickedSignal.RemoveListener(OnPremiumButtonClicked);
			view.PremiumIconButton.ClickedSignal.RemoveListener(OnPremiumButtonClicked);
			view.PremiumTextButton.ClickedSignal.RemoveListener(OnPremiumButtonClicked);
			view.GrindMenuButton.ClickedSignal.RemoveListener(OnGrindButtonClicked);
			view.GrindIconButton.ClickedSignal.RemoveListener(OnGrindButtonClicked);
			view.GrindTextButton.ClickedSignal.RemoveListener(OnGrindButtonClicked);
			view.StoreMenuButton.ClickedSignal.RemoveListener(OnStoreButtonClicked);
			view.BackgroundButton.ClickedSignal.RemoveListener(CloseMenuAndCurrency);
		}

		private void RemoveVillainLairSignals()
		{
			enableVillainHudSignal.RemoveListener(OnEnableVillainHud);
			view.ExitLairButton.ClickedSignal.RemoveListener(OnExitLairButtonClicked);
		}

		private void DisplayAllWorldToGlassUI(bool display)
		{
			toggleAllFloatingTextSignal.Dispatch(display);
			if (display)
			{
				if (!characterLoopIsOn && villainLairModel.currentActiveLair == null)
				{
					showAllWayFindersSignal.Dispatch();
				}
				showAllResourceIconsSignal.Dispatch();
			}
			else
			{
				if (!characterLoopIsOn && villainLairModel.currentActiveLair == null)
				{
					hideAllWayFindersSignal.Dispatch();
				}
				hideAllResourceIconsSignal.Dispatch();
			}
		}

		private void CheckStorageExpansionLimitReached()
		{
			global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(314);
			if (byInstanceId.CurrentStorageBuildingLevel == byInstanceId.Definition.StorageUpgrades.Count - 1)
			{
				view.StorageExpandButton.gameObject.SetActive(false);
			}
		}

		private void OnExpandStorageSuccess()
		{
			playSFXSignal.Dispatch("Play_expand_storage_01");
			SetStorage();
			CheckStorageExpansionLimitReached();
		}

		private void ExpandStorage()
		{
			int num = 314;
			global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(num);
			if (byInstanceId.State != global::Kampai.Game.BuildingState.Broken && byInstanceId.State != global::Kampai.Game.BuildingState.Inaccessible)
			{
				int transactionId = byInstanceId.Definition.StorageUpgrades[byInstanceId.CurrentStorageBuildingLevel].TransactionId;
				rushDialogPurchaseHelper.Init(transactionId, global::Kampai.Game.TransactionTarget.STORAGEBUILDING, new global::Kampai.Game.TransactionArg(num));
				rushDialogPurchaseHelper.TryAction(true);
			}
		}

		private void HandlePetsButtonRequest(bool enable)
		{
			if (enable)
			{
				CheckShowPetsXPromo();
			}
			else
			{
				TogglePetsButton(false);
			}
		}

		private void ShowPetsXPromo()
		{
			playSFXSignal.Dispatch("Play_menu_popUp_01");
			telemetryService.Send_Telemetry_EVT_GAME_BUTTON_PRESSED_GENERIC(global::Kampai.Util.GameConstants.TrackedGameButton.PetsXPromo_HUD, string.Empty);
			showPetsXPromoSignal.Dispatch();
		}

		private void OpenStorage()
		{
			if (!ButtonClicked())
			{
				return;
			}
			global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
			if (minionPartyInstance == null || (!minionPartyInstance.CharacterUnlocking && !minionPartyInstance.IsPartyHappening))
			{
				global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(314);
				if (byInstanceId.State != global::Kampai.Game.BuildingState.Broken && byInstanceId.State != global::Kampai.Game.BuildingState.Inaccessible)
				{
					OpenStorageBuildingSignal.Dispatch(byInstanceId, true);
				}
			}
		}

		private void ShowStore(global::Kampai.Util.Tuple<int, int> categorySettings)
		{
			closeAllOtherMenusSignal.Dispatch(base.gameObject);
			view.MoveMenu(true);
			displayCurrencyStoreSignal.Dispatch(categorySettings);
			view.ActivateBackgroundButton();
			if (playerService.GetHighestFtueCompleted() < 9)
			{
				questScriptService.PauseQuestScripts();
			}
			DisplayAllWorldToGlassUI(false);
			UpdateSaleBadgeCount();
		}

		private void ShowStoreInternal(int storeCategoryDefintiionID, int amountNeeded)
		{
			ShowStore(new global::Kampai.Util.Tuple<int, int>(storeCategoryDefintiionID, amountNeeded));
		}

		internal void CloseAllMenu(global::UnityEngine.GameObject exception)
		{
			if (base.gameObject != exception)
			{
				CloseMenu(false);
			}
		}

		private void SetStorage()
		{
			uint currentStorageCapacity = playerService.GetCurrentStorageCapacity();
			uint storageCount = playerService.GetStorageCount();
			view.SetStorage(storageCount, currentStorageCapacity);
			view.PlayStorageVFX();
		}

		internal void SetGrindCurrency()
		{
			uint quantity = playerService.GetQuantity(global::Kampai.Game.StaticItem.GRIND_CURRENCY_ID);
			view.SetGrindCurrency(quantity);
		}

		internal void SetPremiumCurrency()
		{
			uint quantity = playerService.GetQuantity(global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID);
			view.SetPremiumCurrency(quantity);
		}

		internal void CloseMenu(bool closeCurrency)
		{
			if (closeCurrency)
			{
				CloseMenuAndCurrency();
			}
			view.MoveMenu(false);
			UpdateSaleBadgeCount();
		}

		internal void CloseMenuAndCurrency()
		{
			currencyService.CurrencyDialogClosed(false);
			OnCurrencyDone();
		}

		internal void OnPremiumButtonClicked()
		{
			if (ButtonClicked())
			{
				ShowStoreInternal(800002, 0);
			}
		}

		internal void OnGrindButtonClicked()
		{
			if (ButtonClicked())
			{
				ShowStoreInternal(800001, 0);
			}
		}

		internal void OnStoreButtonClicked()
		{
			if (ButtonClicked())
			{
				ShowStoreInternal(800003, 0);
			}
		}

		internal void OnEnableVillainHud(bool isEnabled)
		{
			view.EnableVillainHud(isEnabled);
			setBuildMenuEnabledSignal.Dispatch(!isEnabled);
		}

		internal void OnExitLairButtonClicked()
		{
			if (!pickControllerModel.PanningCameraBlocked)
			{
				exitVillainLairSignal.Dispatch(new global::Kampai.Util.Boxed<global::System.Action>(OnExitLairComplete));
			}
		}

		private void OnExitLairComplete()
		{
			displayDisco3DElements.Dispatch(true);
		}

		internal void QuestDialogDismissed()
		{
			StopCoroutine(RenableStoreButton());
			view.EnableStoreMenuButton(false);
			StartCoroutine(RenableStoreButton());
		}

		internal global::System.Collections.IEnumerator RenableStoreButton()
		{
			yield return new global::UnityEngine.WaitForSeconds(0.25f);
			view.EnableStoreMenuButton(true);
		}

		internal void OnCurrencyDone()
		{
			if (playerService.GetHighestFtueCompleted() < 9)
			{
				questScriptService.ResumeQuestScripts();
			}
			DisplayAllWorldToGlassUI(true);
			if (view.isInForeground)
			{
				view.MoveMenu(false);
			}
		}

		internal void UpdateSaleBadgeCount()
		{
			int num = 0;
			foreach (global::Kampai.Game.CurrencyStoreCategoryDefinition currencyStoreCategoryDefinition in definitionService.GetCurrencyStoreCategoryDefinitions())
			{
				num += currencyStoreService.GetBadgeCount(currencyStoreCategoryDefinition);
			}
			bool flag = num != 0;
			if (flag)
			{
				view.SaleCount.text = num.ToString();
			}
			view.SaleBadge.SetActive(flag);
		}

		internal void CheckShowPetsXPromo()
		{
			global::Kampai.Game.ConfigurationDefinition configurations = configurationsService.GetConfigurations();
			bool flag = configurations != null && configurations.promoPetsEnabled;
			if (flag)
			{
				global::Kampai.Game.PetsXPromoDefinition petsXPromoDefinition = definitionService.Get<global::Kampai.Game.PetsXPromoDefinition>(95000);
				if (petsXPromoDefinition != null && petsXPromoDefinition.PetsXPromoEnabled)
				{
					int quantity = (int)playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID);
					if (quantity < petsXPromoDefinition.PetsXPromoSurfaceLevel)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			TogglePetsButton(flag);
		}

		internal bool ButtonClicked()
		{
			global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
			if (villainLairModel.currentActiveLair != null)
			{
				return true;
			}
			if (view.IsHiding() || minionPartyInstance.IsPartyHappening || uiModel.LevelUpUIOpen)
			{
				return false;
			}
			return true;
		}

		internal void MenuMoved(bool show)
		{
			if (show)
			{
				uiAddedSignal.Dispatch(base.gameObject, delegate
				{
					CloseAllMenu(null);
				});
				playSFXSignal.Dispatch("Play_main_menu_open_01");
			}
			else
			{
				uiRemovedSignal.Dispatch(base.gameObject);
				playSFXSignal.Dispatch("Play_main_menu_close_01");
			}
		}

		internal void TogglePopup(bool enable)
		{
			DisplayAllWorldToGlassUI(!enable);
			view.TogglePopup(enable);
		}

		internal void ToggleHUD(bool enable)
		{
			global::UnityEngine.Debug.Log("[HUDMediator] ToggleHUD called with: " + enable);
			CancelPeek();
			view.Toggle(enable);
			if (enable)
			{
				currencyService.ResumeTransactionsHandling();
			}
		}

		internal void ToggleSettings(bool enable)
		{
			view.ToggleSettings(enable);
		}

		internal void TogglePetsButton(bool enable)
		{
			view.TogglePetsButton(enable);
		}

		private void OnLevelUp(global::Kampai.Game.Transaction.TransactionDefinition td)
		{
			CheckShowPetsXPromo();
		}

		private void CancelPeek()
		{
			if (currentPeekToken != null)
			{
				currentPeekToken.Set(false);
				currentPeekToken = null;
			}
		}

		internal void PeekHUD(float seconds)
		{
			if (view.IsHiding())
			{
				CancelPeek();
				view.Toggle(true);
				SetupHide(seconds);
			}
		}

		internal void SetupHide(float seconds)
		{
			if (!view.IsHiding())
			{
				currentPeekToken = new global::Kampai.Util.MutableBoxed<bool>(true);
				StartCoroutine(HideAfterSeconds(seconds, currentPeekToken));
			}
		}

		private global::System.Collections.IEnumerator HideAfterSeconds(float seconds, global::Kampai.Util.Boxed<bool> shouldStillHide)
		{
			yield return new global::UnityEngine.WaitForSeconds(seconds);
			if (shouldStillHide.Value)
			{
				view.Toggle(false);
				currentPeekToken = null;
			}
		}

		private void SetStorageButtonVisible(bool visible)
		{
			view.SetStorageButtonVisible(visible);
		}

		private void SetButtonsVisible(bool visible)
		{
			view.SetButtonsVisible(visible);
		}

		internal void ShowFTUEXP(bool show)
		{
			view.ToggleDarkSkrim(false);
		}

		private void PlayPremiumVFX()
		{
			view.PlayPremiumVFX();
		}

		private void PlayGrindVFX()
		{
			view.PlayGrindVFX();
		}

		private void OnCharacterLoopBegin(global::Kampai.Game.View.CharacterObject co)
		{
			characterLoopIsOn = true;
		}

		private void OnCharacterLoopEnd(global::Kampai.Game.View.CharacterObject co, int routeIndex)
		{
			characterLoopIsOn = false;
		}
	}
}
