namespace Kampai.Game
{
	public class InitializeMarketplaceSlotsCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("InitializeMarketplaceSlotsCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.CreateMarketplaceSlotSignal createMarketplaceSlotSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CreateLockedPremiumSlotSignal createLockedPremiumSlotSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateMarketplaceSlotStateSignal updateSlotStatesSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ICoppaService coppaService { get; set; }

		public override void Execute()
		{
			//global::UnityEngine.Debug.Log("<color=green>[MARKETPLACE TRACE] InitializeMarketplaceSlotsCommand.Execute() CALLED</color>");
			global::Kampai.Game.MarketplaceDefinition marketplaceDefinition = definitionService.Get<global::Kampai.Game.MarketplaceDefinition>();
			if (marketplaceDefinition == null)
			{
				//global::UnityEngine.Debug.LogError("<color=red>[MARKETPLACE TRACE] InitializeMarketplaceSlotsCommand: MarketplaceDefinition is NULL!</color>");
				logger.Warning("MarketplaceDefinition is null");
				return;
			}
			int num = marketplaceDefinition.StandardSlots + global::System.Convert.ToInt32(playerService.GetQuantity(global::Kampai.Game.StaticItem.MARKETPLACE_ADDITIONAL_SALE_SLOTS_ID));
			//global::UnityEngine.Debug.Log(string.Format("<color=green>[MARKETPLACE TRACE] InitializeMarketplaceSlotsCommand: StandardSlots={0}, totalSlots={1}, CoppaRestricted={2}</color>", marketplaceDefinition.StandardSlots, num, coppaService.Restricted()));
			if (coppaService.Restricted())
			{
				num += marketplaceDefinition.FacebookSlots;
			}
			CreateSlots(global::Kampai.Game.MarketplaceSaleSlotDefinition.SlotType.DEFAULT, 1000008094, num);
			if (!coppaService.Restricted())
			{
				CreateSlots(global::Kampai.Game.MarketplaceSaleSlotDefinition.SlotType.FACEBOOK_UNLOCKABLE, 1000008095, marketplaceDefinition.FacebookSlots);
			}
			if (!coppaService.Restricted())
			{
				createLockedPremiumSlotSignal.Dispatch();
			}
			updateSlotStatesSignal.Dispatch();
			//global::UnityEngine.Debug.Log("<color=green>[MARKETPLACE TRACE] InitializeMarketplaceSlotsCommand: Slots created successfully.</color>");
			logger.Debug("InitializeMarketplaceSlotsCommand: Marketplace slots created.");
		}

		private void CreateSlots(global::Kampai.Game.MarketplaceSaleSlotDefinition.SlotType slotType, int slotDefinitionId, int slotCount)
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.MarketplaceSaleSlot> byDefinitionId = playerService.GetByDefinitionId<global::Kampai.Game.MarketplaceSaleSlot>(slotDefinitionId);
			while (byDefinitionId.Count < slotCount)
			{
				createMarketplaceSlotSignal.Dispatch(slotType);
				byDefinitionId = playerService.GetByDefinitionId<global::Kampai.Game.MarketplaceSaleSlot>(slotDefinitionId);
			}
		}
	}
}
