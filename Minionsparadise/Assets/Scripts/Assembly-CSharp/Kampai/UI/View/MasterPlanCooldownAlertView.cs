namespace Kampai.UI.View
{
	public class MasterPlanCooldownAlertView : global::Kampai.UI.View.PopupMenuView
	{
		public global::UnityEngine.UI.Text timerText;

		public global::UnityEngine.UI.Text rewardCountText;

		public global::UnityEngine.UI.Text rushCostText;

		public ScrollableButtonView rushButton;

		public global::Kampai.UI.View.ButtonView waitButton;

		public global::UnityEngine.GameObject buildingSlot;

		public global::UnityEngine.Transform rewardsPanel;

		public int m_clockAnimationInternval = 1;

		internal global::UnityEngine.Coroutine timerRoutine;

		internal int rushCost;

		private global::Kampai.Main.ILocalizationService localizationService;

		private global::Kampai.Game.ITimeEventService timeEventService;

		private global::Kampai.UI.IFancyUIService fancyUIService;

		private global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.UI.View.IGUIService guiService;

		private global::Kampai.Game.Building building;

		private global::Kampai.Game.View.BuildingObject buildingObj;

		internal void Init(global::Kampai.Game.MasterPlan plan, bool HasReceivedFirstReward, global::Kampai.Game.ITimeEventService timeEventService, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Main.ILocalizationService localService, global::Kampai.UI.IFancyUIService fancyUIService, global::Kampai.UI.View.IGUIService guiService)
		{
			base.Init();
			localizationService = localService;
			this.timeEventService = timeEventService;
			this.fancyUIService = fancyUIService;
			this.definitionService = definitionService;
			this.guiService = guiService;
			rushButton.EnableDoubleConfirm();
			global::Kampai.Game.MasterPlanDefinition definition = plan.Definition;
			global::Kampai.Game.Transaction.TransactionDefinition coolDownTransactionDef = (HasReceivedFirstReward ? definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(definition.SubsequentCooldownRewardTransactionID) : definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(definition.CooldownRewardTransactionID));
			CreateRewardList(coolDownTransactionDef);
			int timeRemaining = timeEventService.GetTimeRemaining(plan.ID);
			SetTimerValues(timeRemaining);
			timerRoutine = StartCoroutine(TimerCoroutine(timeRemaining));
			base.Open();
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
				buildingObj = fancyUIService.CreateDummyBuildingObject(buildingDefinition, buildingSlot, out building);
				rewardCountText.text = localizationService.GetString(buildingDefinition.LocalizedKey);
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
			}
			return false;
		}

		private global::System.Collections.IEnumerator TimerCoroutine(int timeRemaining)
		{
			while (timeRemaining > 0)
			{
				yield return new global::UnityEngine.WaitForSeconds(m_clockAnimationInternval);
				timeRemaining -= m_clockAnimationInternval;
				SetTimerValues(timeRemaining);
			}
		}

		private void SetTimerValues(int timeRemaining)
		{
			timerText.text = UIUtils.FormatTime(timeRemaining, localizationService);
			rushCost = timeEventService.CalculateRushCostForTimer(timeRemaining, global::Kampai.Game.RushActionType.CONSTRUCTION);
			rushCostText.text = rushCost.ToString();
		}

		internal void Cleanup()
		{
			if (buildingObj != null)
			{
				fancyUIService.ReleaseBuildingObject(buildingObj, building);
			}
		}
	}
}
