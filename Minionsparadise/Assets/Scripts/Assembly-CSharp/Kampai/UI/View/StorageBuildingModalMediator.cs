namespace Kampai.UI.View
{
	public class StorageBuildingModalMediator : global::Kampai.UI.View.UIStackMediator<global::Kampai.UI.View.StorageBuildingModalView>
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("StorageBuildingModalMediator") as global::Kampai.Util.IKampaiLogger;

		private int currentUpgradeTransactionId;

		private int currentStorageBuildingId;

		private bool marketplaceUnlocked;

		private global::Kampai.UI.View.StorageBuildingModalTypes currentMode;

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdateStorageItemsSignal updateStorageItemsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeAllMenuSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal soundFXSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public RushDialogPurchaseHelper rushDialogPurchaseHelper { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideSkrimSignal hideSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal stateChangeSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetStorageCapacitySignal storageCapacitySignal { get; set; }

		[Inject]
		public global::Kampai.Game.IMarketplaceService marketplaceService { get; set; }

		[Inject]
		public global::Kampai.UI.View.EnableStorageBuildingItemDescriptionSignal enableItemDescriptionSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.UpdateSaleSlotsStateSignal updateSaleSlotsStateSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceOpenSalePanelSignal openSalePanelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceOpenBuyPanelSignal openBuyPanelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceCloseSalePanelSignal closeSalePanelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceCloseBuyPanelSignal closeBuyPanel { get; set; }

		[Inject]
		public global::Kampai.UI.View.MarketplaceCloseAllSalePanels closeAllSalePanels { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveFloatingTextSignal removeFloatingTextSignal { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistance { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.UI.View.PopupMessageSignal popupMessageSignal { get; set; }

		[Inject]
		public global::Kampai.Game.GenerateBuyItemsSignal generateBuyItemsSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveStorageBuildingItemDescriptionSignal removeItemDescriptionSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SelectStorageBuildingItemSignal selectStorageBuildingItemSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeOtherMenusSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal sfxSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.GoToResourceButtonClickedSignal gotoSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IGoToService gotoService { get; set; }

		public override void OnRegister()
		{
			base.OnRegister();
			base.view.Init();
			base.view.UpgradeButtonView.ClickedSignal.AddListener(UpgradeButtonClicked);
			base.view.SellButtonView.ClickedSignal.AddListener(OnSellPanelClick);
			base.view.BuyButtonView.ClickedSignal.AddListener(OnBuyPanelClick);
			base.view.ScrollListButtonView.ClickedSignal.AddListener(CloseSalePanel);
			base.view.OnMenuClose.AddListener(OnMenuClose);
			closeSalePanelSignal.AddListener(OnSellPanelClosed);
			closeBuyPanel.AddListener(OnBuyPanelClosed);
			updateStorageItemsSignal.AddListener(UpdateItems);
			closeAllMenuSignal.AddListener(CloseDialog);
			rushDialogPurchaseHelper.actionSuccessfulSignal.AddListener(OnTransactionSuccess);
			marketplaceUnlocked = marketplaceService.IsUnlocked();
			UpdateSellButton();
			base.view.InfoLabel.gameObject.SetActive(!marketplaceUnlocked);
			if (!marketplaceUnlocked)
			{
				global::Kampai.Game.MarketplaceDefinition marketplaceDefinition = definitionService.Get<global::Kampai.Game.MarketplaceDefinition>();
				base.view.InfoLabel.text = localService.GetString("MarketplaceUnlock", marketplaceDefinition.LevelGate);
			}
			global::Kampai.Game.StorageBuilding firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.StorageBuilding>(3018);
			if (firstInstanceByDefinitionId != null)
			{
				firstInstanceByDefinitionId.MenuOpened = true;
				firstInstanceByDefinitionId.MenuOpening = false;
			}
			gotoSignal.AddListener(GotoResourceBuilding);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			base.view.UpgradeButtonView.ClickedSignal.RemoveListener(UpgradeButtonClicked);
			base.view.OnMenuClose.RemoveListener(OnMenuClose);
			base.view.SellButtonView.ClickedSignal.RemoveListener(OnSellPanelClick);
			base.view.BuyButtonView.ClickedSignal.RemoveListener(OnBuyPanelClick);
			base.view.ScrollListButtonView.ClickedSignal.RemoveListener(CloseSalePanel);
			closeBuyPanel.RemoveListener(OnBuyPanelClosed);
			closeSalePanelSignal.RemoveListener(OnSellPanelClosed);
			updateStorageItemsSignal.RemoveListener(UpdateItems);
			closeAllMenuSignal.RemoveListener(CloseDialog);
			rushDialogPurchaseHelper.actionSuccessfulSignal.RemoveListener(OnTransactionSuccess);
			rushDialogPurchaseHelper.Cleanup();
			global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(currentStorageBuildingId);
			if (byInstanceId != null)
			{
				byInstanceId.MenuOpened = false;
			}
			gotoSignal.RemoveListener(GotoResourceBuilding);
		}

		public override void Initialize(global::Kampai.UI.View.GUIArguments args)
		{
			global::Kampai.Game.StorageBuilding building = args.Get<global::Kampai.Game.StorageBuilding>();
			soundFXSignal.Dispatch("Play_menu_popUp_01");
			CheckToGenerateBuyItems();
			currentMode = args.Get<global::Kampai.UI.View.StorageBuildingModalTypes>();
			switch (currentMode)
			{
			case global::Kampai.UI.View.StorageBuildingModalTypes.STORAGE:
				LoadItems(building);
				break;
			case global::Kampai.UI.View.StorageBuildingModalTypes.BUY:
				OpenBuyPanel(true);
				break;
			case global::Kampai.UI.View.StorageBuildingModalTypes.SELL:
				OpenSellPanel(true);
				break;
			}
			CheckForMarketplaceSurfacing();
			closeOtherMenusSignal.Dispatch(base.gameObject);
		}

		private void CheckToGenerateBuyItems()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.MarketplaceBuyItem> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.MarketplaceBuyItem>();
			if (marketplaceUnlocked && instancesByType != null && instancesByType.Count == 0)
			{
				generateBuyItemsSignal.Dispatch();
			}
		}

		internal void UpdateSellButton()
		{
			base.view.EnableMarketplace(marketplaceUnlocked);
		}

		internal void UpdateItems()
		{
			removeItemDescriptionSignal.Dispatch();
			global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(currentStorageBuildingId);
			LoadItems(byInstanceId);
			base.view.RearrangeItemView(true);
			if (currentMode == global::Kampai.UI.View.StorageBuildingModalTypes.SELL)
			{
				StartCoroutine(EnableItemDescriptionPopupDelay());
			}
		}

		private void OnTransactionSuccess()
		{
			UpdateItems();
			soundFXSignal.Dispatch("Play_expand_storage_01");
			storageCapacitySignal.Dispatch();
			global::Kampai.Game.StorageBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.StorageBuilding>(currentStorageBuildingId);
			if (byInstanceId.CurrentStorageBuildingLevel == byInstanceId.Definition.StorageUpgrades.Count - 1)
			{
				string type = localizationService.GetString("MaxStorageExpansionReached");
				popupMessageSignal.Dispatch(type, global::Kampai.UI.View.PopupMessageType.NORMAL);
			}
		}

		internal void UpgradeButtonClicked()
		{
			rushDialogPurchaseHelper.TryAction(true);
		}

		internal void CloseDialog(global::UnityEngine.GameObject sender)
		{
			if (sender != base.gameObject)
			{
				Close();
			}
		}

		protected override void Close()
		{
			soundFXSignal.Dispatch("Play_menu_disappear_01");
			CloseView();
		}

		private void CloseView()
		{
			base.view.Close();
			removeItemDescriptionSignal.Dispatch();
		}

		private void OnMenuClose()
		{
			stateChangeSignal.Dispatch(currentStorageBuildingId, global::Kampai.Game.BuildingState.Idle);
			hideSignal.Dispatch("StorageSkrim");
			guiService.Execute(global::Kampai.UI.View.GUIOperation.Unload, "screen_StorageBuilding");
		}

		internal void LoadItems(global::Kampai.Game.StorageBuilding building)
		{
			removeItemDescriptionSignal.Dispatch();
			base.view.scrollView.ClearItems();
			uint totalStorableQuantity = 0u;
			global::Kampai.UI.View.StorageBuildingModalTypes storageBuildingModalTypes = currentMode;
			global::System.Collections.Generic.ICollection<global::Kampai.Game.Item> collection;
			if (storageBuildingModalTypes == global::Kampai.UI.View.StorageBuildingModalTypes.SELL)
			{
				uint totalSellableQuantity = 0u;
				collection = playerService.GetSellableItems(out totalStorableQuantity, out totalSellableQuantity);
			}
			else
			{
				collection = playerService.GetStorableItems(out totalStorableQuantity);
			}
			uint currentStorageCapacity = playerService.GetCurrentStorageCapacity();
			currentStorageBuildingId = building.ID;
			if (building.CurrentStorageBuildingLevel == building.Definition.StorageUpgrades.Count - 1)
			{
				base.view.DisableExpandButton();
			}
			currentUpgradeTransactionId = building.Definition.StorageUpgrades[building.CurrentStorageBuildingLevel].TransactionId;
			rushDialogPurchaseHelper.Init(currentUpgradeTransactionId, global::Kampai.Game.TransactionTarget.STORAGEBUILDING, new global::Kampai.Game.TransactionArg(currentStorageBuildingId));
			base.view.SetCap((int)currentStorageCapacity);
			base.view.SetCurrentItemCount((int)totalStorableQuantity);
			if (totalStorableQuantity >= currentStorageCapacity)
			{
				base.view.UpdateStorageStatus(true);
			}
			else
			{
				base.view.UpdateStorageStatus(false);
			}
			if (collection != null)
			{
				foreach (global::Kampai.Game.Item item in collection)
				{
					global::Kampai.UI.View.StorageBuildingItemView slotView = global::Kampai.UI.View.StorageBuildingItemBuilder.Build(item, item.Definition, (int)item.Quantity, logger);
					base.view.scrollView.AddItem(slotView);
				}
				base.view.scrollView.SetupScrollView();
			}
			updateSaleSlotsStateSignal.Dispatch();
		}

		private void OnSellPanelClick()
		{
			base.view.HighlightSellButton(false);
			OpenSellPanel();
		}

		private void OpenSellPanel(bool isInstant = false)
		{
			if (base.view.SellPanel == null)
			{
				if (base.view.SellGrayImage.gameObject.activeSelf)
				{
					OnMarketplaceDisableClicked();
					return;
				}
				base.view.LoadSellMarketplacePanel();
			}
			if (base.view.BuyPanel != null && base.view.BuyPanel.IsOpen)
			{
				base.view.BuyPanel.SetOpen(false);
			}
			currentMode = global::Kampai.UI.View.StorageBuildingModalTypes.SELL;
			global::Kampai.Game.StorageBuilding firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.StorageBuilding>(3018);
			if (firstInstanceByDefinitionId != null)
			{
				LoadItems(firstInstanceByDefinitionId);
			}
			StartCoroutine(EnableItemDescriptionPopupDelay());
			if (base.view.SellPanel != null)
			{
				openSalePanelSignal.Dispatch(isInstant);
				base.view.SellButtonView.gameObject.SetActive(false);
				base.view.RearrangeItemView();
				CheckForMarketplaceSurfacing();
			}
			selectStorageBuildingItemSignal.Dispatch(0);
		}

		private global::System.Collections.IEnumerator EnableItemDescriptionPopupDelay()
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			enableItemDescriptionSignal.Dispatch(true);
		}

		private void OnSellPanelClosed()
		{
			base.view.SellButtonView.gameObject.SetActive(true);
			enableItemDescriptionSignal.Dispatch(false);
			currentMode = global::Kampai.UI.View.StorageBuildingModalTypes.STORAGE;
			UpdateItems();
		}

		private void OnBuyPanelClick()
		{
			base.view.HighlightBuyButton(false);
			OpenBuyPanel();
		}

		private void OpenBuyPanel(bool isInstant = false)
		{
			if (base.view.BuyPanel == null)
			{
				if (base.view.BuyGrayImage.gameObject.activeSelf)
				{
					OnMarketplaceDisableClicked();
					return;
				}
				base.view.LoadBuyMarketplacePanel();
			}
			if (base.view.BuyPanel != null)
			{
				openBuyPanelSignal.Dispatch(isInstant);
				base.view.BuyButtonView.gameObject.SetActive(false);
				base.view.SellButtonView.gameObject.SetActive(true);
				CheckForMarketplaceSurfacing();
			}
			if (!(base.view.SellPanel == null) && base.view.SellPanel.isOpen)
			{
				currentMode = global::Kampai.UI.View.StorageBuildingModalTypes.BUY;
				selectStorageBuildingItemSignal.Dispatch(0);
				closeAllSalePanels.Dispatch();
			}
		}

		private void OnBuyPanelClosed()
		{
			base.view.BuyButtonView.gameObject.SetActive(true);
			currentMode = global::Kampai.UI.View.StorageBuildingModalTypes.STORAGE;
			UpdateItems();
		}

		private void CloseSalePanel()
		{
			if (!(base.view.SellPanel == null) && base.view.SellPanel.isOpen)
			{
				closeAllSalePanels.Dispatch();
			}
		}

		private void CheckForMarketplaceSurfacing()
		{
			if (localPersistance.HasKeyPlayer("MarketSurfacing"))
			{
				localPersistance.DeleteKeyPlayer("MarketSurfacing");
				if (!localPersistance.HasKeyPlayer("MarketSurfacingButtonPulse"))
				{
					localPersistance.PutDataPlayer("MarketSurfacingButtonPulse", bool.FalseString);
					base.view.HighlightSellButton(true);
					base.view.HighlightBuyButton(true);
				}
				removeFloatingTextSignal.Dispatch(currentStorageBuildingId);
			}
		}

		private void OnMarketplaceDisableClicked()
		{
			global::Kampai.Game.MarketplaceDefinition marketplaceDefinition = definitionService.Get<global::Kampai.Game.MarketplaceDefinition>();
			if (playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID) < marketplaceDefinition.LevelGate)
			{
				string type = localService.GetString("MarketplaceUnlock", marketplaceDefinition.LevelGate);
				popupMessageSignal.Dispatch(type, global::Kampai.UI.View.PopupMessageType.NORMAL);
				sfxSignal.Dispatch("Play_action_locked_01");
			}
		}

		private void GotoResourceBuilding(int itemDefinitionId)
		{
			Close();
			gotoService.GoToBuildingFromItem(itemDefinitionId);
		}
	}
}
