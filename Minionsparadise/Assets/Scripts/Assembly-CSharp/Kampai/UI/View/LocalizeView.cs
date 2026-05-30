namespace Kampai.UI.View
{
	[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.UI.Text))]
	public class LocalizeView : global::Kampai.Util.KampaiView
	{
		public enum TextCaseType
		{
			None = 0,
			Lower = 1,
			Upper = 2
		}

		[global::UnityEngine.Tooltip("The Localized Key to be looked in the correct localized source.")]
		[global::UnityEngine.SerializeField]
		private string Key = string.Empty;

		[global::UnityEngine.SerializeField]
		[global::UnityEngine.Tooltip("Tell the view what type of case to apply to the text view when it is displayed.")]
		private global::Kampai.UI.View.LocalizeView.TextCaseType m_textCaseType;

		private bool m_isOverriden;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LocalizeView") as global::Kampai.Util.IKampaiLogger;

		private global::UnityEngine.UI.Text textView;

		public global::Kampai.UI.View.LocalizeView.TextCaseType TextCase
		{
			get
			{
				return m_textCaseType;
			}
			set
			{
				m_textCaseType = value;
				Translate();
			}
		}

		public string LocKey
		{
			get
			{
				return Key;
			}
			set
			{
				Key = value;
				Translate();
				m_isOverriden = false;
			}
		}

		public string text
		{
			get
			{
				return textView.text;
			}
			set
			{
				SetTextInternal(value);
				m_isOverriden = true;
			}
		}

		private void SetTextInternal(string value)
		{
			if (textView == null)
			{
				textView = GetComponent<global::UnityEngine.UI.Text>();
			}
			textView.text = ((!(textView.name == "txt_ItemQuantity")) ? GetCaseString(value) : ("x" + GetCaseString(value)));
		}

		public global::UnityEngine.Color color
		{
			get
			{
				return textView.color;
			}
			set
			{
				if (textView == null)
				{
					textView = GetComponent<global::UnityEngine.UI.Text>();
				}
				textView.color = value;
			}
		}

		[Inject]
		public global::Kampai.Main.ILocalizationService service { get; set; }

		[Inject]
		public global::Kampai.Main.LanguageChangedSignal languageChangedSignal { get; set; }

		protected override void Awake()
		{
			base.Awake();
			textView = GetComponent<global::UnityEngine.UI.Text>();
		}

		private void OnEnable()
		{
			global::Kampai.Util.KampaiView.BubbleToContextOnStart(this, ref currentContext);
			if (languageChangedSignal != null)
			{
				languageChangedSignal.AddListener(OnLanguageChanged);
			}
			if (!m_isOverriden && !string.IsNullOrEmpty(LocKey))
			{
				Translate();
			}
		}

		private void OnDisable()
		{
			if (languageChangedSignal != null)
			{
				languageChangedSignal.RemoveListener(OnLanguageChanged);
			}
		}

		private void OnLanguageChanged()
		{
			Translate();
		}

		protected override void Start()
		{
			base.Start();
			if (textView == null)
			{
				logger.Error("LocalizeView: GameObject {0} is missing parent component Text!", base.name);
			}
			else if (!m_isOverriden && !string.IsNullOrEmpty(LocKey))
			{
				Translate();
			}
		}

		public void Format(bool useLocKey, params object[] args)
		{
			text = string.Format((!useLocKey) ? text : Key, args);
		}

		public void Format(string locKey, params object[] args)
		{
			Key = locKey;
			if (service == null)
			{
				logger.Error("service is null!");
			}
			else
			{
				text = service.GetString(locKey, args);
			}
		}

		private void Translate()
		{
			if (service != null)
			{
				SetTextInternal(service.GetString(Key));
			}
		}

		private string GetCaseString(string str)
		{
			switch (m_textCaseType)
			{
			case global::Kampai.UI.View.LocalizeView.TextCaseType.Lower:
				str = service.StringToLower(str);
				break;
			case global::Kampai.UI.View.LocalizeView.TextCaseType.Upper:
				str = service.StringToUpper(str);
				break;
			}
			return str;
		}
	}
}
