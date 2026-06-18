namespace Kampai.Tools.AnimationToolKit
{
	public class LoadAnimationToolKitCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadAnimationToolKitCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject(global::strange.extensions.context.api.ContextKeys.CONTEXT_VIEW)]
		public global::UnityEngine.GameObject ContextView { get; set; }

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Game.LoadDefinitionsSignal LoadDefinitionsSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.LoadCameraSignal LoadCameraSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.LoadCanvasSignal LoadCanvasSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.LoadEventSystemSignal LoadEventSystemSignal { get; set; }

		[Inject]
		public global::Kampai.Common.SetupManifestSignal setupManifestSignal { get; set; }

		public override void Execute()
		{
			global::Kampai.Util.DeviceCapabilities.Initialize();
			LoadDevDefinitions();
			LoadCameraSignal.Dispatch();
			LoadCanvasSignal.Dispatch();
			LoadEventSystemSignal.Dispatch();
			setupManifestSignal.Dispatch();
			LoadFMOD();
			LoadMinionGroup();
			LoadVillainGroup();
			LoadCharacterGroup();
			base.injectionBinder.GetInstance<global::Kampai.Tools.AnimationToolKit.AnimationToolKit>();
		}

		private void LoadDevDefinitions()
		{
			global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load("dev_definitions") as global::UnityEngine.TextAsset;
			if (textAsset != null && textAsset.text != null)
			{
				LoadDefinitionsCommand.LoadDefinitionsData loadDefinitionsData = new LoadDefinitionsCommand.LoadDefinitionsData();
				loadDefinitionsData.Json = textAsset.text;
				LoadDefinitionsSignal.Dispatch(false, loadDefinitionsData);
			}
			else
			{
				logger.Debug("Unable to load dev_definitions.json from Resources.");
			}
		}

		private void LoadFMOD()
		{
			routineRunner.StartCoroutine(fmodService.InitializeSystem());
			global::UnityEngine.Camera main = global::UnityEngine.Camera.main;
			main.gameObject.AddComponent<global::FMODUnity.StudioListener>();
			global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find("FMOD_StudioSystem");
			gameObject.transform.parent = ContextView.transform;
			global::UnityEngine.GameObject gameObject2 = new global::UnityEngine.GameObject("EnvironmentAudioManager");
			EnvironmentAudioManagerView environmentAudioManagerView = gameObject2.AddComponent<EnvironmentAudioManagerView>();
			environmentAudioManagerView.mainCamera = global::UnityEngine.Camera.main;
			gameObject2.transform.localPosition = global::UnityEngine.Vector3.zero;
			gameObject2.transform.parent = main.transform;
		}

		private void LoadMinionGroup()
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Minions");
			gameObject.transform.parent = ContextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Tools.AnimationToolKit.AnimationToolKitElement.MINIONS);
		}

		private void LoadVillainGroup()
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Villains");
			gameObject.transform.parent = ContextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Tools.AnimationToolKit.AnimationToolKitElement.VILLAINS);
		}

		private void LoadCharacterGroup()
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Characters");
			gameObject.transform.parent = ContextView.transform;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Tools.AnimationToolKit.AnimationToolKitElement.CHARACTERS);
		}
	}
}
