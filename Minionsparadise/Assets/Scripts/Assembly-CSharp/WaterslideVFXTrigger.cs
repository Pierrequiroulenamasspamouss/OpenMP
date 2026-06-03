public class WaterslideVFXTrigger : global::UnityEngine.MonoBehaviour
{
	public global::UnityEngine.GameObject VFXGameObject;

	public global::UnityEngine.Transform VFXParentTransform;

	public bool PlaySplashAudio;

	private global::Kampai.Game.Mignette.WaterSlide.View.WaterSlideMignetteManagerView parentView;

	public void Start()
	{
		parentView = global::UnityEngine.Object.FindFirstObjectByType<global::Kampai.Game.Mignette.WaterSlide.View.WaterSlideMignetteManagerView>();
		DisableAudio();
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		PathAgent componentInParent = other.transform.GetComponentInParent<PathAgent>();
		if (componentInParent != null && VFXGameObject != null)
		{
			global::UnityEngine.Transform transform = global::UnityEngine.Object.Instantiate(VFXGameObject).transform;
			transform.SetParent(VFXParentTransform, false);
			if (PlaySplashAudio)
			{
				parentView.EnableWaterAudio(true);
				Invoke("DisableAudio", 1f);
			}
		}
	}

	private void DisableAudio()
	{
		parentView.EnableWaterAudio(false);
	}
}
