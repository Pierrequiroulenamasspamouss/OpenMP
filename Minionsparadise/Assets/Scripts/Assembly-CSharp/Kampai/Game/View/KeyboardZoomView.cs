using UnityEngine.InputSystem;

namespace Kampai.Game.View
{
	public class KeyboardZoomView : global::Kampai.Game.View.ZoomView
	{
		private int frameCount;

		protected override bool IsInputDone()
		{
			float scroll = 0f;
			if (Mouse.current != null)
			{
				scroll = Mouse.current.scroll.ReadValue().y;
			}
			if (global::UnityEngine.Mathf.Abs(scroll) < 1E-07f)
			{
				frameCount++;
			}
			else
			{
				frameCount = 0;
			}
			if (frameCount >= 3)
			{
				frameCount = 3;
				return true;
			}
			return false;
		}

		public override void CalculateBehaviour(global::UnityEngine.Vector3 position)
		{
			mouseRay = new global::UnityEngine.Ray(base.transform.position, base.transform.forward);
			groundPlane.Raycast(mouseRay, out hitDistance);
			hitPosition = mouseRay.GetPoint(hitDistance);
			global::UnityEngine.Vector3 v = base.transform.worldToLocalMatrix.MultiplyPoint3x4(hitPosition);
			base.transform.localToWorldMatrix.MultiplyVector(v);
			float scroll = 0f;
			if (Mouse.current != null)
			{
				scroll = Mouse.current.scroll.ReadValue().y;
			}
			velocity = new global::UnityEngine.Vector3(0f, scroll * 2000f, 0f);
		}
	}
}
