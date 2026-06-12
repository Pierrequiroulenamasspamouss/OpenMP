namespace Kampai.Game
{
	public class GooglePlayService : global::Kampai.Game.ISocialService, global::Kampai.Game.ISynergyService
	{
		private global::strange.extensions.signal.impl.Signal<global::Kampai.Game.ISocialService> successSignal;

		private global::strange.extensions.signal.impl.Signal<global::Kampai.Game.ISocialService> failureSignal;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("GooglePlayService") as global::Kampai.Util.IKampaiLogger;

		private bool killSwitchFlag;

		private string serverAuthCode;

		private bool attemptToAuthenticate;

		private global::System.Action callback;

		[Inject]
		public ILocalPersistanceService localPersistence { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public global::Kampai.Util.IInvokerService invoker { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		[Inject]
		public global::Kampai.Game.GooglePlayServerAuthCodeReceivedSignal googlePlayServerAuthCodeReceivedSignal { get; set; }

		public string LoginSource { get; set; }

		public string ServerAuthCode
		{
			get
			{
				return serverAuthCode;
			}
		}

		public string userID
		{
			get
			{
				if (global::GooglePlayGames.PlayGamesPlatform.Instance != null && global::GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated())
				{
					return global::GooglePlayGames.PlayGamesPlatform.Instance.GetUserId();
				}
				return string.Empty;
			}
		}

		public string userName
		{
			get
			{
				if (global::GooglePlayGames.PlayGamesPlatform.Instance != null && global::GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated())
				{
					return global::GooglePlayGames.PlayGamesPlatform.Instance.GetUserDisplayName();
				}
				return string.Empty;
			}
		}

		public bool isLoggedIn
		{
			get
			{
				return global::GooglePlayGames.PlayGamesPlatform.Instance != null && global::GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated();
			}
		}

		public string accessToken
		{
			get
			{
				if (global::UnityEngine.Social.localUser.authenticated)
				{
					return serverAuthCode;
				}
				return string.Empty;
			}
		}

		public bool isKillSwitchEnabled
		{
			get
			{
				return killSwitchFlag;
			}
		}

		public global::System.DateTime tokenExpiry
		{
			get
			{
				return default(global::System.DateTime);
			}
		}

		public global::Kampai.Game.SocialServices type
		{
			get
			{
				return global::Kampai.Game.SocialServices.GOOGLEPLAY;
			}
		}

		public string locKey
		{
			get
			{
				return "AccountTypeGoogle";
			}
		}

		public void RequestServerAuthCode()
		{
			if (!isLoggedIn)
			{
				logger.Error("Server auth code can be requested when player is authenticated");
				googlePlayServerAuthCodeReceivedSignal.Dispatch(false, this);
			}
			else
			{
				global::GooglePlayGames.PlayGamesPlatform.Instance.GetServerAuthCode(OnServerAuthCodeRequest);
			}
		}

		public void ResetServerAuthCode()
		{
			serverAuthCode = null;
		}

		public void Init(global::strange.extensions.signal.impl.Signal<global::Kampai.Game.ISocialService> successSignal, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.ISocialService> failureSignal)
		{
			updateKillSwitchFlag();
			global::GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = global::UnityEngine.Debug.isDebugBuild;
			global::GooglePlayGames.PlayGamesPlatform.Activate();
			this.successSignal = successSignal;
			this.failureSignal = failureSignal;
			logger.Debug("GOOGLE PLAY INIT START");
			localPersistence.PutData("SocialInProgress", "False");
			if (!isLoggedIn)
			{
				logger.Debug("GOOGLE PLAY USER NOT SIGNED IN");
				int dataInt = localPersistence.GetDataInt("GoogleFailCount");
				if (dataInt >= 1)
				{
					logger.Debug("GOOGLE PLAY MAX ATTEMPTS");
					return;
				}
				logger.Debug("GOOGLE PLAY USER NOT SIGNED IN - BELOW MAX ATTEMPTS");
				if (isKillSwitchEnabled)
				{
					failureSignal.Dispatch(this);
				}
				else
				{
					Authenticate();
				}
			}
			else
			{
				logger.Debug("GOOGLE PLAY USER ALREADY LOGGED IN");
				attemptToAuthenticate = true;
				global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
				if (serverAuthCode == null && userSession != null && (userSession.SocialIdentities == null || userSession.SocialIdentities.Count == 0))
				{
					AuthSuccess();
				}
				else
				{
					successSignal.Dispatch(this);
				}
			}
		}

		public void Login(global::strange.extensions.signal.impl.Signal<global::Kampai.Game.ISocialService> successSignal, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.ISocialService> failureSignal, global::System.Action callback)
		{
			if (isKillSwitchEnabled)
			{
				failureSignal.Dispatch(this);
				return;
			}
			this.successSignal = successSignal;
			this.failureSignal = failureSignal;
			this.callback = callback;
			Authenticate();
		}

		private void Authenticate()
		{
			serverAuthCode = null;
			attemptToAuthenticate = true;
			localPersistence.PutData("SocialInProgress", "True");
			global::GooglePlayGames.PlayGamesPlatform.Instance.Authenticate(OnAuthenticate);
		}

		private void OnAuthenticate(bool success)
		{
			if (success)
			{
				LoginSource = "Authentication";
				AuthSuccess();
			}
			else
			{
				AuthFailure("Social.localUser.Authenticate failed");
			}
		}

		public void AuthSuccess()
		{
			if (!attemptToAuthenticate)
			{
				return;
			}
			if (string.IsNullOrEmpty(serverAuthCode))
			{
				global::GooglePlayGames.PlayGamesPlatform.Instance.GetServerAuthCode(OnServerAuthCodeRequestOnLogin);
				return;
			}
			attemptToAuthenticate = false;
			localPersistence.PutData("SocialInProgress", "False");
			logger.Debug("GOOGLE PLAY AUTH SUCCESS");
			logger.Debug("GP PLAYER ID: {0}", userID);
			logger.Debug("GP NAME: {0}", userName);
			logger.Debug("GP Server auth code: {0}", serverAuthCode);
			localPersistence.PutDataInt("GoogleFailCount", 0);
			successSignal.Dispatch(this);
			if (callback != null)
			{
				callback();
			}
		}

		private void OnServerAuthCodeRequestOnLogin(global::GooglePlayGames.BasicApi.CommonStatusCodes status, string serverAuthCode)
		{
			if ((status == global::GooglePlayGames.BasicApi.CommonStatusCodes.Success || status == global::GooglePlayGames.BasicApi.CommonStatusCodes.SuccessCached) && !string.IsNullOrEmpty(serverAuthCode))
			{
				logger.Debug("OnServerAuthCodeRequestOnLogin: success, status: {0}, serverAuthCode {1}", status, serverAuthCode);
				this.serverAuthCode = serverAuthCode;
				AuthSuccess();
			}
			else
			{
				logger.Debug("OnServerAuthCodeRequestOnLogin: failure, status: {0}", status);
				Logout();
				string error = string.Format("Couldn't fetch Google Play server auth code, status: {0}, serverAuthCode: {1}", status, serverAuthCode ?? "null");
				AuthFailure(error);
			}
		}

		private void OnServerAuthCodeRequest(global::GooglePlayGames.BasicApi.CommonStatusCodes status, string serverAuthCode)
		{
			if ((status == global::GooglePlayGames.BasicApi.CommonStatusCodes.Success || status == global::GooglePlayGames.BasicApi.CommonStatusCodes.SuccessCached) && !string.IsNullOrEmpty(serverAuthCode))
			{
				logger.Debug("OnServerAuthCodeRequest: success, status: {0}, serverAuthCode {1}", status, serverAuthCode);
				this.serverAuthCode = serverAuthCode;
			}
			else
			{
				logger.Debug("Couldn't fetch Google Play server auth code, status: {0}, serverAuthCode: {1}", status, serverAuthCode ?? "null");
				this.serverAuthCode = null;
			}
			bool type = this.serverAuthCode != null;
			googlePlayServerAuthCodeReceivedSignal.Dispatch(type, this);
		}

		public void AuthFailure(string error)
		{
			if (attemptToAuthenticate)
			{
				attemptToAuthenticate = false;
				localPersistence.PutData("SocialInProgress", "False");
				logger.Debug("Fail msg: {0}", error);
				logger.Debug("GOOGLE PLAY AUTH FAILURE");
				int dataInt = localPersistence.GetDataInt("GoogleFailCount");
				localPersistence.PutDataInt("GoogleFailCount", ++dataInt);
				failureSignal.Dispatch(this);
			}
		}

		public void Logout()
		{
			attemptToAuthenticate = false;
			((global::GooglePlayGames.PlayGamesPlatform)global::UnityEngine.Social.Active).SignOut();
		}

		public void SendLoginTelemetry(string loginLocation)
		{
			telemetryService.Send_Telemetry_EVT_EBISU_LOGIN_GOOGLEPLAY(loginLocation);
		}

		public void updateKillSwitchFlag()
		{
			killSwitchFlag = configurationsService.isKillSwitchOn(global::Kampai.Game.KillSwitch.GOOGLEPLAY);
		}

		public void incrementAchievement(string achievementID, float percentComplete)
		{
			global::System.Action<bool> action = delegate(bool success)
			{
				logger.Debug("GooglePlayService.incrementAchievement(): achievement: {0} update to percent: {1} success: {2}", achievementID, (int)percentComplete, success);
			};
			global::GooglePlayGames.PlayGamesPlatform.Instance.IncrementAchievement(achievementID, (int)percentComplete, action);
		}

		public void ShowAchievements()
		{
			global::UnityEngine.Social.ShowAchievementsUI();
		}
	}
}
