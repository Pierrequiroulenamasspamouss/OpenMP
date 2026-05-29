namespace Kampai.UI.View
{
	public class SpawnDooberCommand : global::Kampai.UI.View.DooberCommand, global::strange.extensions.pool.api.IPoolable, global::Kampai.Util.IFastPooledCommand<global::UnityEngine.Vector3, global::Kampai.UI.View.DestinationType, int, bool>, global::Kampai.Util.IFastPooledCommandBase
	{
		private global::Kampai.UI.View.DestinationType type;

		private float iconWidth = 70f;

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject(global::Kampai.UI.View.UIElement.CAMERA)]
		public global::UnityEngine.Camera uiCamera { get; set; }

		[Inject(global::Kampai.UI.View.UIElement.HUD, optional = true)]
		public global::UnityEngine.GameObject hud { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistence { get; set; }

		[Inject(global::Kampai.Main.MainElement.UI_DOOBER_CANVAS)]
		public global::UnityEngine.GameObject glassCanvas { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.UI.View.PeekHUDSignal peekHUDSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.PeekStoreSignal peekStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.TokenDooberHasBeenSpawnedSignal tokenDooberHasBeenSpawnedSignal { get; set; }

		public void Execute(global::UnityEngine.Vector3 iconPos, global::Kampai.UI.View.DestinationType _type, int itemDefinitionID, bool fromWorldCanvas)
		{
			base.fromWorldCanvas = fromWorldCanvas;
			iconPosition = iconPos;
			type = _type;
			itemDefinitionId = itemDefinitionID;
			global::UnityEngine.Transform transform = glassCanvas.transform;
			global::UnityEngine.GameObject gameObject = CreateTweenObject(transform);
			global::Kampai.UI.View.KampaiImage component = gameObject.GetComponent<global::Kampai.UI.View.KampaiImage>();
			global::UnityEngine.Vector3 destination = global::UnityEngine.Vector3.zero;
			peekHUDSignal.Dispatch(3f);
			peekStoreSignal.Dispatch(3f);
			switch (type)
			{
			case global::Kampai.UI.View.DestinationType.XP:
				destination = GetXPGlassPosition();
				itemDefinitionId = 2;
				break;
			case global::Kampai.UI.View.DestinationType.GRIND:
				destination = GetGrindGlassPosition();
				itemDefinitionId = 0;
				break;
			case global::Kampai.UI.View.DestinationType.PREMIUM:
				destination = GetPremiumGlassPosition();
				itemDefinitionId = 1;
				break;
			case global::Kampai.UI.View.DestinationType.MINIONS:
				destination = GetInspirationGlassPosition();
				itemDefinitionId = 5;
				break;
			case global::Kampai.UI.View.DestinationType.STORAGE:
			case global::Kampai.UI.View.DestinationType.STORAGE_POPULATION_GOAL:
				destination = GetStorageGlassPosition();
				break;
			case global::Kampai.UI.View.DestinationType.BUFF:
				destination = GetDiscoGlassPosition();
				break;
			case global::Kampai.UI.View.DestinationType.STICKER:
				destination = GetTikiHutGlassPosition();
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.ToggleStickerbookGlowSignal>().Dispatch(true);
				localPersistence.PutDataPlayer("StickerbookGlow", "Enable");
				break;
			case global::Kampai.UI.View.DestinationType.STORE:
				destination = GetStoreGlassPosition();
				break;
			case global::Kampai.UI.View.DestinationType.MINION_LEVEL_TOKEN:
				tokenDooberHasBeenSpawnedSignal.Dispatch();
				destination = GetMinionLevelTokenGlassPosition();
				break;
			case global::Kampai.UI.View.DestinationType.MYSTERY_BOX:
				destination = DetermineMysteryDestination();
				break;
			case global::Kampai.UI.View.DestinationType.TIMER_POPULATION_GOAL:
				destination = GetMiscGlassPosition();
				break;
			}
			component.sprite = GetIconFromDefinitionID(itemDefinitionId);
			component.material = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Material>("CircleIconAlphaMaskMat");
			component.materialForRendering.renderQueue = 3001;
			component.maskSprite = GetMaskFromDefinitionId(itemDefinitionId);
			destination.z = transform.position.z;
			TweenToDestination(gameObject, destination, 1f, type);
		}

		private global::UnityEngine.Vector3 DetermineMysteryDestination()
		{
			if (itemDefinitionId == 1)
			{
				return GetPremiumGlassPosition();
			}
			return GetStorageGlassPosition();
		}

		private global::UnityEngine.Vector3 GetXPGlassPosition()
		{
			if (base.dooberModel.XPGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.XPGlassPosition = hud.GetComponent<global::Kampai.UI.View.HUDView>().PointsPanel.position;
				}
			}
			return base.dooberModel.XPGlassPosition;
		}

		private global::UnityEngine.Vector3 GetGrindGlassPosition()
		{
			if (base.dooberModel.GrindGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.GrindGlassPosition = hud.transform.Find("group_Store_Element/group_Currency_Grind/icn_Currency_Grind").position;
				}
			}
			return base.dooberModel.GrindGlassPosition;
		}

		private global::UnityEngine.Vector3 GetPremiumGlassPosition()
		{
			if (base.dooberModel.PremiumGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.PremiumGlassPosition = hud.transform.Find("group_Store_Element/group_Currency_Premium/icn_Currency_Premium").position;
				}
			}
			return base.dooberModel.PremiumGlassPosition;
		}

		private global::UnityEngine.Vector3 GetStorageGlassPosition()
		{
			if (base.dooberModel.StorageGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.StorageGlassPosition = hud.transform.Find("group_Store_Element/group_Storage/icn_Storage").position;
				}
			}
			return base.dooberModel.StorageGlassPosition;
		}

		private global::UnityEngine.Vector3 GetInspirationGlassPosition()
		{
			if (base.dooberModel.InspirationGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.InspirationGlassPosition = hud.transform.Find("PointsPanel/XP_FunMeter/Fun_Meter/screen_InspirationPanel").position;
				}
			}
			return base.dooberModel.InspirationGlassPosition;
		}

		private global::UnityEngine.Vector3 GetDiscoGlassPosition()
		{
			if (base.dooberModel.DiscoBallGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.DiscoBallGlassPosition = hud.transform.Find("panel_DiscoGlobe/dooberFlyUpPosition").position;
				}
			}
			return base.dooberModel.DiscoBallGlassPosition;
		}

		private global::UnityEngine.Vector3 GetTikiHutGlassPosition()
		{
			if (base.dooberModel.TikiBarWorldPosition == global::UnityEngine.Vector3.zero)
			{
				base.dooberModel.TikiBarWorldPosition = GetBuildingGlassPosition(313);
			}
			return uiCamera.ViewportToWorldPoint(base.dooberModel.TikiBarWorldPosition);
		}

		private global::UnityEngine.Vector3 GetStoreGlassPosition()
		{
			if (base.dooberModel.StoreGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.StoreGlassPosition = hud.transform.Find("panel_BuildMenu/img_backing").position;
				}
			}
			return base.dooberModel.StoreGlassPosition;
		}

		private global::UnityEngine.Vector3 GetMinionLevelTokenGlassPosition()
		{
			if (base.dooberModel.TokenInfoHUDPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					global::UnityEngine.RectTransform rectTransform = hud.transform.Find("panel_TokenInfo/img_Token").transform as global::UnityEngine.RectTransform;
					global::UnityEngine.Vector3 tokenInfoHUDPosition = new global::UnityEngine.Vector3(rectTransform.position.x + 4f, rectTransform.position.y, rectTransform.position.z);
					base.dooberModel.TokenInfoHUDPosition = tokenInfoHUDPosition;
				}
			}
			return base.dooberModel.TokenInfoHUDPosition;
		}

		private global::UnityEngine.Vector3 GetBuildingGlassPosition(int buildingInstanceId)
		{
			global::UnityEngine.GameObject instance = gameContext.injectionBinder.GetInstance<global::UnityEngine.GameObject>(global::Kampai.Game.GameElement.BUILDING_MANAGER);
			global::Kampai.Game.View.BuildingManagerView component = instance.GetComponent<global::Kampai.Game.View.BuildingManagerView>();
			global::Kampai.Game.View.BuildingObject buildingObject = component.GetBuildingObject(buildingInstanceId);
			global::UnityEngine.Camera cam = MainCamera;
			if (cam == null)
			{
				cam = global::UnityEngine.Camera.main;
			}
			if (cam != null)
			{
				return cam.WorldToViewportPoint(buildingObject.transform.position);
			}
			return new global::UnityEngine.Vector3(0.5f, 0.5f, 0f);
		}

		private global::UnityEngine.Vector3 GetMiscGlassPosition()
		{
			if (base.dooberModel.MiscGlassPosition == global::UnityEngine.Vector3.zero)
			{
				if (hud != null)
				{
					base.dooberModel.MiscGlassPosition = hud.transform.Find("misc_DooberLocation").position;
				}
			}
			return base.dooberModel.MiscGlassPosition;
		}

		private global::UnityEngine.GameObject CreateTweenObject(global::UnityEngine.Transform glassTransform)
		{
			global::UnityEngine.Vector2 screenStartPosition = GetScreenStartPosition();
			global::UnityEngine.GameObject original = ((type != global::Kampai.UI.View.DestinationType.MINION_LEVEL_TOKEN && type != global::Kampai.UI.View.DestinationType.MYSTERY_BOX && type != global::Kampai.UI.View.DestinationType.STORAGE_POPULATION_GOAL && type != global::Kampai.UI.View.DestinationType.TIMER_POPULATION_GOAL) ? (global::Kampai.Util.KampaiResources.Load("TweeningDoober") as global::UnityEngine.GameObject) : (global::Kampai.Util.KampaiResources.Load("MysteryBox_TweeningDoober") as global::UnityEngine.GameObject));
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
			gameObject.name = "ScreenTween Object";
			global::UnityEngine.RectTransform component = gameObject.GetComponent<global::UnityEngine.RectTransform>();
			component.anchorMin = global::UnityEngine.Vector2.zero;
			component.anchorMax = global::UnityEngine.Vector2.zero;
			gameObject.transform.SetParent(glassTransform, false);
			gameObject.transform.localPosition = global::UnityEngine.Vector3.zero;
			gameObject.transform.position = new global::UnityEngine.Vector3(gameObject.transform.position.x, gameObject.transform.position.y, glassTransform.position.z);
			component.offsetMin = new global::UnityEngine.Vector2(screenStartPosition.x - iconWidth / 2f, screenStartPosition.y - iconWidth / 2f);
			component.offsetMax = new global::UnityEngine.Vector2(screenStartPosition.x + iconWidth / 2f, screenStartPosition.y + iconWidth / 2f);
			gameObject.transform.localScale = global::UnityEngine.Vector3.one;
			return gameObject;
		}

		private global::UnityEngine.Sprite GetIconFromDefinitionID(int id)
		{
			global::Kampai.Game.DisplayableDefinition displayableDefinition = definitionService.Get<global::Kampai.Game.DisplayableDefinition>(id);
			if (displayableDefinition != null)
			{
				return UIUtils.LoadSpriteFromPath(displayableDefinition.Image);
			}
			return null;
		}

		private global::UnityEngine.Sprite GetMaskFromDefinitionId(int id)
		{
			global::Kampai.Game.DisplayableDefinition displayableDefinition = definitionService.Get<global::Kampai.Game.DisplayableDefinition>(id);
			if (displayableDefinition != null)
			{
				return UIUtils.LoadSpriteFromPath(displayableDefinition.Mask);
			}
			return null;
		}
	}
}
