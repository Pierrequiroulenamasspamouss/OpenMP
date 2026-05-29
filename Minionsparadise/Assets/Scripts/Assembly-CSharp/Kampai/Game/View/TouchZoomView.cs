using UnityEngine.InputSystem;

namespace Kampai.Game.View
{
	public class TouchZoomView : global::Kampai.Game.View.ZoomView
	{
		private global::UnityEngine.Vector3 touch1PreviousPosition;

		private global::UnityEngine.Vector3 touch2PreviousPosition;

		protected override bool IsInputStationary()
		{
			int touchCount = global::Kampai.Game.InputUtils.touchCount;
			if (touchCount <= 0)
			{
				return false;
			}
			if (touchCount == 1)
			{
				global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.GetTouch(0);
				return touch.phase == global::UnityEngine.TouchPhase.Stationary || touch.phase == global::UnityEngine.TouchPhase.Began;
			}
			global::UnityEngine.Touch touch2 = global::Kampai.Game.InputUtils.GetTouch(0);
			global::UnityEngine.Touch touch3 = global::Kampai.Game.InputUtils.GetTouch(1);
			return ((touch2.phase == global::UnityEngine.TouchPhase.Stationary || touch2.phase == global::UnityEngine.TouchPhase.Began) && touch3.phase != global::UnityEngine.TouchPhase.Moved) || ((touch3.phase == global::UnityEngine.TouchPhase.Stationary || touch3.phase == global::UnityEngine.TouchPhase.Began) && touch2.phase != global::UnityEngine.TouchPhase.Moved);
		}
 
		protected override bool IsInputDone()
		{
			int touchCount = global::Kampai.Game.InputUtils.touchCount;
			if (touchCount <= 0)
			{
				return true;
			}
			if (touchCount == 1)
			{
				global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.GetTouch(0);
				return touch.phase == global::UnityEngine.TouchPhase.Ended || touch.phase == global::UnityEngine.TouchPhase.Canceled;
			}
			global::UnityEngine.Touch touch2 = global::Kampai.Game.InputUtils.GetTouch(0);
			global::UnityEngine.Touch touch3 = global::Kampai.Game.InputUtils.GetTouch(1);
			return (touch2.phase == global::UnityEngine.TouchPhase.Ended || touch2.phase == global::UnityEngine.TouchPhase.Canceled) && (touch3.phase == global::UnityEngine.TouchPhase.Ended || touch3.phase == global::UnityEngine.TouchPhase.Canceled);
		}

		public override void CalculateBehaviour(global::UnityEngine.Vector3 position)
		{
			if (global::UnityEngine.Application.isEditor)
			{
				float axis = 0f;
				if (Mouse.current != null)
				{
					axis = Mouse.current.scroll.ReadValue().y;
				}
				if (axis != 0f)
				{
					velocity = new global::UnityEngine.Vector3(0f, axis * 100f, 0f);
					return;
				}
			}
			if (global::Kampai.Game.InputUtils.touchCount < 2)
			{
				return;
			}
			global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.GetTouch(0);
			global::UnityEngine.Touch touch2 = global::Kampai.Game.InputUtils.GetTouch(1);
			if (touch.phase == global::UnityEngine.TouchPhase.Moved || touch2.phase == global::UnityEngine.TouchPhase.Moved)
			{
				if (touch1PreviousPosition == global::UnityEngine.Vector3.zero && touch2PreviousPosition == global::UnityEngine.Vector3.zero)
				{
					touch1PreviousPosition = touch.position;
					touch2PreviousPosition = touch2.position;
					return;
				}
				float magnitude = (touch1PreviousPosition - touch2PreviousPosition).magnitude;
				float magnitude2 = (touch.position - touch2.position).magnitude;
				float y = magnitude2 - magnitude;
				velocity = new global::UnityEngine.Vector3(0f, y, 0f);
				touch1PreviousPosition = touch.position;
				touch2PreviousPosition = touch2.position;
			}
		}

		public override void ResetBehaviour()
		{
			touch1PreviousPosition = global::UnityEngine.Vector3.zero;
			touch2PreviousPosition = global::UnityEngine.Vector3.zero;
		}
	}
}
