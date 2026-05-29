using UnityEngine.InputSystem;

namespace Kampai.Util
{
	public class DebugButton : global::Kampai.Util.KampaiView
	{
		private bool visible;

		private global::UnityEngine.GameObject instance;

		private bool captured;

		public global::UnityEngine.UI.Button button;

		public global::System.Collections.Generic.List<global::UnityEngine.UI.Image> images;

		private bool rightClickEnabled;

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.UI.View.HUDChangedSiblingIndexSignal hudChangedSiblingIndexSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DebugKeyHitSignal openSignal { get; set; }

		[Inject]
		public global::Kampai.Util.DebugPickService debugPickService { get; set; }

		[Inject]
		public global::Kampai.Util.DebugConsoleController controller { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisplayDebugButtonSignal displayDebugButtonSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeSignal { get; set; }

		protected override void Start()
		{
			base.Start();
			hudChangedSiblingIndexSignal.AddListener(OnHudChangedIndex);
			openSignal.AddListener(ToggleOpen);
			controller.ToggleRightClickSignal.AddListener(OnToggleRightClick);
			displayDebugButtonSignal.AddListener(EnableButton);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			openSignal.RemoveListener(ToggleOpen);
			hudChangedSiblingIndexSignal.RemoveListener(OnHudChangedIndex);
			controller.ToggleRightClickSignal.RemoveListener(OnToggleRightClick);
			displayDebugButtonSignal.RemoveListener(EnableButton);
		}

		private void OnHudChangedIndex(int index)
		{
			base.transform.SetAsLastSibling();
		}

		public void OnClick(global::UnityEngine.UI.Button button)
		{
			ToggleOpen(global::Kampai.Util.DebugArgument.OPEN_CONSOLE);
		}

		private void EnableButton(bool enable)
		{
			button.enabled = enable;
			foreach (global::UnityEngine.UI.Image image in images)
			{
				image.enabled = enable;
			}
		}

		private void Update()
		{
			bool pressed = false;
			global::UnityEngine.Vector2 mousePos = default(global::UnityEngine.Vector2);
			if (Mouse.current != null)
			{
				mousePos = Mouse.current.position.ReadValue();
			}
			debugPickService.OnGameInput(mousePos, 0, pressed);
		}

		private void OnToggleRightClick()
		{
			rightClickEnabled = !rightClickEnabled;
		}

		private void ToggleOpen(global::Kampai.Util.DebugArgument arg)
		{
			if (arg != global::Kampai.Util.DebugArgument.OPEN_CONSOLE)
			{
				return;
			}
			if (captured)
			{
				visible = instance.activeSelf;
			}
			if (!visible)
			{
				visible = true;
				if (!captured)
				{
					global::Kampai.UI.View.IGUICommand command = guiService.BuildCommand(global::Kampai.UI.View.GUIOperation.LoadStatic, "DebugConsole");
					instance = guiService.Execute(command);
					captured = true;
				}
				closeSignal.Dispatch(instance);
				instance.SetActive(visible);
			}
			else
			{
				visible = false;
				instance.SetActive(visible);
				closeSignal.Dispatch(null);
			}
		}
	}
}
