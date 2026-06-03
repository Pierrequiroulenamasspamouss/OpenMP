public class HockeyAppIOS : global::UnityEngine.MonoBehaviour
{
	protected const string HOCKEYAPP_BASEURL = "https://rink.hockeyapp.net/";

	protected const string HOCKEYAPP_CRASHESPATH = "api/2/apps/[APPID]/crashes/upload";

	protected const int MAX_CHARS = 199800;

	protected const string LOG_FILE_DIR = "/logs/";

	protected const string DATE_LABEL = "Date:";

	protected const string TELEMETRY_DATE_FORMAT = "yyyymmdd_HHmmss";

	protected const string LOG_DATE_FORMAT = "ddd MMM dd HH:mm:ss {}zzzz yyyy";

	protected const string LOCAL_TIMEZONE = "GMT";

	public string appID = string.Empty;

	public string secret = string.Empty;

	public string authenticationType = string.Empty;

	public string serverURL = string.Empty;

	public bool autoUpload;

	public bool exceptionLogging;

	public bool updateManager;

	public string userId = string.Empty;

	public global::System.Action crashReportCallback;

	internal global::Kampai.Common.ITelemetryService telemetryService;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void GameViewLoaded(string message)
	{
	}

	protected virtual global::System.Collections.Generic.List<string> GetLogHeaders()
	{
		return new global::System.Collections.Generic.List<string>();
	}

	protected virtual global::UnityEngine.WWWForm CreateForm(string log)
	{
		return new global::UnityEngine.WWWForm();
	}

	protected virtual global::System.Collections.Generic.List<string> GetLogFiles()
	{
		return new global::System.Collections.Generic.List<string>();
	}

	protected virtual global::System.Collections.IEnumerator SendLogs(global::System.Collections.Generic.List<string> logs)
	{
		string crashPath = "api/2/apps/[APPID]/crashes/upload";
		string url = GetBaseURL() + crashPath.Replace("[APPID]", appID);
		foreach (string log in logs)
		{
			global::UnityEngine.WWWForm postForm = CreateForm(log);
			if (postForm.data != null)
			{
				SendCrashTelemetry(global::System.Text.Encoding.UTF8.GetString(postForm.data));
			}
			string lContent = postForm.headers["Content-Type"].ToString();
			lContent = lContent.Replace("\"", string.Empty);
			
			using (global::UnityEngine.Networking.UnityWebRequest webRequest = new global::UnityEngine.Networking.UnityWebRequest(url, "POST"))
			{
				webRequest.uploadHandler = new global::UnityEngine.Networking.UploadHandlerRaw(postForm.data);
				webRequest.uploadHandler.contentType = lContent;
				webRequest.downloadHandler = new global::UnityEngine.Networking.DownloadHandlerBuffer();
				webRequest.SetRequestHeader("Content-Type", lContent);
				yield return webRequest.SendWebRequest();
				
				if (string.IsNullOrEmpty(webRequest.error))
				{
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
				}
				if (crashReportCallback != null)
				{
					crashReportCallback();
				}
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
	}

	protected virtual string GetBaseURL()
	{
		return string.Empty;
	}

	protected virtual bool IsConnected()
	{
		return false;
	}

	protected virtual void HandleException(string logString, string stackTrace)
	{
	}

	public void OnHandleLogCallback(string logString, string stackTrace, global::UnityEngine.LogType type)
	{
	}

	public void OnHandleUnresolvedException(object sender, global::System.UnhandledExceptionEventArgs args)
	{
	}
}
