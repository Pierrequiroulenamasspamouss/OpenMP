public class WaypointNode : global::UnityEngine.MonoBehaviour
{
	public bool UpdateSpeed;

	public float Speed;

	public float acceleration;

	public bool LockInput;

	public bool TurnAnimation;

	public bool EnableWakeVFX;

	public bool DisableWakeVFX;

	public bool FollowSplineRotation = true;

	private global::Kampai.Game.Mignette.WaterSlide.View.WaterSlideMignetteManagerView parentView;

	public void Start()
	{
		parentView = global::UnityEngine.Object.FindFirstObjectByType<global::Kampai.Game.Mignette.WaterSlide.View.WaterSlideMignetteManagerView>();
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		PathAgent componentInParent = other.transform.GetComponentInParent<PathAgent>();
		if (componentInParent != null)
		{
			if (UpdateSpeed)
			{
				componentInParent.ChangeSpeed(Speed, acceleration);
			}
			if (TurnAnimation)
			{
				componentInParent.OnMinionTurn();
			}
			else
			{
				componentInParent.OnMinionOutofTurn();
			}
			if (EnableWakeVFX)
			{
				componentInParent.VFXManager.DisplayMinionWake(true);
				parentView.EnableWaterAudio(true);
			}
			if (DisableWakeVFX)
			{
				componentInParent.VFXManager.DisplayMinionWake(false);
				parentView.EnableWaterAudio(false);
			}
			if (componentInParent.FollowSplineRotation != FollowSplineRotation)
			{
				componentInParent.FollowSplineRotation = FollowSplineRotation;
			}
			componentInParent.OnRotateMinion(base.transform.localRotation);
		}
	}
}
