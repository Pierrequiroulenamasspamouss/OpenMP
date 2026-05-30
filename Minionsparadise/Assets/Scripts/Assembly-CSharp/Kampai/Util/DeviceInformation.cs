namespace Kampai.Util
{
	public class DeviceInformation
	{
		public string model;

		public int processorCount;

		public int graphicsShaderLevel;

		public int vram;

		public int ram;

		public int screenWidth;

		public int screenHeight;

		public int screenRefresh;

		public DeviceInformation()
		{
			global::UnityEngine.Resolution currentResolution = global::UnityEngine.Screen.currentResolution;
			screenWidth = currentResolution.width;
			screenHeight = currentResolution.height;
			screenRefresh = (int)global::System.Math.Round(currentResolution.refreshRateRatio.value);
			model = global::UnityEngine.SystemInfo.deviceModel;
			processorCount = global::UnityEngine.SystemInfo.processorCount;
			ram = global::UnityEngine.SystemInfo.systemMemorySize;
			vram = global::UnityEngine.SystemInfo.graphicsMemorySize;
			graphicsShaderLevel = global::UnityEngine.SystemInfo.graphicsShaderLevel;
		}

		public bool IsSamsung()
		{
			return model.ToUpper().Contains("SAMSUNG");
		}
	}
}
