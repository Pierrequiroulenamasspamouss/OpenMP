namespace Kampai.UI.View
{
	public class BuddyBarView : global::Kampai.Util.KampaiView
	{
		public global::Kampai.UI.View.ButtonView SkrimButtonView;

		public global::UnityEngine.RectTransform ScrollView;

		public global::UnityEngine.RectTransform ScrollItemParent;

		private int rowCount;

		private global::System.Collections.Generic.IList<global::Kampai.UI.View.BuddyAvatarView> itemViews;

		private float itemWidth;

		internal void Init()
		{
			itemViews = new global::System.Collections.Generic.List<global::Kampai.UI.View.BuddyAvatarView>();
		}

		internal void SetupRowCount(int itemCount)
		{
			rowCount = ((!global::Kampai.Util.DeviceCapabilities.IsTablet()) ? 1 : 2);
			rowCount = ((itemCount <= 1) ? 1 : rowCount);
			if (rowCount == 1)
			{
				ScrollView.anchorMin = new global::UnityEngine.Vector2(ScrollView.anchorMin.x, ScrollView.anchorMax.y / 2f);
			}
		}

		internal void InitScrollView(int itemCount)
		{
			float num = itemWidth * (float)(itemCount / rowCount + ((itemCount % rowCount != 0) ? 1 : 0));
			float num2 = ScrollView.offsetMax.x - ScrollView.offsetMin.x;
			if (num <= num2)
			{
				ScrollView.offsetMin = new global::UnityEngine.Vector2(ScrollView.offsetMax.x - num, ScrollView.offsetMin.y);
				ScrollView.GetComponent<global::UnityEngine.UI.ScrollRect>().horizontal = false;
			}
			else
			{
				ScrollView.GetComponent<global::UnityEngine.UI.ScrollRect>().horizontal = true;
			}
			ScrollItemParent.offsetMin = new global::UnityEngine.Vector2(0f, 0f);
			ScrollItemParent.offsetMax = new global::UnityEngine.Vector2(num, 0f);
			base.gameObject.SetActive(true);
			SkrimButtonView.gameObject.SetActive(true);
		}

		internal void AddItem(global::Kampai.UI.View.BuddyAvatarView view, int index)
		{
			int num = index % rowCount;
			int num2 = index / rowCount;
			global::UnityEngine.RectTransform rectTransform = view.transform as global::UnityEngine.RectTransform;
			itemWidth = rectTransform.sizeDelta.x / 2f;
			rectTransform.SetParent(ScrollItemParent, false);
			rectTransform.offsetMin = new global::UnityEngine.Vector2((float)num2 * itemWidth, 0f);
			rectTransform.offsetMax = new global::UnityEngine.Vector2((float)(num2 + 1) * itemWidth, 0f);
			rectTransform.anchorMin = new global::UnityEngine.Vector2(0f, 1f / (float)rowCount * (float)num);
			rectTransform.anchorMax = new global::UnityEngine.Vector2(0f, 1f / (float)rowCount * (float)(num + 1));
			rectTransform.localPosition = new global::UnityEngine.Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
			rectTransform.localScale = global::UnityEngine.Vector3.one;
			itemViews.Add(view);
		}

		internal void Close()
		{
			base.gameObject.SetActive(false);
			SkrimButtonView.gameObject.SetActive(false);
			foreach (global::Kampai.UI.View.BuddyAvatarView itemView in itemViews)
			{
				global::UnityEngine.Object.Destroy(itemView.gameObject);
			}
			itemViews.Clear();
		}

		internal bool IsOpen()
		{
			return itemViews.Count > 0;
		}
	}
}
