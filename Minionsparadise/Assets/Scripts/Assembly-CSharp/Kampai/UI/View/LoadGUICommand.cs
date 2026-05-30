namespace Kampai.UI.View
{
	public class LoadGUICommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadGUICommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CreateFunMeterSignal createXPBar { get; set; }

		[Inject]
		public global::Kampai.UI.View.CreatePartyMeterSignal createPartyMeter { get; set; }

		public override void Execute()
		{
			logger.EventStart("LoadGUICommand.Execute");
			global::UnityEngine.Debug.Log("[LoadGUICommand] Loading screen_HUD static GUI...");
			global::UnityEngine.GameObject o = guiService.Execute(global::Kampai.UI.View.GUIOperation.LoadStatic, "screen_HUD");
			if (o == null)
			{
				global::UnityEngine.Debug.LogError("[LoadGUICommand] Failed to load screen_HUD!");
				logger.Fatal(global::Kampai.Util.FatalCode.CMD_NULL_PREFAB, "LoadGUICommand: Failed to load 'screen_HUD' prefab! Check KampaiAssetManifest and Resources folder.", new object[0]);
			}
			else
			{
				global::UnityEngine.Debug.Log("[LoadGUICommand] Successfully loaded screen_HUD: " + o.name);
			}
			base.injectionBinder.Bind<global::UnityEngine.GameObject>().ToValue(o).ToName(global::Kampai.UI.View.UIElement.HUD);
			global::UnityEngine.Debug.Log("[LoadGUICommand] Dispatching createXPBar signal.");
			createXPBar.Dispatch();
			global::UnityEngine.Debug.Log("[LoadGUICommand] Dispatching createPartyMeter signal.");
			createPartyMeter.Dispatch();
			logger.EventStart("LoadGUICommand.Execute");
		}
	}
}
