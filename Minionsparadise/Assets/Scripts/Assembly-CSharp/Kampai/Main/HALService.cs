namespace Kampai.Main
{
	public class HALService : global::Kampai.Main.ILocalizationService
	{
		private const char OPEN_KEY_DELIM = '{';

		private const char CLOSE_KEY_DELIM = '}';

		private const char VERSION_DELIM = '$';

		private const string KEY_NOT_FOUND = "KEY NOT FOUND";

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("HALService") as global::Kampai.Util.IKampaiLogger;

		private global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString> localStringDict;

		private string jsonPath;

		private bool isLanguageSupported = true;

		private string language;

		private string country;

		private global::System.Globalization.CultureInfo m_cultureInfo;

		public global::System.Globalization.CultureInfo CultureInfo
		{
			get
			{
				return m_cultureInfo;
			}
		}

		public void Initialize(string langCode)
		{
			jsonPath = GetResourcePath(langCode);
			if (string.IsNullOrEmpty(jsonPath))
			{
				isLanguageSupported = false;
				jsonPath = "EN-US";
			}
			language = ExtractLanguageFromLocale(jsonPath);
			try
			{
				localStringDict = GetLocalizedDictionary(global::Kampai.Util.Native.GetStreamingTextAsset(string.Format("{0}{1}.json", "Loc_Text_Preinstalled/", jsonPath)));
			}
			catch (global::System.IO.FileNotFoundException ex)
			{
				logger.Error("Error obtaining preinstalled localization file: {0}", ex.ToString());
				localStringDict = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString>();
			}
			catch (global::Newtonsoft.Json.JsonSerializationException ex2)
			{
				logger.Error("Error parsing preinstalled localization file: {0}", ex2.ToString());
				localStringDict = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString>();
			}
			catch (global::Newtonsoft.Json.JsonReaderException ex3)
			{
				logger.Error("Error reading preinstalled localization file: {0}", ex3.ToString());
				localStringDict = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString>();
			}
			foreach (global::System.Collections.Generic.KeyValuePair<string, global::Kampai.Main.ILocalString> item in localStringDict)
			{
				ParseLocalString(item.Value);
			}
			MergeFullLocalizationStrings();
		}

		public bool IsInitialized()
		{
			return localStringDict != null;
		}

		public void Update()
		{
			MergeFullLocalizationStrings();
		}

		private void MergeFullLocalizationStrings()
		{
			string jsonString = string.Empty;
			try
			{
				jsonString = global::Kampai.Util.Native.GetStreamingTextAsset(string.Format("content/dlc/loc_text/{0}.json", jsonPath));
			}
			catch (global::System.IO.FileNotFoundException)
			{
				logger.Warning("Full localization asset not found for {0}, using preinstalled strings only.", jsonPath);
				return;
			}
			if (string.IsNullOrEmpty(jsonString))
			{
				logger.Error("Error obtaining full localization asset: {0}", jsonPath);
				return;
			}
			foreach (global::System.Collections.Generic.KeyValuePair<string, global::Kampai.Main.ILocalString> item in GetLocalizedDictionary(jsonString))
			{
				ParseLocalString(item.Value);
				localStringDict[item.Key] = item.Value;
			}
		}

		public string GetLanguage()
		{
			return language;
		}

		public string GetCountry()
		{
			return country;
		}

		public bool IsLanguageSupported()
		{
			return isLanguageSupported;
		}

		public bool Contains(string key)
		{
			if (key == null || !localStringDict.ContainsKey(key))
			{
				return false;
			}
			return true;
		}

		public string GetString(string key, params object[] args)
		{
			if (key == null)
			{
				string text = string.Format("{0}: {1}", "KEY NOT FOUND", "null key");
				logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, text);
				return text;
			}
			if (!localStringDict.ContainsKey(key))
			{
				string text2 = string.Format("{0}: {1}", "KEY NOT FOUND", key);
				logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, text2);
				return text2;
			}
			global::Kampai.Main.LocalQuantityString localQuantityString = localStringDict[key] as global::Kampai.Main.LocalQuantityString;
			if (localQuantityString != null)
			{
				return localQuantityString.GetStringFormat(args);
			}
			global::Kampai.Main.LocalString localString = localStringDict[key] as global::Kampai.Main.LocalString;
			if (localString != null)
			{
				return localString.GetStringFormat(args);
			}
			string text3 = string.Format("{0}: {1}", "KEY NOT FOUND", key);
			logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, text3);
			return text3;
		}

		public string GetStringUpper(string key, params object[] args)
		{
			return StringToUpper(GetString(key, args));
		}

		public string StringToUpper(string str)
		{
			if (m_cultureInfo != null)
			{
				return str.ToUpper(m_cultureInfo);
			}
			return str.ToUpper();
		}

		public string GetStringLower(string key, params object[] args)
		{
			return StringToLower(GetString(key, args));
		}

		public string StringToLower(string str)
		{
			if (m_cultureInfo != null)
			{
				return str.ToLower(m_cultureInfo);
			}
			return str.ToLower();
		}

		private global::Newtonsoft.Json.JsonConverter GetStringConverter()
		{
			return new global::Kampai.Main.LocalStringConverter();
		}

		private void ParseLocalString(global::Kampai.Main.ILocalString iLocalString)
		{
			global::Kampai.Main.LocalString localString = iLocalString as global::Kampai.Main.LocalString;
			if (localString == null)
			{
				return;
			}
			string text = localString.GetString();
			int num = text.IndexOf('{', 0);
			while (num != -1)
			{
				int num2 = text.IndexOf('}', num);
				string text2 = text.Substring(num + 1, num2 - num - 1);
				num = text.IndexOf('{', num2);
				if (localStringDict.ContainsKey(text2))
				{
					global::Kampai.Main.LocalString localString2 = localStringDict[text2] as global::Kampai.Main.LocalString;
					if (localString2 != null)
					{
						ParseLocalString(localStringDict[text2]);
						string tag = string.Format("{0}{1}{2}", '{', text2, '}');
						string tagValue = localString2.GetString();
						localString.SetKeyValue(tag, tagValue);
					}
				}
			}
		}

		public string GetLanguageKey()
		{
			return jsonPath;
		}

		public static string GetResourcePath(string languageCode)
		{
			languageCode = languageCode.ToLower();
			string text = languageCode;
			if (languageCode.Contains("_"))
			{
				text = languageCode.Split('_')[0];
			}
			else if (languageCode.Contains("-"))
			{
				text = languageCode.Split('-')[0];
			}
			if (text.Equals("en") || text.Equals("english"))
			{
				return "EN-US";
			}
			if (text.Equals("fr") || text.Equals("french"))
			{
				return "FR-FR";
			}
			if (text.Equals("de") || text.Equals("german"))
			{
				return "DE-DE";
			}
			if (text.Equals("es") || text.Equals("spanish"))
			{
				return "ES-ES";
			}
			if (text.Equals("it") || text.Equals("italian"))
			{
				return "IT-IT";
			}
			if (text.Equals("pt") || text.Equals("portuguese"))
			{
				return "PT-BR";
			}
			if (text.Equals("nl") || text.Equals("dutch"))
			{
				return "NL-NL";
			}
			if (text.Equals("ko") || text.Equals("korean"))
			{
				return "KO-KR";
			}
			if (text.Equals("ru") || text.Equals("russian"))
			{
				return "RU-RU";
			}
			if (text.Equals("ja") || text.Equals("japanese"))
			{
				return "JA";
			}
			if (text.Equals("lolcat"))
			{
				return "LOLCAT";
			}
			if (text.Equals("minion"))
			{
				return "MINION";
			}
			if (languageCode.Equals("zh-hans") || languageCode.Equals("zh_hans") || languageCode.Equals("zh_cn") || languageCode.Equals("zh-cn"))
			{
				return "ZH-CN";
			}
			if (languageCode.Equals("zh-hant") || languageCode.Equals("zh_hant") || languageCode.Equals("zh_tw") || languageCode.Equals("zh-tw") || languageCode.Equals("zh_hk") || languageCode.Equals("zh-hk"))
			{
				return "ZH-TW";
			}
			if (text.Equals("zh"))
			{
				return "ZH-CN";
			}
			if (text.Equals("tr"))
			{
				return "TR";
			}
			if (text.Equals("id") || languageCode.Equals("in_id"))
			{
				return "ID";
			}
			return string.Empty;
		}

		public static string ExtractLanguageFromLocale(string locale)
		{
			return ((!locale.Contains("-")) ? locale : locale.Substring(0, locale.IndexOf('-'))).ToLower();
		}

		private global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString> GetLocalizedDictionary(string jsonString)
		{
			global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString> dictionary = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString>();
			global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString> dictionary2 = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString>();
			try
			{
				dictionary = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::System.Collections.Generic.Dictionary<string, global::Kampai.Main.ILocalString>>(jsonString, new global::Newtonsoft.Json.JsonConverter[1] { GetStringConverter() });
			}
			catch (global::System.IO.FileNotFoundException ex)
			{
				logger.Error("Error obtaining preinstalled localization file: {0}", ex.ToString());
			}
			global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>(dictionary.Keys);
			list.Sort();
			global::System.Version version = new global::System.Version(global::Kampai.Util.Native.BundleVersion);
			foreach (string item in list)
			{
				int num = item.LastIndexOf('$');
				string key = item;
				global::System.Version version2 = null;
				if (num > 0 && num < item.Length - 1)
				{
					key = item.Substring(0, num);
					string text = item.Substring(num + 1);
					if (text.IndexOf('.') == -1)
					{
						text += ".0";
					}
					version2 = new global::System.Version(text);
				}
				if (version2 == null || version2 <= version)
				{
					dictionary2[key] = dictionary[item];
				}
			}
			return dictionary2;
		}

		public void RetrieveCultureInfo(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			global::System.Collections.Generic.IDictionary<string, string> headers = response.Headers;
			string headerValue = null;
			if (headers != null && CaseInsensitiveHeaderSearch("X-Kampai-Country", headers, out headerValue))
			{
				logger.Info("New country code from server: {0}", headerValue);
				SetCultureInfo(headerValue);
			}
		}

		private bool CaseInsensitiveHeaderSearch(string headerName, global::System.Collections.Generic.IDictionary<string, string> headers, out string headerValue)
		{
			string text = headerName.ToLower();
			foreach (global::System.Collections.Generic.KeyValuePair<string, string> header in headers)
			{
				if (text == header.Key.ToLower())
				{
					headerValue = header.Value;
					return true;
				}
			}
			headerValue = null;
			return false;
		}

		public void SetCultureInfo(string cultureInfoStr)
		{
			string deviceLanguage = global::Kampai.Util.Native.GetDeviceLanguage();
			try
			{
				country = cultureInfoStr;
				cultureInfoStr = (deviceLanguage.Contains("zh") ? "zh-CN" : ((!string.IsNullOrEmpty(cultureInfoStr) && !deviceLanguage.Contains("_") && !deviceLanguage.Contains("-")) ? string.Format("{0}-{1}", deviceLanguage, cultureInfoStr) : deviceLanguage.Replace('_', '-')));
				m_cultureInfo = global::System.Globalization.CultureInfo.CreateSpecificCulture(cultureInfoStr);
			}
			catch (global::System.ArgumentException)
			{
				try
				{
					deviceLanguage = deviceLanguage.Replace('_', '-');
					m_cultureInfo = global::System.Globalization.CultureInfo.CreateSpecificCulture(deviceLanguage);
				}
				catch (global::System.ArgumentException ex2)
				{
					logger.Error("Could not create Culture Info from {0}, error: {1}", m_cultureInfo, ex2);
					m_cultureInfo = global::System.Globalization.CultureInfo.InvariantCulture;
				}
			}
		}
	}
}
