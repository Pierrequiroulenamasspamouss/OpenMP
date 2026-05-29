using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Kampai.Main
{
	internal sealed class SetupEventSystemCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("SetupEventSystemCommand") as global::Kampai.Util.IKampaiLogger;

		private GameObject eventSystem;

		[Inject]
		public global::Kampai.UI.View.LoadUICompleteSignal uiLoadCompleteSignal { get; set; }

		public override void Execute()
		{
			logger.EventStart("SetupEventSystemCommand.Execute");

			var existing = Object.FindObjectOfType<EventSystem>();
			if (existing != null)
			{
				eventSystem = existing.gameObject;
			}
			else
			{
				eventSystem = new GameObject("EventSystem");
				eventSystem.AddComponent<EventSystem>();
			}

			var inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
			if (inputSystemModule == null)
			{
				inputSystemModule = eventSystem.AddComponent<InputSystemUIInputModule>();
			}

			var legacyStandalone = eventSystem.GetComponent<StandaloneInputModule>();
			if (legacyStandalone != null)
			{
				Object.DestroyImmediate(legacyStandalone);
			}

			var kampaiModule = eventSystem.GetComponent<global::Kampai.Game.KampaiTouchInputModule>();
			if (kampaiModule != null)
			{
				Object.DestroyImmediate(kampaiModule);
			}

			base.injectionBinder.Bind<GameObject>().ToValue(eventSystem).ToName(global::Kampai.Main.MainElement.UI_EVENTSYSTEM)
				.CrossContext();
			uiLoadCompleteSignal.AddListener(SetParent);
			logger.EventStop("SetupEventSystemCommand.Execute");
		}

		private void SetParent(GameObject contextView)
		{
			if (!(eventSystem == null))
			{
				eventSystem.transform.parent = contextView.transform;
			}
		}
	}
}
