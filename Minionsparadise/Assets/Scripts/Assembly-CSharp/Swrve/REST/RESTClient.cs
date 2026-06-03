namespace Swrve.REST
{
	public class RESTClient : global::Swrve.REST.IRESTClient
	{
		private global::System.Collections.Generic.List<string> metrics = new global::System.Collections.Generic.List<string>();

		public virtual global::System.Collections.IEnumerator Get(string url, global::System.Action<global::Swrve.REST.RESTResponse> listener)
		{
			global::System.Collections.Generic.Dictionary<string, string> headers = new global::System.Collections.Generic.Dictionary<string, string>();
			if (!global::UnityEngine.Application.isEditor)
			{
				headers = AddMetricsHeader(headers);
				headers.Add("Accept-Encoding", "gzip");
			}
			long start = global::Swrve.Helpers.SwrveHelper.GetMilliseconds();
			using (global::UnityEngine.Networking.UnityWebRequest webRequest = global::Swrve.CrossPlatformUtils.MakeWebRequest(url, null, headers))
			{
				yield return webRequest.SendWebRequest();
				long wwwTime = global::Swrve.Helpers.SwrveHelper.GetMilliseconds() - start;
				ProcessResponse(webRequest, wwwTime, url, listener);
			}
		}

		public virtual global::System.Collections.IEnumerator Post(string url, byte[] encodedData, global::System.Collections.Generic.Dictionary<string, string> headers, global::System.Action<global::Swrve.REST.RESTResponse> listener)
		{
			if (!global::UnityEngine.Application.isEditor)
			{
				headers = AddMetricsHeader(headers);
			}
			long start = global::Swrve.Helpers.SwrveHelper.GetMilliseconds();
			using (global::UnityEngine.Networking.UnityWebRequest webRequest = global::Swrve.CrossPlatformUtils.MakeWebRequest(url, encodedData, headers))
			{
				yield return webRequest.SendWebRequest();
				long wwwTime = global::Swrve.Helpers.SwrveHelper.GetMilliseconds() - start;
				ProcessResponse(webRequest, wwwTime, url, listener);
			}
		}

		protected global::System.Collections.Generic.Dictionary<string, string> AddMetricsHeader(global::System.Collections.Generic.Dictionary<string, string> headers)
		{
			if (metrics.Count > 0)
			{
				string value = string.Join(";", metrics.ToArray());
				headers.Add("Swrve-Latency-Metrics", value);
				metrics.Clear();
			}
			return headers;
		}

		private void AddMetrics(string url, long wwwTime, bool error)
		{
			global::System.Uri uri = new global::System.Uri(url);
			url = string.Format("{0}{1}{2}", uri.Scheme, global::System.Uri.SchemeDelimiter, uri.Authority);
			string item = ((!error) ? string.Format("u={0},c={1},sh={1},sb={1},rh={1},rb={1}", url, wwwTime.ToString()) : string.Format("u={0},c={1},c_error=1", url, wwwTime.ToString()));
			metrics.Add(item);
		}

		protected void ProcessResponse(global::UnityEngine.Networking.UnityWebRequest webRequest, long wwwTime, string url, global::System.Action<global::Swrve.REST.RESTResponse> listener)
		{
			try
			{
				global::Swrve.Helpers.WwwDeducedError webRequestError = global::Swrve.Helpers.UnityWwwHelper.DeduceWebRequestError(webRequest);
				if (webRequestError == global::Swrve.Helpers.WwwDeducedError.NoError)
				{
					byte[] responseData = webRequest.downloadHandler.data;
					string decodedString = null;
					bool flag = global::Swrve.Helpers.ResponseBodyTester.TestUTF8(responseData, out decodedString);
					global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
					string value = null;
					var responseHeaders = webRequest.GetResponseHeaders();
					if (responseHeaders != null)
					{
						foreach (string key in responseHeaders.Keys)
						{
							if (string.Equals(key, "Content-Encoding", global::System.StringComparison.OrdinalIgnoreCase))
							{
								responseHeaders.TryGetValue(key, out value);
								break;
							}
							dictionary.Add(key.ToUpper(), responseHeaders[key]);
						}
					}
					if (responseData != null && responseData.Length > 4 && value != null && string.Equals(value, "gzip", global::System.StringComparison.OrdinalIgnoreCase) && decodedString != null && (!decodedString.StartsWith("{") || !decodedString.EndsWith("}")) && (!decodedString.StartsWith("[") || !decodedString.EndsWith("]")))
					{
						int num = global::System.BitConverter.ToInt32(responseData, 0);
						if (num > 0)
						{
							byte[] array = new byte[num];
							using (global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream(responseData))
							{
								using (global::ICSharpCode.SharpZipLib.GZip.GZipInputStream gZipInputStream = new global::ICSharpCode.SharpZipLib.GZip.GZipInputStream(memoryStream))
								{
									gZipInputStream.Read(array, 0, array.Length);
									gZipInputStream.Close();
								}
								flag = global::Swrve.Helpers.ResponseBodyTester.TestUTF8(array, out decodedString);
								memoryStream.Close();
							}
						}
					}
					if (flag)
					{
						AddMetrics(url, wwwTime, false);
						listener(new global::Swrve.REST.RESTResponse(decodedString, dictionary));
					}
					else
					{
						AddMetrics(url, wwwTime, true);
						listener(new global::Swrve.REST.RESTResponse(global::Swrve.Helpers.WwwDeducedError.ApplicationErrorBody));
					}
				}
				else
				{
					AddMetrics(url, wwwTime, true);
					listener(new global::Swrve.REST.RESTResponse(webRequestError));
				}
			}
			catch (global::System.Exception message)
			{
				SwrveLog.LogError(message);
			}
		}
	}
}
