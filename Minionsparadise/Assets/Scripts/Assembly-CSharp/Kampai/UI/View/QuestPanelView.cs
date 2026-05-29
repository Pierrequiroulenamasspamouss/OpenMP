namespace Kampai.UI.View
{
	public class QuestPanelView : global::Kampai.UI.View.PopupMenuView
	{
		public global::UnityEngine.RectTransform taskScrollViewTransform;

		public global::UnityEngine.RectTransform questTabScrollViewTransform;

		public global::UnityEngine.RectTransform rewardsPopupTransform;

		public global::UnityEngine.UI.Text questName;

		public global::Kampai.UI.View.QuestPanelProgressBarView questPanelProgressBar;

		public global::Kampai.UI.View.QuestLineProgressBarView questLineProgressBar;

		public global::Kampai.UI.View.CurrentQuestView currentQuestView;

		public global::UnityEngine.RectTransform currencyRewardList;

		public global::UnityEngine.RectTransform itemRewardList;

		public global::Kampai.UI.View.QuestPopupButtonView RewardItemButton;

		internal global::Kampai.Game.IQuestService questService;

		internal global::Kampai.Main.ILocalizationService localizationService;

		internal global::Kampai.Game.ITimeService timeService;

		internal global::Kampai.UI.View.ModalSettings modalSettings;

		private float taskPanelWidth;

		private float normalHeight;

		private float questBookPadding;

		private global::UnityEngine.GameObject taskPanelPrefab;

		private global::System.Collections.Generic.IList<global::UnityEngine.GameObject> questStepViews = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

		private global::System.Collections.Generic.IList<global::Kampai.UI.View.QuestView> questTabs = new global::System.Collections.Generic.List<global::Kampai.UI.View.QuestView>();

		internal void CreateQuestSteps(global::Kampai.Game.Quest quest, global::Kampai.Game.Transaction.TransactionDefinition rewardTransaction, global::Kampai.Game.IDefinitionService definitionService)
		{
			foreach (global::UnityEngine.GameObject questStepView in questStepViews)
			{
				global::UnityEngine.Object.Destroy(questStepView);
			}
			questStepViews.Clear();
			SetupQuestPanelInfo(quest.GetActiveDefinition(), rewardTransaction, definitionService);
			if (quest.GetActiveDefinition().SurfaceType == global::Kampai.Game.QuestSurfaceType.LimitedEvent || quest.GetActiveDefinition().SurfaceType == global::Kampai.Game.QuestSurfaceType.TimedEvent)
			{
				questPanelProgressBar.gameObject.SetActive(true);
				SetupQuestProgressBar(quest);
			}
			else
			{
				questPanelProgressBar.gameObject.SetActive(false);
			}
			global::System.Collections.Generic.IList<global::Kampai.Game.QuestStep> steps = quest.Steps;
			if (steps == null || steps.Count == 0)
			{
				return;
			}
			int count = quest.GetActiveDefinition().QuestSteps.Count;
			for (int i = 0; i < count; i++)
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(taskPanelPrefab);
				global::UnityEngine.Transform transform = gameObject.transform;
				transform.SetParent(taskScrollViewTransform, false);
				global::UnityEngine.RectTransform rectTransform = transform as global::UnityEngine.RectTransform;
				rectTransform.localPosition = global::UnityEngine.Vector3.zero;
				rectTransform.localScale = global::UnityEngine.Vector3.one;
				rectTransform.offsetMin = new global::UnityEngine.Vector2(taskPanelWidth * (float)i, 0f);
				rectTransform.offsetMax = new global::UnityEngine.Vector2(taskPanelWidth * (float)(i + 1), 0f);
				global::Kampai.UI.View.QuestStepView component = gameObject.GetComponent<global::Kampai.UI.View.QuestStepView>();
				component.questInstanceID = quest.ID;
				component.stepNumber = i;
				component.quest = quest;
				component.stepDefinition = quest.GetActiveDefinition().QuestSteps[i];
				component.step = steps[i];
				component.HighlightGoTo(true);
				if (modalSettings.enableDeliverThrob)
				{
					component.HighlightDeliver(true);
				}
				if (!modalSettings.enablePurchaseButtons)
				{
					component.SetTaskButtonState(true);
				}
				questStepViews.Add(gameObject);
			}
			taskScrollViewTransform.offsetMin = new global::UnityEngine.Vector2(0f, 0f);
			taskScrollViewTransform.offsetMax = new global::UnityEngine.Vector2((int)((float)quest.GetActiveDefinition().QuestSteps.Count * taskPanelWidth), 0f);
		}

		internal void SetupQuestProgressBar(global::Kampai.Game.Quest quest)
		{
			global::Kampai.Game.TimedQuestDefinition timedQuestDefinition = quest.GetActiveDefinition() as global::Kampai.Game.TimedQuestDefinition;
			if (timedQuestDefinition != null)
			{
				questPanelProgressBar.Init(quest.UTCQuestStartTime + timedQuestDefinition.Duration, timeService, localizationService);
			}
			global::Kampai.Game.LimitedQuestDefinition limitedQuestDefinition = quest.GetActiveDefinition() as global::Kampai.Game.LimitedQuestDefinition;
			if (limitedQuestDefinition != null)
			{
				questPanelProgressBar.Init(limitedQuestDefinition.ServerStopTimeUTC, timeService, localizationService);
			}
		}

		internal void RemoveCharacters()
		{
			currentQuestView.RemoveCoroutine();
			foreach (global::Kampai.UI.View.QuestView questTab in questTabs)
			{
				questTab.RemoveCoroutine();
			}
		}

		internal void CloseView()
		{
			base.Close();
		}

		private void ClearTabs()
		{
			foreach (global::Kampai.UI.View.QuestView questTab in questTabs)
			{
				global::UnityEngine.Object.Destroy(questTab.gameObject);
			}
			questTabs.Clear();
		}

		internal void InitCurrentQuestImage(int characterDefId, global::Kampai.UI.IFancyUIService fancyUIService, global::Kampai.UI.DummyCharacterType characterType)
		{
			currentQuestView.Init(characterDefId, fancyUIService, characterType);
		}

		internal void SetCurrentQuestImage(int characterDefId, global::Kampai.UI.DummyCharacterType characterType)
		{
			currentQuestView.UpdateQuest(characterDefId, characterType);
		}

		internal void InitQuestTabs(global::System.Collections.Generic.List<global::Kampai.Game.Quest> quests, int selectedQuestID)
		{
			ClearTabs();
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load("cmp_QuestBookIcon") as global::UnityEngine.GameObject;
			global::Kampai.UI.View.QuestView component = gameObject.GetComponent<global::Kampai.UI.View.QuestView>();
			questBookPadding = component.PaddingInPixels;
			global::UnityEngine.RectTransform rectTransform = gameObject.transform as global::UnityEngine.RectTransform;
			normalHeight = rectTransform.sizeDelta.y;
			int num = 0;
			global::System.Collections.Generic.List<global::Kampai.Game.Quest> list = QuestUtils.ResolveQuests(quests);
			foreach (global::Kampai.Game.Quest item in list)
			{
				if (item.state == global::Kampai.Game.QuestState.RunningTasks && item.GetActiveDefinition().SurfaceType != global::Kampai.Game.QuestSurfaceType.ProcedurallyGenerated && item.ID != selectedQuestID)
				{
					InitQuestBookPrefab(gameObject, normalHeight, num, item);
					num++;
				}
			}
			questTabScrollViewTransform.sizeDelta = new global::UnityEngine.Vector2(0f, (float)num * (normalHeight + questBookPadding));
		}

		internal void SwapQuest(global::Kampai.Game.Quest newQuest, int oldQuestID)
		{
			foreach (global::Kampai.UI.View.QuestView questTab in questTabs)
			{
				questTab.button.GetComponent<global::UnityEngine.UI.Button>().interactable = false;
				if (questTab.quest.ID == oldQuestID)
				{
					questTab.quest = newQuest;
					questTab.UpdateQuest(MoveHalfWayComplete);
				}
			}
		}

		private void MoveHalfWayComplete()
		{
			foreach (global::Kampai.UI.View.QuestView questTab in questTabs)
			{
				questTab.button.GetComponent<global::UnityEngine.UI.Button>().interactable = true;
			}
		}

		private void InitQuestBookPrefab(global::UnityEngine.GameObject prefab, float height, int index, global::Kampai.Game.Quest quest)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(prefab);
			global::UnityEngine.Transform transform = gameObject.transform;
			transform.SetParent(questTabScrollViewTransform, false);
			global::UnityEngine.RectTransform rectTransform = transform as global::UnityEngine.RectTransform;
			rectTransform.localPosition = global::UnityEngine.Vector3.zero;
			rectTransform.localScale = global::UnityEngine.Vector3.one;
			float num = (normalHeight + questBookPadding) * (float)index;
			rectTransform.offsetMin = new global::UnityEngine.Vector2(0f, num);
			rectTransform.offsetMax = new global::UnityEngine.Vector2(0f, num + height);
			global::Kampai.UI.View.QuestView component = gameObject.GetComponent<global::Kampai.UI.View.QuestView>();
			component.quest = quest;
			questTabs.Add(component);
		}

		public override void Init()
		{
			base.Init();
			global::UnityEngine.RectTransform rectTransform = base.transform as global::UnityEngine.RectTransform;
			rectTransform.offsetMin = global::UnityEngine.Vector2.zero;
			rectTransform.offsetMax = global::UnityEngine.Vector2.zero;
			float num = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
			rectTransform = taskScrollViewTransform.parent as global::UnityEngine.RectTransform;
			taskPanelWidth = (rectTransform.anchorMax.x - rectTransform.anchorMin.x) * num * (float)global::UnityEngine.Screen.width / UIUtils.GetHeightScale() / 3f;
			taskPanelPrefab = global::Kampai.Util.KampaiResources.Load("cmp_TaskPanel") as global::UnityEngine.GameObject;
			base.Open();
		}

		private void SetupQuestPanelInfo(global::Kampai.Game.QuestDefinition def, global::Kampai.Game.Transaction.TransactionDefinition rewardTransaction, global::Kampai.Game.IDefinitionService definitionService)
		{
			if (def.LocalizedKey != null)
			{
				questName.text = questService.GetQuestName(def.LocalizedKey);
			}
			else
			{
				questName.text = " ";
			}
			if (rewardTransaction == null)
			{
				return;
			}
			string path = "cmp_QuestReward";
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load(path) as global::UnityEngine.GameObject;
			if (gameObject == null)
			{
				logger.Error("Unable to load QuestReward prefab.");
				return;
			}
			CleanUp();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (global::Kampai.Util.QuantityItem output in rewardTransaction.Outputs)
			{
				if (num3 >= def.RewardDisplayCount)
				{
					continue;
				}
				global::Kampai.Game.DisplayableDefinition displayableDefinition = definitionService.Get<global::Kampai.Game.DisplayableDefinition>(output.ID);
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
				global::UnityEngine.GameObject gameObject3 = gameObject2.FindChild("txt_RewardCount");
				if (gameObject3 != null)
				{
					global::UnityEngine.UI.Text component = gameObject3.GetComponent<global::UnityEngine.UI.Text>();
					if (component != null)
					{
						switch (output.ID)
						{
						case 0:
							component.text = UIUtils.FormatLargeNumber(global::Kampai.Game.Transaction.TransactionUtil.SumOutputsForStaticItem(rewardTransaction, global::Kampai.Game.StaticItem.GRIND_CURRENCY_ID));
							break;
						case 2:
							component.text = UIUtils.FormatLargeNumber(global::Kampai.Game.Transaction.TransactionUtil.SumOutputsForStaticItem(rewardTransaction, global::Kampai.Game.StaticItem.XP_ID));
							break;
						default:
							component.text = UIUtils.FormatLargeNumber((int)output.Quantity);
							break;
						}
					}
				}
				global::UnityEngine.GameObject gameObject4 = gameObject2.FindChild("icn_RewardItem");
				if (gameObject4 != null)
				{
					global::Kampai.UI.View.KampaiImage component2 = gameObject4.GetComponent<global::Kampai.UI.View.KampaiImage>();
					if (component2 != null)
					{
						component2.sprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Image);
						component2.maskSprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Mask);
					}
				}
				if (displayableDefinition.ID <= 2)
				{
					num++;
					gameObject2.transform.SetParent(currencyRewardList, false);
				}
				else
				{
					num2++;
					gameObject2.transform.SetParent(itemRewardList, false);
					RewardItemButton.questRewards.Add(displayableDefinition);
				}
				num3++;
			}
			currencyRewardList.gameObject.SetActive(num > 0);
			itemRewardList.gameObject.SetActive(num2 > 0);
		}

		private void CleanUp()
		{
			RewardItemButton.questRewards.Clear();
			foreach (global::UnityEngine.Transform itemReward in itemRewardList)
			{
				global::UnityEngine.Object.Destroy(itemReward.gameObject);
			}
			foreach (global::UnityEngine.Transform currencyReward in currencyRewardList)
			{
				global::UnityEngine.Object.Destroy(currencyReward.gameObject);
			}
		}
	}
}
