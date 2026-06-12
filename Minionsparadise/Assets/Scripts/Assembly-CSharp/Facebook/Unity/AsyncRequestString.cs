namespace Discord.Unity
{
	internal class AsyncRequestString : global::UnityEngine.MonoBehaviour
	{
		private global::System.Uri url;

		private global::Discord.Unity.HttpMethod method;

		private global::System.Collections.Generic.IDictionary<string, string> formData;

		private global::UnityEngine.WWWForm query;

		private global::Discord.Unity.FacebookDelegate<global::Discord.Unity.IGraphResult> callback;

		internal static void Post(global::System.Uri url, global::System.Collections.Generic.Dictionary<string, string> formData = null, global::Discord.Unity.FacebookDelegate<global::Discord.Unity.IGraphResult> callback = null)
		{
			Request(url, global::Discord.Unity.HttpMethod.POST, formData, callback);
		}

		internal static void Get(global::System.Uri url, global::System.Collections.Generic.Dictionary<string, string> formData = null, global::Discord.Unity.FacebookDelegate<global::Discord.Unity.IGraphResult> callback = null)
		{
			Request(url, global::Discord.Unity.HttpMethod.GET, formData, callback);
		}

		internal static void Request(global::System.Uri url, global::Discord.Unity.HttpMethod method, global::UnityEngine.WWWForm query = null, global::Discord.Unity.FacebookDelegate<global::Discord.Unity.IGraphResult> callback = null)
		{
			global::Discord.Unity.ComponentFactory.AddComponent<global::Discord.Unity.AsyncRequestString>().SetUrl(url).SetMethod(method)
				.SetQuery(query)
				.SetCallback(callback);
		}

		internal static void Request(global::System.Uri url, global::Discord.Unity.HttpMethod method, global::System.Collections.Generic.IDictionary<string, string> formData = null, global::Discord.Unity.FacebookDelegate<global::Discord.Unity.IGraphResult> callback = null)
		{
			global::Discord.Unity.ComponentFactory.AddComponent<global::Discord.Unity.AsyncRequestString>().SetUrl(url).SetMethod(method)
				.SetFormData(formData)
				.SetCallback(callback);
		}

		internal global::System.Collections.IEnumerator Start()
		{
			global::UnityEngine.Networking.UnityWebRequest webRequest;
			if (method == global::Discord.Unity.HttpMethod.GET)
			{
				string urlParams = ((!url.AbsoluteUri.Contains("?")) ? "?" : "&");
				if (formData != null)
				{
					foreach (global::System.Collections.Generic.KeyValuePair<string, string> pair in formData)
					{
						urlParams += string.Format("{0}={1}&", global::System.Uri.EscapeDataString(pair.Key), global::System.Uri.EscapeDataString(pair.Value));
					}
				}
				string fullUrl = string.Concat(url, urlParams);
				webRequest = global::UnityEngine.Networking.UnityWebRequest.Get(fullUrl);
				webRequest.SetRequestHeader("User-Agent", global::Discord.Unity.Constants.GraphApiUserAgent);
			}
			else
			{
				if (query != null)
				{
					byte[] queryData = query.data;
					if (queryData != null && queryData.Length > 0)
					{
						webRequest = new global::UnityEngine.Networking.UnityWebRequest(url.AbsoluteUri, "POST");
						webRequest.uploadHandler = new global::UnityEngine.Networking.UploadHandlerRaw(queryData);
						webRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
					}
					else
					{
						webRequest = global::UnityEngine.Networking.UnityWebRequest.PostWwwForm(url.AbsoluteUri, "");
					}
				}
				else if (formData != null)
				{
					webRequest = global::UnityEngine.Networking.UnityWebRequest.PostWwwForm(url.AbsoluteUri, "");
					foreach (var kvp in formData)
					{
						webRequest.uploadHandler = new global::UnityEngine.Networking.UploadHandlerRaw(global::System.Text.Encoding.UTF8.GetBytes(kvp.Key + "=" + global::System.Uri.EscapeDataString(kvp.Value) + "&"));
					}
				}
				else
				{
					webRequest = global::UnityEngine.Networking.UnityWebRequest.PostWwwForm(url.AbsoluteUri, "");
				}
				webRequest.SetRequestHeader("User-Agent", global::Discord.Unity.Constants.GraphApiUserAgent);
			}
			webRequest.downloadHandler = new global::UnityEngine.Networking.DownloadHandlerBuffer();
			yield return webRequest.SendWebRequest();
			if (callback != null)
			{
				callback(new global::Discord.Unity.GraphResult(webRequest));
			}
			webRequest.Dispose();
			global::UnityEngine.Object.Destroy(this);
		}

		internal global::Discord.Unity.AsyncRequestString SetUrl(global::System.Uri url)
		{
			this.url = url;
			return this;
		}

		internal global::Discord.Unity.AsyncRequestString SetMethod(global::Discord.Unity.HttpMethod method)
		{
			this.method = method;
			return this;
		}

		internal global::Discord.Unity.AsyncRequestString SetFormData(global::System.Collections.Generic.IDictionary<string, string> formData)
		{
			this.formData = formData;
			return this;
		}

		internal global::Discord.Unity.AsyncRequestString SetQuery(global::UnityEngine.WWWForm query)
		{
			this.query = query;
			return this;
		}

		internal global::Discord.Unity.AsyncRequestString SetCallback(global::Discord.Unity.FacebookDelegate<global::Discord.Unity.IGraphResult> callback)
		{
			this.callback = callback;
			return this;
		}
	}
}
