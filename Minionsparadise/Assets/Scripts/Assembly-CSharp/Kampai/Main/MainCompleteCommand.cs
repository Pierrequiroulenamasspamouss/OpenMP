namespace Kampai.Main
{
	public class MainCompleteCommand : global::strange.extensions.command.impl.Command
	{
		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("MainCompleteCommand") as global::Kampai.Util.IKampaiLogger;

		private global::UnityEngine.AsyncOperation async;

		[Inject]
		public global::Kampai.Game.AutoSavePlayerStateSignal autoSavePlayerSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ReloadConfigurationsPeriodicSignal reloadConfigs { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Main.LoadDevicePrefsSignal loadDevicePrefsSignal { get; set; }



		[Inject]
		public global::Kampai.Common.Service.HealthMetrics.IClientHealthService clientHealthService { get; set; }

		[Inject]
		public global::Kampai.Common.LogTapEventMetricsSignal logTapEventMetricsSignal { get; set; }

		[Inject]
		public global::Kampai.Main.LoadLocalizationServiceSignal localServiceSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupPushNotificationsSignal setupPushNotificationsSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }



		[Inject]
		public global::Kampai.Game.ITimedSocialEventService socialEventService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IDLCService dlcService { get; set; }

		[Inject]
		public global::Kampai.Splash.DLCModel dlcModel { get; set; }

		// NimbleOTSignal injection removed

		[Inject]
		public global::Kampai.Util.ICoroutineProgressMonitor coroutineProgressMonitor { get; set; }

		[Inject]
		public global::Kampai.Main.LoadAudioSignal loadAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Splash.LaunchDownloadSignal launchDownloadSignal { get; set; }

		[Inject]
		public global::Kampai.Splash.SplashProgressUpdateSignal splashProgressDoneSignal { get; set; }

		[Inject]
		public global::Kampai.Splash.ILoadInService loadInService { get; set; }

		[Inject]
		public global::Kampai.Util.FastCommandPool fastCommandPool { get; set; }

		[Inject]
		public global::Kampai.Main.CheckDLCTierSignal checkDLCTier { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistService { get; set; }

		[Inject]
		public global::Kampai.Game.LoadVillainLairAssetsSignal loadVillainLairAssetsSignal { get; set; }

		[Inject]
		public global::Kampai.Main.IAssetsPreloadService assetsPreloadService { get; set; }

		[Inject]
		public global::Kampai.Main.IHindsightService hindsightService { get; set; }

		public override void Execute()
		{
			logger.EventStart("MainCompleteCommand.Execute");
			logger.Info("MainCompleteCommand: Starting Execute phase...");
			checkDLCTier.Dispatch();
			int quantity = (int)playerService.GetQuantity(global::Kampai.Game.StaticItem.TIER_ID);
			dlcService.SetPlayerDLCTier(quantity);
			global::Kampai.Util.TimeProfiler.StartSection("loading scenes");
			// logger.Info("MainCompleteCommand: Dispatching loadDevicePrefsSignal...");
			loadDevicePrefsSignal.Dispatch();
			// logger.Info("MainCompleteCommand: Dispatching loadAudioSignal...");
			loadAudioSignal.Dispatch();
			// logger.Info("MainCompleteCommand: Starting PostExternalScenes coroutine...");
			routineRunner.StartCoroutine(PostExternalScenes());

			hindsightService.Initialize();
			logger.EventStop("MainCompleteCommand.Execute");
			logger.Info("MainCompleteCommand: Execute phase complete.");
		}

		private global::System.Collections.IEnumerator PostExternalScenes()
		{
			yield return null;
			logger.Debug("MainCompleteCommand: PostExternalScenes: Waiting for running tasks...");
			while (coroutineProgressMonitor.HasRunningTasks())
			{
				yield return null;
			}
			logger.Debug("Starting Load Post External Scene");

			assetsPreloadService.StopAssetsPreload();
			localServiceSignal.Dispatch();
			global::Kampai.Util.DeviceCapabilities.Initialize();
			global::Kampai.Util.TimeProfiler.StartSection("loading game scene");
			global::Kampai.Util.StartupTimer.LogCheckpoint("MainCompleteCommand.PostExternalScenes (Scene Loads Dispatched)");
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("Game", global::UnityEngine.SceneManagement.LoadSceneMode.Additive);
			global::UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UI", global::UnityEngine.SceneManagement.LoadSceneMode.Additive);
			splashProgressDoneSignal.Dispatch(100, 3f);
			routineRunner.StartCoroutine(LevelLoadComplete());
		}

		private global::System.Collections.IEnumerator LevelLoadComplete()
		{
			yield return null;
			global::Kampai.Util.StartupTimer.LogCheckpoint("MainCompleteCommand.LevelLoadComplete (Scene Loaded, waiting for building/minion tasks)");
			while (coroutineProgressMonitor.HasRunningTasks())
			{
				yield return null;
			}
			logger.EventStop("MainCompleteCommand.LoadGame");
			global::Kampai.Util.TimeProfiler.EndSection("loading game scene");
			logger.EventStart("MainCompleteCommand.LoadUI");
			autoSavePlayerSignal.Dispatch();
			reloadConfigs.Dispatch();
			// --- Day/Night Cycle Initialization ---
			global::UnityEngine.GameObject dnManagerGo = new global::UnityEngine.GameObject("DayNightCycleManager");
			global::Kampai.Game.DayNightCycleManager dnManager = dnManagerGo.AddComponent<global::Kampai.Game.DayNightCycleManager>();
			base.injectionBinder.injector.Inject(dnManager);
			global::UnityEngine.Object.DontDestroyOnLoad(dnManagerGo);
			clientHealthService.MarkMeterEvent("AppFlow.AppStart");
			telemetryService.Send_Telemetry_EVT_USER_GAME_LOAD_FUNNEL("100 - Load Complete", playerService.SWRVEGroup, dlcService.GetDownloadQualityLevel());
			loadDevicePrefsSignal.Dispatch();
			logTapEventMetricsSignal.Dispatch();
			setupPushNotificationsSignal.Dispatch();
			socialEventService.GetPastEventsWithUnclaimedReward();
			loadInService.SaveTipsForNextLaunch((int)playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID));
			while (coroutineProgressMonitor.HasRunningTasks())
			{
				yield return null;
			}
			logger.EventStop("MainCompleteCommand.LoadUI");
			global::Kampai.Util.TimeProfiler.EndSection("loading scenes");
			// Show the game first, then do heavy cleanup in background
			global::Kampai.Util.TimeProfiler.StartSection("pre-splash-hide");
			fastCommandPool.Warmup();
			global::UnityEngine.Shader.WarmupAllShaders();
			global::Kampai.Util.TimeProfiler.EndSection("pre-splash-hide");

			// Dismiss splash screen as early as possible so the player sees the game
			global::strange.extensions.context.api.ICrossContextCapable splashContext = null;
			try
			{
				splashContext = base.injectionBinder.GetInstance<global::strange.extensions.context.api.ICrossContextCapable>(global::Kampai.Splash.SplashElement.CONTEXT);
			}
			catch (global::strange.extensions.injector.impl.InjectionException ex)
			{
				global::strange.extensions.injector.impl.InjectionException e = ex;
				logger.Warning(e.ToString());
			}
			if (splashContext != null)
			{
				global::Kampai.Util.StartupTimer.LogCheckpoint("MainCompleteCommand.LevelLoadComplete (Splash Screen Dismissed, Load Complete!)");
				splashContext.injectionBinder.GetInstance<global::Kampai.Splash.HideSplashSignal>().Dispatch();
				yield return null;
				ResumeCurrencyService();
			}

			// Now do heavy cleanup after game is visible (non-blocking for the user)
			global::Kampai.Util.TimeProfiler.StartSection("cleanup");
			logger.EventStart("MainCompleteCommand.CleanUp");
			async = global::UnityEngine.Resources.UnloadUnusedAssets();
			routineRunner.StartCoroutine(PostSplashCleanup());
		}

		private global::System.Collections.IEnumerator PostSplashCleanup()
		{
			while (!async.isDone)
			{
				yield return null;
			}
			global::System.GC.Collect();
			logger.EventStop("MainCompleteCommand.CleanUp");
			global::Kampai.Util.TimeProfiler.EndSection("cleanup");

			global::Kampai.Game.VillainLairEntranceBuilding portal = playerService.GetByInstanceId<global::Kampai.Game.VillainLairEntranceBuilding>(374);
			if (portal != null && portal.IsUnlocked)
			{
				loadVillainLairAssetsSignal.Dispatch(portal.VillainLairInstanceID);
			}
			if (localPersistService.HasKey("RelinkingAccount"))
			{
				localPersistService.DeleteKey("RelinkingAccount");
			}
		}

		private void ResumeCurrencyService()
		{
			global::Kampai.Game.ICurrencyService instance = base.injectionBinder.GetInstance<global::Kampai.Game.ICurrencyService>();
			instance.ResumeTransactionsHandling();
		}
	}
}
