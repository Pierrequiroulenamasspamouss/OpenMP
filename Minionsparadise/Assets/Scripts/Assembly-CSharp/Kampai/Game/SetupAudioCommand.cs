namespace Kampai.Game
{
	public class SetupAudioCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("SetupAudioCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject(global::strange.extensions.context.api.ContextKeys.CONTEXT_VIEW)]
		public global::UnityEngine.GameObject contextView { get; set; }

		[Inject(global::Kampai.Main.MainElement.MANAGER_PARENT)]
		public global::UnityEngine.GameObject managers { get; set; }

		[Inject(global::Kampai.Main.MainElement.CAMERA)]
		public global::UnityEngine.Camera camera { get; set; }

		public override void Execute()
		{
			logger.EventStart("SetupAudioCommand.Execute");
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("PositionalAudioListener");
			gameObject.AddComponent<global::Kampai.Game.View.Audio.PositionalAudioListenerView>();
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Main.MainElement.AUDIO_LISTENER)
				.CrossContext();
			gameObject.transform.parent = contextView.transform;
			gameObject.gameObject.AddComponent<global::FMODUnity.StudioListener>();
			global::UnityEngine.GameObject gameObject2 = global::FMOD_StudioSystem.instance.gameObject;
			gameObject2.transform.parent = managers.transform;
			global::UnityEngine.GameObject gameObject3 = new global::UnityEngine.GameObject("EnvironmentAudioManager");
			gameObject3.AddComponent<EnvironmentAudioManagerView>();
			gameObject3.transform.localPosition = new global::UnityEngine.Vector3(0f, 0f, 0f);
			gameObject3.transform.parent = camera.transform;
			global::UnityEngine.GameObject gameObject4 = new global::UnityEngine.GameObject("VolcanoEmitter");
			gameObject4.transform.SetParent(camera.transform.parent, false);
			gameObject4.transform.position = new global::UnityEngine.Vector3(150f, 0f, 205f);
			global::Kampai.Game.View.EnvironmentAudioEmitterView environmentAudioEmitterView = gameObject4.AddComponent<global::Kampai.Game.View.EnvironmentAudioEmitterView>();
			environmentAudioEmitterView.AudioName = "Play_volcano_lava_01";
			logger.EventStop("SetupAudioCommand.Execute");
		}
	}
}
