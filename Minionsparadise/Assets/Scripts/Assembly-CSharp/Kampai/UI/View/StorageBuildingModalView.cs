namespace Kampai.UI.View
{
	public class StorageBuildingModalView : global::Kampai.UI.View.PopupMenuView
	{
		private const int SELL_PANEL_OPEN_COLUMN_NUM = 3;

		private const float REARRANGE_ITEM_TIME = 0.3f;

		private const float INSTANCE_REARRANGE_ITEM_TIME = 0.0001f;

		private const float SELL_PANEL_OPEN_ANCHOR = 0.52f;

		public global::Kampai.UI.View.ButtonView UpgradeButtonView;

		public global::Kampai.UI.View.ButtonView SellButtonView;

		public global::Kampai.UI.View.ButtonView BuyButtonView;

		public global::Kampai.UI.View.ButtonView ScrollListButtonView;

		public global::UnityEngine.RectTransform ItemsPanel;

		public global::Kampai.UI.View.KampaiImage SellGrayImage;

		public global::Kampai.UI.View.KampaiImage BuyGrayImage;

		public global::Kampai.UI.View.KampaiImage BuyBackgroundImage;

		public global::Kampai.UI.View.KampaiImage SellBackgroundImage;

		public global::Kampai.UI.View.KampaiLabel InfoLabel;

		public global::UnityEngine.UI.Text Capacity;

		public global::Kampai.UI.View.KampaiScrollView scrollView;

		public global::UnityEngine.RectTransform FillImage;

		public global::UnityEngine.Animator backgroundAnim;

		public global::UnityEngine.GameObject CapacityPanel;

		private int currentItemCount;

		private int currentCapacity;

		internal global::Kampai.UI.View.SellPanelView SellPanel { get; set; }

		internal global::Kampai.UI.View.BuyMarketplacePanelView BuyPanel { get; set; }

		public override void Init()
		{
			base.Init();
			Open();
		}

		internal void RearrangeItemView(bool isMoveInstance = false)
		{
			int num = 0;
			int num2 = global::UnityEngine.Mathf.FloorToInt(scrollView.ColumnNumber);
			float num3 = 1f;
			if (SellPanel != null && SellPanel.isOpen)
			{
				num2 = 3;
				num3 = 0.52f;
			}
			int num4 = 0;
			int num5 = 0;
			foreach (global::UnityEngine.MonoBehaviour itemView in scrollView.ItemViewList)
			{
				global::Kampai.UI.View.StorageBuildingItemView storageBuildingItemView = itemView as global::Kampai.UI.View.StorageBuildingItemView;
				global::UnityEngine.RectTransform rectTransform = itemView.transform as global::UnityEngine.RectTransform;
				if (!(storageBuildingItemView == null) && !(rectTransform == null))
				{
					num4 = num / num2;
					num5 = num % num2;
					float num6 = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
					global::UnityEngine.Vector2 newOffsetMin = new global::UnityEngine.Vector2(0f, (float)(-num4 - 1) * scrollView.ItemSize);
					global::UnityEngine.Vector2 newOffsetMax = new global::UnityEngine.Vector2(0f, (float)(-num4) * scrollView.ItemSize);
					global::UnityEngine.Vector2 newAnchorMin = new global::UnityEngine.Vector2(num3 / (float)num2 * (float)num5, 1f);
					storageBuildingItemView.MoveToAnchorOffset(newAnchorMax: new global::UnityEngine.Vector2(newAnchorMin.x + num6, 1f), moveTime: (!isMoveInstance) ? 0.3f : 0.0001f, newOffsetMin: newOffsetMin, newOffsetMax: newOffsetMax, newAnchorMin: newAnchorMin);
					num++;
				}
			}
			scrollView.SetupScrollView(num2, global::Kampai.UI.View.KampaiScrollView.MoveDirection.Start);
		}

		internal void EnableMarketplace(bool isEnabled)
		{
			BuyGrayImage.gameObject.SetActive(!isEnabled);
			SellGrayImage.gameObject.SetActive(!isEnabled);
			BuyBackgroundImage.gameObject.SetActive(isEnabled);
			SellBackgroundImage.gameObject.SetActive(isEnabled);
		}

		internal void LoadSellMarketplacePanel()
		{
			SellPanel = AddMarketplacePanel<global::Kampai.UI.View.SellPanelView>("cmp_marketPlaceSellPanel");
		}

		internal void LoadBuyMarketplacePanel()
		{
			BuyPanel = AddMarketplacePanel<global::Kampai.UI.View.BuyMarketplacePanelView>("cmp_marketPlaceBuyPanel");
		}

		private T AddMarketplacePanel<T>(string prefabName) where T : global::UnityEngine.MonoBehaviour, global::strange.extensions.mediation.api.IView
		{
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load(prefabName) as global::UnityEngine.GameObject;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original, ItemsPanel, false);
			if (gameObject == null)
			{
				return (T)null;
			}
			return gameObject.GetComponent<T>();
		}

		internal void DisableExpandButton()
		{
			UpgradeButtonView.gameObject.SetActive(false);
		}

		internal void UpdateProgressBar()
		{
			float progressBar = (float)currentItemCount / (float)currentCapacity;
			SetProgressBar(progressBar);
			if (backgroundAnim != null)
			{
				if (currentCapacity - currentItemCount == 0)
				{
					backgroundAnim.Play("Full");
				}
				else if (currentCapacity - currentItemCount < 10)
				{
					backgroundAnim.Play("AlmostFull");
				}
				else
				{
					backgroundAnim.Play("Init");
				}
			}
		}

		internal void SetCap(int cap)
		{
			currentCapacity = cap;
			UpdateProgressBar();
		}

		internal void SetCurrentItemCount(int itemCount)
		{
			currentItemCount = itemCount;
			UpdateProgressBar();
		}

		internal void UpdateStorageStatus(bool isStorageFull)
		{
			Capacity.text = string.Format("{0}/{1}", currentItemCount, currentCapacity);
			HighlightExpand(isStorageFull);
		}

		internal void HighlightExpand(bool isHighlighted)
		{
			HighlightButton(isHighlighted, UpgradeButtonView);
		}

		internal void HighlightSellButton(bool isHighlighted)
		{
			HighlightButton(isHighlighted, SellButtonView);
		}

		internal void HighlightBuyButton(bool isHighlighted)
		{
			HighlightButton(isHighlighted, BuyButtonView);
		}

		internal void HighlightButton(bool isHighlighted, global::Kampai.UI.View.ButtonView highlightedButton)
		{
			global::UnityEngine.Animator[] componentsInChildren = highlightedButton.GetComponentsInChildren<global::UnityEngine.Animator>();
			if (isHighlighted)
			{
				global::UnityEngine.Animator[] array = componentsInChildren;
				foreach (global::UnityEngine.Animator animator in array)
				{
					animator.enabled = false;
				}
				global::UnityEngine.Vector3 originalScale = global::UnityEngine.Vector3.one;
				global::Kampai.Util.TweenUtil.Throb(highlightedButton.transform, 1.2f, 0.5f, out originalScale);
			}
			else
			{
				Go.killAllTweensWithTarget(highlightedButton.transform);
				highlightedButton.transform.localScale = global::UnityEngine.Vector3.one;
				global::UnityEngine.Animator[] array2 = componentsInChildren;
				foreach (global::UnityEngine.Animator animator2 in array2)
				{
					animator2.enabled = true;
				}
			}
		}

		internal void SetProgressBar(float ratio)
		{
			ratio = global::UnityEngine.Mathf.Clamp(ratio, 0f, 1f);
			FillImage.anchorMax = new global::UnityEngine.Vector2(ratio, FillImage.anchorMax.y);
			global::UnityEngine.RectTransform rectTransform = FillImage.transform as global::UnityEngine.RectTransform;
			if (rectTransform == null)
			{
				return;
			}
			global::UnityEngine.RectTransform rectTransform2 = CapacityPanel.transform as global::UnityEngine.RectTransform;
			if (!(rectTransform2 == null))
			{
				if (currentItemCount == 0)
				{
					global::UnityEngine.Vector2 anchorMin = (rectTransform2.anchorMax = new global::UnityEngine.Vector2(1f / (float)currentCapacity, rectTransform2.anchorMin.y));
					rectTransform2.anchorMin = anchorMin;
				}
				else
				{
					global::UnityEngine.Vector2 anchorMin2 = (rectTransform2.anchorMax = new global::UnityEngine.Vector2(ratio, rectTransform2.anchorMin.y));
					rectTransform2.anchorMin = anchorMin2;
				}
			}
		}
	}
}
