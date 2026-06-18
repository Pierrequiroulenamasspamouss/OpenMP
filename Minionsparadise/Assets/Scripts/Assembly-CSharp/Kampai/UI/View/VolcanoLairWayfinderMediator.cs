namespace Kampai.UI.View
{
	public class VolcanoLairWayfinderMediator : global::Kampai.UI.View.AbstractWayFinderMediator
	{
		private global::Kampai.Game.MasterPlanComponentTaskUpdatedSignal taskUpdatedSignal;

		private global::Kampai.Game.SetMasterPlanWayfinderIconToCompleteSignal setCompleteIconSignal;

		[Inject]
		public global::Kampai.UI.View.VolcanoLairWayfinderView view { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IMasterPlanService planService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeAllOtherMenusSignal { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.UI.View.ResetLairWayfinderIconSignal resetIconSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideFluxWayfinder hideWayfinder { get; set; }

		[Inject]
		public global::Kampai.Game.VillainLairModel villainLairModel { get; set; }

		public override global::Kampai.UI.View.IWayFinderView View
		{
			get
			{
				return view;
			}
		}

		public override void OnRegister()
		{
			base.OnRegister();
			hideWayfinder.AddListener(ShowWayfinder);
			resetIconSignal.AddListener(view.ResetDefaultIcon);
			taskUpdatedSignal = gameContext.injectionBinder.GetInstance<global::Kampai.Game.MasterPlanComponentTaskUpdatedSignal>();
			setCompleteIconSignal = gameContext.injectionBinder.GetInstance<global::Kampai.Game.SetMasterPlanWayfinderIconToCompleteSignal>();
			taskUpdatedSignal.AddListener(view.TaskUpdated);
			setCompleteIconSignal.AddListener(view.SetBuildReadyIcon);
			planService.SetWayfinderState();
			view.SetOffset();
			if (villainLairModel.currentActiveLair == null)
			{
				view.SetForceHide(true);
			}
		}

		public override void OnRemove()
		{
			base.OnRemove();
			hideWayfinder.RemoveListener(ShowWayfinder);
			resetIconSignal.RemoveListener(view.ResetDefaultIcon);
			taskUpdatedSignal.RemoveListener(view.TaskUpdated);
			taskUpdatedSignal = null;
			setCompleteIconSignal.RemoveListener(view.SetBuildReadyIcon);
			setCompleteIconSignal = null;
		}

		protected override void GoToClicked()
		{
			if (!base.pickModel.PanningCameraBlocked)
			{
				closeAllOtherMenusSignal.Dispatch(null);
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.DetermineLairUISignal>().Dispatch();
			}
		}

		private void ShowWayfinder(bool enabled)
		{
			if (playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.VillainLair>(3137).hasVisited)
			{
				if (villainLairModel.currentActiveLair == null)
				{
					view.SetForceHide(true);
				}
				else
				{
					view.SetForceHide(enabled);
				}
			}
		}
	}
}
