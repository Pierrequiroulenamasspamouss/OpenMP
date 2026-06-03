#if !UNITY_WEBPLAYER && (!UNITY_ANDROID || UNITY_EDITOR)
using Kampai.Util;
using UnityEngine;

namespace Kampai.Util
{
	public class NativeDefault : INativePlatform
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
				global::UnityEngine.Debug.LogError("NativeDefault: Failed to load config from Resources");
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

		private static global::System.IO.StreamWriter sw = null;

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
			string result = string.Empty;
			string lastFilePath = GetLastFilePath();
			if (global::System.IO.File.Exists(lastFilePath))
			{
				string text = global::System.IO.File.ReadAllText(lastFilePath);
				result = ((text.Length <= maxsize) ? text : text.Substring(text.Length - maxsize));
			}
			return result;
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

		public void AndroidFileLog(string text)
		{
			string filePath = GetFilePath();
			if (sw == null)
			{
				if (!global::System.IO.File.Exists(filePath))
				{
					sw = global::System.IO.File.CreateText(filePath);
				}
				else
				{
					sw = global::System.IO.File.AppendText(filePath);
				}
			}
			if (sw != null)
			{
				sw.WriteLine(text + "\n");
			}
		}

		public void CloseFileLog()
		{
			if (sw != null)
			{
				sw.Close();
				sw = null;
			}
			if (global::System.IO.File.Exists(GetLastFilePath()))
			{
				global::System.IO.File.Delete(GetLastFilePath());
			}
			if (global::System.IO.File.Exists(GetFilePath()))
			{
				global::System.IO.File.Copy(GetFilePath(), GetLastFilePath());
			}
		}

		public int GetAndroidOSVersion()
		{
			return 19;
		}

		private string cachedPersistentDataPath = null;

		public string GetPersistentDataPath()
		{
			if (cachedPersistentDataPath == null)
			{
				cachedPersistentDataPath = global::UnityEngine.Application.persistentDataPath;
			}
			return cachedPersistentDataPath;
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
			string fullPath = global::System.IO.Path.Combine(global::UnityEngine.Application.streamingAssetsPath, path);
			if (global::System.IO.File.Exists(fullPath))
			{
				return global::System.IO.File.ReadAllBytes(fullPath);
			}
			throw new global::System.IO.FileNotFoundException(fullPath);
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
			string fullPath = global::System.IO.Path.Combine(global::UnityEngine.Application.streamingAssetsPath, path);
			if (global::System.IO.File.Exists(fullPath))
			{
				return global::System.IO.File.ReadAllText(fullPath);
			}
			throw new global::System.IO.FileNotFoundException(fullPath);
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
			return true;
		}

		public void OpenURL(string URL)
		{
			global::UnityEngine.Application.OpenURL(URL);
		}
	}
}
#endif
