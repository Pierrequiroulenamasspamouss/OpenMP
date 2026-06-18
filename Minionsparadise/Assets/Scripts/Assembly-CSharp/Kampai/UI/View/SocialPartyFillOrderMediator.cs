namespace Kampai.UI.View
{
	public class SocialPartyFillOrderMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.SocialPartyFillOrderView>
	{
		private const int MAX_FRIENDS = 4;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("SocialPartyFillOrderMediator") as global::Kampai.Util.IKampaiLogger;

		private int autoFillOrder;

		private global::System.Collections.Generic.IList<global::Kampai.Game.FBUser> friends;

		private global::Kampai.Game.SocialTeam team;

		private global::System.DateTime finishTime;

		private bool bShowFinishMenu;

		private int count;

		private int picturesComplete;

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSocialPartyFillOrderButtonSignal showPartyFillOrderButton { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSocialPartyFillOrderProfileButtonSignal showPartyFillOrderProfile { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject(global::Kampai.Game.SocialServices.FACEBOOK)]
		public global::Kampai.Game.ISocialService facebookService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimedSocialEventService timedSocialEventService { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSocialPartyStartSignal showSocialPartyStartSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSocialPartyInviteAlertSignal showSocialPartyInviteAlertSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowSocialPartyEventEndSignal showSocialPartyEventEndSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal globalSFX { get; set; }

		[Inject]
		public global::Kampai.Common.ICoppaService coppaService { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideSkrimSignal hideSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SocialPartyFillOrderSetupUISignal socialPartyFillOrderSetupUISignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SocialPartyFillOrderButtonMediatorUpdateSignal socialPartyFillOrderButtonMediatorUpdateSignal { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkConnectionLostSignal networkConnectionLostSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideHUDAndIconsSignal hideHUDSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SocialLoginSuccessSignal loginSuccess { get; set; }

		[Inject]
		public global::Kampai.Game.DisplayPlayerTrainingSignal displaySignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IDevicePrefsService notifPrefs { get; set; }

		public override void OnRemove()
		{
			base.OnRemove();
			global::Kampai.Game.FacebookService facebookService = this.facebookService as global::Kampai.Game.FacebookService;
			if (facebookService.userPictures != null)
			{
				facebookService.userPictures.Clear();
			}
			base.view.leaveTeamButton.ClickedSignal.RemoveListener(LeaveTeamButton);
			base.view.messageAlertButton.ClickedSignal.RemoveListener(MessageAlertButton);
			base.view.teamPanelButton.ClickedSignal.RemoveListener(OpenTeam);
			base.view.closeTeamButton.ClickedSignal.RemoveListener(CloseTeam);
			base.view.OnMenuClose.RemoveListener(CloseAnimationComplete);
			socialPartyFillOrderSetupUISignal.RemoveListener(RefreshUI);
			loginSuccess.RemoveListener(OnLoginSuccess);
			hideHUDSignal.Dispatch(true);
		}

		public override void Initialize(global::Kampai.UI.View.GUIArguments args)
		{
			global::Kampai.UI.View.GUIAutoAction<int> gUIAutoAction = args.Get<global::Kampai.UI.View.GUIAutoAction<int>>();
			if (gUIAutoAction != null)
			{
				autoFillOrder = gUIAutoAction.value;
			}
			bShowFinishMenu = false;
			count = 0;
			base.view.Init();
			base.view.leaveTeamButton.ClickedSignal.AddListener(LeaveTeamButton);
			base.view.messageAlertButton.ClickedSignal.AddListener(MessageAlertButton);
			base.view.teamPanelButton.ClickedSignal.AddListener(OpenTeam);
			base.view.closeTeamButton.ClickedSignal.AddListener(CloseTeam);
			base.view.OnMenuClose.AddListener(CloseAnimationComplete);
			socialPartyFillOrderSetupUISignal.AddListener(RefreshUI);
			loginSuccess.AddListener(OnLoginSuccess);
			base.view.messageAlertButton.gameObject.SetActive(false);
			// Pre-initialize finishTime from the event definition so UpdateTime() does not
			// falsely trigger the "event ended" popup during the async team-load HTTP call.
			global::Kampai.Game.TimedSocialEventDefinition def = timedSocialEventService.GetCurrentSocialEvent();
			if (def != null)
			{
				finishTime = new global::System.DateTime(1970, 1, 1, 0, 0, 0, 0, global::System.DateTimeKind.Utc).AddSeconds(def.FinishTime).ToLocalTime();
			}
			LoadTeam();
			hideHUDSignal.Dispatch(false);
		}

		private void OnLoginSuccess(global::Kampai.Game.ISocialService socialService)
		{
			if (socialService.type == global::Kampai.Game.SocialServices.FACEBOOK)
			{
				LoadTeam();
			}
		}

		private void LoadTeam()
		{
			global::Kampai.Game.TimedSocialEventDefinition currentSocialEvent = timedSocialEventService.GetCurrentSocialEvent();
			global::Kampai.Game.SocialTeamResponse socialEventStateCached = timedSocialEventService.GetSocialEventStateCached(currentSocialEvent.ID);
			if (socialEventStateCached != null && socialEventStateCached.Team != null)
			{
				OnGetTeamSuccess(socialEventStateCached, null);
				return;
			}
			global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse>();
			signal.AddListener(OnGetTeamSuccess);
			timedSocialEventService.GetSocialEventState(timedSocialEventService.GetCurrentSocialEvent().ID, signal);
		}

		private void OpenTeam()
		{
			base.view.OpenTeam();
		}

		private void CloseTeam()
		{
			base.view.CloseTeam();
		}

		public void OnGetTeamSuccess(global::Kampai.Game.SocialTeamResponse response, global::Kampai.Game.ErrorResponse error)
		{
			if (error != null)
			{
				networkConnectionLostSignal.Dispatch();
			}
			else if (response != null && response.Team != null)
			{
				team = response.Team;
				if (response.Team != null && response.Team.Members != null && response.Team.Members.Count > 1)
				{
					base.view.leaveTeamButton.gameObject.SetActive(true);
				}
				else
				{
					base.view.leaveTeamButton.gameObject.SetActive(false);
				}
				if (response.UserEvent.Invitations != null && response.UserEvent.Invitations.Count > 0)
				{
					handleInviteButton(response.UserEvent.Invitations);
				}
				else
				{
					base.view.messageAlertButton.gameObject.SetActive(false);
				}
				GetFBFriendsInTeam();
				long num = timedSocialEventService.GetCurrentSocialEvent().FinishTime;
				finishTime = new global::System.DateTime(1970, 1, 1, 0, 0, 0, 0, global::System.DateTimeKind.Utc);
				finishTime = finishTime.AddSeconds(num).ToLocalTime();
			}
		}

		private void handleInviteButton(global::System.Collections.Generic.IList<global::Kampai.Game.SocialEventInvitation> invitations)
		{
			bool flag = false;
			foreach (global::Kampai.Game.SocialEventInvitation invitation in invitations)
			{
				if (!playerService.SeenSocialInvitation(invitation.Team.TeamID))
				{
					flag = true;
					playerService.AddSocialInvitationSeen(invitation.Team.TeamID);
				}
			}
			base.view.messageAlertButton.gameObject.SetActive(true);
			global::UnityEngine.Animator component = base.view.messageAlertButton.gameObject.GetComponent<global::UnityEngine.Animator>();
			component.SetBool("Normal", true);
			if (flag)
			{
				if (component != null)
				{
					component.SetBool("Pulse", true);
				}
			}
			else if (component != null)
			{
				component.SetBool("Pulse", false);
			}
		}

		public void GetFBFriendsInTeam()
		{
			global::Kampai.Game.FacebookService facebookService = this.facebookService as global::Kampai.Game.FacebookService;
			if (!facebookService.isLoggedIn)
			{
				SetupUI();
				return;
			}
			friends = new global::System.Collections.Generic.List<global::Kampai.Game.FBUser>();
			picturesComplete = 0;
			object obj;
			if (team != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.UserIdentity> members = team.Members;
				obj = members;
			}
			else
			{
				obj = null;
			}
			global::System.Collections.Generic.IList<global::Kampai.Game.UserIdentity> list = (global::System.Collections.Generic.IList<global::Kampai.Game.UserIdentity>)obj;
			if (list == null)
			{
				return;
			}
			int num = global::UnityEngine.Mathf.Min(list.Count, 4);
			foreach (global::Kampai.Game.UserIdentity item in list)
			{
				if (item != null && item.ExternalID != null && item.Type == global::Kampai.Game.IdentityType.discord)
				{
					friends.Add(facebookService.GetFriend(item.ExternalID) ?? new global::Kampai.Game.FBUser(string.Empty, item.ExternalID));
					global::strange.extensions.signal.impl.Signal<string> signal = new global::strange.extensions.signal.impl.Signal<string>();
					signal.AddListener(OnFacebookPictureComplete);
					StartCoroutine(facebookService.DownloadUserPicture(item.ExternalID, signal));
					if (friends.Count == num)
					{
						break;
					}
				}
			}
			// No discord friends to download pictures for — call SetupUI directly
			if (friends.Count == 0)
			{
				SetupUI();
			}
		}

		private void OnFacebookPictureComplete(string id)
		{
			global::Kampai.Game.FacebookService facebookService = this.facebookService as global::Kampai.Game.FacebookService;
			if (facebookService.GetUserPicture(id) == null)
			{
				logger.Warning("OnFacebookPictureComplete texture is null for Discord ID: {0}", id);
			}
			picturesComplete++;
			if (picturesComplete == friends.Count)
			{
				SetupUI();
			}
		}

		private global::Kampai.UI.View.SocialPartyFillOrderButtonMediator.SocialPartyFillOrderButtonMediatorData GetData(global::Kampai.Game.SocialEventOrderDefinition orderDefinition, int index)
		{
			global::Kampai.Game.SocialOrderProgress progress = null;
			if (team.OrderProgress != null)
			{
				foreach (global::Kampai.Game.SocialOrderProgress p in team.OrderProgress)
				{
					if (p.OrderId == orderDefinition.OrderID)
					{
						progress = p;
						break;
					}
				}
			}
			global::Kampai.UI.View.SocialPartyFillOrderButtonMediator.SocialPartyFillOrderButtonMediatorData socialPartyFillOrderButtonMediatorData = new global::Kampai.UI.View.SocialPartyFillOrderButtonMediator.SocialPartyFillOrderButtonMediatorData();
			socialPartyFillOrderButtonMediatorData.team = team;
			socialPartyFillOrderButtonMediatorData.progress = progress;
			socialPartyFillOrderButtonMediatorData.orderDefintion = orderDefinition;
			socialPartyFillOrderButtonMediatorData.parent = base.view.SocialFillOrderButtonContainer;
			socialPartyFillOrderButtonMediatorData.index = index;
			socialPartyFillOrderButtonMediatorData.fillOrderSignal = base.view.FillOrderSignal;
			socialPartyFillOrderButtonMediatorData.autoFill = orderDefinition.OrderID == autoFillOrder;
			return socialPartyFillOrderButtonMediatorData;
		}

		public void RefreshUI(global::Kampai.Game.SocialTeam socialTeam)
		{
			int num = 0;
			if (socialTeam != null)
			{
				team = socialTeam;
			}
			if (team == null)
			{
				return;
			}
			foreach (global::Kampai.Game.SocialEventOrderDefinition order in team.Definition.Orders)
			{
				global::Kampai.UI.View.SocialPartyFillOrderButtonMediator.SocialPartyFillOrderButtonMediatorData data = GetData(order, num);
				socialPartyFillOrderButtonMediatorUpdateSignal.Dispatch(data);
				num++;
			}
			UpdateProgress();
		}

		public void SetupUI()
		{
			if (team != null)
			{
				int num = 0;
				foreach (global::Kampai.Game.SocialEventOrderDefinition order in team.Definition.Orders)
				{
					global::Kampai.UI.View.SocialPartyFillOrderButtonMediator.SocialPartyFillOrderButtonMediatorData data = GetData(order, num);
					showPartyFillOrderButton.Dispatch(data, num);
					num++;
				}
				if (!coppaService.Restricted())
				{
					int num2 = team.Members.Count;
					int num3 = 0;
					foreach (global::Kampai.Game.UserIdentity member in team.Members)
					{
						SocialPartyFillOrderProfileButtonMediator.SocialPartyFillOrderProfileButtonMediatorData socialPartyFillOrderProfileButtonMediatorData = new SocialPartyFillOrderProfileButtonMediator.SocialPartyFillOrderProfileButtonMediatorData();
						socialPartyFillOrderProfileButtonMediatorData.identity = member;
						socialPartyFillOrderProfileButtonMediatorData.parent = base.view.SocialFillTeamPanel;
						socialPartyFillOrderProfileButtonMediatorData.index = num3;
						socialPartyFillOrderProfileButtonMediatorData.teamId = team.ID;
						showPartyFillOrderProfile.Dispatch(socialPartyFillOrderProfileButtonMediatorData);
						num3++;
					}
					for (int i = num2; i < 4; i++)
					{
						SocialPartyFillOrderProfileButtonMediator.SocialPartyFillOrderProfileButtonMediatorData socialPartyFillOrderProfileButtonMediatorData2 = new SocialPartyFillOrderProfileButtonMediator.SocialPartyFillOrderProfileButtonMediatorData();
						socialPartyFillOrderProfileButtonMediatorData2.identity = null;
						socialPartyFillOrderProfileButtonMediatorData2.parent = base.view.SocialFillTeamPanel;
						socialPartyFillOrderProfileButtonMediatorData2.index = num3;
						socialPartyFillOrderProfileButtonMediatorData2.teamId = team.ID;
						showPartyFillOrderProfile.Dispatch(socialPartyFillOrderProfileButtonMediatorData2);
						num3++;
					}
				}
				UpdateTime();
				UpdateProgress();
			}
			setupText();
		}

		private void UpdateProgress()
		{
			int num = team.Definition.Orders.Count;
			int num2 = team.OrderProgress.Count;
			base.view.progressBar.transform.localScale = new global::UnityEngine.Vector3((float)num2 / (float)num, 1f, 1f);
			if (num == num2)
			{
				base.view.ordersRemaining.text = localService.GetString("socialpartyfillordercompleted");
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.CancelNotificationSignal>().Dispatch(global::Kampai.Game.NotificationType.SocialEventComplete.ToString());
				return;
			}
			base.view.ordersRemaining.text = string.Format("{0} / {1}", num2, num);
			if (notifPrefs.GetDevicePrefs().SocialEventNotif && num2 > 0)
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.SocialEventNotificationsSignal>().Dispatch();
			}
		}

		private void setupText()
		{
			base.view.leaveTeamButtonText.text = localService.GetString("socialpartyfillorderleavebutton");
			base.view.teamTitle.text = localService.GetString("socialpartyfillorderteamtitle");
			base.view.teamOrderBoardText.text = localService.GetString("socialpartyfillorderboard");
			base.view.descriptionText.text = localService.GetString("socialpartyfillorderdescription");
			base.view.questTitle.text = localService.GetString("Rewards");
			base.view.teamButtonText.text = localService.GetString("socialpartyfillorderteamtitle");
			global::Kampai.Game.TimedSocialEventDefinition currentSocialEvent = timedSocialEventService.GetCurrentSocialEvent();
			global::Kampai.Game.Transaction.TransactionDefinition reward = currentSocialEvent.GetReward(definitionService);
			base.view.grindRewardText.text = "0";
			base.view.premiumRewardText.text = "0";
			foreach (global::Kampai.Util.QuantityItem output in reward.Outputs)
			{
				if (output.ID == 0)
				{
					base.view.grindRewardText.text = UIUtils.FormatLargeNumber((int)output.Quantity);
				}
				else if (output.ID == 1)
				{
					base.view.premiumRewardText.text = UIUtils.FormatLargeNumber((int)output.Quantity);
				}
			}
		}

		public void UpdateTime()
		{
			count++;
			global::System.DateTime dateTime = new global::System.DateTime(1970, 1, 1, 0, 0, 0, 0, global::System.DateTimeKind.Utc).AddSeconds(timeService.CurrentTime()).ToLocalTime();
			global::System.TimeSpan timeSpan = finishTime.Subtract(dateTime);
			if (dateTime < finishTime)
			{
				base.view.timeRemaining.text = UIUtils.FormatTime(timeSpan.TotalSeconds, localService);
				return;
			}
			if (base.view.timeRemaining != null)
			{
				base.view.timeRemaining.text = localService.GetString("socialpartyfillordereventfinished");
			}
			if (!bShowFinishMenu && count > 10)
			{
				bShowFinishMenu = true;
				UnloadFillOrderScreen();
				showSocialPartyEventEndSignal.Dispatch();
			}
		}

		private void UnloadFillOrderScreen()
		{
			hideSignal.Dispatch("SocialSkrim");
			guiService.Execute(global::Kampai.UI.View.GUIOperation.Unload, "SocialPartyFillOrderScreen");
			displaySignal.Dispatch(19000014, false, new global::strange.extensions.signal.impl.Signal<bool>());
		}

		protected override void Update()
		{
			UpdateTime();
		}

		public void LeaveTeamResponse(bool bLeave)
		{
			if (bLeave)
			{
				global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse>();
				signal.AddListener(LeaveSocialTeamServerResponse);
				timedSocialEventService.LeaveSocialTeam(team.SocialEventId, team.ID, signal);
			}
		}

		public void LeaveSocialTeamServerResponse(global::Kampai.Game.SocialTeamResponse newTeam, global::Kampai.Game.ErrorResponse error)
		{
			if (error != null)
			{
				networkConnectionLostSignal.Dispatch();
				return;
			}
			UnloadFillOrderScreen();
			if (newTeam.Team != null)
			{
				team = newTeam.Team;
				SetupUI();
				return;
			}
			global::Kampai.Game.SocialTeamResponse socialEventStateCached = timedSocialEventService.GetSocialEventStateCached(timedSocialEventService.GetCurrentSocialEvent().ID);
			if (socialEventStateCached.UserEvent != null && socialEventStateCached.UserEvent.Invitations != null && socialEventStateCached.UserEvent.Invitations.Count > 0 && facebookService.isLoggedIn)
			{
				showSocialPartyInviteAlertSignal.Dispatch();
			}
			else
			{
				showSocialPartyStartSignal.Dispatch();
			}
		}

		public void MessageAlertButton()
		{
			globalSFX.Dispatch("Play_button_click_01");
			UnloadFillOrderScreen();
			showSocialPartyInviteAlertSignal.Dispatch();
		}

		public void LeaveTeamButton()
		{
			global::strange.extensions.signal.impl.Signal<bool> signal = new global::strange.extensions.signal.impl.Signal<bool>();
			signal.AddListener(LeaveTeamResponse);
			global::Kampai.UI.View.PopupConfirmationSetting type = new global::Kampai.UI.View.PopupConfirmationSetting("socialpartyleaveteamconfirmationtitle", "socialpartyleaveteamconfirmationdescription", false, "img_char_Min_FeedbackChecklist01", signal, string.Empty, string.Empty);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.DisplayConfirmationSignal>().Dispatch(type);
		}

		public void CloseAnimationComplete()
		{
			UnloadFillOrderScreen();
		}

		public void QuitButton()
		{
			globalSFX.Dispatch("Play_button_click_01");
			base.view.Close();
		}

		protected override void Close()
		{
			QuitButton();
		}
	}
}
