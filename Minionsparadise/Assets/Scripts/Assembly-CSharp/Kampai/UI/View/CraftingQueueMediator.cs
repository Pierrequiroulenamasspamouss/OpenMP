namespace Kampai.UI.View
{
	public class CraftingQueueMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private bool isMidRecipeDrag;

		private GoTween activeScaleTween;

		[Inject]
		public global::Kampai.UI.View.CraftingQueueView view { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeEventService timeEventService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.UI.View.SpawnDooberSignal tweenSignal { get; set; }

		[Inject(global::Kampai.UI.View.UIElement.CAMERA)]
		public global::UnityEngine.Camera uiCamera { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdateQueueIcon updateQueueSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal globalSFXSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetPremiumCurrencySignal setPremiumCurrencySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveCraftingQueueSignal removeCraftingQueueSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RefreshQueueSlotSignal purchaseSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ResetDoubleTapSignal resetDoubleTapSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeDragStartSignal recipeDragStartSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeDragStopSignal recipeDragStopSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeUpdateSignal recipeUpdateSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingCompleteSignal craftingComplete { get; set; }

		[Inject]
		public global::Kampai.Game.OpenStorageBuildingSignal openStorageBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IQuestService questService { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal changeStateSignal { get; set; }

		public override void OnRegister()
		{
			view.Init(definitionService, playerService, timeEventService, localizationService, changeStateSignal);
			view.inProgressRush.ClickedSignal.AddListener(RushButton);
			view.inProgressHarvest.ClickedSignal.AddListener(HarvestCraftable);
			view.lockedPurchase.ClickedSignal.AddListener(UnlockButton);
			updateQueueSignal.AddListener(UpdateView);
			recipeDragStartSignal.AddListener(OnRecipeDragStart);
			recipeDragStopSignal.AddListener(OnRecipeDragStop);
			view.onPointerEnterSignal.AddListener(OnPointerEnter);
			view.onPointerExitSignal.AddListener(OnPointerExit);
		}

		public override void OnRemove()
		{
			HandleHarvestables();
			view.inProgressRush.ClickedSignal.RemoveListener(RushButton);
			view.inProgressHarvest.ClickedSignal.RemoveListener(HarvestCraftable);
			view.lockedPurchase.ClickedSignal.RemoveListener(UnlockButton);
			updateQueueSignal.RemoveListener(UpdateView);
			recipeDragStartSignal.RemoveListener(OnRecipeDragStart);
			recipeDragStopSignal.RemoveListener(OnRecipeDragStop);
			view.onPointerEnterSignal.RemoveListener(OnPointerEnter);
			view.onPointerExitSignal.RemoveListener(OnPointerExit);
		}

		private void UpdateView()
		{
			view.Init(definitionService, playerService, timeEventService, localizationService, changeStateSignal);
		}

		private void HandleHarvestables()
		{
			if (view.harvestReady)
			{
				global::Kampai.Game.CraftingBuilding building = view.building;
				craftingComplete.Dispatch(building.ID);
			}
		}

		private void RushButton()
		{
			if (global::Kampai.Game.InputUtils.touchCount <= 1)
			{
				resetDoubleTapSignal.Dispatch(view.index);
				if (view.isLocked)
				{
					playerService.ProcessRush(view.purchaseCost, true, PurchaseTransactionCallback);
				}
				else
				{
					Rush(view.rushCost);
				}
			}
		}

		public void Rush(int rushCost, bool checkStorage = true, bool checkDoubleConfirmation = true)
		{
			if (checkStorage && playerService.isStorageFull())
			{
				ShowStore();
			}
			else if (!checkDoubleConfirmation || view.inProgressRush.isDoubleConfirmed())
			{
				playerService.ProcessRush(rushCost, true, RushTransactionCallback, view.itemDef.ID);
			}
		}

		private void HarvestCraftable()
		{
			if (playerService.isStorageFull())
			{
				ShowStore();
				return;
			}
			global::Kampai.Game.CraftingBuilding building = view.building;
			HandleTween(uiCamera.WorldToScreenPoint(view.inProgressImage.transform.position));
			removeCraftingQueueSignal.Dispatch(new global::Kampai.Util.Tuple<int, int>(building.ID, 0));
			view.harvestReady = false;
			StartNextCraft(building);
		}

		private void ShowStore()
		{
			if (playerService.HasStorageBuilding())
			{
				global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(314);
				openStorageBuildingSignal.Dispatch(byInstanceId, false);
			}
		}

		private void UnlockButton()
		{
			if (view.lockedPurchase.isDoubleConfirmed() && view.isLocked)
			{
				playerService.ProcessSlotPurchase(view.purchaseCost, true, view.index + 1, PurchaseTransactionCallback, view.building.ID);
			}
		}

		private void RushTransactionCallback(global::Kampai.Game.PendingCurrencyTransaction pct)
		{
			if (pct.Success)
			{
				global::Kampai.Game.CraftingBuilding building = view.building;
				globalSFXSignal.Dispatch("Play_button_premium_01");
				if (view.inProduction)
				{
					HandleTween(uiCamera.WorldToScreenPoint(view.inProgressImage.transform.position));
					timeEventService.RushEvent(view.building.ID);
					removeCraftingQueueSignal.Dispatch(new global::Kampai.Util.Tuple<int, int>(building.ID, 0));
				}
				else
				{
					HandleTween(uiCamera.WorldToScreenPoint(view.availableImage.transform.position));
					removeCraftingQueueSignal.Dispatch(new global::Kampai.Util.Tuple<int, int>(building.ID, view.index));
				}
				questService.UpdateAllQuestsWithQuestStepType(global::Kampai.Game.QuestStepType.Harvest, global::Kampai.Game.QuestTaskTransition.Complete);
				setPremiumCurrencySignal.Dispatch();
				StartNextCraft(building);
			}
		}

		private void StartNextCraft(global::Kampai.Game.CraftingBuilding building)
		{
			if (building.RecipeInQueue.Count > 0)
			{
				building.CraftingStartTime = timeService.CurrentTime();
				global::Kampai.Game.IngredientsItemDefinition ingredientsItemDefinition = definitionService.Get<global::Kampai.Game.IngredientsItemDefinition>(building.RecipeInQueue[0]);
				timeEventService.AddEvent(building.ID, global::System.Convert.ToInt32(timeService.CurrentTime()), (int)ingredientsItemDefinition.TimeToHarvest, craftingComplete, global::Kampai.Game.TimeEventType.ProductionBuff);
			}
		}

		private void HandleTween(global::UnityEngine.Vector3 origin)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(view.itemDef.TransactionId);
			foreach (global::Kampai.Util.QuantityItem output in transactionDefinition.Outputs)
			{
				tweenSignal.Dispatch(origin, DooberUtil.GetDestinationType(output.ID, definitionService), output.ID, false);
			}
			StartCoroutine(WaitForDooberTween());
		}

		private global::System.Collections.IEnumerator WaitForDooberTween()
		{
			yield return new global::UnityEngine.WaitForSeconds(2.5f);
			recipeUpdateSignal.Dispatch(view.itemDef.ID);
		}

		private void PurchaseTransactionCallback(global::Kampai.Game.PendingCurrencyTransaction pct)
		{
			if (pct.Success)
			{
				globalSFXSignal.Dispatch("Play_button_premium_01");
				purchaseSignal.Dispatch(true);
				setPremiumCurrencySignal.Dispatch();
			}
			else if (pct.ParentSuccess)
			{
				RushButton();
			}
		}

		private void OnRecipeDragStart(int recipeDefId)
		{
			isMidRecipeDrag = true;
		}

		private void OnRecipeDragStop(int recipeDefId)
		{
			isMidRecipeDrag = false;
			TweenScale(false);
		}

		private void OnPointerEnter(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (isMidRecipeDrag && view.index >= view.building.RecipeInQueue.Count && !view.isLocked)
			{
				TweenScale(true);
			}
		}

		private void OnPointerExit(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			TweenScale(false);
		}

		private void TweenScale(bool isFocused)
		{
			if (activeScaleTween != null)
			{
				activeScaleTween.destroy();
			}
			global::UnityEngine.Vector3 one = global::UnityEngine.Vector3.one;
			if (view.isLocked || view.index > 0)
			{
				one *= 0.8f;
			}
			if (isMidRecipeDrag && isFocused)
			{
				one *= 1.15f;
			}
			if (one != view.transform.localScale)
			{
				activeScaleTween = Go.to(view.transform, 0.2f, new GoTweenConfig().scale(one).onComplete(delegate(AbstractGoTween tween)
				{
					tween.destroy();
					activeScaleTween = null;
				}));
			}
		}
	}
}
