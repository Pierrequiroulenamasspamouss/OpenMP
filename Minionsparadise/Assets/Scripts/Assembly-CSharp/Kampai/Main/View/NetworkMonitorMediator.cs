namespace Kampai.Main.View
{
	public class NetworkMonitorMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("NetworkMonitorMediator") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Common.NetworkModel model { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkConnectionLostSignal connectionLostSignal { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkTypeChangedSignal typeChangedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		private global::System.Collections.IEnumerator Start()
		{
			model.reachability = global::Kampai.Util.NetworkUtil.GetNetworkReachability();
			logger.Info("Initial network connection type: {0}.", model.reachability);
			while (true)
			{
				if (!model.isConnectionLost && model.reachability == global::UnityEngine.NetworkReachability.NotReachable && !userSessionService.IsOffline)
				{
					connectionLostSignal.Dispatch();
				}
				yield return new global::UnityEngine.WaitForSeconds(15f);
			}
		}

		private void Update()
		{
			if (model == null)
			{
				return;
			}
			global::UnityEngine.NetworkReachability networkReachability = global::Kampai.Util.NetworkUtil.GetNetworkReachability();
			if (networkReachability != model.reachability)
			{
				logger.Info("Network connection type switched: {0} -> {1}.", model.reachability, networkReachability);
				typeChangedSignal.Dispatch(networkReachability);
				model.reachability = networkReachability;
			}
		}
	}
}
