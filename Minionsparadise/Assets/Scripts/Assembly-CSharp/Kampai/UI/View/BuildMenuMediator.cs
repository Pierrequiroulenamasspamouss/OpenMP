namespace Kampai.UI.View
{
	public class BuildMenuMediator : global::strange.extensions.mediation.impl.EventMediator
	{
		private global::Kampai.Game.InventoryBuildingMovementSignal toInventorySignal;

		private global::Kampai.Util.MutableBoxed<bool> currentPeekToken;

		[Inject]
		public global::Kampai.UI.View.BuildMenuView view { get; set; }

		[Inject]
		public global::Kampai.UI.View.MoveBuildMenuSignal moveSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeAllMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.LoadDefinitionForUISignal loadDefinitionSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal playSFXSignal { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.UI.View.BuildMenuButtonClickedSignal openButtonClickedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowStoreSignal showStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.BuildMenuOpenedSignal buildMenuOpenedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetNewUnlockForBuildMenuSignal setNewUnlockForBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetInventoryCountForBuildMenuSignal setInventoryCountForBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.IncreaseInventoryCountForBuildMenuSignal increaseInventoryCountForBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideStoreHighlightSignal hideStoreHighlightSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.UI.IBuildMenuService buildMenuService { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIAddedSignal uiAddedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIRemovedSignal uiRemovedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetBuildMenuEnabledSignal setBuildMenuEnabledSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.StopAutopanSignal stopAutopanSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveUnlockForBuildMenuSignal removeUnlockForBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllMessageDialogs closeAllDialogsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowMoveBuildingMenuSignal showMoveBuildingMenuSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisableBuildMenuButtonSignal disableBuildMenuSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIModel uiModel { get; set; }

		[Inject]
		public global::Kampai.Game.VillainLairModel lairModel { get; set; }

		[Inject]
		public global::Kampai.UI.View.EnableBuildMenuFromLairSignal enableBuildMenuFromLairSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.PeekStoreSignal peekStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideDelayStoreSignal hideDelayStoreSignal { get; set; }

		public override void OnRegister()
		{
			global::Kampai.Util.TimeProfiler.StartSection("BuildMenuMediator");
			view.Init();
			loadDefinitionSignal.Dispatch();
			view.MenuButton.ClickedSignal.AddListener(OnMenuButtonClicked);
			closeAllMenuSignal.AddListener(CloseAllMenu);
			moveSignal.AddListener(MoveMenu);
			setBuildMenuEnabledSignal.AddListener(SetBuildMenuEnabled);
			enableBuildMenuFromLairSignal.AddListener(SetBuildMenuEnabledNoCheck);
			showMoveBuildingMenuSignal.AddListener(ShowMoveBuildingMenu);
			toInventorySignal = gameContext.injectionBinder.GetInstance<global::Kampai.Game.InventoryBuildingMovementSignal>();
			toInventorySignal.AddListener(ToInventory);
			showStoreSignal.AddListener(ToggleStore);
			setNewUnlockForBuildMenuSignal.AddListener(SetNewUnlock);
			removeUnlockForBuildMenuSignal.AddListener(RemoveUnlock);
			setInventoryCountForBuildMenuSignal.AddListener(SetBadgeCount);
			increaseInventoryCountForBuildMenuSignal.AddListener(IncreaseInventoryCount);
			disableBuildMenuSignal.AddListener(view.DisableBuildButton);
			peekStoreSignal.AddListener(PeekStore);
			hideDelayStoreSignal.AddListener(SetupHide);
			global::Kampai.Util.TimeProfiler.EndSection("BuildMenuMediator");
		}

		public override void OnRemove()
		{
			view.MenuButton.ClickedSignal.RemoveListener(OnMenuButtonClicked);
			moveSignal.RemoveListener(MoveMenu);
			toInventorySignal.RemoveListener(ToInventory);
			closeAllMenuSignal.RemoveListener(CloseAllMenu);
			setBuildMenuEnabledSignal.RemoveListener(SetBuildMenuEnabled);
			enableBuildMenuFromLairSignal.RemoveListener(SetBuildMenuEnabledNoCheck);
			showMoveBuildingMenuSignal.RemoveListener(ShowMoveBuildingMenu);
			showStoreSignal.RemoveListener(ToggleStore);
			setNewUnlockForBuildMenuSignal.RemoveListener(SetNewUnlock);
			removeUnlockForBuildMenuSignal.RemoveListener(RemoveUnlock);
			setInventoryCountForBuildMenuSignal.RemoveListener(SetBadgeCount);
			increaseInventoryCountForBuildMenuSignal.RemoveListener(IncreaseInventoryCount);
			disableBuildMenuSignal.RemoveListener(view.DisableBuildButton);
			peekStoreSignal.RemoveListener(PeekStore);
			hideDelayStoreSignal.RemoveListener(SetupHide);
		}

		internal void ToInventory()
		{
			view.IncreaseBadgeCounter();
		}

		internal void RemoveUnlock(int count)
		{
			view.RemoveUnlockBadge(count);
		}

		internal void SetNewUnlock(int count)
		{
			view.SetUnlockBadge(count);
		}

		internal void SetBadgeCount(int count)
		{
			view.SetBadgeCount(count);
		}

		internal void IncreaseInventoryCount()
		{
			view.IncreaseBadgeCounter();
		}

		internal void CloseAllMenu(global::UnityEngine.GameObject exception)
		{
			if (base.gameObject != exception && view.isOpen)
			{
				moveSignal.Dispatch(false);
			}
		}

		internal void OnMenuButtonClicked()
		{
			if (global::Kampai.Game.InputUtils.touchCount <= 1)
			{
				telemetryService.Send_Telemetry_EVT_IGE_STORE_VISIT("Menu Button", "Building Menu");
				moveSignal.Dispatch(!view.isOpen);
				if (view.isOpen)
				{
					buildMenuService.SetStoreUnlockChecked();
					closeAllMenuSignal.Dispatch(base.gameObject);
					Kampai.Input.InputCompat.MultiTouchEnabled = true;
				}
				openButtonClickedSignal.Dispatch();
				uiModel.GoToInEffect = false;
			}
		}

		internal void MoveMenu(bool show)
		{
			if (show == view.isOpen)
			{
				return;
			}
			if (show)
			{
				global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
				if (minionPartyInstance != null && minionPartyInstance.CharacterUnlocking)
				{
					return;
				}
				stopAutopanSignal.Dispatch();
			}
			view.MoveMenu(show);
			if (show)
			{
				uiAddedSignal.Dispatch(view.gameObject, OnMenuButtonClicked);
				playSFXSignal.Dispatch("Play_main_menu_open_01");
			}
			else
			{
				closeAllDialogsSignal.Dispatch();
				uiRemovedSignal.Dispatch(view.gameObject);
				hideStoreHighlightSignal.Dispatch();
				playSFXSignal.Dispatch("Play_main_menu_close_01");
			}
			buildMenuOpenedSignal.Dispatch();
		}

		private bool shouldEnableBuildMenu(bool enable)
		{
			return enable && (uiModel.UIState & global::Kampai.UI.View.UIModel.UIStateFlags.StoreButtonHiddenFromQuest) == 0 && lairModel.currentActiveLair == null;
		}

		internal void ToggleStore(bool enable)
		{
			CancelPeek();
			view.Toggle(enable);
			view.DisableBuildButton(!enable);
		}

		private void SetBuildMenuEnabled(bool isEnabled)
		{
			if (lairModel.currentActiveLair == null)
			{
				SetBuildMenuEnabledNoCheck(isEnabled);
			}
		}

		private void SetBuildMenuEnabledNoCheck(bool isEnabled)
		{
			bool buildMenuButtonEnabled = shouldEnableBuildMenu(isEnabled);
			view.SetBuildMenuButtonEnabled(buildMenuButtonEnabled);
		}

		private void ShowMoveBuildingMenu(bool show, global::Kampai.UI.View.MoveBuildingSetting setting)
		{
			bool buildMenuButtonEnabled = shouldEnableBuildMenu(!show);
			view.SetBuildMenuButtonEnabled(buildMenuButtonEnabled);
		}

		private void CancelPeek()
		{
			if (currentPeekToken != null)
			{
				currentPeekToken.Set(false);
				currentPeekToken = null;
			}
		}

		private void PeekStore(float seconds)
		{
			if (view.IsHiding())
			{
				CancelPeek();
				view.Toggle(true);
				SetupHide(seconds);
			}
		}

		private void SetupHide(float seconds)
		{
			if (!view.IsHiding())
			{
				currentPeekToken = new global::Kampai.Util.MutableBoxed<bool>(true);
				StartCoroutine(HideAfter(seconds, currentPeekToken));
			}
		}

		private global::System.Collections.IEnumerator HideAfter(float seconds, global::Kampai.Util.Boxed<bool> shouldStillHide)
		{
			yield return new global::UnityEngine.WaitForSeconds(seconds);
			if (shouldStillHide.Value)
			{
				view.Toggle(false);
				currentPeekToken = null;
			}
		}
	}
}
