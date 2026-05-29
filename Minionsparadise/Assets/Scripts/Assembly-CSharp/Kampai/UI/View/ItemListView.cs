namespace Kampai.UI.View
{
	public class ItemListView : global::Kampai.Util.KampaiView
	{
		public global::Kampai.UI.View.ButtonView Title;

		public global::UnityEngine.UI.Text TitleText;

		public global::UnityEngine.RectTransform ScrollViewParent;

		public global::Kampai.UI.View.KampaiImage TabIcon;

		private global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> buttonViews;

		private float itemButtonHeight;

		private float itemPadding;

		internal global::Kampai.Game.StoreItemType currentType;

		private global::UnityEngine.Animator animator;

		public void Init()
		{
			animator = base.transform.GetComponentInParent<global::UnityEngine.Animator>();
			buttonViews = new global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>>();
		}

		internal void SetupButtonHeight(float buttonHeight, float buttonPadding)
		{
			itemButtonHeight = (buttonHeight > 0f) ? buttonHeight : 300f; // Sane default height for UI items
			itemPadding = (buttonPadding >= 0f) ? buttonPadding : 10f;    // Sane default padding
		}

		internal global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> GetAllButtonViews()
		{
			return buttonViews;
		}

		internal void AddStoreButton(global::Kampai.Game.StoreItemType type, global::Kampai.UI.View.StoreButtonView buttonView)
		{
			if (!buttonViews.ContainsKey(type))
			{
				buttonViews[type] = new global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>();
			}
			buttonViews[type].Add(buttonView);
		}

		internal global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> GetStoreButtonViews(global::Kampai.Game.StoreItemType type)
		{
			if (buttonViews.ContainsKey(type))
			{
				return buttonViews[type];
			}
			return null;
		}

		internal global::Kampai.UI.View.StoreButtonView GetStoreButtonViewByID(int ID)
		{
			foreach (global::Kampai.Game.StoreItemType key in buttonViews.Keys)
			{
				global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView> list = buttonViews[key];
				foreach (global::Kampai.UI.View.StoreButtonView item in list)
				{
					if (item.storeItemDefinition.ID == ID)
					{
						return item;
					}
				}
			}
			return null;
		}

		internal global::Kampai.Game.StoreItemType UpdateStoreButtonState(int buildingDefinitionID, bool isAddingBuilding)
		{
			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> buttonView in buttonViews)
			{
				foreach (global::Kampai.UI.View.StoreButtonView item in buttonView.Value)
				{
					if (item.definition.ID == buildingDefinitionID)
					{
						item.SetNewUnlockState(false);
						item.ChangeBuildingCount(isAddingBuilding);
						item.AdjustIncrementalCost();
						return buttonView.Key;
					}
				}
			}
			return global::Kampai.Game.StoreItemType.BaseResource;
		}

		internal bool SetupItemMenu(global::Kampai.Game.StoreItemType type, string localizedTitle)
		{
			if (buttonViews.ContainsKey(type))
			{
				if (buttonViews[type].Count == 0)
				{
					return false;
				}
				TitleText.text = localizedTitle;
				ShowAndPositionMenuItems(type);
				return true;
			}
			return false;
		}

		internal void RefreshStoreButtonLayout()
		{
			if (buttonViews.ContainsKey(currentType))
			{
				ShowAndPositionMenuItems(currentType);
			}
		}

		internal void ShowAndPositionMenuItems(global::Kampai.Game.StoreItemType type)
		{
			if (!buttonViews.ContainsKey(type))
			{
				buttonViews[type] = new global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>();
			}

			int count = buttonViews[type].Count;
			if (count == 0)
			{
				// Hide previously shown items anyway
				if (buttonViews.ContainsKey(currentType))
				{
					foreach (global::Kampai.UI.View.StoreButtonView item in buttonViews[currentType])
					{
						item.gameObject.SetActive(false);
					}
				}
				currentType = type;
				return;
			}

			foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.UI.View.StoreButtonView>> kvp in buttonViews)
			{
				foreach (global::Kampai.UI.View.StoreButtonView item2 in kvp.Value)
				{
					item2.gameObject.SetActive(false);
				}
			}

			currentType = type;
			int num = 0;
			float h = (itemButtonHeight > 0f) ? itemButtonHeight : 300f;
			float p = (itemPadding >= 0f) ? itemPadding : 10f;

			for (int i = 0; i < count; i++)
			{
				global::Kampai.UI.View.StoreButtonView storeButtonView = buttonViews[type][i];
				if (storeButtonView.ShouldBeRendered() || currentType == global::Kampai.Game.StoreItemType.Featured)
				{
					global::UnityEngine.RectTransform rectTransform = storeButtonView.transform as global::UnityEngine.RectTransform;
					rectTransform.offsetMin = new global::UnityEngine.Vector2(0f, (0f - (h + p)) * (float)num - h);
					rectTransform.offsetMax = new global::UnityEngine.Vector2(0f, (0f - (h + p)) * (float)num);
					storeButtonView.gameObject.SetActive(true);
					num++;
				}
			}
			ScrollViewParent.offsetMin = new global::UnityEngine.Vector2(0f, (float)(-num) * (h + p) + ScrollViewParent.offsetMax.y);
		}

		internal void MoveSubMenu(bool show)
		{
			animator.SetBool("OnOpenSubMenu", show);
		}
	}
}
