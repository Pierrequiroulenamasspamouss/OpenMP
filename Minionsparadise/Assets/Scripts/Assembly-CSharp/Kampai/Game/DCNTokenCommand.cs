namespace Kampai.Game
{
	public class DCNTokenCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DCNTokenCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.IDCNService dcnService { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationService { get; set; }

		[Inject]
		public global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequestFactory requestFactory { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		public override void Execute()
		{
			if (userSessionService.IsOffline)
			{
				logger.Info("DCN disabled because client is in offline mode");
			}
			else if (configurationService.isKillSwitchOn(global::Kampai.Game.KillSwitch.DCN))
			{
				logger.Info("DCN disabled by killswitch");
			}
			else
			{
				dcnService.Perform(Request, true);
			}
		}

		private global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest Request()
		{
			string uri = string.Format("{0}{1}", global::Kampai.Util.GameConstants.DCN.SERVER, "/token");
			string s = string.Format("{{ \"{0}\": \"{1}\" }}", "app_token", global::Kampai.Util.GameConstants.DCN.APP_TOKEN);
			byte[] bytes = global::System.Text.Encoding.UTF8.GetBytes(s);
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(Response);
			return requestFactory.Resource(uri).WithMethod("POST").WithContentType("application/json")
				.WithBody(bytes)
				.WithResponseSignal(signal);
		}

		private void Response(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			if (response == null)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "DCNTokenCommand response is null");
			}
			else if (!response.Success)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, string.Format("DCNTokenCommand failed with response code: {0}", response.Code));
			}
			else
			{
				Deserialize(response.Body);
			}
		}

		private void Deserialize(string json)
		{
			global::Kampai.Game.DCNToken dCNToken = null;
			try
			{
				dCNToken = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.DCNToken>(json);
			}
			catch (global::Newtonsoft.Json.JsonSerializationException e)
			{
				HandleJsonException(e);
			}
			catch (global::Newtonsoft.Json.JsonReaderException e2)
			{
				HandleJsonException(e2);
			}
			if (dCNToken == null)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Error: token is null");
			}
			else
			{
				dcnService.SetToken(dCNToken);
			}
		}

		private void HandleJsonException(global::System.Exception e)
		{
			logger.Error("[Error]\n{0}", e.Message);
			logger.Error("[StackTrace]\n{0}", e.StackTrace);
		}
	}
}
