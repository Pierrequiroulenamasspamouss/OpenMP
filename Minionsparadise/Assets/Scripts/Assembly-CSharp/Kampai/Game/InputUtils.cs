namespace Kampai.Game
{
	public static class InputUtils
	{
		private sealed class TouchComparer : global::System.Collections.Generic.IComparer<global::UnityEngine.Touch>
		{
			int global::System.Collections.Generic.IComparer<global::UnityEngine.Touch>.Compare(global::UnityEngine.Touch x, global::UnityEngine.Touch y)
			{
				return x.fingerId - y.fingerId;
			}
		}

		private const int MAX_HANDLED_TOUCHES = 256;

		private static global::Kampai.Game.InputUtils.TouchComparer comparer = new global::Kampai.Game.InputUtils.TouchComparer();

		private static bool[] touchCorruptedBySamsung = new bool[256];

		private static global::UnityEngine.Touch[] touches = new global::UnityEngine.Touch[256];

		private static int _touchCount;

		private static int frameUpdated;

#if UNITY_EDITOR || UNITY_STANDALONE
		private static global::UnityEngine.Vector2 lastMousePosition;
		private static float lastZoomDistance;
#endif

		public static int touchCount
		{
			get
			{
				if (frameUpdated < global::UnityEngine.Time.frameCount)
				{
					UpdateTouchStates();
				}
				return _touchCount;
			}
		}

		private static global::System.Reflection.FieldInfo m_FingerIdField = typeof(global::UnityEngine.Touch).GetField("m_FingerId", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic);

		private static global::System.Reflection.FieldInfo m_PositionField = typeof(global::UnityEngine.Touch).GetField("m_Position", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic);

		private static global::System.Reflection.FieldInfo m_PositionDeltaField = typeof(global::UnityEngine.Touch).GetField("m_PositionDelta", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic);

		private static global::System.Reflection.FieldInfo m_PhaseField = typeof(global::UnityEngine.Touch).GetField("m_Phase", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic);

#if UNITY_EDITOR || UNITY_STANDALONE
		private static float editorZoomDistance = 200f;

		private static bool editorIsZooming = false;
#endif

		public static global::UnityEngine.Touch CreateTouch(int fingerId, global::UnityEngine.Vector2 position, global::UnityEngine.Vector2 deltaPosition, global::UnityEngine.TouchPhase phase)
		{
			global::UnityEngine.Touch touch = default(global::UnityEngine.Touch);
			object obj = touch;
			m_FingerIdField.SetValue(obj, fingerId);
			m_PositionField.SetValue(obj, position);
			m_PositionDeltaField.SetValue(obj, deltaPosition);
			m_PhaseField.SetValue(obj, phase);
			return (global::UnityEngine.Touch)obj;
		}

		private static void UpdateTouchStates()
		{
			frameUpdated = global::UnityEngine.Time.frameCount;
			for (int i = 0; i < global::UnityEngine.Input.touchCount; i++)
			{
				global::UnityEngine.Touch touch = global::UnityEngine.Input.GetTouch(i);
				global::UnityEngine.TouchPhase phase = touch.phase;
				int fingerId = touch.fingerId;
				if (phase != global::UnityEngine.TouchPhase.Moved && phase != global::UnityEngine.TouchPhase.Stationary)
				{
					CorruptTouch(fingerId, false);
				}
			}
			int num = 0;
			if (global::UnityEngine.Input.touchCount > 0)
			{
				for (int j = 0; j < global::UnityEngine.Input.touchCount; j++)
				{
					global::UnityEngine.Touch touch2 = global::UnityEngine.Input.GetTouch(j);
					if (!IsTouchCorrupted(touch2.fingerId))
					{
						touches[num] = touch2;
						num++;
					}
				}
			}
#if UNITY_EDITOR || UNITY_STANDALONE
			else
			{
				float axis = global::UnityEngine.Input.GetAxis("Mouse ScrollWheel");
				bool flag = global::UnityEngine.Input.GetKey(global::UnityEngine.KeyCode.LeftControl) || global::UnityEngine.Input.GetKey(global::UnityEngine.KeyCode.RightControl);
				global::UnityEngine.Vector2 vector = global::UnityEngine.Input.mousePosition;
				if (flag && (axis != 0f || editorIsZooming))
				{
					global::UnityEngine.TouchPhase phase2;
					if (!editorIsZooming)
					{
						phase2 = global::UnityEngine.TouchPhase.Began;
						editorIsZooming = true;
						lastZoomDistance = editorZoomDistance;
					}
					else if (axis != 0f)
					{
						phase2 = global::UnityEngine.TouchPhase.Moved;
						lastZoomDistance = editorZoomDistance;
						editorZoomDistance += axis * 1000f;
					}
					else
					{
						phase2 = global::UnityEngine.TouchPhase.Stationary;
					}
					global::UnityEngine.Vector2 vector2 = new global::UnityEngine.Vector2(editorZoomDistance, 0f);
					global::UnityEngine.Vector2 vectorDelta = new global::UnityEngine.Vector2(editorZoomDistance - lastZoomDistance, 0f);
					touches[num++] = CreateTouch(10, vector - vector2, -vectorDelta, phase2);
					touches[num++] = CreateTouch(11, vector + vector2, vectorDelta, phase2);
				}
				else
				{
					if (editorIsZooming)
					{
						global::UnityEngine.Vector2 vector3 = global::UnityEngine.Input.mousePosition;
						global::UnityEngine.Vector2 vector4 = new global::UnityEngine.Vector2(editorZoomDistance, 0f);
						touches[num++] = CreateTouch(10, vector3 - vector4, global::UnityEngine.Vector2.zero, global::UnityEngine.TouchPhase.Ended);
						touches[num++] = CreateTouch(11, vector3 + vector4, global::UnityEngine.Vector2.zero, global::UnityEngine.TouchPhase.Ended);
						editorIsZooming = false;
					}
					if (global::UnityEngine.Input.GetMouseButtonDown(0) || global::UnityEngine.Input.GetMouseButton(0) || global::UnityEngine.Input.GetMouseButtonUp(0))
					{
						global::UnityEngine.TouchPhase phase3;
						global::UnityEngine.Vector2 delta = global::UnityEngine.Vector2.zero;
						if (global::UnityEngine.Input.GetMouseButtonDown(0))
						{
							phase3 = global::UnityEngine.TouchPhase.Began;
							lastMousePosition = vector;
						}
						else if (global::UnityEngine.Input.GetMouseButtonUp(0))
						{
							phase3 = global::UnityEngine.TouchPhase.Ended;
							delta = vector - lastMousePosition;
						}
						else
						{
							phase3 = global::UnityEngine.TouchPhase.Moved;
							delta = vector - lastMousePosition;
							lastMousePosition = vector;
						}
						touches[num++] = CreateTouch(99, vector, delta, phase3);
					}
				}
			}
#endif
			_touchCount = num;
			if (_touchCount > 0)
			{
				global::System.Array.Sort(touches, 0, _touchCount, comparer);
			}
		}

		private static bool IsTouchCorrupted(int fingerId)
		{
			return fingerId >= 256 || touchCorruptedBySamsung[fingerId];
		}

		private static void CorruptTouch(int fingerId, bool isCorrupted)
		{
			if (fingerId < 256)
			{
				touchCorruptedBySamsung[fingerId] = isCorrupted;
			}
		}

		public static global::UnityEngine.Touch GetTouch(int i)
		{
			if (frameUpdated < global::UnityEngine.Time.frameCount)
			{
				UpdateTouchStates();
			}
			return touches[i];
		}
	}
}