using UnityEngine.Networking;

namespace Swrve
{
	public static class CrossPlatformUtils
	{
		public static UnityWebRequest MakeWebRequest(string url, byte[] encodedData, global::System.Collections.Generic.Dictionary<string, string> headers)
		{
			UnityWebRequest webRequest;
			
			if (encodedData != null && encodedData.Length > 0)
			{
				webRequest = UnityWebRequest.PostWwwForm(url, "");
				webRequest.uploadHandler = new UploadHandlerRaw(encodedData);
				webRequest.uploadHandler.contentType = "application/json";
			}
			else
			{
				webRequest = UnityWebRequest.Get(url);
			}
			
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			
			if (headers != null)
			{
				foreach (var header in headers)
				{
					webRequest.SetRequestHeader(header.Key, header.Value);
				}
			}
			
			return webRequest;
		}
	}
}
