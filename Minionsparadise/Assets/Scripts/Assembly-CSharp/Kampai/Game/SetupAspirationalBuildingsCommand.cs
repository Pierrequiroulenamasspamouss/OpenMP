namespace Kampai.Game
{
	public class SetupAspirationalBuildingsCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.ILandExpansionService landExpansionService { get; set; }

		[Inject]
		public global::Kampai.Game.ILandExpansionConfigService landExpansionConfigService { get; set; }

		[Inject]
		public global::Kampai.Game.CreateInventoryBuildingSignal createInventoryBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Util.ICoroutineProgressMonitor coroutineProgressMonitor { get; set; }

		public override void Execute()
		{
			coroutineProgressMonitor.StartTask(Setup(), "setup aspirational");
		}

		private bool IsBuildingInInventory(global::Kampai.Game.AspirationalBuildingDefinition aspirationalDef, global::System.Collections.Generic.HashSet<string> existingBuildings)
		{
			string key = string.Format("{0}_{1}_{2}", aspirationalDef.BuildingDefinitionID, aspirationalDef.Location.x, aspirationalDef.Location.y);
			return existingBuildings.Contains(key);
		}

		private global::System.Collections.IEnumerator Setup()
		{
			global::Kampai.Game.PurchasedLandExpansion purchasedLandExpansion = playerService.GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			
			global::System.Collections.Generic.HashSet<string> existingBuildings = new global::System.Collections.Generic.HashSet<string>();
			foreach (global::Kampai.Game.Building b in playerService.GetInstancesByType<global::Kampai.Game.Building>())
			{
				if (b != null && b.Definition != null && b.Location != null)
				{
					existingBuildings.Add(string.Format("{0}_{1}_{2}", b.Definition.ID, b.Location.x, b.Location.y));
				}
			}

			global::System.Diagnostics.Stopwatch sw = global::System.Diagnostics.Stopwatch.StartNew();

			foreach (int expansionId in landExpansionConfigService.GetExpansionIds())
			{
				if (purchasedLandExpansion.HasPurchased(expansionId))
				{
					continue;
				}
				global::Kampai.Game.LandExpansionConfig config = landExpansionConfigService.GetExpansionConfig(expansionId);
				if (config.containedAspirationalBuildings == null)
				{
					continue;
				}
				foreach (int aspirationalDefId in config.containedAspirationalBuildings)
				{
					global::Kampai.Game.AspirationalBuildingDefinition aspirationalDef = definitionService.Get<global::Kampai.Game.AspirationalBuildingDefinition>(aspirationalDefId);
					if (aspirationalDef != null && !IsBuildingInInventory(aspirationalDef, existingBuildings))
					{
						global::Kampai.Game.BuildingDefinition buildingDef = definitionService.Get<global::Kampai.Game.BuildingDefinition>(aspirationalDef.BuildingDefinitionID);
						if (buildingDef != null)
						{
							global::Kampai.Game.Building aspirationalBuilding = buildingDef.BuildBuilding();
							aspirationalBuilding.Location = new global::Kampai.Game.Location(aspirationalDef.Location.x, aspirationalDef.Location.y);
							aspirationalBuilding.SetState(global::Kampai.Game.BuildingState.Idle);
							aspirationalBuilding.ID = -aspirationalDefId;
							landExpansionService.TrackAspirationalBuilding(aspirationalDefId, aspirationalBuilding);
							createInventoryBuildingSignal.Dispatch(aspirationalBuilding, aspirationalBuilding.Location);

							if (sw.ElapsedMilliseconds > 15)
							{
								sw.Reset();
								sw.Start();
								yield return coroutineProgressMonitor.waitForNextFrame;
							}
						}
					}
				}
				if (sw.ElapsedMilliseconds > 15)
				{
					sw.Reset();
					sw.Start();
					yield return coroutineProgressMonitor.waitForNextFrame;
				}
			}
		}
	}
}
