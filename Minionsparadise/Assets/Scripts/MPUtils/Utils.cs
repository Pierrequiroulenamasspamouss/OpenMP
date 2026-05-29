using System.Collections.Generic;
using UnityEngine;

namespace MPUtils
{
    public static class Utils
    {
        private const string game = "v0.0.4";
        private const string auth = "markut";

        private static readonly Dictionary<string, float> _timers = new Dictionary<string, float>();

        private static string Platform
        {
            get
            {
#if UNITY_EDITOR
                return "Editor";
#elif UNITY_ANDROID
                return "Android";
#elif UNITY_IOS
                return "iOS";
#elif UNITY_WEBGL
                return "WebGL";
#elif UNITY_STANDALONE_WIN
                return "Windows";
#elif UNITY_STANDALONE_OSX
                return "macOS";
#elif UNITY_STANDALONE_LINUX
                return "Linux";
#else
                return "Unknown";
#endif
            }
        }

        private static string UnityVersion
        {
            get
            {
                string full = Application.unityVersion;
                string[] parts = full.Split('.');
                if (parts.Length > 0 && parts[0] == "6000")
                {
                    return "Unity 6";
                }
                return parts.Length > 0 ? string.Concat("Unity ", parts[0]) : string.Concat("Unity ", full);
            }
        }

        private static string Header
        {
            get
            {
                return string.Concat("[", auth, " | ", game, " | ", UnityVersion, " | ", Platform, "]");
            }
        }

        public static void Log(string message)
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            Debug.Log(string.Concat(Header, " ", message));
#endif
        }

        public static void Warning(string message)
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            Debug.LogWarning(string.Concat(Header, " WARNING: ", message));
#endif
        }

        public static void Error(string message)
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            Debug.LogError(string.Concat(Header, " ERROR: ", message));
#endif
        }

        public static void Separator()
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            Debug.Log(string.Concat(Header, " ---------------------------------"));
#endif
        }

        public static void LogIf(bool condition, string message)
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            if (condition) Log(message);
#endif
        }

        public static void Assert(bool condition, string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!condition) 
            {
                Debug.LogError(string.Concat(Header, " ASSERT FAILED: ", message));
                if (Application.isEditor)
                {
                    Debug.Break();
                }
            }
#endif
        }

        public static void StartTimer(string name)
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            _timers[name] = Time.realtimeSinceStartup;
            Log(string.Concat("TIMER: '", name, "' started."));
#endif
        }

        public static void StopTimer(string name)
        {
#if UNITY_EDITOR || !UNITY_EDITOR
            if (_timers.ContainsKey(name))
            {
                float elapsed = Time.realtimeSinceStartup - _timers[name];
                _timers.Remove(name);
                Log(string.Concat("TIMER: '", name, "' stopped - ", elapsed.ToString("F3"), "s"));
            }
            else
            {
                Warning(string.Concat("TIMER: '", name, "' not found."));
            }
#endif
        }

        public static void LogPerformance(string metric, float value)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Log(string.Concat("PERF: ", metric, " = ", value.ToString("F2")));
#endif
        }

        public static void DrawDebugRay(Vector3 start, Vector3 end, Color color)
        {
#if UNITY_EDITOR
            Debug.DrawRay(start, end, color);
#endif
        }

        public static void DrawDebugLine(Vector3 start, Vector3 end, Color color)
        {
#if UNITY_EDITOR
            Debug.DrawLine(start, end, color);
#endif
        }

        public static void DrawDebugSphere(Vector3 position, float radius, Color color)
        {
#if UNITY_EDITOR
            Debug.DrawRay(position, Vector3.up * radius, color);
#endif
        }

        public static void LogDeviceInfo()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            float fps = 1f / Time.deltaTime;
            int ram = SystemInfo.systemMemorySize;

            Separator();
            Log(string.Concat("RAM    : ", ram, " MB"));
            Log(string.Concat("FPS    : ", fps.ToString("F1")));
            Log(string.Concat("Device : ", SystemInfo.deviceModel));
            Log(string.Concat("OS     : ", SystemInfo.operatingSystem));
            Separator();
#endif
        }

        public static int GetRAM()
        {
            return SystemInfo.systemMemorySize;
        }

        public static float GetFPS()
        {
            return 1f / Time.deltaTime;
        }
    }
}
