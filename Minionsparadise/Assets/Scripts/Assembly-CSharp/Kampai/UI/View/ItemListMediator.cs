namespace Kampai.UI.View
{
	public class ItemListMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.ItemListView>
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("ItemListMediator") as global::Kampai.Util.IKampaiLogger;

		private global::UnityEngine.GameObject dragIcon;

		private bool placingNonIconBuilding;

		private bool placingIconBuilding;

		private bool dragingLockedItem = true;

		private global::UnityEngine.UI.ScrollRect scrollRect;

		private bool isDragging;

		private global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::Kampai.UI.View.StoreTab> storeTabs;

		private bool isSubMenuOpen;

		private global::System.Collections.Generic.Queue<global::UnityEngine.Vector2> HorizontalDrag;

		private global::Kampai.Game.PurchaseNewBuildingSignal purchaseNewBuildingSignal;

		private global::Kampai.Game.CreateInventoryBuildingSignal createInventoryBuildingSignal;

		private global::Kampai.Game.PostMinionPartyStartSignal postMinionPartyStartSignal;

		private global::Kampai.Game.PostMinionPartyEndSignal postMinionPartyEndSignal;

		private global::Kampai.Game.StoreItemType lastPickedType;

		[Inject]
		public global::Kampai.UI.View.BuildMenuDefinitionLoadedSignal defLoadedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.AddStoreTabSignal addTabSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.OnTabClickedSignal tabClickSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MoveTabMenuSignal moveTabSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MoveBuildMenuSignal moveBaseMenuSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal audioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IQuestService questService { get; set; }

		[Inject(global::Kampai.Main.MainElement.UI_GLASSCANVAS)]
		public global::UnityEngine.GameObject glassCanvas { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.UI.View.BuildMenuOpenedSignal buildMenuOpened { get; set; }

		[Inject]
		public global::Kampai.UI.View.HighlightStoreItemSignal highlightStoreItemSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.DragFromStoreSignal dragFromStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideStoreHighlightSignal hideHightlightSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdateUIButtonsSignal updateStoreButtonsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdatePartyPointButtonsSignal updatePartyButtonsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.IBuildMenuService buildMenuService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.UI.View.PopupMessageSignal popupMessageSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MessageDialogClosed messageDialogClosed { get; set; }

		[Inject]
		public global::Kampai.Game.CancelPurchaseSignal cancelPurchaseSignal { get; set; }

		[Inject]
		public global::Kampai.Common.AppPauseSignal pauseSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HighlightTabSignal highlightTabSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIModel uimodel { get; set; }

		[Inject]
		public global::Kampai.Game.SendBuildingToInventorySignal sendBuildingToInventorySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ToggleStoreTabSignal toggleStoreTabSignal { get; set; }

		public override void OnRegister()
		{
			global::strange.extensions.injector.api.ICrossContextInjectionBinder injectionBinder = gameContext.injectionBinder;
			base.OnRegister();
			base.view.Init();
			defLoadedSignal.AddListener(OnDefinitionLoaded);
			updateStoreButtonsSignal.AddListener(UpdateStoreButtons);
			updatePartyButtonsSignal.AddListener(UpdatePartyPointButtons);
			addTabSignal.AddListener(AddStoreTab);
			highlightTabSignal.AddListener(HighlightStoreTab);
			tabClickSignal.AddListener(OnTabClicked);
			buildMenuOpened.AddListener(OnBuildMenuOpened);
			highlightStoreItemSignal.AddListener(HighlightStoreItem);
			hideHightlightSignal.AddListener(OnHideHighlight);
			base.view.Title.ClickedSignal.AddListener(OnItemMenuTitleClicked);
			postMinionPartyStartSignal = injectionBinder.GetInstance<global::Kampai.Game.PostMinionPartyStartSignal>();
			postMinionPartyStartSignal.AddListener(UpdatePartyPointButtons);
			postMinionPartyEndSignal = injectionBinder.GetInstance<global::Kampai.Game.PostMinionPartyEndSignal>();
			postMinionPartyEndSignal.AddListener(UpdatePartyPointButtons);
			purchaseNewBuildingSignal = injectionBinder.GetInstance<global::Kampai.Game.PurchaseNewBuildingSignal>();
			purchaseNewBuildingSignal.AddListener(NewBuildingPurchased);
			createInventoryBuildingSignal = injectionBinder.GetInstance<global::Kampai.Game.CreateInventoryBuildingSignal>();
			createInventoryBuildingSignal.AddListener(BuildingDraggedFromInventory);
			sendBuildingToInventorySignal.AddListener(BuildingSentToInventory);
			scrollRect = base.view.ScrollViewParent.parent.GetComponent<global::UnityEngine.UI.ScrollRect>();
			storeTabs = new global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::Kampai.UI.View.StoreTab>();
			HorizontalDrag = new global::System.Collections.Generic.Queue<global::UnityEngine.Vector2>();
			moveBaseMenuSignal.AddListener(BaseMenuMoved);
			pauseSignal.AddListener(ResetIconDragState);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			defLoadedSignal.RemoveListener(OnDefinitionLoaded);
			updateStoreButtonsSignal.RemoveListener(UpdateStoreButtons);
			updatePartyButtonsSignal.RemoveListener(UpdatePartyPointButtons);
			addTabSignal.RemoveListener(AddStoreTab);
			tabClickSignal.RemoveListener(OnTabClicked);
			highlightTabSignal.RemoveListener(HighlightStoreTab);
			buildMenuOpened.RemoveListener(OnBuildMenuOpened);
			highlightStoreItemSignal.RemoveListener(HighlightStoreItem);
			hideHightlightSignal.RemoveListener(OnHideHighlight);
			base.view.Title.ClickedSignal.RemoveListener(OnItemMenuTitleClicked);
			postMinionPartyStartSignal.RemoveListener(UpdatePartyPointButtons);
			postMinionPartyEndSignal.RemoveListener(UpdatePartyPointButtons);
			purchaseNewBuildingSignal.RemoveListener(NewBuildingPurchased);
			createInventoryBuildingSignal.RemoveListener(BuildingDraggedFromInventory);
			sendBuildingToInventorySignal.RemoveListener(BuildingSentToInventory);
			moveBaseMenuSignal.RemoveListener(BaseMenuMoved);
			pauseSignal.RemoveListener(ResetIconDragState);
		}

		public void BaseMenuMoved(bool show)
		{
			uimodel.AllowMultiTouch = show;
			if (!show)
			{
				Close();
				ResetIconDragState();
			}
		}

		internal void OnDefinitionLoaded(global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.Game.Definition>> storeMenuDefs)
		{
			global::Kampai.UI.View.StoreButtonView storeButtonView = null;
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.Game.Definition>> storeMenuDef in storeMenuDefs)
			{
				global::Kampai.Game.StoreItemType key = storeMenuDef.Key;
				storeMenuDef.Value.Sort(delegate(global::Kampai.Game.Definition x, global::Kampai.Game.Definition definition2)
				{
					global::Kampai.Game.StoreItemDefinition storeItemDefinition2 = x as global::Kampai.Game.StoreItemDefinition;
					global::Kampai.Game.StoreItemDefinition storeItemDefinition3 = definition2 as global::Kampai.Game.StoreItemDefinition;
					if (storeItemDefinition2 == null)
					{
						return 1;
					}
					if (storeItemDefinition3 == null)
					{
						return -1;
					}
					int levelItemUnlocksAt = (storeItemDefinition2.Type == global::Kampai.Game.StoreItemType.SalePack || storeItemDefinition2.Type == global::Kampai.Game.StoreItemType.PremiumCurrency || storeItemDefinition2.Type == global::Kampai.Game.StoreItemType.GrindCurrency) ? 0 : definitionService.GetLevelItemUnlocksAt(storeItemDefinition2.ReferencedDefID);
					int levelItemUnlocksAt2 = (storeItemDefinition3.Type == global::Kampai.Game.StoreItemType.SalePack || storeItemDefinition3.Type == global::Kampai.Game.StoreItemType.PremiumCurrency || storeItemDefinition3.Type == global::Kampai.Game.StoreItemType.GrindCurrency) ? 0 : definitionService.GetLevelItemUnlocksAt(storeItemDefinition3.ReferencedDefID);
					if (levelItemUnlocksAt < levelItemUnlocksAt2)
					{
						return -1;
					}
					if (levelItemUnlocksAt > levelItemUnlocksAt2)
					{
						return 1;
					}
					if (storeItemDefinition2.PriorityDefinition < storeItemDefinition3.PriorityDefinition)
					{
						return -1;
					}
					return (storeItemDefinition2.PriorityDefinition > storeItemDefinition3.PriorityDefinition) ? 1 : 0;
				});
				foreach (global::Kampai.Game.Definition item in storeMenuDef.Value)
				{
					global::Kampai.Game.StoreItemDefinition storeItemDefinition = item as global::Kampai.Game.StoreItemDefinition;
					if (storeItemDefinition == null)
					{
						continue;
					}
					global::Kampai.Game.Definition definition = definitionService.Get(storeItemDefinition.ReferencedDefID);
					if (definition != null)
					{
						global::Kampai.Game.Transaction.TransactionDefinition transaction = definitionService.Get(storeItemDefinition.TransactionID) as global::Kampai.Game.Transaction.TransactionDefinition;
						storeButtonView = base.view.GetStoreButtonViewByID(storeItemDefinition.ID);
						if (storeButtonView == null)
						{
							storeButtonView = global::Kampai.UI.View.StoreButtonBuilder.Build(definition, transaction, storeItemDefinition, base.view.ScrollViewParent, localService, definitionService, logger, playerService);
							storeButtonView.pointerDownSignal.AddListener(OnPointerDown);
							storeButtonView.pointerDragSignal.AddListener(OnPointerDrag);
							storeButtonView.pointerUpSignal.AddListener(OnPointerUp);
						}
						base.view.AddStoreButton(key, storeButtonView);
					}
				}
			}
			if (storeButtonView != null)
			{
				global::UnityEngine.RectTransform rectTransform = storeButtonView.transform as global::UnityEngine.RectTransform;
				float y = rectTransform.sizeDelta.y;
				float paddingInPixels = storeButtonView.PaddingInPixels;
				base.view.SetupButtonHeight(y, paddingInPixels);
			}
			buildMenuService.RetoreBuidMenuState(base.view.GetAllButtonViews());
		}

		private void SetDisplayOnStoreButtonViewCost(global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> views, int buildingDefID)
		{
			foreach (global::Kampai.UI.View.StoreButtonView view in views)
			{
				if (view.definition.ID == buildingDefID)
				{
					view.DisplayOrHideUnlockedCostIcons();
					break;
				}
			}
		}

		private void NewBuildingPurchased(global::Kampai.Game.Building building)
		{
			int iD = building.Definition.ID;
			global::Kampai.Game.StoreItemType type = base.view.UpdateStoreButtonState(iD, true);
			if (buildMenuService.RemoveNewUnlockedItem(type, iD))
			{
				buildMenuService.RemoveUncheckedInventoryItem(type, iD);
			}
		}

		private void BuildingDraggedFromInventory(global::Kampai.Game.Building building, global::Kampai.Game.Location location)
		{
			int iD = building.Definition.ID;
			global::Kampai.Game.StoreItemType type = base.view.UpdateStoreButtonState(iD, true);
			if (buildMenuService.RemoveNewUnlockedItem(type, iD))
			{
				buildMenuService.RemoveUncheckedInventoryItem(type, iD);
			}
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(type);
			if (storeButtonViews != null)
			{
				SetDisplayOnStoreButtonViewCost(storeButtonViews, iD);
			}
		}

		private void BuildingSentToInventory(int buildingInstanceID)
		{
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(buildingInstanceID);
			int iD = byInstanceId.Definition.ID;
			global::Kampai.Game.StoreItemType storeItemType = base.view.UpdateStoreButtonState(iD, false);
			SetBadgeCountForStoreItemType(storeItemType);
			buildMenuService.AddUncheckedInventoryItem(storeItemType, iD);
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(storeItemType);
			if (storeButtonViews != null)
			{
				SetDisplayOnStoreButtonViewCost(storeButtonViews, iD);
			}
		}

		private void UpdateStoreButtons(bool clearUnlock)
		{
			if (clearUnlock)
			{
				buildMenuService.ClearAllNewUnlockItems();
			}
			global::Kampai.UI.IBuildMenuService obj = buildMenuService;
			bool updateBadge = clearUnlock;
			obj.UpdateNewUnlockList(base.view.GetAllButtonViews(), true, updateBadge);
			UpdatePartyPointButtons();
		}

		private void UpdatePartyPointButtons()
		{
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(global::Kampai.Game.StoreItemType.Decoration);
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews2 = base.view.GetStoreButtonViews(global::Kampai.Game.StoreItemType.Leisure);
			if (storeButtonViews != null)
			{
				foreach (global::Kampai.UI.View.StoreButtonView item in storeButtonViews)
				{
					item.UpdatePartyPointText(localService);
				}
			}
			if (storeButtonViews2 == null)
			{
				return;
			}
			foreach (global::Kampai.UI.View.StoreButtonView item2 in storeButtonViews2)
			{
				item2.UpdatePartyPointText(localService);
			}
		}

		internal void OnHideHighlight()
		{
			foreach (global::Kampai.Game.StoreItemType key in storeTabs.Keys)
			{
				global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(key);
				if (storeButtonViews != null)
				{
					foreach (global::Kampai.UI.View.StoreButtonView item in storeButtonViews)
					{
						item.SetHighlight(false);
					}
				}
			}
		}

		internal void OnItemMenuTitleClicked()
		{
			moveTabSignal.Dispatch(true);
			if (isSubMenuOpen)
			{
				audioSignal.Dispatch("Play_shop_pane_out_01");
			}
			isSubMenuOpen = false;
			base.view.MoveSubMenu(false);
		}

		protected override void Close()
		{
			OnItemMenuTitleClicked();
		}

		internal void AddStoreTab(global::Kampai.UI.View.StoreTab tab)
		{
			storeTabs.Add(tab.Type, tab);
		}

		internal void OnTabClicked(global::Kampai.Game.StoreItemType type, string localizedTitle)
		{
			base.view.TabIcon.maskSprite = global::Kampai.UI.View.StoreTabBuilder.SetTabIcon(type, logger);
			global::UnityEngine.RectTransform rectTransform = scrollRect.content.transform as global::UnityEngine.RectTransform;
			rectTransform.anchoredPosition = global::UnityEngine.Vector2.zero;
			RefreshWhatButtonsShouldBeVisible(type);
			if (base.view.SetupItemMenu(type, localizedTitle))
			{
				moveTabSignal.Dispatch(false);
				audioSignal.Dispatch("Play_shop_pane_in_01");
				base.view.MoveSubMenu(true);
				isSubMenuOpen = true;
			}
			else
			{
				audioSignal.Dispatch("Play_action_locked_01");
			}
			lastPickedType = type;
			CheckPinataQuest();
		}

		private void CheckPinataQuest()
		{
			if (lastPickedType != global::Kampai.Game.StoreItemType.Leisure || !questService.GetQuestMap().ContainsKey(101120) || questService.GetQuestMap()[101120].State != global::Kampai.Game.QuestState.RunningTasks || playerService.GetInstancesByDefinitionID(3123).Count != 0)
			{
				return;
			}
			cancelPurchaseSignal.AddListener(OnCancelBuildingPlacement);
			popupMessageSignal.Dispatch(localService.GetString("BuildingHelperDialog"), global::Kampai.UI.View.PopupMessageType.AUTO_CLOSE_OVERRIDE);
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(global::Kampai.Game.StoreItemType.Leisure);
			foreach (global::Kampai.UI.View.StoreButtonView item in storeButtonViews)
			{
				if (item.definition.ID == 3123)
				{
					item.SetHighlight(true);
					break;
				}
			}
		}

		private void OnCancelBuildingPlacement(bool invalid)
		{
			cancelPurchaseSignal.RemoveListener(OnCancelBuildingPlacement);
			if (isSubMenuOpen && invalid)
			{
				messageDialogClosed.AddListener(OnMessageDialogClosed);
			}
		}

		private void OnMessageDialogClosed()
		{
			messageDialogClosed.RemoveListener(OnMessageDialogClosed);
			if (isSubMenuOpen)
			{
				CheckPinataQuest();
			}
		}

		private void RefreshWhatButtonsShouldBeVisible(global::Kampai.Game.StoreItemType type)
		{
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(type);
			toggleStoreTabSignal.Dispatch(type, buildMenuService.ShowingAChild(storeButtonViews));
		}

		private void OnBuildMenuOpened()
		{
			foreach (global::Kampai.Game.StoreItemType key in storeTabs.Keys)
			{
				SetBadgeCountForStoreItemType(key);
				RefreshWhatButtonsShouldBeVisible(key);
			}
			if (isSubMenuOpen)
			{
				base.view.RefreshStoreButtonLayout();
			}
		}

		internal void SetBadgeCountForStoreItemType(global::Kampai.Game.StoreItemType type)
		{
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(type);
			if (storeButtonViews == null)
			{
				return;
			}
			foreach (global::Kampai.UI.View.StoreButtonView item in storeButtonViews)
			{
				int iD = item.definition.ID;
				int inventoryCountByDefinitionID = playerService.GetInventoryCountByDefinitionID(iD);
				item.SetBadge(inventoryCountByDefinitionID);
			}
		}

		protected override void OnCloseAllMenu(global::UnityEngine.GameObject exception)
		{
			ResetIconDragState();
		}

		private void ResetIconDragState()
		{
			if (dragIcon != null)
			{
				base.view.Title.ClickedSignal.AddListener(OnItemMenuTitleClicked);
				isDragging = false;
				global::UnityEngine.Object.Destroy(dragIcon);
				dragIcon = null;
				OnHideHighlight();
			}
		}

		internal void HighlightStoreItem(global::Kampai.Game.StoreItemDefinition definition, global::Kampai.UI.View.HighlightType type)
		{
			if (!storeTabs.ContainsKey(definition.Type))
			{
				return;
			}
			global::Kampai.UI.View.StoreTab storeTab = storeTabs[definition.Type];
			OnTabClicked(storeTab.Type, storeTab.LocalizedName);
			global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> storeButtonViews = base.view.GetStoreButtonViews(definition.Type);
			float num = 0f;
			foreach (global::Kampai.UI.View.StoreButtonView item in storeButtonViews)
			{
				if (item.gameObject.activeSelf)
				{
					global::UnityEngine.RectTransform rectTransform = item.transform as global::UnityEngine.RectTransform;
					if (item.definition.ID == definition.ReferencedDefID)
					{
						item.SetHighlight(item.IsUnlocked(), type);
						global::UnityEngine.RectTransform rectTransform2 = scrollRect.content.transform as global::UnityEngine.RectTransform;
						rectTransform2.anchoredPosition = new global::UnityEngine.Vector2(0f, num);
						break;
					}
					item.SetHighlight(false, type);
					num += rectTransform.rect.height + item.PaddingInPixels;
				}
			}
		}

		internal void HighlightStoreTab(global::Kampai.Game.StoreItemType type)
		{
			if (storeTabs.ContainsKey(type))
			{
				moveBaseMenuSignal.Dispatch(true);
				global::Kampai.UI.View.StoreTab storeTab = storeTabs[type];
				OnTabClicked(storeTab.Type, storeTab.LocalizedName);
			}
		}

		private void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData, global::Kampai.Game.Definition definition, global::Kampai.Game.Transaction.TransactionDefinition transactionDef, bool canPurchase)
		{
			placingNonIconBuilding = false;
			placingIconBuilding = false;
			if (global::Kampai.Game.InputUtils.touchCount > 1)
			{
				if (dragIcon != null)
				{
					global::UnityEngine.Object.Destroy(dragIcon);
					dragIcon = null;
					base.view.Title.ClickedSignal.AddListener(OnItemMenuTitleClicked);
				}
				return;
			}
			isDragging = true;
			if (!canPurchase)
			{
				audioSignal.Dispatch("Play_action_locked_01");
				dragIcon = null;
				scrollRect.OnBeginDrag(eventData);
				dragingLockedItem = true;
				return;
			}
			audioSignal.Dispatch("Play_button_click_01");
			dragingLockedItem = false;
			if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.name.CompareTo("img_ItemIcon") == 0 && dragIcon == null)
			{
				popoutIcon(definition, eventData);
			}
			else
			{
				scrollRect.OnBeginDrag(eventData);
			}
		}

		private void popoutIcon(global::Kampai.Game.Definition definition, global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			global::Kampai.Game.DisplayableDefinition displayableDefinition = definition as global::Kampai.Game.DisplayableDefinition;
			base.view.Title.ClickedSignal.RemoveListener(OnItemMenuTitleClicked);
			dragIcon = new global::UnityEngine.GameObject("DragIcon");
			dragIcon.transform.SetParent(glassCanvas.transform, false);
			dragIcon.layer = 5;
			global::Kampai.UI.View.KampaiIngoreRaycastImage kampaiIngoreRaycastImage = dragIcon.AddComponent<global::Kampai.UI.View.KampaiIngoreRaycastImage>();
			kampaiIngoreRaycastImage.sprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Image);
			kampaiIngoreRaycastImage.material = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Material>("CircleIconAlphaMaskMat");
			kampaiIngoreRaycastImage.maskSprite = UIUtils.LoadSpriteFromPath(displayableDefinition.Mask);
			global::UnityEngine.RectTransform component = dragIcon.GetComponent<global::UnityEngine.RectTransform>();
			component.localPosition = new global::UnityEngine.Vector3(0f, 0f, 0f);
			global::Kampai.UI.View.KampaiImage component2 = dragIcon.GetComponent<global::Kampai.UI.View.KampaiImage>();
			component.anchoredPosition = new global::UnityEngine.Vector2(eventData.position.x / UIUtils.GetHeightScale(), eventData.position.y / UIUtils.GetHeightScale() + component2.sprite.rect.height * component2.pixelsPerUnit / 2f);
			component.localScale = global::UnityEngine.Vector3.one;
			component.anchorMin = new global::UnityEngine.Vector2(0f, 0f);
			component.anchorMax = new global::UnityEngine.Vector2(0f, 0f);
			component.pivot = new global::UnityEngine.Vector2(0.5f, 0.5f);
			float num = global::UnityEngine.Mathf.Max(component2.sprite.rect.width, component2.sprite.rect.height);
			component.sizeDelta = new global::UnityEngine.Vector2(component2.sprite.rect.width / num * 100f, component2.sprite.rect.height / num * 100f);
		}

		private void OnPointerDrag(global::UnityEngine.EventSystems.PointerEventData eventData, global::Kampai.Game.Definition definition, global::Kampai.Game.Transaction.TransactionDefinition transactionDef, int badgeCount)
		{
			if (isDragging && dragIcon == null)
			{
				scrollRect.OnDrag(eventData);
			}
			if (dragingLockedItem)
			{
				return;
			}
			int num = 10;
			float num2 = 0.3f;
			float num3 = 3f;
			if (dragIcon == null)
			{
				if (HorizontalDrag.Count >= num)
				{
					HorizontalDrag.Dequeue();
				}
				HorizontalDrag.Enqueue(eventData.position);
				float num4 = 0f;
				float num5 = 0f;
				global::UnityEngine.Vector2[] array = HorizontalDrag.ToArray();
				for (int i = 0; i < HorizontalDrag.Count; i++)
				{
					if (i != 0)
					{
						num5 += array[i].x - array[i - 1].x;
						num4 += array[i].y - array[i - 1].y;
					}
				}
				float value = ((!(num5 <= 0.001f)) ? (num4 / num5) : 100f);
				if (global::System.Math.Abs(value) <= num2 && num5 >= num3 && !placingIconBuilding && !placingNonIconBuilding)
				{
					popoutIcon(definition, eventData);
				}
				if (eventData.pointerCurrentRaycast.gameObject == null && !placingNonIconBuilding && !dragingLockedItem && dragIcon == null && !placingIconBuilding)
				{
					isDragging = false;
					scrollRect.OnEndDrag(eventData);
					dragFromStoreSignal.Dispatch(definition, transactionDef, eventData.position, true);
					moveBaseMenuSignal.Dispatch(false);
					placingNonIconBuilding = true;
				}
			}
			else if (eventData.pointerCurrentRaycast.gameObject == null)
			{
				if (dragIcon != null && !placingIconBuilding && !placingNonIconBuilding)
				{
					base.view.Title.ClickedSignal.AddListener(OnItemMenuTitleClicked);
					isDragging = false;
					global::UnityEngine.Object.Destroy(dragIcon);
					dragIcon = null;
					placingIconBuilding = true;
					scrollRect.OnEndDrag(eventData);
					moveBaseMenuSignal.Dispatch(false);
					dragFromStoreSignal.Dispatch(definition, transactionDef, eventData.position, true);
				}
			}
			else if (dragIcon != null)
			{
				setDragIconPosition(eventData);
			}
		}

		private void setDragIconPosition(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			global::UnityEngine.RectTransform component = dragIcon.GetComponent<global::UnityEngine.RectTransform>();
			component.anchoredPosition = eventData.position / UIUtils.GetHeightScale();
			global::Kampai.UI.View.KampaiImage component2 = dragIcon.GetComponent<global::Kampai.UI.View.KampaiImage>();
			component.anchoredPosition = new global::UnityEngine.Vector2(component.anchoredPosition.x, component.anchoredPosition.y + component2.sprite.rect.height * component2.pixelsPerUnit / 2f);
		}

		private void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData, global::Kampai.Game.Definition definition, global::Kampai.Game.Transaction.TransactionDefinition transactionDef)
		{
			dragingLockedItem = true;
			placingIconBuilding = false;
			placingNonIconBuilding = false;
			HorizontalDrag = new global::System.Collections.Generic.Queue<global::UnityEngine.Vector2>();
			if (dragIcon != null)
			{
				base.view.Title.ClickedSignal.AddListener(OnItemMenuTitleClicked);
				isDragging = false;
				global::UnityEngine.Object.Destroy(dragIcon);
				dragIcon = null;
			}
			else
			{
				scrollRect.OnEndDrag(eventData);
			}
		}
	}
}
