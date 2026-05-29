public static class UIUtils
{
	private const int BASE_SCREEN_HEIGHT = 640;

	private const int BASE_SCREEN_WIDTH = 960;

	private static float widthScale;

	private static float heightScale;

	public static float GetScreenScale()
	{
		float num = GetWidthScale();
		float num2 = GetHeightScale();
		return (!(num > num2)) ? num : num2;
	}

	public static void ScaleFonts(global::UnityEngine.GameObject gameobject)
	{
		float screenScale = GetScreenScale();
		global::UnityEngine.UI.Text[] componentsInChildren = gameobject.GetComponentsInChildren<global::UnityEngine.UI.Text>();
		global::UnityEngine.UI.Text[] array = componentsInChildren;
		foreach (global::UnityEngine.UI.Text text in array)
		{
			text.fontSize = (int)((float)text.fontSize * screenScale);
		}
	}

	public static void MultiplyFontSize(global::UnityEngine.GameObject gameObject, float multipler)
	{
		global::UnityEngine.UI.Text[] componentsInChildren = gameObject.GetComponentsInChildren<global::UnityEngine.UI.Text>();
		global::UnityEngine.UI.Text[] array = componentsInChildren;
		foreach (global::UnityEngine.UI.Text text in array)
		{
			text.fontSize = (int)((float)text.fontSize * multipler);
		}
	}

	public static float GetWidthScale()
	{
#if UNITY_EDITOR
		return (float)global::UnityEngine.Screen.width / 960f;
#else
		if (widthScale.CompareTo(0f) == 0)
		{
			widthScale = (float)global::UnityEngine.Screen.width / 960f;
		}
		return widthScale;
#endif
	}

	public static float GetHeightScale()
	{
#if UNITY_EDITOR
		return (float)global::UnityEngine.Screen.height / 640f;
#else
		if (heightScale.CompareTo(0f) == 0)
		{
			heightScale = (float)global::UnityEngine.Screen.height / 640f;
		}
		return heightScale;
#endif
	}

	public static int GetReferencedScreenHeight()
	{
		return 640;
	}

	public static int GetReferencedScreenWidth()
	{
		return 960;
	}

	public static string FormatTime(int time, global::Kampai.Main.ILocalizationService localizationService)
	{
		return FormatTime(global::System.Convert.ToDouble(time), localizationService);
	}

	public static string FormatTime(double time, global::Kampai.Main.ILocalizationService localizationService)
	{
		int num = (int)time / 86400;
		int num2 = (int)time / 3600 % 24;
		int num3 = (int)time / 60 % 60;
		int num4 = (int)time % 60;
		if (num > 2)
		{
			return string.Format(localizationService.GetString("TimerDaysLeft"), num);
		}
		if (num > 0)
		{
			if (num2 > 0)
			{
				return string.Format("{0:0}{1} {2:0}{3}", num, localizationService.GetString("TimerDaysAbbreviation"), num2, localizationService.GetString("TimerHoursAbbreviation"));
			}
			return string.Format("{0:0}{1}", num, localizationService.GetString("TimerDaysAbbreviation"));
		}
		if (num2 > 0)
		{
			if (num3 > 0)
			{
				return string.Format("{0:0}{1} {2:0}{3}", num2, localizationService.GetString("TimerHoursAbbreviation"), num3, localizationService.GetString("TimerMinutesAbbreviation"));
			}
			return string.Format("{0:0}{1}", num2, localizationService.GetString("TimerHoursAbbreviation"));
		}
		if (num3 > 0)
		{
			if (num4 > 0)
			{
				return string.Format("{0:0}{1} {2:0}{3}", num3, localizationService.GetString("TimerMinutesAbbreviation"), num4, localizationService.GetString("TimerSecondsAbbreviation"));
			}
			return string.Format("{0:0}{1}", num3, localizationService.GetString("TimerMinutesAbbreviation"));
		}
		return string.Format("{0:0}{1}", num4, localizationService.GetString("TimerSecondsAbbreviation"));
	}

	public static global::Kampai.Util.Tuple<int, int, int, int> DigitalClockValues(float time)
	{
		int time2 = global::UnityEngine.Mathf.FloorToInt(time);
		return DigitalClockValues(time2);
	}

	public static global::Kampai.Util.Tuple<int, int, int, int> DigitalClockValues(int time)
	{
		int num = time / 3600 % 24;
		int num2 = time / 60 % 60;
		int num3 = time % 60;
		global::Kampai.Util.Tuple<int, int, int, int> tuple = new global::Kampai.Util.Tuple<int, int, int, int>(0, 0, 0, 0);
		if (num > 0)
		{
			tuple.Item1 = num / 10;
			tuple.Item2 = num % 10;
			tuple.Item3 = num2 / 10;
			tuple.Item4 = num2 % 10;
			return tuple;
		}
		tuple.Item1 = num2 / 10;
		tuple.Item2 = num2 % 10;
		tuple.Item3 = num3 / 10;
		tuple.Item4 = num3 % 10;
		return tuple;
	}

	public static string FormatSocialTime(double time)
	{
		int num = (int)time / 3600;
		int num2 = (int)time / 60 % 60;
		int num3 = (int)time % 60;
		return string.Format("{0:0}:{1:0}:{2:0}", num, num2, num3);
	}

	public static string FormatDate(int timestamp, global::Kampai.Main.ILocalizationService localizationService)
	{
		return global::Kampai.Util.GameConstants.Timers.epochStart.AddSeconds(timestamp).ToString("d", localizationService.CultureInfo);
	}

	public static global::System.Collections.Generic.List<string> BreakDialog(string targetText, int pieceCount, char[] languageDelimiters, global::Kampai.Util.IKampaiLogger logger)
	{
		global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
		global::System.Collections.Generic.List<string> list2 = new global::System.Collections.Generic.List<string>();
		if (string.IsNullOrEmpty(targetText))
		{
			return list2;
		}
		list.Add(0);
		int startIndex = 0;
		for (int i = 1; i < pieceCount; i++)
		{
			int num = targetText.Length / pieceCount * i;
			int item = FindCloestIndexOfWordBreaker(targetText, startIndex, num, languageDelimiters, logger);
			list.Add(item);
			startIndex = num;
		}
		list.Add(targetText.Length);
		for (int j = 0; j < list.Count - 1; j++)
		{
			string item2 = targetText.Substring(list[j] + ((j != 0) ? 1 : 0), list[j + 1] - list[j] + ((j != list.Count - 2) ? 1 : (-1)));
			list2.Add(item2);
		}
		return list2;
	}

	private static int FindCloestIndexOfWordBreaker(string targetText, int startIndex, int endIndex, char[] languageDelimiters, global::Kampai.Util.IKampaiLogger logger)
	{
		int num = -1;
		int balanceIndex = -1;
		bool closeTagIsInFront = false;
		global::System.Collections.Generic.List<int> leftBracketList = null;
		int num2 = HTMLTagValidator(targetText, startIndex, endIndex, out balanceIndex, out closeTagIsInFront, out leftBracketList, logger);
		if (num2 == 0)
		{
			if (closeTagIsInFront)
			{
				num = ((balanceIndex != -1) ? balanceIndex : targetText.LastIndexOf('<', endIndex, endIndex - startIndex));
			}
			else
			{
				num = targetText.LastIndexOfAny(languageDelimiters, endIndex, endIndex - startIndex);
				if (balanceIndex != -1 && num < balanceIndex)
				{
					num = balanceIndex;
				}
			}
		}
		else
		{
			if (leftBracketList == null || leftBracketList.Count == 0)
			{
				return endIndex;
			}
			bool flag = false;
			if (num2 > 0)
			{
				for (int num3 = leftBracketList.Count - 1; num3 >= 0; num3--)
				{
					if (!flag && leftBracketList[num3] >= 0)
					{
						flag = true;
					}
					else if (flag && leftBracketList[num3] <= 0)
					{
						num = leftBracketList[num3 + 1] - 1;
						break;
					}
				}
				if (flag && num == -1)
				{
					num = leftBracketList[0] - 1;
				}
			}
			else
			{
				int leftAngleBracketIndex = 0;
				for (int i = 0; i < leftBracketList.Count; i++)
				{
					if (!flag && leftBracketList[i] <= 0)
					{
						flag = true;
					}
					else if (flag && leftBracketList[i] >= 0)
					{
						leftAngleBracketIndex = leftBracketList[i + 1];
						break;
					}
				}
				if (flag && num == -1)
				{
					leftAngleBracketIndex = leftBracketList[leftBracketList.Count - 1];
				}
				num = FindRightAngleBracket(leftAngleBracketIndex, targetText, logger);
			}
		}
		if (num == -1)
		{
			return endIndex;
		}
		return num;
	}

	public static int HTMLTagValidator(string targetString, int startIndex, int endIndex, out int balanceIndex, out bool closeTagIsInFront, out global::System.Collections.Generic.List<int> leftBracketList, global::Kampai.Util.IKampaiLogger logger)
	{
		balanceIndex = -1;
		closeTagIsInFront = false;
		leftBracketList = new global::System.Collections.Generic.List<int>();
		if (endIndex <= startIndex)
		{
			return 0;
		}
		int num = 0;
		bool flag = false;
		for (int i = startIndex; i <= endIndex; i++)
		{
			if (targetString[i] != '<')
			{
				continue;
			}
			if (targetString[i + 1] == '/')
			{
				num--;
				leftBracketList.Add(-i);
				if (!flag)
				{
					closeTagIsInFront = true;
				}
			}
			else
			{
				leftBracketList.Add(i);
				num++;
			}
			if (num == 0 || (closeTagIsInFront && flag && num == -1))
			{
				balanceIndex = FindRightAngleBracket(i, targetString, logger);
			}
			flag = true;
		}
		return num;
	}

	private static int FindRightAngleBracket(int leftAngleBracketIndex, string text, global::Kampai.Util.IKampaiLogger logger)
	{
		int result = -1;
		try
		{
			for (int i = leftAngleBracketIndex; i < text.Length; i++)
			{
				if (text[i] == '>')
				{
					result = ((i == text.Length - 1) ? i : ((text[i + 1] != '<') ? (i + 1) : i));
					break;
				}
			}
		}
		catch (global::System.IndexOutOfRangeException ex)
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, "UIUtils FindRightAngleBracket failed. LeftAngleBracketIndex: {0}. Text: {1}. With exception:{3}", leftAngleBracketIndex.ToString(), text, ex.StackTrace);
		}
		return result;
	}

	public static void FlashingColor(global::UnityEngine.UI.Text text, int index)
	{
		text.color = global::Kampai.Util.GameConstants.UI.UI_BOOSTCOLOR;
		index = 0;
	}

	public static global::UnityEngine.Sprite LoadSpriteFromPath(string path, string defaultImage = "btn_Circle01_mask")
	{
		global::UnityEngine.Sprite sprite = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Sprite>(path);
		if (sprite == null)
		{
			sprite = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Sprite>(defaultImage);
		}
		return sprite;
	}

	public static void SetItemIcon(global::Kampai.UI.View.KampaiImage itemImage, global::Kampai.Game.ItemDefinition itemDefinitions)
	{
		if (itemDefinitions != null)
		{
			SetItemIcon(itemImage, (global::Kampai.Game.IDisplayableDefinition)itemDefinitions);
		}
	}

	public static void SetItemIcon(global::Kampai.UI.View.KampaiImage itemImage, global::Kampai.Game.IDisplayableDefinition itemDefinitions)
	{
		if (!(itemImage == null))
		{
			object displayableDefinition = itemDefinitions;
			if (displayableDefinition == null)
			{
				global::Kampai.Game.DisplayableDefinition displayableDefinition2 = new global::Kampai.Game.DisplayableDefinition();
				displayableDefinition2.Image = "btn_Main01_fill";
				displayableDefinition2.Mask = "btn_Main01_mask";
				displayableDefinition = displayableDefinition2;
			}
			itemDefinitions = (global::Kampai.Game.IDisplayableDefinition)displayableDefinition;
			string text = itemDefinitions.Image;
			if (string.IsNullOrEmpty(text))
			{
				text = "btn_Main01_fill";
			}
			global::UnityEngine.Sprite sprite = LoadSpriteFromPath(text);
			itemImage.sprite = sprite;
			string text2 = itemDefinitions.Mask;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "btn_Main01_mask";
			}
			global::UnityEngine.Sprite maskSprite = LoadSpriteFromPath(text2);
			itemImage.maskSprite = maskSprite;
		}
	}

	public static string FormatLargeNumber(int number)
	{
		if (number == 0)
		{
			return number.ToString();
		}
		global::System.Globalization.NumberFormatInfo numberFormatInfo = (global::System.Globalization.NumberFormatInfo)global::System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
		numberFormatInfo.NumberGroupSeparator = " ";
		return number.ToString("#,#", numberFormatInfo);
	}

	public static int GetIntFromFormattedLargeNumber(string stringNumber)
	{
		string s = global::System.Text.RegularExpressions.Regex.Replace(stringNumber, "\\s+", string.Empty);
		int result;
		int.TryParse(s, out result);
		return result;
	}

	public static void SafeDestoryViews<T>(global::System.Collections.Generic.IList<T> views) where T : global::UnityEngine.MonoBehaviour
	{
		if (views == null || views.Count == 0)
		{
			return;
		}
		T val = (T)null;
		for (int i = 0; i < views.Count; i++)
		{
			val = views[i];
			if (!(val == null) && !(val.gameObject == null))
			{
				val.gameObject.SetActive(false);
				global::UnityEngine.Object.Destroy(val.gameObject);
			}
		}
		views.Clear();
	}

	public static void SafeDestoryViews<T>(T[] views) where T : global::UnityEngine.MonoBehaviour
	{
		if (views == null || views.Length == 0)
		{
			return;
		}
		T val = (T)null;
		for (int i = 0; i < views.Length; i++)
		{
			val = views[i];
			if (!(val == null) && !(val.gameObject == null))
			{
				val.gameObject.SetActive(false);
				global::UnityEngine.Object.Destroy(val.gameObject);
			}
		}
	}
}
