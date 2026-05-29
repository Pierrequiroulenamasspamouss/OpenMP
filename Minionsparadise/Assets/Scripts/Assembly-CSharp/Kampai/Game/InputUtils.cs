using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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

			// Try to use the Input System Enhanced Touch API when available
			try
			{
				EnhancedTouchSupport.Enable();
				var active = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
				int activeCount = active.Count;

				for (int i = 0; i < activeCount; i++)
				{
					var et = active[i];
					var phase = MapPhase(et.phase);
					int fingerId = et.touchId;
					if (phase != global::UnityEngine.TouchPhase.Moved && phase != global::UnityEngine.TouchPhase.Stationary)
					{
						CorruptTouch(fingerId, false);
					}
				}

				int num = 0;
				if (activeCount > 0)
				{
					for (int j = 0; j < activeCount; j++)
					{
						var et = active[j];
						int id = et.touchId;
						if (!IsTouchCorrupted(id))
						{
							var pos = et.screenPosition;
							var delta = et.delta;
							var ph = MapPhase(et.phase);
							touches[num] = CreateTouch(id, pos, delta, ph);
							num++;
						}
					}
				}
				#if UNITY_EDITOR || UNITY_STANDALONE
				else
				{
					float axis = 0f;
					if (Mouse.current != null)
					{
						axis = Mouse.current.scroll.ReadValue().y;
					}
					bool flag = (Keyboard.current != null && (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed));
					global::UnityEngine.Vector2 vector = Mouse.current != null ? Mouse.current.position.ReadValue() : global::UnityEngine.Vector2.zero;
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
							global::UnityEngine.Vector2 vector3 = Mouse.current != null ? Mouse.current.position.ReadValue() : global::UnityEngine.Vector2.zero;
							global::UnityEngine.Vector2 vector4 = new global::UnityEngine.Vector2(editorZoomDistance, 0f);
							touches[num++] = CreateTouch(10, vector3 - vector4, global::UnityEngine.Vector2.zero, global::UnityEngine.TouchPhase.Ended);
							touches[num++] = CreateTouch(11, vector3 + vector4, global::UnityEngine.Vector2.zero, global::UnityEngine.TouchPhase.Ended);
							editorIsZooming = false;
						}
						if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasReleasedThisFrame))
						{
							global::UnityEngine.TouchPhase phase3;
							global::UnityEngine.Vector2 delta = global::UnityEngine.Vector2.zero;
							var mousePos = Mouse.current.position.ReadValue();
							if (Mouse.current.leftButton.wasPressedThisFrame)
							{
								phase3 = global::UnityEngine.TouchPhase.Began;
								lastMousePosition = mousePos;
							}
							else if (Mouse.current.leftButton.wasReleasedThisFrame)
							{
								phase3 = global::UnityEngine.TouchPhase.Ended;
								delta = mousePos - lastMousePosition;
							}
							else
							{
								phase3 = global::UnityEngine.TouchPhase.Moved;
								delta = mousePos - lastMousePosition;
								lastMousePosition = mousePos;
							}
							touches[num++] = CreateTouch(99, mousePos, delta, phase3);
						}
					}
				}
				#endif

				_touchCount = num;
				if (_touchCount > 0)
				{
					global::System.Array.Sort(touches, 0, _touchCount, comparer);
				}
				return;
			}
			catch (Exception)
			{
				// If EnhancedTouch or Input System is not available, fall back to legacy Input is not allowed when Input System is active.
			}
		}

		private static global::UnityEngine.TouchPhase MapPhase(UnityEngine.InputSystem.TouchPhase p)
		{
			switch (p)
			{
				case UnityEngine.InputSystem.TouchPhase.Began:
					return global::UnityEngine.TouchPhase.Began;
				case UnityEngine.InputSystem.TouchPhase.Moved:
					return global::UnityEngine.TouchPhase.Moved;
				case UnityEngine.InputSystem.TouchPhase.Stationary:
					return global::UnityEngine.TouchPhase.Stationary;
				case UnityEngine.InputSystem.TouchPhase.Ended:
					return global::UnityEngine.TouchPhase.Ended;
				case UnityEngine.InputSystem.TouchPhase.Canceled:
					return global::UnityEngine.TouchPhase.Canceled;
				default:
					return global::UnityEngine.TouchPhase.Canceled;
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