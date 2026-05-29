using UnityEngine.InputSystem;

namespace Kampai.Fatal
{
	public class FatalView : global::Kampai.Util.KampaiView
	{
		[Inject]
		public global::Kampai.Common.ReInitializeGameSignal reInitializeGameSignal { get; set; }

		private void OnApplicationPause(bool isPausing)
		{
			if (!isPausing && reInitializeGameSignal != null)
			{
				reInitializeGameSignal.Dispatch(string.Empty);
			}
		}

		private void Update()
		{
			if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				reInitializeGameSignal.Dispatch(string.Empty);
			}
		}
	}
}
