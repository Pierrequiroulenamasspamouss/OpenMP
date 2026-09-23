public class EndSpecialEventCommand : global::strange.extensions.command.impl.Command
{
	public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("EndSpecialEventCommand") as global::Kampai.Util.IKampaiLogger;

	[Inject]
	public global::Kampai.Game.SpecialEventItemDefinition specialEventItemDefinition { get; set; }

	[Inject]
	public global::Kampai.Game.IPlayerService playerService { get; set; }

	[Inject]
	public global::Kampai.Game.IDefinitionService definitionService { get; set; }

	[Inject(global::Kampai.Game.GameElement.SPECIAL_EVENT_PARENT)]
	public global::UnityEngine.GameObject parent { get; set; }

	public override void Execute()
	{
		logger.Info("Ending special event {0}", specialEventItemDefinition.LocalizedKey);
		if (parent != null)
		{
			for (int i = parent.transform.childCount - 1; i >= 0; i--)
			{
				global::UnityEngine.Object.Destroy(parent.transform.GetChild(i).gameObject);
			}
		}
		CleanUpSpecialEventCharacter();
	}

	private void CleanUpSpecialEventCharacter()
	{
		global::System.Collections.Generic.ICollection<global::Kampai.Game.SpecialEventCharacter> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.SpecialEventCharacter>();
		if (instancesByType == null)
		{
			return;
		}
		global::System.Collections.Generic.List<global::Kampai.Game.Quest> instancesByType2 = playerService.GetInstancesByType<global::Kampai.Game.Quest>();
		global::System.Collections.Generic.List<global::Kampai.Game.Quest> list = new global::System.Collections.Generic.List<global::Kampai.Game.Quest>();
		if (instancesByType2 != null)
		{
			foreach (global::Kampai.Game.Quest item in instancesByType2)
			{
				if (item.Definition.SurfaceID == specialEventItemDefinition.PrestigeDefinitionID)
				{
					list.Add(item);
				}
			}
			foreach (global::Kampai.Game.Quest item2 in list)
			{
				playerService.Remove(item2);
			}
		}
		foreach (global::Kampai.Game.SpecialEventCharacter item3 in instancesByType)
		{
			if (item3.SpecialEventID == specialEventItemDefinition.ID)
			{
				playerService.Remove(item3);
			}
		}
		GrantCostume();
	}

	private int FindMinionForCostumeGrant()
	{
		int awardCostumeId = specialEventItemDefinition.AwardCostumeId;
		if (awardCostumeId > 0)
		{
			global::Kampai.Game.Minion minion = null;
			foreach (global::Kampai.Game.Minion item in playerService.GetInstancesByType<global::Kampai.Game.Minion>())
			{
				if (!item.HasPrestige)
				{
					minion = item;
					continue;
				}
				global::Kampai.Game.Prestige byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Prestige>(item.PrestigeId);
				if (byInstanceId == null || byInstanceId.Definition.ID != specialEventItemDefinition.PrestigeDefinitionID)
				{
					continue;
				}
				logger.Error("Minion {0} already has costume!", item.ID);
				minion = null;
				break;
			}
			if (minion != null)
			{
				logger.Info("Transmuting generic minion {0} to event costume ID {1}", minion.ID, specialEventItemDefinition.AwardCostumeId);
				return minion.ID;
			}
			logger.Error("Unable to satisfy costume grant");
		}
		return -1;
	}

	private void GrantCostume()
	{
		int num = FindMinionForCostumeGrant();
		if (num <= 0)
		{
			return;
		}
		global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(num);
		if (byInstanceId != null)
		{
			int prestigeDefinitionID = specialEventItemDefinition.PrestigeDefinitionID;
			global::Kampai.Game.Prestige prestige = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.Prestige>(prestigeDefinitionID);
			if (prestige == null)
			{
				global::Kampai.Game.PrestigeDefinition def = definitionService.Get<global::Kampai.Game.PrestigeDefinition>(prestigeDefinitionID);
				prestige = new global::Kampai.Game.Prestige(def);
				playerService.Add(prestige);
			}
			byInstanceId.PrestigeId = prestige.ID;
			prestige.trackedInstanceId = byInstanceId.ID;
			prestige.state = global::Kampai.Game.PrestigeState.Taskable;
		}
	}
}
