namespace Kampai.Game
{
	public class StartSaleCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("StartSaleCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeEventService timeEventService { get; set; }

		[Inject]
		public global::Kampai.Game.EndSaleSignal endSaleSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateSaleBadgeSignal updateSaleBadgeSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.OpenUpSellModalSignal openUpSellModalSignal { get; set; }

		[Inject]
		public int instanceId { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		public override void Execute()
		{
			logger.Debug("Sale Started: {0}", instanceId);
			global::Kampai.Game.Sale item = playerService.GetByInstanceId<global::Kampai.Game.Sale>(instanceId);
			if (item == null)
			{
				return;
			}
			bool started = item.Started;
			item.Started = true;
			if (!item.Viewed)
			{
				updateSaleBadgeSignal.Dispatch();
			}
			item.UTCUserStartTime = timeService.CurrentTime();
			int duration = item.Definition.Duration;
			if (duration > 0)
			{
				timeEventService.AddEvent(instanceId, item.UTCUserStartTime, duration, endSaleSignal);
			}
			if (item.Definition.Type == global::Kampai.Game.SalePackType.Upsell && !started)
			{
				routineRunner.StartCoroutine(WaitAFrame(delegate
				{
					openUpSellModalSignal.Dispatch(item.Definition, "Automatic", false);
				}));
			}
		}

		private global::System.Collections.IEnumerator WaitAFrame(global::System.Action a)
		{
			yield return null;
			a();
		}
	}
}
