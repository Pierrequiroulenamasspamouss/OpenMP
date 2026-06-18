namespace Kampai.UI.View
{
	public class CreateFunMeterCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("CreateFunMeterCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject(global::Kampai.UI.View.UIElement.HUD)]
		public global::UnityEngine.GameObject hud { get; set; }

		[Inject]
		public global::Kampai.UI.View.FinishCreateFunMeterSignal finishCreateFunMeterSignal { get; set; }

		public override void Execute()
		{
			logger.EventStart("CreateFunMeterCommand.Execute");
			bool isUnlocked = playerService.IsMinionPartyUnlocked();
			global::UnityEngine.Debug.Log("[CreateFunMeterCommand] Executing. IsMinionPartyUnlocked: " + isUnlocked);
			if (isUnlocked)
			{
				global::UnityEngine.GameObject gameObject = CreateNewXPBar();
				global::UnityEngine.Debug.Log("[CreateFunMeterCommand] Created XP Bar GameObject: " + (gameObject != null ? gameObject.name : "null"));
				
				if (hud == null)
				{
					global::UnityEngine.Debug.LogError("[CreateFunMeterCommand] HUD GameObject is null!");
					return;
				}
				
				global::Kampai.UI.View.HUDView component = hud.GetComponent<global::Kampai.UI.View.HUDView>();
				if (component == null)
				{
					global::UnityEngine.Debug.LogError("[CreateFunMeterCommand] HUDView component is null on HUD GameObject!");
					return;
				}
				
				global::UnityEngine.RectTransform pointsPanel = component.PointsPanel;
				global::UnityEngine.Debug.Log("[CreateFunMeterCommand] pointsPanel is: " + (pointsPanel != null ? pointsPanel.name : "null"));
				
				if (pointsPanel != null)
				{
					if (gameObject != null)
					{
						gameObject.transform.SetParent(pointsPanel, false);
						global::UnityEngine.Debug.Log("[CreateFunMeterCommand] Successfully set parent of XP Bar to pointsPanel.");
						finishCreateFunMeterSignal.Dispatch();
					}
					else
					{
						global::UnityEngine.Debug.LogError("[CreateFunMeterCommand] XP Bar GameObject is null, cannot set parent!");
					}
				}
				else
				{
					global::UnityEngine.Debug.LogError("[CreateFunMeterCommand] component.PointsPanel is null!");
				}
			}
			else
			{
				global::UnityEngine.Debug.LogWarning("[CreateFunMeterCommand] IsMinionPartyUnlocked is false, skipping XP bar creation.");
			}
			logger.EventStart("CreateFunMeterCommand.Execute");
		}

		private global::UnityEngine.GameObject CreateNewXPBar()
		{
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>("XP_FunMeter");
			if (gameObject == null)
			{
				global::UnityEngine.Debug.LogError("[CreateFunMeterCommand] Failed to load XP_FunMeter resource!");
				logger.Error("Invalid GUISettings.Path: {0}", "XP_FunMeter");
				return null;
			}
			global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
			if (gameObject2 == null)
			{
				global::UnityEngine.Debug.LogError("[CreateFunMeterCommand] Failed to instantiate XP_FunMeter!");
				logger.Error("Unable to create instance of {0}", "XP_FunMeter");
				return null;
			}
			global::UnityEngine.Debug.Log("[CreateFunMeterCommand] Successfully instantiated XP_FunMeter.");
			gameObject2.name = "XP_FunMeter";
			return gameObject2;
		}
	}
}
