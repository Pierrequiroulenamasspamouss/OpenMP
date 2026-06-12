namespace Kampai.Game
{
	public class SocialLogoutCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("SocialLogoutCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.ISocialService socialService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.UI.View.PopupMessageSignal popupMessageSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateMarketplaceSlotStateSignal updateSlotStateSignal { get; set; }

		[Inject("game.server.host")]
		public string ServerUrl { get; set; }

		[Inject]
		public global::Kampai.Splash.IDownloadService downloadService { get; set; }

		[Inject]
		public global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequestFactory requestFactory { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public global::Kampai.Main.ReloadGameSignal reloadGameSignal { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistService { get; set; }

		public override void Execute()
		{
			global::Kampai.Game.SocialServices type = socialService.type;
			logger.Debug("Social Logout Command Called With {0}", type.ToString());
			
			if (type == global::Kampai.Game.SocialServices.FACEBOOK)
			{
				// Call server to unlink Discord
				string userId = userSessionService.UserSession.UserID;
				string unlinkUrl = ServerUrl + "/rest/v2/user/" + global::System.Uri.EscapeDataString(userId) + "/discord/unlink";
				
				global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
				signal.AddListener(OnUnlinkResponse);
				
				downloadService.Perform(requestFactory.Resource(unlinkUrl)
					.WithHeaderParam("user_id", userSessionService.UserSession.UserID)
					.WithHeaderParam("session_key", userSessionService.UserSession.SessionID)
					.WithContentType("application/json")
					.WithMethod("POST")
					.WithResponseSignal(signal));
			}
			else
			{
				// Non-Discord logout (Google Play etc.)
				PerformLocalLogout();
			}
		}

		private void OnUnlinkResponse(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			if (response.Success)
			{
				logger.Debug("Discord unlink successful on server");
			}
			else
			{
				logger.Warning("Discord unlink failed on server: {0}", response.Body);
			}
			
			// Always perform local logout regardless of server response
			PerformLocalLogout();
			
			// Clear local Discord credentials
			localPersistService.PutData("Discord_AccessToken", "");
			localPersistService.PutData("Discord_UserID", "");
			
			// Reload the game so it starts fresh with the unlinked account
			reloadGameSignal.Dispatch();
		}

		private void PerformLocalLogout()
		{
			global::Kampai.Game.SocialServices type = socialService.type;
			socialService.Logout();
			switch (type)
			{
			case global::Kampai.Game.SocialServices.FACEBOOK:
			{
				string type3 = localService.GetString("fbLogoutSuccess");
				popupMessageSignal.Dispatch(type3, global::Kampai.UI.View.PopupMessageType.NORMAL);
				updateSlotStateSignal.Dispatch();
				break;
			}
			case global::Kampai.Game.SocialServices.GOOGLEPLAY:
			{
				string type2 = localService.GetString("googleplaylogoutsuccess");
				popupMessageSignal.Dispatch(type2, global::Kampai.UI.View.PopupMessageType.NORMAL);
				break;
			}
			}
		}
	}
}
