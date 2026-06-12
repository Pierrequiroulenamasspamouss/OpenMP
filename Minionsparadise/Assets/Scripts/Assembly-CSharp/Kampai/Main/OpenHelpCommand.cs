namespace Kampai.Main
{
	public class OpenHelpCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("OpenHelpCommand") as global::Kampai.Util.IKampaiLogger;

		private string platform;

		[Inject]
		public global::Kampai.Main.HelpType helpType { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService loc { get; set; }

		[Inject]
		public ILocalPersistanceService LocalPersistService { get; set; }

		[Inject]
		public IEncryptionService encryptionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerDurationService playerDurationService { get; set; }

		[Inject]
		public global::Kampai.Util.IClientVersion clientVersion { get; set; }

		[Inject]
		public global::Kampai.Common.ICoppaService coppaService { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		public override void Execute()
		{
			string text = BuildUrl();
			logger.Info(text);
			userSessionService.OpenURL(text);
		}

		private string GetWWCELocaleCode()
		{
			string result = loc.GetLanguage();
			if (loc.GetLanguage().Equals("pt"))
			{
				result = "br";
			}
			return result;
		}

		public string BuildUrl()
		{
			string helpPlatform = getHelpPlatform();
			if (helpType == global::Kampai.Main.HelpType.ONLINE_HELP)
			{
				global::System.Collections.Generic.Dictionary<string, string> value = preparePlayLoad();
				string text = global::Newtonsoft.Json.JsonConvert.SerializeObject(value);
				logger.Info("Tesla tptk payload :" + text);
				string text2 = global::System.Uri.EscapeDataString(TeslaActivate.Encrypt(text, global::Kampai.Util.GameConstants.StaticConfig.WWCE_SECRET));
				return string.Format(global::Kampai.Util.GameConstants.StaticConfig.WWCE_URL, GetWWCELocaleCode(), global::Kampai.Util.GameConstants.StaticConfig.WWCE_GAME_NAME, global::Kampai.Util.GameConstants.StaticConfig.WWCE_GAME_NAME, helpPlatform, global::Kampai.Util.GameConstants.StaticConfig.WWCE_GAME_NAME, text2);
			}
			return string.Format(global::Kampai.Util.GameConstants.StaticConfig.WWCE_CONTACTUS_URL, GetWWCELocaleCode(), global::Kampai.Util.GameConstants.StaticConfig.WWCE_GAME_NAME, global::Kampai.Util.GameConstants.StaticConfig.WWCE_GAME_NAME, helpPlatform);
		}

		public string getHelpPlatform()
		{
			if (platform == null)
			{
				platform = ((!global::Kampai.Util.DeviceCapabilities.IsTablet()) ? "android-phone" : "android-tablet");
			}
			return platform;
		}

		private string getDefinitionVariants()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.Append(configurationsService.GetDefinitionVariants()).Replace("_", ",");
			return stringBuilder.ToString();
		}

		private global::System.Collections.Generic.Dictionary<string, string> preparePlayLoad()
		{
			string plainText = LocalPersistService.GetData("AnonymousID");
			encryptionService.TryDecrypt(plainText, "Kampai!", out plainText);
			global::System.Collections.Generic.Dictionary<string, string> dictionary = new global::System.Collections.Generic.Dictionary<string, string>();
			dictionary.Add("platform", getHelpPlatform());
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			dictionary.Add("internal", LocalPersistService.GetData("UserID"));
			dictionary.Add("anonymous", plainText);
			dictionary.Add("synergy", userSession.SynergyID);
			dictionary.Add("Level", playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID).ToString());
			dictionary.Add("Grind Currency", playerService.GetQuantity(global::Kampai.Game.StaticItem.GRIND_CURRENCY_ID).ToString());
			dictionary.Add("Preminum Currency", playerService.GetQuantity(global::Kampai.Game.StaticItem.PREMIUM_CURRENCY_ID).ToString());
			dictionary.Add("Game Play Duration", playerDurationService.GameplaySecondsSinceLevelUp.ToString());
			dictionary.Add("Build", clientVersion.GetClientVersion());
			dictionary.Add("Age", coppaService.GetAge().ToString());
			dictionary.Add("Definition Variants", getDefinitionVariants());
			dictionary.Add("spend", playerService.GetQuantity(global::Kampai.Game.StaticItem.TRANSACTIONS_LIFETIME_COUNT_ID).ToString());
			AddSocialIdentitiesToPayload(dictionary);
			return dictionary;
		}

		private void AddSocialIdentitiesToPayload(global::System.Collections.Generic.Dictionary<string, string> payload)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			global::System.Collections.Generic.IList<global::Kampai.Game.UserIdentity> socialIdentities = userSession.SocialIdentities;
			if (socialIdentities == null)
			{
				return;
			}
			foreach (global::Kampai.Game.UserIdentity socialIdentity in userSession.SocialIdentities)
			{
				string externalID = socialIdentity.ExternalID;
				if (!string.IsNullOrEmpty(externalID))
				{
					payload[socialIdentity.Type.ToString()] = externalID;
				}
			}
		}
	}
}
