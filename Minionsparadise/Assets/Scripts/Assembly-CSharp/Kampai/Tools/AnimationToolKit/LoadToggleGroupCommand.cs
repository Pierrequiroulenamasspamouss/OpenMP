namespace Kampai.Tools.AnimationToolKit
{
	public class LoadToggleGroupCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public global::UnityEngine.Canvas Canvas { get; set; }

		public override void Execute()
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Toggle Group", typeof(global::UnityEngine.RectTransform), typeof(global::UnityEngine.UI.ToggleGroup));
			global::UnityEngine.RectTransform rectTransform = gameObject.GetComponent<global::UnityEngine.RectTransform>();
			rectTransform.SetParent(Canvas.transform, false);
			rectTransform.anchorMin = global::UnityEngine.Vector2.zero;
			rectTransform.anchorMax = global::UnityEngine.Vector2.zero;
			rectTransform.pivot = global::UnityEngine.Vector2.zero;
			rectTransform.anchoredPosition = global::UnityEngine.Vector2.zero;
			rectTransform.sizeDelta = global::UnityEngine.Vector2.zero;
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(gameObject).ToName(global::Kampai.Tools.AnimationToolKit.AnimationToolKitElement.TOGGLE_GROUP);
		}
	}
}
