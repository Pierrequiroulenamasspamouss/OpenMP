public class AlligatorAgent : global::UnityEngine.MonoBehaviour
{
	private const string CHOMP_ANIM_STATE_NAME = "Chomp";

	private const string CHOMP_EXIT_ANIM_STATE_NAME = "ChompExit";

	private const string SLOWSWIM_ANIM_STATE_NAME = "SlowSwim";

	private const string FASTSWIM_ANIM_STATE_NAME = "Swimming";

	public const float MIN_MINION_DISTANCE = 4f;

	public const float MAX_MINION_DISTANCE = 5f;

	public const float MINION_SPEED_ADJUSTMENT = 0.025f;

	public const float MINION_SLOWTOSTOP_ADJUSTMENT = 0.01f;

	public global::UnityEngine.Animator AlligatorAnimator;

	public global::UnityEngine.GameObject FishingPoleGO;

	public global::UnityEngine.Animator FishingPoleAnimator;

	public global::UnityEngine.Animator MinionAnimator;

	public AlligatorWaypointController Waypoints;

	public float StartingAlligatorSpeed = 5f;

	public global::UnityEngine.Transform MinionRiderTransform;

	public AlligatorVFXManager VFXManager;

	public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorSkiingJumpController JumpController;

	public global::UnityEngine.Transform MinionHardpointTransform;

	public float SinkRate = -1f;

	public float SinkDelay = 0.2f;

	public float MinionTurnAmount;

	public bool Initialized;

	public global::UnityEngine.Transform AlligatorLineTransform;

	public global::System.Action StartGameViewCallback;

	public global::UnityEngine.GameObject HamGO;

	private GoSpline alligatorSpline;

	private GoSpline minionSpline;

	private float totalPathLength;

	private float alligatorTraveledDistance;

	private float currrentAlligatorSpeed = 1f;

	private float alligatorMaxSpeed = 1f;

	private float alligatorAcceleration = 1f;

	private float totalMinionPathLength;

	private float totalMinionPathTime;

	private float minionElapsedTravelTime;

	private float minionSpeedCoefficient = 1f;

	private global::UnityEngine.LineRenderer _ropeRenderer;

	private GoTween minionRotationTween;

	private global::UnityEngine.Transform FishingPoleTransform;

	private bool gameOver;

	private bool minionAttachedToAlligator;

	private bool updateAlligatorPosition;

	private bool updateMinionPosition;

	public bool introChompAnimationPlaying;

	private GoTween introTween;

	public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignettePathCompletedSignal pathCompletedSignal { get; set; }

	public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignetteMinionHitObstacleSignal hitObstacleSignal { get; set; }

	public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignetteMinionHitCollectableSignal hitCollectableSignal { get; set; }

	public global::Kampai.Game.Mignette.AlligatorSkiing.AlligatorMignetteJumpLandedSignal jumpLandedSignal { get; set; }

	public global::Kampai.Game.Mignette.AlligatorSkiing.View.AlligatorSkiingMignetteManagerView View { get; set; }

	private void Start()
	{
		_ropeRenderer = GetComponent<global::UnityEngine.LineRenderer>();
		global::UnityEngine.GameObject fishingPoleGO = FishingPoleGO;
		FishingPoleGO = global::UnityEngine.Object.Instantiate(fishingPoleGO);
		FishingPoleGO.transform.parent = JumpController.transform;
		FishingPoleGO.transform.localPosition = global::UnityEngine.Vector3.zero;
		FishingPoleAnimator = FishingPoleGO.GetComponent<global::UnityEngine.Animator>();
		FishingPoleTransform = FishingPoleGO.FindChild("LineAnchor").transform;
		FishingPoleGO.GetComponent<AlligatorHamController>().DisplayHam(true);
		_ropeRenderer.enabled = false;
	}

	private void Update()
	{
		if (!Initialized || View.IsPaused)
		{
			return;
		}
		if (introChompAnimationPlaying)
		{
			var stateInfo = AlligatorAnimator.GetCurrentAnimatorStateInfo(0);
			bool isChompExit = stateInfo.IsName("ChompExit");
			bool isSwimming = stateInfo.IsName("Swimming");
			bool isChomp = stateInfo.IsName("Chomp");
			bool isSlowSwim = stateInfo.IsName("SlowSwim");
			global::UnityEngine.Debug.Log(string.Format("[AlligatorAgent] Waiting for chomp exit - AnimatorEnabled={0}, StateHash={1}, IsChomp={2}, IsChompExit={3}, IsSwimming={4}, IsSlowSwim={5}, NormalizedTime={6:F2}",
				AlligatorAnimator.enabled, stateInfo.fullPathHash, isChomp, isChompExit, isSwimming, isSlowSwim, stateInfo.normalizedTime));
			if (isChompExit || isSwimming)
			{
				global::UnityEngine.Debug.Log("[AlligatorAgent] Chomp animation completed -> calling StartAlligator() + scheduling OnStartGame");
				StartAlligator();
				introChompAnimationPlaying = false;
				Invoke("OnStartGame", 0.5f);
			}
		}
		if (!gameOver)
		{
			if (minionAttachedToAlligator)
			{
				MaintainMinionDistance();
			}
			if (alligatorSpline != null && updateAlligatorPosition)
			{
				UpdateAlligatorPosition();
			}
			if (minionSpline != null && updateMinionPosition)
			{
				UpdateMinionPosition();
			}
		}
	}

	public void LateUpdate()
	{
		if (Initialized && !View.IsPaused && MinionHardpointTransform != null)
		{
			_ropeRenderer.SetPosition(0, FishingPoleTransform.position);
			_ropeRenderer.SetPosition(1, AlligatorLineTransform.position);
		}
	}

	public void StartAlligator()
	{
		global::UnityEngine.Debug.Log("[AlligatorAgent] StartAlligator() called - enabling alligator position updates");
		updateAlligatorPosition = true;
	}

	public void AttachMinionAndGo()
	{
		global::UnityEngine.Debug.Log("[AlligatorAgent] AttachMinionAndGo() called - enabling minion position updates");
		minionAttachedToAlligator = true;
		updateMinionPosition = true;
		_ropeRenderer.enabled = true;
	}

	public void InitializePaths()
	{
		alligatorSpline = Waypoints.GetAlligatorSpline();
		totalPathLength = alligatorSpline.pathLength;
		currrentAlligatorSpeed = StartingAlligatorSpeed;
		alligatorMaxSpeed = StartingAlligatorSpeed;
		minionSpline = Waypoints.GetMinionSpline();
		totalMinionPathLength = minionSpline.pathLength;
		totalMinionPathTime = totalMinionPathLength / StartingAlligatorSpeed;
	}

	public float GetPctComplete()
	{
		return alligatorTraveledDistance / totalPathLength;
	}

	public void SetMinionRiderParent()
	{
		MinionRiderTransform.SetParent(base.transform.parent);
	}

	private void UpdateAlligatorPosition()
	{
		if (currrentAlligatorSpeed < alligatorMaxSpeed)
		{
			if (currrentAlligatorSpeed + alligatorAcceleration > alligatorMaxSpeed)
			{
				currrentAlligatorSpeed = alligatorMaxSpeed;
			}
			else
			{
				currrentAlligatorSpeed += alligatorAcceleration;
			}
		}
		else if (currrentAlligatorSpeed > alligatorMaxSpeed)
		{
			if (currrentAlligatorSpeed - alligatorAcceleration < alligatorMaxSpeed)
			{
				currrentAlligatorSpeed = alligatorMaxSpeed;
			}
			else
			{
				currrentAlligatorSpeed -= alligatorAcceleration;
			}
		}
		alligatorTraveledDistance += global::UnityEngine.Time.deltaTime * currrentAlligatorSpeed;
		float t = alligatorTraveledDistance / totalPathLength;
		global::UnityEngine.Vector3 pointOnPath = alligatorSpline.getPointOnPath(t);
		base.transform.rotation = ((!(pointOnPath == base.transform.position)) ? global::UnityEngine.Quaternion.LookRotation(pointOnPath - base.transform.position) : global::UnityEngine.Quaternion.identity);
		base.transform.position = pointOnPath;
	}

	private void UpdateMinionPosition()
	{
		minionElapsedTravelTime += global::UnityEngine.Time.deltaTime * minionSpeedCoefficient;
		float num = minionElapsedTravelTime / totalMinionPathTime;
		global::UnityEngine.Vector3 pointOnPath = minionSpline.getPointOnPath(num);
		MinionRiderTransform.rotation = ((!(pointOnPath == MinionRiderTransform.position)) ? global::UnityEngine.Quaternion.LookRotation(pointOnPath - MinionRiderTransform.position) : global::UnityEngine.Quaternion.identity);
		MinionRiderTransform.position = pointOnPath;
		if (num >= 1f)
		{
			pathCompletedSignal.Dispatch();
			gameOver = true;
		}
	}

	private void MaintainMinionDistance()
	{
		if (global::UnityEngine.Vector3.Distance(base.transform.position, MinionRiderTransform.position) > 5f)
		{
			minionSpeedCoefficient += 0.025f;
		}
		else if (global::UnityEngine.Vector3.Distance(base.transform.position, MinionRiderTransform.position) < 4f)
		{
			minionSpeedCoefficient -= 0.025f;
		}
		else
		{
			minionSpeedCoefficient = 1f;
		}
	}

	public void AttachMinionToPathrider(global::UnityEngine.GameObject minionGO)
	{
		minionGO.transform.parent = MinionHardpointTransform;
		minionGO.transform.localPosition = global::UnityEngine.Vector3.zero;
		minionGO.transform.localRotation = global::UnityEngine.Quaternion.Euler(global::UnityEngine.Vector3.zero);
		MinionRiderTransform.position = minionSpline.getPointOnPath(0f);
		MinionRiderTransform.rotation = global::UnityEngine.Quaternion.LookRotation(minionSpline.getPointOnPath(0.1f) - minionSpline.getPointOnPath(0f));
		minionGO.SetActive(true);
	}

	public void OnMinionHitObstacle()
	{
		hitObstacleSignal.Dispatch();
		VFXManager.PlayImpactVfx(MinionRiderTransform.position);
	}

	public void OnMinionLand()
	{
		VFXManager.DisplayMinionWake(true);
		jumpLandedSignal.Dispatch();
	}

	public void OnMinionJump()
	{
		VFXManager.DisplayMinionWake(false);
	}

	public void InputDown()
	{
		JumpController.DoJump();
	}

	public void InputUp()
	{
		JumpController.JumpReleased();
	}

	public void OnMinionHitCollectable(global::UnityEngine.Vector3 collectablePosition, int collectablePoints, global::Kampai.Game.Mignette.AlligatorSkiing.View.CollectibleType type)
	{
		hitCollectableSignal.Dispatch(collectablePosition, collectablePoints, type);
	}

	public void OnRotateMinion(global::UnityEngine.Quaternion rot)
	{
		if (!JumpController.IsJumping() && !rot.Equals(MinionHardpointTransform.localRotation))
		{
			if (minionRotationTween != null && minionRotationTween.state == GoTweenState.Running)
			{
				Go.removeTween(minionRotationTween);
			}
			GoTweenConfig goTweenConfig = new GoTweenConfig();
			RotationQuaternionTweenProperty tweenProp = new RotationQuaternionTweenProperty(rot, false, true);
			goTweenConfig.addTweenProperty(tweenProp);
			minionRotationTween = new GoTween(MinionHardpointTransform, 0.25f, goTweenConfig);
			Go.addTween(minionRotationTween);
		}
	}

	public void ChangeSpeed(float Speed, float acceleration)
	{
		alligatorMaxSpeed = Speed;
		alligatorAcceleration = acceleration;
	}

	public void TrySetAnimationState(string AnimationStateName)
	{
		AlligatorAnimator.Play(global::UnityEngine.Animator.StringToHash(AnimationStateName), 0, 0f);
	}

	public void OnStartGame()
	{
		global::UnityEngine.Debug.Log(string.Format("[AlligatorAgent] OnStartGame() called - AlligatorAnimator.enabled={0}, playing 'Swimming'", AlligatorAnimator.enabled));
		AlligatorAnimator.Play(global::UnityEngine.Animator.StringToHash("Swimming"), 0);
		StartGameViewCallback();
	}

	public void IntroPointArrived(AbstractGoTween tween)
	{
		global::UnityEngine.Debug.Log(string.Format("[AlligatorAgent] IntroPointArrived() - AlligatorAnimator.enabled={0}, playing 'Chomp', setting introChompAnimationPlaying=true", AlligatorAnimator.enabled));
		AlligatorAnimator.Play(global::UnityEngine.Animator.StringToHash("Chomp"), 0);
		View.PlayMinionPulledIn();
		introChompAnimationPlaying = true;
	}

	public void SwimToHam()
	{
		global::UnityEngine.Debug.Log(string.Format("[AlligatorAgent] SwimToHam() called - AlligatorAnimator={0}, enabled={1}, HamGO={2}, StartingHampoint={3}",
			AlligatorAnimator != null, AlligatorAnimator != null ? AlligatorAnimator.enabled.ToString() : "N/A",
			HamGO != null, Waypoints != null && Waypoints.StartingHampoint != null ? Waypoints.StartingHampoint.position.ToString() : "NULL"));
		if (introTween != null && introTween.state == GoTweenState.Running)
		{
			Go.removeTween(introTween);
		}
		HamGO.SetActive(false);
		AlligatorAnimator.Play(global::UnityEngine.Animator.StringToHash("SlowSwim"), 0);
		base.transform.LookAt(Waypoints.StartingHampoint);
		global::UnityEngine.Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.rotation = global::UnityEngine.Quaternion.Euler(eulerAngles);
		AlligatorAnimator.Play(global::UnityEngine.Animator.StringToHash("SlowSwim"), 0);
		GoTweenConfig goTweenConfig = new GoTweenConfig();
		PositionTweenProperty tweenProp = new PositionTweenProperty(Waypoints.StartingHampoint.position);
		goTweenConfig.addTweenProperty(tweenProp);
		goTweenConfig.onComplete(IntroPointArrived);
		introTween = new GoTween(base.transform, 1f, goTweenConfig);
		Go.addTween(introTween);
		global::UnityEngine.Debug.Log("[AlligatorAgent] SwimToHam() - intro tween started, will call IntroPointArrived on complete");
	}

	public void OnAlligatorBiteHam()
	{
		FishingPoleGO.GetComponent<AlligatorHamController>().DisplayHam(false);
		HamGO.SetActive(true);
		_ropeRenderer.enabled = true;
	}
}
