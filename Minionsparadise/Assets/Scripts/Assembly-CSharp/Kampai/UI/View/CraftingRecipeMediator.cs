namespace Kampai.UI.View
{
	public class CraftingRecipeMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private GoTween tween;

		private global::UnityEngine.GameObject dragIcon;

		private global::UnityEngine.GameObject dragGlow;

		private global::UnityEngine.GameObject dragPrefab;

		private global::Kampai.Game.CraftingBuilding craftingBuilding;

		private global::Kampai.Game.IngredientsItemDefinition currentItemDef;

		private global::UnityEngine.RectTransform dragTransform;

		private global::UnityEngine.Vector2 initialIconPosition;

		private bool midDrag;

		private global::System.Collections.IEnumerator PointerDownWait;

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeView view { get; set; }

		[Inject(global::Kampai.Main.MainElement.UI_GLASSCANVAS)]
		public global::UnityEngine.GameObject glassCanvas { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IQuestService questService { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal playSFXSignal { get; set; }

		[Inject]
		public global::Kampai.Common.AppPauseSignal pauseSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeEventService timeEventService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal changeStateSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingCompleteSignal craftingComplete { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdateQueueIcon updateQueueSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingQueuePositionUpdateSignal queuePositionSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingModalClosedSignal closedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeUpdateSignal updateSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingUpdateReagentsSignal craftingUpdateReagentsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisplayItemPopupSignal displayItemPopupSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideItemPopupSignal hideItemPopupSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RushDialogConfirmationSignal rushedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ResetDoubleTapSignal resetDoubleTapSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.UI.View.PopupMessageSignal popupMessageSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeDragStartSignal dragStartSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CraftingRecipeDragStopSignal dragStopSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetStorageCapacitySignal setStorageSignal { get; set; }

		public override void OnRegister()
		{
			pauseSignal.AddListener(OnPause);
			closedSignal.AddListener(HandleClose);
			updateSignal.AddListener(OnUpdate);
			craftingUpdateReagentsSignal.AddListener(UpdateReagents);
			rushedSignal.AddListener(ItemRushed);
			hideItemPopupSignal.AddListener(RemovePopupDelay);
			view.pointerDownSignal.AddListener(PointerDown);
			view.pointerDragSignal.AddListener(PointerDrag);
			view.pointerUpSignal.AddListener(PointerUp);
			Init();
		}

		public override void OnRemove()
		{
			pauseSignal.RemoveListener(OnPause);
			closedSignal.RemoveListener(HandleClose);
			updateSignal.RemoveListener(OnUpdate);
			craftingUpdateReagentsSignal.RemoveListener(UpdateReagents);
			rushedSignal.RemoveListener(ItemRushed);
			hideItemPopupSignal.RemoveListener(RemovePopupDelay);
			view.pointerDownSignal.RemoveListener(PointerDown);
			view.pointerDragSignal.RemoveListener(PointerDrag);
			view.pointerUpSignal.RemoveListener(PointerUp);
		}

		private void Init()
		{
			view.Init(definitionService, playerService);
			craftingBuilding = playerService.GetByInstanceId<global::Kampai.Game.CraftingBuilding>(view.instanceID);
			dragPrefab = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>("cmp_DragIcon");
		}

		private void OnPause()
		{
			if (midDrag && dragIcon != null)
			{
				HandleClose();
			}
			hideItemPopupSignal.Dispatch();
		}

		private void PointerDown(global::UnityEngine.EventSystems.PointerEventData eventData, global::Kampai.Game.IngredientsItemDefinition iid)
		{
			resetDoubleTapSignal.Dispatch(-1);
			hideItemPopupSignal.Dispatch();
			displayItemPopupSignal.Dispatch(iid.ID, view.GetComponent<global::UnityEngine.RectTransform>(), global::Kampai.UI.View.UIPopupType.CRAFTING);
			if (IsPointerDownValid())
			{
				global::UnityEngine.GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
				if (!(gameObject == null))
				{
					currentItemDef = null;
					playSFXSignal.Dispatch("Play_pick_item_01");
					dragIcon = global::UnityEngine.Object.Instantiate(dragPrefab);
					dragIcon.transform.SetParent(glassCanvas.transform, false);
					global::Kampai.UI.View.KampaiIngoreRaycastImage component = dragIcon.transform.Find("img_RecipeItem").gameObject.GetComponent<global::Kampai.UI.View.KampaiIngoreRaycastImage>();
					component.sprite = UIUtils.LoadSpriteFromPath(iid.Image);
					component.maskSprite = UIUtils.LoadSpriteFromPath(iid.Mask);
					dragGlow = dragIcon.transform.Find("backing_glow").gameObject;
					SetSize();
					dragTransform.anchoredPosition = new global::UnityEngine.Vector2((eventData.position / UIUtils.GetHeightScale()).x, (eventData.position / UIUtils.GetHeightScale()).y);
					initialIconPosition = dragTransform.anchoredPosition;
					midDrag = true;
					view.SetHighlight(false);
					dragStartSignal.Dispatch(iid.ID);
				}
			}
		}

		private void SetSize()
		{
			dragTransform = dragIcon.GetComponent<global::UnityEngine.RectTransform>();
			dragTransform.localPosition = global::UnityEngine.Vector3.zero;
			dragTransform.localScale = new global::UnityEngine.Vector3(1.25f, 1.25f, 1.25f);
			dragTransform.anchorMin = global::UnityEngine.Vector2.zero;
			dragTransform.anchorMax = global::UnityEngine.Vector2.zero;
			dragTransform.pivot = new global::UnityEngine.Vector2(0.5f, 0.5f);
			dragTransform.sizeDelta = new global::UnityEngine.Vector2(100f, 100f);
		}

		private bool IsPointerDownValid()
		{
			if (global::Kampai.Game.InputUtils.touchCount > 1)
			{
				return false;
			}
			if (midDrag)
			{
				return false;
			}
			if (!view.isUnlocked)
			{
				playSFXSignal.Dispatch("Play_action_locked_01");
				return false;
			}
			return true;
		}

		private void PointerDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!midDrag || !(dragIcon != null))
			{
				return;
			}
			dragTransform.anchoredPosition = new global::UnityEngine.Vector2((eventData.position / UIUtils.GetHeightScale()).x, (eventData.position / UIUtils.GetHeightScale()).y);
			global::UnityEngine.GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			if (gameObject != null && gameObject.name.Equals("DragArea"))
			{
				if (craftingBuilding.RecipeInQueue.Count < craftingBuilding.Slots)
				{
					view.IsValidDragAreaSignal.Dispatch(true, view.greenCircle.gameObject.activeSelf);
				}
				dragGlow.SetActive(true);
			}
			else
			{
				view.IsValidDragAreaSignal.Dispatch(false, false);
				dragGlow.SetActive(false);
			}
		}

		private void PointerUp(global::UnityEngine.EventSystems.PointerEventData eventData, global::Kampai.Game.IngredientsItemDefinition itemDef)
		{
			if (PointerDownWait != null)
			{
				return;
			}
			PointerDownWait = PopupDelay();
			StartCoroutine(PointerDownWait);
			view.IsValidDragAreaSignal.Dispatch(false, false);
			if (!midDrag)
			{
				return;
			}
			global::UnityEngine.GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			if (gameObject != null)
			{
				bool flag = gameObject.name == "DragArea";
				global::Kampai.UI.View.CraftingQueueView craftingQueueView = (flag ? null : gameObject.GetComponentInParent<global::Kampai.UI.View.CraftingQueueView>());
				if (flag || craftingQueueView != null)
				{
					if (craftingBuilding.RecipeInQueue.Count < craftingBuilding.Slots)
					{
						if (flag || (!craftingQueueView.isLocked && craftingQueueView.index >= craftingBuilding.RecipeInQueue.Count))
						{
							playSFXSignal.Dispatch("Play_place_item_01");
							currentItemDef = itemDef;
							RunTransaction();
							return;
						}
					}
					else
					{
						popupMessageSignal.Dispatch(localService.GetString("CraftQueueFull"), global::Kampai.UI.View.PopupMessageType.NORMAL);
					}
				}
			}
			TweenBackToOrigin();
		}

		private void TweenBackToOrigin()
		{
			tween = Go.to(dragTransform, 0.25f, new GoTweenConfig().setEaseType(GoEaseType.Linear).vector2Prop("anchoredPosition", initialIconPosition).onComplete(delegate
			{
				HandleClose();
			}));
		}

		private global::System.Collections.IEnumerator PopupDelay()
		{
			yield return new global::UnityEngine.WaitForSeconds(0.5f);
			if (PointerDownWait != null)
			{
				hideItemPopupSignal.Dispatch();
			}
		}

		private void RemovePopupDelay()
		{
			if (PointerDownWait != null)
			{
				StopCoroutine(PointerDownWait);
				PointerDownWait = null;
			}
		}

		private void ItemRushed()
		{
			if (currentItemDef != null && craftingBuilding != null)
			{
				RunTransaction();
			}
		}

		private void RunTransaction()
		{
			playerService.StartTransaction(currentItemDef.TransactionId, global::Kampai.Game.TransactionTarget.INGREDIENT, TransactionCallback, new global::Kampai.Game.TransactionArg(craftingBuilding.ID));
		}

		private void TransactionCallback(global::Kampai.Game.PendingCurrencyTransaction pct)
		{
			if (pct.Success)
			{
				setStorageSignal.Dispatch();
				int iD = craftingBuilding.ID;
				if (craftingBuilding.RecipeInQueue.Count == 0)
				{
					timeEventService.AddEvent(iD, global::System.Convert.ToInt32(timeService.CurrentTime()), (int)currentItemDef.TimeToHarvest, craftingComplete, global::Kampai.Game.TimeEventType.ProductionBuff);
					craftingBuilding.CraftingStartTime = timeService.CurrentTime();
					changeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Working);
				}
				playerService.UpdateCraftingQueue(iD, currentItemDef.ID);
				global::Kampai.Game.DynamicIngredientsDefinition definition;
				if (definitionService.TryGet<global::Kampai.Game.DynamicIngredientsDefinition>(currentItemDef.ID, out definition))
				{
					int iD2 = definition.ID;
					int num = questService.IsOneOffCraftableDisplayable(definition.QuestDefinitionUnlockId, iD2);
					int num2 = SumDynamicCount(iD2);
					if (num2 >= num)
					{
						view.gameObject.SetActive(false);
					}
				}
				updateQueueSignal.Dispatch();
				queuePositionSignal.Dispatch();
				craftingUpdateReagentsSignal.Dispatch();
				HandleClose();
			}
			else if (pct.ParentSuccess)
			{
				RunTransaction();
			}
			else
			{
				HandleClose();
			}
			currentItemDef = null;
		}

		private int SumDynamicCount(int defID)
		{
			int num = 0;
			num += (int)playerService.GetQuantityByDefinitionId(defID);
			global::System.Collections.Generic.ICollection<global::Kampai.Game.CraftingBuilding> byDefinitionId = playerService.GetByDefinitionId<global::Kampai.Game.CraftingBuilding>(craftingBuilding.Definition.ID);
			foreach (global::Kampai.Game.CraftingBuilding item in byDefinitionId)
			{
				foreach (int item2 in item.RecipeInQueue)
				{
					if (item2 == defID)
					{
						num++;
					}
				}
				foreach (int completedCraft in item.CompletedCrafts)
				{
					if (completedCraft == defID)
					{
						num++;
					}
				}
			}
			return num;
		}

		private void OnUpdate(int recipeDefId)
		{
			if (recipeDefId == view.recipeID && view.isUnlocked)
			{
				view.SetQuantity();
			}
		}

		private void UpdateReagents()
		{
			view.SetImageBorder();
			view.SetQuantity();
		}

		private void HandleClose()
		{
			if (midDrag)
			{
				midDrag = false;
				global::UnityEngine.Object.Destroy(dragIcon);
				if (tween != null)
				{
					tween.destroy();
				}
				dragStopSignal.Dispatch(view.recipeID);
			}
		}
	}
}
