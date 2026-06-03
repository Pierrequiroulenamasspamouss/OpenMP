public class AlligatorCameraController : global::UnityEngine.MonoBehaviour
{
	private global::System.Action onTweenComplete;

	private GoTween currentTween;

	private global::Kampai.Game.Mignette.AlligatorSkiing.View.AlligatorSkiingMignetteManagerView parentView;

	public void Start()
	{
		parentView = global::UnityEngine.Object.FindFirstObjectByType<global::Kampai.Game.Mignette.AlligatorSkiing.View.AlligatorSkiingMignetteManagerView>();
	}

	public void AlignWithTransform(global::UnityEngine.Transform t, float duration, GoEaseType easeType, global::System.Action onComplete = null)
	{
		if (!parentView.isGameOver)
		{
			if (currentTween != null && currentTween.state == GoTweenState.Running)
			{
				Go.removeTween(currentTween);
			}
			GoTweenConfig goTweenConfig = new GoTweenConfig();
			PositionTweenProperty tweenProp = new PositionTweenProperty(t.localPosition, false, true);
			goTweenConfig.addTweenProperty(tweenProp);
			goTweenConfig.easeType = easeType;
			RotationTweenProperty tweenProp2 = new RotationTweenProperty(t.localRotation.eulerAngles, false, true);
			goTweenConfig.addTweenProperty(tweenProp2);
			onTweenComplete = onComplete;
			goTweenConfig.onComplete(OnTweenComplete);
			currentTween = new GoTween(parentView.mignetteCamera.transform, duration, goTweenConfig);
			Go.addTween(currentTween);
		}
	}

	private void OnTweenComplete(AbstractGoTween obj)
	{
		if (onTweenComplete != null)
		{
			onTweenComplete();
		}
	}
}
