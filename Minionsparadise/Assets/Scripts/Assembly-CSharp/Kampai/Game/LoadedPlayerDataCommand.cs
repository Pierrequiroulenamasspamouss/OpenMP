namespace Kampai.Game
{
	public class LoadedPlayerDataCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadedPlayerDataCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public string playerJSON { get; set; }

		[Inject]
		public PlayerMetaData meta { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerDurationService playerDurationService { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistService { get; set; }

		[Inject]
		public global::Kampai.Game.IDLCService dlcService { get; set; }

		[Inject]
		public global::Kampai.Main.MainCompleteSignal completeSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ReloadGameSignal reloadGameSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Common.ISwrveService swrveService { get; set; }

		public override void Execute()
		{
			global::UnityEngine.Debug.LogFormat("[DEBUG] LoadedPlayerDataCommand.Execute starting: playerJSON null={0}, len={1}", playerJSON == null, playerJSON != null ? playerJSON.Length : 0);
			logger.Info("LoadedPlayerDataCommand.Execute: playerJSON len={0}", playerJSON != null ? playerJSON.Length : -1);
			global::Kampai.Util.StartupTimer.LogCheckpoint("LoadedPlayerDataCommand.Execute");
			string text = localPersistService.GetData("LoadMode");
			if (text == "remote")
			{
				if (string.IsNullOrEmpty(playerJSON))
				{
					logger.Fatal(global::Kampai.Util.FatalCode.GS_ERROR_FETCHING_PLAYER_DATA);
				}
			}
			else if (text == "externalLogin")
			{
				if (!string.IsNullOrEmpty(playerJSON))
				{
					reloadGameSignal.Dispatch();
					return;
				}
				logger.Fatal(global::Kampai.Util.FatalCode.GS_ERROR_FETCHING_PLAYER_DATA);
			}
			this.telemetryService.Send_Telemetry_EVT_USER_GAME_LOAD_FUNNEL("90 - Loaded Player Data", playerService.SWRVEGroup, dlcService.GetDownloadQualityLevel());
			if (playerJSON.Length != 0)
			{
				if (!DeserializePlayerData(playerJSON))
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "DeserializingPlayerData returned false, recovering with fallback player instance.");
					if (playerService != null)
					{
						string initPlayer = (localPersistService != null) ? localPersistService.GetData("initialPlayer") : null;
						playerService.Deserialize(initPlayer, true);
					}
				}
				global::Kampai.Common.TelemetryService telemetryService = this.telemetryService as global::Kampai.Common.TelemetryService;
				if (telemetryService != null)
				{
					telemetryService.SetPlayerServiceReference(playerService);
					telemetryService.SetPlayerDurationServiceReference(playerDurationService);
				}
				global::Kampai.Common.SwrveService swrveService = this.swrveService as global::Kampai.Common.SwrveService;
				if (swrveService != null)
				{
					swrveService.SetPlayerServiceReference(playerService);
				}
				else
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "SwrveService was not setup properly");
				}
				if (this.swrveService != null)
				{
					this.swrveService.SendUserStatsUpdate();
				}
				completeSignal.Dispatch();
			}
			else
			{
				logger.FatalNoThrow(global::Kampai.Util.FatalCode.CMD_LOAD_PLAYER);
			}
		}

		private bool DeserializePlayerData(string json)
		{
			try
			{
				global::Kampai.Util.TimeProfiler.StartSection("read player");
				playerService.Deserialize(json);
				if (meta != null)
				{
					playerService.IngestPlayerMeta(meta);
				}
				global::Kampai.Util.TimeProfiler.EndSection("read player");
				return playerService != null && playerService.IsPlayerInitialized();
			}
			catch (global::System.Exception ex2)
			{
				global::UnityEngine.Debug.LogErrorFormat("[DEBUG] LoadedPlayerDataCommand.DeserializePlayerData EXCEPTION: {0}\n{1}", ex2.Message, ex2.StackTrace);
				logger.Error("Unexpected player deser-n error. Json: {0}, Exception: {1}", json, ex2);
			}
			return playerService != null && playerService.IsPlayerInitialized();
		}
	}
}
