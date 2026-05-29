using UnityEngine.InputSystem;

public class SimpleCamera : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Plane groundPlane;

	private global::UnityEngine.Vector3 currentPosition;

	private global::UnityEngine.Vector3 hitPosition;

	private global::UnityEngine.Vector3 velocity;

	public global::UnityEngine.Vector2 MinBounds;

	public global::UnityEngine.Vector2 MaxBounds;

	public float decayAmount = 0.925f;

	private void Start()
	{
		global::UnityEngine.Application.targetFrameRate = 60;
		global::UnityEngine.Vector3 inNormal = new global::UnityEngine.Vector3(0f, 1f, 0f);
		global::UnityEngine.Vector3 inPoint = new global::UnityEngine.Vector3(0f, 0f, 0f);
		groundPlane = new global::UnityEngine.Plane(inNormal, inPoint);
	}

	private void Update()
	{
		if (global::UnityEngine.Application.isMobilePlatform && global::Kampai.Game.InputUtils.touchCount > 0)
		{
			var t = global::Kampai.Game.InputUtils.GetTouch(0);
			if (t.phase == global::UnityEngine.TouchPhase.Began)
			{
				hitPosition = GroundPlaneRaycast(new global::UnityEngine.Vector3(t.position.x, t.position.y, 0f));
				velocity = global::UnityEngine.Vector3.zero;
			}
			else if (t.phase == global::UnityEngine.TouchPhase.Moved)
			{
				currentPosition = GroundPlaneRaycast(new global::UnityEngine.Vector3(t.position.x, t.position.y, 0f));
				velocity = hitPosition - currentPosition;
			}
		}
		else if (global::UnityEngine.Application.isEditor)
		{
			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
				var mp = Mouse.current.position.ReadValue();
				hitPosition = GroundPlaneRaycast(new global::UnityEngine.Vector3(mp.x, mp.y, 0f));
				velocity = global::UnityEngine.Vector3.zero;
			}
			else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
			{
				var mp = Mouse.current.position.ReadValue();
				currentPosition = GroundPlaneRaycast(new global::UnityEngine.Vector3(mp.x, mp.y, 0f));
				velocity = hitPosition - currentPosition;
			}
		}
		global::UnityEngine.Transform transform = global::UnityEngine.Camera.main.transform;
		transform.position = new global::UnityEngine.Vector3(global::UnityEngine.Mathf.Clamp(transform.position.x + velocity.x, MinBounds.x, MaxBounds.x), transform.position.y + velocity.y, global::UnityEngine.Mathf.Clamp(transform.position.z + velocity.z, MinBounds.y, MaxBounds.y));
		velocity *= decayAmount;
	}

	private global::UnityEngine.Vector3 GroundPlaneRaycast(global::UnityEngine.Vector3 screenPosition)
	{
		global::UnityEngine.Ray ray = global::UnityEngine.Camera.main.ScreenPointToRay(screenPosition);
		float enter;
		groundPlane.Raycast(ray, out enter);
		return ray.GetPoint(enter);
	}
}
