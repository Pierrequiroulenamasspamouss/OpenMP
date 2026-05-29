using UnityEngine.InputSystem;

namespace Kampai.Common
{
	public class StartAndroidBackButtonCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Common.AndroidBackButtonSignal androidBackButtonSignal { get; set; }

		public override void Execute()
		{
			routineRunner.StartCoroutine(Update());
		}

		private global::System.Collections.IEnumerator Update()
		{
			while (true)
			{
				if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
				{
					androidBackButtonSignal.Dispatch();
				}
				yield return null;
			}
		}
	}
}
