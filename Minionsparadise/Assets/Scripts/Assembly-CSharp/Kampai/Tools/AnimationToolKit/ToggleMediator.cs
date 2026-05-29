namespace Kampai.Tools.AnimationToolKit
{
	public class ToggleMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private const float toggleHeight = 30f;

		private int toggleId;

		[Inject(global::Kampai.Tools.AnimationToolKit.AnimationToolKitElement.TOGGLE_GROUP)]
		public global::UnityEngine.GameObject ToggleGroupGameObject { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.ToggleView view { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.AnimationToolKit AnimationToolKit { get; set; }

		public override void OnRegister()
		{
			(base.transform as global::UnityEngine.RectTransform).SetParent(ToggleGroupGameObject.transform, false);
			global::UnityEngine.UI.Toggle component = GetComponent<global::UnityEngine.UI.Toggle>();
			global::UnityEngine.UI.ToggleGroup toggleGroup = (component.group = ToggleGroupGameObject.GetComponent<global::UnityEngine.UI.ToggleGroup>());
			toggleGroup.RegisterToggle(component);
			global::UnityEngine.Vector3 position = new global::UnityEngine.Vector3(65f, 15f + (float)(ToggleGroupGameObject.transform.childCount - 1) * 30f);
			view.SetPosition(position);
			component.onValueChanged.AddListener(OnToggle);
		}

		public override void OnRemove()
		{
			global::UnityEngine.UI.Toggle component = GetComponent<global::UnityEngine.UI.Toggle>();
			component.onValueChanged.RemoveListener(OnToggle);
			global::UnityEngine.UI.ToggleGroup component2 = ToggleGroupGameObject.GetComponent<global::UnityEngine.UI.ToggleGroup>();
			component2.UnregisterToggle(component);
		}

		public void InitializeToggle(int instanceId, string labelText)
		{
			toggleId = instanceId;
			view.SetLabel(labelText);
			if (toggleId == 1000)
			{
				GetComponent<global::UnityEngine.UI.Toggle>().isOn = true;
			}
		}

		private void OnToggle(bool toggleOn)
		{
			if (toggleOn)
			{
				AnimationToolKit.ToggleOn(toggleId);
			}
		}
	}
}
