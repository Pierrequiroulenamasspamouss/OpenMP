using UnityEngine.InputSystem;

namespace Prime31
{
	public class MonoBehaviourGUI : global::UnityEngine.MonoBehaviour
	{
		protected float _width;

		protected float _buttonHeight;

		protected global::System.Collections.Generic.Dictionary<string, bool> _toggleButtons = new global::System.Collections.Generic.Dictionary<string, bool>();

		protected global::System.Text.StringBuilder _logBuilder = new global::System.Text.StringBuilder();

		private bool _logRegistered;

		private global::UnityEngine.Vector2 _logScrollPosition;

		private bool _isShowingLogConsole;

		private float _doubleClickDelay = 0.15f;

		private float _previousClickTime;

		private bool _isWindowsPhone;

		private global::UnityEngine.Texture2D _normalBackground;

		private global::UnityEngine.Texture2D _bottomButtonBackground;

		private global::UnityEngine.Texture2D _activeBackground;

		private global::UnityEngine.Texture2D _toggleButtonBackground;

		private bool _didRetinaIpadCheck;

		private bool _isRetinaIpad;

		private global::UnityEngine.Texture2D normalBackground
		{
			get
			{
				if (!_normalBackground)
				{
					_normalBackground = new global::UnityEngine.Texture2D(1, 1);
					_normalBackground.SetPixel(0, 0, global::UnityEngine.Color.gray);
					_normalBackground.Apply();
				}
				return _normalBackground;
			}
		}

		private global::UnityEngine.Texture2D bottomButtonBackground
		{
			get
			{
				if (!_bottomButtonBackground)
				{
					_bottomButtonBackground = new global::UnityEngine.Texture2D(1, 1);
					_bottomButtonBackground.SetPixel(0, 0, global::UnityEngine.Color.Lerp(global::UnityEngine.Color.gray, global::UnityEngine.Color.black, 0.5f));
					_bottomButtonBackground.Apply();
				}
				return _bottomButtonBackground;
			}
		}

		private global::UnityEngine.Texture2D activeBackground
		{
			get
			{
				if (!_activeBackground)
				{
					_activeBackground = new global::UnityEngine.Texture2D(1, 1);
					_activeBackground.SetPixel(0, 0, global::UnityEngine.Color.yellow);
					_activeBackground.Apply();
				}
				return _activeBackground;
			}
		}

		private global::UnityEngine.Texture2D toggleButtonBackground
		{
			get
			{
				if (!_toggleButtonBackground)
				{
					_toggleButtonBackground = new global::UnityEngine.Texture2D(1, 1);
					_toggleButtonBackground.SetPixel(0, 0, global::UnityEngine.Color.black);
					_toggleButtonBackground.Apply();
				}
				return _toggleButtonBackground;
			}
		}

		private bool isRetinaOrLargeScreen()
		{
			return _isWindowsPhone || global::UnityEngine.Screen.width >= 960 || global::UnityEngine.Screen.height >= 960;
		}

		private bool isRetinaIpad()
		{
			if (!_didRetinaIpadCheck)
			{
				if (global::UnityEngine.Screen.height >= 2048 || global::UnityEngine.Screen.width >= 2048)
				{
					_isRetinaIpad = true;
				}
				_didRetinaIpadCheck = true;
			}
			return _isRetinaIpad;
		}

		private int buttonHeight()
		{
			if (isRetinaOrLargeScreen())
			{
				if (isRetinaIpad())
				{
					return 140;
				}
				return 70;
			}
			return 30;
		}

		private int buttonFontSize()
		{
			if (isRetinaOrLargeScreen())
			{
				if (isRetinaIpad())
				{
					return 40;
				}
				return 25;
			}
			return 15;
		}

		private void paintWindow(int id)
		{
			global::UnityEngine.GUI.skin.label.alignment = global::UnityEngine.TextAnchor.UpperLeft;
			global::UnityEngine.GUI.skin.label.fontSize = buttonFontSize();
			_logScrollPosition = global::UnityEngine.GUILayout.BeginScrollView(_logScrollPosition);
			if (global::UnityEngine.GUILayout.Button("Clear Console"))
			{
				_logBuilder.Remove(0, _logBuilder.Length);
			}
			global::UnityEngine.GUILayout.Label(_logBuilder.ToString());
			global::UnityEngine.GUILayout.EndScrollView();
		}

		private void handleLog(string logString, string stackTrace, global::UnityEngine.LogType type)
		{
			_logBuilder.AppendFormat("{0}\n", logString);
		}

		private void OnDestroy()
		{
			global::UnityEngine.Application.RegisterLogCallback(null);
		}

		private void Update()
		{
			if (!_logRegistered)
			{
				global::UnityEngine.Application.RegisterLogCallback(handleLog);
				_logRegistered = true;
				_isWindowsPhone = global::UnityEngine.Application.platform.ToString().ToLower().Contains("wp8");
			}
			bool flag = false;
			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
				float num = global::UnityEngine.Time.time - _previousClickTime;
				if (num < _doubleClickDelay)
				{
					flag = true;
				}
				else
				{
					_previousClickTime = global::UnityEngine.Time.time;
				}
			}
			if (flag)
			{
				_isShowingLogConsole = !_isShowingLogConsole;
			}
		}

		protected void beginColumn()
		{
			_width = global::UnityEngine.Screen.width / 2 - 15;
			_buttonHeight = buttonHeight();
			global::UnityEngine.GUI.skin.button.fontSize = buttonFontSize();
			global::UnityEngine.GUI.skin.button.margin = new global::UnityEngine.RectOffset(0, 0, 10, 0);
			global::UnityEngine.GUI.skin.button.stretchWidth = true;
			global::UnityEngine.GUI.skin.button.fixedHeight = _buttonHeight;
			global::UnityEngine.GUI.skin.button.wordWrap = false;
			global::UnityEngine.GUI.skin.button.hover.background = normalBackground;
			global::UnityEngine.GUI.skin.button.normal.background = normalBackground;
			global::UnityEngine.GUI.skin.button.active.background = activeBackground;
			global::UnityEngine.GUI.skin.button.active.textColor = global::UnityEngine.Color.black;
			global::UnityEngine.GUI.skin.label.normal.textColor = global::UnityEngine.Color.black;
			global::UnityEngine.GUI.skin.label.fontSize = buttonFontSize();
			if (_isShowingLogConsole)
			{
				global::UnityEngine.GUILayout.BeginArea(new global::UnityEngine.Rect(0f, 0f, 0f, 0f));
			}
			else
			{
				global::UnityEngine.GUILayout.BeginArea(new global::UnityEngine.Rect(10f, 10f, _width, global::UnityEngine.Screen.height));
			}
			global::UnityEngine.GUILayout.BeginVertical();
		}

		protected void endColumn()
		{
			endColumn(false);
		}

		protected void endColumn(bool hasSecondColumn)
		{
			global::UnityEngine.GUILayout.EndVertical();
			global::UnityEngine.GUILayout.EndArea();
			if (_isShowingLogConsole)
			{
				global::UnityEngine.GUILayout.Window(1, new global::UnityEngine.Rect(0f, 0f, global::UnityEngine.Screen.width, global::UnityEngine.Screen.height), paintWindow, "prime[31] Log Console - double tap to dismiss");
			}
			if (hasSecondColumn)
			{
				beginRightColumn();
			}
		}

		private void beginRightColumn()
		{
			if (_isShowingLogConsole)
			{
				global::UnityEngine.GUILayout.BeginArea(new global::UnityEngine.Rect(0f, 0f, 0f, 0f));
			}
			else
			{
				global::UnityEngine.GUILayout.BeginArea(new global::UnityEngine.Rect((float)global::UnityEngine.Screen.width - _width - 10f, 10f, _width, global::UnityEngine.Screen.height));
			}
			global::UnityEngine.GUILayout.BeginVertical();
		}

		protected bool button(string text)
		{
			return global::UnityEngine.GUILayout.Button(text);
		}

		protected bool bottomRightButton(string text, float width = 150f)
		{
			global::UnityEngine.GUI.skin.button.hover.background = bottomButtonBackground;
			global::UnityEngine.GUI.skin.button.normal.background = bottomButtonBackground;
			width = (float)global::UnityEngine.Screen.width / 2f - 35f - 20f;
			return global::UnityEngine.GUI.Button(new global::UnityEngine.Rect((float)global::UnityEngine.Screen.width - width - 10f, (float)global::UnityEngine.Screen.height - _buttonHeight - 10f, width, _buttonHeight), text);
		}

		protected bool bottomLeftButton(string text, float width = 150f)
		{
			global::UnityEngine.GUI.skin.button.hover.background = bottomButtonBackground;
			global::UnityEngine.GUI.skin.button.normal.background = bottomButtonBackground;
			width = (float)global::UnityEngine.Screen.width / 2f - 35f - 20f;
			return global::UnityEngine.GUI.Button(new global::UnityEngine.Rect(10f, (float)global::UnityEngine.Screen.height - _buttonHeight - 10f, width, _buttonHeight), text);
		}

		protected bool bottomCenterButton(string text, float width = 150f)
		{
			global::UnityEngine.GUI.skin.button.hover.background = bottomButtonBackground;
			global::UnityEngine.GUI.skin.button.normal.background = bottomButtonBackground;
			float x = (float)(global::UnityEngine.Screen.width / 2) - width / 2f;
			return global::UnityEngine.GUI.Button(new global::UnityEngine.Rect(x, (float)global::UnityEngine.Screen.height - _buttonHeight - 10f, width, _buttonHeight), text);
		}

		protected bool toggleButton(string defaultText, string selectedText)
		{
			if (!_toggleButtons.ContainsKey(defaultText))
			{
				_toggleButtons[defaultText] = true;
			}
			string text = ((!_toggleButtons[defaultText]) ? selectedText : defaultText);
			global::UnityEngine.GUI.skin.button.normal.background = toggleButtonBackground;
			if (!_toggleButtons[defaultText])
			{
				global::UnityEngine.GUI.contentColor = global::UnityEngine.Color.yellow;
			}
			else
			{
				global::UnityEngine.GUI.skin.button.fontStyle = global::UnityEngine.FontStyle.Bold;
				global::UnityEngine.GUI.contentColor = global::UnityEngine.Color.red;
			}
			if (global::UnityEngine.GUILayout.Button(text))
			{
				_toggleButtons[defaultText] = text != defaultText;
			}
			global::UnityEngine.GUI.skin.button.normal.background = normalBackground;
			global::UnityEngine.GUI.skin.button.fontStyle = global::UnityEngine.FontStyle.Normal;
			global::UnityEngine.GUI.contentColor = global::UnityEngine.Color.white;
			return _toggleButtons[defaultText];
		}

		protected bool toggleButtonState(string defaultText)
		{
			if (!_toggleButtons.ContainsKey(defaultText))
			{
				_toggleButtons[defaultText] = true;
			}
			return _toggleButtons[defaultText];
		}

		public static void loadLevel(int level)
		{
			try
			{
				typeof(global::UnityEngine.Application).GetMethod("LoadLevel", new global::System.Type[1] { typeof(int) }).Invoke(null, new object[1] { level });
			}
			catch
			{
				try
				{
					typeof(global::UnityEngine.SceneManagement.SceneManager).GetMethod("LoadScene", new global::System.Type[1] { typeof(int) }).Invoke(null, new object[1] { level });
				}
				catch
				{
					global::UnityEngine.Debug.LogError("Could not load scene. Both Application.LoadLevel and SceneManager.LoadScene failed to load the scene.");
				}
			}
		}

		public static void loadLevel(string level)
		{
			try
			{
				typeof(global::UnityEngine.Application).GetMethod("LoadLevel", new global::System.Type[1] { typeof(string) }).Invoke(null, new object[1] { level });
			}
			catch
			{
				try
				{
					typeof(global::UnityEngine.SceneManagement.SceneManager).GetMethod("LoadScene", new global::System.Type[1] { typeof(string) }).Invoke(null, new object[1] { level });
				}
				catch
				{
					global::UnityEngine.Debug.LogError("Could not load scene. Both Application.LoadLevel and SceneManager.LoadScene failed to load the scene.");
				}
			}
		}
	}
}
