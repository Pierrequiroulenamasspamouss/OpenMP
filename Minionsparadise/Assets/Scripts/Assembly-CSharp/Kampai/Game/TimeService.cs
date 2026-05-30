namespace Kampai.Game
{
	public class TimeService : global::Kampai.Game.ITimeService
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("TimeService") as global::Kampai.Util.IKampaiLogger;

		private int serverTimestamp = int.MinValue;

		private int syncTime;

		private int appTickCount;

		private global::System.DateTime startTime;

		public TimeService()
		{
#if !UNITY_WEBPLAYER && !UNITY_ANDROID
			try
			{
				startTime = global::System.Diagnostics.Process.GetCurrentProcess().StartTime;
			}
			catch (global::System.Exception)
			{
				startTime = global::System.DateTime.Now;
			}
#else
			startTime = global::System.DateTime.Now;
#endif
			new global::System.Threading.Timer(delegate
			{
				appTickCount++;
			}, null, 1000, 1000);
		}

		public int CurrentTime()
		{
			if (serverTimestamp == int.MinValue)
			{
				return ClientTime();
			}
			return ServerTime();
		}

		public int ServerTime()
		{
			return serverTimestamp + (Uptime() - syncTime);
		}

		public int ClientTime()
		{
			return global::System.Convert.ToInt32((global::System.DateTime.UtcNow - global::Kampai.Util.GameConstants.Timers.epochStart).TotalSeconds);
		}

		public int Uptime()
		{
#if !UNITY_WEBPLAYER && UNITY_ANDROID && !UNITY_EDITOR
			using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("android.os.SystemClock"))
			{
				long num = androidJavaClass.CallStatic<long>("elapsedRealtime", new object[0]);
				return global::System.Convert.ToInt32(num / 1000);
			}
#else
			return global::System.Convert.ToInt32(global::UnityEngine.Time.realtimeSinceStartup);
#endif
		}

		public int AppTime()
		{
			return appTickCount;
		}

		public void SyncServerTime(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			global::System.Collections.Generic.IDictionary<string, string> headers = response.Headers;
			if (headers != null && headers.ContainsKey("Date"))
			{
				string text = headers["Date"];
				global::System.DateTime result;
				if (global::System.DateTime.TryParse(text, out result))
				{
					serverTimestamp = global::System.Convert.ToInt32((result.ToUniversalTime() - global::Kampai.Util.GameConstants.Timers.epochStart).TotalSeconds);
					syncTime = Uptime();
					logger.Info("New game time from server: {0}", text);
				}
				else
				{
					logger.Error("Unable to parse server time '{0}'", text);
				}
			}
			else
			{
				logger.Error("Unable to set server time; using device time.");
			}
		}

		public bool WithinRange(int a, int b)
		{
			if (b < a)
			{
				int num = a;
				a = b;
				b = num;
			}
			int num2 = CurrentTime();
			return a <= num2 && num2 <= b;
		}

		public float RealtimeSinceStartup()
		{
			return (float)(global::System.DateTime.Now - startTime).TotalSeconds;
		}

		public bool WithinRange(global::Kampai.Util.IUTCRangeable rangeable, bool eternal = false)
		{
			if (eternal && rangeable.UTCStartDate == 0 && rangeable.UTCEndDate == 0)
			{
				return true;
			}
			return WithinRange(rangeable.UTCStartDate, rangeable.UTCEndDate);
		}

		public override string ToString()
		{
			return string.Format("Server: {0} Sync: {1} App: {2}", serverTimestamp, syncTime, appTickCount);
		}
	}
}
