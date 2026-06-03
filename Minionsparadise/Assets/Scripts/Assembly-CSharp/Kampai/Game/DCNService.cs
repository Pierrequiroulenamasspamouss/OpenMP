namespace Kampai.Game
{
	public class DCNService : global::Kampai.Game.IDCNService
	{
		public const string DCNPersistenceKey = "DCNStore";

		public const string DCNPersistenceDoNotShowKey = "DCNStoreDoNotShow";

		public const int MaxSeenMemory = 10;

		private const char DELIMITER = ',';

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DCNService") as global::Kampai.Util.IKampaiLogger;

		private global::Kampai.Game.DCNModel dcnModel = new global::Kampai.Game.DCNModel();

		private global::System.Collections.Generic.IList<global::System.Func<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest>> requests = new global::System.Collections.Generic.List<global::System.Func<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest>>();

		[Inject]
		public global::Kampai.Common.ICoppaService coppaService { get; set; }

		[Inject]
		public global::Kampai.Splash.IDownloadService downloadService { get; set; }

		[Inject]
		public global::Kampai.Game.DCNTokenSignal dcnTokenSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DCNEventSignal eventSignal { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistanceService { get; set; }

		public void Perform(global::System.Func<global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest> request, bool isTokenRequest = false)
		{
			if (!coppaService.Restricted())
			{
				if ((dcnModel.Token == null || !dcnModel.Token.IsValid()) && !isTokenRequest)
				{
					requests.Add(request);
					dcnTokenSignal.Dispatch();
					return;
				}
				global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequest request2 = request();
				logger.Info("DCN Request {0}", request2.Uri);
				downloadService.Perform(request2);
			}
		}

		public void SetToken(global::Kampai.Game.DCNToken token)
		{
			logger.Info("DCN Token {0}", token.Token);
			dcnModel.Token = token;
			if (requests.Count > 0)
			{
				Perform(requests[0]);
				requests.RemoveAt(0);
			}
		}

		public string GetToken()
		{
			if (dcnModel == null)
			{
				return string.Empty;
			}
			if (dcnModel.Token == null)
			{
				return string.Empty;
			}
			return dcnModel.Token.Token;
		}

		private global::System.Collections.Generic.List<int> SeenContentIds()
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			string data = localPersistanceService.GetData("DCNStore");
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					try
					{
						int item = global::System.Convert.ToInt32(text);
						list.Add(item);
					}
					catch (global::System.Exception ex)
					{
						logger.Error("DCN Bad Content ID {0} {1}", text, ex.Message);
					}
				}
			}
			return list;
		}

		public bool HasSeenFeaturedContent(int featuredContentId)
		{
			return SeenContentIds().Contains(featuredContentId);
		}

		public void MarkFeaturedContentAsSeen(int featuredContentId)
		{
			global::System.Collections.Generic.List<int> list = SeenContentIds();
			list.Add(featuredContentId);
			while (list.Count > 10)
			{
				list.RemoveAt(0);
			}
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(list[i].ToString());
				if (i < count - 1)
				{
					stringBuilder.Append(',');
				}
			}
			localPersistanceService.PutData("DCNStore", stringBuilder.ToString());
		}

		public bool SetFeaturedContent(int featuredContentId, string htmlUrl)
		{
			logger.Info("DCN featured content ID={0} URL={1}", featuredContentId, htmlUrl);
			if (HasSeenFeaturedContent(featuredContentId))
			{
				logger.Warning("DCN Ignoring seen content {0}", featuredContentId);
				return false;
			}
			dcnModel.FeaturedContentId = featuredContentId;
			dcnModel.FeaturedUrl = htmlUrl;
			return true;
		}

		public int GetFeaturedContentId()
		{
			return dcnModel.FeaturedContentId;
		}

		public void OpenFeaturedContent(bool open)
		{
			if (!HasFeaturedContent())
			{
				logger.Error("Unable to show DCN content [no url]");
				return;
			}
			if (open)
			{
				MarkFeaturedContentAsSeen(dcnModel.FeaturedContentId);
			}
			string launchURL = GetLaunchURL();
			if (open)
			{
				logger.Info("Launching DCN Content {0}", launchURL);
				global::UnityEngine.Application.OpenURL(launchURL);
				eventSignal.Dispatch();
			}
			else
			{
				logger.Info("Declining DCN Content {0}", launchURL);
			}
		}

		public string GetLaunchURL()
		{
			string value = global::System.Uri.EscapeDataString("minions:\\dcn");
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(dcnModel.FeaturedUrl).Append("&token=").Append(GetToken()).Append("&return_name=")
				.Append("Minions")
				.Append("&return_url=")
				.Append(value);
			return stringBuilder.ToString();
		}

		public bool HasFeaturedContent()
		{
			return !string.IsNullOrEmpty(dcnModel.FeaturedUrl);
		}
	}
}
