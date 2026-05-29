namespace Kampai.Download.View
{
	public class NoWiFiMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private bool canShowSettings;

		[Inject] public global::Kampai.Splash.View.NoWiFiView                  view                         { get; set; }
		[Inject] public global::Kampai.Splash.DLCModel                         dlcModel                     { get; set; }
		[Inject] public global::Kampai.Splash.ShowNoWiFiPanelSignal            showNoWiFiPanelSignal        { get; set; }
		[Inject] public global::Kampai.Common.NetworkTypeChangedSignal         networkTypeChangedSignal     { get; set; }
		[Inject] public global::Kampai.Common.NetworkModel                     networkModel                 { get; set; }
		[Inject] public global::Kampai.Common.NetworkConnectionLostSignal      networkConnectionLostSignal  { get; set; }
		[Inject] public global::Kampai.UI.View.ShowOfflinePopupSignal          showOfflinePopupSignal       { get; set; }
		[Inject] public global::Kampai.Common.ResumeNetworkOperationSignal     resumeNetworkOperationSignal { get; set; }

		public override void OnRegister()
		{
			canShowSettings = global::Kampai.Util.Native.CanShowNetworkSettings();
			view.Init(!canShowSettings);
			if (canShowSettings)
			{
				view.continueButton2.ClickedSignal.AddListener(ContinueButton);
				view.exitButton2.ClickedSignal.AddListener(ExitButton);
				view.settingsButton.ClickedSignal.AddListener(SettingsButton);
			}
			else
			{
				view.continueButton1.ClickedSignal.AddListener(ContinueButton);
				view.exitButton1.ClickedSignal.AddListener(ExitButton);
			}
		}

		private void OnEnable()
		{
			if (view != null)
			{
				Start();
			}
		}

		private void Start()
		{
			networkTypeChangedSignal.AddListener(OnNetworkTypeChanged);
			showOfflinePopupSignal.AddListener(OnShowOfflinePopup);
		}

		private void OnDisable()
		{
			networkTypeChangedSignal.RemoveListener(OnNetworkTypeChanged);
			showOfflinePopupSignal.RemoveListener(OnShowOfflinePopup);
		}

		public override void OnRemove()
		{
			if (canShowSettings)
			{
				view.continueButton2.ClickedSignal.RemoveListener(ContinueButton);
				view.exitButton2.ClickedSignal.RemoveListener(ExitButton);
				view.settingsButton.ClickedSignal.RemoveListener(SettingsButton);
			}
			else
			{
				view.continueButton1.ClickedSignal.RemoveListener(ContinueButton);
				view.exitButton1.ClickedSignal.RemoveListener(ExitButton);
			}
		}

		private void OnNetworkTypeChanged(global::UnityEngine.NetworkReachability type)
		{
			switch (type)
			{
			case global::UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork:
				Close();
				break;
			case global::UnityEngine.NetworkReachability.NotReachable:
				if (!networkModel.isConnectionLost)
				{
					networkConnectionLostSignal.Dispatch();
				}
				break;
			case global::UnityEngine.NetworkReachability.ReachableViaCarrierDataNetwork:
				break;
			}
		}

		private void OnShowOfflinePopup(bool isShown)
		{
			if (isShown)
			{
				Close();
			}
		}

		private void ContinueButton()
		{
			if (networkModel.isConnectionLost || !networkModel.isConnected)
			{
				resumeNetworkOperationSignal.Dispatch();
			}
			showNoWiFiPanelSignal.Dispatch(false);
		}

		private void SettingsButton()
		{
			global::Kampai.Util.Native.OpenNetworkSettings();
		}

		private void ExitButton()
		{
			showNoWiFiPanelSignal.Dispatch(false);
		}

		private void Close()
		{
			showNoWiFiPanelSignal.Dispatch(false);
			if (!networkModel.isConnectionLost)
			{
				resumeNetworkOperationSignal.Dispatch();
			}
		}
	}
}
