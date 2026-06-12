namespace Kampai.Splash
{
	public class DownloadService : global::Kampai.Splash.IDownloadService
	{
		public const int MAX_CONCURRENT_REQUESTS = 5;

		private global::System.Collections.Generic.Queue<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest> requestQueue;

		private global::System.Collections.Generic.LinkedList<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest> runningRequests;

		private bool logInfo;

		private global::System.Collections.Generic.LinkedList<global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>> globalResponseSignals;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DownloadService") as global::Kampai.Util.IKampaiLogger;

		private global::Kampai.Game.TimeService timeService;

		private string deviceTypeUrlEscaped;

		[Inject]
		public global::Kampai.Util.IInvokerService invoker { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkModel networkModel { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkConnectionLostSignal networkConnectionLostSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ResumeNetworkOperationSignal resumeNetworkOperationSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IClientVersion clientVersion { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }

		[Inject("game.server.host")]
		public string ServerUrl { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeServiceInstance { get; set; }

		public bool IsRunning { get; set; }

		private int requestCounter { get; set; }

		public DownloadService()
		{
			global::System.Net.ServicePointManager.DefaultConnectionLimit = 5;
			IsRunning = true;
			requestQueue = new global::System.Collections.Generic.Queue<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest>();
			runningRequests = new global::System.Collections.Generic.LinkedList<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest>();
			globalResponseSignals = new global::System.Collections.Generic.LinkedList<global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>>();
			global::Ea.Sharkbite.HttpPlugin.Http.Api.ConnectionSettings.ConnectionLimit = 5;
		}

		[PostConstruct]
		public void PostConstruct()
		{
			deviceTypeUrlEscaped = global::System.Uri.EscapeDataString(clientVersion.GetClientDeviceType());
			timeService = timeServiceInstance as global::Kampai.Game.TimeService;
			resumeNetworkOperationSignal.AddListener(ProcessQueue);
		}

		public void Perform(global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request, bool forceRequest = false)
		{
			if (forceRequest)
			{
				DoPerform(request.WithRunInBackground(true));
				return;
			}
			invoker.Add(delegate
			{
				DoPerform(request);
			});
		}

		private void DoPerform(global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request)
		{
			logInfo = logger.IsAllowedLevel(global::Kampai.Util.KampaiLogLevel.Info);
			if (request.ProgressSignal != null)
			{
				if (!string.IsNullOrEmpty(request.FilePath))
				{
					request.RegisterNotifiable(ProgressCallback);
				}
				else
				{
					logger.Warning("Unable to notify if request is not notifiable");
				}
			}
			requestQueue.Enqueue(request.WithHeaderParam("K-Platform", clientVersion.GetClientPlatform()).WithHeaderParam("K-Device", deviceTypeUrlEscaped).WithHeaderParam("K-Version", clientVersion.GetClientVersion()));
			if (!request.IsRestarted())
			{
				ProcessQueue();
			}
		}

		public void AddGlobalResponseListener(global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal)
		{
			globalResponseSignals.AddLast(signal);
		}

		public void ProcessQueue()
		{
			if (!IsRunning || requestQueue.Count <= 0)
			{
				return;
			}
			if (!networkModel.isConnectionLost)
			{
				if (global::Kampai.Util.NetworkUtil.IsConnected())
				{
					if (runningRequests.Count < 5)
					{
						DoDownload(requestQueue.Dequeue());
					}
				}
				else
				{
					NetworkLost();
				}
				return;
			}
			global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request = null;
			int count = requestQueue.Count;
			while (count-- != 0)
			{
				global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request2 = requestQueue.Dequeue();
				if (request == null && request2.IsAborted() && !request2.IsRestarted())
				{
					request = request2;
				}
				else
				{
					requestQueue.Enqueue(request2);
				}
			}
			if (request != null)
			{
				DoDownload(request);
			}
		}

		private void NetworkLost()
		{
			if (!networkModel.isConnectionLost)
			{
				networkConnectionLostSignal.Dispatch();
			}
		}

		private void RetryDownload(global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request)
		{
			if (IsRunning)
			{
				logger.Warning("Failed to download {0}, {1} retries left, trying again...", request.Uri, request.RetryCount);
				request.RetryCount--;
				if (IsRunning && runningRequests.Count < 5)
				{
					DoDownload(request);
					return;
				}
				requestQueue.Enqueue(request);
				ProcessQueue();
			}
		}

		private void DoDownload(global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request)
		{
			int requestCount = request.requestCount;
			if (requestCount > 0)
			{
				if (requestCount < requestCounter)
				{
					logger.Warning("HTTP START ABORTED [Attempting to save old save:{0} over new save:{1}] {2}: {3}", requestCount.ToString(), requestCounter.ToString(), request.Method, request.Uri);
					return;
				}
				logger.Debug("Save Counter {0}", requestCount.ToString());
				requestCounter = requestCount;
			}
			else
			{
				logger.Debug("Save Counter Untracked");
			}
			runningRequests.AddLast(request);
			if (logInfo)
			{
				logger.Info("HTTP START [{0}] {1}: {2}", runningRequests.Count, request.Method, request.Uri);
			}
			request.Execute(RequestCallbackProxy);
		}

		private void ProgressCallbackProxy(global::Ea.Sharkbite.HttpPlugin.Http.Api.DownloadProgress progress, global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request)
		{
			invoker.Add(delegate
			{
				ProgressCallback(progress, request);
			});
		}

		private void ProgressCallback(global::Ea.Sharkbite.HttpPlugin.Http.Api.DownloadProgress progress, global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request)
		{
			if (!IsRunning)
			{
				global::Kampai.Util.Native.LogError("Ignoring HTTP progress (shutting down)");
			}
			else
			{
				NotifyProgress(progress, request);
			}
		}

		private void RequestCallbackProxy(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			invoker.Add(delegate
			{
				RequestCallback(response);
			});
		}

		private void RequestCallback(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			if (!IsRunning)
			{
				logger.Error("Ignoring HTTP response (shutting down)");
				return;
			}
			bool flag = false;
			string text = "unknown";
			bool flag2 = false;
			if (response != null)
			{
				flag = response.Success;
				if (response.IsConnectionLost)
				{
					NetworkLost();
				}
				global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request = response.Request;
				if (request != null)
				{
					runningRequests.Remove(request);
					text = request.Uri;
					flag2 = request.IsAborted();
					if (!flag)
					{
						logger.Warning("Error downloading {0} HTTP RESPONSE => {1} Error => {2}", text, response.Code, response.Error);
						if ((networkModel.isConnectionLost && !flag2) || request.IsRestarted())
						{
							requestQueue.Enqueue(request);
							return;
						}
						if (request.CanRetry && request.RetryCount > 0 && !flag2)
						{
							RetryDownload(request);
							return;
						}
					}
					else
					{
						if (text.Contains(ServerUrl))
						{
							timeService.SyncServerTime(response);
							localizationService.RetrieveCultureInfo(response);
						}
						if (logInfo)
						{
							logger.Info("Successfully downloaded " + text);
						}
					}
				}
				else
				{
					logger.Error("Null request on response");
				}
			}
			else
			{
				logger.Error("Null response");
				response = new global::Ea.Sharkbite.HttpPlugin.Http.Impl.DefaultResponse().WithCode(500).WithBody("Null response");
			}
			if (flag || !networkModel.isConnectionLost || flag2)
			{
				NotifyResponse(response);
			}
			if (logInfo)
			{
				logger.Info("HTTP END [" + runningRequests.Count + "] " + text);
			}
			ProcessQueue();
		}

		private void NotifyProgress(global::Ea.Sharkbite.HttpPlugin.Http.Api.DownloadProgress progress, global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request)
		{
			if (IsRunning)
			{
				global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.DownloadProgress, global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest> progressSignal = request.ProgressSignal;
				if (progressSignal != null)
				{
					progressSignal.Dispatch(progress, request);
				}
			}
		}

		private void NotifyResponse(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			if (!IsRunning)
			{
				return;
			}
			global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request = response.Request;
			if (request != null)
			{
				global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> responseSignal = request.ResponseSignal;
				if (responseSignal != null)
				{
					responseSignal.Dispatch(response);
				}
			}
			else
			{
				logger.Error("Null request on response");
			}
			foreach (global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> globalResponseSignal in globalResponseSignals)
			{
				globalResponseSignal.Dispatch(response);
			}
		}

		public void Shutdown()
		{
			IsRunning = false;
			ClearQueueAndAbortRunning();
			runningRequests.Clear();
			resumeNetworkOperationSignal.RemoveListener(ProcessQueue);
		}

		public void Abort()
		{
			foreach (global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest item in requestQueue)
			{
				item.Abort();
				NotifyResponse(new global::Ea.Sharkbite.HttpPlugin.Http.Impl.DefaultResponse().WithCode(500).WithRequest(item));
			}
			ClearQueueAndAbortRunning();
		}

		private void ClearQueueAndAbortRunning()
		{
			requestQueue.Clear();
			foreach (global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest runningRequest in runningRequests)
			{
				runningRequest.Abort();
			}
		}

		public void Restart()
		{
			foreach (global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest runningRequest in runningRequests)
			{
				if (!runningRequest.IsAborted())
				{
					runningRequest.Restart();
				}
			}
		}
	}
}
