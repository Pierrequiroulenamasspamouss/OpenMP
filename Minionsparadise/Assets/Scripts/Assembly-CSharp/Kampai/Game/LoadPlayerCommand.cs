namespace Kampai.Game
{
	public class LoadPlayerCommand : global::strange.extensions.command.impl.Command
	{
		public const string PLAYER_DATA_ENDPOINT = "/rest/gamestate/{0}";

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadPlayerCommand") as global::Kampai.Util.IKampaiLogger;

		private bool retried;

		[Inject]
		public IResourceService resourceService { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistService { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject("game.server.host")]
		public string ServerUrl { get; set; }

		[Inject]
		public global::Kampai.Splash.IDownloadService downloadService { get; set; }

		[Inject]
		public global::Kampai.Game.LoadedPlayerDataSignal loadedPlayerDataSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService defService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimedSocialEventService socialEventService { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkConnectionLostSignal networkConnectionLostSignal { get; set; }

		[Inject]
		public global::Kampai.Splash.SplashProgressUpdateSignal splashProgressUpdateSignal { get; set; }

		[Inject]
		public global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequestFactory requestFactory { get; set; }

		[Inject]
		public global::Kampai.Common.IVideoService videoService { get; set; }

		public override void Execute()
		{
			global::Kampai.Util.StartupTimer.LogCheckpoint("LoadPlayerCommand.Execute");
			logger.EventStart("LoadPlayerCommand.Execute");
			global::Kampai.Util.TimeProfiler.StartSection("load player");
			string text = localPersistService.GetData("LoadMode");
			string text2 = string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				switch (text)
				{
				case "default":
					break;
				case "local":
				{
					string text3 = localPersistService.GetData("LocalID");
					text2 = localPersistService.GetData("Player_" + text3);
					goto IL_00fe;
				}
				case "file":
				{
					string path = localPersistService.GetData("LocalFileName");
					text2 = resourceService.LoadText(path);
					goto IL_00fe;
				}
				case "remote":
					if (userSessionService.IsOffline)
					{
						logger.Info("[OfflineMode] Offline mode detected, checking local save at {0}", global::Kampai.Util.OfflineModeUtility.PlayerSavePath);
						text2 = global::Kampai.Util.OfflineModeUtility.LoadLocal(global::Kampai.Util.OfflineModeUtility.PlayerSavePath);
						if (string.IsNullOrEmpty(text2))
						{
							logger.Warning("[OfflineMode] No local save found, falling back to initial player.");
							text2 = defService.GetInitialPlayer();
						}
						else
						{
							logger.Info("[OfflineMode] Successfully loaded local save ({0} bytes).", text2.Length);
						}
					}
					else
					{
						RemoteLoadPlayerData();
					}
					goto IL_00fe;
				case "externalLogin":
					RemoteLoadPlayerData();
					goto IL_00fe;
				default:
					goto IL_00fe;
				}
			}
			if (string.IsNullOrEmpty(text2) && userSessionService.IsOffline)
			{
				logger.Info("[OfflineMode] LoadMode was '{0}', but we are offline. Attempting local save recovery.", text);
				text2 = global::Kampai.Util.OfflineModeUtility.LoadLocal(global::Kampai.Util.OfflineModeUtility.PlayerSavePath);
			}

			if (string.IsNullOrEmpty(text2))
			{
				logger.Warning("[OfflineMode] Falling back to initial player.");
				text2 = defService.GetInitialPlayer();
			}
			
			IL_00fe:
			if (!string.IsNullOrEmpty(text2))
			{
				loadedPlayerDataSignal.Dispatch(text2, new PlayerMetaData());
				splashProgressUpdateSignal.Dispatch(35, 10f);
			}
			logger.EventStop("LoadPlayerCommand.Execute");
		}

		private void RemoteLoadPlayerData()
		{
			localPersistService.PutDataIntPlayer("COPPA_Age_Year", 2000);
			localPersistService.PutDataIntPlayer("COPPA_Age_Month", 1);
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			logger.Debug("Existing User");
			if (userSession != null)
			{
				string userID = userSession.UserID;
				LoadCurrentSocialTeam();
				global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
				signal.AddListener(OnPlayerLoaded);
				downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/gamestate/{0}", userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
					.WithResponseSignal(signal));
				logger.Debug("LoadPlayerCommand: Requesting player data with user id {0}", userSession.UserID);
			}
			else
			{
				logger.Fatal(global::Kampai.Util.FatalCode.CMD_LOAD_PLAYER, "No user session");
			}
		}

		private void LoadCurrentSocialTeam()
		{
			global::Kampai.Game.TimedSocialEventDefinition currentSocialEvent = socialEventService.GetCurrentSocialEvent();
			if (currentSocialEvent != null)
			{
				global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse>();
				socialEventService.GetSocialEventState(currentSocialEvent.ID, resultSignal);
			}
		}

		private void OnPlayerLoaded(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			logger.Debug("LoadPlayerCommand: Received player data with response code {0}", response.Code);
			global::Kampai.Util.TimeProfiler.EndSection("load player");
			string empty = string.Empty;
			PlayerMetaData playerMetaData = new PlayerMetaData();
			if (!response.Success)
			{
				int code = response.Code;
				if (code != 404)
				{
					if (!retried)
					{
						retried = true;
						logger.Error("OnPlayerLoaded failed with response code {0}", code);
						networkConnectionLostSignal.Dispatch();
					}
					else
					{
						logger.Fatal(global::Kampai.Util.FatalCode.GS_ERROR_LOAD_PLAYER, "Response code {0}", code);
					}
					return;
				}
				empty = defService.GetInitialPlayer();
			}
			else
			{
				global::System.Collections.Generic.IDictionary<string, string> headers = response.Headers;
				if (headers.ContainsKey("X-Kampai-Cumulative"))
				{
					float result = 0f;
					if (float.TryParse(headers["X-Kampai-Cumulative"], out result))
					{
						playerMetaData.USD = (int)result;
					}
				}
				if (headers.ContainsKey("X-Kampai-Segments"))
				{
					playerMetaData.Segments = headers["X-Kampai-Segments"];
				}
				if (headers.ContainsKey("X-Kampai-Churn"))
				{
					playerMetaData.Churn = headers["X-Kampai-Churn"];
					logger.Info("churn={0}", playerMetaData.Churn);
				}
				empty = response.Body;
				if (string.IsNullOrEmpty(empty))
				{
					logger.Fatal(global::Kampai.Util.FatalCode.PS_EMPTY_SERVER_JSON);
					return;
				}
				if (global::Kampai.Util.OfflineModeUtility.HasLocalSave())
				{
					string localData = global::Kampai.Util.OfflineModeUtility.LoadLocal(global::Kampai.Util.OfflineModeUtility.PlayerSavePath);
					string latestData = global::Kampai.Util.OfflineModeUtility.GetLatestSave(empty, localData);
					empty = latestData;
				}
				// DEBUG: log what HasEnded value is in the final JSON before deserialization
				{
					int idx = empty.IndexOf("\"HasEnded\"", global::System.StringComparison.OrdinalIgnoreCase);
					string snippet = idx >= 0 ? empty.Substring(idx, global::System.Math.Min(30, empty.Length - idx)) : "(not found)";
					global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] LoadPlayerCommand: JSON HasEnded snippet before dispatch: {0}", snippet);
				}
				if (!string.IsNullOrEmpty(empty))
				{
					global::Kampai.Util.OfflineModeUtility.SaveLocal(global::Kampai.Util.OfflineModeUtility.PlayerSavePath, empty);
				}
			}
			loadedPlayerDataSignal.Dispatch(empty, playerMetaData);
		}
	}
}
