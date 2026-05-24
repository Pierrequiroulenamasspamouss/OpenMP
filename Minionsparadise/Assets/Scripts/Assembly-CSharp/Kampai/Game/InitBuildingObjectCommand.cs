namespace Kampai.Game
{
	public class InitBuildingObjectCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("InitBuildingObjectCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.View.BuildingObject buildingObject { get; set; }

		[Inject]
		public global::Kampai.Game.Building building { get; set; }

		[Inject]
		public global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> animatorControllers { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Util.PathFinder pathFinder { get; set; }

		public override void Execute()
		{
			global::Kampai.Game.View.ActionableObject component = buildingObject.GetComponent<global::Kampai.Game.View.ActionableObject>();
			if (component != null)
			{
				component.fmodService = fmodService;
				component.playerService = playerService;
				global::Kampai.Game.View.MignetteBuildingObject mignette = component as global::Kampai.Game.View.MignetteBuildingObject;
				if (mignette != null)
				{
					mignette.pathFinder = pathFinder;
				}
			}
			buildingObject.Init(building, logger, animatorControllers, definitionService);
		}
	}
}
