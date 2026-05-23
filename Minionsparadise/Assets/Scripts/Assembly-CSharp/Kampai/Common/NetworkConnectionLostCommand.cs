namespace Kampai.Common
{
	public class NetworkConnectionLostCommand : global::strange.extensions.command.impl.Command
	{
		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("NetworkConnectionLostCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Common.NetworkModel networkModel { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowOfflinePopupSignal showOfflinePopupSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		public override void Execute()
		{
			if (userSessionService != null && userSessionService.IsOffline)
			{
				return;
			}
			logger.Info("NetworkConnectionLostCommand");
			networkModel.isConnectionLost = true;
			showOfflinePopupSignal.Dispatch(true);
		}
	}
}
