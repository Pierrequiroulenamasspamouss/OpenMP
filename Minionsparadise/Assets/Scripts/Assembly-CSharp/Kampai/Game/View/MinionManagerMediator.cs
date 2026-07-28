namespace Kampai.Game.View
{
	public class MinionManagerMediator : global::strange.extensions.mediation.impl.EventMediator
	{
		private bool animationsLoaded;

		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("MinionManagerMediator") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.View.MinionManagerView view { get; set; }

		[Inject]
		public global::Kampai.Game.MinionMoveToSignal minionMoveToSignal { get; set; }

		[Inject]
		public global::Kampai.Game.AddMinionSignal addMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionWalkPathSignal minionWalkPathSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionRunPathSignal minionRunPathSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionAppearSignal minionAppearSignal { get; set; }

		[Inject]
		public global::Kampai.Game.AnimateSelectedMinionSignal animateSelectedMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionStateChangeSignal stateChangeSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartMinionRouteSignal startMinionRouteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartTeleportTaskSignal startTeleportTaskSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartTaskSignal startTaskSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SignalActionSignal stopTaskSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RelocateCharacterSignal relocateSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Common.IRandomService randomService { get; set; }

		[Inject]
		public global::Kampai.Game.StartGroupGachaSignal startGroupGachaSignal { get; set; }

		[Inject]
		public global::Kampai.Common.DeselectAllMinionsSignal deselectAllMinionsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartIncidentalAnimationSignal startIncidentalAnimationSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionAcknowledgeSignal minionAcknowledgeSignal { get; set; }

		[Inject]
		public global::Kampai.Util.PathFinder pathFinder { get; set; }

		[Inject]
		public global::Kampai.Game.UpdateTaskedMinionSignal updateTaskedMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.RestoreMinionStateSignal restoreMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionReactSignal reactSignal { get; set; }

		[Inject]
		public global::Kampai.Game.EnableMinionRendererSignal enableRendererSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MoveMinionFinishedSignal moveMinionFinishedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.PlayMinionNoAnimAudioSignal playMinionNoAnimAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Game.AddMinionToTikiBarSignal addMinionTikiBarSignal { get; set; }

		[Inject]
		public global::Kampai.Game.MinionSeekPositionSignal minionSeekPositionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetPartyStatesSignal setPartyStateSignal { get; set; }

		[Inject]
		public global::Kampai.Game.TapMinionSignal tapMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.TeleportMinionsToTownForPartySignal teleportMinionsToTownSignal { get; set; }

		[Inject]
		public global::Kampai.Game.EndTownhallMinionPartyAnimationSignal endTownhallMinionPartyAnimationSignal { get; set; }

		[Inject]
		public global::Kampai.Util.ICoroutineProgressMonitor coroutineProgressMonitor { get; set; }

		[Inject]
		public global::Kampai.Game.MinionPartyAnimationSignal minionPartyAnimationSginal { get; set; }

		[Inject]
		public global::Kampai.Game.AllMinionLoadedSignal allMinionLoadedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StopMinionCampingSignal stopMinionCampingSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IPartyFavorAnimationService partyFavorService { get; set; }

		[Inject]
		public global::Kampai.Game.AddMinionsPartyFavorSignal addMinionsToPartyFavorAnimSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StartMinionPartyIntroSignal startMinionPartyIntroSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StuartShowStartSignal stuartShowStartSignal { get; set; }

		[Inject]
		public global::Kampai.Game.StuartShowCompleteSignal stuartShowCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.AddSpecificMinionPartyFavorSignal addSpecificMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IncidentalPartyFavorAnimationCompletedSignal incidentalPartyFavorAnimationCompletedSignal { get; set; }

		public override void OnRegister()
		{
			view.Init();
			coroutineProgressMonitor.StartTask(CacheAnimations(), "cache minion anims");
			SetupSignals();
			SetupMoreSignals();
			global::Kampai.Game.MinionPartyDefinition minionPartyDefinition = definitionService.Get<global::Kampai.Game.MinionPartyDefinition>(80000);
			view.SetPartyLocation(new global::Kampai.Util.Boxed<global::UnityEngine.Vector3>((global::UnityEngine.Vector3)minionPartyDefinition.Center), minionPartyDefinition.PartyRadius);
		}

		[Inject]
		public global::Kampai.Main.IAssetsPreloadService assetsPreloadService { get; set; }

		private global::System.Collections.IEnumerator CacheAnimations()
		{
			yield return null;
			while (assetsPreloadService != null && assetsPreloadService.IsPreloading)
			{
				yield return null;
			}
			global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedDefinition> weights = definitionService.GetAllGachaDefinitions();
			for (int i = 0; i < weights.Count; i++)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.Transaction.WeightedQuantityItem> gachaChoices = weights[i].Entities;
				global::System.Collections.Generic.List<global::Kampai.Game.AnimationDefinition> gachas = new global::System.Collections.Generic.List<global::Kampai.Game.AnimationDefinition>(gachaChoices.Count);
				for (int j = 0; j < gachaChoices.Count; j++)
				{
					gachas.Add(definitionService.Get<global::Kampai.Game.GachaAnimationDefinition>(gachaChoices[j].ID));
				}
				yield return view.CacheAnimationsCoroutine(gachas);
			}
			global::System.Collections.Generic.List<global::Kampai.Game.MinionAnimationDefinition> anims = definitionService.GetAll<global::Kampai.Game.MinionAnimationDefinition>();
			yield return view.CacheAnimationsCoroutine(anims);
			global::System.Collections.Generic.List<global::Kampai.Game.UIAnimationDefinition> uiAnims = definitionService.GetAll<global::Kampai.Game.UIAnimationDefinition>();
			for (int k = 0; k < uiAnims.Count; k++)
			{
				global::Kampai.Game.UIAnimationDefinition anim = uiAnims[k];
				global::Kampai.Util.KampaiResources.Load(anim.AnimationClipName, typeof(global::UnityEngine.AnimationClip));
				yield return null;
			}
			animationsLoaded = true;
		}

		private void SetupSignals()
		{
			view.idleMinionSignal.AddListener(IdleMinion);
			addMinionSignal.AddListener(AddMinion);
			minionWalkPathSignal.AddListener(WalkPath);
			minionRunPathSignal.AddListener(RunPath);
			startMinionRouteSignal.AddListener(StartMinionRoute);
			minionAppearSignal.AddListener(MinionAppear);
			animateSelectedMinionSignal.AddListener(SelectMinion);
			startGroupGachaSignal.AddListener(StartGroupGacha);
			startIncidentalAnimationSignal.AddListener(StartIncidentalAnimation);
			minionAcknowledgeSignal.AddListener(MinionAcknowledgement);
			updateTaskedMinionSignal.AddListener(UpdateTaskedMinion);
			reactSignal.AddListener(MinionReact);
			enableRendererSignal.AddListener(EnableMinionRenderer);
			startTeleportTaskSignal.AddListener(MinionTeleport);
			playMinionNoAnimAudioSignal.AddListener(PlayMinionAudio);
			minionSeekPositionSignal.AddListener(SeekPosition);
			setPartyStateSignal.AddListener(SetPartyStates);
			tapMinionSignal.AddListener(TapMinion);
			teleportMinionsToTownSignal.AddListener(TeleportMinionsToTown);
		}

		private void SetupMoreSignals()
		{
			endTownhallMinionPartyAnimationSignal.AddListener(EndTownhallMinionPartyAnimation);
			minionPartyAnimationSginal.AddListener(PlayPartyAnimation);
			allMinionLoadedSignal.AddListener(RestoreMinionParty);
			stopMinionCampingSignal.AddListener(StopMinionCamping);
			addMinionsToPartyFavorAnimSignal.AddListener(AddMinionsToPartyFavorAnimation);
			startMinionPartyIntroSignal.AddListener(ResetPartyAnimationCount);
			stuartShowStartSignal.AddListener(OnStuartConcertStart);
			stuartShowCompleteSignal.AddListener(OnStuartConcertEnd);
			incidentalPartyFavorAnimationCompletedSignal.AddListener(IncidentalPartyFavorComplete);
			addSpecificMinionSignal.AddListener(AddMinionToPartyFavorAnimation);
		}

		public override void OnRemove()
		{
			view.idleMinionSignal.RemoveListener(IdleMinion);
			addMinionSignal.RemoveListener(AddMinion);
			minionWalkPathSignal.RemoveListener(WalkPath);
			minionRunPathSignal.RemoveListener(RunPath);
			startMinionRouteSignal.RemoveListener(StartMinionRoute);
			minionAppearSignal.RemoveListener(MinionAppear);
			animateSelectedMinionSignal.RemoveListener(SelectMinion);
			startGroupGachaSignal.RemoveListener(StartGroupGacha);
			startIncidentalAnimationSignal.RemoveListener(StartIncidentalAnimation);
			minionAcknowledgeSignal.RemoveListener(MinionAcknowledgement);
			updateTaskedMinionSignal.RemoveListener(UpdateTaskedMinion);
			reactSignal.RemoveListener(MinionReact);
			enableRendererSignal.RemoveListener(EnableMinionRenderer);
			startTeleportTaskSignal.RemoveListener(MinionTeleport);
			playMinionNoAnimAudioSignal.RemoveListener(PlayMinionAudio);
			minionSeekPositionSignal.RemoveListener(SeekPosition);
			setPartyStateSignal.RemoveListener(SetPartyStates);
			tapMinionSignal.RemoveListener(TapMinion);
			teleportMinionsToTownSignal.RemoveListener(TeleportMinionsToTown);
			CleanupSignals();
		}

		private void CleanupSignals()
		{
			endTownhallMinionPartyAnimationSignal.RemoveListener(EndTownhallMinionPartyAnimation);
			minionPartyAnimationSginal.RemoveListener(PlayPartyAnimation);
			allMinionLoadedSignal.RemoveListener(RestoreMinionParty);
			stopMinionCampingSignal.RemoveListener(StopMinionCamping);
			addMinionsToPartyFavorAnimSignal.RemoveListener(AddMinionsToPartyFavorAnimation);
			startMinionPartyIntroSignal.RemoveListener(ResetPartyAnimationCount);
			stuartShowStartSignal.RemoveListener(OnStuartConcertStart);
			stuartShowCompleteSignal.RemoveListener(OnStuartConcertEnd);
			incidentalPartyFavorAnimationCompletedSignal.RemoveListener(IncidentalPartyFavorComplete);
			addSpecificMinionSignal.RemoveListener(AddMinionToPartyFavorAnimation);
		}

		private void AddMinionsToPartyFavorAnimation(int partyFavorId)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Minion>();
			for (int i = 0; i < instancesByType.Count; i++)
			{
				global::Kampai.Game.Minion minion = instancesByType[i];
				global::Kampai.Game.View.MinionObject minionObject = view.GetMinionObject(minion);
				if (!(minionObject == null) && !minion.IsDoingPartyFavorAnimation && minion.IsInMinionParty)
				{
					minion.IsDoingPartyFavorAnimation = true;
					partyFavorService.AddMinionsToPartyFavor(partyFavorId, minionObject);
					break;
				}
			}
		}

		private void AddMinionToPartyFavorAnimation(int partyFavorId, int minionID)
		{
			global::Kampai.Game.View.MinionObject minionObjectByID = view.GetMinionObjectByID(minionID);
			partyFavorService.AddMinionsToPartyFavor(partyFavorId, minionObjectByID);
		}

		private void StartGroupGacha(global::Kampai.Game.MinionAnimationInstructions instructions)
		{
			global::System.Collections.Generic.HashSet<int> minionIds = instructions.MinionIds;
			int count = minionIds.Count;
			global::Kampai.Game.Transaction.WeightedDefinition gachaWeightsForNumMinions = definitionService.GetGachaWeightsForNumMinions(count, instructions.Party);
			global::Kampai.Game.Transaction.WeightedInstance weightedInstance = playerService.GetWeightedInstance(gachaWeightsForNumMinions.ID);
			global::Kampai.Util.QuantityItem quantityItem = weightedInstance.NextPick(randomService);
			if (quantityItem.ID > 0)
			{
				global::Kampai.Game.GachaAnimationDefinition gachaPick = definitionService.Get<global::Kampai.Game.GachaAnimationDefinition>(quantityItem.ID);
				view.StartGroupGacha(gachaPick, minionIds, instructions.Center.Value, pathFinder);
			}
			else
			{
				deselectAllMinionsSignal.Dispatch();
			}
		}

		private void AddMinion(global::Kampai.Game.View.MinionObject minionObj)
		{
			view.Add(minionObj);
			restoreMinionSignal.Dispatch(minionObj.ID);
		}

		private void WalkPath(int minionID, global::System.Collections.Generic.IList<global::UnityEngine.Vector3> path, float speed, bool muteStatus)
		{
			view.StartPathing(minionID, path, speed, muteStatus, moveMinionFinishedSignal);
		}

		private void RunPath(int minionID, global::System.Collections.Generic.IList<global::UnityEngine.Vector3> path, float timeout, bool muteStatus)
		{
			view.StartPathing(minionID, path, 4.5f, muteStatus, moveMinionFinishedSignal);
		}

		private void StartMinionRoute(global::Kampai.Game.View.RouteInstructions routing)
		{
			global::Kampai.Game.TaskableBuilding taskableBuilding = routing.TargetBuilding as global::Kampai.Game.TaskableBuilding;
			if (taskableBuilding == null)
			{
				logger.Error("Trying to task a minion to a no-taskable building.");
			}
			view.StartMinionTask(routing.minion, taskableBuilding, startTaskSignal, stopTaskSignal, relocateSignal, routing.Path, routing.Rotation);
		}

		private void PlayMinionAudio(int MinionID, string audioEvent)
		{
			view.playMinionAudio(MinionID, audioEvent);
		}

		private void EnableMinionRenderer(int minionID, bool enable)
		{
			view.EnableRenderer(minionID, enable);
		}

		private void MinionTeleport(global::Kampai.Game.Minion minion, global::Kampai.Game.TaskableBuilding building)
		{
			view.TeleportMinionTask(minion, building, startTaskSignal, stopTaskSignal, relocateSignal);
		}

		private void UpdateTaskedMinion(int minionID, global::Kampai.Game.View.MinionTaskInfo taskInfo)
		{
			view.UpdateTaskedMinion(minionID, taskInfo);
		}

		private void MinionAppear(int minionID, global::UnityEngine.Vector3 pos)
		{
			view.MinionAppear(minionID, pos);
		}

		private void StopMinionCamping()
		{
			view.StopMinionCamping();
		}

		private void SelectMinion(global::Kampai.Game.SelectMinionState state)
		{
			global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = null;
			if (state.triggerIncidentalAnimation)
			{
				gachaAnimationDefinition = GetNextGacha(false);
			}
			global::Kampai.Game.MinionAnimationDefinition minionAnimDef = null;
			if (gachaAnimationDefinition != null)
			{
				minionAnimDef = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(gachaAnimationDefinition.AnimationID);
			}
			view.SelectMinion(state.minionID, minionAnimDef, state.runLocation, minionMoveToSignal, state.muteStatus);
		}

		private void TapMinion(int minionID)
		{
			global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(minionID);
			if (byInstanceId == null)
			{
				logger.Debug("MinionManagerMediator:TapMinion - KAMPAI 7668 minion was null");
			}
			else
			{
				if (byInstanceId.State != global::Kampai.Game.MinionState.Idle)
				{
					return;
				}
				global::Kampai.Game.GachaAnimationDefinition nextGacha = GetNextGacha(false);
				if (nextGacha == null)
				{
					logger.Debug("MinionManagerMediator:TapMinion - KAMPAI 7668 gacha was null");
					return;
				}
				global::Kampai.Game.MinionAnimationDefinition minionAnimationDefinition = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(nextGacha.AnimationID);
				if (minionAnimationDefinition == null)
				{
					logger.Debug("MinionManagerMediator:TapMinion - KAMPAI 7668 minionAnimDef was null");
				}
				else
				{
					view.AnimateMinion(minionID, minionAnimationDefinition, false);
				}
			}
		}

		private global::Kampai.Game.GachaAnimationDefinition GetNextGacha(bool party)
		{
			global::Kampai.Game.Transaction.WeightedInstance weightedInstance = playerService.GetWeightedInstance(definitionService.GetGachaWeightsForNumMinions(1, party).ID);
			return definitionService.Get<global::Kampai.Game.GachaAnimationDefinition>(weightedInstance.NextPick(randomService).ID);
		}

		public void IdleMinion(global::Kampai.Game.View.MinionObject minionObject)
		{
			global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(minionObject.ID);
			if (byInstanceId == null)
			{
				return;
			}
			global::Kampai.Game.MinionState state = byInstanceId.State;
			if (byInstanceId.HasPrestige)
			{
				if (!minionObject.IsSeatedInTikiBar)
				{
					global::Kampai.Game.Prestige byInstanceId2 = playerService.GetByInstanceId<global::Kampai.Game.Prestige>(byInstanceId.PrestigeId);
					global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition = playerService.GetInstancesByDefinition<global::Kampai.Game.TikiBarBuildingDefinition>();
					if (byInstanceId2 != null && byInstanceId2.state == global::Kampai.Game.PrestigeState.Questing && instancesByDefinition != null && instancesByDefinition.Count != 0)
					{
						global::Kampai.Game.TikiBarBuilding tikiBarBuilding = instancesByDefinition[0] as global::Kampai.Game.TikiBarBuilding;
						addMinionTikiBarSignal.Dispatch(tikiBarBuilding, byInstanceId, byInstanceId2, tikiBarBuilding.GetMinionSlotIndex(byInstanceId2.Definition.ID));
						return;
					}
				}
				if (state == global::Kampai.Game.MinionState.Questing)
				{
					return;
				}
			}
			if (state != global::Kampai.Game.MinionState.Selected && state != global::Kampai.Game.MinionState.Idle)
			{
				view.SetMinionMute(byInstanceId.ID, false);
				stateChangeSignal.Dispatch(byInstanceId.ID, global::Kampai.Game.MinionState.Idle);
				minionObject.Wander();
				return;
			}
			switch (state)
			{
			case global::Kampai.Game.MinionState.Idle:
				minionObject.Wander();
				break;
			case global::Kampai.Game.MinionState.Selected:
				view.SetMinionReady(byInstanceId.ID);
				break;
			}
		}

		private void StartIncidentalAnimation(int minionID, int animationDefinitionId)
		{
			view.StartMinionAnimation(minionID, definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(animationDefinitionId), true);
		}

		private void MinionAcknowledgement(int minionID, float rotateTo, int animationDefinitionId)
		{
			if (animationsLoaded)
			{
				view.MinionAcknowledgement(minionID, rotateTo, definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(animationDefinitionId));
			}
		}

		private void SeekPosition(int minionID, global::UnityEngine.Vector3 pos, float threshold)
		{
			view.SeekPosition(minionID, pos, threshold);
		}

		private void MinionReact(global::System.Collections.Generic.ICollection<int> minionIds, global::Kampai.Util.Boxed<global::UnityEngine.Vector3> buildingPos)
		{
			if (!animationsLoaded)
			{
				return;
			}
			global::Kampai.Game.Transaction.WeightedInstance weightedInstance = playerService.GetWeightedInstance(4005);
			global::Kampai.Util.QuantityItem quantityItem = weightedInstance.NextPick(randomService);
			if (quantityItem.ID > 0)
			{
				global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = definitionService.Get<global::Kampai.Game.GachaAnimationDefinition>(quantityItem.ID);
				if (gachaAnimationDefinition == null)
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Bad Gacha ID: {0}", quantityItem.ID);
				}
				else
				{
					view.MinionReact(gachaAnimationDefinition, minionIds, buildingPos);
				}
			}
		}

		private void SetPartyStates(bool gameIsStarting)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Minion>();
			for (int i = 0; i < instancesByType.Count; i++)
			{
				if (instancesByType[i].State == global::Kampai.Game.MinionState.Tasking || instancesByType[i].State == global::Kampai.Game.MinionState.Leisure)
				{
					continue;
				}
				global::Kampai.Game.MinionAnimationDefinition partyStartAnimation = null;
				if (gameIsStarting)
				{
					global::Kampai.Game.MinionPartyDefinition minionPartyDefinition = definitionService.Get<global::Kampai.Game.MinionPartyDefinition>(80000);
					if (minionPartyDefinition.PartyAnimations > 0)
					{
						global::Kampai.Game.Transaction.WeightedInstance weightedInstance = playerService.GetWeightedInstance(minionPartyDefinition.PartyAnimations);
						partyStartAnimation = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(weightedInstance.NextPick(randomService).ID);
					}
				}
				view.SetPartyState(instancesByType[i].ID, instancesByType[i].Partying, gameIsStarting, partyStartAnimation);
			}
		}

		public void RestoreMinionParty()
		{
			global::Kampai.Game.MinionPartyDefinition definition = playerService.GetMinionPartyInstance().Definition;
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				if (item.IsInMinionParty)
				{
					view.Get(item.ID).EnterMinionParty((global::UnityEngine.Vector3)definition.Center, definition.PartyRadius, definition.partyAnimationRestMin, definition.partyAnimationRestMax);
				}
			}
		}

		private void TeleportMinionsToTown()
		{
			pathFinder.ShuffleLists();
			global::Kampai.Game.MinionPartyDefinition definition = playerService.GetMinionPartyInstance().Definition;
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				item.IsInMinionParty = true;
				global::Kampai.Game.View.MinionObject minionObject = view.Get(item.ID);
				if (minionObject != null)
				{
					minionObject.EnterMinionParty((global::UnityEngine.Vector3)definition.Center, definition.PartyRadius, definition.partyAnimationRestMin, definition.partyAnimationRestMax);
					if (item.State != global::Kampai.Game.MinionState.Tasking && item.State != global::Kampai.Game.MinionState.Questing && item.State != global::Kampai.Game.MinionState.Leisure && item.State != global::Kampai.Game.MinionState.PlayingMignette)
					{
						global::UnityEngine.Vector3 pos = pathFinder.RandomPosition(item.IsInMinionParty);
						MinionAppear(item.ID, pos);
					}
				}
			}
		}

		private void OnStuartConcertStart()
		{
			global::Kampai.Game.StageBuildingDefinition stageBuildingDefinition = definitionService.Get<global::Kampai.Game.StageBuildingDefinition>(3054);
			global::Kampai.Game.Location location = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.StageBuilding>(3054).Location;
			global::System.Collections.Generic.Queue<int> minionListSortedByDistanceAndState = view.GetMinionListSortedByDistanceAndState(new global::UnityEngine.Vector3(location.x, 0f, location.y));
			int num = 0;
			foreach (int item in minionListSortedByDistanceAndState)
			{
				global::Kampai.Game.View.MinionObject minionObject = view.Get(item);
				global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(item);
				if (byInstanceId.State != global::Kampai.Game.MinionState.Tasking && byInstanceId.State != global::Kampai.Game.MinionState.Questing && byInstanceId.State != global::Kampai.Game.MinionState.Leisure && byInstanceId.State != global::Kampai.Game.MinionState.PlayingMignette)
				{
					minionObject.ClearActionQueue();
					global::UnityEngine.Vector3 stageBuildingPosition = pathFinder.GetStageBuildingPosition(num);
					MinionAppear(byInstanceId.ID, stageBuildingPosition);
					global::UnityEngine.RuntimeAnimatorController controller = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(stageBuildingDefinition.temporaryMinionASM);
					minionObject.EnqueueAction(new global::Kampai.Game.View.RotateAction(minionObject, global::UnityEngine.Camera.main.transform.eulerAngles.y, 360f, logger));
					global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
					dictionary.Add("randomizer", randomService.NextInt(stageBuildingDefinition.temporaryMinionAnimationCount));
					minionObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minionObject, controller, logger, dictionary));
					minionObject.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(minionObject, global::UnityEngine.Animator.StringToHash("Base Layer.Exit"), logger));
					num = ((!(randomService.NextFloat(0f, 1f) < stageBuildingDefinition.posSkipPercent)) ? (num + 1) : (num + 2));
					if (num >= pathFinder.GetStageBuildingCapacity())
					{
						break;
					}
				}
			}
		}

		private void OnStuartConcertEnd()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				global::Kampai.Game.View.MinionObject minionObject = view.Get(item.ID);
				if (minionObject != null)
				{
					if (item.State != global::Kampai.Game.MinionState.Tasking && item.State != global::Kampai.Game.MinionState.Questing && item.State != global::Kampai.Game.MinionState.Leisure && item.State != global::Kampai.Game.MinionState.PlayingMignette)
					{
						minionObject.ClearActionQueue();
					}
				}
				else
				{
					logger.Warning("Minion {0} was null when we tried to stop concert animation.", item.ID);
				}
			}
		}

		private void EndTownhallMinionPartyAnimation()
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Minion>();
			foreach (global::Kampai.Game.Minion item in instancesByType)
			{
				item.IsInMinionParty = false;
				item.IsDoingPartyFavorAnimation = false;
				global::Kampai.Game.View.MinionObject minionObject = view.Get(item.ID);
				if (minionObject != null)
				{
					minionObject.LeaveMinionParty();
				}
			}
		}

		private void ResetPartyAnimationCount()
		{
			view.ResetPartyAnimationCount();
		}

		private void PlayPartyAnimation(int minionID)
		{
			global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(minionID);
			global::Kampai.Game.MinionPartyDefinition definition = playerService.GetMinionPartyInstance().Definition;
			int num = 0;
			int costumeId = byInstanceId.GetCostumeId(playerService, definitionService);
			global::Kampai.Game.CostumeItemDefinition definition2 = null;
			if (definitionService.TryGet<global::Kampai.Game.CostumeItemDefinition>(costumeId, out definition2))
			{
				num = definition2.PartyAnimations;
			}
			if (num < 1)
			{
				num = definition.PartyAnimations;
			}
			global::Kampai.Game.MinionAnimationDefinition minionAnimationDefinition = null;
			if (num > 0)
			{
				global::Kampai.Game.Transaction.WeightedInstance weightedInstance = playerService.GetWeightedInstance(num);
				minionAnimationDefinition = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(weightedInstance.NextPick(randomService).ID);
			}
			if (minionAnimationDefinition != null)
			{
				view.PlayPartyAnimation(byInstanceId.ID, minionAnimationDefinition, definition.MinionsPlayingAudioCount);
			}
		}

		private void IncidentalPartyFavorComplete(int id)
		{
			partyFavorService.ReleasePartyFavor(id);
		}

		public int GetIdleMinionCount()
		{
			int num = 0;
			foreach (global::Kampai.Game.Minion item in playerService.GetInstancesByType<global::Kampai.Game.Minion>())
			{
				if (item.State == global::Kampai.Game.MinionState.Idle || item.State == global::Kampai.Game.MinionState.Selectable || item.State == global::Kampai.Game.MinionState.Selected)
				{
					num++;
				}
			}
			return num;
		}
	}
}
