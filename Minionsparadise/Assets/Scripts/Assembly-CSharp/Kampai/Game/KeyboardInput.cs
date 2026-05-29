using UnityEngine;
using UnityEngine.InputSystem;

namespace Kampai.Game
{
	public class KeyboardInput : global::Kampai.Game.IInput
	{
		private bool previousState;

		private bool pressed;

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Common.IPickService pickService { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.IMignetteService mignetteService { get; set; }

		[Inject]
		public global::Kampai.Game.DebugKeyHitSignal debugKeyHitSignal { get; set; }

		[PostConstruct]
		public void PostConstruct()
		{
			routineRunner.StartCoroutine(Update());
		}

		private global::System.Collections.IEnumerator Update()
		{
			while (true)
			{
				bool currentState = Mouse.current != null && Mouse.current.leftButton.isPressed;
				int input = 0;
				if (currentState)
				{
					input |= 1;
				}
				if (Mouse.current != null && global::UnityEngine.Mathf.Abs(Mouse.current.scroll.ReadValue().y) > 0f)
				{
					input |= 2;
				}
				if (!previousState && currentState)
				{
					pressed = true;
				}
				else if (previousState && !currentState)
				{
					pressed = false;
				}
				var mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
				var mousePosition = new global::UnityEngine.Vector3(mousePos.x, mousePos.y, 0f);
				pickService.OnGameInput(mousePosition, input, pressed);
				mignetteService.OnGameInput(mousePosition, input, pressed);
				previousState = currentState;
				yield return null;
			}
		}
	}
}
