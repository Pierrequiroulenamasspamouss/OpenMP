namespace Kampai.Main
{
	public class MainStartCommand : global::strange.extensions.command.impl.Command
	{
		private const string MEDIA_RECEIVER_CLASS_NAME = "com.ea.gp.minions.app.MediaReceiver";

		private global::Kampai.Util.IKampaiLogger logger;

		[Inject(global::Kampai.Main.MainElement.MANAGER_PARENT)]
		public global::UnityEngine.GameObject managers { get; set; }

		[Inject]
		public global::Kampai.Main.InitLocalizationServiceSignal initLocalizationServiceSignal { get; set; }

		[Inject]
		public global::Kampai.Main.LoadDevicePrefsSignal loadDevicePrefsSignal { get; set; }

		[Inject]
		public global::Kampai.Common.CheckAvailableStorageSignal checkAvailableStorageSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IInvokerService invokerService { get; set; }

		[Inject]
		public global::Kampai.Game.IDLCService dlcService { get; set; }

		[Inject]
		public global::Kampai.Main.SetupHockeyAppSignal setupHockeyAppSignal { get; set; }

		[Inject]
		public global::Kampai.Main.SetupEventSystemSignal loadEventSystemSignal { get; set; }

		[Inject]
		public global::Kampai.Game.LoadConfigurationSignal loadConfigurationSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		// NimbleTelemetrySender injection removed

		[Inject]
		public global::Kampai.Main.SetupNativeAlertManagerSignal setupNativeAlertManagerSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject(global::strange.extensions.context.api.ContextKeys.CONTEXT_VIEW)]
		public global::UnityEngine.GameObject contextView { get; set; }

		[Inject]
		public global::Kampai.Common.SetupLoggingTargetsSignal setupLoggingTargetsSignal { get; set; }

		[Inject]
		public global::Kampai.Common.MediaStateChangedSignal mediaStateChangedSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ReloadGameSignal reloadGameSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public IResourceService resourceService { get; set; }

		public override void Execute()
		{
			setupLoggingTargetsSignal.Dispatch();
			logger = global::Elevation.Logging.LogManager.GetClassLogger("MainStartCommand") as global::Kampai.Util.IKampaiLogger;
			
			
			string defPath = global::Kampai.Util.OfflineModeUtility.DefinitionsCachePath;
			if (!global::System.IO.File.Exists(defPath))
			{
				string defsText = resourceService.LoadText("definitions");
				if (!string.IsNullOrEmpty(defsText))
				{
					global::Kampai.Util.OfflineModeUtility.SaveLocal(defPath, defsText);
				}
			}

			string configServerPath = global::Kampai.Util.OfflineModeUtility.ConfigCachePath;
			if (!global::System.IO.File.Exists(configServerPath))
			{
				// config_server.json is the resource name mentioned by the user
				string configServerText = resourceService.LoadText("config_server");
				if (!string.IsNullOrEmpty(configServerText) && !configServerText.Contains("ERROR")) // Basic check
				{
					global::Kampai.Util.OfflineModeUtility.SaveLocal(configServerPath, configServerText);
				}
			}

			logger.EventStart("MainStartCommand.Execute");
			global::Kampai.Util.StartupTimer.LogCheckpoint("MainStartCommand.Execute");
			global::Kampai.Util.KampaiResources.SetLogger();
			global::Kampai.Util.KampaiResources.InitializeAssetMap();
			global::Kampai.Util.AndroidPermissions.RequestPermissions();
			loadDevicePrefsSignal.Dispatch();
			initLocalizationServiceSignal.Dispatch();
			checkAvailableStorageSignal.Dispatch(string.Empty, 2097152uL, ContinueExecution);
			logger.EventStop("MainStartCommand.Execute");
		}

		private void ContinueExecution()
		{
			// Restore telemetry with a simple log sender for debugging
			telemetryService.AddTelemetrySender(new global::Kampai.Common.SimpleLogTelemetrySender());
			telemetryService.SharingUsageCompliance();
			telemetryService.COPPACompliance();

			string sWRVEGroup = "default";
			telemetryService.Send_Telemetry_EVT_USER_GAME_LOAD_FUNNEL("10 - Load Start", sWRVEGroup, dlcService.GetDownloadQualityLevel());
			
			global::Kampai.Util.MediaClient.Start();
			global::Kampai.Util.ScreenUtils.ToggleAutoRotation(true);
			SetupBindings();

			telemetryService.Send_Telemetry_EVT_USER_GAME_LOAD_FUNNEL("13 - Akamai Media Client Start", sWRVEGroup, dlcService.GetDownloadQualityLevel());
			setupHockeyAppSignal.Dispatch();
			
			telemetryService.Send_Telemetry_EVT_USER_GAME_LOAD_FUNNEL("16 - HockeyApp And Loggy Start", sWRVEGroup, dlcService.GetDownloadQualityLevel());
			loadEventSystemSignal.Dispatch();
			loadConfigurationSignal.Dispatch(true);
			setupNativeAlertManagerSignal.Dispatch();
		}

		private void SetupBindings()
		{
			managers.transform.SetParent(contextView.transform, false);
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Invoker");
			gameObject.transform.parent = managers.transform;
			global::Kampai.Util.Invoker invoker = gameObject.AddComponent<global::Kampai.Util.Invoker>();
			global::Kampai.Util.InvokerService invokerService = this.invokerService as global::Kampai.Util.InvokerService;
			if (invokerService != null)
			{
				invoker.Initialize(invokerService);
			}
			else
			{
				logger.Error("Unexpected binding to IInvokerService. InvokerService is expected.");
			}
			global::UnityEngine.GameObject gameObject2 = new global::UnityEngine.GameObject("NetworkMonitor");
			gameObject2.transform.SetParent(managers.transform, false);
			gameObject2.AddComponent<global::Kampai.Main.View.NetworkMonitorView>();
			mediaStateChangedSignal.AddOnce(OnMediaStateChanged);
		}

		private void OnMediaStateChanged(global::Kampai.Util.Tuple<string, string> state)
		{
			string item = state.Item1;
			string item2 = state.Item2;
			logger.Warning("Media state changed to {0} for {1}", item.Substring(item.LastIndexOf('.') + 1), item2);
			reloadGameSignal.Dispatch();
		}

		private void RegisterMediaReceiver()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.ea.gp.minions.app.MediaReceiver"))
			{
				androidJavaClass.CallStatic("setOnReceiveBroadcastListener", new global::Kampai.Util.Native.OnReceiveBroadcastListener(delegate(global::UnityEngine.AndroidJavaObject context, global::UnityEngine.AndroidJavaObject intent)
				{
					invokerService.Add(delegate
					{
						try
						{
							using (global::UnityEngine.AndroidJavaObject androidJavaObject = intent.Call<global::UnityEngine.AndroidJavaObject>("getData", new object[0]))
							{
								mediaStateChangedSignal.Dispatch(global::Kampai.Util.Tuple.Create(intent.Call<string>("getAction", new object[0]), androidJavaObject.Call<string>("getPath", new object[0])));
							}
						}
						finally
						{
							context.Dispose();
							intent.Dispose();
						}
					});
				}));
			}
#endif
		}

		private void UnregisterMediaReceiver()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.ea.gp.minions.app.MediaReceiver"))
			{
				androidJavaClass.CallStatic("setOnReceiveBroadcastListener", null);
			}
#endif
		}
	}
}
