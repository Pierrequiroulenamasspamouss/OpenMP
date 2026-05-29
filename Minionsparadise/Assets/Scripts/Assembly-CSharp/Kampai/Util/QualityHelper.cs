namespace Kampai.Util
{
	public static class QualityHelper
	{
		private const int AntiAliasSamplesMedium = 4;

		private const int AntiAliasSamplesHigh = 4;

		public static global::Kampai.Util.TargetPerformance getDlcHD(global::Kampai.Util.TargetPerformance target)
		{
			switch (target)
			{
			case global::Kampai.Util.TargetPerformance.HIGH:
				return global::Kampai.Util.TargetPerformance.HIGH;
			case global::Kampai.Util.TargetPerformance.MED:
				return global::Kampai.Util.TargetPerformance.HIGH;
			case global::Kampai.Util.TargetPerformance.LOW:
				return global::Kampai.Util.TargetPerformance.MED;
			default:
				return global::Kampai.Util.TargetPerformance.VERYLOW;
			}
		}

		public static global::Kampai.Util.TargetPerformance getDlcSD(global::Kampai.Util.TargetPerformance target)
		{
			switch (target)
			{
			case global::Kampai.Util.TargetPerformance.HIGH:
				return global::Kampai.Util.TargetPerformance.MED;
			case global::Kampai.Util.TargetPerformance.MED:
				return global::Kampai.Util.TargetPerformance.MED;
			case global::Kampai.Util.TargetPerformance.LOW:
				return global::Kampai.Util.TargetPerformance.LOW;
			default:
				return global::Kampai.Util.TargetPerformance.VERYLOW;
			}
		}

		public static string getStartingQuality(global::Kampai.Util.TargetPerformance target)
		{
			switch (target)
			{
			case global::Kampai.Util.TargetPerformance.HIGH:
				return "DLCHDPack";
			case global::Kampai.Util.TargetPerformance.MED:
				return "DLCSDPack";
			default:
				return "DLCSDPack";
			}
		}

		public static global::Kampai.Util.TargetPerformance getCurrentTarget(global::Kampai.Util.TargetPerformance deviceTarget, string Quality)
		{
			if (Quality == "DLCHDPack")
			{
				return getDlcHD(deviceTarget);
			}
			return getDlcSD(deviceTarget);
		}

		public static int getAntiAliasingSamples(global::Kampai.Util.TargetPerformance target)
		{
			switch (target)
			{
			case global::Kampai.Util.TargetPerformance.HIGH:
				return AntiAliasSamplesHigh;
			case global::Kampai.Util.TargetPerformance.MED:
				return AntiAliasSamplesMedium;
			default:
				return 0;
			}
		}

		public static void applyAntiAliasing(global::Kampai.Util.TargetPerformance target)
		{
			global::UnityEngine.QualitySettings.antiAliasing = getAntiAliasingSamples(target);
		}
	}
}
