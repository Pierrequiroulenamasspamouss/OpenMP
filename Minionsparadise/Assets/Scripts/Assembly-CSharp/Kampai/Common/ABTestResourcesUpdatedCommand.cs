namespace Kampai.Common
{
	public class ABTestResourcesUpdatedCommand : global::strange.extensions.command.impl.Command
	{
		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("ABTestResourcesUpdatedCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public bool resourcesUpdateSucceed { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		[Inject]
		public global::Kampai.Game.FetchDefinitionsSignal fetchDefinitionsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.LoadDefinitionsSignal loadDefinitionsSignal { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistanceService { get; set; }

		public override void Execute()
		{
			TryFetchDefinitionsBasedOnCurrentABTest();
		}

		private void TryFetchDefinitionsBasedOnCurrentABTest()
		{
			logger.Debug("FetchDefinitionsBasedOnCurrentABTest(): resourcesUpdateSucceed = {0}", resourcesUpdateSucceed);
			string definitionsToFetchUrl;
			bool isOffline = configurationsService is global::Kampai.Game.ConfigurationsService && (configurationsService as global::Kampai.Game.ConfigurationsService).userSessionService.IsOffline;
			
			if (NeedFetchDefinitions(out definitionsToFetchUrl) || (isOffline && !global::System.IO.File.Exists(FetchDefinitionsCommand.GetDefinitionsPath())))
			{
				logger.Debug("FetchDefinitionsBasedOnCurrentABTest(): fetch definitions, definitionsToFetchUrl = {0}", definitionsToFetchUrl);
				if (configurationsService.GetConfigurations() != null)
				{
					configurationsService.GetConfigurations().definitions = definitionsToFetchUrl;
				}
				fetchDefinitionsSignal.Dispatch(configurationsService.GetConfigurations());
			}
			else
			{
				logger.Debug("FetchDefinitionsBasedOnCurrentABTest(): definitions are up to date, load definitions.");
				LoadDefinitionsCommand.LoadDefinitionsData loadDefinitionsData = new LoadDefinitionsCommand.LoadDefinitionsData();
				loadDefinitionsData.Path = FetchDefinitionsCommand.GetDefinitionsPath();
				loadDefinitionsSignal.Dispatch(false, loadDefinitionsData);
			}
			logger.Debug("ABTestResourcesUpdatedCommand:TryFetchDefinitionsBasedOnCurrentABTest should be either fetching or loading definitions now");
		}

		private bool NeedFetchDefinitions(out string definitionsToFetchUrl)
		{
			string value = localPersistanceService.GetData("DefinitionsUrl");
			if (!global::System.IO.File.Exists(FetchDefinitionsCommand.GetDefinitionsPath()))
			{
				value = null;
			}
			string finalDefinitionsUrl = GetFinalDefinitionsUrl();
			if (!string.IsNullOrEmpty(finalDefinitionsUrl) && !finalDefinitionsUrl.Equals(value))
			{
				definitionsToFetchUrl = finalDefinitionsUrl;
				return true;
			}
			definitionsToFetchUrl = null;
			return false;
		}

		private string GetFinalDefinitionsUrl()
		{
			string result = null;
			global::Kampai.Game.ConfigurationDefinition configurations = configurationsService.GetConfigurations();
			if (configurations != null && configurations.definitions != null)
			{
				result = configurations.definitions;
			}
			string aBTestDefinitionsUrl = GetABTestDefinitionsUrl();
			if (!string.IsNullOrEmpty(aBTestDefinitionsUrl))
			{
				result = aBTestDefinitionsUrl;
			}
			return result;
		}

		private string GetABTestDefinitionsUrl()
		{
			return (!global::Kampai.Util.ABTestModel.abtestEnabled) ? null : ((global::Kampai.Util.ABTestModel.definitionURL == null) ? string.Format("{0}/{1}", configurationsService.GetConfigurations().definitions, global::Kampai.Util.ABTestModel.configurationVariant) : global::Kampai.Util.ABTestModel.definitionURL);
		}
	}
}
