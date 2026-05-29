#if UNITY_WEBPLAYER
using Kampai.Util;
using UnityEngine;

namespace Kampai.Util
{
	public class NativeWebPlayer : INativePlatform
	{
		public string StaticConfig
		{
			get
			{
				global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>("config");
				if (textAsset != null)
				{
					return textAsset.text.Trim();
				}
				global::UnityEngine.Debug.LogError("NativeWebPlayer: Failed to load config from Resources");
				return string.Empty;
			}
		}

		public string BundleVersion { get { return "1.0.0"; } }
		public string BundleIdentifier { get { return global::UnityEngine.Application.identifier; } }

		public bool IsUserMusicPlaying()
		{
			return false;
		}

		public void LogError(string text) { }
		public void LogWarning(string text) { }
		public void LogInfo(string text) { }
		public void LogDebug(string text) { }
		public void LogVerbose(string text) { }

		public string GetFilePath()
		{
			return global::Kampai.Util.GameConstants.PERSISTENT_DATA_PATH + "/log.txt";
		}

		public string GetLastFilePath()
		{
			return global::Kampai.Util.GameConstants.PERSISTENT_DATA_PATH + "/log-last.txt";
		}

		public string GetLogFilesContent(int maxsize)
		{
			return string.Empty;
		}

		public uint GetMemoryUsage()
		{
			return (uint)global::UnityEngine.Profiling.Profiler.usedHeapSizeLong;
		}

		public void Crash() { }

		public void ScheduleLocalNotification(string type, int secondsFromNow, string title, string text, string stackedTitle, string stackedText, string action, string sound, string launchImage, int badgeNumber) { }
		public void CancelLocalNotification(string type) { }
		public void CancelAllLocalNotifications() { }

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

		public void SetBackupFlag(string path, bool shouldBackup) { }

		public void AndroidFileLog(string text) { }
		public void CloseFileLog() { }

		public int GetAndroidOSVersion()
		{
			return 19;
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
			return true;
		}

		public void OpenNetworkSettings() { }

		public byte[] GetStreamingAsset(string path)
		{
			string fullPath = global::System.IO.Path.Combine(global::UnityEngine.Application.streamingAssetsPath, path).Replace("\\", "/");
			if (!fullPath.Contains("://"))
			{
				fullPath = "file:///" + fullPath;
			}
			global::UnityEngine.WWW www = new global::UnityEngine.WWW(fullPath);
			while (!www.isDone) {}
			if (!string.IsNullOrEmpty(www.error) || www.bytesDownloaded == 0)
			{
				throw new global::System.IO.FileNotFoundException(fullPath);
			}
			return www.bytes;
		}

		public string GetStreamingTextAsset(string path)
		{
			string resourcePath = path;
			if (resourcePath.EndsWith(".json"))
			{
				resourcePath = resourcePath.Substring(0, resourcePath.Length - 5);
			}
			global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>(resourcePath);
			if (textAsset != null)
			{
				return textAsset.text;
			}
			string fullPath = global::System.IO.Path.Combine(global::UnityEngine.Application.streamingAssetsPath, path).Replace("\\", "/");
			if (!fullPath.Contains("://"))
			{
				fullPath = "file:///" + fullPath;
			}
			global::UnityEngine.WWW www = new global::UnityEngine.WWW(fullPath);
			while (!www.isDone) {}
			if (!string.IsNullOrEmpty(www.error) || (www.text != null && www.text.TrimStart().StartsWith("<")) || www.bytesDownloaded == 0)
			{
				throw new global::System.IO.FileNotFoundException(fullPath);
			}
			return www.text;
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
			return 0L;
		}

		public void OpenAppStoreLink(string appId) { }

		public bool IsAppInstalled(string appIdURL)
		{
			return false;
		}

		public void LaunchApp(string appId) { }

		public bool CanOpenURL(string URL)
		{
			return false;
		}

		public void OpenURL(string URL)
		{
			global::UnityEngine.Application.OpenURL(URL);
		}
	}
}
#endif
