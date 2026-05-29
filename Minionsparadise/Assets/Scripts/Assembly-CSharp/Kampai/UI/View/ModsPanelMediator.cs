using System.Collections.Generic;
using Kampai.Game;
using Kampai.Main;
using Kampai.Util;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Kampai.UI.View
{
	public class ModsPanelMediator : Mediator
	{
		[Inject]
		public ModsPanelView view { get; set; }

		[Inject]
		public ILocalizationService localService { get; set; }

		[Inject]
		public IDevicePrefsService prefs { get; set; }

		[Inject]
		public SaveDevicePrefsSignal saveDevicePrefsSignal { get; set; }

		[Inject]
		public LanguageChangedSignal languageChangedSignal { get; set; }

		[Inject]
		public IConfigurationsService configurationsService { get; set; }

		[Inject]
		public PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		[Inject]
		public IGlobalChatService chatService { get; set; }

		[Inject]
		public GlobalChatUpdateSignal chatUpdateSignal { get; set; }

		[Inject]
		public GlobalChatErrorSignal chatErrorSignal { get; set; }

		[Inject]
		public PopupMessageSignal popupMessageSignal { get; set; }
		
		[Inject]
		public LoadDefinitionForUISignal loadDefSignal { get; set; }
		
		[Inject]
		public ClearStoreTabsSignal clearTabsSignal { get; set; }

		private static readonly string[] ALL_LANGUAGES = new string[] { "en", "fr", "de", "es", "it", "pt", "nl", "ko", "ru", "ja", "zh-cn", "zh-tw", "tr", "id", "lolcat", "minion" };

		private List<string> m_availableLanguages;
		private HashSet<string> m_renderedTimestamps = new HashSet<string>();
		private Dictionary<string, GameObject> m_pendingMessages = new Dictionary<string, GameObject>();
		
		private float m_lastClickTime = 0f;
		private const float CLICK_DEBOUNCE = 0.2f;

		public override void OnRegister()
		{
			Debug.Log("[ModsMediator] Registering Mods Panel Logic...");

			m_availableLanguages = new List<string>(ALL_LANGUAGES);
			if (configurationsService.GetConfigurations() == null || !configurationsService.GetConfigurations().AprilsFool)
			{
				m_availableLanguages.Remove("lolcat");
				m_availableLanguages.Remove("minion");
			}

			BindButton(view.languageButton);
			BindButton(view.nightToggleButton);
			BindButton(view.offlineToggleButton);

			if (view.languageButton != null)
			{
				view.languageButton.ClickedSignal.AddListener(OnLanguageButtonClicked);
			}

			if (view.nightToggleButton != null)
			{
				view.nightToggleButton.ClickedSignal.AddListener(OnNightToggleClicked);
			}
			
			if (view.offlineToggleButton != null)
			{
				view.offlineToggleButton.ClickedSignal.AddListener(OnOfflineToggleClicked);
			}

			UpdateLanguageText();
			UpdateNightToggleText();
			UpdateOfflineToggleText();

			EnsureChatLayout();

			if (view.sendButton != null)
			{
				view.sendButton.ClickedSignal.AddListener(OnSendClicked);
			}

			chatUpdateSignal.AddListener(OnChatUpdated);
			chatErrorSignal.AddListener(OnChatError);

			chatService.StartPolling();
			OnChatUpdated(chatService.GetCachedMessages());
		}

		private void BindButton(ButtonView bv)
		{
			if (bv == null) return;
			Button btn = bv.GetComponent<Button>();
			if (btn != null)
			{
				Debug.Log(string.Format("[ModsMediator] Programmatically binding onClick for {0}", bv.name));
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(bv.OnClickEvent);
			}
			else
			{
				Debug.LogWarning(string.Format("[ModsMediator] {0} is missing a native Button component!", bv.name));
			}
		}

		public override void OnRemove()
		{
			if (view.languageButton != null)
			{
				view.languageButton.ClickedSignal.RemoveListener(OnLanguageButtonClicked);
			}

			if (view.nightToggleButton != null)
			{
				view.nightToggleButton.ClickedSignal.RemoveListener(OnNightToggleClicked);
			}
			
			if (view.offlineToggleButton != null)
			{
				view.offlineToggleButton.ClickedSignal.RemoveListener(OnOfflineToggleClicked);
			}

			if (view.sendButton != null)
			{
				view.sendButton.ClickedSignal.RemoveListener(OnSendClicked);
			}

			chatUpdateSignal.RemoveListener(OnChatUpdated);
			chatErrorSignal.RemoveListener(OnChatError);
			
			chatService.StopPolling();
		}

		private void UpdateLanguageText()
		{
			if (view.languageText != null)
			{
				view.languageText.text = localService.GetLanguage().ToUpper();
			}
		}

		private void OnLanguageButtonClicked()
		{
			if (Time.time - m_lastClickTime < CLICK_DEBOUNCE) return;
			m_lastClickTime = Time.time;

			Debug.Log("[ModsMediator] Language Button Clicked!");
			string language = prefs.GetDevicePrefs().Language;
			if (string.IsNullOrEmpty(language))
			{
				language = Native.GetDeviceLanguage();
			}
			language = language.ToLower();
			int num = m_availableLanguages.IndexOf(language);
			if (num == -1)
			{
				num = 0;
			}
			num = (num + 1) % m_availableLanguages.Count;
			string nextLang = m_availableLanguages[num];
			
			Debug.Log(string.Format("[ModsMediator] Switching language from {0} to {1}", language, nextLang));
			
			prefs.GetDevicePrefs().Language = nextLang;
			saveDevicePrefsSignal.Dispatch();
			localService.Initialize(nextLang);
			localService.Update();
			UpdateLanguageText();
			
			// Refresh Build Shop Tabs
			clearTabsSignal.Dispatch();
			loadDefSignal.Dispatch();
			
			languageChangedSignal.Dispatch();
			soundFXSignal.Dispatch("Play_minion_confirm_select_01");
		}

		private void OnNightToggleClicked()
		{
			if (Time.time - m_lastClickTime < CLICK_DEBOUNCE) return;
			m_lastClickTime = Time.time;

			Debug.Log("[ModsMediator] Night Toggle Clicked!");
			DayNightCycleManager manager = Object.FindObjectOfType<DayNightCycleManager>();
			if (manager != null)
			{
				manager.CycleNightMode();
				UpdateNightToggleText();
				Debug.Log(string.Format("[ModsMediator] Night mode now: {0}", manager.GetCurrentMode()));
				soundFXSignal.Dispatch("Play_minion_confirm_select_02");
			}
			else
			{
				Debug.LogError("[ModsMediator] Could not find DayNightCycleManager in scene!");
			}
		}

		private void UpdateNightToggleText()
		{
			if (view.nightToggleText != null)
			{
				DayNightCycleManager manager = Object.FindObjectOfType<DayNightCycleManager>();
				if (manager != null)
				{
					view.nightToggleText.text = "MODE: " + manager.GetCurrentMode().ToString();
				}
				else
				{
					view.nightToggleText.text = "NIGHT: NO MGR";
				}
			}
		}

		private void OnOfflineToggleClicked()
		{
			if (Time.time - m_lastClickTime < CLICK_DEBOUNCE) return;
			m_lastClickTime = Time.time;

			DevicePrefs devicePrefs = prefs.GetDevicePrefs();
			devicePrefs.OfflineMode_Pref = !devicePrefs.OfflineMode_Pref;
			saveDevicePrefsSignal.Dispatch();
			UpdateOfflineToggleText();
			soundFXSignal.Dispatch("Play_minion_confirm_select_01");
			Debug.Log(string.Format("[ModsMediator] Offline Preference toggled to: {0}", devicePrefs.OfflineMode_Pref));
		}

		private void UpdateOfflineToggleText()
		{
			bool offlinePref = prefs.GetDevicePrefs().OfflineMode_Pref;
			string newText = "OFFLINE: " + (offlinePref ? "ON" : "OFF");
			
			if (view.offlineToggleText != null)
			{
				view.offlineToggleText.text = newText;
			}
		}


		private void OnSendClicked()
		{
			if (view.chatInput != null && !string.IsNullOrEmpty(view.chatInput.text))
			{
				string text = view.chatInput.text;
				
				// Optimistic Update: Show message immediately in UI
				Debug.Log("[ModsMediator] Optimistically adding message to UI.");
				ChatMessage localMsg = new ChatMessage();
				localMsg.user = "You";
				localMsg.text = text;
				localMsg.timestamp = System.DateTime.Now.ToString("HH:mm:ss");
				
				GameObject go = CreateChatItem(localMsg, true);
				if (go != null)
				{
					// Track this pending message by its text so we can replace it later
					m_pendingMessages[text] = go;
				}

				chatService.SendMessage(text);
				view.chatInput.text = string.Empty;
				soundFXSignal.Dispatch("Play_button_click_01");
			}
		}

		private void OnChatUpdated(List<ChatMessage> messages)
		{
			if (messages == null) return;
			
			// Legacy text fallback
			if (view.chatDisplayText != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				for (int i = 0; i < messages.Count; i++)
				{
					ChatMessage msg = messages[i];
					sb.AppendFormat("<b>{0}:</b> {1}\n", msg.user, msg.text);
				}
				view.chatDisplayText.text = sb.ToString();
			}

			// New Dynamic Item Spawner
			if (view.chatItemPrefab != null && view.chatItemsContainer != null)
			{
				foreach (var msg in messages)
				{
					// Use a key combination to avoid duplicates (timestamp + user + first 20 chars of text)
					string key = msg.timestamp + msg.user + (msg.text.Length > 20 ? msg.text.Substring(0, 20) : msg.text);
					if (!m_renderedTimestamps.Contains(key))
					{
						// Check if this confirms a pending message
						if (m_pendingMessages.ContainsKey(msg.text))
						{
							Debug.Log("[ModsMediator] Server confirmed message. Removing optimistic item.");
							Object.Destroy(m_pendingMessages[msg.text]);
							m_pendingMessages.Remove(msg.text);
						}

						CreateChatItem(msg, false);
						m_renderedTimestamps.Add(key);
					}
				}
			}
		}

		private void EnsureChatLayout()
		{
			if (view.chatItemsContainer == null) return;

			Debug.Log("[ModsMediator] Auto-configuring Chat Container Layout...");
			
			// 1. Ensure Pivot is at the Top so it grows downwards
			RectTransform containerRect = view.chatItemsContainer.GetComponent<RectTransform>();
			if (containerRect != null)
			{
				containerRect.pivot = new Vector2(0.5f, 1f);
				containerRect.anchorMin = new Vector2(0f, 1f);
				containerRect.anchorMax = new Vector2(1f, 1f);
			}

			// 2. Add/Configure Vertical Layout Group
			VerticalLayoutGroup layout = view.chatItemsContainer.GetComponent<VerticalLayoutGroup>();
			if (layout == null)
			{
				layout = view.chatItemsContainer.gameObject.AddComponent<VerticalLayoutGroup>();
			}
			layout.padding = new RectOffset(0, 0, 0, 0);
			layout.spacing = 0f;
			layout.childAlignment = TextAnchor.UpperCenter;
			layout.childForceExpandHeight = false;
			layout.childForceExpandWidth = true;

			// 3. Add/Configure Content Size Fitter
			ContentSizeFitter fitter = view.chatItemsContainer.GetComponent<ContentSizeFitter>();
			if (fitter == null)
			{
				fitter = view.chatItemsContainer.gameObject.AddComponent<ContentSizeFitter>();
			}
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		private GameObject CreateChatItem(ChatMessage msg, bool isOptimistic)
		{
			if (view.chatItemPrefab == null || view.chatItemsContainer == null) return null;

			// Get the prefab's default dimensions to calculate aspect ratio
			RectTransform prefabRect = view.chatItemPrefab.GetComponent<RectTransform>();
			float prefabWidth = (prefabRect != null) ? prefabRect.rect.width : 100f;
			float prefabHeight = (prefabRect != null) ? prefabRect.rect.height : 50f;

			// Instantiate
			GameObject go = Object.Instantiate(view.chatItemPrefab, view.chatItemsContainer) as GameObject;
			
			RectTransform rt = go.GetComponent<RectTransform>();
			if (rt != null)
			{
				rt.SetParent(view.chatItemsContainer, false);
				
				// 1. Force Pivots and Anchors
				rt.anchorMin = new Vector2(0f, 1f);
				rt.anchorMax = new Vector2(1f, 1f);
				rt.pivot = new Vector2(0.5f, 1f);
				
				// 2. Scaling
				RectTransform containerRect = view.chatItemsContainer.GetComponent<RectTransform>();
				float containerWidth = (containerRect != null) ? containerRect.rect.width : prefabWidth;
				float scaleFactor = containerWidth / prefabWidth;
				rt.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
				
				// 3. Layout
				float scaledHeight = prefabHeight * scaleFactor;
				LayoutElement le = go.GetComponent<LayoutElement>();
				if (le == null) le = go.AddComponent<LayoutElement>();
				le.preferredHeight = scaledHeight;
				le.minHeight = scaledHeight;
				
				rt.sizeDelta = new Vector2(prefabWidth, prefabHeight);
				rt.localPosition = Vector3.zero;
			}
			
			go.SetActive(true);
			
			ChatItemView item = go.GetComponent<ChatItemView>();
			if (item != null)
			{
				string userLabel = isOptimistic ? "<b>" + msg.user + " (Sending...)</b>" : "<b>" + msg.user + ":</b>";
				item.Setup(userLabel, msg.text, msg.timestamp);
			}

			if (view.chatItemsContainer.childCount > 50)
			{
				Object.Destroy(view.chatItemsContainer.GetChild(0).gameObject);
			}
			
			if (view.chatScrollView != null)
			{
				AutoScrollToBottom autoScroll = view.chatScrollView.GetComponent<AutoScrollToBottom>();
				if (autoScroll != null)
				{
					autoScroll.ScrollToBottom();
				}
			}

			return go;
		}

		private void OnChatError(string error)
		{
			Debug.LogError("[ModsMediator] Chat Error: " + error);
			// popupMessageSignal.Dispatch("Chat Error: " + error, PopupMessageType.NORMAL);
		}
	}
}
