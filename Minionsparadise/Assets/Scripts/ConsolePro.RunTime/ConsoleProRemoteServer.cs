public class ConsoleProRemoteServer : global::UnityEngine.MonoBehaviour
{
	public class HTTPContext
	{
		public global::System.Net.HttpListenerContext context;

		public string path;

		public string Command
		{
			get
			{
				return global::UnityEngine.Networking.UnityWebRequest.UnEscapeURL(context.Request.Url.AbsolutePath);
			}
		}

		public global::System.Net.HttpListenerRequest Request
		{
			get
			{
				return context.Request;
			}
		}

		public global::System.Net.HttpListenerResponse Response
		{
			get
			{
				return context.Response;
			}
		}

		public HTTPContext(global::System.Net.HttpListenerContext inContext)
		{
			context = inContext;
		}

		public void RespondWithString(string inString)
		{
			Response.StatusDescription = "OK";
			Response.StatusCode = 200;
			if (!string.IsNullOrEmpty(inString))
			{
				Response.ContentType = "text/plain";
				byte[] bytes = global::System.Text.Encoding.UTF8.GetBytes(inString);
				Response.ContentLength64 = bytes.Length;
				Response.OutputStream.Write(bytes, 0, bytes.Length);
			}
		}
	}

	public class QueuedLog
	{
		public string message;

		public string stackTrace;

		public global::UnityEngine.LogType type;
	}

	public int port = 51000;

	private static global::System.Threading.Thread mainThread;

	private static global::System.Net.HttpListener listener = new global::System.Net.HttpListener();

	private static global::System.Collections.Generic.List<ConsoleProRemoteServer.QueuedLog> logs = new global::System.Collections.Generic.List<ConsoleProRemoteServer.QueuedLog>();

	private void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		mainThread = global::System.Threading.Thread.CurrentThread;
		global::UnityEngine.Debug.Log("Starting Console Pro Server on port : " + port);
		listener.Prefixes.Add("http://*:" + port + "/");
		listener.Start();
		listener.BeginGetContext(ListenerCallback, null);
	}

	private void OnEnable()
	{
		global::UnityEngine.Application.logMessageReceived += LogCallback;
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
	}

	private void OnDisable()
	{
		global::UnityEngine.Application.logMessageReceived -= LogCallback;
	}

	public static void LogCallback(string logString, string stackTrace, global::UnityEngine.LogType type)
	{
		if (!logString.StartsWith("CPIGNORE"))
		{
			QueueLog(logString, stackTrace, type);
		}
	}

	private static void QueueLog(string logString, string stackTrace, global::UnityEngine.LogType type)
	{
		logs.Add(new ConsoleProRemoteServer.QueuedLog
		{
			message = logString,
			stackTrace = stackTrace,
			type = type
		});
	}

	private void ListenerCallback(global::System.IAsyncResult result)
	{
		ConsoleProRemoteServer.HTTPContext context = new ConsoleProRemoteServer.HTTPContext(listener.EndGetContext(result));
		HandleRequest(context);
		listener.BeginGetContext(ListenerCallback, null);
	}

	private void HandleRequest(ConsoleProRemoteServer.HTTPContext context)
	{
		bool flag = false;
		switch (context.Command)
		{
		case "/NewLogs":
		{
			flag = true;
			if (logs.Count <= 0)
			{
				break;
			}
			string text = "";
			foreach (ConsoleProRemoteServer.QueuedLog log in logs)
			{
				text = text + "::::" + log.type;
				text = text + "||||" + log.message;
				text = text + ">>>>" + log.stackTrace + ">>>>";
			}
			context.RespondWithString(text);
			logs.Clear();
			break;
		}
		}
		if (!flag)
		{
			context.Response.StatusCode = 404;
			context.Response.StatusDescription = "Not Found";
		}
		context.Response.OutputStream.Close();
	}
}
