public class WaterslideCameraTrigger : global::UnityEngine.MonoBehaviour
{
	public global::UnityEngine.Transform CameraTransform;

	public float FieldOfView = -1f;

	public float Delay;

	public float TransitionDuration = 1f;

	public GoEaseType EaseType = GoEaseType.QuadInOut;

	private PathAgent pathAgent;

	private global::Kampai.Game.Mignette.WaterSlide.View.WaterSlideMignetteManagerView parentView;

	public void Start()
	{
		parentView = global::UnityEngine.Object.FindFirstObjectByType<global::Kampai.Game.Mignette.WaterSlide.View.WaterSlideMignetteManagerView>();
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		PathAgent pathAgent = null;
		global::UnityEngine.Transform parent = other.transform;
		while (parent.parent != null && pathAgent == null)
		{
			parent = parent.parent;
			pathAgent = parent.gameObject.GetComponent<PathAgent>();
		}
		if (pathAgent != null)
		{
			this.pathAgent = pathAgent;
			Invoke("MoveCamera", Delay);
		}
	}

	public void MoveCamera()
	{
		if (!parentView.isGameOver && pathAgent != null)
		{
			pathAgent.OnCameraOverride(CameraTransform, FieldOfView, TransitionDuration, EaseType);
			pathAgent = null;
		}
	}
}
