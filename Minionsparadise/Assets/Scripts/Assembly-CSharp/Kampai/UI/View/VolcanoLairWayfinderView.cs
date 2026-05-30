namespace Kampai.UI.View
{
	public class VolcanoLairWayfinderView : global::Kampai.UI.View.AbstractWayFinderView
	{
		protected override string UIName
		{
			get
			{
				return "VolcanoLairWayFinder";
			}
		}

		protected override string WayFinderDefaultIcon
		{
			get
			{
				return wayFinderDefinition.KevinLairIcon;
			}
		}

		internal void SetOffset()
		{
			UIOffset = new global::UnityEngine.Vector3(0f, 0.5f, 0f);
		}

		protected override bool OnCanUpdate()
		{
			if (zoomCameraModel.ZoomedIn)
			{
				return false;
			}
			if (gameContext != null)
			{
				global::Kampai.Game.VillainLairModel instance = gameContext.injectionBinder.GetInstance<global::Kampai.Game.VillainLairModel>();
				if (instance == null || instance.currentActiveLair == null)
				{
					return false;
				}
			}
			return true;
		}

		internal void TaskUpdated(global::Kampai.Game.MasterPlanComponent component)
		{
			bool flag = true;
			bool flag2 = false;
			for (int i = 0; i < component.tasks.Count; i++)
			{
				global::Kampai.Game.MasterPlanComponentTask masterPlanComponentTask = component.tasks[i];
				if (masterPlanComponentTask.isHarvestable)
				{
					flag2 = true;
				}
				if (!masterPlanComponentTask.isComplete)
				{
					flag = false;
				}
			}
			if (flag && component.State <= global::Kampai.Game.MasterPlanComponentState.TasksComplete)
			{
				SetBuildReadyIcon();
			}
			else if (flag2)
			{
				UpdateIcon(wayFinderDefinition.MasterPlanComponentTaskCompleteIcon);
			}
			else
			{
				ResetDefaultIcon();
			}
		}

		internal void SetBuildReadyIcon()
		{
			UpdateIcon(wayFinderDefinition.MasterPlanComponentCompleteIcon);
		}

		internal void ResetDefaultIcon()
		{
			UpdateIcon(wayFinderDefinition.KevinLairIcon);
		}
	}
}
