using System.Diagnostics;

namespace Kampai.Util
{
    public static class StartupTimer
    {
        private static Stopwatch _stopwatch;
        private static float _lastTime;

        public static void Start()
        {
            _stopwatch = Stopwatch.StartNew();
            _lastTime = 0f;
            UnityEngine.Debug.LogFormat("<color=orange>[STARTUP_TIMER] Starting startup timer...</color>");
        }

        public static void LogCheckpoint(string name)
        {
            if (_stopwatch == null)
            {
                _stopwatch = Stopwatch.StartNew();
            }
            float total = (float)_stopwatch.Elapsed.TotalSeconds;
            float diff = total - _lastTime;
            _lastTime = total;
            UnityEngine.Debug.LogFormat("<color=cyan>[STARTUP_TIMER] Checkpoint: {0} | Elapsed since last: {1:F3}s | Total: {2:F3}s</color>", name, diff, total);
        }
    }
}
