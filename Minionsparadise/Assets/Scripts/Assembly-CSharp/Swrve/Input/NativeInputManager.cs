using UnityEngine;
using UnityEngine.InputSystem;

namespace Swrve.Input
{
	public class NativeInputManager : global::Swrve.Input.IInputManager
	{
		private static global::Swrve.Input.NativeInputManager instance;

		public static global::Swrve.Input.NativeInputManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new global::Swrve.Input.NativeInputManager();
				}
				return instance;
			}
		}

		private NativeInputManager()
		{
		}

		bool global::Swrve.Input.IInputManager.GetMouseButtonUp(int buttonId)
		{
			var mouse = Mouse.current;
			if (mouse == null)
				return true;

			switch (buttonId)
			{
				case 0:
					return !mouse.leftButton.isPressed;
				case 1:
					return !mouse.rightButton.isPressed;
				case 2:
					return !mouse.middleButton.isPressed;
				default:
					return !mouse.leftButton.isPressed;
			}
		}

		bool global::Swrve.Input.IInputManager.GetMouseButtonDown(int buttonId)
		{
			var mouse = Mouse.current;
			if (mouse == null)
				return false;

			switch (buttonId)
			{
				case 0:
					return mouse.leftButton.isPressed;
				case 1:
					return mouse.rightButton.isPressed;
				case 2:
					return mouse.middleButton.isPressed;
				default:
					return mouse.leftButton.isPressed;
			}
		}

		global::UnityEngine.Vector3 global::Swrve.Input.IInputManager.GetMousePosition()
		{
			var mouse = Mouse.current;
			Vector2 pos = mouse != null ? mouse.position.ReadValue() : Vector2.zero;
			Vector3 mousePosition = new Vector3(pos.x, pos.y, 0f);
			mousePosition.y = (float)Screen.height - mousePosition.y;
			return mousePosition;
		}
	}
}
