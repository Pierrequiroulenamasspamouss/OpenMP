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

	[Inject(global::Kampai.Game.GameElement.SPECIAL_EVENT_PARENT)]
	public global::UnityEngine.GameObject specialEventParent { get; set; }

	public override void Execute()
	{
		global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] InitializeSpecialEventCommand.Execute started. Event defs count = {0}", definitionService.GetAll<global::Kampai.Game.SpecialEventItemDefinition>().Count);
		foreach (global::Kampai.Game.SpecialEventItemDefinition item in definitionService.GetAll<global::Kampai.Game.SpecialEventItemDefinition>())
		{
			bool isValid = ValidateEvent(item);
			global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] Catalog Def ID={0} ({1}): DefExists=True", item.ID, item.LocalizedKey);
			if (!isValid)
			{
				global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] Event ID={0} failed validation!", item.ID);
				break;
			}
			global::Kampai.Game.SpecialEventItem firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.SpecialEventItem>(item.ID);
			bool hasEnded = (firstInstanceByDefinitionId != null) ? firstInstanceByDefinitionId.HasEnded : false;
			bool playerActive = (firstInstanceByDefinitionId != null) && !firstInstanceByDefinitionId.HasEnded;
			global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] Player Profile Event Status: ID={0}, PlayerActive={1}, HasEnded={2}", item.ID, playerActive, hasEnded);
			if (item.IsActive)
			{
				if (firstInstanceByDefinitionId != null)
				{
					if (firstInstanceByDefinitionId.HasEnded)
					{
						global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] EVENT IS DISABLED FOR PLAYER! Event ID={0} HasEnded=True -> Disabling Special_Event parent object.", item.ID);
						if (specialEventParent != null)
						{
							foreach (global::UnityEngine.Transform child in specialEventParent.transform)
							{
								global::UnityEngine.Object.Destroy(child.gameObject);
							}
							specialEventParent.SetActive(false);
						}
						logger.Error("Event {0} slated to start, but has already ended!", item.ID);
						break;
					}
					global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] RESTORING Special Event ID={0} via restoreSpecialEventSignal", item.ID);
					restoreSpecialEventSignal.Dispatch(item);
				}
				else
				{
					global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] STARTING NEW Special Event ID={0} via startSpecialEventSignal", item.ID);
					playerService.Add(item.Build());
					startSpecialEventSignal.Dispatch(item);
				}
			}
			else if (firstInstanceByDefinitionId != null && !firstInstanceByDefinitionId.HasEnded)
			{
				global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] ENDING Special Event ID={0} because item.IsActive is false", item.ID);
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
