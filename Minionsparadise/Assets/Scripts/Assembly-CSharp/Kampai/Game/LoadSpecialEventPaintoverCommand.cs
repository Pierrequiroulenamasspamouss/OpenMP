namespace Kampai.Game
{
	public class LoadSpecialEventPaintoverCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadSpecialEventPaintoverCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.SpecialEventItemDefinition specialEventItemDefinition { get; set; }

		[Inject(global::Kampai.Game.GameElement.SPECIAL_EVENT_PARENT)]
		public global::UnityEngine.GameObject parent { get; set; }

		public override void Execute()
		{
			string paintover = specialEventItemDefinition.Paintover;
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(paintover);
			if (gameObject == null)
			{
				logger.Debug("Unable to load Special_Event paintover prefab");
				return;
			}
			global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
			if (gameObject2 == null)
			{
				logger.Debug("Unable to instantiate Special_Event paintover object");
			}
			else
			{
				gameObject2.transform.parent = parent.transform;
			}
		}
	}
}
