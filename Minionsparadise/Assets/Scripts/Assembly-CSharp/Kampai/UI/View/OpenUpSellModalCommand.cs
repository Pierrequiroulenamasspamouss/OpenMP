namespace Kampai.UI.View
{
	public class OpenUpSellModalCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("OpenUpSellModalCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject]
		public global::Kampai.Game.PackDefinition packDefinition { get; set; }

		[Inject]
		public string source { get; set; }

		[Inject]
		public bool disableSkrimButton { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CloseAllOtherMenuSignal closeSignal { get; set; }

		[Inject(optional = true)]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		public override void Execute()
		{
			if (packDefinition == null)
			{
				logger.Error("Pack Definition is null returning");
				return;
			}
			if (userSessionService != null && userSessionService.IsOffline && (source == "Automatic" || source == "Impression"))
			{
				logger.Info("[OfflineMode] Suppressed upsell modal for source: {0}", source);
				return;
			}
			SendTelemetry();
			closeSignal.Dispatch(null);
			LoadGameObject();
		}

		private void SendTelemetry()
		{
			global::Kampai.Game.SalePackDefinition salePackDefinition = packDefinition as global::Kampai.Game.SalePackDefinition;
			telemetryService.Send_Telemetry_EVT_IGE_STORE_VISIT(source, (salePackDefinition != null) ? salePackDefinition.Type.ToString() : "StoreBundle");
		}

		public global::UnityEngine.GameObject LoadGameObject()
		{
			return LoadGameObject(global::Kampai.UI.View.GUIOperation.Load);
		}

		public global::UnityEngine.GameObject LoadGameObject(global::Kampai.UI.View.GUIOperation guiOperation)
		{
			string text = SetByType();
			global::Kampai.UI.View.IGUICommand iGUICommand = guiService.BuildCommand(guiOperation, text);
			global::Kampai.UI.View.GUIArguments args = iGUICommand.Args;
			args.Add(disableSkrimButton);
			args.Add(typeof(global::Kampai.Game.PackDefinition), packDefinition);
			args.Add(text);
			return guiService.Execute(iGUICommand);
		}

		public string SetByType()
		{
			switch (packDefinition.Layout)
			{
			case global::Kampai.Game.SalePackLayout.FreeOne:
			case global::Kampai.Game.SalePackLayout.PayOne:
				return string.Format("screen_Upsell{0}Pack", 1);
			case global::Kampai.Game.SalePackLayout.FreeTwo:
			case global::Kampai.Game.SalePackLayout.PayTwo:
				return string.Format("screen_Upsell{0}Pack", 2);
			case global::Kampai.Game.SalePackLayout.FreeThree:
			case global::Kampai.Game.SalePackLayout.PayThree:
				return string.Format("screen_Upsell{0}Pack", 3);
			case global::Kampai.Game.SalePackLayout.FreeFour:
			case global::Kampai.Game.SalePackLayout.PayFour:
			case global::Kampai.Game.SalePackLayout.Starter:
				return string.Format("screen_Upsell{0}Pack", 4);
			case global::Kampai.Game.SalePackLayout.Custom:
				return packDefinition.LayoutPrefab;
			default:
			{
				int outputCount = GetOutputCount(packDefinition);
				return string.Format("screen_Upsell{0}Pack", outputCount);
			}
			}
		}

		public int GetOutputCount(global::Kampai.Game.PackDefinition packDefinition)
		{
			if (packDefinition == null || packDefinition.TransactionDefinition == null || packDefinition.TransactionDefinition.Outputs == null)
			{
				return 0;
			}
			int num = 0;
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> outputs = packDefinition.TransactionDefinition.Outputs;
			for (int i = 0; i < outputs.Count; i++)
			{
				global::Kampai.Util.QuantityItem quantityItem = outputs[i];
				if (quantityItem != null)
				{
					global::Kampai.Game.Definition definition = definitionService.Get(quantityItem.ID);
					if (definition != null && definition.ID != 2 && !(definition is global::Kampai.Game.UnlockDefinition))
					{
						num++;
					}
				}
			}
			return num;
		}
	}
}
