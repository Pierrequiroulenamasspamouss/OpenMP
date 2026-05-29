namespace Kampai.UI.View
{
	public class UIModel
	{
		[global::System.Flags]
		public enum UIStateFlags
		{
			StoreButtonHiddenFromQuest = 1
		}

		private struct UIStackElement
		{
			public int id;

			public global::System.Action callback;
		}

		private global::System.Collections.Generic.List<global::Kampai.UI.View.UIModel.UIStackElement> objectStack = new global::System.Collections.Generic.List<global::Kampai.UI.View.UIModel.UIStackElement>();

		public bool UIOpen
		{
			get
			{
				return objectStack.Count > 0;
			}
		}

		public bool AllowMultiTouch { get; set; }

		public bool CraftingUIOpen { get; set; }

		public bool DisableBack { get; set; }

		public bool LevelUpUIOpen { get; set; }

		public bool WelcomeBuddyOpen { get; set; }

		public bool StageUIOpen { get; set; }

		public bool LeisureMenuOpen { get; set; }

		public bool PopupAnimationIsPlaying { get; set; }

		public bool BuildingDragMode { get; set; }

		public bool GoToClicked { get; set; }

		public bool GoToInEffect { get; set; }

		public bool CaptainTeaserModalOpen { get; set; }

		public global::Kampai.UI.View.UIModel.UIStateFlags UIState { get; set; }

		private void CheckAllowMultitouch()
		{
			Kampai.Input.InputCompat.MultiTouchEnabled = !UIOpen || AllowMultiTouch;
		}

		public void AddUI(int id, global::System.Action callback)
		{
			int num = objectStack.FindIndex((global::Kampai.UI.View.UIModel.UIStackElement x) => x.id == id);
			if (num != -1)
			{
				objectStack.RemoveAt(num);
			}
			PopupAnimationIsPlaying = false;
			DisableBack = false;
			global::Kampai.UI.View.UIModel.UIStackElement item = new global::Kampai.UI.View.UIModel.UIStackElement
			{
				id = id,
				callback = callback
			};
			objectStack.Insert(0, item);
			CheckAllowMultitouch();
		}

		public void RemoveUI(int id)
		{
			int num = objectStack.FindIndex((global::Kampai.UI.View.UIModel.UIStackElement x) => x.id == id);
			if (num != -1)
			{
				objectStack.RemoveAt(num);
			}
			CheckAllowMultitouch();
		}

		public global::System.Action RemoveTopUI()
		{
			if (objectStack.Count > 0)
			{
				global::System.Action callback = objectStack[0].callback;
				objectStack.RemoveAt(0);
				CheckAllowMultitouch();
				return callback;
			}
			return null;
		}
	}
}
