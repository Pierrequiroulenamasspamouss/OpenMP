namespace Kampai.Game
{
	public class GameStartCommand : global::strange.extensions.command.impl.Command
	{
		private const int maxSocialInitTries = 3;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("GameStartCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.PopulateEnvironmentSignal environmentSignal { get; set; }

		[Inject]
		public global::Kampai.Game.InitializeSpecialEventSignal initializeSpecialEventSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CancelAllNotificationSignal cancelAllNotificationSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupObjectManagersSignal setupMinionsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupBuildingManagerSignal setupBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateVolumeSignal updateVolumeSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MuteVolumeSignal muteVolumeSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalMusicSignal musicSignal { get; set; }

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject]
		public global::Kampai.Game.EnableCameraBehaviourSignal enableCameraSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DisableCameraBehaviourSignal disableCameraSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupTimeEventServiceSignal timeEventServiceSignal { get; set; }

		[Inject(global::strange.extensions.context.api.ContextKeys.CONTEXT_VIEW)]
		public global::UnityEngine.GameObject contextView { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Game.ILandExpansionService landExpansionService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerDurationService playerDurationService { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Game.SocialInitAllServicesSignal socialInitSignal { get; set; }

		[Inject]
		public global::Kampai.Game.INotificationService notificationService { get; set; }

		[Inject]
		public global::Kampai.Util.ICoroutineProgressMonitor coroutineProgressMonitor { get; set; }

		[Inject]
		public global::Kampai.Common.ICoppaService coppaService { get; set; }

		[Inject]
		public global::Kampai.Game.SetupAudioSignal setupAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.LoadMinionDataSignal loadMinionDataSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupNamedCharactersSignal setupNamedCharactersSignal { get; set; }

		[Inject]
		public global::Kampai.Common.VillainIslandMessageSignal villainIslandMessageSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RandomizeMinionPositionsSignal randomizeMinionPositionsSignal { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistence { get; set; }

		[Inject]
		public global::Kampai.Game.ToggleStickerbookGlowSignal stickerbookGlow { get; set; }

		[Inject]
		public global::Kampai.Game.RandomFlyOverSignal randomFlyOverSignal { get; set; }

		[Inject]
		public global::Kampai.Game.InitializeMarketplaceSlotsSignal initializeSlotsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestorePlayersSalesSignal restoreSalesSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreMinionPartySignal restoreMinionPartySignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartMarketplaceOnboardingSignal startMarketplaceOnboardingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MarketplaceUpdateSoldItemsSignal updateSoldItemsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.LoadServerSalesSignal loadServerSalesSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DisplayRedemptionConfirmationSignal displayRedemptionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CheckSystemNotificationSettingsSignal checkNotificationsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.LoadEnvironmentSignal loadEnvironmentSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IMasterPlanService masterPlanService { get; set; }

		[Inject]
		public global::Kampai.Game.IBuildingUtilities buildingUtilities { get; set; }

		[Inject]
		public global::Kampai.Game.SetupTSMCharacterSignal setupTSMCharacterSignal { get; set; }

		public override void Execute()
		{
			logger.EventStart("GameStartCommand.Execute");
			global::Kampai.Util.TimeProfiler.StartSection("game start");
			loadEnvironmentSignal.Dispatch();
			global::UnityEngine.Camera main = global::UnityEngine.Camera.main;
			global::UnityEngine.GameObject gameObject = main.gameObject;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Main.MainElement.CAMERA)
				.CrossContext();
			main.cullingMask = -148513;
			global::Kampai.Game.View.CameraUtils o = gameObject.AddComponent<global::Kampai.Game.View.CameraUtils>();
			base.injectionBinder.Bind<global::Kampai.Game.View.CameraUtils>().ToValue(o).ToSingleton();
			SetupGameObjects();
			SetupCamera(gameObject);
			masterPlanService.Initialize();
			if (localPersistence.HasKey("AutoSaveLock"))
			{
				localPersistence.DeleteKey("AutoSaveLock");
			}
		}

		private void SetupGameObjects()
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Flowers");
			gameObject.transform.parent = contextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Game.GameElement.FLOWER_PARENT);
			global::UnityEngine.GameObject gameObject2 = new global::UnityEngine.GameObject("ForSaleSigns");
			gameObject2.transform.parent = contextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject2).ToName(global::Kampai.Game.GameElement.FOR_SALE_SIGN_PARENT);
			global::UnityEngine.GameObject gameObject3 = new global::UnityEngine.GameObject("LandExpansions");
			gameObject3.transform.parent = contextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject3).ToName(global::Kampai.Game.GameElement.LAND_EXPANSION_PARENT);
			global::UnityEngine.GameObject gameObject4 = new global::UnityEngine.GameObject("Special_Event");
			// Leave Special_Event at root level so it is not hidden when splash screen/contextView is deactivated
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject4).ToName(global::Kampai.Game.GameElement.SPECIAL_EVENT_PARENT);
			global::UnityEngine.GameObject gameObject5 = new global::UnityEngine.GameObject("VillainLair");
			gameObject5.transform.parent = contextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject5).ToName(global::Kampai.Game.GameElement.VILLAIN_LAIR_PARENT);
			coroutineProgressMonitor.StartTask(StartGame(), "start game");
		}

		private global::System.Collections.IEnumerator PostLoadRoutine()
		{
			while (coroutineProgressMonitor.HasRunningTasks())
			{
				yield return null;
			}
			InitSocialIfNeeded();
			UpdatePlayerGridSize();
			routineRunner.StartCoroutine(WaitAFrame());
		}

		private void InitMarketplaceSlotsIfNeeded()
		{
			if (coppaService.IsBirthdateKnown())
			{
				initializeSlotsSignal.Dispatch();
			}
		}

		private void InitSocialIfNeeded()
		{
			if (coppaService.IsBirthdateKnown())
			{
				RetrySocialInit(0);
			}
		}

		private void UpdatePlayerGridSize()
		{
			int num = buildingUtilities.AvailableLandSpaceCount();
			logger.Debug("Total Available Space: {0}", num);
			playerService.SetQuantity(global::Kampai.Game.StaticItem.TOTAL_AVAILABLE_LAND_SPACE, num);
		}

		private void RetrySocialInit(int tries)
		{
			routineRunner.StartCoroutine(WaitSomeTime(2f, delegate
			{
				if (userSessionService.UserSession != null)
				{
					if (!string.IsNullOrEmpty(userSessionService.UserSession.SessionID))
					{
						socialInitSignal.Dispatch();
					}
					else
					{
						logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "User Session exists but SessionID is empty, waiting...");
						if (tries < 5)
						{
							tries++;
							RetrySocialInit(tries);
						}
					}
				}
				else if (tries < 5)
				{
					tries++;
					logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "User Session was not available, will retry to initialize social networks in 2 seconds (try {0}/5)", tries);
					RetrySocialInit(tries);
				}
				else
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "User Session was never initialized after multiple retries, social services will not be initialized");
				}
			}));
		}

		private void SetupCamera(global::UnityEngine.GameObject camera)
		{
			camera.AddComponent<global::Kampai.Game.View.TouchPanView>();
			camera.AddComponent<global::Kampai.Game.View.TouchZoomView>();
			camera.AddComponent<global::Kampai.Game.View.TouchDragPanView>();
			routineRunner.StartCoroutine(CameraSignalDelay());
		}

		private global::System.Collections.IEnumerator CameraSignalDelay()
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			enableCameraSignal.Dispatch(7);
			disableCameraSignal.Dispatch(8);
		}

		private global::System.Collections.IEnumerator WaitSomeTime(float time, global::System.Action a)
		{
			yield return new global::UnityEngine.WaitForSeconds(time);
			a();
		}

		private global::System.Collections.IEnumerator WaitAFrame()
		{
			yield return null;
			int tokenCount = (int)playerService.GetQuantity(global::Kampai.Game.StaticItem.MINION_LEVEL_TOKEN);
			int minions = playerService.GetMinionCount();
			int seconds = playerService.LastGameStartUTC;
			playerDurationService.SetLastGameStartUTC();
			if (seconds != 0)
			{
				seconds = playerService.LastGameStartUTC - seconds;
			}
			int expansionCount = playerService.GetPurchasedExpansionCount();
			int expansionsLeft = landExpansionService.GetLandExpansionCount();
			string expansions = string.Format("{0}/{1}", expansionCount, expansionsLeft + expansionCount);
			if (localPersistence.HasKeyPlayer("StickerbookGlow"))
			{
				stickerbookGlow.Dispatch(true);
			}
			if (localPersistence.HasKeyPlayer("MarketSurfacing"))
			{
				int buildingID = global::System.Convert.ToInt32(localPersistence.GetDataPlayer("MarketSurfacing"));
				startMarketplaceOnboardingSignal.Dispatch(buildingID);
			}
			string swrveGroup = playerService.SWRVEGroup;
			telemetryService.Send_Telemetry_EVT_USER_DATA_AT_APP_START(seconds, tokenCount, minions, (!string.IsNullOrEmpty(swrveGroup)) ? swrveGroup : "anyVariant", expansions);
			if (localPersistence.HasKeyPlayer("StickerbookGlow"))
			{
				stickerbookGlow.Dispatch(true);
			}
			if (playerService.GetHighestFtueCompleted() == 999999)
			{
				randomFlyOverSignal.Dispatch(-1);
			}
			displayRedemptionSignal.Dispatch();
			setupTSMCharacterSignal.Dispatch();
			global::Kampai.Main.LoadState.Set(global::Kampai.Main.LoadStateType.STARTED);
			global::Kampai.Util.TimeProfiler.EndSection("game start");
		}

		private global::System.Collections.IEnumerator StartGame()
		{
			while (userSessionService.UserSession == null)
			{
				yield return null;
			}
			string userIdStr = userSessionService.UserSession.UserID;
			long userId = 1L;
			if (!long.TryParse(userIdStr, out userId))
			{
				if (userIdStr != null && userIdStr.StartsWith("OFFLINE_"))
				{
					string numericPart = userIdStr.Substring(8);
					if (!long.TryParse(numericPart, out userId))
					{
						logger.Error("Failed to parse numeric part of offline UserID: '{0}'. Using default '1'.", userIdStr);
						userId = 1L;
					}
					else
					{
						logger.Info("Parsed offline UserID '{0}' as long: {1}", userIdStr, userId);
					}
				}
				else
				{
					logger.Error("Failed to parse UserID: '{0}'. Using default '1'.", userIdStr);
					userId = 1L;
				}
			}
			playerService.ID = userId;
			global::UnityEngine.GameObject footprint = global::UnityEngine.Object.Instantiate(global::Kampai.Util.KampaiResources.Load("Footprint")) as global::UnityEngine.GameObject;
			footprint.transform.parent = contextView.transform;
			footprint.AddComponent<global::Kampai.Game.View.FootprintView>();
			notificationService.Initialize();
			logger.Debug("GameStartCommand: Notification Service initialized.");
			cancelAllNotificationSignal.Dispatch();
			checkNotificationsSignal.Dispatch();
			timeEventServiceSignal.Dispatch(contextView);
			setupAudioSignal.Dispatch();
			environmentSignal.Dispatch(false);
			initializeSpecialEventSignal.Dispatch();
			restoreMinionPartySignal.Dispatch();
			global::System.Collections.Generic.Dictionary<string, float> parameters = new global::System.Collections.Generic.Dictionary<string, float>();
			musicSignal.Dispatch("Play_backGroundMusic_01", parameters);
			setupBuildingSignal.Dispatch();
			updateVolumeSignal.Dispatch();
			muteVolumeSignal.Dispatch();
			setupMinionsSignal.Dispatch();
			yield return coroutineProgressMonitor.waitForPreviousTaskToComplete;
			loadMinionDataSignal.Dispatch();
			yield return null;
			while (fmodService.BanksLoadingInProgress())
			{
				yield return coroutineProgressMonitor.waitForNextFrame;
			}
			setupNamedCharactersSignal.Dispatch();
			playerDurationService.InitLevelUpUTC();
			yield return null;
			randomizeMinionPositionsSignal.Dispatch();
			villainIslandMessageSignal.Dispatch(false);
			InitMarketplaceSlotsIfNeeded();
			global::Kampai.Game.MarketplaceDefinition definition = definitionService.Get<global::Kampai.Game.MarketplaceDefinition>();
			if (definition != null)
			{
				if (playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID) >= definition.LevelGate)
				{
					restoreSalesSignal.Dispatch();
				}
				else
				{
					updateSoldItemsSignal.Dispatch(false);
				}
			}
			else
			{
				logger.Warning("Marketplace Definition is null.");
			}
			loadServerSalesSignal.Dispatch();
			routineRunner.StartCoroutine(PostLoadRoutine());
			logger.EventStop("GameStartCommand.Execute");
		}
	}
}
