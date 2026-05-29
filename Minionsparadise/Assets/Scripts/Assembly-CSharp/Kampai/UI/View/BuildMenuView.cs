namespace Kampai.UI.View
{
	public class BuildMenuView : global::Kampai.Util.KampaiView
	{
		public global::Kampai.UI.View.ButtonView MenuButton;

		public global::UnityEngine.RectTransform BackGround;

		public global::UnityEngine.GameObject Root;

		public global::Kampai.UI.View.TabMenuView TabMenu;

		public global::Kampai.UI.View.StoreBadgeView StoreBadge;

		public global::UnityEngine.RectTransform Backing;

		public global::UnityEngine.RectTransform BackingGlow;

		internal bool isOpen;

		private global::UnityEngine.Animator animator;

		internal void Init()
		{
			animator = GetComponent<global::UnityEngine.Animator>();
			MenuButton.PlaySoundOnClick = false;
			global::UnityEngine.RectTransform rectTransform = base.transform as global::UnityEngine.RectTransform;
			if (rectTransform != null)
			{
				global::UnityEngine.Vector2 anchorMin = rectTransform.anchorMin;
				global::UnityEngine.Vector2 anchorMax = rectTransform.anchorMax;
				anchorMin.y = 0f;
				anchorMax.y = 1f;
				rectTransform.anchorMin = anchorMin;
				rectTransform.anchorMax = anchorMax;
				global::UnityEngine.Vector2 offsetMin = rectTransform.offsetMin;
				global::UnityEngine.Vector2 offsetMax = rectTransform.offsetMax;
				offsetMin.y = 0f;
				offsetMax.y = 0f;
				rectTransform.offsetMin = offsetMin;
				rectTransform.offsetMax = offsetMax;
			}

			// Enforce bottom vertical anchoring and position to keep the button fully visible at the bottom of the screen
			if (MenuButton != null)
			{
				global::UnityEngine.RectTransform rt = MenuButton.transform as global::UnityEngine.RectTransform;
				if (rt != null)
				{
					global::UnityEngine.Vector2 anchorMin = rt.anchorMin;
					global::UnityEngine.Vector2 anchorMax = rt.anchorMax;
					anchorMin.y = 0f;
					anchorMax.y = 0f;
					rt.anchorMin = anchorMin;
					rt.anchorMax = anchorMax;

					global::UnityEngine.Vector2 pos = rt.anchoredPosition;
					pos.y = 50.2f;
					rt.anchoredPosition = pos;
				}
			}
			if (Backing != null)
			{
				global::UnityEngine.RectTransform rt = Backing.transform as global::UnityEngine.RectTransform;
				if (rt != null)
				{
					global::UnityEngine.Vector2 anchorMin = rt.anchorMin;
					global::UnityEngine.Vector2 anchorMax = rt.anchorMax;
					anchorMin.y = 0f;
					anchorMax.y = 0f;
					rt.anchorMin = anchorMin;
					rt.anchorMax = anchorMax;

					global::UnityEngine.Vector2 pos = rt.anchoredPosition;
					pos.y = 50.2f;
					rt.anchoredPosition = pos;
				}
			}
		}

		public void MoveMenu()
		{
			MoveMenu(!isOpen);
		}

		internal void MoveMenu(bool show)
		{
			animator.SetBool("OnOpen", show);
			isOpen = show;
			ToggleBadgeCounterVisibility(isOpen);
		}

		internal void IncreaseBadgeCounter()
		{
			StoreBadge.IncreaseBadgeCounter();
		}

		internal void ToggleBadgeCounterVisibility(bool isHide)
		{
			StoreBadge.ToggleBadgeCounterVisibility(isHide);
		}

		internal void RemoveUnlockBadge(int count)
		{
			StoreBadge.RemoveUnlockBadge(count);
		}

		internal void SetUnlockBadge(int count)
		{
			StoreBadge.SetNewUnlockCounter(count);
		}

		internal void SetBadgeCount(int count)
		{
			StoreBadge.SetBadgeCount(count);
		}

		internal void Toggle(bool show)
		{
			animator.SetBool("OnHide", !show);
		}

		internal bool IsHiding()
		{
			return animator.GetBool("OnHide");
		}

		public void SetBuildMenuButtonEnabled(bool isEnabled)
		{
			if (MenuButton != null && Backing != null && BackingGlow != null)
			{
				MenuButton.gameObject.SetActive(isEnabled);
				Backing.gameObject.SetActive(isEnabled);
				BackingGlow.gameObject.SetActive(isEnabled);
				StoreBadge.EnableBadge(isEnabled);
			}
		}

		internal void DisableBuildButton(bool disable)
		{
			MenuButton.GetComponent<global::UnityEngine.UI.Button>().enabled = !disable;
		}
	}
}
