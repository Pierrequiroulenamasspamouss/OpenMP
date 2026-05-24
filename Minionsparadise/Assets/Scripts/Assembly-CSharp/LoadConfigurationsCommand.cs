public class LoadConfigurationsCommand : global::strange.extensions.command.impl.Command
{
	public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadConfigurationsCommand") as global::Kampai.Util.IKampaiLogger;

	private global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> downloadResponse = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();

	[Inject]
	public bool init { get; set; }

	[Inject]
	public global::Kampai.Splash.IDownloadService downloadService { get; set; }

	[Inject]
	public global::Kampai.Game.IConfigurationsService ConfigurationsService { get; set; }

	[Inject]
	public ILocalPersistanceService localPersistService { get; set; }

	[Inject]
	public global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequestFactory requestFactory { get; set; }

	[Inject]
	public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

	[Inject]
	public IResourceService resourceService { get; set; }

	public override void Execute()
	{
		global::Kampai.Util.StartupTimer.LogCheckpoint("LoadConfigurationsCommand.Execute");
		logger.EventStart("LoadConfigurationsCommand.Execute");
		logger.Info("Executing LoadConfigurationsCommand:{0}", init);
		global::Kampai.Util.TimeProfiler.StartSection("retrieve config");
		string configURL = ConfigurationsService.GetConfigURL();
		logger.Info("ClientConfigUrl: {0}", configURL);
		localPersistService.PutData("configURL", configURL);
		ConfigurationsService.setInitonCallback(init);

		if (userSessionService.IsOffline)
		{
			ConfigurationsService.LoadLocalConfiguration();
			logger.EventStop("LoadConfigurationsCommand.Execute");
			return;
		}

		downloadResponse.AddListener(ConfigurationsService.GetConfigurationCallback);
		downloadService.Perform(requestFactory.Resource(configURL).WithResponseSignal(downloadResponse).WithRetry());
		logger.EventStop("LoadConfigurationsCommand.Execute");
	}
}
