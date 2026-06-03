public class SwrveSDK
{
	protected class CoroutineReference<T>
	{
		private T val;

		public CoroutineReference()
		{
		}

		public CoroutineReference(T val)
		{
			this.val = val;
		}

		public T Value()
		{
			return val;
		}

		public void Value(T val)
		{
			this.val = val;
		}
	}

	public const string SdkVersion = "3.4.3";

	private const string Platform = "Unity ";

	private const float DefaultDPI = 160f;

	protected const string EventsSave = "Swrve_Events";

	protected const string InstallTimeEpochSave = "Swrve_JoinedDate";

	protected const string iOSdeviceTokenSave = "Swrve_iOSDeviceToken";

	protected const string GcmdeviceTokenSave = "Swrve_gcmDeviceToken";

	protected const string AbTestUserResourcesSave = "srcngt2";

	protected const string AbTestUserResourcesDiffSave = "rsdfngt2";

	protected const string DeviceIdSave = "Swrve_DeviceId";

	protected const string SeqNumSave = "Swrve_SeqNum";

	protected const string ResourcesCampaignTagSave = "cmpg_etag";

	protected const string ResourcesCampaignFlushFrequencySave = "swrve_cr_flush_frequency";

	protected const string ResourcesCampaignFlushDelaySave = "swrve_cr_flush_delay";

	private const string DeviceIdKey = "Swrve.deviceUniqueIdentifier";

	private const string EmptyJSONObject = "{}";

	private const float DefaultCampaignResourcesFlushFrenquency = 60f;

	private const float DefaultCampaignResourcesFlushRefreshDelay = 5f;

	public const string DefaultAutoShowMessagesTrigger = "Swrve.Messages.showAtSessionStart";

	private const string SwrveAndroidPushPluginPackageName = "com.swrve.unity.gcm.SwrveGcmDeviceRegistration";

	private const int DefaultDelayFirstMessage = 150;

	private const long DefaultMaxShows = 99999L;

	private const int DefaultMinDelay = 55;

	private const int GooglePlayPushPluginVersion = 3;

	private int gameId;

	private string apiKey;

	protected string userId;

	protected global::Swrve.SwrveConfig config;

	public string Language;

	public global::Swrve.ResourceManager.SwrveResourceManager ResourceManager;

	public global::UnityEngine.MonoBehaviour Container;

	public global::Swrve.Messaging.ISwrveInstallButtonListener GlobalInstallButtonListener;

	public global::Swrve.Messaging.ISwrveCustomButtonListener GlobalCustomButtonListener;

	public global::Swrve.Messaging.ISwrveMessageListener GlobalMessageListener;

	public global::Swrve.ISwrvePushNotificationListener PushNotificationListener;

	public global::Swrve.Messaging.ISwrveTriggeredMessageListener TriggeredMessageListener;

	public global::System.Action ResourcesUpdatedCallback;

	public bool Initialised;

	public bool Destroyed;

	private string escapedUserId;

	private long installTimeEpoch;

	private string installTimeFormatted;

	private string lastPushEngagedId;

	private string gcmDeviceToken;

	private static global::UnityEngine.AndroidJavaObject androidPlugin;

	private static bool androidPluginInitialized;

	private static bool androidPluginInitializedSuccessfully;

	private int deviceWidth;

	private int deviceHeight;

	private long lastSessionTick;

	private global::Swrve.Device.ICarrierInfo deviceCarrierInfo;

	protected global::System.Text.StringBuilder eventBufferStringBuilder;

	protected string eventsPostString;

	protected string swrvePath;

	protected global::Swrve.ISwrveStorage storage;

	protected global::Swrve.REST.IRESTClient restClient;

	private string eventsUrl;

	private string abTestResourcesDiffUrl;

	protected bool eventsConnecting;

	protected bool abTestUserResourcesDiffConnecting;

	protected string userResourcesRaw;

	protected global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> userResources;

	protected float campaignsAndResourcesFlushFrequency;

	protected float campaignsAndResourcesFlushRefreshDelay;

	protected string lastETag;

	protected long campaignsAndResourcesLastRefreshed;

	protected bool campaignsAndResourcesInitialized;

	private static readonly int CampaignAPIVersion = 1;

	private static readonly int CampaignEndpointVersion = 4;

	protected static readonly string CampaignsSave = "cmcc2";

	protected static readonly string CampaignsSettingsSave = "Swrve_CampaignsData";

	private static readonly string WaitTimeFormat = "HH\\:mm\\:ss zzz";

	protected static readonly string InstallTimeFormat = "yyyyMMdd";

	private string resourcesAndCampaignsUrl;

	protected string swrveTemporaryPath;

	protected bool campaignsConnecting;

	protected bool autoShowMessagesEnabled;

	protected bool assetsCurrentlyDownloading;

	protected global::System.Collections.Generic.HashSet<string> assetsOnDisk;

	protected global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveCampaign> campaigns = new global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveCampaign>();

	protected global::System.Collections.Generic.Dictionary<string, object> campaignSettings = new global::System.Collections.Generic.Dictionary<string, object>();

	protected global::System.Collections.Generic.Dictionary<string, string> gameStoreLinks = new global::System.Collections.Generic.Dictionary<string, string>();

	protected global::Swrve.Messaging.SwrveMessageFormat currentMessage;

	protected global::Swrve.Messaging.SwrveMessageFormat currentDisplayingMessage;

	protected global::Swrve.Messaging.SwrveOrientation currentOrientation;

	protected global::Swrve.Input.IInputManager inputManager = global::Swrve.Input.NativeInputManager.Instance;

	private string cdn = "http://swrve-content.s3.amazonaws.com/messaging/message_image/";

	private global::System.DateTime initialisedTime;

	private global::System.DateTime showMessagesAfterLaunch;

	private global::System.DateTime showMessagesAfterDelay;

	private long messagesLeftToShow;

	private int minDelayBetweenMessage;

	protected global::Swrve.Messaging.SwrveQAUser qaUser;

	private bool campaignAndResourcesCoroutineEnabled = true;

	private global::System.Collections.IEnumerator campaignAndResourcesCoroutineInstance;

	public int GameId
	{
		get
		{
			return gameId;
		}
	}

	public string ApiKey
	{
		get
		{
			return apiKey;
		}
	}

	public string UserId
	{
		get
		{
			return userId;
		}
	}

	public global::UnityEngine.Color? DefaultBackgroundColor
	{
		get
		{
			return config.DefaultBackgroundColor;
		}
	}

	public void Init(global::UnityEngine.MonoBehaviour container, int gameId, string apiKey)
	{
		Init(container, gameId, apiKey, new global::Swrve.SwrveConfig());
	}

	public void Init(global::UnityEngine.MonoBehaviour container, int gameId, string apiKey, string userId)
	{
		global::Swrve.SwrveConfig swrveConfig = new global::Swrve.SwrveConfig();
		swrveConfig.UserId = userId;
		Init(container, gameId, apiKey, swrveConfig);
	}

	public virtual void Init(global::UnityEngine.MonoBehaviour container, int gameId, string apiKey, string userId, global::Swrve.SwrveConfig config)
	{
		config.UserId = userId;
		Init(container, gameId, apiKey, config);
	}

	public virtual void Init(global::UnityEngine.MonoBehaviour container, int gameId, string apiKey, global::Swrve.SwrveConfig config)
	{
		Container = container;
		ResourceManager = new global::Swrve.ResourceManager.SwrveResourceManager();
		this.config = config;
		this.gameId = gameId;
		this.apiKey = apiKey;
		userId = config.UserId;
		Language = config.Language;
		lastSessionTick = GetSessionTime();
		initialisedTime = global::Swrve.Helpers.SwrveHelper.GetNow();
		campaignsAndResourcesInitialized = false;
		autoShowMessagesEnabled = true;
		assetsOnDisk = new global::System.Collections.Generic.HashSet<string>();
		assetsCurrentlyDownloading = false;
		if (string.IsNullOrEmpty(apiKey))
		{
			throw new global::System.Exception("The api key has not been specified.");
		}
		if (string.IsNullOrEmpty(userId))
		{
			userId = GetDeviceUniqueId();
		}
		if (!string.IsNullOrEmpty(userId))
		{
			global::UnityEngine.PlayerPrefs.SetString("Swrve.deviceUniqueIdentifier", userId);
			global::UnityEngine.PlayerPrefs.Save();
		}
		SwrveLog.Log("Your user id is: " + userId);
		escapedUserId = global::System.Uri.EscapeDataString(userId);
		if (string.IsNullOrEmpty(Language))
		{
			Language = GetDeviceLanguage();
			if (string.IsNullOrEmpty(Language))
			{
				Language = config.DefaultLanguage;
			}
		}
		config.CalculateEndpoints(gameId);
		string contentServer = config.ContentServer;
		eventsUrl = config.EventsServer + "/1/batch";
		abTestResourcesDiffUrl = contentServer + "/api/1/user_resources_diff";
		resourcesAndCampaignsUrl = contentServer + "/api/1/user_resources_and_campaigns";
		swrvePath = GetSwrvePath();
		if (storage == null)
		{
			storage = CreateStorage();
		}
		storage.SetSecureFailedListener(delegate
		{
			NamedEventInternal("Swrve.signature_invalid", null, false);
		});
		restClient = CreateRestClient();
		eventBufferStringBuilder = new global::System.Text.StringBuilder(config.MaxBufferChars);
		string savedInstallTimeEpoch = GetSavedInstallTimeEpoch();
		LoadData();
		InitUserResources();
		deviceCarrierInfo = new global::Swrve.Device.DeviceCarrierInfo();
		GetDeviceScreenInfo();
		Initialised = true;
		if (config.AutomaticSessionManagement)
		{
			QueueSessionStart();
			GenerateNewSessionInterval();
		}
		if (string.IsNullOrEmpty(savedInstallTimeEpoch))
		{
			NamedEvent("Swrve.first_session");
		}
		if (config.PushNotificationEnabled && !string.IsNullOrEmpty(config.GCMSenderId))
		{
			GooglePlayRegisterForPushNotification(Container, config.GCMSenderId);
		}
		QueueDeviceInfo();
		SendQueuedEvents();
		if (config.TalkEnabled)
		{
			if (string.IsNullOrEmpty(Language))
			{
				throw new global::System.Exception("Language needed to use Talk");
			}
			if (string.IsNullOrEmpty(config.AppStore))
			{
				config.AppStore = "google";
			}
			try
			{
				swrveTemporaryPath = GetSwrveTemporaryCachePath();
				LoadTalkData();
			}
			catch (global::System.Exception ex)
			{
				SwrveLog.LogError("Error while initializing " + ex);
			}
		}
		StartCampaignsAndResourcesTimer();
		DisableAutoShowAfterDelay();
	}

	public void SessionStart()
	{
		QueueSessionStart();
		SendQueuedEvents();
	}

	public void SessionEnd()
	{
		global::System.Collections.Generic.Dictionary<string, object> eventParameters = new global::System.Collections.Generic.Dictionary<string, object>();
		AppendEventToBuffer("session_end", eventParameters);
	}

	public virtual void NamedEvent(string name, global::System.Collections.Generic.Dictionary<string, string> payload = null)
	{
		NamedEventInternal(name, payload);
	}

	public void UserUpdate(global::System.Collections.Generic.Dictionary<string, string> attributes)
	{
		if (attributes != null && attributes.Count > 0)
		{
			global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
			dictionary.Add("attributes", attributes);
			AppendEventToBuffer("user", dictionary);
		}
		else
		{
			SwrveLog.LogError("Invoked user update with no update attributes");
		}
	}

	public void Purchase(string item, string currency, int cost, int quantity)
	{
		global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
		dictionary.Add("item", item);
		dictionary.Add("currency", currency);
		dictionary.Add("cost", cost);
		dictionary.Add("quantity", quantity);
		AppendEventToBuffer("purchase", dictionary);
	}

	public void Iap(int quantity, string productId, double productPrice, string currency)
	{
		IapRewards rewards = new IapRewards();
		Iap(quantity, productId, productPrice, currency, rewards);
	}

	public void Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards)
	{
		_Iap(quantity, productId, productPrice, currency, rewards, string.Empty, string.Empty, string.Empty, "unknown_store");
	}

	public void IapGooglePlay(string productId, double productPrice, string currency, string purchaseData, string dataSignature)
	{
		IapRewards rewards = new IapRewards();
		IapGooglePlay(productId, productPrice, currency, rewards, purchaseData, dataSignature);
	}

	public void IapGooglePlay(string productId, double productPrice, string currency, IapRewards rewards, string purchaseData, string dataSignature)
	{
		if (config.AppStore != "google")
		{
			throw new global::System.Exception("This function can only be called to validate IAP events from Google");
		}
		if (string.IsNullOrEmpty(purchaseData))
		{
			SwrveLog.LogError("IAP event not sent: purchase data cannot be empty for Google Play Store verification");
		}
		else if (string.IsNullOrEmpty(dataSignature))
		{
			SwrveLog.LogError("IAP event not sent: data signature cannot be empty for Google Play Store verification");
		}
		else
		{
			_Iap(1, productId, productPrice, currency, rewards, purchaseData, dataSignature, string.Empty, config.AppStore);
		}
	}

	public void CurrencyGiven(string givenCurrency, double amount)
	{
		global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
		dictionary.Add("given_currency", givenCurrency);
		dictionary.Add("given_amount", amount);
		AppendEventToBuffer("currency_given", dictionary);
	}

	public bool SendQueuedEvents()
	{
		bool result = false;
		if (Initialised)
		{
			if (!eventsConnecting)
			{
				byte[] array = null;
				if (eventsPostString == null || eventsPostString.Length == 0)
				{
					eventsPostString = eventBufferStringBuilder.ToString();
					eventBufferStringBuilder.Length = 0;
				}
				if (eventsPostString.Length > 0)
				{
					long seconds = global::Swrve.Helpers.SwrveHelper.GetSeconds();
					array = global::Swrve.Helpers.PostBodyBuilder.Build(apiKey, gameId, userId, GetDeviceId(), GetAppVersion(), seconds, eventsPostString);
				}
				if (array != null)
				{
					eventsConnecting = true;
					SwrveLog.Log("Sending events to Swrve");
					global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
					dictionary.Add("Content-Type", "application/json; charset=utf-8");
					dictionary.Add("Content-Length", array.Length.ToString());
					global::System.Collections.Generic.Dictionary<string, string> requestHeaders = dictionary;
					result = true;
					StartTask("PostEvents_Coroutine", PostEvents_Coroutine(requestHeaders, array));
				}
				else
				{
					eventsPostString = null;
				}
			}
			else
			{
				SwrveLog.LogWarning("Sending events already in progress");
			}
		}
		return result;
	}

	public void GetUserResources(global::System.Action<global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>, string> onResult, global::System.Action<global::System.Exception> onError)
	{
		if (Initialised)
		{
			if (userResources != null)
			{
				onResult(userResources, userResourcesRaw);
			}
			else
			{
				onResult(new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>(), "[]");
			}
		}
	}

	public void GetUserResourcesDiff(global::System.Action<global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>, global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>, string> onResult, global::System.Action<global::System.Exception> onError)
	{
		if (Initialised && !abTestUserResourcesDiffConnecting)
		{
			abTestUserResourcesDiffConnecting = true;
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(abTestResourcesDiffUrl);
			stringBuilder.AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", escapedUserId, apiKey, global::System.Uri.EscapeDataString(GetAppVersion()), installTimeEpoch);
			SwrveLog.Log("AB Test User Resources Diff request: " + stringBuilder.ToString());
			StartTask("GetUserResourcesDiff_Coroutine", GetUserResourcesDiff_Coroutine(stringBuilder.ToString(), onResult, onError, "rsdfngt2"));
		}
		else
		{
			string message = "Failed to initiate A/B test Diff GET request";
			SwrveLog.LogError(message);
			if (onError != null)
			{
				onError(new global::System.Exception(message));
			}
		}
	}

	public void LoadFromDisk()
	{
		LoadEventsFromDisk();
	}

	public void FlushToDisk(bool saveEventsBeingSent = false)
	{
		if (!Initialised || eventBufferStringBuilder == null)
		{
			return;
		}
		global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
		string text = eventBufferStringBuilder.ToString();
		eventBufferStringBuilder.Length = 0;
		if (saveEventsBeingSent)
		{
			stringBuilder.Append(eventsPostString);
			eventsPostString = null;
			if (text.Length > 0)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(text);
			}
		}
		else
		{
			stringBuilder.Append(text);
		}
		try
		{
			string value = storage.Load("Swrve_Events", userId);
			if (!string.IsNullOrEmpty(value))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(value);
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogWarning("Could not read events from cache (" + ex.ToString() + ")");
		}
		string data = stringBuilder.ToString();
		storage.Save("Swrve_Events", data, userId);
	}

	public string BasePath()
	{
		return swrvePath;
	}

	public global::System.Collections.Generic.Dictionary<string, string> GetDeviceInfo()
	{
		string deviceModel = GetDeviceModel();
		string operatingSystem = global::UnityEngine.SystemInfo.operatingSystem;
		string value = "Android";
		float num = ((global::UnityEngine.Screen.dpi != 0f) ? global::UnityEngine.Screen.dpi : 160f);
		global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
		dictionary.Add("swrve.device_name", deviceModel);
		dictionary.Add("swrve.os", value);
		dictionary.Add("swrve.device_width", deviceWidth.ToString());
		dictionary.Add("swrve.device_height", deviceHeight.ToString());
		dictionary.Add("swrve.device_dpi", num.ToString());
		dictionary.Add("swrve.language", Language);
		dictionary.Add("swrve.os_version", operatingSystem);
		dictionary.Add("swrve.app_store", config.AppStore);
		dictionary.Add("swrve.sdk_version", "Unity 3.4.3");
		dictionary.Add("swrve.unity_version", global::UnityEngine.Application.unityVersion);
		dictionary.Add("swrve.install_date", installTimeFormatted);
		global::System.Collections.Generic.Dictionary<string, string> dictionary2 = dictionary;
		string value2 = global::System.DateTimeOffset.Now.Offset.TotalSeconds.ToString();
		dictionary2["swrve.utc_offset_seconds"] = value2;
		if (!string.IsNullOrEmpty(gcmDeviceToken))
		{
			dictionary2["swrve.gcm_token"] = gcmDeviceToken;
		}
		string value3 = AndroidGetTimezone();
		if (!string.IsNullOrEmpty(value3))
		{
			dictionary2["swrve.timezone_name"] = value3;
		}
		global::Swrve.Device.ICarrierInfo carrierInfoProvider = GetCarrierInfoProvider();
		if (carrierInfoProvider != null)
		{
			string name = carrierInfoProvider.GetName();
			if (!string.IsNullOrEmpty(name))
			{
				dictionary2["swrve.sim_operator.name"] = name;
			}
			string isoCountryCode = carrierInfoProvider.GetIsoCountryCode();
			if (!string.IsNullOrEmpty(isoCountryCode))
			{
				dictionary2["swrve.sim_operator.iso_country_code"] = isoCountryCode;
			}
			string carrierCode = carrierInfoProvider.GetCarrierCode();
			if (!string.IsNullOrEmpty(carrierCode))
			{
				dictionary2["swrve.sim_operator.code"] = carrierCode;
			}
		}
		return dictionary2;
	}

	public void OnSwrvePause()
	{
		if (Initialised)
		{
			FlushToDisk();
			GenerateNewSessionInterval();
			if (config != null && config.AutoDownloadCampaignsAndResources)
			{
				StopCheckForCampaignAndResources();
			}
		}
	}

	public void OnSwrveResume()
	{
		if (Initialised)
		{
			LoadFromDisk();
			QueueDeviceInfo();
			long sessionTime = GetSessionTime();
			if (sessionTime >= lastSessionTick)
			{
				SessionStart();
			}
			else
			{
				SendQueuedEvents();
			}
			GenerateNewSessionInterval();
			StartCampaignsAndResourcesTimer();
			DisableAutoShowAfterDelay();
		}
	}

	public void OnSwrveDestroy()
	{
		if (!Destroyed)
		{
			Destroyed = true;
			if (Initialised)
			{
				FlushToDisk(true);
			}
			if (config != null && config.AutoDownloadCampaignsAndResources)
			{
				StopCheckForCampaignAndResources();
			}
		}
	}

	public global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveCampaign> GetCampaigns()
	{
		return campaigns;
	}

	public void ButtonWasPressedByUser(global::Swrve.Messaging.SwrveButton button)
	{
		if (button == null)
		{
			return;
		}
		try
		{
			SwrveLog.Log(string.Concat("Button ", button.ActionType, ": ", button.Action, " game id: ", button.GameId));
			if (button.ActionType != global::Swrve.Messaging.SwrveActionType.Dismiss)
			{
				string text = "Swrve.Messages.Message-" + button.Message.Id + ".click";
				SwrveLog.Log("Sending click event: " + text);
				global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
				dictionary.Add("name", button.Name);
				NamedEventInternal(text, dictionary);
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Error while processing button click " + ex);
		}
	}

	public void MessageWasShownToUser(global::Swrve.Messaging.SwrveMessageFormat messageFormat)
	{
		try
		{
			SetMessageMinDelayThrottle();
			messagesLeftToShow--;
			global::Swrve.Messaging.SwrveCampaign campaign = messageFormat.Message.Campaign;
			if (campaign != null)
			{
				campaign.MessageWasShownToUser(messageFormat);
				SaveCampaignData(campaign);
			}
			string text = "Swrve.Messages.Message-" + messageFormat.Message.Id + ".impression";
			SwrveLog.Log("Sending view event: " + text);
			global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
			dictionary.Add("format", messageFormat.Name);
			dictionary.Add("orientation", messageFormat.Orientation.ToString());
			dictionary.Add("size", messageFormat.Size.X + "x" + messageFormat.Size.Y);
			NamedEventInternal(text, dictionary);
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Error while processing message impression " + ex);
		}
	}

	public bool IsMessageDispaying()
	{
		return currentMessage != null;
	}

	public string GetAppStoreLink(int gameId)
	{
		string value = null;
		if (gameStoreLinks != null)
		{
			gameStoreLinks.TryGetValue(gameId.ToString(), out value);
		}
		return value;
	}

	public global::Swrve.Messaging.SwrveMessage GetMessageForEvent(string eventName)
	{
		global::Swrve.Messaging.SwrveMessage swrveMessage = null;
		global::Swrve.Messaging.SwrveCampaign swrveCampaign = null;
		global::System.DateTime now = global::Swrve.Helpers.SwrveHelper.GetNow();
		global::System.Collections.Generic.Dictionary<int, string> dictionary = null;
		global::System.Collections.Generic.Dictionary<int, int> dictionary2 = null;
		SwrveLog.Log("Trying to get message for: " + eventName);
		if (campaigns != null)
		{
			if (campaigns.Count == 0)
			{
				NoMessagesWereShown(eventName, "No campaigns available");
				return null;
			}
			if (!string.Equals(eventName, "Swrve.Messages.showAtSessionStart", global::System.StringComparison.OrdinalIgnoreCase) && IsTooSoonToShowMessageAfterLaunch(now))
			{
				NoMessagesWereShown(eventName, "{App throttle limit} Too soon after launch. Wait until " + showMessagesAfterLaunch.ToString(WaitTimeFormat));
				return null;
			}
			if (IsTooSoonToShowMessageAfterDelay(now))
			{
				NoMessagesWereShown(eventName, "{App throttle limit} Too soon after last message. Wait until " + showMessagesAfterDelay.ToString(WaitTimeFormat));
				return null;
			}
			if (HasShowTooManyMessagesAlready())
			{
				NoMessagesWereShown(eventName, "{App throttle limit} Too many messages shown");
				return null;
			}
			if (qaUser != null)
			{
				dictionary = new global::System.Collections.Generic.Dictionary<int, string>();
				dictionary2 = new global::System.Collections.Generic.Dictionary<int, int>();
			}
			global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveMessage> list = new global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveMessage>();
			int num = int.MaxValue;
			global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveMessage> list2 = new global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveMessage>();
			global::System.Collections.Generic.IEnumerator<global::Swrve.Messaging.SwrveCampaign> enumerator = campaigns.GetEnumerator();
			global::Swrve.Messaging.SwrveOrientation deviceOrientation = GetDeviceOrientation();
			while (enumerator.MoveNext() && swrveMessage == null)
			{
				global::Swrve.Messaging.SwrveCampaign current = enumerator.Current;
				global::Swrve.Messaging.SwrveMessage messageForEvent = current.GetMessageForEvent(eventName, dictionary);
				if (messageForEvent == null)
				{
					continue;
				}
				if (messageForEvent.SupportsOrientation(deviceOrientation))
				{
					list.Add(messageForEvent);
					if (messageForEvent.Priority <= num)
					{
						num = messageForEvent.Priority;
						if (messageForEvent.Priority < num)
						{
							list2.Clear();
						}
						list2.Add(messageForEvent);
					}
				}
				else if (qaUser != null)
				{
					dictionary2.Add(current.Id, messageForEvent.Id);
					dictionary.Add(current.Id, "Message didn't support the current device orientation: " + deviceOrientation);
				}
			}
			global::Swrve.Helpers.SwrveHelper.Shuffle(list2);
			if (list2.Count > 0)
			{
				swrveMessage = list2[0];
				swrveCampaign = swrveMessage.Campaign;
			}
			if (qaUser != null && swrveCampaign != null && swrveMessage != null)
			{
				global::System.Collections.Generic.IEnumerator<global::Swrve.Messaging.SwrveMessage> enumerator2 = list.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					global::Swrve.Messaging.SwrveMessage current2 = enumerator2.Current;
					if (current2 != swrveMessage)
					{
						int id = current2.Campaign.Id;
						if (!dictionary2.ContainsKey(id))
						{
							dictionary2.Add(id, current2.Id);
							dictionary.Add(id, "Campaign " + swrveCampaign.Id + " was selected for display ahead of this campaign");
						}
					}
				}
			}
		}
		if (qaUser != null)
		{
			qaUser.Trigger(eventName, swrveMessage, dictionary, dictionary2);
		}
		if (swrveMessage == null)
		{
			SwrveLog.LogWarning("Not showing message: no candidate messages for " + eventName);
		}
		else
		{
			global::System.Collections.Generic.Dictionary<string, string> dictionary3 = new global::System.Collections.Generic.Dictionary<string, string>();
			dictionary3.Add("id", swrveMessage.Id.ToString());
			NamedEventInternal("Swrve.Messages.message_returned", dictionary3, false);
		}
		return swrveMessage;
	}

	public global::Swrve.Messaging.SwrveMessage GetMessageForId(int messageId)
	{
		global::Swrve.Messaging.SwrveMessage swrveMessage = null;
		global::System.Collections.Generic.IEnumerator<global::Swrve.Messaging.SwrveCampaign> enumerator = campaigns.GetEnumerator();
		while (enumerator.MoveNext() && swrveMessage == null)
		{
			global::Swrve.Messaging.SwrveCampaign current = enumerator.Current;
			swrveMessage = current.GetMessageForId(messageId);
			if (swrveMessage != null)
			{
				return swrveMessage;
			}
		}
		SwrveLog.LogWarning("Message with id " + messageId + " not found");
		return null;
	}

	public global::System.Collections.IEnumerator ShowMessageForEvent(string eventName, global::Swrve.Messaging.ISwrveInstallButtonListener installButtonListener = null, global::Swrve.Messaging.ISwrveCustomButtonListener customButtonListener = null, global::Swrve.Messaging.ISwrveMessageListener messageListener = null)
	{
		if (TriggeredMessageListener != null)
		{
			global::Swrve.Messaging.SwrveMessage message = GetMessageForEvent(eventName);
			if (message != null)
			{
				TriggeredMessageListener.OnMessageTriggered(message);
			}
		}
		else if (currentMessage == null)
		{
			global::Swrve.Messaging.SwrveMessage message2 = GetMessageForEvent(eventName);
			yield return Container.StartCoroutine(LaunchMessage(message2, installButtonListener, customButtonListener, messageListener));
		}
		TaskFinished("ShowMessageForEvent");
	}

	public void DismissMessage()
	{
		if (TriggeredMessageListener != null)
		{
			TriggeredMessageListener.DismissCurrentMessage();
			return;
		}
		try
		{
			if (currentMessage != null)
			{
				SetMessageMinDelayThrottle();
				currentMessage.Dismiss();
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Error while dismissing a message " + ex);
		}
	}

	public virtual void RefreshUserResourcesAndCampaigns()
	{
		LoadResourcesAndCampaigns();
	}

	public void RegistrationIdReceived(string registrationId)
	{
		if (!string.IsNullOrEmpty(registrationId) && gcmDeviceToken != registrationId)
		{
			gcmDeviceToken = registrationId;
			storage.Save("Swrve_gcmDeviceToken", gcmDeviceToken);
			if (qaUser != null)
			{
				qaUser.UpdateDeviceInfo();
			}
			SendDeviceInfo();
		}
	}

	public void NotificationReceived(string notificationJson)
	{
		global::System.Collections.Generic.Dictionary<string, object> dictionary = (global::System.Collections.Generic.Dictionary<string, object>)global::SwrveMiniJSON.Json.Deserialize(notificationJson);
		if (androidPlugin != null && dictionary != null)
		{
			string pushId = GetPushId(dictionary);
			if (pushId != null)
			{
				androidPlugin.CallStatic("sdkAcknowledgeReceivedNotification", pushId);
			}
		}
		if (PushNotificationListener != null)
		{
			PushNotificationListener.OnNotificationReceived(dictionary);
		}
	}

	private string GetPushId(global::System.Collections.Generic.Dictionary<string, object> notification)
	{
		if (notification != null && notification.ContainsKey("_p"))
		{
			return notification["_p"].ToString();
		}
		SwrveLog.Log("Got unidentified notification");
		return null;
	}

	public void OpenedFromPushNotification(string notificationJson)
	{
		global::System.Collections.Generic.Dictionary<string, object> dictionary = (global::System.Collections.Generic.Dictionary<string, object>)global::SwrveMiniJSON.Json.Deserialize(notificationJson);
		string pushId = GetPushId(dictionary);
		SendPushNotificationEngagedEvent(pushId);
		if (pushId != null && androidPlugin != null)
		{
			androidPlugin.CallStatic("sdkAcknowledgeOpenedNotification", pushId);
		}
		if (PushNotificationListener != null)
		{
			PushNotificationListener.OnOpenedFromPushNotification(dictionary);
		}
	}

	private void QueueSessionStart()
	{
		global::System.Collections.Generic.Dictionary<string, object> eventParameters = new global::System.Collections.Generic.Dictionary<string, object>();
		AppendEventToBuffer("session_start", eventParameters);
	}

	private void NamedEventInternal(string name, global::System.Collections.Generic.Dictionary<string, string> payload = null, bool allowShowMessage = true)
	{
		if (payload == null)
		{
			payload = new global::System.Collections.Generic.Dictionary<string, string>();
		}
		global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
		dictionary.Add("name", name);
		dictionary.Add("payload", payload);
		AppendEventToBuffer("event", dictionary, allowShowMessage);
	}

	protected static string GetSwrvePath()
	{
		string text = global::UnityEngine.Application.persistentDataPath;
		if (string.IsNullOrEmpty(text))
		{
			text = global::UnityEngine.Application.temporaryCachePath;
			SwrveLog.Log("Swrve path (tried again): " + text);
		}
		return text;
	}

	protected static string GetSwrveTemporaryCachePath()
	{
		string text = global::UnityEngine.Application.temporaryCachePath;
		if (text == null || text.Length == 0)
		{
			text = global::UnityEngine.Application.persistentDataPath;
		}
		return text;
	}

	private void _Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards, string receipt, string receiptSignature, string transactionId, string appStore)
	{
		if (!_Iap_check_arguments(quantity, productId, productPrice, currency, appStore))
		{
			SwrveLog.LogError("ERROR: IAP event not sent because it received an illegal argument");
			return;
		}
		global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
		dictionary.Add("app_store", appStore);
		dictionary.Add("local_currency", currency);
		dictionary.Add("cost", productPrice);
		dictionary.Add("product_id", productId);
		dictionary.Add("quantity", quantity);
		dictionary.Add("rewards", rewards.getRewards());
		if (!string.IsNullOrEmpty(GetAppVersion()))
		{
			dictionary.Add("app_version", GetAppVersion());
		}
		if (appStore == "apple")
		{
			dictionary.Add("receipt", receipt);
			if (!string.IsNullOrEmpty(transactionId))
			{
				dictionary.Add("transaction_id", transactionId);
			}
		}
		else if (appStore == "google")
		{
			dictionary.Add("receipt", receipt);
			dictionary.Add("receipt_signature", receiptSignature);
		}
		else
		{
			dictionary.Add("receipt", receipt);
		}
		AppendEventToBuffer("iap", dictionary);
		if (config.AutoDownloadCampaignsAndResources)
		{
			CheckForCampaignsAndResourcesUpdates(false);
		}
	}

	protected virtual global::Swrve.Messaging.SwrveOrientation GetDeviceOrientation()
	{
		switch (global::UnityEngine.Screen.orientation)
		{
		case global::UnityEngine.ScreenOrientation.LandscapeLeft:
		case global::UnityEngine.ScreenOrientation.LandscapeRight:
			return global::Swrve.Messaging.SwrveOrientation.Landscape;
		case global::UnityEngine.ScreenOrientation.Portrait:
		case global::UnityEngine.ScreenOrientation.PortraitUpsideDown:
			return global::Swrve.Messaging.SwrveOrientation.Portrait;
		default:
			if (global::UnityEngine.Screen.height >= global::UnityEngine.Screen.width)
			{
				return global::Swrve.Messaging.SwrveOrientation.Portrait;
			}
			return global::Swrve.Messaging.SwrveOrientation.Landscape;
		}
	}

	private bool _Iap_check_arguments(int quantity, string productId, double productPrice, string currency, string appStore)
	{
		if (string.IsNullOrEmpty(productId))
		{
			SwrveLog.LogError("IAP event illegal argument: productId cannot be empty");
			return false;
		}
		if (string.IsNullOrEmpty(currency))
		{
			SwrveLog.LogError("IAP event illegal argument: currency cannot be empty");
			return false;
		}
		if (string.IsNullOrEmpty(appStore))
		{
			SwrveLog.LogError("IAP event illegal argument: appStore cannot be empty");
			return false;
		}
		if (quantity <= 0)
		{
			SwrveLog.LogError("IAP event illegal argument: quantity must be greater than zero");
			return false;
		}
		if (productPrice < 0.0)
		{
			SwrveLog.LogError("IAP event illegal argument: productPrice must be greater than or equal to zero");
			return false;
		}
		return true;
	}

	private global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> ProcessUserResources(global::System.Collections.Generic.IList<object> userResources)
	{
		global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> dictionary = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>();
		if (userResources != null)
		{
			global::System.Collections.Generic.IEnumerator<object> enumerator = userResources.GetEnumerator();
			while (enumerator.MoveNext())
			{
				global::System.Collections.Generic.Dictionary<string, object> dictionary2 = (global::System.Collections.Generic.Dictionary<string, object>)enumerator.Current;
				string key = (string)dictionary2["uid"];
				dictionary.Add(key, NormalizeJson(dictionary2));
			}
		}
		return dictionary;
	}

	private global::System.Collections.Generic.Dictionary<string, string> NormalizeJson(global::System.Collections.Generic.Dictionary<string, object> json)
	{
		global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
		foreach (string key in json.Keys)
		{
			object obj = json[key];
			if (obj != null)
			{
				dictionary.Add(key, obj.ToString());
			}
		}
		return dictionary;
	}

	private global::System.Collections.IEnumerator GetUserResourcesDiff_Coroutine(string getRequest, global::System.Action<global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>, global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>, string> onResult, global::System.Action<global::System.Exception> onError, string saveCategory)
	{
		global::System.Exception wwwException = null;
		string abTestCandidate = null;
		string saveCategory2 = default(string);
		yield return Container.StartCoroutine(restClient.Get(getRequest, delegate(global::Swrve.REST.RESTResponse response)
		{
			if (response.Error == global::Swrve.Helpers.WwwDeducedError.NoError)
			{
				abTestCandidate = response.Body;
				SwrveLog.Log("AB Test result: " + abTestCandidate);
				storage.SaveSecure(saveCategory2, abTestCandidate, userId);
				TaskFinished("GetUserResourcesDiff_Coroutine");
			}
			else
			{
				wwwException = new global::System.Exception(response.Error.ToString());
				SwrveLog.LogError("AB Test request failed: " + response.Error);
				TaskFinished("GetUserResourcesDiff_Coroutine");
			}
		}));
		abTestUserResourcesDiffConnecting = false;
		if (wwwException != null || string.IsNullOrEmpty(abTestCandidate))
		{
			try
			{
				string loadedData = storage.LoadSecure(saveCategory, userId);
				if (string.IsNullOrEmpty(loadedData))
				{
					onError(wwwException);
				}
				else if (global::Swrve.Helpers.ResponseBodyTester.TestUTF8(loadedData, out abTestCandidate))
				{
					global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> userResourcesDiffNew = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>();
					global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> userResourcesDiffOld = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>();
					ProcessUserResourcesDiff(abTestCandidate, userResourcesDiffNew, userResourcesDiffOld);
					onResult(userResourcesDiffNew, userResourcesDiffOld, abTestCandidate);
				}
				else
				{
					onError(wwwException);
				}
			}
			catch (global::System.Exception ex)
			{
				global::System.Exception e = ex;
				SwrveLog.LogWarning("Could not read user resources diff from cache (" + e.ToString() + ")");
				onError(wwwException);
			}
		}
		if (!string.IsNullOrEmpty(abTestCandidate))
		{
			global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> userResourcesDiffNew2 = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>();
			global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> userResourcesDiffOld2 = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>>();
			ProcessUserResourcesDiff(abTestCandidate, userResourcesDiffNew2, userResourcesDiffOld2);
			onResult(userResourcesDiffNew2, userResourcesDiffOld2, abTestCandidate);
		}
	}

	private void ProcessUserResourcesDiff(string abTestJson, global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> newResources, global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> oldResources)
	{
		global::System.Collections.Generic.IList<object> list = (global::System.Collections.Generic.List<object>)global::SwrveMiniJSON.Json.Deserialize(abTestJson);
		if (list == null)
		{
			return;
		}
		global::System.Collections.Generic.IEnumerator<object> enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			global::System.Collections.Generic.Dictionary<string, object> dictionary = (global::System.Collections.Generic.Dictionary<string, object>)enumerator.Current;
			string key = (string)dictionary["uid"];
			global::System.Collections.Generic.Dictionary<string, object> dictionary2 = (global::System.Collections.Generic.Dictionary<string, object>)dictionary["diff"];
			global::System.Collections.Generic.IEnumerator<string> enumerator2 = dictionary2.Keys.GetEnumerator();
			global::System.Collections.Generic.Dictionary<string, string> dictionary3 = new global::System.Collections.Generic.Dictionary<string, string>();
			global::System.Collections.Generic.Dictionary<string, string> dictionary4 = new global::System.Collections.Generic.Dictionary<string, string>();
			while (enumerator2.MoveNext())
			{
				global::System.Collections.Generic.Dictionary<string, string> dictionary5 = NormalizeJson((global::System.Collections.Generic.Dictionary<string, object>)dictionary2[enumerator2.Current]);
				dictionary3.Add(enumerator2.Current, dictionary5["new"]);
				dictionary4.Add(enumerator2.Current, dictionary5["old"]);
			}
			newResources.Add(key, dictionary3);
			oldResources.Add(key, dictionary4);
		}
	}

	private long GetInstallTimeEpoch()
	{
		string savedInstallTimeEpoch = GetSavedInstallTimeEpoch();
		if (!string.IsNullOrEmpty(savedInstallTimeEpoch))
		{
			long result = 0L;
			if (long.TryParse(savedInstallTimeEpoch, out result))
			{
				return result;
			}
		}
		long sessionTime = GetSessionTime();
		storage.Save("Swrve_JoinedDate", sessionTime.ToString(), userId);
		return sessionTime;
	}

	private string GetDeviceId()
	{
		string text = storage.Load("Swrve_DeviceId", userId);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		short num = (short)new global::System.Random().Next(32767);
		storage.Save("Swrve_DeviceId", num.ToString(), userId);
		return num.ToString();
	}

	private string getNextSeqNum()
	{
		string s = storage.Load("Swrve_SeqNum", userId);
		int result;
		s = ((!int.TryParse(s, out result)) ? "1" : (++result).ToString());
		storage.Save("Swrve_SeqNum", s, userId);
		return s;
	}

	protected string GetDeviceLanguage()
	{
		string text = null;
#if UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("java.util.Locale"))
			{
				global::UnityEngine.AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<global::UnityEngine.AndroidJavaObject>("getDefault", new object[0]);
				text = androidJavaObject.Call<string>("getLanguage", new object[0]);
				string text2 = androidJavaObject.Call<string>("getCountry", new object[0]);
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + "-" + text2;
				}
				string text3 = androidJavaObject.Call<string>("getVariant", new object[0]);
				if (!string.IsNullOrEmpty(text3))
				{
					text = text + "-" + text3;
				}
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get the device language, make sure you are running on an Android device: " + ex.ToString());
		}
#endif
		if (string.IsNullOrEmpty(text))
		{
			global::System.Globalization.CultureInfo currentUICulture = global::System.Globalization.CultureInfo.CurrentUICulture;
			string text4 = currentUICulture.TwoLetterISOLanguageName.ToLower();
			if (text4 != "iv")
			{
				text = text4;
			}
		}
		return text;
	}

	protected string GetSavedInstallTimeEpoch()
	{
		try
		{
			string text = storage.Load("Swrve_JoinedDate", userId);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Couldn't obtain saved install time: " + ex.Message);
		}
		return null;
	}

	protected void InvalidateETag()
	{
		lastETag = string.Empty;
		storage.Remove("cmpg_etag", userId);
	}

	private void InitUserResources()
	{
		userResourcesRaw = storage.LoadSecure("srcngt2", userId);
		if (!string.IsNullOrEmpty(userResourcesRaw))
		{
			global::System.Collections.Generic.IList<object> list = (global::System.Collections.Generic.IList<object>)global::SwrveMiniJSON.Json.Deserialize(userResourcesRaw);
			userResources = ProcessUserResources(list);
			NotifyUpdateUserResources();
		}
		else
		{
			InvalidateETag();
		}
	}

	private void NotifyUpdateUserResources()
	{
		if (userResources != null)
		{
			ResourceManager.SetResourcesFromJSON(userResources);
			if (ResourcesUpdatedCallback != null)
			{
				ResourcesUpdatedCallback();
			}
		}
	}

	private void LoadEventsFromDisk()
	{
		try
		{
			string value = storage.Load("Swrve_Events", userId);
			storage.Remove("Swrve_Events", userId);
			if (!string.IsNullOrEmpty(value))
			{
				if (eventBufferStringBuilder.Length != 0)
				{
					eventBufferStringBuilder.Insert(0, ",");
				}
				eventBufferStringBuilder.Insert(0, value);
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogWarning("Could not read events from cache (" + ex.ToString() + ")");
		}
	}

	private void LoadData()
	{
		LoadEventsFromDisk();
		installTimeEpoch = GetInstallTimeEpoch();
		installTimeFormatted = global::Swrve.Helpers.SwrveHelper.EpochToFormat(installTimeEpoch, InstallTimeFormat);
		lastETag = storage.Load("cmpg_etag", userId);
		string text = storage.Load("swrve_cr_flush_frequency", userId);
		if (!string.IsNullOrEmpty(text) && float.TryParse(text, out campaignsAndResourcesFlushFrequency))
		{
			campaignsAndResourcesFlushFrequency /= 1000f;
		}
		if (campaignsAndResourcesFlushFrequency == 0f)
		{
			campaignsAndResourcesFlushFrequency = 60f;
		}
		string text2 = storage.Load("swrve_cr_flush_delay", userId);
		if (!string.IsNullOrEmpty(text2) && float.TryParse(text2, out campaignsAndResourcesFlushRefreshDelay))
		{
			campaignsAndResourcesFlushRefreshDelay /= 1000f;
		}
		if (campaignsAndResourcesFlushRefreshDelay == 0f)
		{
			campaignsAndResourcesFlushRefreshDelay = 5f;
		}
	}

	protected string GetUniqueKey()
	{
		return apiKey + userId;
	}

	private string GetDeviceUniqueId()
	{
		string text = global::UnityEngine.PlayerPrefs.GetString("Swrve.deviceUniqueIdentifier", null);
		if (string.IsNullOrEmpty(text))
		{
			text = GetRandomUUID();
		}
		return text;
	}

	private string GetRandomUUID()
	{
		try
		{
			global::System.Type type = global::System.Type.GetType("System.Guid");
			if (type != null)
			{
				global::System.Reflection.MethodInfo method = type.GetMethod("NewGuid");
				if (method != null)
				{
					object obj = method.Invoke(null, null);
					if (obj != null)
					{
						string text = obj.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get random UUID: " + ex.ToString());
		}
		string text2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		string text3 = string.Empty;
		for (int i = 0; i < 128; i++)
		{
			text3 += text2[global::UnityEngine.Random.Range(0, text2.Length - 1)];
		}
		return text3;
	}

	protected virtual global::Swrve.REST.IRESTClient CreateRestClient()
	{
		return new global::Swrve.REST.RESTClient();
	}

	protected virtual global::Swrve.ISwrveStorage CreateStorage()
	{
		if (config.StoreDataInPlayerPrefs)
		{
			return new global::Swrve.Storage.SwrvePlayerPrefsStorage();
		}
		return new global::Swrve.Storage.SwrveFileStorage(swrvePath, GetUniqueKey());
	}

	private global::System.Collections.IEnumerator PostEvents_Coroutine(global::System.Collections.Generic.Dictionary<string, string> requestHeaders, byte[] eventsPostEncodedData)
	{
		yield return Container.StartCoroutine(restClient.Post(eventsUrl, eventsPostEncodedData, requestHeaders, delegate(global::Swrve.REST.RESTResponse response)
		{
			if (response.Error != global::Swrve.Helpers.WwwDeducedError.NetworkError)
			{
				ClearEventBuffer();
			}
			eventsConnecting = false;
			TaskFinished("PostEvents_Coroutine");
		}));
	}

	protected virtual void ClearEventBuffer()
	{
		eventsPostString = null;
	}

	private void AppendEventToBuffer(string eventType, global::System.Collections.Generic.Dictionary<string, object> eventParameters, bool allowShowMessage = true)
	{
		eventParameters.Add("type", eventType);
		eventParameters.Add("seqnum", getNextSeqNum());
		eventParameters.Add("time", GetSessionTime());
		string text = global::SwrveMiniJSON.Json.Serialize(eventParameters);
		string eventName = global::Swrve.Helpers.SwrveHelper.GetEventName(eventParameters);
		bool flag = eventBufferStringBuilder.Length + text.Length <= config.MaxBufferChars;
		if (flag || config.SendEventsIfBufferTooLarge)
		{
			if (!flag && config.SendEventsIfBufferTooLarge)
			{
				SendQueuedEvents();
			}
			if (eventBufferStringBuilder.Length > 0)
			{
				eventBufferStringBuilder.Append(',');
			}
			AppendEventToBuffer(text);
		}
		else
		{
			global::UnityEngine.Debug.LogError("Could not append the event to the buffer. Please consider enabling SendEventsIfBufferTooLarge");
		}
		if (allowShowMessage && config.TalkEnabled)
		{
			StartTask("ShowMessageForEvent", ShowMessageForEvent(eventName, GlobalInstallButtonListener, GlobalCustomButtonListener, GlobalMessageListener));
		}
	}

	protected virtual void AppendEventToBuffer(string eventJson)
	{
		eventBufferStringBuilder.Append(eventJson);
	}

	protected virtual global::UnityEngine.Coroutine StartTask(string tag, global::System.Collections.IEnumerator task)
	{
		return Container.StartCoroutine(task);
	}

	protected virtual void TaskFinished(string tag)
	{
	}

	private bool IsAlive()
	{
		return Container != null && !Destroyed;
	}

	protected virtual void GetDeviceScreenInfo()
	{
		deviceWidth = global::UnityEngine.Screen.width;
		deviceHeight = global::UnityEngine.Screen.height;
		if (deviceWidth > deviceHeight)
		{
			int num = deviceWidth;
			deviceWidth = deviceHeight;
			deviceHeight = num;
		}
	}

	private void QueueDeviceInfo()
	{
		global::System.Collections.Generic.Dictionary<string, string> deviceInfo = GetDeviceInfo();
		UserUpdate(deviceInfo);
	}

	private void SendDeviceInfo()
	{
		QueueDeviceInfo();
		SendQueuedEvents();
	}

	private global::System.Collections.IEnumerator WaitAndRefreshResourcesAndCampaigns_Coroutine(float delay)
	{
		yield return new global::UnityEngine.WaitForSeconds(delay);
		RefreshUserResourcesAndCampaigns();
	}

	private void CheckForCampaignsAndResourcesUpdates(bool invokedByTimer)
	{
		if (IsAlive())
		{
			if (SendQueuedEvents())
			{
				Container.StartCoroutine(WaitAndRefreshResourcesAndCampaigns_Coroutine(campaignsAndResourcesFlushRefreshDelay));
			}
			if (!invokedByTimer)
			{
				StopCheckForCampaignAndResources();
				StartCheckForCampaignsAndResources();
			}
		}
	}

	private void StartCheckForCampaignsAndResources()
	{
		if (campaignAndResourcesCoroutineInstance == null)
		{
			campaignAndResourcesCoroutineInstance = CheckForCampaignsAndResourcesUpdates_Coroutine();
			Container.StartCoroutine(campaignAndResourcesCoroutineInstance);
		}
		campaignAndResourcesCoroutineEnabled = true;
	}

	private void StopCheckForCampaignAndResources()
	{
		if (campaignAndResourcesCoroutineInstance != null)
		{
			Container.StopCoroutine(campaignAndResourcesCoroutineInstance);
			campaignAndResourcesCoroutineInstance = null;
		}
		campaignAndResourcesCoroutineEnabled = false;
	}

	private global::System.Collections.IEnumerator CheckForCampaignsAndResourcesUpdates_Coroutine()
	{
		yield return new global::UnityEngine.WaitForSeconds(campaignsAndResourcesFlushFrequency);
		CheckForCampaignsAndResourcesUpdates(true);
		if (campaignAndResourcesCoroutineEnabled)
		{
			campaignAndResourcesCoroutineInstance = null;
			StartCheckForCampaignsAndResources();
		}
	}

	protected virtual long GetSessionTime()
	{
		return global::Swrve.Helpers.SwrveHelper.GetMilliseconds();
	}

	private void GenerateNewSessionInterval()
	{
		lastSessionTick = GetSessionTime() + config.NewSessionInterval * 1000;
	}

	public void Update()
	{
		if (currentDisplayingMessage != null && !currentMessage.Closing)
		{
			if (inputManager.GetMouseButtonDown(0))
			{
				ProcessButtonDown();
			}
			else if (inputManager.GetMouseButtonUp(0))
			{
				ProcessButtonUp();
			}
		}
	}

	public void OnGUI()
	{
		if (currentDisplayingMessage == null)
		{
			return;
		}
		global::Swrve.Messaging.SwrveOrientation deviceOrientation = GetDeviceOrientation();
		if (deviceOrientation != currentOrientation)
		{
			if (currentDisplayingMessage.Orientation != deviceOrientation)
			{
				if (currentDisplayingMessage.Message.SupportsOrientation(deviceOrientation))
				{
					StartTask("SwitchMessageOrienation", SwitchMessageOrienation(deviceOrientation));
				}
				else
				{
					currentDisplayingMessage.Rotate = true;
				}
			}
			else
			{
				currentDisplayingMessage.Rotate = false;
			}
		}
		int depth = global::UnityEngine.GUI.depth;
		global::UnityEngine.Matrix4x4 matrix = global::UnityEngine.GUI.matrix;
		global::UnityEngine.GUI.depth = 0;
		global::Swrve.Messaging.SwrveMessageRenderer.DrawMessage(currentMessage, global::UnityEngine.Screen.width / 2 + currentMessage.Message.Position.X, global::UnityEngine.Screen.height / 2 + currentMessage.Message.Position.Y);
		global::UnityEngine.GUI.matrix = matrix;
		global::UnityEngine.GUI.depth = depth;
		if (currentDisplayingMessage.MessageListener != null)
		{
			currentDisplayingMessage.MessageListener.OnShowing(currentDisplayingMessage);
		}
		if (currentMessage.Dismissed)
		{
			currentMessage = null;
			currentDisplayingMessage = null;
		}
		currentOrientation = deviceOrientation;
	}

	private global::System.Collections.IEnumerator SwitchMessageOrienation(global::Swrve.Messaging.SwrveOrientation newOrientation)
	{
		global::Swrve.Messaging.SwrveMessageFormat newFormat = currentMessage.Message.GetFormat(newOrientation);
		if (newFormat != null && newFormat != currentMessage)
		{
			global::Swrve.Messaging.SwrveMessageFormat oldFormat = currentMessage;
			SwrveSDK.CoroutineReference<bool> wereAllLoaded = new SwrveSDK.CoroutineReference<bool>(false);
			yield return StartTask("PreloadFormatAssets", PreloadFormatAssets(newFormat, wereAllLoaded));
			if (wereAllLoaded.Value())
			{
				currentMessage = (currentDisplayingMessage = newFormat);
				oldFormat.UnloadAssets();
			}
			else
			{
				SwrveLog.LogError("Could not switch orientation. Not all assets could be preloaded");
			}
			TaskFinished("SwitchMessageOrienation");
		}
	}

	private void ProcessButtonDown()
	{
		global::UnityEngine.Vector3 mousePosition = inputManager.GetMousePosition();
		foreach (global::Swrve.Messaging.SwrveButton button in currentMessage.Buttons)
		{
			if (button.Rect.Contains(mousePosition))
			{
				button.Pressed = true;
			}
		}
	}

	private void ProcessButtonUp()
	{
		global::Swrve.Messaging.SwrveButton swrveButton = null;
		int num = currentMessage.Buttons.Count - 1;
		while (num >= 0 && swrveButton == null)
		{
			global::Swrve.Messaging.SwrveButton swrveButton2 = currentMessage.Buttons[num];
			global::UnityEngine.Vector3 mousePosition = inputManager.GetMousePosition();
			if (swrveButton2.Rect.Contains(mousePosition) && swrveButton2.Pressed)
			{
				swrveButton = swrveButton2;
			}
			else
			{
				swrveButton2.Pressed = false;
			}
			num--;
		}
		if (swrveButton == null)
		{
			return;
		}
		SwrveLog.Log("Clicked button " + swrveButton.ActionType);
		ButtonWasPressedByUser(swrveButton);
		if (swrveButton.ActionType == global::Swrve.Messaging.SwrveActionType.Install)
		{
			string text = swrveButton.GameId.ToString();
			if (gameStoreLinks.ContainsKey(text))
			{
				string text2 = gameStoreLinks[text];
				if (!string.IsNullOrEmpty(text2))
				{
					bool flag = true;
					if (currentMessage.InstallButtonListener != null)
					{
						flag = currentMessage.InstallButtonListener.OnAction(text2);
					}
					if (flag)
					{
						global::UnityEngine.Application.OpenURL(text2);
					}
				}
				else
				{
					SwrveLog.LogError("No app store url for game " + text);
				}
			}
			else
			{
				SwrveLog.LogError("Install button app store url empty!");
			}
		}
		else if (swrveButton.ActionType == global::Swrve.Messaging.SwrveActionType.Custom)
		{
			string action = swrveButton.Action;
			if (currentMessage.CustomButtonListener != null)
			{
				currentMessage.CustomButtonListener.OnAction(action);
			}
			else
			{
				SwrveLog.Log("No custom button listener, treating action as URL");
				if (!string.IsNullOrEmpty(action))
				{
					global::UnityEngine.Application.OpenURL(action);
				}
			}
		}
		swrveButton.Pressed = false;
		DismissMessage();
	}

	protected void SetMessageMinDelayThrottle()
	{
		showMessagesAfterDelay = global::Swrve.Helpers.SwrveHelper.GetNow() + global::System.TimeSpan.FromSeconds(minDelayBetweenMessage);
	}

	private void AutoShowMessages()
	{
		if (!autoShowMessagesEnabled || !campaignsAndResourcesInitialized || campaigns == null || campaigns.Count == 0)
		{
			return;
		}
		foreach (global::Swrve.Messaging.SwrveCampaign campaign in campaigns)
		{
			if (!campaign.HasMessageForEvent("Swrve.Messages.showAtSessionStart"))
			{
				continue;
			}
			if (TriggeredMessageListener != null)
			{
				global::Swrve.Messaging.SwrveMessage messageForEvent = GetMessageForEvent("Swrve.Messages.showAtSessionStart");
				if (messageForEvent != null)
				{
					autoShowMessagesEnabled = false;
					TriggeredMessageListener.OnMessageTriggered(messageForEvent);
				}
			}
			else if (currentMessage == null)
			{
				global::Swrve.Messaging.SwrveMessage messageForEvent2 = GetMessageForEvent("Swrve.Messages.showAtSessionStart");
				if (messageForEvent2 != null)
				{
					autoShowMessagesEnabled = false;
					Container.StartCoroutine(LaunchMessage(messageForEvent2, GlobalInstallButtonListener, GlobalCustomButtonListener, GlobalMessageListener));
				}
			}
			break;
		}
	}

	private global::System.Collections.IEnumerator LaunchMessage(global::Swrve.Messaging.SwrveMessage message, global::Swrve.Messaging.ISwrveInstallButtonListener installButtonListener, global::Swrve.Messaging.ISwrveCustomButtonListener customButtonListener, global::Swrve.Messaging.ISwrveMessageListener messageListener)
	{
		if (message == null)
		{
			yield break;
		}
		global::Swrve.Messaging.SwrveOrientation currentOrientation = GetDeviceOrientation();
		global::Swrve.Messaging.SwrveMessageFormat selectedFormat = message.GetFormat(currentOrientation);
		if (selectedFormat != null)
		{
			SwrveSDK.CoroutineReference<bool> wereAllLoaded = new SwrveSDK.CoroutineReference<bool>(false);
			yield return StartTask("PreloadFormatAssets", PreloadFormatAssets(selectedFormat, wereAllLoaded));
			if (wereAllLoaded.Value())
			{
				ShowMessageFormat(selectedFormat, installButtonListener, customButtonListener, messageListener);
			}
			else
			{
				SwrveLog.LogError("Could not preload all the assets for message " + message.Id);
			}
		}
		else
		{
			SwrveLog.LogError("Could not get a format for the current orientation: " + currentOrientation);
		}
	}

	private void NoMessagesWereShown(string eventName, string reason)
	{
		SwrveLog.Log("Not showing message for " + eventName + ": " + reason);
		if (qaUser != null)
		{
			qaUser.TriggerFailure(eventName, reason);
		}
	}

	private global::System.Collections.IEnumerator PreloadFormatAssets(global::Swrve.Messaging.SwrveMessageFormat format, SwrveSDK.CoroutineReference<bool> wereAllLoaded)
	{
		SwrveLog.Log("Preloading format");
		bool allLoaded = true;
		foreach (global::Swrve.Messaging.SwrveImage image in format.Images)
		{
			if (image.Texture == null && !string.IsNullOrEmpty(image.File))
			{
				SwrveLog.Log("Preloading image file " + image.File);
				SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D> result = new SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D>();
				yield return StartTask("LoadAsset", LoadAsset(image.File, result));
				if (result.Value() != null)
				{
					image.Texture = result.Value();
				}
				else
				{
					allLoaded = false;
				}
			}
		}
		foreach (global::Swrve.Messaging.SwrveButton button in format.Buttons)
		{
			if (button.Texture == null && !string.IsNullOrEmpty(button.Image))
			{
				SwrveLog.Log("Preloading button image " + button.Image);
				SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D> result2 = new SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D>();
				yield return StartTask("LoadAsset", LoadAsset(button.Image, result2));
				if (result2.Value() != null)
				{
					button.Texture = result2.Value();
				}
				else
				{
					allLoaded = false;
				}
			}
		}
		wereAllLoaded.Value(allLoaded);
		TaskFinished("PreloadFormatAssets");
	}

	private bool HasShowTooManyMessagesAlready()
	{
		return messagesLeftToShow <= 0;
	}

	private bool IsTooSoonToShowMessageAfterLaunch(global::System.DateTime now)
	{
		return now < showMessagesAfterLaunch;
	}

	private bool IsTooSoonToShowMessageAfterDelay(global::System.DateTime now)
	{
		return now < showMessagesAfterDelay;
	}

	private global::Swrve.Messaging.SwrveMessageFormat ShowMessageFormat(global::Swrve.Messaging.SwrveMessageFormat format, global::Swrve.Messaging.ISwrveInstallButtonListener installButtonListener, global::Swrve.Messaging.ISwrveCustomButtonListener customButtonListener, global::Swrve.Messaging.ISwrveMessageListener messageListener)
	{
		currentMessage = format;
		format.MessageListener = messageListener;
		format.CustomButtonListener = customButtonListener;
		format.InstallButtonListener = installButtonListener;
		currentDisplayingMessage = currentMessage;
		currentOrientation = GetDeviceOrientation();
		global::Swrve.Messaging.SwrveMessageRenderer.InitMessage(currentDisplayingMessage);
		MessageWasShownToUser(currentDisplayingMessage);
		return format;
	}

	private global::System.Collections.IEnumerator LoadAsset(string fileName, SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D> texture)
	{
		string filePath = swrveTemporaryPath + "/" + fileName;
		global::UnityEngine.WWW www = new global::UnityEngine.WWW("file://" + filePath);
		yield return www;
		if (www != null && www.error == null)
		{
			global::UnityEngine.Texture2D loadedTexture = www.texture;
			texture.Value(loadedTexture);
		}
		else
		{
			SwrveLog.LogError("Could not load asset with WWW " + filePath + ": " + www.error);
			if (global::Swrve.CrossPlatformFile.Exists(filePath))
			{
				byte[] byteArray = global::Swrve.CrossPlatformFile.ReadAllBytes(filePath);
				global::UnityEngine.Texture2D loadedTexture2 = new global::UnityEngine.Texture2D(4, 4);
				if (global::UnityEngine.ImageConversion.LoadImage(loadedTexture2, byteArray))
				{
					texture.Value(loadedTexture2);
				}
				else
				{
					SwrveLog.LogWarning("Could not load asset from I/O" + filePath);
				}
			}
			else
			{
				SwrveLog.LogError("The file " + filePath + " does not exist.");
			}
		}
		TaskFinished("LoadAsset");
	}

	protected virtual bool CheckAsset(string fileName)
	{
		if (global::Swrve.CrossPlatformFile.Exists(swrveTemporaryPath + "/" + fileName))
		{
			return true;
		}
		return false;
	}

	protected virtual global::System.Collections.IEnumerator DownloadAsset(string fileName, SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D> texture)
	{
		string url = cdn + fileName;
		SwrveLog.Log("Downloading asset: " + url);
		global::UnityEngine.WWW www = new global::UnityEngine.WWW(url);
		yield return www;
		global::Swrve.Helpers.WwwDeducedError err = global::Swrve.Helpers.UnityWwwHelper.DeduceWwwError(www);
		if (www != null && err == global::Swrve.Helpers.WwwDeducedError.NoError && www.isDone)
		{
			global::UnityEngine.Texture2D loadedTexture = www.texture;
			if (loadedTexture != null)
			{
				string filePath = swrveTemporaryPath + "/" + fileName;
				SwrveLog.Log("Saving to " + filePath);
				byte[] bytes = global::UnityEngine.ImageConversion.EncodeToPNG(loadedTexture);
				global::Swrve.CrossPlatformFile.SaveBytes(filePath, bytes);
				bytes = null;
				texture.Value(loadedTexture);
			}
		}
		TaskFinished("DownloadAsset");
	}

	protected global::System.Collections.IEnumerator DownloadAssets(global::System.Collections.Generic.List<string> assetsQueue)
	{
		assetsCurrentlyDownloading = true;
		foreach (string asset in assetsQueue)
		{
			if (!CheckAsset(asset))
			{
				SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D> resultTexture = new SwrveSDK.CoroutineReference<global::UnityEngine.Texture2D>();
				yield return StartTask("DownloadAsset", DownloadAsset(asset, resultTexture));
				global::UnityEngine.Texture2D texture = resultTexture.Value();
				if (texture != null)
				{
					assetsOnDisk.Add(asset);
					global::UnityEngine.Object.Destroy(texture);
				}
			}
			else
			{
				assetsOnDisk.Add(asset);
			}
		}
		assetsCurrentlyDownloading = false;
		AutoShowMessages();
		TaskFinished("DownloadAssets");
	}

	protected virtual void ProcessCampaigns(global::System.Collections.Generic.Dictionary<string, object> root)
	{
		global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveCampaign> list = new global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveCampaign>();
		global::System.Collections.Generic.List<string> list2 = new global::System.Collections.Generic.List<string>();
		try
		{
			if (root != null && root.ContainsKey("version"))
			{
				int num = global::Swrve.Helpers.MiniJsonHelper.GetInt(root, "version");
				if (num == CampaignAPIVersion)
				{
					cdn = (string)root["cdn_root"];
					global::System.Collections.Generic.Dictionary<string, object> dictionary = (global::System.Collections.Generic.Dictionary<string, object>)root["game_data"];
					foreach (string key in dictionary.Keys)
					{
						if (gameStoreLinks.ContainsKey(key))
						{
							gameStoreLinks.Remove(key);
						}
						global::System.Collections.Generic.Dictionary<string, object> dictionary2 = (global::System.Collections.Generic.Dictionary<string, object>)dictionary[key];
						if (dictionary2 != null && dictionary2.ContainsKey("app_store_url"))
						{
							object obj = dictionary2["app_store_url"];
							if (obj != null && obj is string)
							{
								gameStoreLinks.Add(key, (string)obj);
							}
						}
					}
					global::System.Collections.Generic.Dictionary<string, object> dictionary3 = (global::System.Collections.Generic.Dictionary<string, object>)root["rules"];
					int num2 = ((!dictionary3.ContainsKey("delay_first_message")) ? 150 : global::Swrve.Helpers.MiniJsonHelper.GetInt(dictionary3, "delay_first_message"));
					long num3 = ((!dictionary3.ContainsKey("max_messages_per_session")) ? 99999 : global::Swrve.Helpers.MiniJsonHelper.GetLong(dictionary3, "max_messages_per_session"));
					int num4 = ((!dictionary3.ContainsKey("min_delay_between_messages")) ? 55 : global::Swrve.Helpers.MiniJsonHelper.GetInt(dictionary3, "min_delay_between_messages"));
					global::System.DateTime now = global::Swrve.Helpers.SwrveHelper.GetNow();
					minDelayBetweenMessage = num4;
					messagesLeftToShow = num3;
					showMessagesAfterLaunch = initialisedTime + global::System.TimeSpan.FromSeconds(num2);
					SwrveLog.Log("Game rules OK: Delay Seconds: " + num2 + " Max shows: " + num3);
					SwrveLog.Log("Time is " + now.ToString() + " show messages after " + showMessagesAfterLaunch.ToString());
					global::System.Collections.Generic.Dictionary<int, string> dictionary4 = null;
					if (root.ContainsKey("qa"))
					{
						global::System.Collections.Generic.Dictionary<string, object> dictionary5 = (global::System.Collections.Generic.Dictionary<string, object>)root["qa"];
						SwrveLog.Log("You are a QA user!");
						dictionary4 = new global::System.Collections.Generic.Dictionary<int, string>();
						qaUser = new global::Swrve.Messaging.SwrveQAUser(this, dictionary5);
						if (dictionary5.ContainsKey("campaigns"))
						{
							global::System.Collections.Generic.IList<object> list3 = (global::System.Collections.Generic.List<object>)dictionary5["campaigns"];
							for (int i = 0; i < list3.Count; i++)
							{
								global::System.Collections.Generic.Dictionary<string, object> dictionary6 = (global::System.Collections.Generic.Dictionary<string, object>)list3[i];
								int num5 = global::Swrve.Helpers.MiniJsonHelper.GetInt(dictionary6, "id");
								string text = (string)dictionary6["reason"];
								SwrveLog.Log("Campaign " + num5 + " not downloaded because: " + text);
								dictionary4.Add(num5, text);
							}
						}
					}
					else
					{
						qaUser = null;
					}
					global::System.Collections.Generic.IList<object> list4 = (global::System.Collections.Generic.List<object>)root["campaigns"];
					int j = 0;
					for (int count = list4.Count; j < count; j++)
					{
						global::System.Collections.Generic.Dictionary<string, object> campaignData = (global::System.Collections.Generic.Dictionary<string, object>)list4[j];
						global::Swrve.Messaging.SwrveCampaign swrveCampaign = global::Swrve.Messaging.SwrveCampaign.LoadFromJSON(this, campaignData, initialisedTime, swrveTemporaryPath);
						if (swrveCampaign.Messages.Count <= 0)
						{
							continue;
						}
						list2.AddRange(swrveCampaign.ListOfAssets());
						if (campaignSettings != null && (qaUser == null || !qaUser.ResetDevice))
						{
							if (campaignSettings.ContainsKey("Next" + swrveCampaign.Id))
							{
								int next = global::Swrve.Helpers.MiniJsonHelper.GetInt(campaignSettings, "Next" + swrveCampaign.Id);
								swrveCampaign.Next = next;
							}
							if (campaignSettings.ContainsKey("Impressions" + swrveCampaign.Id))
							{
								int impressions = global::Swrve.Helpers.MiniJsonHelper.GetInt(campaignSettings, "Impressions" + swrveCampaign.Id);
								swrveCampaign.Impressions = impressions;
							}
						}
						list.Add(swrveCampaign);
						if (qaUser != null)
						{
							dictionary4.Add(swrveCampaign.Id, null);
						}
					}
					if (qaUser != null)
					{
						qaUser.TalkSession(dictionary4);
					}
				}
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Could not process campaigns: " + ex.ToString());
		}
		StartTask("DownloadAssets", DownloadAssets(list2));
		campaigns = new global::System.Collections.Generic.List<global::Swrve.Messaging.SwrveCampaign>(list);
	}

	private void LoadResourcesAndCampaigns()
	{
		try
		{
			if (campaignsConnecting)
			{
				return;
			}
			if (!config.AutoDownloadCampaignsAndResources)
			{
				if (campaignsAndResourcesLastRefreshed != 0L)
				{
					long sessionTime = GetSessionTime();
					if (sessionTime < campaignsAndResourcesLastRefreshed)
					{
						SwrveLog.Log("Request to retrieve campaign and user resource data was rate-limited.");
						return;
					}
				}
				campaignsAndResourcesLastRefreshed = GetSessionTime() + (long)(campaignsAndResourcesFlushFrequency * 1000f);
			}
			campaignsConnecting = true;
			float num = ((global::UnityEngine.Screen.dpi != 0f) ? global::UnityEngine.Screen.dpi : 160f);
			string deviceModel = GetDeviceModel();
			string operatingSystem = global::UnityEngine.SystemInfo.operatingSystem;
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(resourcesAndCampaignsUrl).AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", escapedUserId, ApiKey, global::UnityEngine.WWW.EscapeURL(GetAppVersion()), installTimeEpoch);
			if (config.TalkEnabled)
			{
				stringBuilder.AppendFormat("&version={0}&orientation={1}&language={2}&app_store={3}&device_width={4}&device_height={5}&device_dpi={6}&os_version={7}&device_name={8}", CampaignEndpointVersion, config.Orientation.ToString().ToLower(), Language, config.AppStore, deviceWidth, deviceHeight, num, global::System.Uri.EscapeDataString(operatingSystem), global::System.Uri.EscapeDataString(deviceModel));
			}
			if (!string.IsNullOrEmpty(lastETag))
			{
				stringBuilder.AppendFormat("&etag={0}", lastETag);
			}
			StartTask("GetCampaignsAndResources_Coroutine", GetCampaignsAndResources_Coroutine(stringBuilder.ToString()));
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Error while trying to get user resources and campaign data: " + ex);
		}
	}

	private string GetDeviceModel()
	{
		string text = global::UnityEngine.SystemInfo.deviceModel;
		if (string.IsNullOrEmpty(text))
		{
			text = "ModelUnknown";
		}
		return text;
	}

    private global::System.Collections.IEnumerator GetCampaignsAndResources_Coroutine(string getRequest)
    {
        SwrveLog.Log("Campaigns and resources request: " + getRequest);

        // Utilisation du délégué pour traiter la réponse du serveur
        yield return Container.StartCoroutine(restClient.Get(getRequest, delegate (global::Swrve.REST.RESTResponse response)
        {
            if (response.Error == global::Swrve.Helpers.WwwDeducedError.NoError)
            {
                // --- Traitement des Headers (ETag) ---
                string value = null;
                if (response.Headers != null)
                {
                    response.Headers.TryGetValue("ETAG", out value);
                    if (!string.IsNullOrEmpty(value))
                    {
                        lastETag = value;
                        storage.Save("cmpg_etag", value, userId);
                    }
                }

                // --- Traitement du Corps (JSON) ---
                if (!string.IsNullOrEmpty(response.Body))
                {
                    // CORRECTION : On désérialise en 'object' d'abord pour éviter le crash
                    object deserializedObject = global::SwrveMiniJSON.Json.Deserialize(response.Body);

                    // Utilisation de l'opérateur 'as' (C# 4.0 compatible)
                    // Si deserializedObject n'est pas un Dictionary, 'dictionary' sera simplement null au lieu de lever une exception.
                    global::System.Collections.Generic.Dictionary<string, object> dictionary = deserializedObject as global::System.Collections.Generic.Dictionary<string, object>;

                    if (dictionary != null)
                    {
                        // 1. Flush Frequency
                        if (dictionary.ContainsKey("flush_frequency"))
                        {
                            string text = global::Swrve.Helpers.MiniJsonHelper.GetString(dictionary, "flush_frequency");
                            if (!string.IsNullOrEmpty(text) && float.TryParse(text, out campaignsAndResourcesFlushFrequency))
                            {
                                campaignsAndResourcesFlushFrequency /= 1000f;
                                storage.Save("swrve_cr_flush_frequency", text, userId);
                            }
                        }

                        // 2. Flush Refresh Delay
                        if (dictionary.ContainsKey("flush_refresh_delay"))
                        {
                            string text2 = global::Swrve.Helpers.MiniJsonHelper.GetString(dictionary, "flush_refresh_delay");
                            if (!string.IsNullOrEmpty(text2) && float.TryParse(text2, out campaignsAndResourcesFlushRefreshDelay))
                            {
                                campaignsAndResourcesFlushRefreshDelay /= 1000f;
                                storage.Save("swrve_cr_flush_delay", text2, userId);
                            }
                        }

                        // 3. User Resources
                        if (dictionary.ContainsKey("user_resources"))
                        {
                            // Safe cast pour la liste de ressources
                            global::System.Collections.Generic.IList<object> obj = dictionary["user_resources"] as global::System.Collections.Generic.IList<object>;
                            if (obj != null)
                            {
                                string data = global::SwrveMiniJSON.Json.Serialize(obj);
                                storage.SaveSecure("srcngt2", data, userId);
                                userResources = ProcessUserResources(obj);
                                userResourcesRaw = data;
                                if (campaignsAndResourcesInitialized)
                                {
                                    NotifyUpdateUserResources();
                                }
                            }
                        }

                        // 4. Campaigns (Talk)
                        if (config.TalkEnabled && dictionary.ContainsKey("campaigns"))
                        {
                            global::System.Collections.Generic.Dictionary<string, object> dictionary2 = dictionary["campaigns"] as global::System.Collections.Generic.Dictionary<string, object>;

                            if (dictionary2 != null)
                            {
                                string cacheContent = global::SwrveMiniJSON.Json.Serialize(dictionary2);
                                SaveCampaignsCache(cacheContent);
                                AutoShowMessages();
                                ProcessCampaigns(dictionary2);

                                global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
                                for (int i = 0; i < campaigns.Count; i++)
                                {
                                    global::Swrve.Messaging.SwrveCampaign swrveCampaign = campaigns[i];
                                    if (i != 0) stringBuilder.Append(',');
                                    stringBuilder.Append(swrveCampaign.Id);
                                }

                                global::System.Collections.Generic.Dictionary<string, string> payload = new global::System.Collections.Generic.Dictionary<string, string>();
                                payload.Add("ids", stringBuilder.ToString());
                                payload.Add("count", (campaigns != null) ? campaigns.Count.ToString() : "0");

                                NamedEventInternal("Swrve.Messages.campaigns_downloaded", payload);
                            }
                        }
                    }
                    else
                    {
                        // Si on arrive ici, c'est que le JSON reçu n'était pas un objet { } 
                        // (C'était probablement une liste [ ] ou un message d'erreur texte)
                        SwrveLog.LogError("Swrve Error: Expected JSON Dictionary but received " + deserializedObject.GetType().Name + ". Content: " + response.Body);
                    }
                }
            }
            else
            {
                SwrveLog.LogError("Resources and campaigns request error: " + response.Error.ToString() + ":" + response.Body);
            }

            // --- Finalisation ---
            if (!campaignsAndResourcesInitialized)
            {
                campaignsAndResourcesInitialized = true;
                AutoShowMessages();
                NotifyUpdateUserResources();
            }
            campaignsConnecting = false;
            TaskFinished("GetCampaignsAndResources_Coroutine");
        }));
    }

    private void SaveCampaignsCache(string cacheContent)
	{
		try
		{
			if (cacheContent == null)
			{
				cacheContent = string.Empty;
			}
			storage.SaveSecure(CampaignsSave, cacheContent, userId);
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Error while saving campaigns to the cache " + ex);
		}
	}

	private void SaveCampaignData(global::Swrve.Messaging.SwrveCampaign campaign)
	{
		try
		{
			campaignSettings["Next" + campaign.Id] = campaign.Next;
			campaignSettings["Impressions" + campaign.Id] = campaign.Impressions;
			string data = global::SwrveMiniJSON.Json.Serialize(campaignSettings);
			storage.Save(CampaignsSettingsSave, data, userId);
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Error while trying to save campaign settings " + ex);
		}
	}

	private void LoadTalkData()
	{
		try
		{
			string text = storage.Load(CampaignsSettingsSave, userId);
			string decodedString;
			if (text != null && text.Length != 0 && global::Swrve.Helpers.ResponseBodyTester.TestUTF8(text, out decodedString))
			{
				campaignSettings = (global::System.Collections.Generic.Dictionary<string, object>)global::SwrveMiniJSON.Json.Deserialize(decodedString);
			}
		}
		catch (global::System.Exception)
		{
		}
		try
		{
			string text2 = storage.LoadSecure(CampaignsSave, userId);
			if (!string.IsNullOrEmpty(text2))
			{
				string decodedString2 = null;
				if (global::Swrve.Helpers.ResponseBodyTester.TestUTF8(text2, out decodedString2))
				{
					global::System.Collections.Generic.Dictionary<string, object> root = (global::System.Collections.Generic.Dictionary<string, object>)global::SwrveMiniJSON.Json.Deserialize(decodedString2);
					ProcessCampaigns(root);
				}
				else
				{
					SwrveLog.Log("Failed to parse campaigns cache");
					InvalidateETag();
				}
			}
			else
			{
				InvalidateETag();
			}
		}
		catch (global::System.Exception ex2)
		{
			SwrveLog.LogWarning("Could not read campaigns from cache, using default (" + ex2.ToString() + ")");
			InvalidateETag();
		}
	}

	private void SendPushNotificationEngagedEvent(string pushId)
	{
		if (pushId != lastPushEngagedId)
		{
			lastPushEngagedId = pushId;
			string name = "Swrve.Messages.Push-" + pushId + ".engaged";
			NamedEvent(name);
			SwrveLog.Log("Got Swrve notification with ID " + pushId);
		}
	}

	protected int ConvertInt64ToInt32Hack(long val)
	{
		return (int)(val & 0xFFFFFFFFu);
	}

	protected virtual global::Swrve.Device.ICarrierInfo GetCarrierInfoProvider()
	{
		return deviceCarrierInfo;
	}

	private void GooglePlayRegisterForPushNotification(global::UnityEngine.MonoBehaviour container, string senderId)
	{
		try
		{
			bool flag = false;
			gcmDeviceToken = storage.Load("Swrve_gcmDeviceToken");
			if (!androidPluginInitialized)
			{
				androidPluginInitialized = true;
				using (new global::UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					string name = "com.swrve.unity.gcm.SwrveGcmDeviceRegistration".Replace(".", "/");
					if (global::UnityEngine.AndroidJNI.FindClass(name).ToInt32() != 0)
					{
						androidPlugin = new global::UnityEngine.AndroidJavaClass("com.swrve.unity.gcm.SwrveGcmDeviceRegistration");
						if (androidPlugin != null)
						{
							int num = androidPlugin.CallStatic<int>("getVersion", new object[0]);
							if (num != 3)
							{
								androidPlugin = null;
								throw new global::System.Exception("The version of the Swrve Android Push plugin is different. This Swrve SDK needs version " + 3);
							}
							androidPluginInitializedSuccessfully = true;
						}
					}
				}
			}
			if (androidPluginInitializedSuccessfully)
			{
				flag = androidPlugin.CallStatic<bool>("registerDevice", new object[3] { container.name, senderId, config.GCMPushNotificationTitle });
			}
			if (!flag)
			{
				SwrveLog.LogError("Could not communicate with the Swrve Android Push plugin. Have you copied all the jars to the directory?");
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogError("Could not retrieve the device Registration Id: " + ex.ToString());
		}
	}

	private string AndroidGetTimezone()
	{
		try
		{
			global::UnityEngine.AndroidJavaObject androidJavaObject = new global::UnityEngine.AndroidJavaObject("java.util.GregorianCalendar");
			return androidJavaObject.Call<global::UnityEngine.AndroidJavaObject>("getTimeZone", new object[0]).Call<string>("getID", new object[0]);
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get the device timezone, make sure you are running on an Android device: " + ex.ToString());
		}
		return null;
	}

	private string AndroidGetAppVersion()
	{
		try
		{
			using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				global::UnityEngine.AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<global::UnityEngine.AndroidJavaObject>("currentActivity");
				string text = androidJavaObject.Call<string>("getPackageName", new object[0]);
				return androidJavaObject.Call<global::UnityEngine.AndroidJavaObject>("getPackageManager", new object[0]).Call<global::UnityEngine.AndroidJavaObject>("getPackageInfo", new object[2] { text, 0 }).Get<string>("versionName");
			}
		}
		catch (global::System.Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get the device app version, make sure you are running on an Android device: " + ex.ToString());
		}
		return null;
	}

	public string GetAppVersion()
	{
		if (string.IsNullOrEmpty(config.AppVersion))
		{
			config.AppVersion = AndroidGetAppVersion();
		}
		return config.AppVersion;
	}

	private void SetInputManager(global::Swrve.Input.IInputManager inputManager)
	{
		this.inputManager = inputManager;
	}

	protected void StartCampaignsAndResourcesTimer()
	{
		if (config.AutoDownloadCampaignsAndResources)
		{
			RefreshUserResourcesAndCampaigns();
			StartCheckForCampaignsAndResources();
			Container.StartCoroutine(WaitAndRefreshResourcesAndCampaigns_Coroutine(campaignsAndResourcesFlushRefreshDelay));
		}
	}

	protected void DisableAutoShowAfterDelay()
	{
		Container.StartCoroutine(DisableAutoShowAfterDelay_Coroutine());
	}

	private global::System.Collections.IEnumerator DisableAutoShowAfterDelay_Coroutine()
	{
		yield return new global::UnityEngine.WaitForSeconds(config.AutoShowMessagesMaxDelay);
		autoShowMessagesEnabled = false;
	}
}
