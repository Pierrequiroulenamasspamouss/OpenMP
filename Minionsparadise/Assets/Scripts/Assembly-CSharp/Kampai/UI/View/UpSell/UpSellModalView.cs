namespace Kampai.UI.View.UpSell
{
	public class UpSellModalView : global::Kampai.UI.View.PopupMenuView
	{
		public global::Kampai.UI.View.LocalizeView title;

		public global::UnityEngine.RuntimeAnimatorController freeCollectButton;

		public global::Kampai.UI.View.LocalizeView dealDescription;

		[global::UnityEngine.Header("Timer Items")]
		public global::Kampai.UI.View.LocalizeView timerTitle;

		public global::Kampai.UI.View.LocalizeView offerTime;

		public global::UnityEngine.Color giftTitleColor = new global::UnityEngine.Color32(103, 170, 200, byte.MaxValue);

		[global::UnityEngine.Header("Banner Items")]
		public global::UnityEngine.GameObject bannerPanel;

		public global::Kampai.UI.View.LocalizeView percentOffBannerText;

		public global::Kampai.UI.View.UpSell.UpsellPriceMarkdownView priceMarkdownView;

		[global::UnityEngine.Header("Currency Icons")]
		public global::Kampai.UI.View.KampaiImage inputItemIcon;

		public global::Kampai.UI.View.DoubleConfirmButtonView purchaseCurrencyButton;

		public global::Kampai.UI.View.LocalizeView purchaseButtonCost;

		[global::UnityEngine.Header("Item Prefabs")]
		public global::UnityEngine.GameObject itemViewPrefab;

		[global::UnityEngine.Header("Item Locations")]
		public global::UnityEngine.Transform[] itemTransforms;

		[global::UnityEngine.Header("Skrim")]
		public global::Kampai.UI.View.ButtonView backGroundButton;

		protected global::Kampai.Game.PackDefinition packDefinition;

		protected global::Kampai.Game.SalePackDefinition salePackDefinition;

		protected global::Kampai.Game.ICurrencyService currencyService;

		protected global::Kampai.Game.IDefinitionService definitionService;

		protected global::Kampai.Main.ILocalizationService localizationService;

		protected global::Kampai.Game.IPlayerService playerService;

		protected global::Kampai.Game.ITimeEventService timeEventService;

		private global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> outputs;

		private global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> inputs;

		private global::Kampai.Game.IUpsellService upsellService;

		private global::System.Collections.IEnumerator m_updateSaleTime;

		public global::System.Collections.Generic.IList<global::Kampai.UI.View.UpSell.UpSellItemView> views { get; private set; }

		internal void Init(global::Kampai.Game.PackDefinition packDefinition, global::Kampai.Game.IUpsellService upsellService, global::Kampai.Game.IPlayerService playerService, global::Kampai.Game.ICurrencyService currencyService, global::Kampai.Game.IDefinitionService defService, global::Kampai.Main.ILocalizationService locService, global::Kampai.Game.ITimeEventService timeEventService)
		{
			base.Init();
			this.currencyService = currencyService;
			definitionService = defService;
			localizationService = locService;
			this.playerService = playerService;
			this.timeEventService = timeEventService;
			this.upsellService = upsellService;
			this.packDefinition = packDefinition;
			salePackDefinition = packDefinition as global::Kampai.Game.SalePackDefinition;
			if (packDefinition == null)
			{
				logger.Error("Sale Pack Definition is null for Upsell");
				Close(true);
				return;
			}
			if (packDefinition != null && packDefinition.TransactionDefinition != null)
			{
				outputs = packDefinition.TransactionDefinition.Outputs;
				inputs = packDefinition.TransactionDefinition.Inputs;
			}
			LoadSaleInfo();
			SetupPackItems();
		}

		protected void SetupTopBanner(bool isEnabled)
		{
			if (!isEnabled || percentOffBannerText == null || packDefinition.PercentagePer100 == 0)
			{
				if (!(bannerPanel == null))
				{
					bannerPanel.SetActive(false);
				}
				return;
			}
			percentOffBannerText.gameObject.SetActive(true);
			percentOffBannerText.text = string.Format("{0}%", packDefinition.PercentagePer100);
			if (!(bannerPanel == null))
			{
				bannerPanel.SetActive(true);
			}
		}

		protected bool SetMarkDownPriceBanner()
		{
			if (priceMarkdownView == null || priceMarkdownView.gameObject == null)
			{
				return false;
			}
			if (packDefinition.TransactionType == global::Kampai.Game.UpsellTransactionType.Cash && packDefinition.PercentagePer100 != 0)
			{
				priceMarkdownView.gameObject.SetActive(true);
				priceMarkdownView.Init(packDefinition, definitionService, currencyService);
				return true;
			}
			priceMarkdownView.gameObject.SetActive(false);
			return false;
		}

		protected void SetupBanner()
		{
			if (SetMarkDownPriceBanner())
			{
				SetupTopBanner(false);
			}
			else
			{
				SetupTopBanner(true);
			}
		}

		protected virtual void SetupPackItems()
		{
			ToggleDealDescription();
			UpdateCostText();
			if (itemTransforms != null && itemTransforms.Length > 0)
			{
				CreateItems(itemTransforms);
				SetupBanner();
			}
		}

		internal virtual void Release()
		{
			if (views != null)
			{
				for (int i = 0; i < views.Count; i++)
				{
					ReleaseItem(views[i]);
				}
				views = null;
			}
		}

		protected void LoadSaleInfo()
		{
			if (!(title == null) && !string.IsNullOrEmpty(packDefinition.LocalizedKey))
			{
				title.LocKey = packDefinition.LocalizedKey;
			}
		}

		public void ToggleDealDescription()
		{
			if (!(dealDescription == null) && !(dealDescription.gameObject == null))
			{
				if (string.IsNullOrEmpty(packDefinition.Description))
				{
					dealDescription.gameObject.SetActive(false);
				}
				else
				{
					dealDescription.LocKey = packDefinition.Description;
				}
			}
		}

		protected global::Kampai.UI.View.UpSell.UpSellItemView CreateItemView(global::UnityEngine.GameObject prefab, global::UnityEngine.Transform parent)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(prefab, global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.identity) as global::UnityEngine.GameObject;
			if (gameObject == null)
			{
				logger.Error("Could not create UpSellItemView from prefab");
				return null;
			}
			global::Kampai.UI.View.UpSell.UpSellItemView component = gameObject.GetComponent<global::Kampai.UI.View.UpSell.UpSellItemView>();
			if (component == null)
			{
				logger.Error("Could not get UpSellItemView from prefab");
				return null;
			}
			component.transform.SetParent(parent, false);
			SetUpItemTransform(component.transform as global::UnityEngine.RectTransform);
			return component;
		}

		private void SetUpItemTransform(global::UnityEngine.RectTransform rect)
		{
			if (!(rect == null))
			{
				rect.anchorMin = global::UnityEngine.Vector2.zero;
				rect.anchorMax = global::UnityEngine.Vector2.one;
				rect.sizeDelta = global::UnityEngine.Vector2.zero;
				rect.localScale = global::UnityEngine.Vector3.one;
				rect.localPosition = global::UnityEngine.Vector3.zero;
			}
		}

		protected void ReleaseItem(global::Kampai.UI.View.UpSell.UpSellItemView itemView)
		{
			if (!(itemView == null))
			{
				itemView.Release();
				global::UnityEngine.Object.Destroy(itemView);
			}
		}

		protected void CreateItems(params global::UnityEngine.Transform[] itemParents)
		{
			if (outputs != null && itemParents != null && outputs.Count != 0 && itemParents.Length != 0)
			{
				views = new global::Kampai.UI.View.UpSell.UpSellItemView[itemParents.Length];
				for (int i = 0; i < itemParents.Length; i++)
				{
					global::Kampai.UI.View.UpSell.UpSellItemView itemView = CreateItemView(itemViewPrefab, itemParents[i]);
					SetupItemView(i, itemView);
				}
			}
		}

		protected void SetupItemView(int i, global::Kampai.UI.View.UpSell.UpSellItemView itemView)
		{
			if (!(itemView == null))
			{
				views[i] = itemView;
				itemView.Item = upsellService.GetItemDef(i, outputs);
				if (itemView.Item != null)
				{
					bool audible = PackUtil.IsAudible(itemView.Item.ID, packDefinition);
					bool isExclusive = PackUtil.IsItemExclusive(itemView.Item.ID, packDefinition);
					itemView.SetAudible(audible);
					itemView.SetIsExclusive(isExclusive);
					itemView.ShowMtxID(packDefinition.CurrencyImageID);
					global::UnityEngine.Rect rect = (itemTransforms[0].transform as global::UnityEngine.RectTransform).rect;
					global::UnityEngine.Rect rect2 = (itemView.transform.parent.transform as global::UnityEngine.RectTransform).rect;
					itemView.AdditionalUIScale = global::UnityEngine.Mathf.Min(rect2.width / rect.width, rect2.height / rect.height);
				}
			}
		}

		protected void UpdateCostText()
		{
			bool isCash = false;
			string text = SetPrice(out isCash);
			if (!string.IsNullOrEmpty(text))
			{
				if (!isCash || string.Compare(text, localizationService.GetString("StoreBuy"), global::System.StringComparison.Ordinal) == 0)
				{
					purchaseButtonCost.text = text;
				}
				else
				{
					purchaseButtonCost.text = string.Format(purchaseButtonCost.text, text);
				}
			}
		}

		protected void SetFreeItem()
		{
			upsellService.SetupFreeItemButton(purchaseButtonCost, purchaseCurrencyButton, "Collect", freeCollectButton);
			ToggleDealDescription();
		}

		protected void SetupBuyButton()
		{
			if (inputs == null || inputs.Count == 0)
			{
				return;
			}
			global::Kampai.Util.QuantityItem quantityItem = inputs[0];
			if (quantityItem == null)
			{
				logger.Error(string.Format("Item input is null for sale pack definition: {0}", packDefinition));
				return;
			}
			global::Kampai.Game.ItemDefinition itemDefinition = definitionService.Get<global::Kampai.Game.ItemDefinition>(quantityItem.ID);
			if (itemDefinition == null)
			{
				logger.Error(string.Format("Item input is not an item definition for sale pack definition: {0}", packDefinition));
				return;
			}
			inputItemIcon.gameObject.SetActive(true);
			UIUtils.SetItemIcon(inputItemIcon, itemDefinition);
		}

		private string SetPrice(out bool isCash)
		{
			isCash = false;
			if (inputItemIcon != null && inputItemIcon.gameObject != null)
			{
				inputItemIcon.gameObject.SetActive(false);
			}
			bool flag = !string.IsNullOrEmpty(packDefinition.SKU);
			bool flag2 = packDefinition.PercentagePer100 > 0;
			bool flag3 = inputs.Count > 0;
			inputItemIcon.gameObject.SetActive(false);
			if (salePackDefinition != null && salePackDefinition.Type == global::Kampai.Game.SalePackType.Redeemable)
			{
				SetFreeItem();
				SetTimerTitle("Gift", false);
				return string.Empty;
			}
			if (flag3)
			{
				if (inputs[0].ID == 1)
				{
					purchaseCurrencyButton.EnableDoubleConfirm();
				}
				if (flag2)
				{
					int transactionCurrencyCost = global::Kampai.Game.Transaction.TransactionUtil.GetTransactionCurrencyCost(packDefinition.TransactionDefinition.ToDefinition(), definitionService, playerService, (global::Kampai.Game.StaticItem)inputs[0].ID);
					SetupBuyButton();
					SetTimerTitle("UpSellTimeLeft", true);
					return ((int)((float)transactionCurrencyCost * packDefinition.getDiscountRate())).ToString();
				}
				SetupBuyButton();
				SetTimerTitle("UpSellTimeLeft", true);
				return upsellService.SumOutput(inputs, inputs[0].ID).ToString();
			}
			if (flag)
			{
				isCash = true;
				SetTimerTitle("UpSellTimeLeft", true);
				return currencyService.GetPriceWithCurrencyAndFormat(packDefinition.SKU);
			}
			if (!flag3)
			{
				SetFreeItem();
				SetTimerTitle("Gift", false);
				return string.Empty;
			}
			return string.Empty;
		}

		protected void SetTimerTitle(string locKey, bool startTimeUpdater)
		{
			if (timerTitle == null)
			{
				logger.Error("timerTitle GameObject is null");
				return;
			}
			if (!string.IsNullOrEmpty(packDefinition.BannerAd))
			{
				timerTitle.LocKey = packDefinition.BannerAd;
				if (offerTime != null && offerTime.gameObject != null)
				{
					offerTime.gameObject.SetActive(false);
				}
			}
			if (salePackDefinition == null || salePackDefinition.Duration == 0)
			{
				return;
			}
			if (offerTime == null)
			{
				logger.Error("offerTime GameObject is null");
				return;
			}
			if (salePackDefinition.TransactionType != global::Kampai.Game.UpsellTransactionType.Free && string.IsNullOrEmpty(salePackDefinition.BannerAd))
			{
				SetUpTimer(locKey, startTimeUpdater);
				return;
			}
			if (offerTime != null && offerTime.gameObject != null)
			{
				offerTime.gameObject.SetActive(false);
			}
			timerTitle.LocKey = ((!string.IsNullOrEmpty(salePackDefinition.BannerAd)) ? salePackDefinition.BannerAd : locKey);
		}

		private void SetUpTimer(string locKey, bool startTimeUpdater)
		{
			timerTitle.LocKey = locKey;
			if (!startTimeUpdater || offerTime == null)
			{
				if (offerTime != null && offerTime.gameObject != null)
				{
					offerTime.gameObject.SetActive(false);
				}
			}
			else
			{
				offerTime.gameObject.SetActive(true);
				m_updateSaleTime = UpdateSaleTime("UpSellTimeLeftFormat");
				StartCoroutine(m_updateSaleTime);
			}
		}

		public override void Close(bool instant = false)
		{
			if (m_updateSaleTime != null)
			{
				StopCoroutine(m_updateSaleTime);
			}
			base.Close(instant);
		}

		internal global::System.Collections.IEnumerator UpdateSaleTime(string timeLocKey)
		{
			bool isValidString = true;
			while (isValidString)
			{
				if (offerTime == null)
				{
					isValidString = false;
					continue;
				}
				offerTime.LocKey = salePackDefinition.BannerAd;
				global::Kampai.Game.Sale saleItem = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.Sale>(salePackDefinition.ID);
				if (saleItem != null)
				{
					int saleTime = timeEventService.GetTimeRemaining(saleItem.ID);
					offerTime.LocKey = timeLocKey;
					string saleTimeStr;
					if (saleTime <= 0)
					{
						saleTimeStr = UIUtils.FormatTime(0, localizationService);
						isValidString = false;
					}
					else
					{
						saleTimeStr = UIUtils.FormatTime(saleTime, localizationService);
					}
					offerTime.text = string.Format(offerTime.text, saleTimeStr);
				}
				yield return new global::UnityEngine.WaitForSeconds(1f);
			}
			m_updateSaleTime = null;
		}
	}
}
