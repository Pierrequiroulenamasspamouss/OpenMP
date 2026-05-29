namespace Kampai.Game
{
	internal sealed class DefinitionsChangedCommand : global::strange.extensions.command.impl.Command
	{
		private const string lowestQualityName = "VeryLow";

		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DefinitionsChangedCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public bool hotSwap { get; set; }

		[Inject]
		public global::Kampai.Util.IMinionBuilder minionBuilder { get; set; }

		[Inject]
		public global::Kampai.Game.IDLCService dlcService { get; set; }

		[Inject]
		public global::Kampai.Game.LoadPlayerSignal loadPlayerSignal { get; set; }

		[Inject]
		public global::Kampai.Game.LoadMarketplaceOverridesSignal loadMarketplaceOverridesSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistanceService { get; set; }

		[Inject]
		public global::Kampai.Game.DefinitionsHotSwapCompleteSignal definitionsHotSwapCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.Common.DLCLevelChangedSignal dlcLevelChangedSignal { get; set; }

		public override void Execute()
		{
			string defUrl = localPersistanceService.GetData("DefinitionsUrl");
			if (string.IsNullOrEmpty(defUrl)) {
				defUrl = "local://Assets/Resources/definitions.json"; // Fallback to avoid error
			}
			configurationsService.GetConfigurations().definitions = defUrl;
			logger.Info("DefinitionsChangedCommand:: Definitions URL: {0}", configurationsService.GetConfigurations().definitions);
			global::Kampai.Util.TargetPerformance lOD = (global::Kampai.Util.TargetPerformance)(int)global::System.Enum.Parse(typeof(global::Kampai.Util.TargetPerformance), dlcService.GetDownloadQualityLevel().ToUpper());
			minionBuilder.SetLOD(lOD);
			int num = -1;
			int num2 = 0;
			string[] names = global::UnityEngine.QualitySettings.names;
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (text.Equals(dlcService.GetDownloadQualityLevel(), global::System.StringComparison.OrdinalIgnoreCase))
				{
					num = i;
				}
				if (text.Equals("VeryLow", global::System.StringComparison.OrdinalIgnoreCase))
				{
					num2 = i;
				}
			}
			if (num < 0)
			{
				num = num2;
			}
			global::UnityEngine.QualitySettings.SetQualityLevel(num);
			global::Kampai.Util.QualityHelper.applyAntiAliasing(lOD);
			dlcLevelChangedSignal.Dispatch();
			if (!hotSwap)
			{
				loadPlayerSignal.Dispatch();
				loadMarketplaceOverridesSignal.Dispatch();
			}
			else
			{
				definitionsHotSwapCompleteSignal.Dispatch(base.injectionBinder as global::strange.extensions.injector.api.ICrossContextInjectionBinder);
			}
			logger.Info("DefinitionsChangedCommand: Execution complete (HotSwap: {0})", hotSwap);
		}
	}
}
