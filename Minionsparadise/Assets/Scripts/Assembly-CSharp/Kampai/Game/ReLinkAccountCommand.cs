namespace Kampai.Game
{
	public class ReLinkAccountCommand : global::strange.extensions.command.impl.Command
	{
		public const string ACCOUNT_LINK_ENDPOINT = "/rest/v2/user/{0}/identity/{1}";

		public const string ACCOUNT_REVERSE_LINK_ENDPOINT = "/rest/v2/user/{0}/identity/{1}/reverseLink";

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("ReLinkAccountCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.ISocialService socialService { get; set; }

		[Inject]
		public string toUserId { get; set; }

		[Inject]
		public bool reverseLink { get; set; }

		[Inject("game.server.host")]
		public string ServerUrl { get; set; }

		[Inject]
		public global::Kampai.Splash.IDownloadService downloadService { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public ILocalPersistanceService LocalPersistService { get; set; }

		[Inject]
		public IEncryptionService encryptionService { get; set; }

		[Inject]
		public global::Kampai.Main.ReloadGameSignal reloadGameSiganl { get; set; }

		[Inject]
		public global::Kampai.Game.ITimedSocialEventService socialEventService { get; set; }

		[Inject]
		public global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequestFactory requestFactory { get; set; }

		[Inject]
		public global::Kampai.Game.SocialLogoutSignal socialLogout { get; set; }

		[Inject]
		public global::Kampai.Game.GooglePlayServerAuthCodeReceivedSignal googlePlayServerAuthCodeReceivedSignal { get; set; }

		public override void Execute()
		{
			global::Kampai.Game.GooglePlayService googlePlayService = socialService as global::Kampai.Game.GooglePlayService;
			if (googlePlayService != null && googlePlayService.isLoggedIn && googlePlayService.ServerAuthCode == null)
			{
				googlePlayServerAuthCodeReceivedSignal.AddOnce(OnGooglePlayServerAuthCodeReceived);
				googlePlayService.RequestServerAuthCode();
			}
			else
			{
				RelinkAccount();
			}
		}

		private void OnGooglePlayServerAuthCodeReceived(bool success, global::Kampai.Game.ISocialService socialService)
		{
			RelinkAccount();
		}

		private void RelinkAccount()
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			string arg = global::System.Uri.EscapeDataString(userSession.UserID);
			string plainText = LocalPersistService.GetData("AnonymousID");
			encryptionService.TryDecrypt(plainText, "Kampai!", out plainText);
			string arg2 = global::System.Uri.EscapeDataString(plainText);
			global::Kampai.Game.AccountReLinkRequest accountReLinkRequest = new global::Kampai.Game.AccountReLinkRequest();
			if (socialService.isLoggedIn)
			{
				accountReLinkRequest.credentials = socialService.accessToken;
				accountReLinkRequest.externalId = socialService.userID;
				accountReLinkRequest.identityType = socialService.type.ToString().ToLower();
			}
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(OnAccountLinkResponse);
			accountReLinkRequest.toUserId = toUserId;
			string format = "/rest/v2/user/{0}/identity/{1}";
			if (reverseLink)
			{
				format = "/rest/v2/user/{0}/identity/{1}/reverseLink";
			}
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format(format, arg, arg2)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithEntity(accountReLinkRequest)
				.WithResponseSignal(signal));
		}

		private void OnAccountLinkResponse(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			string body = response.Body;
			int code = response.Code;
			if (response.Success)
			{
				logger.Debug("Relink Success: {0}", body);
				global::Kampai.Game.UserIdentity userIdentity = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.UserIdentity>(body);
				if (!reverseLink)
				{
					LocalPersistService.PutDataInt("RelinkingAccount", 1);
					socialEventService.ClearCache();
					LocalPersistService.PutData("UserID", userIdentity.UserID);
					LocalPersistService.PutData("LoadMode", "externalLogin");
					userSessionService.UserSession.UserID = userIdentity.UserID;
					userSessionService.UserSession.SocialIdentities.Add(userIdentity);
					reloadGameSiganl.Dispatch();
				}
			}
			else if (code == 409)
			{
				logger.Error("Social Account is already linked to an account");
				RemoveSocialIdentity();
				logger.Debug(body);
			}
			else
			{
				RemoveSocialIdentity();
				logger.Error("Error ReLinking Social Account");
				logger.Debug(body);
			}
			if (!response.Success)
			{
				socialLogout.Dispatch(socialService);
			}
		}

		private void RemoveSocialIdentity()
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.UserIdentity> socialIdentities = userSessionService.UserSession.SocialIdentities;
			for (int i = 0; i < socialIdentities.Count; i++)
			{
				if (socialIdentities[i].ExternalID == socialService.userID)
				{
					socialIdentities.RemoveAt(i);
					break;
				}
			}
		}
	}
}
