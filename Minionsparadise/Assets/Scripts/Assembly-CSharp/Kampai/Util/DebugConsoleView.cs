using UnityEngine.InputSystem;

namespace Kampai.Util
{
	public class DebugConsoleView : global::Kampai.Util.KampaiView
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DebugConsoleView") as global::Kampai.Util.IKampaiLogger;

		public global::Kampai.Util.DebugConsoleInputField inputField;

		public global::UnityEngine.UI.Text textField;

		public global::UnityEngine.UI.Scrollbar scrollbar;

		public global::UnityEngine.RectTransform QuestItemPanel;

		private int maxLines = 100;

		private bool initialized;

		private global::UnityEngine.GameObject questDebugPanelPrefab;

		private global::UnityEngine.GameObject questDebugPanelInst;

		private global::System.Collections.Generic.List<string> commandStack = new global::System.Collections.Generic.List<string>();

		private int stackIndex;

		private bool arrowDown;

		[Inject(global::Kampai.Main.MainElement.CAMERA)]
		public global::UnityEngine.GameObject cameraGO { get; set; }

		[Inject]
		public global::Kampai.UI.View.LogToScreenSignal logToScreenSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowQuestPanelSignal showQuestPanel { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Util.DebugConsoleController controller { get; set; }

		[Inject(global::Kampai.Main.MainElement.UI_GLASSCANVAS)]
		public global::UnityEngine.GameObject glassCanvas { get; set; }

		public void OnGUI()
		{
			if (!inputField.isFocused)
			{
				return;
			}
			if (Keyboard.current != null && (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame))
			{
				arrowDown = true;
			}
			if (!arrowDown)
			{
				return;
			}
			if (Keyboard.current != null && Keyboard.current.upArrowKey.wasReleasedThisFrame)
			{
				if (stackIndex != 0)
				{
					stackIndex--;
					inputField.text = commandStack[stackIndex];
					inputField.MoveTextEnd(false);
					arrowDown = false;
				}
			}
			else if (Keyboard.current != null && Keyboard.current.downArrowKey.wasReleasedThisFrame && stackIndex < commandStack.Count - 1)
			{
				stackIndex++;
				inputField.text = commandStack[stackIndex];
				arrowDown = false;
			}
		}

		protected override void Start()
		{
			if (global::UnityEngine.PlayerPrefs.HasKey("DebugHistory"))
			{
				commandStack.Add(global::UnityEngine.PlayerPrefs.GetString("DebugHistory"));
				stackIndex = 1;
			}
			inputField.onEndEdit.AddListener(InputRecieved);
			global::UnityEngine.RectTransform rectTransform = base.transform as global::UnityEngine.RectTransform;
			rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f - rectTransform.sizeDelta.y);
			global::UnityEngine.RectTransform component = inputField.GetComponent<global::UnityEngine.RectTransform>();
			global::UnityEngine.RectTransform rectTransform2 = component;
			global::UnityEngine.Vector2 offsetMin = (component.offsetMax = global::UnityEngine.Vector2.zero);
			rectTransform2.offsetMin = offsetMin;
			component = inputField.GetComponentInChildren<global::UnityEngine.UI.Text>().GetComponent<global::UnityEngine.RectTransform>();
			global::UnityEngine.RectTransform rectTransform3 = component;
			offsetMin = (component.offsetMax = global::UnityEngine.Vector2.zero);
			rectTransform3.offsetMin = offsetMin;
			component = textField.GetComponent<global::UnityEngine.RectTransform>();
			global::UnityEngine.RectTransform rectTransform4 = component;
			offsetMin = (component.offsetMax = global::UnityEngine.Vector2.zero);
			rectTransform4.offsetMin = offsetMin;
			base.Start();
			if (!initialized)
			{
				textField.text = string.Empty;
				AddNewMessage("Kampai.  Type 'help' for help.  Press UP for previous command.\n");
				initialized = true;
				controller.CloseConsoleSignal.AddListener(OnClose);
				controller.FlushSignal.AddListener(OnFlush);
				controller.EnableQuestDebugSignal.AddListener(OnQuestDebug);
			}
			StartCoroutine(WaitAFrame());
			questDebugPanelPrefab = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>("QuestDebugPanel");
		}

		protected override void OnDestroy()
		{
			controller.CloseConsoleSignal.RemoveListener(OnClose);
			controller.FlushSignal.RemoveListener(OnFlush);
		}

		private void InitQuestButtons()
		{
			questDebugPanelInst = global::UnityEngine.Object.Instantiate(questDebugPanelPrefab);
			global::UnityEngine.Transform transform = questDebugPanelInst.transform;
			transform.SetParent(glassCanvas.transform, true);
			global::UnityEngine.RectTransform rectTransform = transform as global::UnityEngine.RectTransform;
			rectTransform.localPosition = global::UnityEngine.Vector3.zero;
			rectTransform.localScale = global::UnityEngine.Vector3.one;
		}

		private void OnQuestDebug()
		{
			global::Kampai.Util.DebugQuestView component = questDebugPanelInst.GetComponent<global::Kampai.Util.DebugQuestView>();
			component.Toggle();
		}

		private global::System.Collections.IEnumerator WaitAFrame()
		{
			yield return null;
			logToScreenSignal.AddListener(AddNewMessage);
			InitQuestButtons();
		}

		public void OnEnable()
		{
			StartCoroutine(FocusAfterframe());
		}

		private void AddNewMessage(string text)
		{
			textField.text = cropLines(textField.text + string.Format("\n{0}", text));
		}

		private void InputRecieved(string field)
		{
			if (true)
			{
				StartCoroutine(DelayedInputRecieved(field));
			}
		}

		private global::System.Collections.IEnumerator DelayedInputRecieved(string field)
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			yield return new global::UnityEngine.WaitForEndOfFrame();
			commandStack.Add(field);
			stackIndex = commandStack.Count;
			global::UnityEngine.PlayerPrefs.SetString("DebugHistory", field);
			UpdateTextFields();
			ProcessInput(field);
			scrollbar.value = 0f;
		}

		private global::System.Collections.IEnumerator FocusAfterframe()
		{
			yield return null;
			inputField.ActivateInputField();
			inputField.Select();
		}

		private void UpdateTextFields()
		{
			textField.text = cropLines(textField.text + string.Format("\n{0}", inputField.text));
			inputField.text = string.Empty;
		}

		private int countNewLines(string text)
		{
			int num = 0;
			foreach (char c in text)
			{
				if (c == '\n')
				{
					num++;
				}
			}
			return num;
		}

		private string cropLines(string text)
		{
			int num = countNewLines(text) - maxLines;
			if (num > 0)
			{
				int num2 = 0;
				foreach (char c in text)
				{
					if (c == '\n')
					{
						num--;
						if (num < 0)
						{
							return text.Substring(num2);
						}
					}
					num2++;
				}
			}
			return text;
		}

		private void ProcessInput(string input)
		{
			string[] array = input.Split(' ');
			array[0] = array[0].ToLower();
			if (array[0].Length != 0)
			{
				global::Kampai.Util.DebugCommand command;
				switch (controller.GetCommand(array, out command))
				{
				case global::Kampai.Util.DebugCommandError.NotFound:
					AddNewMessage(input + " -> INVALID COMMAND");
					break;
				case global::Kampai.Util.DebugCommandError.NotEnoughArguments:
					AddNewMessage(input + " -> NOT ENOUGH ARGUMENTS");
					break;
				default:
					command(controller, array);
					AddNewMessage(controller.GetOutput());
					break;
				}
			}
		}

		public void OnClose()
		{
			base.transform.gameObject.SetActive(false);
		}

		private void OnFlush()
		{
			AddNewMessage(controller.GetOutput());
		}
	}
}
