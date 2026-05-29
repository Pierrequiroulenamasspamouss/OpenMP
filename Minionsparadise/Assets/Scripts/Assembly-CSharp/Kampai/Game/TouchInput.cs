using UnityEngine.InputSystem;

namespace Kampai.Game
{
	public class TouchInput : global::Kampai.Game.IInput
	{
		private global::UnityEngine.Vector3 position;

		private int touchCount;

		private bool pressed;

		private bool isDeviceSamsung;

		private bool wasStylusActive;

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Common.IPickService pickService { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.IMignetteService mignetteService { get; set; }

		[Inject]
		public global::Kampai.Util.DeviceInformation deviceInformation { get; set; }

		[PostConstruct]
		public void PostConstruct()
		{
			isDeviceSamsung = deviceInformation.IsSamsung();
			routineRunner.StartCoroutine(Update());
		}

		private global::System.Collections.IEnumerator Update()
		{
			yield return null;
			while (true)
			{
				position = global::UnityEngine.Vector3.zero;
				touchCount = global::Kampai.Game.InputUtils.touchCount;
				if (touchCount > 0)
				{
					global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.GetTouch(0);
					position = touch.position;
					if (touch.phase == global::UnityEngine.TouchPhase.Began)
					{
						pressed = true;
						global::Kampai.Util.ScreenUtils.ToggleAutoRotation(false);
					}
					else if (touch.phase == global::UnityEngine.TouchPhase.Ended)
					{
						pressed = false;
						global::Kampai.Util.ScreenUtils.ToggleAutoRotation(true);
					}
				}
				else if (isDeviceSamsung || global::UnityEngine.Application.isEditor)
				{
					bool isStylusActive = Mouse.current != null && Mouse.current.leftButton.isPressed;
					if (isStylusActive || wasStylusActive)
					{
						var mp = Mouse.current != null ? Mouse.current.position.ReadValue() : global::UnityEngine.Vector2.zero;
						position = new global::UnityEngine.Vector3(mp.x, mp.y, 0f);
						touchCount = 1;
						pressed = (wasStylusActive = isStylusActive);
					}
				}
				pressed = pressed && touchCount > 0;
				pickService.OnGameInput(position, touchCount, pressed);
				mignetteService.OnGameInput(position, touchCount, pressed);
				yield return null;
			}
		}
	}
}
