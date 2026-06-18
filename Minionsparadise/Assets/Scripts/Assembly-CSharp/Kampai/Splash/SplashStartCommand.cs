namespace Kampai.Splash
{
	public class SplashStartCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public global::Kampai.Splash.SplashProgressUpdateSignal splashProgressUpdateSignal { get; set; }

		[Inject(global::Kampai.Main.MainElement.MANAGER_PARENT)]
		public global::UnityEngine.GameObject managers { get; set; }

		public override void Execute()
		{
			global::Kampai.Util.StartupTimer.Start();
			global::Kampai.Util.StartupTimer.LogCheckpoint("SplashStartCommand.Execute");
			global::Elevation.Logging.LogManager.RegisterLogger(global::Kampai.Util.KampaiLoggerV2.BuildingKampaiLogger);
			global::Kampai.Util.IKampaiLogger kampaiLogger = global::Elevation.Logging.LogManager.GetClassLogger("SplashStartCommand") as global::Kampai.Util.IKampaiLogger;
			global::Kampai.Util.TimeProfiler.InitializeLogger(kampaiLogger);
			kampaiLogger.Debug("Loggly test: This log should have a user ID attached in Loggly!");
			global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find("SplashRoot");
			global::UnityEngine.GameObject gameObject2 = gameObject.FindChild("ToolTip");
			gameObject2.AddComponent<global::Kampai.Splash.LoadInTipView>();
			global::UnityEngine.GameObject gameObject3 = gameObject.FindChild("meter_bar");
			gameObject3.AddComponent<global::Kampai.Splash.LoadingBarView>();
			SetupBindings();
			splashProgressUpdateSignal.Dispatch(10, 10f);
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("Main", global::UnityEngine.SceneManagement.LoadSceneMode.Additive);
			global::UnityEngine.GameObject gameObject4 = gameObject.FindChild("LogoPrefab");
			global::Kampai.Splash.LogoPanelView logoPanelView = gameObject4.AddComponent<global::Kampai.Splash.LogoPanelView>();
			logoPanelView.SetNoWifiPanel(gameObject.FindChild("popup_NoWiFi"));
		}

		public void SetupBindings()
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("AppTracker");
			gameObject.transform.parent = managers.transform;
			gameObject.AddComponent<AppTrackerView>();
		}
	}
}
