namespace Kampai.Game.View
{
	internal sealed class BuildingManagerMediator : global::strange.extensions.mediation.impl.EventMediator
	{
		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("BuildingManagerMediator") as global::Kampai.Util.IKampaiLogger;

		private global::Kampai.Game.View.DummyBuildingObject currentDummyBuilding;

		private global::strange.extensions.signal.impl.Signal<int> triggerGagAnimation = new global::strange.extensions.signal.impl.Signal<int>();

		private global::strange.extensions.signal.impl.Signal<int> minionTaskingAnimationDone = new global::strange.extensions.signal.impl.Signal<int>();

		private bool allowStorable = true;

		private global::Kampai.UI.View.SetBuildingRushedSignal setBuildingRushedSignal;

		private global::Kampai.UI.View.RushRevealBuildingSignal rushRevealBuildingSignal;

		private int buildingID = -1;

		[Inject(global::Kampai.UI.View.UIElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable uiContext { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeEventService timeEventService { get; set; }

		[Inject(global::Kampai.Game.GameElement.MINION_MANAGER)]
		public global::UnityEngine.GameObject minionManager { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.ILandExpansionService landExpansionService { get; set; }

		[Inject]
		public global::Kampai.Game.IQuestService questService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.View.BuildingManagerView view { get; set; }

		[Inject]
		public global::Kampai.Game.SelectBuildingSignal selectBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DeselectBuildingSignal deselectBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MoveBuildingSignal moveBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MoveScaffoldingSignal moveScaffoldingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartTaskSignal startTaskSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SignalActionSignal stopTaskSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal playLocalAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StopLocalAudioSignal stopLocalAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RevealBuildingSignal revealBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveWayFinderSignal removeWayFinderSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CreateWayFinderSignal createWayFinderSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DisableCameraBehaviourSignal disableCameraSignal { get; set; }

		[Inject]
		public global::Kampai.Game.EnableCameraBehaviourSignal enableCameraSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CreateDummyBuildingSignal createDummyBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ShowBuildingFootprintSignal showBuildingFootprintSignal { get; set; }

		[Inject]
		public global::Kampai.Common.PickControllerModel model { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal buildingChangeStateSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingHarvestSignal buildingHarvestSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateTaskedMinionSignal updateTaskedMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartConstructionSignal startConstructionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreBuildingSignal restoreBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreScaffoldingViewSignal restoreScaffoldingViewSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreRibbonViewSignal restoreRibbonViewSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestorePlatformViewSignal restorePlatformViewSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreBuildingViewSignal restoreBuildingViewSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CameraAutoMoveSignal autoMoveSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ShowHiddenBuildingsSignal showHiddenBuildingsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UITryHarvestSignal tryHarvestSignal { get; set; }

		[Inject]
		public global::Kampai.Common.TryHarvestBuildingSignal harvestBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Common.RepairBuildingSignal repairBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Common.RecreateBuildingSignal recreateBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingReactionSignal buildingReactionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RotateBuildingSignal rotateBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Common.MinionTaskCompleteSignal minionTaskCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.EjectMinionFromBuildingSignal ejectMinionFromBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionStateChangeSignal minionStateChange { get; set; }

		[Inject]
		public global::Kampai.Game.EjectAllMinionsFromBuildingSignal ejectAllMinionsFromBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.PopupMessageSignal popupMessageSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.Game.PurchaseNewBuildingSignal purchaseNewBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SendBuildingToInventorySignal sendBuildingToInventorySignal { get; set; }

		[Inject]
		public global::Kampai.Game.CreateInventoryBuildingSignal createInventoryBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CancelPurchaseSignal cancelPurchaseSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetBuildingPositionSignal setBuildingPositionSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal globalAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.AddFootprintSignal addFootprintSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupBrokenBridgesSignal setupBrokenBridgesSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BurnLandExpansionSignal burnLandExpansionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupLandExpansionsSignal setupLandExpansionsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupDebrisSignal setupDebrisSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupAspirationalBuildingsSignal setupAspirationalBuildingsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DisplayBuildingSignal displayBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPrestigeService characterService { get; set; }

		[Inject]
		public global::Kampai.Game.BRBCelebrationAnimationSignal brbExitAnimimationSignal { get; set; }

		[Inject]
		public global::strange.extensions.injector.api.IInjectionBinder injectionBinder { get; set; }

		[Inject]
		public global::Kampai.Game.InitBuildingObjectSignal initBuildingObjectSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetBuildMenuEnabledSignal setBuildMenuEnabledSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CleanupDebrisSignal cleanupDebrisSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ToggleMinionRendererSignal toggleMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RelocateTaskedMinionsSignal relocateMinionsSignal { get; set; }

		[Inject]
		public global::Kampai.Util.ICoroutineProgressMonitor coroutineProgressMonitor { get; set; }

		[Inject]
		public global::Kampai.Game.MarketplaceUpdateSoldItemsSignal updateSoldItemsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.OpenStoreHighlightItemSignal openStoreSignal { get; set; }

		[Inject]
		public global::Kampai.Game.HighlightBuildingSignal highlightBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetMinionPartyBuildingStateSignal setMinionPartyBuildingStateSignal { get; set; }

		[Inject]
		public global::Kampai.Game.PreLoadPartyAssetsSignal preLoadPartyAssetsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DisplayPlayerTrainingSignal playerTrainingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.PrepareTaskingMinionsForPartySignal prepareTaskingMinionsForPartySignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreTaskingMinionsFromPartySignal restoreTaskingMinionFromPartySignal { get; set; }

		[Inject]
		public global::Kampai.Game.EnableVillainIslandCollidersSignal enableVillainIslandCollidersSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetStorageMenuEnabledSignal setStorageMenuEnabledSignal { get; set; }

		[Inject]
		public global::Kampai.Game.PhilSignFixedSignal philSignFixedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StopGagAnimationSignal stopGagAnimationSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ShowHarvestReadySignal showHarvestReadySignal { get; set; }

		[Inject]
		public global::Kampai.Game.ISpecialEventService specialEventService { get; set; }

		[Inject]
		public global::Kampai.Game.MasterPlanCompleteSignal masterPlanCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RevealMasterPlanComponentSignal revealMasterPlanComponentSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideFluxWayfinder hideWayfinder { get; set; }

		[Inject]
		public global::Kampai.Game.SetMasterPlanWayfinderIconToCompleteSignal setCompleteWayfinderSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DisplayVolcanoLairVillainWayfinderSignal displayVolcanoWayfinderSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IMasterPlanService masterPlanService { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowHUDSignal showHUDSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowAllWayFindersSignal showAllWayFindersSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideAllWayFindersSignal hideAllWayFindersSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RelocateCharacterSignal relocateCharacterSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DecoGridModel decoGridModel { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateConnectablesSignal updateConnectablesSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Scaffolding currentScaffolding { get; set; }

		public float HarvestTimer { get; set; }

		public int LastHarvestedBuildingID { get; set; }

		public override void OnRegister()
		{
			view.Init(logger, definitionService, masterPlanService, specialEventService.IsSpecialEventActive());
			view.updateMinionSignal.AddListener(UpdateTaskedMinions);
			view.addFootprintSignal.AddListener(AddFootprint);
			ManageRegisterStackSize();
			restoreBuildingViewSignal.AddListener(RestoreBuildingView);
			buildingChangeStateSignal.AddListener(UpdateViewFromBuildingState);
			triggerGagAnimation.AddListener(TriggerGagAnimation);
			repairBuildingSignal.AddListener(RepairBuilding);
			recreateBuildingSignal.AddListener(RecreateBuilding);
			displayBuildingSignal.AddListener(DisplayBuilding);
			tryHarvestSignal.AddListener(TryHarvest);
			deselectBuildingSignal.AddListener(DeselectBuilding);
			purchaseNewBuildingSignal.AddListener(PurchaseNewBuilding);
			sendBuildingToInventorySignal.AddListener(SendToInventory);
			createInventoryBuildingSignal.AddListener(CreateInventoryBuilding);
			cancelPurchaseSignal.AddListener(CancelPurchaseStart);
			setBuildingPositionSignal.AddListener(SetBuildingPosition);
			ejectAllMinionsFromBuildingSignal.AddListener(EjectAllMinionsFromBuilding);
			minionTaskingAnimationDone.AddListener(OnMinionTaskingAnimationDone);
			burnLandExpansionSignal.AddListener(OnBurnedLandExpansion);
			setMinionPartyBuildingStateSignal.AddListener(SetMinionPartyBuildingState);
			preLoadPartyAssetsSignal.AddListener(PreLoadMinionPartyBuildings);
			ejectMinionFromBuildingSignal.AddListener(EjectMinionFromBuilding);
			stopGagAnimationSignal.AddListener(StopGagAnimation);
			showHarvestReadySignal.AddListener(ShowHarvestReady);
			coroutineProgressMonitor.StartTask(Init(), "init buildings");
		}

		private void ManageRegisterStackSize()
		{
			view.updateResourceBuildingSignal.AddListener(VerifyResourceBuildingSlots);
			view.setBuildingNumberSignal.AddListener(SetBuildingNumber);
			setBuildMenuEnabledSignal.AddListener(AllowStorable);
			view.initBuildingObject.AddListener(InitBuildingObject);
			selectBuildingSignal.AddListener(SelectBuilding);
			moveBuildingSignal.AddListener(MoveBuilding);
			moveScaffoldingSignal.AddListener(MoveDummyBuildingObject);
			startTaskSignal.AddListener(StartMinionTask);
			revealBuildingSignal.AddListener(OnRevealBuilding);
			createDummyBuildingSignal.AddListener(CreateDummyBuilding);
			buildingHarvestSignal.AddListener(HarvestComplete);
			restoreScaffoldingViewSignal.AddListener(RestoreScaffoldingView);
			restoreRibbonViewSignal.AddListener(RestoreRibbonView);
			restorePlatformViewSignal.AddListener(RestorePlatformView);
			highlightBuildingSignal.AddListener(HighlightBuilding);
			prepareTaskingMinionsForPartySignal.AddListener(PrepareTaskingMinionForMinionParty);
			restoreTaskingMinionFromPartySignal.AddListener(RestoreTaskingMinionFromParty);
			if (uiContext != null && uiContext.injectionBinder != null)
			{
				setBuildingRushedSignal = uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetBuildingRushedSignal>();
				rushRevealBuildingSignal = uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.RushRevealBuildingSignal>();
				if (setBuildingRushedSignal != null)
				{
					setBuildingRushedSignal.AddListener(SetBuildingRushed);
				}
				if (rushRevealBuildingSignal != null)
				{
					rushRevealBuildingSignal.AddListener(RushRevealBuilding);
				}
			}
			rotateBuildingSignal.AddListener(RotateBuilding);
		}

		public override void OnRemove()
		{
			view.addFootprintSignal.RemoveListener(AddFootprint);
			ManageRemoveStackSize();
			buildingHarvestSignal.RemoveListener(HarvestComplete);
			restoreScaffoldingViewSignal.RemoveListener(RestoreScaffoldingView);
			restorePlatformViewSignal.RemoveListener(RestorePlatformView);
			restoreRibbonViewSignal.RemoveListener(RestoreRibbonView);
			restoreBuildingViewSignal.RemoveListener(RestoreBuildingView);
			buildingChangeStateSignal.RemoveListener(UpdateViewFromBuildingState);
			triggerGagAnimation.RemoveListener(TriggerGagAnimation);
			repairBuildingSignal.RemoveListener(RepairBuilding);
			recreateBuildingSignal.RemoveListener(RecreateBuilding);
			displayBuildingSignal.RemoveListener(DisplayBuilding);
			setBuildMenuEnabledSignal.RemoveListener(AllowStorable);
			tryHarvestSignal.RemoveListener(TryHarvest);
			deselectBuildingSignal.RemoveListener(DeselectBuilding);
			purchaseNewBuildingSignal.RemoveListener(PurchaseNewBuilding);
			sendBuildingToInventorySignal.RemoveListener(SendToInventory);
			createInventoryBuildingSignal.RemoveListener(CreateInventoryBuilding);
			cancelPurchaseSignal.RemoveListener(CancelPurchaseStart);
			setBuildingPositionSignal.RemoveListener(SetBuildingPosition);
			ejectAllMinionsFromBuildingSignal.RemoveListener(EjectAllMinionsFromBuilding);
			minionTaskingAnimationDone.RemoveListener(OnMinionTaskingAnimationDone);
			burnLandExpansionSignal.RemoveListener(OnBurnedLandExpansion);
			ejectMinionFromBuildingSignal.RemoveListener(EjectMinionFromBuilding);
			stopGagAnimationSignal.RemoveListener(StopGagAnimation);
			showHarvestReadySignal.RemoveListener(ShowHarvestReady);
			view.initBuildingObject.RemoveListener(InitBuildingObject);
			if (setBuildingRushedSignal != null)
			{
				setBuildingRushedSignal.RemoveListener(SetBuildingRushed);
			}
			if (rushRevealBuildingSignal != null)
			{
				rushRevealBuildingSignal.RemoveListener(RushRevealBuilding);
			}
			rotateBuildingSignal.RemoveListener(RotateBuilding);
		}

		private void ManageRemoveStackSize()
		{
			view.updateResourceBuildingSignal.RemoveListener(VerifyResourceBuildingSlots);
			view.setBuildingNumberSignal.RemoveListener(SetBuildingNumber);
			selectBuildingSignal.RemoveListener(SelectBuilding);
			moveBuildingSignal.RemoveListener(MoveBuilding);
			moveScaffoldingSignal.RemoveListener(MoveDummyBuildingObject);
			startTaskSignal.RemoveListener(StartMinionTask);
			revealBuildingSignal.RemoveListener(OnRevealBuilding);
			createDummyBuildingSignal.RemoveListener(CreateDummyBuilding);
			highlightBuildingSignal.RemoveListener(HighlightBuilding);
			prepareTaskingMinionsForPartySignal.RemoveListener(PrepareTaskingMinionForMinionParty);
			restoreTaskingMinionFromPartySignal.RemoveListener(RestoreTaskingMinionFromParty);
			setBuildingRushedSignal.RemoveListener(SetBuildingRushed);
			rushRevealBuildingSignal.RemoveListener(RushRevealBuilding);
			setMinionPartyBuildingStateSignal.RemoveListener(SetMinionPartyBuildingState);
			preLoadPartyAssetsSignal.RemoveListener(PreLoadMinionPartyBuildings);
			rotateBuildingSignal.RemoveListener(RotateBuilding);
		}

		[Inject]
		public global::Kampai.Main.IAssetsPreloadService assetsPreloadService { get; set; }

		private global::System.Collections.IEnumerator Init()
		{
			global::Kampai.Util.StartupTimer.LogCheckpoint("BuildingManagerMediator.Init (Start)");
			yield return null;
			while (assetsPreloadService != null && assetsPreloadService.IsPreloading)
			{
				yield return null;
			}
			global::Kampai.Util.TimeProfiler.StartSection("buildings");
			global::System.Collections.Generic.ICollection<global::Kampai.Game.Building> buildingList = playerService.GetInstancesByType<global::Kampai.Game.Building>();
			global::Kampai.Util.TimeProfiler.StartSection("restoring");
			global::System.Diagnostics.Stopwatch sw = global::System.Diagnostics.Stopwatch.StartNew();
			foreach (global::Kampai.Game.Building building in buildingList)
			{
				restoreBuildingSignal.Dispatch(building);
				if (sw.ElapsedMilliseconds > 1500)
				{
					sw.Reset();
					sw.Start();
					yield return null;
				}
			}
			sw.Stop();
			global::Kampai.Util.TimeProfiler.EndSection("restoring");
			yield return coroutineProgressMonitor.waitForPreviousTaskToComplete;
			global::Kampai.Util.TimeProfiler.StartSection("expansions");
			setupBrokenBridgesSignal.Dispatch();
			setupLandExpansionsSignal.Dispatch();
			setupDebrisSignal.Dispatch();
			global::Kampai.Util.TimeProfiler.EndSection("expansions");
			setupAspirationalBuildingsSignal.Dispatch();
			global::Kampai.Util.TimeProfiler.EndSection("buildings");
			global::Kampai.Util.StartupTimer.LogCheckpoint("BuildingManagerMediator.Init (Complete)");
		}

		private void AllowStorable(bool storable)
		{
			allowStorable = storable;
		}

		private void InitBuildingObject(global::Kampai.Game.View.BuildingObject buildingObject, global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> controllers, global::Kampai.Game.Building building)
		{
			initBuildingObjectSignal.Dispatch(buildingObject, controllers, building);
		}

		public void AddFootprint(global::Kampai.Game.Building building, global::Kampai.Game.Location location)
		{
			addFootprintSignal.Dispatch(building, location);
		}

		public void Update()
		{
			if (HarvestTimer > 0f)
			{
				HarvestTimer -= global::UnityEngine.Time.deltaTime;
			}
			if (model.ForceDisabled)
			{
				DestroyDummyBuilding();
			}
		}

		private void InjectBuildingObject(global::UnityEngine.GameObject go, int id)
		{
			switch (id)
			{
			case 3041:
				SetBuildingBinding(go, global::Kampai.Game.StaticItem.TIKI_BAR_BUILDING_ID_DEF);
				break;
			case 3070:
				SetBuildingBinding(go, global::Kampai.Game.StaticItem.WELCOME_BOOTH_BUILDING_ID_DEF);
				break;
			}
		}

		private void SetBuildingBinding(global::UnityEngine.GameObject go, object name)
		{
			global::strange.extensions.injector.api.IInjectionBinding binding = injectionBinder.GetBinding<global::UnityEngine.GameObject>(name);
			if (binding == null)
			{
				injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(go).ToName(name);
			}
			else
			{
				binding.SetValue(go);
			}
		}

		private void RepairBuilding(global::Kampai.Game.Building building)
		{
			int iD = building.ID;
			global::Kampai.Game.BuildingState state = building.State;
			if (state != global::Kampai.Game.BuildingState.Broken && state != global::Kampai.Game.BuildingState.MissingTikiSign)
			{
				return;
			}
			HandleBuildingState(iD, state);
			RecreateBuilding(building);
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(iD);
			buildingObject.SetVFXState("RepairBuilding");
			global::Kampai.Game.StageBuilding stageBuilding = building as global::Kampai.Game.StageBuilding;
			global::Kampai.Game.WelcomeHutBuilding welcomeHutBuilding = building as global::Kampai.Game.WelcomeHutBuilding;
			global::Kampai.Game.CabanaBuilding cabanaBuilding = building as global::Kampai.Game.CabanaBuilding;
			global::Kampai.Game.FountainBuilding fountainBuilding = building as global::Kampai.Game.FountainBuilding;
			global::Kampai.Game.StorageBuilding storageBuilding = building as global::Kampai.Game.StorageBuilding;
			global::Kampai.Game.TikiBarBuilding tikiBarBuilding = building as global::Kampai.Game.TikiBarBuilding;
			global::Kampai.Game.VillainLairEntranceBuilding villainLairEntranceBuilding = building as global::Kampai.Game.VillainLairEntranceBuilding;
			global::Kampai.Game.MinionUpgradeBuilding minionUpgradeBuilding = building as global::Kampai.Game.MinionUpgradeBuilding;
			if (stageBuilding != null)
			{
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.StageRepair, global::Kampai.Game.QuestTaskTransition.Complete);
			}
			else if (welcomeHutBuilding != null)
			{
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.WelcomeHutRepair, global::Kampai.Game.QuestTaskTransition.Complete);
			}
			else if (villainLairEntranceBuilding != null)
			{
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.LairPortalRepair, global::Kampai.Game.QuestTaskTransition.Complete);
			}
			else if (fountainBuilding != null)
			{
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.FountainRepair, global::Kampai.Game.QuestTaskTransition.Complete);
			}
			else if (minionUpgradeBuilding != null)
			{
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.MinionUpgradeBuildingRepair, global::Kampai.Game.QuestTaskTransition.Complete);
			}
			else if (storageBuilding != null)
			{
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.StorageRepair, global::Kampai.Game.QuestTaskTransition.Complete);
				updateSoldItemsSignal.Dispatch(false);
				setStorageMenuEnabledSignal.Dispatch(true);
			}
			else if (cabanaBuilding != null)
			{
				global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.IQuestController> questMap = questService.GetQuestMap();
				foreach (global::Kampai.Game.IQuestController value in questMap.Values)
				{
					if (value.State == global::Kampai.Game.QuestState.RunningTasks && value.IsTrackingThisBuilding(iD, global::Kampai.Game.QuestStepType.CabanaRepair))
					{
						value.UpdateTask(global::Kampai.Game.QuestStepType.CabanaRepair, global::Kampai.Game.QuestTaskTransition.Complete, building);
					}
				}
			}
			else if (tikiBarBuilding != null && state == global::Kampai.Game.BuildingState.MissingTikiSign)
			{
				StartCoroutine(WaitAFrame(delegate
				{
					philSignFixedSignal.Dispatch();
				}));
			}
			CheckForMinionParty(building);
		}

		private void HandleBuildingState(int buildingId, global::Kampai.Game.BuildingState buildingState)
		{
			globalAudioSignal.Dispatch("Play_building_repair_01");
			removeWayFinderSignal.Dispatch(buildingId);
			if (buildingState == global::Kampai.Game.BuildingState.MissingTikiSign)
			{
				buildingChangeStateSignal.Dispatch(buildingId, global::Kampai.Game.BuildingState.Idle);
			}
			else if (buildingId == 313)
			{
				buildingChangeStateSignal.Dispatch(buildingId, global::Kampai.Game.BuildingState.MissingTikiSign);
			}
			else
			{
				buildingChangeStateSignal.Dispatch(buildingId, global::Kampai.Game.BuildingState.Idle);
			}
		}

		private void CheckForMinionParty(global::Kampai.Game.Building building = null)
		{
			global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
			if (minionPartyInstance != null && minionPartyInstance.IsPartyHappening)
			{
				if (building != null && building.Definition is global::Kampai.Game.IMinionPartyBuilding)
				{
					view.StartBuildingMinionParty(building.ID, (global::Kampai.Game.IMinionPartyBuilding)building, building.Location, minionPartyInstance.PartyType);
				}
				else
				{
					SetMinionPartyBuildingState(true);
				}
			}
		}

		private void HighlightBuilding(int buildingId, bool highlight)
		{
			view.HighlightBuilding(buildingId, highlight);
		}

		private void RecreateBuilding(global::Kampai.Game.Building building)
		{
			int iD = building.ID;
			view.RemoveBuilding(iD);
			InjectBuildingObject(view.CreateBuilding(building), building.Definition.ID);
		}

		private void RestoreBuildingView(global::Kampai.Game.Building building)
		{
			logger.Debug("Restoring building with id: {0}, type: {1}, state: {2}", building.ID, building.GetType(), building.State);
			if (building is global::Kampai.Game.ConnectableBuilding)
			{
				RestoreConnectableDeco(building);
			}
			else
			{
				InjectBuildingObject(view.CreateBuilding(building), building.Definition.ID);
			}
			global::Kampai.Game.BuildingState state = building.State;
			if ((state == global::Kampai.Game.BuildingState.Construction || state == global::Kampai.Game.BuildingState.Complete || state == global::Kampai.Game.BuildingState.Inactive) && (building.Definition.ConstructionTime > 0 || building is global::Kampai.Game.MasterPlanComponentBuilding))
			{
				DisplayBuilding(false, building.ID);
			}
		}

		private void RestoreConnectableDeco(global::Kampai.Game.Building building)
		{
			global::Kampai.Game.ConnectableBuilding connectableBuilding = building as global::Kampai.Game.ConnectableBuilding;
			global::Kampai.Game.Location location = building.Location;
			global::Kampai.Game.ConnectableBuildingDefinition connectableBuildingDefinition = building.Definition as global::Kampai.Game.ConnectableBuildingDefinition;
			decoGridModel.AddDeco(location.x, location.y, connectableBuildingDefinition.connectableType);
			global::UnityEngine.GameObject gameObject = view.CreateBuilding(building, (int)connectableBuilding.pieceType);
			global::Kampai.Game.View.BuildingObject component = gameObject.GetComponent<global::Kampai.Game.View.BuildingObject>();
			component.transform.localEulerAngles = new global::UnityEngine.Vector3(0f, connectableBuilding.rotation, 0f);
		}

		private void PrepareTaskingMinionForMinionParty()
		{
			MoveMinionInTaskingBuildingForParty(true);
		}

		private void RestoreTaskingMinionFromParty()
		{
			MoveMinionInTaskingBuildingForParty(false);
		}

		private void MoveMinionInTaskingBuildingForParty(bool moveOut)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.TaskableBuilding> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.TaskableBuilding>();
			foreach (global::Kampai.Game.TaskableBuilding item in instancesByType)
			{
				global::Kampai.Game.BuildingState state = item.State;
				global::System.Collections.Generic.IList<int> minionList = item.MinionList;
				if ((state != global::Kampai.Game.BuildingState.Working && state != global::Kampai.Game.BuildingState.HarvestableAndWorking && state != global::Kampai.Game.BuildingState.Harvestable) || minionList.Count <= 0)
				{
					continue;
				}
				if (moveOut)
				{
					view.PrepareTaskingMinionForMinionParty(item);
					if (item is global::Kampai.Game.DebrisBuilding)
					{
						cleanupDebrisSignal.Dispatch(item.ID, false);
						timeEventService.RemoveEvent(item.ID);
					}
					foreach (int item2 in minionList)
					{
						minionStateChange.Dispatch(item2, global::Kampai.Game.MinionState.Idle);
					}
					continue;
				}
				buildingChangeStateSignal.Dispatch(buildingID, global::Kampai.Game.BuildingState.Working);
				foreach (int item3 in minionList)
				{
					global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(item3);
					byInstanceId.IsInMinionParty = false;
					global::UnityEngine.GameObject gameObject = minionManager.GetComponent<global::Kampai.Game.View.MinionManagerView>().GetGameObject(item3);
					view.StartMinionTask(item, gameObject.GetComponent<global::Kampai.Game.View.MinionObject>(), false);
					minionStateChange.Dispatch(item3, global::Kampai.Game.MinionState.Tasking);
				}
			}
		}

		private void PurchaseNewBuilding(global::Kampai.Game.Building building)
		{
			DestroyDummyBuilding();
			global::Kampai.Game.BuildingDefinition definition = building.Definition;
			global::Kampai.Game.Location location = building.Location;
			bool flag = building is global::Kampai.Game.MasterPlanComponentBuilding;
			global::Kampai.Game.ConnectableBuilding connectableBuilding = building as global::Kampai.Game.ConnectableBuilding;
			int prefabIndex = 0;
			int outRotation = 0;
			if (connectableBuilding != null)
			{
				prefabIndex = InitializeConnectableBuilding(connectableBuilding, out outRotation);
			}
			global::UnityEngine.Vector3 position = new global::UnityEngine.Vector3(location.x, 0f, location.y);
			int iD = building.ID;
			global::UnityEngine.GameObject gameObject = view.CreateBuilding(building, prefabIndex);
			global::Kampai.Game.View.BuildingObject component = gameObject.GetComponent<global::Kampai.Game.View.BuildingObject>();
			if (connectableBuilding != null)
			{
				component.transform.localEulerAngles = new global::UnityEngine.Vector3(0f, outRotation, 0f);
			}
			component.ID = iD;
			InjectBuildingObject(gameObject, definition.ID);
			if (definition.ConstructionTime > 0 || flag)
			{
				DisplayBuilding(false, iD);
				if (!flag)
				{
					view.CreatePlatformBuildingObject(building, position);
				}
				global::Kampai.Game.View.ScaffoldingBuildingObject scaffoldingBuildingObject = view.CreateScaffoldingBuildingObject(building, position);
				if (masterPlanService.CurrentMasterPlan != null && definition.ID == masterPlanService.CurrentMasterPlan.Definition.BuildingDefID)
				{
					playLocalAudioSignal.Dispatch(global::Kampai.Util.Audio.GetAudioEmitter.Get(scaffoldingBuildingObject.gameObject, "partile"), "Play_crate_sparkle_01", null);
				}
				GrowScaffolding(component, true);
				if (definition.Type != BuildingType.BuildingTypeIdentifier.MASTER_COMPONENT)
				{
					TriggerVFX(position, "FX_Drop_Prefab", definition);
				}
			}
			else
			{
				GrowScaffolding(component, false);
			}
		}

		private int InitializeConnectableBuilding(global::Kampai.Game.ConnectableBuilding building, out int outRotation)
		{
			global::Kampai.Game.Location location = building.Location;
			global::Kampai.Game.ConnectableBuildingDefinition definition = building.Definition;
			global::Kampai.Game.ConnectableBuildingPieceType connectablePieceType = decoGridModel.GetConnectablePieceType(location.x, location.y, definition.connectableType, out outRotation);
			building.pieceType = connectablePieceType;
			building.rotation = outRotation;
			decoGridModel.AddDeco(location.x, location.y, definition.connectableType);
			updateConnectablesSignal.Dispatch(location, definition.connectableType);
			return (int)connectablePieceType;
		}

		private void GrowScaffolding(global::Kampai.Game.View.BuildingObject building, bool isScaffoldingPrefab)
		{
			if (building != null)
			{
				if (isScaffoldingPrefab)
				{
					PlaySFX(building.ID, "Play_scaffold_construction_01", true);
				}
				else
				{
					PlaySFX(building.ID, "Play_prop_land_01", true);
				}
				if (!(building is global::Kampai.Game.View.ConnectableBuildingObject))
				{
					global::UnityEngine.Vector3 center = building.Center;
					global::Kampai.Util.Boxed<global::UnityEngine.Vector3> type = new global::Kampai.Util.Boxed<global::UnityEngine.Vector3>(new global::UnityEngine.Vector3(center.x, 0f, center.z));
					buildingReactionSignal.Dispatch(type, true);
				}
				startConstructionSignal.Dispatch(building.ID, false);
			}
		}

		private void SetBuildingRushed(int buildingId)
		{
			view.SetBuildingRushed(buildingId);
		}

		private void RushRevealBuilding(int buildingId)
		{
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(buildingId);
			if (byInstanceId != null)
			{
				RevealBuilding(byInstanceId);
			}
		}

		private void OnRevealBuilding(global::Kampai.Game.Building building)
		{
			RevealBuilding(building);
		}

		private void PlaySFX(int buildingId, string sfxName, bool enable)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				if (enable)
				{
					stopLocalAudioSignal.Dispatch(buildingObject.localAudioEmitter);
					playLocalAudioSignal.Dispatch(buildingObject.localAudioEmitter, sfxName, null);
				}
				else
				{
					stopLocalAudioSignal.Dispatch(buildingObject.localAudioEmitter);
				}
			}
		}

		private void RestoreScaffoldingView(global::Kampai.Game.Building building, bool restoreTimer)
		{
			if (building.Definition.ConstructionTime <= 0 && !(building is global::Kampai.Game.MasterPlanComponentBuilding))
			{
				return;
			}
			logger.Debug("Restoring scaffolding for building id: {0} type: {1}", building.ID, building.GetType());
			view.CreateScaffoldingBuildingObject(building, new global::UnityEngine.Vector3(building.Location.x, 0f, building.Location.y));
			if (restoreTimer)
			{
				StartCoroutine(WaitAFrame(delegate
				{
					startConstructionSignal.Dispatch(building.ID, true);
				}));
			}
		}

		private void RestorePlatformView(global::Kampai.Game.Building building)
		{
			if (building.Definition.ConstructionTime > 0)
			{
				logger.Debug("Restoring platform for building id: {0} type: {1}", building.ID, building.GetType());
				view.CreatePlatformBuildingObject(building, new global::UnityEngine.Vector3(building.Location.x, 0f, building.Location.y));
			}
		}

		private void RestoreRibbonView(global::Kampai.Game.Building building)
		{
			if (building.Definition.ConstructionTime > 0)
			{
				logger.Debug("Restoring ribbon for building id: {0} type: {1}", building.ID, building.GetType());
				view.CreateRibbonBuildingObject(building, new global::UnityEngine.Vector3(building.Location.x, 0f, building.Location.y));
				PlaceWayFinderOnBuilding(building);
			}
		}

		private void CreateInventoryBuilding(global::Kampai.Game.Building building, global::Kampai.Game.Location location)
		{
			DestroyDummyBuilding();
			global::UnityEngine.GameObject gameObject = view.CreateBuilding(building);
			InjectBuildingObject(gameObject, building.Definition.ID);
			gameObject.transform.position = new global::UnityEngine.Vector3(location.x, 0f, location.y);
			global::Kampai.Game.View.BuildingObject component = gameObject.GetComponent<global::Kampai.Game.View.BuildingObject>();
			if (component != null && !(component is global::Kampai.Game.View.ConnectableBuildingObject))
			{
				global::UnityEngine.Vector3 center = component.Center;
				global::Kampai.Util.Boxed<global::UnityEngine.Vector3> type = new global::Kampai.Util.Boxed<global::UnityEngine.Vector3>(new global::UnityEngine.Vector3(center.x, 0f, center.z));
				buildingReactionSignal.Dispatch(type, true);
			}
			if (model.CurrentMode != global::Kampai.Common.PickControllerModel.Mode.DragAndDrop)
			{
				model.SelectedBuilding = null;
			}
		}

		private void RevealBuilding(global::Kampai.Game.Building building)
		{
			global::Kampai.Game.BuildingState state = building.State;
			int iD = building.ID;
			if (state != global::Kampai.Game.BuildingState.Complete)
			{
				logger.Info("Can't reveal building id:{0} when construction is not complete!", iD);
				return;
			}
			CheckForMasterPlan(building);
			global::Kampai.Game.Location location = building.Location;
			buildingChangeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Idle);
			RemoveWayFinderFromBuilding(building);
			view.RemoveAllScaffoldingParts(iD);
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(iD);
			DisplayBuilding(true, buildingObject.ID);
			global::UnityEngine.Vector3 center = buildingObject.Center;
			AnimateRevealBuilding(buildingObject);
			global::Kampai.Game.BuildingDefinition definition = building.Definition;
			string prefabName = ((!string.IsNullOrEmpty(definition.RevealVFX)) ? definition.RevealVFX : "FX_Reveal_Prefab");
			TriggerVFX(new global::UnityEngine.Vector3(center.x, 0f, center.z), prefabName, definition);
			if (definition.RewardTransactionId != 0)
			{
				global::Kampai.Game.TransactionArg transactionArg = new global::Kampai.Game.TransactionArg();
				transactionArg.Source = definition.TaxonomyType;
				playerService.RunEntireTransaction(definition.RewardTransactionId, global::Kampai.Game.TransactionTarget.REWARD_BUILDING, null, transactionArg);
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.SpawnDooberSignal>().Dispatch(center, global::Kampai.UI.View.DestinationType.XP, -1, true);
			}
			global::Kampai.Util.Boxed<global::UnityEngine.Vector3> type = new global::Kampai.Util.Boxed<global::UnityEngine.Vector3>(new global::UnityEngine.Vector3(center.x, 0f, center.z));
			buildingReactionSignal.Dispatch(type, true);
			globalAudioSignal.Dispatch("Play_building_repair_01");
			if (!(building is global::Kampai.Game.MasterPlanComponentBuilding))
			{
				addFootprintSignal.Dispatch(building, location);
			}
			else
			{
				DispatchComponentSignals(definition);
			}
			ShowRevealVFX(definition, iD);
			questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.Construction);
			if (definition.PlayerTrainingDefinitionID > 0)
			{
				playerTrainingSignal.Dispatch(definition.PlayerTrainingDefinitionID, false, new global::strange.extensions.signal.impl.Signal<bool>());
			}
		}

		private void DispatchComponentSignals(global::Kampai.Game.BuildingDefinition def)
		{
			revealMasterPlanComponentSignal.Dispatch(def.ID);
			displayVolcanoWayfinderSignal.Dispatch();
			hideWayfinder.Dispatch(false);
			setCompleteWayfinderSignal.Dispatch();
		}

		private void ShowRevealVFX(global::Kampai.Game.BuildingDefinition def, int buildingId)
		{
			if (masterPlanService.CurrentMasterPlan != null && def.ID == masterPlanService.CurrentMasterPlan.Definition.BuildingDefID)
			{
				PlaySFX(buildingId, "Play_villainLair_scaffoldReveal_01", true);
			}
			else
			{
				PlaySFX(buildingId, "Play_scaffold_disappear_01", true);
			}
		}

		private void CheckForMasterPlan(global::Kampai.Game.Building building)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.MasterPlanDefinition> all = definitionService.GetAll<global::Kampai.Game.MasterPlanDefinition>();
			for (int i = 0; i < all.Count; i++)
			{
				if (all[i].BuildingDefID == building.Definition.ID)
				{
					masterPlanCompleteSignal.Dispatch(all[i]);
					break;
				}
			}
		}

		private void AnimateRevealBuilding(global::Kampai.Game.View.BuildingObject buildingObject)
		{
			buildingObject.transform.localScale = new global::UnityEngine.Vector3(0.01f, 0.01f, 0.01f);
			Go.to(buildingObject.transform, 0.5f, new GoTweenConfig().scale(new global::UnityEngine.Vector3(1f, 1f, 1f)).setEaseType(GoEaseType.BackOut).onComplete(delegate(AbstractGoTween tween)
			{
				tween.destroy();
			})).play();
		}

		private void RemoveWayFinderFromBuilding(global::Kampai.Game.Building building)
		{
			removeWayFinderSignal.Dispatch(building.ID);
		}

		private void SelectBuilding(int buildingId, string footprint)
		{
			view.SelectBuilding(buildingId);
			ToggleTaskableMinions(buildingId, false);
			globalAudioSignal.Dispatch("Play_building select_01");
			disableCameraSignal.Dispatch(1);
			enableCameraSignal.Dispatch(8);
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(buildingId);
			if (!(buildingObject == null))
			{
				showBuildingFootprintSignal.Dispatch(buildingObject, buildingObject.transform, global::Kampai.Util.Tuple.Create(buildingObject.Width, buildingObject.Depth), true);
				global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(buildingId);
				GetUISignal<global::Kampai.UI.View.ShowActivitySpinnerSignal>().Dispatch(false, global::UnityEngine.Vector3.zero);
				model.activitySpinnerExists = false;
				if (!byInstanceId.Definition.Storable || !allowStorable)
				{
					GetUISignal<global::Kampai.UI.View.ShowMoveBuildingMenuSignal>().Dispatch(true, new global::Kampai.UI.View.MoveBuildingSetting(byInstanceId.ID, 1, false, false));
				}
				else
				{
					GetUISignal<global::Kampai.UI.View.ShowMoveBuildingMenuSignal>().Dispatch(true, new global::Kampai.UI.View.MoveBuildingSetting(byInstanceId.ID, 16, false, false));
				}
				GetUISignal<global::Kampai.UI.View.ShowHUDSignal>().Dispatch(false);
				GetUISignal<global::Kampai.UI.View.ShowWorldCanvasSignal>().Dispatch(false);
				GetUISignal<global::Kampai.UI.View.HideAllResourceIconsSignal>().Dispatch();
				GetUISignal<global::Kampai.UI.View.ToggleAllFloatingTextSignal>().Dispatch(false);
				enableVillainIslandCollidersSignal.Dispatch(false);
			}
		}

		private void DeselectBuilding(int buildingId)
		{
			view.DeselectBuilding(buildingId);
			ToggleTaskableMinions(buildingId, true);
			disableCameraSignal.Dispatch(8);
			enableCameraSignal.Dispatch(1);
			HideFootprint();
			global::Kampai.Game.BuildingDefinition definition = currentScaffolding.Definition;
			if (buildingId == -1 && definition != null)
			{
				global::Kampai.Game.DecorationBuildingDefinition decorationBuildingDefinition = definition as global::Kampai.Game.DecorationBuildingDefinition;
				bool flag = playerService.CheckIfBuildingIsCapped(definition.ID);
				bool flag2 = decorationBuildingDefinition != null && decorationBuildingDefinition.AutoPlace && !flag;
				if (definition.Type != BuildingType.BuildingTypeIdentifier.CONNECTABLE && !flag2)
				{
					HideBuildingPlacementMenu(buildingId);
				}
			}
			else
			{
				HideBuildingPlacementMenu(buildingId);
			}
			enableVillainIslandCollidersSignal.Dispatch(true);
		}

		private void ToggleTaskableMinions(int buildingID, bool enableRenderers)
		{
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(buildingID);
			global::Kampai.Game.TaskableBuilding taskableBuilding = byInstanceId as global::Kampai.Game.TaskableBuilding;
			global::Kampai.Game.LeisureBuilding leisureBuilding = byInstanceId as global::Kampai.Game.LeisureBuilding;
			global::System.Collections.Generic.IList<int> list3;
			if (taskableBuilding == null)
			{
				global::System.Collections.Generic.IList<int> list2;
				global::System.Collections.Generic.IList<int> list;
				if (leisureBuilding == null)
				{
					list = null;
					list2 = list;
				}
				else
				{
					list2 = leisureBuilding.MinionList;
				}
				list = list2;
				list3 = list;
			}
			else
			{
				list3 = taskableBuilding.MinionList;
			}
			global::System.Collections.Generic.IList<int> list4 = list3;
			if (list4 == null)
			{
				return;
			}
			foreach (int item in list4)
			{
				toggleMinionSignal.Dispatch(item, enableRenderers);
			}
			if (enableRenderers)
			{
				relocateMinionsSignal.Dispatch(byInstanceId);
			}
		}

		private void DisplayBuilding(bool isVisible, int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				buildingObject.gameObject.SetActive(isVisible);
			}
		}

		private void MoveBuilding(int buildingId, global::UnityEngine.Vector3 position, bool isValidPosition)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				PlayAudioOnMove(buildingObject.transform.position, position, isValidPosition);
			}
			view.MoveBuilding(buildingId, position, isValidPosition);
		}

		private void MoveDummyBuildingObject(global::UnityEngine.Vector3 position, bool isValidPosition)
		{
			if (currentDummyBuilding != null)
			{
				position = new global::UnityEngine.Vector3(global::UnityEngine.Mathf.Round(position.x), currentDummyBuilding.transform.position.y, global::UnityEngine.Mathf.Round(position.z));
				PlayAudioOnMove(currentDummyBuilding.transform.position, position, isValidPosition);
				currentDummyBuilding.transform.position = position;
				currentDummyBuilding.SetBlendedColor((!isValidPosition) ? global::Kampai.Util.GameConstants.Building.INVALID_PLACEMENT_COLOR : global::Kampai.Util.GameConstants.Building.VALID_PLACEMENT_COLOR);
			}
		}

		private void PlayAudioOnMove(global::UnityEngine.Vector3 oldPosition, global::UnityEngine.Vector3 newPosition, bool isValidPosition)
		{
			global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(global::UnityEngine.Mathf.Round(oldPosition.x), oldPosition.y, global::UnityEngine.Mathf.Round(oldPosition.z));
			global::UnityEngine.Vector3 vector2 = new global::UnityEngine.Vector3(global::UnityEngine.Mathf.Round(newPosition.x), newPosition.y, global::UnityEngine.Mathf.Round(newPosition.z));
			if (vector2 != vector)
			{
				if (isValidPosition)
				{
					globalAudioSignal.Dispatch("Play_click_snap_01");
				}
				else
				{
					globalAudioSignal.Dispatch("Play_error_button_01");
				}
			}
		}

		private void StartMinionTask(global::Kampai.Game.View.MinionObject minion, global::Kampai.Game.Building building)
		{
			int iD = building.ID;
			global::Kampai.Game.TaskableBuilding taskableBuilding = building as global::Kampai.Game.TaskableBuilding;
			if (taskableBuilding != null)
			{
				bool alreadyRushed = minion.GetMinion().AlreadyRushed;
				buildingChangeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Working);
				view.StartMinionTask(taskableBuilding, minion, alreadyRushed);
				if (taskableBuilding is global::Kampai.Game.TikiBarBuilding)
				{
					global::Kampai.Game.Prestige prestigeFromMinionInstance = characterService.GetPrestigeFromMinionInstance(minion.GetMinion());
					characterService.ChangeToPrestigeState(prestigeFromMinionInstance, global::Kampai.Game.PrestigeState.Questing);
					return;
				}
				global::Kampai.Game.MignetteBuilding mignetteBuilding = taskableBuilding as global::Kampai.Game.MignetteBuilding;
				if (mignetteBuilding == null)
				{
					PlaceMinionInBuilding(taskableBuilding, minion.GetMinion());
				}
				if (alreadyRushed)
				{
					return;
				}
				if (mignetteBuilding != null)
				{
					if (!mignetteBuilding.AreAllMinionSlotsFilled())
					{
						return;
					}
					global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(iD);
					if (buildingObject != null)
					{
						global::Kampai.Game.View.MignetteBuildingObject mignetteBuildingObject = buildingObject as global::Kampai.Game.View.MignetteBuildingObject;
						if (mignetteBuildingObject != null && mignetteBuildingObject.GetMignetteMinionCount() == mignetteBuilding.GetMinionSlotsOwned())
						{
							MoveCameraAndFocusOnBuilding(mignetteBuilding, true);
						}
					}
				}
				else if (taskableBuilding is global::Kampai.Game.DebrisBuilding)
				{
					cleanupDebrisSignal.Dispatch(iD, true);
					view.AppendMinionTaskAnimationCompleteCallback(minion, minionTaskingAnimationDone);
				}
				else
				{
					if (!taskableBuilding.IsEligibleForGag())
					{
						return;
					}
					int num = iD;
					int nextGagPlayTime = taskableBuilding.GetNextGagPlayTime(timeService.CurrentTime());
					if (timeEventService.GetEventDuration(num) == 0)
					{
						if (nextGagPlayTime > 0)
						{
							timeEventService.AddEvent(num, timeService.CurrentTime(), nextGagPlayTime, triggerGagAnimation);
						}
						else
						{
							TriggerGagAnimation(num);
						}
					}
				}
			}
			else
			{
				logger.Fatal(global::Kampai.Util.FatalCode.BV_NOT_A_TASKING_BUILDING);
			}
		}

		private void PlaceMinionInBuilding(global::Kampai.Game.TaskableBuilding taskableBuilding, global::Kampai.Game.Minion minion)
		{
			global::System.Collections.Generic.IList<int> minionList = taskableBuilding.MinionList;
			if (minionList.Count <= 1)
			{
				return;
			}
			minionList.Remove(minion.ID);
			bool flag = false;
			for (int i = 0; i < minionList.Count; i++)
			{
				int id = minionList[i];
				global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(id);
				if (byInstanceId.UTCTaskStartTime > minion.UTCTaskStartTime)
				{
					minionList.Insert(i, minion.ID);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				minionList.Add(minion.ID);
			}
		}

		private void TriggerGagAnimation(int buildingId)
		{
			global::Kampai.Game.TaskableBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.TaskableBuilding>(buildingId);
			int utcTime = timeService.CurrentTime();
			if (byInstanceId.IsEligibleForGag() && byInstanceId.GetNextGagPlayTime(utcTime) == 0 && view.TriggerGagAnimation(buildingId))
			{
				byInstanceId.GagPlayed(utcTime);
				if (byInstanceId.IsEligibleForGag())
				{
					timeEventService.AddEvent(buildingId, timeService.CurrentTime(), byInstanceId.GetNextGagPlayTime(utcTime), triggerGagAnimation);
				}
			}
		}

		private void MoveCameraAndFocusOnBuilding(global::Kampai.Game.TaskableBuilding building, bool showModalOnArrive = false)
		{
			showHiddenBuildingsSignal.Dispatch();
			global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(building.ID);
			global::UnityEngine.Vector3 center = buildingObject.Center;
			global::Kampai.Game.View.MignetteBuildingObject mignetteBuildingObject = buildingObject as global::Kampai.Game.View.MignetteBuildingObject;
			AlligatorSkiingBuildingViewObject alligatorSkiingBuildingViewObject = null;
			if (mignetteBuildingObject != null)
			{
				alligatorSkiingBuildingViewObject = mignetteBuildingObject.GetComponent<AlligatorSkiingBuildingViewObject>();
			}
			global::Kampai.Game.ScreenPosition screenPosition = new global::Kampai.Game.ScreenPosition();
			screenPosition = screenPosition.Clone(building.Definition.ScreenPosition);
			if (alligatorSkiingBuildingViewObject != null)
			{
				screenPosition.x = 0.5f;
				screenPosition.z = 0.5f;
				screenPosition.zoom = 0.484f;
			}
			if (screenPosition == null || !(center != global::UnityEngine.Vector3.zero))
			{
				return;
			}
			int num = building.Definition.GagID;
			if (num == 0)
			{
				num = -1;
			}
			if (num != -1)
			{
				if (playerService.GetByDefinitionId<global::Kampai.Game.Item>(num).Count > 0)
				{
					return;
				}
				global::Kampai.Game.Item i = new global::Kampai.Game.Item(definitionService.Get<global::Kampai.Game.ItemDefinition>(num));
				playerService.Add(i);
			}
			global::Kampai.Game.CameraMovementSettings.Settings settings = (showModalOnArrive ? global::Kampai.Game.CameraMovementSettings.Settings.ShowMenu : global::Kampai.Game.CameraMovementSettings.Settings.None);
			autoMoveSignal.Dispatch(center, new global::Kampai.Util.Boxed<global::Kampai.Game.ScreenPosition>(screenPosition), new global::Kampai.Game.CameraMovementSettings(settings, building, null), false);
		}

		private void StopGagAnimation(int buildingId)
		{
			if (view.IsGagAnimationPlaying(buildingId))
			{
				view.StopGagAnimation(buildingId);
			}
		}

		private void ShowHarvestReady(global::Kampai.Util.Tuple<int, int> ids)
		{
			view.HarvestReady(ids.Item1, ids.Item2);
		}

		private void EjectAllMinionsFromBuilding(int buildingID)
		{
			global::Kampai.Game.TaskableBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.TaskableBuilding>(buildingID);
			if (byInstanceId != null)
			{
				int minionsInBuilding = byInstanceId.GetMinionsInBuilding();
				for (int i = 0; i < minionsInBuilding; i++)
				{
					int minionByIndex = byInstanceId.GetMinionByIndex(0);
					EjectMinionFromBuilding(byInstanceId, minionByIndex);
				}
			}
		}

		private void HarvestComplete(int buildingID)
		{
			global::Kampai.Game.TaskableBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.TaskableBuilding>(buildingID);
			if (byInstanceId != null)
			{
				global::Kampai.Game.ResourceBuilding resourceBuilding = byInstanceId as global::Kampai.Game.ResourceBuilding;
				if (resourceBuilding != null)
				{
					resourceBuilding.CompleteHarvest();
					return;
				}
				int minionID = byInstanceId.HarvestFromCompleteMinions();
				EjectMinionFromBuilding(byInstanceId, minionID);
			}
		}

		private void EjectMinionFromBuilding(global::Kampai.Game.TaskableBuilding taskableBuilding, int minionID)
		{
			int iD = taskableBuilding.ID;
			view.UntrackMinion(iD, minionID, taskableBuilding);
			playerService.StopTask(minionID);
			stopTaskSignal.Dispatch(minionID);
			if (taskableBuilding is global::Kampai.Game.ResourceBuilding)
			{
				brbExitAnimimationSignal.Dispatch(minionID);
			}
			global::Kampai.Game.View.MinionManagerView component = minionManager.GetComponent<global::Kampai.Game.View.MinionManagerView>();
			global::Kampai.Game.View.MinionObject minionObject = component.Get(minionID);
			minionObject.gameObject.SetLayerRecursively(8);
			if (minionObject.currentAction is global::Kampai.Game.View.ConstantSpeedPathAction || minionObject.currentAction is global::Kampai.Game.View.RotateAction || minionObject.currentAction is global::Kampai.Game.View.SetAnimatorAction)
			{
				minionObject.ClearActionQueue();
				minionStateChange.Dispatch(minionID, global::Kampai.Game.MinionState.Idle);
			}
			else
			{
				relocateCharacterSignal.Dispatch(minionObject);
			}
			if (taskableBuilding.GetNumCompleteMinions() != 0 || taskableBuilding.Definition.Type == BuildingType.BuildingTypeIdentifier.RESOURCE)
			{
				return;
			}
			if (taskableBuilding.GetMinionsInBuilding() == 0)
			{
				if (taskableBuilding.Definition is global::Kampai.Game.MignetteBuildingDefinition)
				{
					buildingChangeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Cooldown);
				}
				else
				{
					buildingChangeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Idle);
				}
			}
			else
			{
				buildingChangeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Working);
			}
		}

		private void UpdateViewFromBuildingState(int buildingId, global::Kampai.Game.BuildingState buildingState)
		{
			view.UpdateBuildingState(buildingId, buildingState);
			if (buildingState != global::Kampai.Game.BuildingState.Complete)
			{
				return;
			}
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(buildingId);
			if (byInstanceId.Definition.ConstructionTime > 0)
			{
				global::Kampai.Game.Location location = byInstanceId.Location;
				global::UnityEngine.Vector3 position = new global::UnityEngine.Vector3(location.x, 0f, location.y);
				if (!view.IsBuildingRushed(buildingId))
				{
					PlaySFX(buildingId, null, false);
					view.CreateRibbonBuildingObject(byInstanceId, position);
					TriggerVFX(position, "FX_Bow_Prefab", byInstanceId.Definition);
					PlaceWayFinderOnBuilding(byInstanceId);
				}
			}
		}

		private void AdjustVFXPosition(global::Kampai.Game.BuildingDefinition buildingDef, global::UnityEngine.Transform transform)
		{
			if (view.Is8x8Building(buildingDef))
			{
				transform.localPosition += new global::UnityEngine.Vector3(1f, 0f, -1f);
			}
		}

		private void TriggerVFX(global::UnityEngine.Vector3 position, string prefabName, global::Kampai.Game.BuildingDefinition buildingDef)
		{
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(prefabName);
			if (!(gameObject != null))
			{
				return;
			}
			global::UnityEngine.GameObject vfxInstance = global::UnityEngine.Object.Instantiate(gameObject);
			global::UnityEngine.Transform transform = vfxInstance.transform;
			transform.position = position;
			AdjustVFXPosition(buildingDef, transform);
			float num = 0f;
			foreach (global::UnityEngine.Transform item in transform)
			{
				global::UnityEngine.ParticleSystem component = item.GetComponent<global::UnityEngine.ParticleSystem>();
				if (component != null && component.duration > num)
				{
					num = component.duration;
				}
				component.Play();
			}
			StartCoroutine(WaitSomeTime(num, delegate
			{
				global::UnityEngine.Object.Destroy(vfxInstance);
			}));
		}

		private void PlaceWayFinderOnBuilding(global::Kampai.Game.Building building)
		{
			global::Kampai.UI.View.WayFinderSettings type = new global::Kampai.UI.View.WayFinderSettings(building.ID);
			createWayFinderSignal.Dispatch(type);
		}

		private void UpdateTaskedMinions(int minionID, global::Kampai.Game.View.MinionTaskInfo taskInfo)
		{
			updateTaskedMinionSignal.Dispatch(minionID, taskInfo);
		}

		private void CreateDummyBuilding(global::Kampai.Game.BuildingDefinition buildingDefinition, global::UnityEngine.Vector3 position, bool isInInventory)
		{
			buildingID = buildingDefinition.ID;
			currentDummyBuilding = view.CreateDummyBuilding(buildingDefinition, position);
			if (currentScaffolding != null && currentScaffolding.Building != null)
			{
				currentDummyBuilding.IsRotated = currentScaffolding.Building.IsRotated;
				if (currentDummyBuilding.IsRotated)
				{
					currentDummyBuilding.transform.localEulerAngles = new global::UnityEngine.Vector3(0f, 90f, 0f);
					currentDummyBuilding.transform.localScale = new global::UnityEngine.Vector3(1f, 1f, -1f);
				}
			}
			// Always call SetRotated to ensure BroadFootprint is updated even for new purchases
			currentDummyBuilding.SetRotated(currentDummyBuilding.IsRotated, definitionService);
			showBuildingFootprintSignal.Dispatch(currentDummyBuilding, currentDummyBuilding.transform, global::Kampai.Util.Tuple.Create(currentDummyBuilding.Width, currentDummyBuilding.Depth), true);
			showHUDSignal.Dispatch(false);
			hideAllWayFindersSignal.Dispatch();
		}

		private void RotateBuilding()
		{
			if (currentDummyBuilding != null)
			{
				currentDummyBuilding.IsRotated = !currentDummyBuilding.IsRotated;
				if (currentDummyBuilding.IsRotated)
				{
					currentDummyBuilding.transform.localEulerAngles = new global::UnityEngine.Vector3(0f, 90f, 0f);
					currentDummyBuilding.transform.localScale = new global::UnityEngine.Vector3(1f, 1f, -1f);
				}
				else
				{
					currentDummyBuilding.transform.localEulerAngles = global::UnityEngine.Vector3.zero;
					currentDummyBuilding.transform.localScale = global::UnityEngine.Vector3.one;
				}
				currentDummyBuilding.SetRotated(currentDummyBuilding.IsRotated, definitionService);
				showBuildingFootprintSignal.Dispatch(currentDummyBuilding, currentDummyBuilding.transform, global::Kampai.Util.Tuple.Create(currentDummyBuilding.Width, currentDummyBuilding.Depth), true);
				if (currentScaffolding != null && currentScaffolding.Building != null)
				{
					currentScaffolding.Building.IsRotated = currentDummyBuilding.IsRotated;
				}
			}
			else if (model.SelectedBuilding.HasValue && model.SelectedBuilding.Value != -1)
			{
				int value = model.SelectedBuilding.Value;
				global::Kampai.Game.View.BuildingObject buildingObject = view.GetBuildingObject(value);
				if (buildingObject != null)
				{
					global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(value);
					if (byInstanceId != null)
					{
						byInstanceId.IsRotated = !byInstanceId.IsRotated;
						buildingObject.IsRotated = byInstanceId.IsRotated;
						if (byInstanceId.IsRotated)
						{
							buildingObject.transform.localEulerAngles = new global::UnityEngine.Vector3(0f, 90f, 0f);
							buildingObject.transform.localScale = new global::UnityEngine.Vector3(1f, 1f, -1f);
						}
						else
						{
							buildingObject.transform.localEulerAngles = global::UnityEngine.Vector3.zero;
							buildingObject.transform.localScale = global::UnityEngine.Vector3.one;
						}
						buildingObject.SetRotated(byInstanceId.IsRotated, definitionService);
						showBuildingFootprintSignal.Dispatch(buildingObject, buildingObject.transform, global::Kampai.Util.Tuple.Create(buildingObject.Width, buildingObject.Depth), true);
					}
				}
			}
		}

		public global::Kampai.Game.View.DummyBuildingObject GetCurrentDummyBuilding()
		{
			return currentDummyBuilding;
		}

		private void SetBuildingPosition(int buildingId, global::UnityEngine.Vector3 position)
		{
			view.SetBuildingPosition(buildingId, position);
		}

		private void CancelPurchaseStart(bool invalidLocation)
		{
			if (currentDummyBuilding != null)
			{
				HideFootprint();
				openStoreSignal.Dispatch(buildingID, true);
				global::UnityEngine.Vector3 destination = BuildingUtil.UIToWorldCoords(global::UnityEngine.Camera.main, model.LastBuildingStorePosition);
				view.TweenBuildingToMenu(currentDummyBuilding.gameObject, destination, CancelPurchaseEnd);
				if (invalidLocation)
				{
					string type = localService.GetString("InvalidLocation");
					popupMessageSignal.Dispatch(type, global::Kampai.UI.View.PopupMessageType.NORMAL);
				}
			}
			GetUISignal<global::Kampai.UI.View.ShowMoveBuildingMenuSignal>().Dispatch(false, new global::Kampai.UI.View.MoveBuildingSetting(-1, 0, false, false));
			showHUDSignal.Dispatch(true);
			showAllWayFindersSignal.Dispatch();
		}

		private void CancelPurchaseEnd(global::UnityEngine.GameObject lastScaffolding)
		{
			global::UnityEngine.Object.Destroy(lastScaffolding);
			lastScaffolding = null;
		}

		private void SendToInventory(int buildingId)
		{
			HideFootprint();
			global::Kampai.Game.ConnectableBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.ConnectableBuilding>(buildingId);
			if (byInstanceId != null)
			{
				global::Kampai.Game.ConnectableBuildingDefinition definition = byInstanceId.Definition;
				global::Kampai.Game.Location location = byInstanceId.Location;
				decoGridModel.RemoveDeco(location.x, location.y);
				updateConnectablesSignal.Dispatch(location, definition.connectableType);
			}
			view.ToInventory(buildingId);
			HideBuildingPlacementMenu(buildingId);
		}

		private void HideBuildingPlacementMenu(int buildingId)
		{
			GetUISignal<global::Kampai.UI.View.ShowMoveBuildingMenuSignal>().Dispatch(false, new global::Kampai.UI.View.MoveBuildingSetting(buildingId, 0, false, false));
			GetUISignal<global::Kampai.UI.View.ShowHUDSignal>().Dispatch(true);
			GetUISignal<global::Kampai.UI.View.ShowStoreSignal>().Dispatch(true);
			GetUISignal<global::Kampai.UI.View.ShowWorldCanvasSignal>().Dispatch(true);
			GetUISignal<global::Kampai.UI.View.ShowAllResourceIconsSignal>().Dispatch();
			GetUISignal<global::Kampai.UI.View.ToggleAllFloatingTextSignal>().Dispatch(true);
		}

		private void HideFootprint()
		{
			showBuildingFootprintSignal.Dispatch(null, null, global::Kampai.Util.Tuple.Create(1, 1), false);
		}

		private T GetUISignal<T>()
		{
			return uiContext.injectionBinder.GetInstance<T>();
		}

		private global::System.Collections.IEnumerator WaitAFrame(global::System.Action a)
		{
			yield return null;
			a();
		}

		private global::System.Collections.IEnumerator WaitSomeTime(float delayTime, global::System.Action a)
		{
			yield return new global::UnityEngine.WaitForSeconds(delayTime);
			a();
		}

		private void OnMinionTaskingAnimationDone(int minionId)
		{
			global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(minionId);
			if (byInstanceId != null)
			{
				int num = byInstanceId.BuildingID;
				global::Kampai.Game.TaskableBuilding byInstanceId2 = playerService.GetByInstanceId<global::Kampai.Game.TaskableBuilding>(num);
				if (byInstanceId2 is global::Kampai.Game.DebrisBuilding)
				{
					minionTaskCompleteSignal.Dispatch(minionId);
					cleanupDebrisSignal.Dispatch(num, false);
				}
			}
		}

		private void OnBurnedLandExpansion(int buildingId)
		{
			global::Kampai.Game.LandExpansionBuilding buildingByInstanceID = landExpansionService.GetBuildingByInstanceID(buildingId);
			view.RemoveBuilding(buildingByInstanceID.ID);
		}

		private void VerifyResourceBuildingSlots(global::Kampai.Game.Building building)
		{
			global::Kampai.Game.ResourceBuilding resourceBuilding = building as global::Kampai.Game.ResourceBuilding;
			int num = 0;
			int quantity = (int)playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID);
			SetBuildingNumber(building);
			int num2 = resourceBuilding.BuildingNumber - 1;
			int count = resourceBuilding.Definition.SlotUnlocks.Count;
			num2 = ((num2 <= count - 1) ? num2 : (count - 1));
			foreach (int slotUnlockLevel in resourceBuilding.Definition.SlotUnlocks[num2].SlotUnlockLevels)
			{
				if (slotUnlockLevel <= quantity)
				{
					num++;
				}
			}
			if (num > resourceBuilding.MinionSlotsOwned)
			{
				resourceBuilding.MinionSlotsOwned = num;
			}
		}

		private void SetBuildingNumber(global::Kampai.Game.Building building)
		{
			if (building.BuildingNumber == 0)
			{
				building.BuildingNumber = playerService.GetInstancesByDefinitionID(building.Definition.ID).Count;
			}
		}

		private void TryHarvest(int buildingID, global::System.Action callback, bool fromUI)
		{
			harvestBuildingSignal.Dispatch(view.GetBuildingObject(buildingID), callback, fromUI);
		}

		private void PreLoadMinionPartyBuildings()
		{
			global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
			global::System.Collections.Generic.ICollection<global::Kampai.Game.MinionPartyBuilding> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.MinionPartyBuilding>();
			foreach (global::Kampai.Game.MinionPartyBuilding item in instancesByType)
			{
				if (item.IsBuildingRepaired())
				{
					view.PreloadBuildingMinionParty(item.ID, item, item.Location, minionPartyInstance.PartyType);
				}
			}
		}

		private void SetMinionPartyBuildingState(bool enabled)
		{
			global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
			global::System.Collections.Generic.ICollection<global::Kampai.Game.IMinionPartyBuilding> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.IMinionPartyBuilding>();
			if (enabled)
			{
				foreach (global::Kampai.Game.IMinionPartyBuilding item in instancesByType)
				{
					global::Kampai.Game.RepairableBuilding repairableBuilding = (global::Kampai.Game.RepairableBuilding)item;
					if (repairableBuilding.IsBuildingRepaired())
					{
						view.StartBuildingMinionParty(repairableBuilding.ID, item, repairableBuilding.Location, minionPartyInstance.PartyType);
					}
				}
				return;
			}
			foreach (global::Kampai.Game.IMinionPartyBuilding item2 in instancesByType)
			{
				view.EndBuildingMinionParty(item2.ID);
			}
		}

		private void DestroyDummyBuilding()
		{
			if (currentDummyBuilding != null)
			{
				global::UnityEngine.Object.Destroy(currentDummyBuilding.gameObject);
			}
		}
	}
}
