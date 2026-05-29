namespace Kampai.UI.View
{
	public static class StoreTabBuilder
	{
		public static global::Kampai.UI.View.StoreTabView Build(global::Kampai.UI.View.StoreTab tab, global::UnityEngine.Transform i_parent, global::Kampai.Util.IKampaiLogger logger)
		{
			if (tab == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.EX_NULL_ARG);
			}
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("cmp_MainMenuItem") as global::UnityEngine.GameObject;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
			global::Kampai.UI.View.StoreTabView component = gameObject.GetComponent<global::Kampai.UI.View.StoreTabView>();
			component.Type = tab.Type;
			component.TabIcon.maskSprite = SetTabIcon(tab.Type, logger);
			component.TabName.text = tab.LocalizedName;
			global::UnityEngine.RectTransform rectTransform = gameObject.transform as global::UnityEngine.RectTransform;
			rectTransform.SetParent(i_parent, false);
			rectTransform.SetAsFirstSibling();
			rectTransform.localPosition = new global::UnityEngine.Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
			rectTransform.localScale = global::UnityEngine.Vector3.one;
			gameObject.SetActive(false);
			return component;
		}

		public static global::UnityEngine.Sprite SetTabIcon(global::Kampai.Game.StoreItemType type, global::Kampai.Util.IKampaiLogger logger)
		{
			string text = null;
			switch (type)
			{
			case global::Kampai.Game.StoreItemType.BaseResource:
				text = "make";
				break;
			case global::Kampai.Game.StoreItemType.Crafting:
				text = "mix";
				break;
			case global::Kampai.Game.StoreItemType.Decoration:
				text = "decor";
				break;
			case global::Kampai.Game.StoreItemType.Connectable:
				text = "connectable";
				break;
			case global::Kampai.Game.StoreItemType.Leisure:
				text = "leisure";
				break;
			case global::Kampai.Game.StoreItemType.Featured:
			case global::Kampai.Game.StoreItemType.MasterPlanLeftOvers:
				text = "villainLair";
				break;
			case global::Kampai.Game.StoreItemType.SpecialEvent:
			case global::Kampai.Game.StoreItemType.SalePack:
			case global::Kampai.Game.StoreItemType.Redeemable:
				text = "event";
				break;
			case global::Kampai.Game.StoreItemType.PremiumCurrency:
			case global::Kampai.Game.StoreItemType.GrindCurrency:
				text = "villainLair";
				break;
			default:
				logger.Error("store tab key doesn't exist for StoreItemTyoe: {0}", type);
				text = "event";
				break;
			}
			return UIUtils.LoadSpriteFromPath(string.Format("icn_build_mask_cat_{0}", text));
		}
	}
}
