namespace Kampai.UI.View
{
	public class HelpPanelMediator : global::strange.extensions.mediation.impl.Mediator
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("HelpPanelMediator") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.UI.View.HelpPanelView view { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal buildingChangeStateSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeSignal { get; set; }

		[Inject(global::Kampai.Main.MainElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable mainContext { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		public override void OnRegister()
		{
			view.onlineHelp.ClickedSignal.AddListener(OnlineHelpClicked);
			view.restorePurchases.ClickedSignal.AddListener(OnRestorePurchases);
			Init();
		}

		public override void OnRemove()
		{
			if (view != null)
			{
				if (view.onlineHelp != null && view.onlineHelp.ClickedSignal != null)
				{
					view.onlineHelp.ClickedSignal.RemoveListener(OnlineHelpClicked);
				}
				if (view.restorePurchases != null && view.restorePurchases.ClickedSignal != null)
				{
					view.restorePurchases.ClickedSignal.RemoveListener(OnRestorePurchases);
				}
			}
		}

		private void Init()
		{
			view.helpIdText.text = string.Format("{0}{1}", localizationService.GetString("playerid"), playerService.ID);
			view.onlineHelpText.text = localizationService.GetString("ContactUs");
			view.learnSystemsText.text = localizationService.GetString("LearnSystems");
			view.selectTopicText.text = localizationService.GetString("SelectTopic");
			view.restorePurchaseText.text = localizationService.GetString("RestorePurchases");
			view.restorePurchases.gameObject.SetActive(false);
		}

		private void OnBuildingStateChange(int buildingId, global::Kampai.Game.BuildingState state)
		{
			if (buildingId == 313 && state == global::Kampai.Game.BuildingState.Idle)
			{
				view.restorePurchases.GetComponent<global::UnityEngine.UI.Button>().interactable = true;
				buildingChangeStateSignal.RemoveListener(OnBuildingStateChange);
			}
		}

		private void OnlineHelpClicked()
		{
			telemetryService.Send_Telemetry_CONTACT_US_CLICKED();
			soundFXSignal.Dispatch("Play_button_click_01");
			mainContext.injectionBinder.GetInstance<global::Kampai.Main.OpenHelpSignal>().Dispatch(global::Kampai.Main.HelpType.ONLINE_HELP);
		}

		private void OnRestorePurchases()
		{
			mainContext.injectionBinder.GetInstance<global::Kampai.Game.RestoreMtxPurchaseSignal>().Dispatch();
			closeSignal.Dispatch(null);
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
			if (configurationsService == null || logger == null) return;
			logger.Info("wwce killswitch :{0}", configurationsService.isKillSwitchOn(global::Kampai.Game.KillSwitch.WWCE));
			if (view != null && view.onlineHelp != null)
			{
				view.onlineHelp.gameObject.SetActive(!configurationsService.isKillSwitchOn(global::Kampai.Game.KillSwitch.WWCE));
			}
		}
	}
}
