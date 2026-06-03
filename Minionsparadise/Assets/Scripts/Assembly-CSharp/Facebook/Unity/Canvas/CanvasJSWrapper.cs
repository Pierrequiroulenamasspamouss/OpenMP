namespace Discord.Unity.Canvas
{
	internal class CanvasJSWrapper : global::Discord.Unity.Canvas.ICanvasJSWrapper
	{
		private const string JSSDKBindingFileName = "JSSDKBindings";

		public string IntegrationMethodJs
		{
			get
			{
				global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load("JSSDKBindings") as global::UnityEngine.TextAsset;
				if ((bool)textAsset)
				{
					return textAsset.text;
				}
				return null;
			}
		}

		public string GetSDKVersion()
		{
			return "v2.5";
		}

		public void ExternalCall(string functionName, params object[] args)
		{
		}

		public void ExternalEval(string script)
		{
		}

		public void DisableFullScreen()
		{
			if (global::UnityEngine.Screen.fullScreen)
			{
				global::UnityEngine.Screen.fullScreen = false;
			}
		}
	}
}
