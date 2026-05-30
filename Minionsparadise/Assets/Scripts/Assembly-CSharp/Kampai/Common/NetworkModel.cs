namespace Kampai.Common
{
	public class NetworkModel
	{
		public bool isConnectionLost { get; set; }

		public global::UnityEngine.NetworkReachability reachability { get; set; }

		public bool isConnected
		{
			get
			{
				return reachability != global::UnityEngine.NetworkReachability.NotReachable;
			}
		}
	}
}
