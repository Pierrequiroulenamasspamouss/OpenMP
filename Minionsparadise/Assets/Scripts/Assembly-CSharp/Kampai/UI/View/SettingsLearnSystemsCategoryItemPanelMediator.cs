namespace Kampai.UI.View
{
	public class SettingsLearnSystemsCategoryItemPanelMediator : global::strange.extensions.mediation.impl.Mediator
	{
		[Inject]
		public global::Kampai.UI.View.SettingsLearnSystemsCategoryItemPanelView view { get; set; }

		[Inject]
		public global::Kampai.UI.View.SettingsLearnSystemsCategorySelectedSignal categorySelectedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SettingsLearnSystemsCategoryItemSelectedSignal categoryItemSelectedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }

		[Inject]
		public global::Kampai.UI.IPlayerTrainingService playerTrainingService { get; set; }

		[Inject]
		public global::Kampai.Game.DisplayPlayerTrainingSignal displayPlayerTrainingSignal { get; set; }

		public override void OnRegister()
		{
			categorySelectedSignal.AddListener(OnCategorySelected);
			categoryItemSelectedSignal.AddListener(OnCategoryItemSelected);
		}

		public override void OnRemove()
		{
			if (categorySelectedSignal != null)
			{
				categorySelectedSignal.RemoveListener(OnCategorySelected);
			}
			if (categoryItemSelectedSignal != null)
			{
				categoryItemSelectedSignal.RemoveListener(OnCategoryItemSelected);
			}
		}

		private void OnCategorySelected(int categoryDefinitionId)
		{
			global::Kampai.Game.PlayerTrainingCategoryDefinition playerTrainingCategoryDefinition = definitionService.Get<global::Kampai.Game.PlayerTrainingCategoryDefinition>(categoryDefinitionId);
			if (playerTrainingCategoryDefinition == null)
			{
				return;
			}
			view.ClearPlayerTraining();
			foreach (int trainingDefinitionID in playerTrainingCategoryDefinition.trainingDefinitionIDs)
			{
				if (trainingDefinitionID == 19000022 || trainingDefinitionID == 19000016)
				{
					continue;
				}
				global::Kampai.Game.PlayerTrainingDefinition definition = definitionService.Get<global::Kampai.Game.PlayerTrainingDefinition>(trainingDefinitionID);
				view.AddPlayerTraining(definition, localizationService, playerTrainingService.HasSeen(trainingDefinitionID, global::Kampai.UI.PlayerTrainingVisiblityType.SETTINGS));
			}
		}

		private void OnCategoryItemSelected(int categoryItemDefinitionId)
		{
			playerTrainingService.MarkSeen(categoryItemDefinitionId, global::Kampai.UI.PlayerTrainingVisiblityType.SETTINGS);
			global::Kampai.Game.PlayerTrainingDefinition playerTrainingDefinition = definitionService.Get<global::Kampai.Game.PlayerTrainingDefinition>(categoryItemDefinitionId);
			if (playerTrainingDefinition != null)
			{
				view.AddPlayerTraining(playerTrainingDefinition, localizationService, playerTrainingService.HasSeen(categoryItemDefinitionId, global::Kampai.UI.PlayerTrainingVisiblityType.SETTINGS));
			}
			displayPlayerTrainingSignal.Dispatch(categoryItemDefinitionId, true, new global::strange.extensions.signal.impl.Signal<bool>());
		}
	}
}
