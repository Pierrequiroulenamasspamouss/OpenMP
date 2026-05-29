namespace Kampai.Game.Mignette.WaterSlide.View
{
	public class WaterSlideMignetteManagerView : global::Kampai.Game.Mignette.View.MignetteManagerView
	{
		private const float MUSIC_PROGRESSION_INCREMENTER = 5f;

		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("WaterSlideMignetteManagerView") as global::Kampai.Util.IKampaiLogger;

		private WaterSlideBuildingViewObject buildingViewReference;

		private PathAgent pathRider;

		private global::Kampai.Game.View.MinionObject minionObject;

		private global::UnityEngine.Transform minionTransform;

		private global::UnityEngine.Animator minionAnimator;

		private global::UnityEngine.Transform minionParent;

		private global::UnityEngine.GameObject minionGO;

		private bool rollDiveRoulette;

		private int diveIndex;

		private bool diveSelected;

		private float diveTotalTime = 2f;

		private CustomFMOD_StudioEventEmitter waterEmitter;

		public bool isGameOver;

		private WaterslideSpinnerViewObject spinnerViewObject;

		private global::System.Collections.Generic.Dictionary<int, int> diveScoreMap = new global::System.Collections.Generic.Dictionary<int, int>();

		internal global::UnityEngine.GameObject mignetteDooberGO;

		private float diveElapsedTime;

		private bool introSequencePlaying;

		[Inject]
		public global::Kampai.Game.StopMignetteSignal stopMignette { get; set; }

		[Inject]
		public global::Kampai.UI.View.SpawnMignetteDooberSignal spawnMignetteDooberSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.ChangeMignetteScoreSignal changeScoreSignal { get; set; }

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.WaterSlide.WaterSlideMignettePathCompletedSignal pathCompletedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.WaterSlide.WaterslideMignetteOnDiveTriggerSignal diveTriggerSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.WaterSlide.WaterslideMignetteOnPlayDiveTriggerSignal diveAnimationSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal playGlobalAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal localAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.RequestStopMignetteSignal requestStopMignetteSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		protected override void Start()
		{
			base.Start();
			buildingViewReference = MignetteBuildingObject.GetComponent<WaterSlideBuildingViewObject>();
			if (buildingViewReference == null)
			{
				logger.Error("Couldn't find building ref component");
				return;
			}
			if (!TryInitializeScoreValues())
			{
				logger.Error("Couldn't initialize the score values");
				return;
			}
			InititalizePathRider();
			InititalizeMinion();
			stopMignette.AddListener(OnGameOver);
			pathCompletedSignal.AddListener(OnPathComplete);
			diveTriggerSignal.AddListener(OnDisplayDiveButton);
			diveAnimationSignal.AddListener(OnPlayDiveAnimation);
			pathRider.pathCompletedSignal = pathCompletedSignal;
			pathRider.diveTriggerSignal = diveTriggerSignal;
			pathRider.playDiveAnimation = diveAnimationSignal;
			SaveCurrentCameraPosition();
			SetCameraMoveTime(1f);
			StartCoroutine(StartAnimationSequence());
			TotalEventTime = diveTotalTime;
			spinnerViewObject = GetComponentInChildren<WaterslideSpinnerViewObject>();
			spinnerViewObject.transform.parent = base.mignetteCamera.transform;
			spinnerViewObject.transform.localPosition = buildingViewReference.SpinnerBallOffset;
			spinnerViewObject.transform.forward = base.mignetteCamera.transform.forward;
			spinnerViewObject.gameObject.SetActive(false);
			waterEmitter = base.gameObject.AddComponent<CustomFMOD_StudioEventEmitter>();
			waterEmitter.shiftPosition = false;
			waterEmitter.staticSound = false;
			waterEmitter.path = fmodService.GetGuid("Play_minion_ski_01");
			waterEmitter.startEventOnAwake = false;
		}

		private void OnPlayDiveAnimation()
		{
			minionObject.SetAnimInteger("InDive", diveIndex + 1);
		}

		private void InititalizeMinion()
		{
			minionObject = MignetteBuildingObject.GetChildMinion(0).Minion;
			minionGO = minionObject.gameObject;
			minionParent = minionGO.transform.parent;
			global::UnityEngine.Transform transform = minionGO.transform;
			transform.SetParent(pathRider.MinionHardpoint);
			transform.localPosition = global::UnityEngine.Vector3.zero;
			transform.localRotation = global::UnityEngine.Quaternion.Euler(global::UnityEngine.Vector3.zero);
			pathRider.MinionObject = minionObject;
			minionAnimator = minionGO.GetComponent<global::UnityEngine.Animator>();
			minionTransform = transform;
			minionObject.EnableBlobShadow(false);
		}

		private void InititalizePathRider()
		{
			PathController componentInChildren = GetComponentInChildren<PathController>();
			pathRider = GetComponentInChildren<PathAgent>();
			pathRider.View = this;
			pathRider.Path = componentInChildren;
		}

		private bool TryInitializeScoreValues()
		{
			if (buildingViewReference == null)
			{
				return false;
			}
			global::Kampai.Game.MignetteBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.MignetteBuilding>(MignetteBuildingObject.ID);
			if (byInstanceId == null)
			{
				return false;
			}
			global::Kampai.Game.MignetteBuildingDefinition definition = byInstanceId.Definition;
			if (definition == null)
			{
				return false;
			}
			global::System.Collections.Generic.IList<MignetteRuleDefinition> mignetteRules = definition.MignetteRules;
			if (mignetteRules == null)
			{
				return false;
			}
			int count = mignetteRules.Count;
			int num = 4;
			if (count != num)
			{
				logger.Warning("Mismatch between number of dives and rule definitions.  Should there be a unique score for each dive?");
			}
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = mignetteRules[i].EffectAmount;
			}
			global::System.Array.Sort(array);
			for (int j = 0; j < num; j++)
			{
				if (j < array.Length)
				{
					diveScoreMap.Add(j, array[j]);
				}
			}
			return true;
		}

		private global::System.Collections.IEnumerator StartAnimationSequence()
		{
			yield return null;
			pathRider.CameraController.SetCamera(base.mignetteCamera);
			pathRider.BuildPath();
			global::UnityEngine.Transform pathRiderTransform = pathRider.transform;
			pathRiderTransform.position = buildingViewReference.ClimbPoint.transform.position;
			pathRiderTransform.rotation = buildingViewReference.ClimbPoint.transform.rotation;
			minionObject.PlayAnimation(global::UnityEngine.Animator.StringToHash("ClimbLadder"), 0, 0f);
			introSequencePlaying = true;
			pathRider.VFXManager.DisplayMinionWake(false);
		}

		public void EnableWaterAudio(bool enable)
		{
			if (enable)
			{
				if (waterEmitter.getPlaybackState() != global::FMOD.Studio.PLAYBACK_STATE.PLAYING)
				{
					waterEmitter.Play();
				}
			}
			else if (waterEmitter.getPlaybackState() == global::FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				waterEmitter.Stop();
			}
		}

		private void BeginGame()
		{
			waterEmitter.Play();
			pathRider.SetAtPathStart();
			pathRider.StartFollowPath();
		}

		public void OnDisplayDiveButton(bool startPlay)
		{
			if (startPlay && !isGameOver)
			{
				TimeElapsed = 0f;
				rollDiveRoulette = true;
				spinnerViewObject.gameObject.SetActive(true);
				spinnerViewObject.StartIntro(playGlobalAudioSignal);
			}
		}

		public void OnScreenTapped()
		{
			if (rollDiveRoulette && !isGameOver)
			{
				OnGameButtonPushed();
			}
		}

		private void OnGameButtonPushed(bool playerInput = true)
		{
			if (!rollDiveRoulette)
			{
				return;
			}
			rollDiveRoulette = false;
			float animationPct = spinnerViewObject.GetAnimationPct();
			diveIndex = (int)global::UnityEngine.Mathf.Lerp(0f, 4f, animationPct);
			if (!playerInput)
			{
				diveIndex = 0;
			}
			if (!diveSelected)
			{
				global::System.Action<int> action = delegate(int newScore)
				{
					soundFXSignal.Dispatch("Play_mignette_collect");
					spawnMignetteDooberSignal.Dispatch(mignetteHUD, minionGO.transform.position, newScore, true);
					changeScoreSignal.Dispatch(newScore);
				};
				int value = 0;
				if (diveScoreMap.TryGetValue(diveIndex, out value))
				{
					action(value);
				}
				else if (diveScoreMap.Count > 0)
				{
					action(diveScoreMap[0]);
				}
				spinnerViewObject.SelectDive(diveIndex);
			}
			diveSelected = true;
		}

		public override void Update()
		{
			base.Update();
			if (base.IsPaused || isGameOver)
			{
				return;
			}
			if (introSequencePlaying)
			{
				minionTransform.localPosition = global::UnityEngine.Vector3.zero;
				if (minionAnimator != null && minionAnimator.GetCurrentAnimatorStateInfo(0).IsName("IntroComplete"))
				{
					minionObject.PlayAnimation(global::UnityEngine.Animator.StringToHash("Sliding"), 0, 0f);
					introSequencePlaying = false;
					BeginGame();
				}
			}
			else if (rollDiveRoulette)
			{
				diveElapsedTime += global::UnityEngine.Time.deltaTime;
				if (diveElapsedTime <= diveTotalTime)
				{
					TimeElapsed += global::UnityEngine.Time.deltaTime;
				}
				else
				{
					OnGameButtonPushed(false);
				}
			}
		}

		protected override void OnDestroy()
		{
			if (minionGO != null)
			{
				minionGO.transform.parent = minionParent;
			}
			if (buildingViewReference != null && !playerService.HasPurchasedMinigamePack())
			{
				buildingViewReference.UpdateCooldownView(localAudioSignal, 0, 0f);
			}
			base.OnDestroy();
			global::UnityEngine.Object.Destroy(pathRider);
			global::UnityEngine.Object.Destroy(spinnerViewObject);
			stopMignette.RemoveListener(OnGameOver);
			diveTriggerSignal.RemoveListener(OnDisplayDiveButton);
			pathCompletedSignal.RemoveListener(OnPathComplete);
			diveAnimationSignal.RemoveListener(OnPlayDiveAnimation);
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		public void ResetMignetteObjects()
		{
			isGameOver = true;
			spinnerViewObject.gameObject.SetActive(false);
			global::UnityEngine.Camera cam = base.mignetteCamera;
			if (cam == null)
			{
				cam = global::UnityEngine.Camera.main;
			}
			if (cam != null)
			{
				Go.killAllTweensWithTarget(cam.transform);
			}
			if (minionObject != null)
			{
				minionObject.EnableBlobShadow(true);
			}
			if (mignetteDooberGO != null)
			{
				mignetteDooberGO = null;
			}
		}

		private void OnGameOver(bool isInterrupted)
		{
		}

		private void OnPathComplete()
		{
			requestStopMignetteSignal.Dispatch(true);
		}
	}
}
