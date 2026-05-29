namespace Kampai.UI.View
{
	public class LoadDefinitionForUICommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadDefinitionForUICommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.UI.View.BuildMenuDefinitionLoadedSignal buildMenuLoadedSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.AddStoreTabSignal addTabSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetLevelSignal setLevelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetXPSignal setXPSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetGrindCurrencySignal setGrindCurrencySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetPremiumCurrencySignal setPremiumCurrencySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetStorageCapacitySignal setStorageSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.ReconcileLevelUnlocksSignal reconcileUnlocks { get; set; }

		[Inject]
		public global::Kampai.UI.View.LoadMTXStoreSignal loadMTXStoreSignal { get; set; }

		[Inject]
		public global::Kampai.UI.ICurrencyStoreService currencyStoreService { get; set; }

		public override void Execute()
		{
			logger.EventStart("LoadDefinitionForUICommand.Execute");
			global::Kampai.Util.TimeProfiler.StartSection("unlocks");
			reconcileUnlocks.Dispatch();
			global::Kampai.Util.TimeProfiler.EndSection("unlocks");
			global::Kampai.Util.TimeProfiler.StartSection("load ui defs");
			global::Kampai.Util.TimeProfiler.StartSection("parse");
			global::Kampai.Util.TimeProfiler.StartSection("load");
			global::System.Collections.Generic.IList<global::Kampai.Game.StoreItemDefinition> all = definitionService.GetAll<global::Kampai.Game.StoreItemDefinition>();
			playerService.UpdateMinionPartyPointValues();
			global::Kampai.Util.TimeProfiler.EndSection("load");
			global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.Game.Definition>> buildMenuItems = new global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.Game.Definition>>();
			foreach (global::Kampai.Game.StoreItemDefinition item in all)
			{
				if (item == null || item.Disabled)
				{
					continue;
				}
				switch (item.Type)
				{
				case global::Kampai.Game.StoreItemType.BaseResource:
				case global::Kampai.Game.StoreItemType.Crafting:
				case global::Kampai.Game.StoreItemType.Decoration:
				case global::Kampai.Game.StoreItemType.Leisure:
				case global::Kampai.Game.StoreItemType.Special:
				case global::Kampai.Game.StoreItemType.PremiumCurrency:
				case global::Kampai.Game.StoreItemType.GrindCurrency:
				case global::Kampai.Game.StoreItemType.SalePack:
				case global::Kampai.Game.StoreItemType.Redeemable:
				case global::Kampai.Game.StoreItemType.SpecialEvent:
				case global::Kampai.Game.StoreItemType.MasterPlanLeftOvers:
				case global::Kampai.Game.StoreItemType.Connectable:
					AddBuildStoreItem(buildMenuItems, item, item.Type);
					if (item.IsFeatured)
					{
						AddBuildStoreItem(buildMenuItems, item, global::Kampai.Game.StoreItemType.Featured);
					}
					break;
				}
			}
			global::Kampai.Util.TimeProfiler.EndSection("parse");
			logger.EventStart("LoadDefinitionForUICommand.BuildMenu");
			routineRunner.StartCoroutine(WaitAFrame(buildMenuItems));
			logger.EventStop("LoadDefinitionForUICommand.Execute");
		}

		private void AddBuildStoreItem(global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.Game.Definition>> buildMenuItems, global::Kampai.Game.StoreItemDefinition storeItemDef, global::Kampai.Game.StoreItemType type)
		{
			if (!buildMenuItems.ContainsKey(type))
			{
				buildMenuItems[type] = new global::System.Collections.Generic.List<global::Kampai.Game.Definition>();
			}
			if (storeItemDef.OnlyShowIfInInventory || storeItemDef.OnlyShowIfOwned)
			{
				buildMenuItems[type].Insert(0, storeItemDef);
			}
			else
			{
				buildMenuItems[type].Add(storeItemDef);
			}
		}

		public static string LocaleForType(global::Kampai.Game.StoreItemType type)
		{
			string result = string.Empty;
			switch (type)
			{
			case global::Kampai.Game.StoreItemType.BaseResource:
				result = "MakeStuff";
				break;
			case global::Kampai.Game.StoreItemType.Crafting:
				result = "MixStuff";
				break;
			case global::Kampai.Game.StoreItemType.Decoration:
				result = "DecorateStuff";
				break;
			case global::Kampai.Game.StoreItemType.Leisure:
				result = "LeisureStuff";
				break;
			case global::Kampai.Game.StoreItemType.Special:
				result = "OtherStuff";
				break;
			case global::Kampai.Game.StoreItemType.Featured:
				result = "FeaturedStuff";
				break;
			case global::Kampai.Game.StoreItemType.SpecialEvent:
				result = "SpecialEventStuff";
				break;
			case global::Kampai.Game.StoreItemType.MasterPlanLeftOvers:
				result = "SpecialItems";
				break;
			case global::Kampai.Game.StoreItemType.Connectable:
				result = "ConnectableStuff";
				break;
			case global::Kampai.Game.StoreItemType.PremiumCurrency:
			case global::Kampai.Game.StoreItemType.GrindCurrency:
				result = "Currencies";
				break;
			case global::Kampai.Game.StoreItemType.SalePack:
				result = "SalePacks";
				break;
			case global::Kampai.Game.StoreItemType.Redeemable:
				result = "Redeemables";
				break;
			}
			return result;
		}

		private global::System.Collections.IEnumerator WaitAFrame(global::System.Collections.Generic.Dictionary<global::Kampai.Game.StoreItemType, global::System.Collections.Generic.List<global::Kampai.Game.Definition>> buildMenuItems)
		{
			yield return null;
			global::Kampai.Util.TimeProfiler.StartSection("signals");
			global::System.Collections.Generic.HashSet<string> addedTitles = new global::System.Collections.Generic.HashSet<string>();
			global::System.Collections.Generic.HashSet<global::Kampai.Game.StoreItemType> addedTypes = new global::System.Collections.Generic.HashSet<global::Kampai.Game.StoreItemType>();

			foreach (int typeValue in global::System.Enum.GetValues(typeof(global::Kampai.Game.StoreItemType)))
			{
				global::Kampai.Game.StoreItemType type = (global::Kampai.Game.StoreItemType)typeValue;
				
				// Whitelist: only include these types
				bool isWhitelisted = 
					type == global::Kampai.Game.StoreItemType.BaseResource ||
					type == global::Kampai.Game.StoreItemType.Crafting ||
					type == global::Kampai.Game.StoreItemType.Decoration ||
					type == global::Kampai.Game.StoreItemType.Leisure ||
					type == global::Kampai.Game.StoreItemType.MasterPlanLeftOvers ||
					type == global::Kampai.Game.StoreItemType.Connectable;

				if (isWhitelisted && buildMenuItems.ContainsKey(type))
				{
					string title = localService.GetString(LocaleForType(type));
                    if (!addedTitles.Contains(title) && !addedTypes.Contains(type))
                    {
						addTabSignal.Dispatch(new global::Kampai.UI.View.StoreTab(title, type));
						addedTitles.Add(title);
                        addedTypes.Add(type);
                    }
				}
			}

			if (buildMenuItems.ContainsKey(global::Kampai.Game.StoreItemType.SpecialEvent))
			{
				string title = localService.GetString(LocaleForType(global::Kampai.Game.StoreItemType.SpecialEvent));
				if (!addedTitles.Contains(title) && !addedTypes.Contains(global::Kampai.Game.StoreItemType.SpecialEvent))
				{
					addTabSignal.Dispatch(new global::Kampai.UI.View.StoreTab(title, global::Kampai.Game.StoreItemType.SpecialEvent));
					addedTitles.Add(title);
					addedTypes.Add(global::Kampai.Game.StoreItemType.SpecialEvent);
				}
			}

			buildMenuLoadedSignal.Dispatch(buildMenuItems);
			loadMTXStoreSignal.Dispatch();
			currencyStoreService.Initialize();
			setLevelSignal.Dispatch();
			setXPSignal.Dispatch();
			setGrindCurrencySignal.Dispatch();
			setPremiumCurrencySignal.Dispatch();
			setStorageSignal.Dispatch();
			global::Kampai.Util.TimeProfiler.EndSection("signals");
			global::Kampai.Util.TimeProfiler.EndSection("load ui defs");
			logger.EventStop("LoadDefinitionForUICommand.BuildMenu");
		}
	}
}
