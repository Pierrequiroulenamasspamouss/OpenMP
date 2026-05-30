namespace Kampai.UI.View
{
	public class SocialPartyEventEndMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.SocialPartyEventEndView>
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("SocialPartyEventEndMediator") as global::Kampai.Util.IKampaiLogger;

		private bool eventComplete;

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimedSocialEventService timedSocialEventService { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSocialPartyRewardSignal socialPartyRewardSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		public override void OnRegister()
		{
			base.OnRegister();
			base.view.Init();
			eventComplete = false;
			base.view.TitleText.text = localService.GetString("socialpartyendtitle");
			base.view.MessageText.text = localService.GetString("socialpartyenddescription");
			base.view.YesButtonText.text = localService.GetString("socialpartyendbutton");
			base.view.YesButton.ClickedSignal.AddListener(OkButtonPressed);
			global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse>();
			signal.AddListener(OnGetSocialEventStateResponse);
			base.view.OnMenuClose.AddListener(CloseAnimationComplete);
			timedSocialEventService.GetSocialEventState(timedSocialEventService.GetCurrentSocialEvent().ID, signal);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			base.view.YesButton.ClickedSignal.RemoveListener(OkButtonPressed);
			base.view.OnMenuClose.RemoveListener(CloseAnimationComplete);
		}

		protected override void Close()
		{
			OkButtonPressed();
		}

		public void OkButtonPressed()
		{
			base.view.Close();
		}

		public void OnGetSocialEventStateResponse(global::Kampai.Game.SocialTeamResponse response, global::Kampai.Game.ErrorResponse error)
		{
			if (error == null && response != null && response.UserEvent != null)
			{
				global::Kampai.Game.TimedSocialEventDefinition currentSocialEvent = timedSocialEventService.GetCurrentSocialEvent();
				if (currentSocialEvent != null && !response.UserEvent.RewardClaimed && response.Team != null && response.Team.OrderProgress != null && response.Team.OrderProgress.Count == currentSocialEvent.Orders.Count)
				{
					eventComplete = true;
				}
			}
			else
			{
				logger.Error("OnGetSocialEventStateResponse unexpected result or error: " + ((error != null && error.Error != null) ? error.Error.Message : "Unknown"));
			}
		}

		public void CloseAnimationComplete()
		{
			guiService.Execute(global::Kampai.UI.View.GUIOperation.Unload, "popup_SocialParty_End");
			if (eventComplete)
			{
				socialPartyRewardSignal.Dispatch(timedSocialEventService.GetCurrentSocialEvent().ID);
			}
		}
	}
}
