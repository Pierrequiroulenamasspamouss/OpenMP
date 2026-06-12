using UnityEngine.Networking;

namespace Swrve.Helpers
{
	public class UnityWwwHelper
	{
		public static global::Swrve.Helpers.WwwDeducedError DeduceWebRequestError(UnityWebRequest request)
		{
			if (!string.IsNullOrEmpty(request.error))
			{
				if (request.GetResponseHeaders() != null && request.GetResponseHeaders().Count > 0)
				{
					string value = null;
					foreach (string key in request.GetResponseHeaders().Keys)
					{
						if (string.Equals(key, "X-Swrve-Error", global::System.StringComparison.OrdinalIgnoreCase))
						{
							request.GetResponseHeaders().TryGetValue(key, out value);
							break;
						}
					}
					if (value != null)
					{
						SwrveLog.LogError("Request response headers [\"X-Swrve-Error\"]: " + value + " at " + request.url);
						return global::Swrve.Helpers.WwwDeducedError.ApplicationErrorHeader;
					}
				}

				SwrveLog.LogError("Request error: " + request.error + " in " + request.url);
				return global::Swrve.Helpers.WwwDeducedError.NetworkError;
			}
			return global::Swrve.Helpers.WwwDeducedError.NoError;
		}


	}
}
