public class HockeyAppAndroid : global::UnityEngine.MonoBehaviour
{
	protected const string HOCKEYAPP_BASEURL = "https://rink.hockeyapp.net/api/2/apps/";

	protected const string HOCKEYAPP_CRASHESPATH = "/crashes/upload";

	protected const int MAX_CHARS = 199800;

	protected const string LOG_FILE_DIR = "/logs/";

	protected const string DATE_LABEL = "Date:";

	protected const string TELEMETRY_DATE_FORMAT = "yyyymmdd_HHmmss";

	protected const string LOG_DATE_FORMAT = "ddd MMM dd HH:mm:ss {}zzzz yyyy";

	protected const string LOCAL_TIMEZONE = "GMT";

	public string appID = string.Empty;

	public string packageID = string.Empty;

	public bool exceptionLogging;

	public bool autoUpload;

	public bool updateManager;

	public string userId = string.Empty;

	public global::System.Action crashReportCallback;

	internal global::Kampai.Common.ITelemetryService telemetryService;

	private void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (exceptionLogging)
		{
#if !UNITY_WEBPLAYER
			global::System.Collections.Generic.List<string> logFiles = GetLogFiles();
			if (logFiles.Count > 0)
			{
				StartCoroutine(SendLogs(GetLogFiles()));
			}
#endif
		}
		StartCrashManager(appID, updateManager, autoUpload, userId);
	}

	private void OnEnable()
	{
		if (exceptionLogging)
		{
			global::System.AppDomain.CurrentDomain.UnhandledException += OnHandleUnresolvedException;
			global::UnityEngine.Application.logMessageReceived += OnHandleLogCallback;
		}
	}

	private void OnDisable()
	{
		if (exceptionLogging)
		{
			global::System.AppDomain.CurrentDomain.UnhandledException -= OnHandleUnresolvedException;
			global::UnityEngine.Application.logMessageReceived -= OnHandleLogCallback;
		}
	}

	protected void StartCrashManager(string appID, bool updateManagerEnabled, bool autoSendEnabled, string userID)
	{
#if !UNITY_WEBPLAYER && UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer");
			global::UnityEngine.AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<global::UnityEngine.AndroidJavaObject>("currentActivity");
			global::UnityEngine.AndroidJavaClass androidJavaClass2 = new global::UnityEngine.AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin");
			androidJavaClass2.CallStatic("startHockeyAppManager", appID, androidJavaObject, updateManagerEnabled, autoSendEnabled, userID);
		}
		catch (global::System.Exception ex)
		{
			global::UnityEngine.Debug.LogWarning("Failed to initialize HockeyApp crash manager: " + ex.Message);
		}
#endif
	}

	protected string GetVersion()
	{
#if !UNITY_WEBPLAYER && UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin");
			return androidJavaClass.CallStatic<string>("getAppVersion", new object[0]);
		}
		catch (global::System.Exception ex)
		{
			global::UnityEngine.Debug.LogWarning("Failed to get HockeyApp version: " + ex.Message);
			return "1.0.0";
		}
#else
		return "1.0.0";
#endif
	}

	protected virtual global::System.Collections.Generic.List<string> GetLogHeaders()
	{
		global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>();
		list.Add("Package: " + packageID);
		string version = GetVersion();
		list.Add("Version: " + version);
		string[] array = global::UnityEngine.SystemInfo.operatingSystem.Split('/');
		string item = "Android: " + array[0].Replace("Android OS ", string.Empty);
		list.Add(item);
		list.Add("Model: " + global::UnityEngine.SystemInfo.deviceModel);
		list.Add("GPU vendor: " + global::UnityEngine.SystemInfo.graphicsDeviceVendor);
		list.Add("GPU name: " + global::UnityEngine.SystemInfo.graphicsDeviceName);
		list.Add("VRAM: " + global::UnityEngine.SystemInfo.graphicsMemorySize);
		list.Add("RAM: " + global::UnityEngine.SystemInfo.systemMemorySize);
		list.Add("Date: " + global::System.DateTime.UtcNow.ToString("ddd MMM dd HH:mm:ss {}zzzz yyyy").Replace("{}", "GMT"));
		return list;
	}

	protected virtual global::UnityEngine.WWWForm CreateForm(string log)
	{
		global::UnityEngine.WWWForm wWWForm = new global::UnityEngine.WWWForm();
		byte[] array = null;
#if !UNITY_WEBPLAYER
		using (global::System.IO.FileStream fileStream = global::System.IO.File.OpenRead(log))
		{
			if (fileStream.Length > 199800)
			{
				string text = null;
				using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(fileStream))
				{
					streamReader.BaseStream.Seek(fileStream.Length - 199800, global::System.IO.SeekOrigin.Begin);
					text = streamReader.ReadToEnd();
				}
				global::System.Collections.Generic.List<string> logHeaders = GetLogHeaders();
				string text2 = string.Empty;
				foreach (string item in logHeaders)
				{
					text2 = text2 + item + "\n";
				}
				text = text2 + "\n[...]" + text;
				try
				{
					array = global::System.Text.Encoding.Default.GetBytes(text);
				}
				catch (global::System.ArgumentException ex)
				{
					if (global::UnityEngine.Debug.isDebugBuild)
					{
						global::UnityEngine.Debug.Log("Failed to read bytes of log file: " + ex);
					}
				}
			}
			else
			{
				try
				{
					array = global::System.IO.File.ReadAllBytes(log);
				}
				catch (global::System.SystemException ex2)
				{
					if (global::UnityEngine.Debug.isDebugBuild)
					{
						global::UnityEngine.Debug.Log("Failed to read bytes of log file: " + ex2);
					}
				}
			}
		}
#endif
		if (array != null)
		{
			wWWForm.AddBinaryData("log", array, log, "text/plain");
			string logFilesContent = global::Kampai.Util.Native.GetLogFilesContent(32768);
			if (!string.IsNullOrEmpty(logFilesContent))
			{
				byte[] bytes = global::System.Text.Encoding.UTF8.GetBytes(logFilesContent);
				if (bytes != null)
				{
					wWWForm.AddBinaryData("description", bytes, "description", "text/plain");
				}
			}
		}
		wWWForm.AddField("userID", userId);
		return wWWForm;
	}

	protected virtual global::System.Collections.Generic.List<string> GetLogFiles()
	{
		global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>();
#if !UNITY_WEBPLAYER
		string path = global::UnityEngine.Application.persistentDataPath + "/logs/";
		try
		{
			if (!global::System.IO.Directory.Exists(path))
			{
				global::System.IO.Directory.CreateDirectory(path);
			}
			global::System.IO.DirectoryInfo directoryInfo = new global::System.IO.DirectoryInfo(path);
			global::System.IO.FileInfo[] files = directoryInfo.GetFiles();
			if (files.Length > 0)
			{
				global::System.IO.FileInfo[] array = files;
				foreach (global::System.IO.FileInfo fileInfo in array)
				{
					if (fileInfo.Extension == ".log")
					{
						list.Add(fileInfo.FullName);
					}
					else
					{
						global::System.IO.File.Delete(fileInfo.FullName);
					}
				}
			}
		}
		catch (global::System.Exception ex)
		{
			if (global::UnityEngine.Debug.isDebugBuild)
			{
				global::UnityEngine.Debug.Log("Failed to write exception log to file: " + ex);
			}
		}
#endif
		return list;
	}

	protected virtual global::System.Collections.IEnumerator SendLogs(global::System.Collections.Generic.List<string> logs)
	{
		foreach (string log in logs)
		{
			string url = "https://rink.hockeyapp.net/api/2/apps/" + appID + "/crashes/upload";
			global::UnityEngine.WWWForm postForm = CreateForm(log);
			if (postForm.data != null)
			{
				SendCrashTelemetry(global::System.Text.Encoding.UTF8.GetString(postForm.data));
			}
#if !UNITY_WEBPLAYER
			try
			{
				global::System.IO.File.Delete(log);
			}
			catch (global::System.Exception ex)
			{
				if (global::UnityEngine.Debug.isDebugBuild)
				{
					global::UnityEngine.Debug.Log("Failed to delete exception log: " + ex);
				}
			}
#endif
			string lContent = postForm.headers["Content-Type"].ToString();
			lContent = lContent.Replace("\"", string.Empty);
			
			using (global::UnityEngine.Networking.UnityWebRequest webRequest = new global::UnityEngine.Networking.UnityWebRequest(url, "POST"))
			{
				webRequest.uploadHandler = new global::UnityEngine.Networking.UploadHandlerRaw(postForm.data);
				webRequest.uploadHandler.contentType = lContent;
				webRequest.downloadHandler = new global::UnityEngine.Networking.DownloadHandlerBuffer();
				webRequest.SetRequestHeader("Content-Type", lContent);
				yield return webRequest.SendWebRequest();
			}
			
			if (crashReportCallback != null)
			{
				crashReportCallback();
			}
		}
	}

	protected void SendCrashTelemetry(string crashData)
	{
		if (telemetryService == null || crashData == null)
		{
			return;
		}
		string[] array = crashData.Split(new string[2] { "\r\n", "\n" }, global::System.StringSplitOptions.None);
		string text = null;
		string text2 = null;
		string text3 = null;
		string text4 = string.Empty;
		string[] array2 = array;
		foreach (string text5 in array2)
		{
			if (global::System.Linq.Enumerable.All(text5, char.IsWhiteSpace))
			{
				continue;
			}
			if (text == null)
			{
				int num = text5.IndexOf("Date:");
				if (num != -1)
				{
					text = ConvertDateTimeForTelemetry(text5.Substring(num + "Date:".Length).TrimStart().TrimEnd());
				}
			}
			else if (text2 == null)
			{
				int length = text5.IndexOf(":");
				text2 = text5.Substring(0, length);
				text4 = text5;
			}
			else
			{
				if (text3 == null)
				{
					text3 = text5;
				}
				text4 = text4 + "\n" + text5;
			}
		}
		if (text2 != null)
		{
			telemetryService.Send_Telemetry_EVT_GAME_ERROR_CRASH(text2, text3, text, text4);
		}
	}

	protected string ConvertDateTimeForTelemetry(string crashTime)
	{
		if (crashTime != null)
		{
			crashTime = crashTime.Replace("GMT", string.Empty);
			string text = "ddd MMM dd HH:mm:ss {}zzzz yyyy".Replace("{}", string.Empty);
			string[] formats = new string[1] { text };
			global::System.DateTime result;
			if (global::System.DateTime.TryParseExact(crashTime, formats, new global::System.Globalization.CultureInfo("en-US"), global::System.Globalization.DateTimeStyles.None, out result))
			{
				crashTime = result.ToString("yyyymmdd_HHmmss");
			}
		}
		return crashTime;
	}

	protected virtual void WriteLogToDisk(string logString, string stackTrace)
	{
		string text = global::System.DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss_fff");
		string text2 = logString.Replace("\n", " ");
		string[] array = stackTrace.Split('\n');
		text2 = "\n" + text2 + "\n";
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			if (text3.Length > 0)
			{
				text2 = text2 + "  at " + text3 + "\n";
			}
		}
#if !UNITY_WEBPLAYER
		global::System.Collections.Generic.List<string> logHeaders = GetLogHeaders();
		using (global::System.IO.StreamWriter streamWriter = new global::System.IO.StreamWriter(global::UnityEngine.Application.persistentDataPath + "/logs/LogFile_" + text + ".log", true))
		{
			foreach (string item in logHeaders)
			{
				streamWriter.WriteLine(item);
			}
			streamWriter.WriteLine(text2);
			global::Kampai.Util.Native.AndroidFileLog(text2);
		}
#endif
	}

	protected virtual void HandleException(string logString, string stackTrace)
	{
		WriteLogToDisk(logString, stackTrace);
	}

	public void OnHandleLogCallback(string logString, string stackTrace, global::UnityEngine.LogType type)
	{
		if (exceptionLogging && (type == global::UnityEngine.LogType.Assert || type == global::UnityEngine.LogType.Exception) && (type != global::UnityEngine.LogType.Exception || !logString.StartsWith("FatalException")))
		{
			exceptionLogging = false;
			global::System.AppDomain.CurrentDomain.UnhandledException -= OnHandleUnresolvedException;
			global::UnityEngine.Application.logMessageReceived -= OnHandleLogCallback;
			HandleException(logString, stackTrace);
			global::Kampai.Util.Native.CloseFileLog();
			global::UnityEngine.Application.Quit();
		}
	}

	public void OnHandleUnresolvedException(object sender, global::System.UnhandledExceptionEventArgs args)
	{
		if (exceptionLogging && args != null && args.ExceptionObject != null && args.ExceptionObject.GetType() == typeof(global::System.Exception))
		{
			exceptionLogging = false;
			global::System.AppDomain.CurrentDomain.UnhandledException -= OnHandleUnresolvedException;
			global::UnityEngine.Application.logMessageReceived -= OnHandleLogCallback;
			global::System.Exception ex = (global::System.Exception)args.ExceptionObject;
			HandleException(ex.Source, ex.StackTrace);
			global::Kampai.Util.Native.CloseFileLog();
			global::UnityEngine.Application.Quit();
		}
	}
}
