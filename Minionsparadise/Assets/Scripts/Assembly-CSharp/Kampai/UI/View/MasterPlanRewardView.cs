namespace Kampai.UI.View
{
	public class MasterPlanRewardView : global::Kampai.UI.View.PopupMenuView
	{
		public global::Kampai.UI.View.ButtonView collectButton;

		public global::UnityEngine.UI.Text collectButtonText;

		public global::UnityEngine.UI.Text content;

		public global::UnityEngine.UI.Text title;

		[global::UnityEngine.Header("Normal MasterPlan Reward Only")]
		public global::UnityEngine.RectTransform scrollViewTransform;

		public global::UnityEngine.UI.ScrollRect scrollRect;

		[global::UnityEngine.Header("CoolDown MasterPlan Reward Only")]
		public global::UnityEngine.RectTransform rewardsPanel;

		public global::UnityEngine.GameObject buildingSlot;

		public global::UnityEngine.UI.Text rewardCountText;

		public global::UnityEngine.UI.Text Description;

		public float padding = 10f;

		internal global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.Main.ILocalizationService localizedService;

		private global::Kampai.UI.IFancyUIService fancyUIService;

		private global::Kampai.UI.View.IGUIService guiService;

		internal global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition;

		internal global::System.Collections.Generic.List<global::UnityEngine.GameObject> viewList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

		internal void Init(global::Kampai.Game.Transaction.TransactionDefinition tD, global::Kampai.Main.ILocalizationService localService, global::Kampai.Game.IDefinitionService defService, global::Kampai.Game.IPlayerService playerService, global::Kampai.UI.IFancyUIService fancyUIService, global::Kampai.UI.View.IGUIService guiService, int planInstanceID)
		{
			base.Init();
			definitionService = defService;
			localizedService = localService;
			this.fancyUIService = fancyUIService;
			this.guiService = guiService;
			transactionDefinition = tD;
			if (transactionDefinition != null)
			{
				if (planInstanceID == 0)
				{
					SetupScrollView(RewardUtil.GetRewardQuantityFromTransaction(transactionDefinition, definitionService, playerService));
				}
				else
				{
					CreateRewardList(transactionDefinition);
				}
			}
			base.Open();
		}

		internal void SetupScrollView(global::System.Collections.Generic.List<global::Kampai.Game.View.RewardQuantity> quantityChange)
		{
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("cmp_RewardSlider") as global::UnityEngine.GameObject;
			float x = scrollViewTransform.sizeDelta.x;
			int count = quantityChange.Count;
			for (int i = 0; i < count; i++)
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
				gameObject.transform.SetParent(scrollViewTransform, false);
				global::Kampai.UI.View.RewardSliderView component = gameObject.GetComponent<global::Kampai.UI.View.RewardSliderView>();
				component.scrollRect = scrollRect;
				global::Kampai.Game.UnlockDefinition definition;
				global::Kampai.Game.DisplayableDefinition displayableDefinition = ((quantityChange[i].ID == 0 || quantityChange[i].ID == 1 || quantityChange[i].ID == 5 || !definitionService.TryGet<global::Kampai.Game.UnlockDefinition>(quantityChange[i].ID, out definition)) ? definitionService.Get<global::Kampai.Game.DisplayableDefinition>(quantityChange[i].ID) : definitionService.Get<global::Kampai.Game.DisplayableDefinition>(definition.ReferencedDefinitionID));
				if (displayableDefinition != null)
				{
					component.description.text = localizedService.GetString(displayableDefinition.LocalizedKey);
					component.icon.sprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Image);
					component.icon.maskSprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Mask);
					component.itemQuantity.gameObject.SetActive(true);
					component.itemQuantity.text = UIUtils.FormatLargeNumber(quantityChange[i].Quantity);
					viewList.Add(gameObject);
				}
				else
				{
					logger.Warning("Social reward Item not valid {0}", i);
				}
			}
			int num = 2 * (int)x;
			int num2 = count * (int)x + (int)(padding * (float)count);
			int num3 = 0;
			if (num2 > num)
			{
				num3 = (num2 - num) / 2;
			}
			scrollViewTransform.sizeDelta = new global::UnityEngine.Vector2(num2, x);
			scrollViewTransform.localPosition = new global::UnityEngine.Vector2(num3, scrollViewTransform.localPosition.y);
		}

		private void CreateRewardList(global::Kampai.Game.Transaction.TransactionDefinition coolDownTransactionDef)
		{
			int outputCount = global::Kampai.Game.Transaction.TransactionDataExtension.GetOutputCount(coolDownTransactionDef);
			for (int i = 0; i < outputCount; i++)
			{
				global::Kampai.Util.QuantityItem quantityItem = coolDownTransactionDef.Outputs[i];
				if (CreateRewardItem(quantityItem.ID, (int)quantityItem.Quantity))
				{
					break;
				}
			}
		}

		private bool CreateRewardItem(int itemDefID, int itemCount)
		{
			global::Kampai.Game.Definition definition = definitionService.Get<global::Kampai.Game.Definition>(itemDefID);
			global::Kampai.Game.BuildingDefinition buildingDefinition = definition as global::Kampai.Game.BuildingDefinition;
			if (buildingDefinition != null)
			{
				buildingSlot.transform.parent.gameObject.SetActive(true);
				global::Kampai.Game.Building building;
				fancyUIService.CreateDummyBuildingObject(buildingDefinition, buildingSlot, out building);
				rewardCountText.text = localizedService.GetString(buildingDefinition.LocalizedKey);
				viewList.Add(buildingSlot);
				return true;
			}
			global::Kampai.Game.ItemDefinition itemDefinition = definition as global::Kampai.Game.ItemDefinition;
			if (itemDefinition != null)
			{
				global::UnityEngine.GameObject gameObject = guiService.Execute(global::Kampai.UI.View.GUIOperation.LoadUntrackedInstance, "cmp_MasterPlanCooldownRewardItem");
				global::Kampai.UI.View.MasterPlanCooldownRewardItemView component = gameObject.GetComponent<global::Kampai.UI.View.MasterPlanCooldownRewardItemView>();
				component.SetCount(itemCount);
				UIUtils.SetItemIcon(component.icon, itemDefinition);
				gameObject.transform.SetParent(rewardsPanel, false);
				gameObject.transform.localScale = global::UnityEngine.Vector3.one;
				viewList.Add(component.gameObject);
			}
			return false;
		}
	}
}
