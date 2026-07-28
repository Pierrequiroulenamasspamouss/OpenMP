namespace Kampai.Game
{
	public class SpecialEventService : global::Kampai.Game.ISpecialEventService
	{
		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		public bool IsSpecialEventActive()
		{
			foreach (global::Kampai.Game.SpecialEventItemDefinition item in definitionService.GetAll<global::Kampai.Game.SpecialEventItemDefinition>())
			{
				if (item != null && item.IsActive)
				{
					global::Kampai.Game.SpecialEventItem firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.SpecialEventItem>(item.ID);
					if (firstInstanceByDefinitionId != null && !firstInstanceByDefinitionId.HasEnded)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
