namespace Kampai.Util
{
	public class TimeProfiler : global::System.IDisposable
	{
		private sealed class FrameCounter : global::UnityEngine.MonoBehaviour
		{
			public int Frame;

			public global::System.Action<int, float> LongFrameOccured;

			private global::UnityEngine.Coroutine updateCoroutine;

			private bool isRunning = true;

			public void Start()
			{
				updateCoroutine = StartCoroutine(UpdateFrameStats());
			}

			public void StopUpdateCoroutine()
			{
				isRunning = false;
				StopCoroutine(updateCoroutine);
			}

			public void Update()
			{
				float unscaledDeltaTime = global::UnityEngine.Time.unscaledDeltaTime;
				if ((double)unscaledDeltaTime > 0.1)
				{
					LongFrameOccured(global::UnityEngine.Time.frameCount - 1, unscaledDeltaTime);
				}
			}

			public global::System.Collections.IEnumerator UpdateFrameStats()
			{
				while (isRunning)
				{
					yield return new global::UnityEngine.WaitForEndOfFrame();
					Frame = global::UnityEngine.Time.frameCount + 1;
				}
			}
		}

		private sealed class Section
		{
			private string name;

			private global::System.Diagnostics.Stopwatch timer;

			private global::System.TimeSpan assetsLoadTime;

			private global::System.TimeSpan subsectionLoadTime;

			private int numberOfAssetsLoaded;

			private bool wasReleased;

			public Section(string name)
			{
				this.name = name;
				timer = global::System.Diagnostics.Stopwatch.StartNew();
			}

			public string FormatSectionTime()
			{
				return GetSectionTime().ToString();
			}

			public global::System.TimeSpan GetSectionTime()
			{
				return timer.Elapsed;
			}

			public string GetSectionName()
			{
				return name;
			}

			public void mergeSubSection(global::System.TimeSpan time)
			{
				assetsLoadTime += time;
				numberOfAssetsLoaded++;
			}

			public void mergeSubSection(global::Kampai.Util.TimeProfiler.Section section)
			{
				assetsLoadTime += section.assetsLoadTime;
				numberOfAssetsLoaded += section.numberOfAssetsLoaded;
				subsectionLoadTime += section.timer.Elapsed;
			}

			public string FormatAssetsLoadTime()
			{
				return GetAssetsLoadTime().ToString();
			}

			public global::System.TimeSpan GetAssetsLoadTime()
			{
				return assetsLoadTime;
			}

			public int GetNumberOfAssetsLoaded()
			{
				return numberOfAssetsLoaded;
			}

			public string FormatTotalSubSectionsTime()
			{
				return GetTotalSubSectionsTime().ToString();
			}

			public global::System.TimeSpan GetTotalSubSectionsTime()
			{
				return subsectionLoadTime;
			}

			public void ReleaseSection()
			{
				wasReleased = true;
			}

			public bool IsSectionReleased()
			{
				return wasReleased;
			}
		}

		private static global::Kampai.Util.TimeProfiler currentInstance;

		private global::System.IO.StreamWriter stream;

		private global::System.Text.StringBuilder stringBuilder;

		private global::Kampai.Util.IKampaiLogger logger;

		private global::System.TimeSpan totalAssetsLoadTime;

		private global::UnityEngine.GameObject frameCounterGO;

		private global::Kampai.Util.TimeProfiler.FrameCounter frameCounter;

		private static readonly string PROFILER_LOG_PATH;

		private static readonly global::System.Type MonoProfiler_Profiler;

		private static readonly global::System.Reflection.MethodInfo MonoProfiler_Profiler_Start;

		private static readonly global::System.Reflection.MethodInfo MonoProfiler_Profiler_Stop;

		private global::System.Collections.Generic.List<global::Kampai.Util.TimeProfiler.Section> sections = new global::System.Collections.Generic.List<global::Kampai.Util.TimeProfiler.Section>();

		protected bool Disposed { get; private set; }

		private TimeProfiler()
		{
#if !UNITY_WEBPLAYER
			string path = "timelog_" + global::System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";
			string path2 = global::System.IO.Path.Combine(global::Kampai.Util.GameConstants.PERSISTENT_DATA_PATH, path);
			stream = new global::System.IO.StreamWriter(path2);
			stream.AutoFlush = true;
#endif
			stringBuilder = new global::System.Text.StringBuilder();
			frameCounterGO = new global::UnityEngine.GameObject("FrameCounter");
			frameCounter = frameCounterGO.AddComponent<global::Kampai.Util.TimeProfiler.FrameCounter>();
			global::Kampai.Util.TimeProfiler.FrameCounter obj = frameCounter;
			obj.LongFrameOccured = (global::System.Action<int, float>)global::System.Delegate.Combine(obj.LongFrameOccured, new global::System.Action<int, float>(LogLongFrame));
		}

		static TimeProfiler()
		{
			PROFILER_LOG_PATH = global::Kampai.Util.GameConstants.PERSISTENT_DATA_PATH + "/profiler/";
			global::System.Reflection.Assembly[] assemblies = global::System.AppDomain.CurrentDomain.GetAssemblies();
			foreach (global::System.Reflection.Assembly assembly in assemblies)
			{
				if (assembly.GetName().Name.Equals("MonoProfiler"))
				{
					MonoProfiler_Profiler = assembly.GetType("MonoProfiler.Profiler");
				}
			}
			if (MonoProfiler_Profiler != null)
			{
				MonoProfiler_Profiler_Start = MonoProfiler_Profiler.GetMethod("Start", new global::System.Type[2]
				{
					typeof(string),
					typeof(bool)
				});
				MonoProfiler_Profiler_Stop = MonoProfiler_Profiler.GetMethod("Stop", global::System.Type.EmptyTypes);
			}
		}

		private global::Kampai.Util.TimeProfiler.Section GetLastSection(bool pop)
		{
			int count = sections.Count;
			if (count == 0)
			{
				return null;
			}
			global::Kampai.Util.TimeProfiler.Section section = sections[count - 1];
			if (pop)
			{
				sections.Remove(section);
			}
			return section;
		}

		private void Write(string message)
		{
#if !UNITY_WEBPLAYER
			if (currentInstance != null && currentInstance.stream != null)
			{
				currentInstance.stream.Write(message);
				currentInstance.stringBuilder.Append(message);
			}
#endif
		}

		private void Log()
		{
			if (currentInstance != null && currentInstance.logger != null)
			{
				currentInstance.logger.Log(global::Kampai.Util.KampaiLogLevel.Info, currentInstance.stringBuilder.ToString());
				currentInstance.stringBuilder.Length = 0;
			}
		}

		private void LogLongFrame(int frameNumber, float time)
		{
			// string message = string.Format("!!!! Frame {0} took {1} s.\n", frameNumber, time);
			// Write(message);
		}

		private static string GetTimeStamp()
		{
			return global::System.DateTime.Now.ToString("HH:mm:ss.ffff");
		}

		public static void Reset(bool enabled)
		{
			if (currentInstance != null)
			{
				Flush();
				if (currentInstance.frameCounter != null)
				{
					currentInstance.frameCounter.StopUpdateCoroutine();
					global::UnityEngine.Object.Destroy(currentInstance.frameCounterGO);
				}
				currentInstance.Dispose();
				currentInstance = null;
			}
			if (enabled)
			{
				currentInstance = new global::Kampai.Util.TimeProfiler();
			}
		}

		public static void Flush()
		{
#if !UNITY_WEBPLAYER
			if (currentInstance != null && currentInstance.stream != null)
			{
				currentInstance.stream.Flush();
				currentInstance.Log();
			}
#endif
		}

		public static void StartSection(string sectionName)
		{
			if (currentInstance != null)
			{
				global::Kampai.Util.TimeProfiler.Section section = new global::Kampai.Util.TimeProfiler.Section(sectionName);
				currentInstance.sections.Add(section);
				string message = string.Format("{0}>{1}{2} (frame {3})\n", GetTimeStamp(), new string('\t', currentInstance.sections.Count - 1), section.GetSectionName(), currentInstance.frameCounter.Frame);
				currentInstance.Write(message);
			}
		}

		public static void EndSection(string sectionName)
		{
			if (currentInstance == null)
			{
				return;
			}
			global::Kampai.Util.TimeProfiler.Section lastSection = currentInstance.GetLastSection(false);
			if (lastSection != null && lastSection.GetSectionName() == sectionName)
			{
				lastSection.ReleaseSection();
				while (lastSection != null && lastSection.IsSectionReleased())
				{
					PopLatestSection();
					lastSection = currentInstance.GetLastSection(false);
				}
			}
			else
			{
				foreach (global::Kampai.Util.TimeProfiler.Section section in currentInstance.sections)
				{
					if (section.GetSectionName() == sectionName)
					{
						section.ReleaseSection();
						break;
					}
				}
			}
			currentInstance.Log();
		}

		private static void PopLatestSection()
		{
			global::Kampai.Util.TimeProfiler.Section lastSection = currentInstance.GetLastSection(true);
			global::Kampai.Util.TimeProfiler.Section lastSection2 = currentInstance.GetLastSection(false);
			if (lastSection2 != null)
			{
				lastSection2.mergeSubSection(lastSection);
			}
			string message = string.Format("{0}<{1}{2} in {3}. ({4} assets loaded in {5}). {6} in section. (frame {7})\n", GetTimeStamp(), new string('\t', currentInstance.sections.Count), lastSection.GetSectionName(), lastSection.FormatSectionTime(), lastSection.GetNumberOfAssetsLoaded(), lastSection.FormatAssetsLoadTime(), (lastSection2 == null) ? string.Empty : lastSection2.FormatTotalSubSectionsTime(), currentInstance.frameCounter.Frame);
			currentInstance.Write(message);
		}

		public static void StartAssetLoadSection(string name)
		{
			if (currentInstance != null)
			{
				global::Kampai.Util.TimeProfiler.Section section = new global::Kampai.Util.TimeProfiler.Section(name);
				currentInstance.sections.Add(section);
			}
		}

		public static void EndAssetLoadSection()
		{
			if (currentInstance != null)
			{
				global::Kampai.Util.TimeProfiler.Section lastSection = currentInstance.GetLastSection(true);
				if (lastSection != null)
				{
					global::System.TimeSpan sectionTime = lastSection.GetSectionTime();
					currentInstance.totalAssetsLoadTime += sectionTime;
					if (currentInstance.sections.Count > 0)
					{
						currentInstance.GetLastSection(false).mergeSubSection(sectionTime);
					}
				}
			}
		}

		public static void StartMonoProfiler(string sectionName)
		{
			if (MonoProfiler_Profiler == null)
			{
				if (currentInstance != null && currentInstance.logger != null)
				{
					currentInstance.logger.Debug("Failed to start the profiler, MonoProfiler.Profiler class is not found");
				}
				return;
			}
			if (MonoProfiler_Profiler_Start == null)
			{
				if (currentInstance != null && currentInstance.logger != null)
				{
					currentInstance.logger.Debug("Failed to start the profiler, MonoProfiler.Profiler.Start method is not found");
				}
				return;
			}
#if !UNITY_WEBPLAYER
			if (!global::System.IO.Directory.Exists(PROFILER_LOG_PATH))
			{
				global::System.IO.Directory.CreateDirectory(PROFILER_LOG_PATH);
			}
#endif
			string text = global::System.IO.Path.Combine(PROFILER_LOG_PATH, sectionName);
			MonoProfiler_Profiler_Start.Invoke(null, new object[2] { text, false });
		}

		public static void StopMonoProfiler()
		{
			if (MonoProfiler_Profiler != null && MonoProfiler_Profiler_Stop != null)
			{
				MonoProfiler_Profiler_Stop.Invoke(null, new object[0]);
			}
		}

		public static void InitializeLogger(global::Kampai.Util.IKampaiLogger logger)
		{
			if (currentInstance != null)
			{
				currentInstance.logger = logger;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			global::System.GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
#if !UNITY_WEBPLAYER
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
#endif
			}
			Disposed = true;
		}

		~TimeProfiler()
		{
			Dispose(false);
		}
	}
}
