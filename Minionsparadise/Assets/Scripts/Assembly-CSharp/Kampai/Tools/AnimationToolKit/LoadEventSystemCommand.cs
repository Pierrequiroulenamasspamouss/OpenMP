using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Kampai.Tools.AnimationToolKit
{
	public class LoadEventSystemCommand : global::strange.extensions.command.impl.Command
	{
		[Inject(global::strange.extensions.context.api.ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		public override void Execute()
		{
			GameObject gameObject = new GameObject("Event System");
			gameObject.transform.parent = ContextView.transform;
			EventSystem o = gameObject.AddComponent<EventSystem>();
			base.injectionBinder.Bind<EventSystem>().ToValue(o);
			gameObject.AddComponent<InputSystemUIInputModule>();
		}
	}
}
