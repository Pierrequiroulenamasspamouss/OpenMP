public class InitializeSpecialEventCommand : global::strange.extensions.command.impl.Command
{
	public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("InitializeSpecialEventCommand") as global::Kampai.Util.IKampaiLogger;

	[Inject]
	public global::Kampai.Game.IPlayerService playerService { get; set; }

	[Inject]
	public global::Kampai.Game.IDefinitionService definitionService { get; set; }

	[Inject]
	public global::Kampai.Game.StartSpecialEventSignal startSpecialEventSignal { get; set; }

	[Inject]
	public global::Kampai.Game.RestoreSpecialEventSignal restoreSpecialEventSignal { get; set; }

	[Inject]
	public global::Kampai.Game.EndSpecialEventSignal endSpecialEventSignal { get; set; }

	public override void Execute()
	{
		foreach (global::Kampai.Game.SpecialEventItemDefinition item in definitionService.GetAll<global::Kampai.Game.SpecialEventItemDefinition>())
		{
			if (!ValidateEvent(item))
			{
				break;
			}
			global::Kampai.Game.SpecialEventItem firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.SpecialEventItem>(item.ID);
			logger.Info("Resolving special event {0}:{1}", item.LocalizedKey, firstInstanceByDefinitionId);
			if (item.IsActive)
			{
				if (firstInstanceByDefinitionId != null)
				{
					if (firstInstanceByDefinitionId.HasEnded)
					{
						logger.Error("Event {0} slated to start, but has already ended!", item.ID);
						break;
					}
					restoreSpecialEventSignal.Dispatch(item);
				}
				else
				{
					playerService.Add(item.Build());
					startSpecialEventSignal.Dispatch(item);
				}
			}
			else if (firstInstanceByDefinitionId != null && !firstInstanceByDefinitionId.HasEnded)
			{
				firstInstanceByDefinitionId.HasEnded = true;
				endSpecialEventSignal.Dispatch(item);
			}
		}
	}

	private bool ValidateEvent(global::Kampai.Game.SpecialEventItemDefinition specialEvent)
	{
		return ValidateCostume(global::Kampai.Util.FatalCode.SE_INVALID_COSTUME, specialEvent.AwardCostumeId) && ValidateCostume(global::Kampai.Util.FatalCode.SE_INVALID_COSTUME, specialEvent.EventMinionCostumeId);
	}

	private bool ValidateCostume(global::Kampai.Util.FatalCode code, int costumeId)
	{
		if (costumeId > 0)
		{
			global::Kampai.Game.CostumeItemDefinition definition = null;
			if (!definitionService.TryGet<global::Kampai.Game.CostumeItemDefinition>(costumeId, out definition))
			{
				logger.Fatal(code, costumeId);
				return false;
			}
		}
		return true;
	}
}
