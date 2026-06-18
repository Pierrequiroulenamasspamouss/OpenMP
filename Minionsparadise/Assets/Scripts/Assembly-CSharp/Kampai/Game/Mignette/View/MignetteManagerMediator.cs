namespace Kampai.Game.Mignette.View
{
	public abstract class MignetteManagerMediator<T> : global::strange.extensions.mediation.impl.Mediator where T : global::Kampai.Game.Mignette.View.MignetteManagerView
	{
		private bool hasExited;

		private bool hasRequestedExit;

		private bool hasInitialized;

		private global::Kampai.Game.Mignette.View.MignetteBuildingViewObject mignetteBuildingViewObject;

		[Inject]
		public T view { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.IMignetteService mignetteService { get; set; }

		[Inject]
		public global::Kampai.Game.RequestStopMignetteSignal requestStopMignetteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MignetteEndedSignal mignetteEndedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StopMignetteSignal stopMignetteSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalMusicSignal musicSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayMignetteMusicSignal mignetteMusicSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ShowAndIncreaseMignetteScoreSignal showResultsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.EjectAllMinionsFromBuildingSignal ejectAllMinionsFromBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.MignetteGameModel mignetteGameModel { get; set; }

		[Inject]
		public global::Kampai.Common.ScheduleCooldownSignal scheduleCooldownSignal { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkModel networkModel { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.TemporaryMinionsService temporaryMinionsService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingChangeStateSignal buildingChangeStateSignal { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		public abstract string MusicEventName { get; }

		public override void OnRegister()
		{
			base.OnRegister();
			mignetteService.RegisterListener(OnPressHelper);
			requestStopMignetteSignal.AddListener(OnRequestStopMignette);
			mignetteEndedSignal.AddListener(OnMignetteExit);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			mignetteService.UnregisterListener(OnPressHelper);
			requestStopMignetteSignal.RemoveListener(OnRequestStopMignette);
			mignetteEndedSignal.RemoveListener(OnMignetteExit);
		}

		public virtual void Start()
		{
			mignetteMusicSignal.Dispatch(base.gameObject, MusicEventName, global::Kampai.Game.PlayMignetteMusicCommand.MusicEvent.Start);
		}

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		public virtual void Update()
		{
			if (!hasInitialized)
			{
				hasInitialized = true;
				mignetteGameModel.UsesTimerHUD = false;
				mignetteGameModel.UsesCounterHUD = false;
				if (view.MignetteBuildingObject != null)
				{
					mignetteBuildingViewObject = view.MignetteBuildingObject.GetComponent<global::Kampai.Game.Mignette.View.MignetteBuildingViewObject>();
					if (mignetteBuildingViewObject != null)
					{
						mignetteGameModel.UsesProgressHUD = mignetteBuildingViewObject.UsesProgressHUD;
						mignetteGameModel.UsesTimerHUD = mignetteBuildingViewObject.UsesTimerHUD;
						mignetteGameModel.UsesCounterHUD = mignetteBuildingViewObject.UsesCounterHUD;
					}
				}
			}
			if (!hasExited)
			{
				// In offline mode, never pause due to connection loss — the user chose to play offline.
				bool shouldPause = networkModel.isConnectionLost && !userSessionService.IsOffline;
				T val = view;
				if (val.IsPaused != shouldPause)
				{
					OnPauseStateChanged(shouldPause);
				}
				mignetteGameModel.TotalEventTime = view.TotalEventTime;
				mignetteGameModel.ElapsedTime = view.TimeElapsed;
				mignetteGameModel.TimeRemaining = view.TotalEventTime - view.TimeElapsed;
				mignetteGameModel.PercentCompleted = view.PercentCompleted;
			}
		}

		private void OnRequestStopMignette(bool showScore)
		{
			if (!hasRequestedExit)
			{
				hasRequestedExit = true;
				RequestStopMignette(showScore);
			}
		}

		protected virtual void RequestStopMignette(bool showScore)
		{
			OnMignetteExit(showScore);
		}

		private void RemoveTemporaryMinions(int taskableBuildingID)
		{
			global::Kampai.Game.TaskableBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.TaskableBuilding>(taskableBuildingID);
			global::Kampai.Game.View.BuildingManagerView component = gameContext.injectionBinder.GetInstance<global::UnityEngine.GameObject>(global::Kampai.Game.GameElement.BUILDING_MANAGER).GetComponent<global::Kampai.Game.View.BuildingManagerView>();
			global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.View.MinionObject> temporaryMinions = temporaryMinionsService.getTemporaryMinions();
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.View.MinionObject> item in temporaryMinions)
			{
				int key = item.Key;
				if (key <= -100)
				{
					component.UntrackMinion(taskableBuildingID, key, byInstanceId);
					byInstanceId.RemoveMinion(item.Key, timeService.CurrentTime());
					global::UnityEngine.Object.Destroy(item.Value.gameObject);
					temporaryMinionsService.removeTemporaryMinion(key);
				}
			}
		}

		private void OnMignetteExit(bool showScore)
		{
			if (hasExited)
			{
				return;
			}
			hasExited = true;
			bool flag = playerService.HasPurchasedMinigamePack();
			if (!showScore && !flag)
			{
				ejectAllMinionsFromBuildingSignal.Dispatch(mignetteGameModel.BuildingId);
			}
			int iD = view.MignetteBuildingObject.ID;
			if (flag)
			{
				RemoveTemporaryMinions(iD);
			}
			else
			{
				buildingChangeStateSignal.Dispatch(iD, global::Kampai.Game.BuildingState.Cooldown);
				if (mignetteGameModel.TriggerCooldownOnComplete)
				{
					scheduleCooldownSignal.Dispatch(new global::Kampai.Util.Tuple<int, bool>(mignetteGameModel.BuildingId, true), true);
				}
			}
			if (showScore)
			{
				global::System.Collections.Generic.Dictionary<string, float> dictionary = new global::System.Collections.Generic.Dictionary<string, float>();
				dictionary.Add("Cue", 1f);
				global::System.Collections.Generic.Dictionary<string, float> type = dictionary;
				musicSignal.Dispatch("Play_mignetteTally_loop_01", type);
				showResultsSignal.Dispatch();
			}
			else
			{
				mignetteMusicSignal.Dispatch(base.gameObject, MusicEventName, global::Kampai.Game.PlayMignetteMusicCommand.MusicEvent.Stop);
				stopMignetteSignal.Dispatch(hasRequestedExit);
			}
		}

		private void OnPressHelper(global::UnityEngine.Vector3 pos, int input, bool pressed)
		{
			T val = view;
			if (!val.IsPaused)
			{
				OnPress(pos, input, pressed);
			}
		}

		protected virtual void OnPress(global::UnityEngine.Vector3 pos, int input, bool pressed)
		{
		}

		private void OnPauseStateChanged(bool isPaused)
		{
			T val = view;
			val.OnMignettePause(isPaused);
			OnMignettePause(isPaused);
		}

		protected virtual void OnMignettePause(bool isPaused)
		{
		}
	}
}
