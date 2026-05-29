namespace Kampai.Game.Mignette.AlligatorSkiing.View
{
	public class AlligatorSkiingMignetteManagerView : global::Kampai.Game.Mignette.View.MignetteManagerView
	{
		private const string FISHING_ANIM_STATE_NAME = "Fishing";

		private const string CAST_ANIM_TRIGGER_NAME = "Casting";

		private const string PULLIN_ANIM_TRIGGER_NAME = "OnPulledIn";

		private const string JUMP_ANIM_TRIGGER_NAME = "OnJump";

		private const string CRASH_ANIM_TRIGGER_NAME = "OnCrash";

		private const string STUMBLE_ANIM_TRIGGER_NAME = "OnStumble";

		private const float OBSTACLE_DEFAULT_PENALTY = 1.5f;

		private const float OBSTACLE_FAILUP_PENALTY = 0.25f;

		private const float MUSIC_PROGRESS_INCREMENTER = 5f;

		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("AlligatorSkiingMignetteManagerView") as global::Kampai.Util.IKampaiLogger;

		private AlligatorSkiingBuildingViewObject buildingViewReference;

		private AlligatorWaypointController waypointsController;

		private AlligatorAgent alligatorAgent;

		private global::UnityEngine.GameObject MinionShadow;

		private global::UnityEngine.Transform cameraTransform;

		private global::UnityEngine.Transform cameraMarker;

		private global::UnityEngine.Transform alligatorTransform;

		private global::UnityEngine.Transform minionParentTransform;

		private bool isGameStarted;

		public bool isGameOver;

		private global::Kampai.Game.View.MinionObject minion;

		private bool jumping;

		private bool obstaclePenalty;

		private float obstacleTimer = 1.5f;

		private float obstacleElapsedTime;

		private float randomGrowlTimer = 7f;

		private float growlElapsedTime;

		private global::System.Collections.Generic.Dictionary<global::Kampai.Game.View.MinionObject, global::UnityEngine.Vector3> crowdMinions = new global::System.Collections.Generic.Dictionary<global::Kampai.Game.View.MinionObject, global::UnityEngine.Vector3>();

		private CustomFMOD_StudioEventEmitter waterEmitter;

		private CustomFMOD_StudioEventEmitter minionSoundsEmitter;

		private bool initialized;

		private global::UnityEngine.Transform startingCameraParentTransform;

		private bool firstCheckpointPassed;

		private bool cameraUpdateDebugLog;

		[Inject]
		public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignettePathCompletedSignal pathCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignetteMinionHitObstacleSignal hitObstacleSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignetteMinionHitCollectableSignal hitCollectableSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignetteJumpLandedSignal jumpLandedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SpawnMignetteDooberSignal spawnMignetteDooberSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.ChangeMignetteScoreSignal changeScoreSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal globalAudioSignal { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject]
		public global::Kampai.Game.RequestStopMignetteSignal requestStopMignetteSignal { get; set; }

		public bool IsOnPenalty
		{
			get
			{
				return obstaclePenalty;
			}
		}

		protected override void Start()
		{
			global::Kampai.Util.TimeProfiler.StartMonoProfiler("alligator");
			base.Start();
			buildingViewReference = MignetteBuildingObject.GetComponent<AlligatorSkiingBuildingViewObject>();
			if (buildingViewReference == null)
			{
				logger.Error("Couldn't find building ref component");
				return;
			}
			pathCompleteSignal.AddListener(OnPathComplete);
			hitObstacleSignal.AddListener(OnObstableHit);
			hitCollectableSignal.AddListener(OnCollectable);
			jumpLandedSignal.AddListener(OnJumpLanded);
			SaveCurrentCameraPosition();
			SetCameraMoveTime(1f);
			StartCoroutine(InitializeSetup());
		}

		private void InitializeCamera()
		{
			global::UnityEngine.Debug.Log("[AlligatorMignette] InitializeCamera() called");
			cameraMarker = waypointsController.FollowCameraMarker;
			cameraMarker.SetParent(base.gameObject.transform, false);
			cameraTransform = base.mignetteCamera.transform;
			startingCameraParentTransform = cameraTransform.parent;
			cameraMarker.position = cameraTransform.position;
			cameraMarker.rotation = cameraTransform.rotation;
			cameraTransform.SetParent(cameraMarker, false);
			cameraTransform.localPosition = global::UnityEngine.Vector3.zero;
			cameraTransform.transform.localRotation = global::UnityEngine.Quaternion.Euler(global::UnityEngine.Vector3.zero);
		}

		private global::System.Collections.IEnumerator InitializeSetup()
		{
			yield return null;
			if (!isGameOver)
			{
				waypointsController = base.transform.parent.GetComponentInChildren<AlligatorWaypointController>();
				alligatorAgent = GetComponentInChildren<AlligatorAgent>();
				alligatorTransform = alligatorAgent.transform;
				alligatorTransform.position = waypointsController.StartingWaypoint.position;
				alligatorTransform.rotation = waypointsController.StartingWaypoint.rotation;
				alligatorAgent.View = this;
				alligatorAgent.Waypoints = waypointsController;
				alligatorAgent.pathCompletedSignal = pathCompleteSignal;
				alligatorAgent.hitObstacleSignal = hitObstacleSignal;
				alligatorAgent.hitCollectableSignal = hitCollectableSignal;
				alligatorAgent.jumpLandedSignal = jumpLandedSignal;
				alligatorAgent.StartGameViewCallback = OnStartGame;
				alligatorAgent.VFXManager.DisplayMinionWake(false);
				minion = MignetteBuildingObject.GetChildMinion(0).Minion;
				global::UnityEngine.GameObject minionGO = minion.gameObject;
				minionParentTransform = minionGO.transform.parent;
				alligatorAgent.Initialized = true;
				alligatorAgent.SetMinionRiderParent();
				alligatorAgent.InitializePaths();
				if (minion.GetBlobShadow() != null)
				{
					MinionShadow = global::UnityEngine.Object.Instantiate(minion.GetBlobShadow()) as global::UnityEngine.GameObject;
					MinionShadow.transform.parent = alligatorAgent.MinionRiderTransform;
					MinionShadow.transform.localPosition = global::UnityEngine.Vector3.zero;
					MinionShadow.SetActive(false);
					minion.EnableBlobShadow(false);
				}
				waterEmitter = alligatorAgent.gameObject.AddComponent<CustomFMOD_StudioEventEmitter>();
				waterEmitter.shiftPosition = false;
				waterEmitter.staticSound = false;
				waterEmitter.path = fmodService.GetGuid("Play_minion_ski_01");
				waterEmitter.startEventOnAwake = false;
				minionSoundsEmitter = alligatorAgent.MinionRiderTransform.gameObject.AddComponent<CustomFMOD_StudioEventEmitter>();
				minionSoundsEmitter.startEventOnAwake = false;
				minionSoundsEmitter.shiftPosition = false;
				minionSoundsEmitter.staticSound = false;
				minion.DisableSelection();
				InitializeCamera();
				MoveCameraToStartPosition();
				alligatorAgent.AttachMinionToPathrider(minion.gameObject);
				minion.PlayAnimation(global::UnityEngine.Animator.StringToHash("Fishing"), 0, 0f);
				alligatorAgent.FishingPoleAnimator.Play(global::UnityEngine.Animator.StringToHash("Fishing"), 0, 0f);
				StartCoroutine(DelayAlligatorIntro());
				initialized = true;
			}
		}

		private global::System.Collections.IEnumerator DelayAlligatorIntro()
		{
			global::UnityEngine.Debug.Log("[AlligatorMignette] DelayAlligatorIntro() started - waiting 1s");
			yield return new global::UnityEngine.WaitForSeconds(1f);
			global::UnityEngine.Debug.Log(string.Format("[AlligatorMignette] DelayAlligatorIntro() - playing Casting anim. MinionAnimator={0}, FishingPoleAnimator={1}, FishingPoleAnimator.enabled={2}",
				minion != null, alligatorAgent.FishingPoleAnimator != null, alligatorAgent.FishingPoleAnimator != null ? alligatorAgent.FishingPoleAnimator.enabled.ToString() : "N/A"));
			minion.PlayAnimation(global::UnityEngine.Animator.StringToHash("Casting"), 0, 0f);
			alligatorAgent.FishingPoleAnimator.Play(global::UnityEngine.Animator.StringToHash("Casting"), 0, 0f);
			global::UnityEngine.Debug.Log("[AlligatorMignette] DelayAlligatorIntro() - waiting 2s before SwimToHam");
			yield return new global::UnityEngine.WaitForSeconds(2f);
			global::UnityEngine.Debug.Log("[AlligatorMignette] DelayAlligatorIntro() - calling SwimToHam()");
			alligatorAgent.SwimToHam();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			pathCompleteSignal.RemoveListener(OnPathComplete);
			hitObstacleSignal.RemoveListener(OnObstableHit);
			hitCollectableSignal.RemoveListener(OnCollectable);
		}

		public override void Update()
		{
			base.Update();
			if (initialized && !base.IsPaused && !shutdownInProgress)
			{
				TimeElapsed += global::UnityEngine.Time.deltaTime;
				if (obstaclePenalty)
				{
					UpdateClearObstaclePenalty();
				}
				if (!isGameOver && isGameStarted)
				{
					UpdateAlligatorGrowl();
					UpdateGameCamera();
				}
				PercentCompleted = alligatorAgent.GetPctComplete();
			}
		}

		private void UpdateAlligatorGrowl()
		{
			growlElapsedTime += global::UnityEngine.Time.deltaTime;
			if (growlElapsedTime >= randomGrowlTimer)
			{
				randomGrowlTimer = global::UnityEngine.Random.Range(5f, 10f);
				growlElapsedTime = 0f;
				globalAudioSignal.Dispatch("Play_alligator_pull_growl_01");
			}
		}

		private void UpdateClearObstaclePenalty()
		{
			obstacleElapsedTime += global::UnityEngine.Time.deltaTime;
			if (obstacleTimer < obstacleElapsedTime)
			{
				obstaclePenalty = false;
			}
		}

		public void ResetMignetteObjects()
		{
			isGameOver = true;
			if (!initialized)
			{
				return;
			}
			global::UnityEngine.Camera cam = base.mignetteCamera;
			if (cam == null)
			{
				cam = global::UnityEngine.Camera.main;
			}
			if (cam != null)
			{
				Go.killAllTweensWithTarget(cam.transform);
			}
			global::UnityEngine.GameObject instance = gameContext.injectionBinder.GetInstance<global::UnityEngine.GameObject>(global::Kampai.Game.GameElement.MINION_MANAGER);
			global::Kampai.Game.View.MinionManagerMediator component = instance.GetComponent<global::Kampai.Game.View.MinionManagerMediator>();
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.View.MinionObject, global::UnityEngine.Vector3> crowdMinion in crowdMinions)
			{
				component.stopTaskSignal.Dispatch(crowdMinion.Key.ID);
				crowdMinion.Key.GetAgent().enabled = true;
				crowdMinion.Key.setLocation(crowdMinion.Value);
				crowdMinion.Key.Idle();
			}
			cameraTransform.SetParent(startingCameraParentTransform);
			MignetteBuildingObject.GetChildMinion(0).Minion.gameObject.transform.parent = minionParentTransform;
			if (minion != null)
			{
				minion.EnableBlobShadow(true);
			}
		}

		public void OnPathComplete()
		{
			requestStopMignetteSignal.Dispatch(true);
			waterEmitter.Stop();
			isGameOver = true;
			global::Kampai.Util.TimeProfiler.StopMonoProfiler();
		}

		public void OnObstableHit()
		{
			if (isGameOver)
			{
				return;
			}
			if (obstaclePenalty)
			{
				jumping = false;
				if (obstacleTimer - obstacleElapsedTime < 0.25f)
				{
					obstacleTimer = 0.25f;
					obstacleElapsedTime = 0f;
				}
				minion.SetAnimTrigger("OnStumble");
				alligatorAgent.FishingPoleAnimator.SetTrigger("OnStumble");
			}
			else
			{
				obstaclePenalty = true;
				obstacleTimer = 1.5f;
				obstacleElapsedTime = 0f;
				minion.SetAnimTrigger("OnCrash");
				alligatorAgent.FishingPoleAnimator.SetTrigger("OnCrash");
				alligatorAgent.InputDown();
			}
			minionSoundsEmitter.Stop();
			minionSoundsEmitter.path = fmodService.GetGuid("Play_minion_crash_01");
			minionSoundsEmitter.StartEvent();
		}

		public void OnCollectable(global::UnityEngine.Vector3 collectablePosition, int collectablePoints, global::Kampai.Game.Mignette.AlligatorSkiing.View.CollectibleType type)
		{
			if (!isGameOver)
			{
				if (!firstCheckpointPassed)
				{
					firstCheckpointPassed = true;
				}
				if (!obstaclePenalty && type != global::Kampai.Game.Mignette.AlligatorSkiing.View.CollectibleType.Checkpoint)
				{
					globalAudioSignal.Dispatch("Play_mignette_collect");
					changeScoreSignal.Dispatch(collectablePoints);
					spawnMignetteDooberSignal.Dispatch(mignetteHUD, collectablePosition, collectablePoints, true);
				}
				else if (type == global::Kampai.Game.Mignette.AlligatorSkiing.View.CollectibleType.Checkpoint)
				{
					globalAudioSignal.Dispatch("Play_mignette_checkpoint");
					changeScoreSignal.Dispatch(collectablePoints);
					spawnMignetteDooberSignal.Dispatch(mignetteHUD, collectablePosition, collectablePoints, true);
				}
			}
		}

		private void MoveCameraToStartPosition()
		{
			global::UnityEngine.Debug.Log("[AlligatorMignette] MoveCameraToStartPosition() called");
			GoTweenConfig goTweenConfig = new GoTweenConfig();
			PositionTweenProperty tweenProp = new PositionTweenProperty(buildingViewReference.IntroCamera.position);
			goTweenConfig.addTweenProperty(tweenProp);
			goTweenConfig.easeType = GoEaseType.QuadInOut;
			GoTween tween = new GoTween(cameraMarker, 1f, goTweenConfig);
			Go.addTween(tween);
		}

		public void PlayMinionPulledIn()
		{
			minion.SetAnimTrigger("OnPulledIn");
			alligatorAgent.FishingPoleAnimator.SetTrigger("OnPulledIn");
		}

		private void UpdateGameCamera()
		{
			if (!isGameOver)
			{
				if (!cameraUpdateDebugLog)
				{
					global::UnityEngine.Debug.Log("[AlligatorMignette] UpdateGameCamera() first-run");
					cameraUpdateDebugLog = true;
				}
				global::UnityEngine.Vector3 position = cameraMarker.position;
				global::UnityEngine.Vector3 position2 = alligatorTransform.position;
				position2.y = position.y;
				position2.x += 15f;
				position2.z -= 15.5f;
				cameraMarker.position = global::UnityEngine.Vector3.Lerp(position, position2, 2f * global::UnityEngine.Time.deltaTime);
				global::UnityEngine.Quaternion b = global::UnityEngine.Quaternion.LookRotation(alligatorAgent.MinionRiderTransform.position - position);
				cameraMarker.rotation = global::UnityEngine.Quaternion.Slerp(cameraMarker.rotation, b, 2f * global::UnityEngine.Time.deltaTime);
			}
		}

		public void OnStartGame()
		{
			global::UnityEngine.Debug.Log("[AlligatorMignette] OnStartGame() called - setting isGameStarted=true, attaching minion");
			isGameStarted = true;
			alligatorAgent.AttachMinionAndGo();
			waterEmitter.Play();
			alligatorAgent.VFXManager.DisplayMinionWake(true);
		}

		public void OnInputDown()
		{
			if (isGameStarted && !jumping && !isGameOver && !obstaclePenalty)
			{
				jumping = true;
				minion.SetAnimTrigger("OnJump");
				alligatorAgent.FishingPoleAnimator.SetTrigger("OnJump");
				globalAudioSignal.Dispatch("Play_minion_ski_jump_01");
				waterEmitter.Stop();
				if (MinionShadow != null)
				{
					MinionShadow.SetActive(true);
				}
				alligatorAgent.InputDown();
			}
		}

		public void OnInputUp()
		{
			if (!isGameOver && jumping && !obstaclePenalty)
			{
				alligatorAgent.InputUp();
			}
		}

		public void OnJumpLanded()
		{
			jumping = false;
			if (MinionShadow != null)
			{
				MinionShadow.SetActive(false);
			}
			waterEmitter.StartEvent();
		}
	}
}
