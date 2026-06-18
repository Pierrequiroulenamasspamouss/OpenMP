namespace Kampai.UI.View
{
	public class TabMenuView : global::Kampai.Util.KampaiView
	{
		public global::UnityEngine.UI.Text StoreTitle;

		public global::UnityEngine.RectTransform ScrollViewParent;

		private int count;

		private global::System.Collections.Generic.List<global::Kampai.UI.View.StoreTabView> tabViews;

		private global::UnityEngine.Animator animator;

		private global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, int> oldBadgeCount;

		private global::Kampai.UI.View.RemoveUnlockForBuildMenuSignal removeUnlockForBuildMenuSignal;

		private global::Kampai.UI.View.SetNewUnlockForBuildMenuSignal setNewUnlockForBuildMenuSignal;

		private float savedButtonHeight = 100f;

		private float savedPadding = 5f;

		public void Init(global::Kampai.UI.View.SetNewUnlockForBuildMenuSignal setNewUnlockForBuildMenuSignal, global::Kampai.UI.View.RemoveUnlockForBuildMenuSignal removeUnlockForBuildMenuSignal)
		{
			this.removeUnlockForBuildMenuSignal = removeUnlockForBuildMenuSignal;
			this.setNewUnlockForBuildMenuSignal = setNewUnlockForBuildMenuSignal;
			animator = base.transform.GetComponentInParent<global::UnityEngine.Animator>();
			StoreTitle.rectTransform.offsetMin = global::UnityEngine.Vector2.zero;
			StoreTitle.rectTransform.offsetMax = global::UnityEngine.Vector2.zero;
			tabViews = new global::System.Collections.Generic.List<global::Kampai.UI.View.StoreTabView>();
			oldBadgeCount = new global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, int>();
		}

		private global::Kampai.UI.View.StoreTabView GetStoreTabView(global::Kampai.Game.StoreItemType type)
		{
			foreach (global::Kampai.UI.View.StoreTabView tabView in tabViews)
			{
				if (tabView.Type == type)
				{
					return tabView;
				}
			}
			return null;
		}

		internal void SetBadgeForStoreTab(global::Kampai.Game.StoreItemType type, int badgeCount)
		{
			global::Kampai.UI.View.StoreTabView storeTabView = GetStoreTabView(type);
			if (storeTabView != null)
			{
				storeTabView.SetBadgeCount(badgeCount);
				oldBadgeCount[type] = badgeCount;
			}
		}

		internal void SetUnlockForTab(global::Kampai.Game.StoreItemType type, int badgeCount)
		{
			global::Kampai.UI.View.StoreTabView storeTabView = GetStoreTabView(type);
			if (storeTabView != null)
			{
				storeTabView.SetNewUnlockState(badgeCount);
				oldBadgeCount[type] = badgeCount;
			}
		}

		internal void ClearUnlockForTab(global::Kampai.Game.StoreItemType type)
		{
			global::Kampai.UI.View.StoreTabView storeTabView = GetStoreTabView(type);
			if (storeTabView != null)
			{
				storeTabView.SetNewUnlockState(0);
				oldBadgeCount[type] = 0;
				removeUnlockForBuildMenuSignal.Dispatch(oldBadgeCount[type]);
			}
		}

		public global::UnityEngine.GameObject GetStoreTabObject(global::Kampai.Game.StoreItemType type)
		{
			global::Kampai.UI.View.StoreTabView storeTabView = GetStoreTabView(type);
			if (storeTabView != null)
			{
				return storeTabView.gameObject;
			}
			return null;
		}

		internal void AddStoreTab(global::Kampai.UI.View.StoreTabView tabView, float buttonHeight, float padding)
		{
			tabViews.Add(tabView);
			tabView.gameObject.SetActive(true);
			if (buttonHeight > 0f)
			{
				savedButtonHeight = buttonHeight;
			}
			if (padding >= 0f)
			{
				savedPadding = padding;
			}
			LayoutTabs();
		}

		private void LayoutTabs()
		{
			if (tabViews == null)
			{
				return;
			}
			int num = 0;
			foreach (global::Kampai.UI.View.StoreTabView tabView in tabViews)
			{
				if (tabView != null && tabView.gameObject.activeSelf)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			float num2 = ((savedButtonHeight > 0f) ? savedButtonHeight : 100f);
			float num3 = ((savedPadding >= 0f) ? savedPadding : 5f);
			float num4 = 600f;
			if (ScrollViewParent != null && ScrollViewParent.parent != null)
			{
				global::UnityEngine.RectTransform rectTransform = ScrollViewParent.parent as global::UnityEngine.RectTransform;
				if (rectTransform != null && rectTransform.rect.height > 100f)
				{
					num4 = rectTransform.rect.height;
				}
			}
			else if (base.transform != null)
			{
				global::UnityEngine.RectTransform rectTransform2 = base.transform as global::UnityEngine.RectTransform;
				if (rectTransform2 != null && rectTransform2.rect.height > 100f)
				{
					num4 = rectTransform2.rect.height;
				}
			}
			float num5 = (float)num * num2 + (float)(num + 1) * num3;
			float num6 = 1f;
			if (num5 > num4)
			{
				num6 = num4 / num5;
			}
			float num7 = num2 * num6;
			float num8 = num3 * num6;
			float num9 = (float)num * num7 + (float)(num + 1) * num8;
			if (ScrollViewParent != null)
			{
				ScrollViewParent.offsetMin = new global::UnityEngine.Vector2(0f, 0f - num9);
				ScrollViewParent.offsetMax = global::UnityEngine.Vector2.zero;
			}
			int num10 = 0;
			foreach (global::Kampai.UI.View.StoreTabView tabView2 in tabViews)
			{
				if (tabView2 != null && tabView2.gameObject.activeSelf)
				{
					global::UnityEngine.RectTransform rectTransform3 = tabView2.transform as global::UnityEngine.RectTransform;
					if (rectTransform3 != null)
					{
						num10++;
						rectTransform3.offsetMin = new global::UnityEngine.Vector2(num8, (0f - num7 - num8) * (float)num10);
						rectTransform3.offsetMax = new global::UnityEngine.Vector2(0f - num8, (0f - num7 - num8) * (float)(num10 - 1) - num8);
					}
				}
			}
		}

		internal void ClearTabs()
		{
			if (tabViews != null)
			{
				foreach (global::Kampai.UI.View.StoreTabView tabView in tabViews)
				{
					if (tabView != null && tabView.gameObject != null)
					{
						global::UnityEngine.Object.Destroy(tabView.gameObject);
					}
				}
				tabViews.Clear();
			}
			count = 0;
			if (ScrollViewParent != null)
			{
				ScrollViewParent.offsetMin = global::UnityEngine.Vector2.zero;
				ScrollViewParent.offsetMax = global::UnityEngine.Vector2.zero;
			}
		}

		internal void ToggleStoreTab(global::Kampai.Game.StoreItemType type, bool show)
		{
			foreach (global::Kampai.UI.View.StoreTabView tabView in tabViews)
			{
				if (tabView.Type == type)
				{
					tabView.gameObject.SetActive(show);
					break;
				}
			}
			LayoutTabs();
		}

		internal void HideBadge(global::Kampai.Game.StoreItemType type)
		{
			SetBadgeForStoreTab(type, 0);
			if (oldBadgeCount.ContainsKey(type))
			{
				removeUnlockForBuildMenuSignal.Dispatch(oldBadgeCount[type]);
				oldBadgeCount[type] = 0;
			}
			SetUnlockForTab(type, 0);
		}

		internal void ShowMenu(bool show)
		{
			if (show)
			{
				int num = 0;
				foreach (global::System.Collections.Generic.KeyValuePair<global::Kampai.Game.StoreItemType, int> item in oldBadgeCount)
				{
					num += item.Value;
				}
				setNewUnlockForBuildMenuSignal.Dispatch(num);
			}
			animator.SetBool("OnOpenSubMenu", !show);
		}
	}
}
