using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Kampai.Game
{
	[global::UnityEngine.AddComponentMenu("Event/Kampai Touch Input Module")]
	public class KampaiTouchInputModule : global::UnityEngine.EventSystems.PointerInputModule
	{
		private global::UnityEngine.Vector2 m_LastMousePosition;

		private global::UnityEngine.Vector2 m_MousePosition;

		[global::UnityEngine.SerializeField]
		private bool m_AllowActivationOnStandalone;

		public bool allowActivationOnStandalone
		{
			get
			{
				return m_AllowActivationOnStandalone;
			}
			set
			{
				m_AllowActivationOnStandalone = value;
			}
		}

		protected KampaiTouchInputModule()
		{
		}

		public override void UpdateModule()
		{
			m_LastMousePosition = m_MousePosition;
			m_MousePosition = (Mouse.current != null) ? Mouse.current.position.ReadValue() : global::UnityEngine.Vector2.zero;
		}

		public override bool IsModuleSupported()
		{
			return m_AllowActivationOnStandalone || Touchscreen.current != null;
		}

		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}
			if (UseFakeInput())
			{
				bool mouseButtonDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
				return mouseButtonDown | ((m_MousePosition - m_LastMousePosition).sqrMagnitude > 0f);
			}
			for (int i = 0; i < global::Kampai.Game.InputUtils.touchCount; i++)
			{
				global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.GetTouch(i);
				if (touch.phase == global::UnityEngine.TouchPhase.Began || touch.phase == global::UnityEngine.TouchPhase.Moved || touch.phase == global::UnityEngine.TouchPhase.Stationary)
				{
					return true;
				}
			}
			return false;
		}

		private bool UseFakeInput()
		{
			return Touchscreen.current == null;
		}

		public override void Process()
		{
			if (UseFakeInput() && global::Kampai.Game.InputUtils.touchCount <= 1)
			{
				FakeTouches();
			}
			else
			{
				ProcessTouchEvents();
			}
		}

		private void FakeTouches()
		{
			global::UnityEngine.TouchPhase phase;
			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
				phase = global::UnityEngine.TouchPhase.Began;
			}
			else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
			{
				phase = global::UnityEngine.TouchPhase.Ended;
			}
			else if ((m_MousePosition - m_LastMousePosition).sqrMagnitude > 0f)
			{
				phase = global::UnityEngine.TouchPhase.Moved;
			}
			else
			{
				phase = global::UnityEngine.TouchPhase.Stationary;
			}
			global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.CreateTouch(-1, m_MousePosition, m_MousePosition - m_LastMousePosition, phase);
			bool pressed;
			bool released;
			global::UnityEngine.EventSystems.PointerEventData touchPointerEventData = GetTouchPointerEventData(touch, out pressed, out released);
			ProcessTouchPress(touchPointerEventData, pressed, released);
			if (!released)
			{
				ProcessMove(touchPointerEventData);
				ProcessDrag(touchPointerEventData);
			}
			else
			{
				RemovePointerData(touchPointerEventData);
			}
		}

		private void ProcessTouchEvents()
		{
			for (int i = 0; i < global::Kampai.Game.InputUtils.touchCount; i++)
			{
				global::UnityEngine.Touch touch = global::Kampai.Game.InputUtils.GetTouch(i);
				bool pressed;
				bool released;
				global::UnityEngine.EventSystems.PointerEventData touchPointerEventData = GetTouchPointerEventData(touch, out pressed, out released);
				ProcessTouchPress(touchPointerEventData, pressed, released);
				if (!released)
				{
					ProcessMove(touchPointerEventData);
					ProcessDrag(touchPointerEventData);
				}
				else
				{
					RemovePointerData(touchPointerEventData);
				}
			}
		}

		private void ProcessTouchPress(global::UnityEngine.EventSystems.PointerEventData pointerEvent, bool pressed, bool released)
		{
			global::UnityEngine.GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed)
			{
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = global::UnityEngine.Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
				DeselectIfSelectionChanged(gameObject, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject)
				{
					HandlePointerExitAndEnter(pointerEvent, gameObject);
					pointerEvent.pointerEnter = gameObject;
				}
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.EventSystems.ExecuteEvents.ExecuteHierarchy(gameObject, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = global::UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<global::UnityEngine.EventSystems.IPointerClickHandler>(gameObject);
				}
				float unscaledTime = global::UnityEngine.Time.unscaledTime;
				if (gameObject2 == pointerEvent.lastPress)
				{
					float num = unscaledTime - pointerEvent.clickTime;
					if (num < 0.3f)
					{
						pointerEvent.clickCount++;
					}
					else
					{
						pointerEvent.clickCount = 1;
					}
					pointerEvent.clickTime = unscaledTime;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.pointerPress = gameObject2;
				pointerEvent.rawPointerPress = gameObject;
				pointerEvent.clickTime = unscaledTime;
				pointerEvent.pointerDrag = global::UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<global::UnityEngine.EventSystems.IDragHandler>(gameObject);
				if (pointerEvent.pointerDrag != null)
				{
					global::UnityEngine.EventSystems.ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.initializePotentialDrag);
				}
			}
			if (released)
			{
				global::UnityEngine.EventSystems.ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);
				global::UnityEngine.GameObject eventHandler = global::UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<global::UnityEngine.EventSystems.IPointerClickHandler>(gameObject);
				if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
				{
					global::UnityEngine.EventSystems.ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
				}
				else if (pointerEvent.pointerDrag != null)
				{
					global::UnityEngine.EventSystems.ExecuteEvents.ExecuteHierarchy(gameObject, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.dropHandler);
				}
				pointerEvent.eligibleForClick = false;
				pointerEvent.pointerPress = null;
				pointerEvent.rawPointerPress = null;
				if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					global::UnityEngine.EventSystems.ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.endDragHandler);
				}
				pointerEvent.dragging = false;
				pointerEvent.pointerDrag = null;
				if (pointerEvent.pointerDrag != null)
				{
					global::UnityEngine.EventSystems.ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.endDragHandler);
				}
				pointerEvent.pointerDrag = null;
				global::UnityEngine.EventSystems.ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, global::UnityEngine.EventSystems.ExecuteEvents.pointerExitHandler);
				pointerEvent.pointerEnter = null;
			}
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			ClearSelection();
		}

		public override string ToString()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.AppendLine((!UseFakeInput()) ? "Input: Touch" : "Input: Faked");
			if (UseFakeInput())
			{
				global::UnityEngine.EventSystems.PointerEventData lastPointerEventData = GetLastPointerEventData(-1);
				if (lastPointerEventData != null)
				{
					stringBuilder.AppendLine(lastPointerEventData.ToString());
				}
			}
			else
			{
				foreach (global::System.Collections.Generic.KeyValuePair<int, global::UnityEngine.EventSystems.PointerEventData> pointerDatum in m_PointerData)
				{
					stringBuilder.AppendLine(pointerDatum.ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
