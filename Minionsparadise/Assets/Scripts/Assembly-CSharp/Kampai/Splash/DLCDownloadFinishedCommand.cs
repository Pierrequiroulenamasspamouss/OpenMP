namespace Kampai.Splash
{
	public class DLCDownloadFinishedCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DLCDownloadFinishedCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public ILocalPersistanceService localPersistService { get; set; }

		[Inject]
		public global::Kampai.Game.LoginUserSignal loginSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ReInitializeGameSignal reInitializeGameSignal { get; set; }

		[Inject]
		public global::Kampai.Main.IAssetsPreloadService assetsPreloadService { get; set; }

		public override void Execute()
		{
			logger.Info("DLCDownloadFinishedCommand");
			if (global::Kampai.Main.LoadState.Get() != global::Kampai.Main.LoadStateType.STARTED)
			{
				global::Kampai.Main.LoadState.Set(global::Kampai.Main.LoadStateType.BOOTING);
				// Bypass re-initialization (scene reload) to keep the flow moving and avoid stalls
				if (!localPersistService.HasKeyPlayer("COPPA_Age_Year"))
				{
					assetsPreloadService.PreloadAllAssets();
				}
				loginSignal.Dispatch();
			}
		}
	}
}
