namespace Kampai.Game
{
	public class TimeEventService : global::UnityEngine.MonoBehaviour, global::Kampai.Game.ITimeEventService
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("TimeEventService") as global::Kampai.Util.IKampaiLogger;

		private global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent> timeEventList;

		private global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent> dispatchList;

		private int buffStartTime;

		private global::System.Collections.Generic.Dictionary<global::Kampai.Game.TimeEventType, float> buffMultiplier;

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetPremiumCurrencySignal setPremiumCurrencySignal { get; set; }

		public float TimerScale { get; set; }

		public TimeEventService()
		{
			timeEventList = new global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>();
			dispatchList = new global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>();
			buffMultiplier = new global::System.Collections.Generic.Dictionary<global::Kampai.Game.TimeEventType, float>();
		}

		public bool AddEvent(int instanceId, int startTime, int eventTime, global::strange.extensions.signal.impl.Signal<int> timeEventSignal, global::Kampai.Game.TimeEventType eventType = global::Kampai.Game.TimeEventType.Default)
		{
			global::Kampai.Game.TimeEvent timeEvent = new global::Kampai.Game.TimeEvent(instanceId, startTime, eventTime, eventType, timeEventSignal);
			timeEventList.Add(timeEvent);
			if (TimerScale > 0.01f)
			{
				timeEvent.eventTime = global::System.Math.Max((int)((float)timeEvent.eventTime * TimerScale), 1);
			}
			logger.Log(global::Kampai.Util.KampaiLogLevel.Info, string.Format("Add Time Event: {0}\tStartTime: {1}\tTime: {2}\tSignal: {3}", instanceId, timeService.CurrentTime(), eventTime, timeEventSignal));
			return true;
		}

		public void RushEvent(int instanceId)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent> list = new global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>();
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				if (timeEvent.instanceId == instanceId)
				{
					list.Add(timeEvent);
				}
			}
			foreach (global::Kampai.Game.TimeEvent item in list)
			{
				item.Dispatch();
				timeEventList.Remove(item);
			}
			setPremiumCurrencySignal.Dispatch();
		}

		public void RemoveEvent(int instanceId)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent> list = new global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>();
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				if (timeEvent.instanceId == instanceId)
				{
					list.Add(timeEvent);
				}
			}
			foreach (global::Kampai.Game.TimeEvent item in list)
			{
				item.ClearSignal();
				timeEventList.Remove(item);
			}
		}

		public void StartBuff(global::Kampai.Game.TimeEventType eventType, float buffMultipler, int buffStartTime)
		{
			if (!buffMultiplier.ContainsKey(eventType))
			{
				buffMultiplier.Add(eventType, buffMultipler);
			}
			else
			{
				buffMultiplier[eventType] = buffMultipler;
			}
			this.buffStartTime = buffStartTime;
		}

		public void StopBuff(global::Kampai.Game.TimeEventType eventType, int buffDuration)
		{
			if (!buffMultiplier.ContainsKey(eventType))
			{
				return;
			}
			float num = buffMultiplier[eventType] - 1f;
			if (num < 0f || buffDuration < 0)
			{
				return;
			}
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				if (timeEvent.type != eventType)
				{
					continue;
				}
				if (buffStartTime < timeEvent.startTime)
				{
					int num2 = buffDuration + buffStartTime - timeEvent.startTime;
					if (num2 < 0)
					{
						num2 = 0;
					}
					timeEvent.buffTime += (int)((float)num2 * num);
				}
				else
				{
					timeEvent.buffTime += (int)((float)buffDuration * num);
				}
			}
			buffMultiplier.Remove(eventType);
			if (buffMultiplier.Count == 0)
			{
				RemoveEvent(80000);
				buffStartTime = 0;
			}
		}

		public int GetTimeRemaining(int instanceId)
		{
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				if (timeEvent.instanceId != instanceId)
				{
					continue;
				}
				int num = 0;
				if (timeEvent.type != global::Kampai.Game.TimeEventType.Default)
				{
					if (buffStartTime == 0 || !buffMultiplier.ContainsKey(timeEvent.type))
					{
						num = timeEvent.eventTime - timeEvent.buffTime - (timeService.CurrentTime() - timeEvent.startTime);
						if (num < 0)
						{
							return 0;
						}
						return num;
					}
					int num2 = global::UnityEngine.Mathf.Max(buffStartTime, timeEvent.startTime);
					int num3 = num2 - timeEvent.startTime;
					int num4 = (int)((float)(timeService.CurrentTime() - num2) * GetMultiplier(timeEvent));
					num = timeEvent.eventTime - timeEvent.buffTime - num4 - num3;
					if (num < 0)
					{
						return 0;
					}
					return num;
				}
				num = timeEvent.eventTime - (timeService.CurrentTime() - timeEvent.startTime);
				if (num <= 0)
				{
					return 0;
				}
				return num;
			}
			if (logger != null)
			{
				logger.Warning("[TIMER_BUG] GetTimeRemaining returned -1 for instance: {0}. No event found in list of {1} events.", instanceId, timeEventList.Count);
			}
			return -1;
		}

		public int GetEventDuration(int instanceId)
		{
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				if (timeEvent.instanceId == instanceId)
				{
					return timeEvent.eventTime;
				}
			}
			return 0;
		}

		public int CalculateRushCostForTimer(int timerDurationInSecond, global::Kampai.Game.RushActionType rushActionType)
		{
			if (timerDurationInSecond <= 0)
			{
				return 0;
			}
			if (rushActionType == global::Kampai.Game.RushActionType.HARVESTING)
			{
				uint quantity = playerService.GetQuantity(global::Kampai.Game.StaticItem.FREE_RESOURCE_RUSH_THRESHOLD);
				if (timerDurationInSecond <= quantity)
				{
					return 0;
				}
			}
			global::Kampai.Game.RushTimeBandDefinition rushTimeBandForTime = definitionService.GetRushTimeBandForTime(timerDurationInSecond);
			return rushTimeBandForTime.GetCostForRushActionType(rushActionType);
		}

		public void Update()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent> list = new global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>(timeEventList);
			global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>.Enumerator enumerator = list.GetEnumerator();
			try
			{
				int currentGameTime = timeService.CurrentTime();
				while (enumerator.MoveNext())
				{
					global::Kampai.Game.TimeEvent current = enumerator.Current;
					if (IsEventExpired(current, currentGameTime))
					{
						dispatchList.Add(current);
					}
				}
			}
			finally
			{
				enumerator.Dispose();
			}
			if (dispatchList == null || dispatchList.Count <= 0)
			{
				return;
			}
			global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent> list2 = new global::System.Collections.Generic.List<global::Kampai.Game.TimeEvent>(dispatchList);
			enumerator = list2.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					global::Kampai.Game.TimeEvent current2 = enumerator.Current;
					current2.Dispatch();
					logger.Debug(string.Format("Dispatching ID: {0}", current2.instanceId));
					timeEventList.Remove(current2);
				}
				dispatchList.Clear();
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		private bool IsEventExpired(global::Kampai.Game.TimeEvent timeEvent, int currentGameTime)
		{
			if (timeEvent.type != global::Kampai.Game.TimeEventType.Default)
			{
				if (buffStartTime == 0 || !buffMultiplier.ContainsKey(timeEvent.type))
				{
					return currentGameTime - timeEvent.startTime >= timeEvent.eventTime - timeEvent.buffTime;
				}
				int num = global::UnityEngine.Mathf.Max(timeEvent.startTime, buffStartTime);
				int num2 = currentGameTime - num;
				int num3 = (int)((float)num2 * GetMultiplier(timeEvent)) + num - timeEvent.startTime;
				int num4 = timeEvent.eventTime - timeEvent.buffTime;
				return num3 >= num4;
			}
			return currentGameTime - timeEvent.startTime >= timeEvent.eventTime;
		}

		private float GetMultiplier(global::Kampai.Game.TimeEvent timeEvent)
		{
			return (!buffMultiplier.ContainsKey(timeEvent.type)) ? 0f : buffMultiplier[timeEvent.type];
		}

		public bool HasEventID(int id)
		{
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				if (timeEvent.instanceId == id)
				{
					return true;
				}
			}
			return false;
		}

		public void SpeedUpTimers(int amount)
		{
			foreach (global::Kampai.Game.TimeEvent timeEvent in timeEventList)
			{
				timeEvent.eventTime -= amount;
			}
		}
	}
}
