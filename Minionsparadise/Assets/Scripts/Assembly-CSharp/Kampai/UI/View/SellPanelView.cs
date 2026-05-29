namespace Kampai.UI.View
{
	public class SellPanelView : global::Kampai.UI.View.PopupMenuView
	{
		public global::Kampai.UI.View.ButtonView ArrowButtonView;

		public global::Kampai.UI.View.KampaiScrollView ScrollView;

		public global::Kampai.UI.View.CreateNewSellView CreateSaleView;

		internal bool isOpen;

		internal global::strange.extensions.signal.impl.Signal OnOpenPanelSignal = new global::strange.extensions.signal.impl.Signal();

		internal bool isCreateNewSalePanelOpened;

		protected override void Awake()
		{
			global::Kampai.Util.KampaiView.BubbleToContextOnAwake(this, ref currentContext, true);
		}

		internal void FadeAnimation(bool fade)
		{
			isCreateNewSalePanelOpened = fade;
			if (base.animator != null)
			{
				base.animator.Play((!fade) ? "CloseSidePanel" : "OpenSidePanel");
			}
		}

		internal void SetOpen(bool show, bool isInstant = false)
		{
			if (show)
			{
				if (isInstant)
				{
					float lastFrame = 1f;
					int defaultLayer = -1;
					OpenInstantly(defaultLayer, lastFrame);
				}
				else
				{
					Open();
				}
				OnOpenPanelSignal.Dispatch();
			}
			else
			{
				Close();
			}
			isOpen = show;
		}

		public void FadeOutItems()
		{
			FadeItems(false);
		}

		public void FadeInItems()
		{
			FadeItems(true);
		}

		private void FadeItems(bool fadeIn)
		{
			foreach (global::UnityEngine.MonoBehaviour itemView in ScrollView.ItemViewList)
			{
				global::Kampai.UI.View.StorageBuildingSaleSlotView storageBuildingSaleSlotView = itemView as global::Kampai.UI.View.StorageBuildingSaleSlotView;
				if (!(storageBuildingSaleSlotView == null))
				{
					if (fadeIn)
					{
						storageBuildingSaleSlotView.FadeIn();
					}
					else
					{
						storageBuildingSaleSlotView.FadeOut();
					}
				}
			}
		}

		internal bool HasSlot(global::Kampai.Game.MarketplaceSaleSlot slot)
		{
			foreach (global::UnityEngine.MonoBehaviour item in ScrollView)
			{
				global::Kampai.UI.View.StorageBuildingSaleSlotView storageBuildingSaleSlotView = item as global::Kampai.UI.View.StorageBuildingSaleSlotView;
				if (storageBuildingSaleSlotView == null || slot.ID != storageBuildingSaleSlotView.slotId)
				{
					continue;
				}
				return true;
			}
			return false;
		}
	}
}
