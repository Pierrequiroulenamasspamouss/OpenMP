#if UNITY_ANDROID && !UNITY_EDITOR
using Kampai.Util;
using UnityEngine;

namespace Kampai.Util
{
	public class NativeAndroid : INativePlatform
	{
		private static readonly long APP_START_TIME = 0L;

		public string StaticConfig
		{
			get { return GetStreamingTextAsset("config"); }
		}

		public string BundleVersion
		{
			get { return global::UnityEngine.Application.version; }
		}

		public string BundleIdentifier
		{
			get { return global::UnityEngine.Application.identifier; }
		}

		public bool IsUserMusicPlaying()
		{
			return false;
		}

		public void LogError(string text)
		{
			global::UnityEngine.Debug.LogError(text);
		}

		public void LogWarning(string text)
		{
			global::UnityEngine.Debug.LogWarning(text);
		}

		public void LogInfo(string text)
		{
			global::UnityEngine.Debug.Log(text);
		}

		public void LogDebug(string text)
		{
			global::UnityEngine.Debug.Log(text);
		}

		public void LogVerbose(string text)
		{
			global::UnityEngine.Debug.Log(text);
		}

		public string GetFilePath()
		{
			return global::UnityEngine.Application.persistentDataPath + "/log-android.txt";
		}

		public string GetLastFilePath()
		{
			return global::UnityEngine.Application.persistentDataPath + "/log-android-last.txt";
		}

		public string GetLogFilesContent(int maxsize)
		{
			return string.Empty;
		}

		public uint GetMemoryUsage()
		{
			return 0u;
		}

		public void Crash()
		{
		}

		public void ScheduleLocalNotification(string type, int secondsFromNow, string title, string text, string stackedTitle, string stackedText, string action, string sound, string launchImage, int badgeNumber)
		{
		}

		public void CancelLocalNotification(string type)
		{
		}

		public void CancelAllLocalNotifications()
		{
		}

		public string GetDeviceLanguage()
		{
			return global::UnityEngine.Application.systemLanguage.ToString();
		}

		public bool AutorotationIsOSAllowed()
		{
			return true;
		}

		public bool GetBackupFlag(string path)
		{
			return false;
		}

		public void SetBackupFlag(string path, bool shouldBackup)
		{
		}

		public void AndroidFileLog(string text)
		{
		}

		public void CloseFileLog()
		{
		}

		public int GetAndroidOSVersion()
		{
			return 29;
		}

		public string GetPersistentDataPath()
		{
			return global::UnityEngine.Application.persistentDataPath;
		}

		public ulong GetAvailableStorage(string path)
		{
			return 1024uL * 1024uL * 1024uL;
		}

		public bool CanShowNetworkSettings()
		{
			return false;
		}

		public void OpenNetworkSettings()
		{
		}

		public byte[] GetStreamingAsset(string path)
		{
			return null;
		}

		public string GetStreamingTextAsset(string path)
		{
			if (path.EndsWith(".json"))
			{
				path = path.Substring(0, path.Length - 5);
			}
			global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>(path);
			if (textAsset != null)
			{
				return textAsset.text.Trim();
			}
			global::UnityEngine.Debug.LogError("NativeAndroid: Failed to load streaming asset " + path + " from Resources");
			return string.Empty;
		}

		public void Exit()
		{
			global::UnityEngine.Application.Quit();
		}

		public bool AreNotificationsEnabled()
		{
			return true;
		}

		public string GetDeviceHardwareModel()
		{
			return global::UnityEngine.SystemInfo.deviceModel;
		}

		public long GetAppStartupTime()
		{
			return APP_START_TIME;
		}

		public void OpenAppStoreLink(string appId)
		{
		}

		public bool IsAppInstalled(string appIdURL)
		{
			return false;
		}

		public void LaunchApp(string appId)
		{
		}

		public bool CanOpenURL(string URL)
		{
			return true;
		}

		public void OpenURL(string URL)
		{
			global::UnityEngine.Application.OpenURL(URL);
		}
	}
}
#endif
