namespace Kampai.Game.View
{
	public class MinionManagerView : global::Kampai.Game.View.ObjectManagerView, global::Kampai.Game.View.MinionIdleNotifier
	{
		public class Knuckleheaddedness
		{
			private global::System.Collections.Generic.HashSet<int> wrigglers;

			private float wriggle;

			public Knuckleheaddedness(global::Kampai.Game.GachaAnimationDefinition gacha, global::System.Collections.Generic.ICollection<int> minionIds, global::Kampai.Common.IRandomService randomService)
			{
				if (gacha.knuckleheadednessInfo == null)
				{
					AssignDefaults(gacha);
				}
				wriggle = gacha.knuckleheadednessInfo.KnuckleheaddednessScale;
				float num = gacha.knuckleheadednessInfo.KnuckleheaddednessMin;
				if (!(num > 0f) || !(wriggle > 0f))
				{
					return;
				}
				int count = minionIds.Count;
				if (count > 1)
				{
					if (gacha.knuckleheadednessInfo.KnuckleheaddednessMax > num)
					{
						num += randomService.NextFloat(num, gacha.knuckleheadednessInfo.KnuckleheaddednessMax);
					}
					count = global::System.Convert.ToInt32(global::System.Math.Floor((float)count * num));
					if (count > 0)
					{
						wrigglers = new global::System.Collections.Generic.HashSet<int>();
						global::Kampai.Util.ListUtil.RandomSublist(randomService, minionIds, wrigglers, count);
					}
				}
			}

			public float DelayTime(int minionId, global::Kampai.Common.IRandomService rand)
			{
				if (wrigglers != null && wrigglers.Contains(minionId))
				{
					return rand.NextFloat(wriggle);
				}
				return 0f;
			}

			private void AssignDefaults(global::Kampai.Game.GachaAnimationDefinition gacha)
			{
				global::Kampai.Game.KnuckleheadednessInfo knuckleheadednessInfo = new global::Kampai.Game.KnuckleheadednessInfo();
				if (gacha.Minions != 0)
				{
					knuckleheadednessInfo.KnuckleheaddednessMin = 0f;
					knuckleheadednessInfo.KnuckleheaddednessMax = 0f;
					knuckleheadednessInfo.KnuckleheaddednessScale = 0f;
				}
				else
				{
					knuckleheadednessInfo.KnuckleheaddednessMin = 0.2f;
					knuckleheadednessInfo.KnuckleheaddednessMax = 1f;
					knuckleheadednessInfo.KnuckleheaddednessScale = 0.2f;
				}
				gacha.knuckleheadednessInfo = knuckleheadednessInfo;
			}
		}

		internal global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.MinionObject> idleMinionSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.MinionObject>();

		private global::System.Collections.Generic.IList<int> coordinatedAnimations = new global::System.Collections.Generic.List<int>();

		private static bool alternatePlayed;

		private global::Kampai.Util.Boxed<global::UnityEngine.Vector3> partyLocation;

		private float partyRadius;

		private global::System.Collections.Generic.IList<int> minionsPlayingAudio;

		[Inject]
		public global::Kampai.Game.View.CameraUtils cameraUtils { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService player { get; set; }

		[Inject]
		public global::Kampai.Common.IRandomService randomService { get; set; }

		[Inject]
		public global::Kampai.Game.MinionStateChangeSignal stateChangeSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal playLocalAudio { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		protected override void CacheAnimation(global::Kampai.Game.AnimationDefinition animDef)
		{
			base.CacheAnimation(animDef);
			global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = animDef as global::Kampai.Game.GachaAnimationDefinition;
			if (gachaAnimationDefinition != null && gachaAnimationDefinition.Prefab != null && !coordinatedAnimations.Contains(gachaAnimationDefinition.ID))
			{
				global::UnityEngine.Object obj = global::Kampai.Util.KampaiResources.Load(gachaAnimationDefinition.Prefab);
				if (obj == null)
				{
					logger.Fatal(global::Kampai.Util.FatalCode.AN_UNABLE_TO_LOAD_PREFAB, gachaAnimationDefinition.Prefab);
				}
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(obj) as global::UnityEngine.GameObject;
				if (gameObject == null)
				{
					logger.Fatal(global::Kampai.Util.FatalCode.AN_UNABLE_TO_LOAD_PREFAB, gachaAnimationDefinition.Prefab);
				}
				global::UnityEngine.Object.Destroy(gameObject);
				coordinatedAnimations.Add(gachaAnimationDefinition.ID);
			}
		}

		protected override string GetAnimationStateMachine(global::Kampai.Game.AnimationDefinition animDef)
		{
			string animationStateMachine = base.GetAnimationStateMachine(animDef);
			if (animationStateMachine.Length == 0)
			{
				global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = animDef as global::Kampai.Game.GachaAnimationDefinition;
				if (gachaAnimationDefinition != null)
				{
					return definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(gachaAnimationDefinition.AnimationID).StateMachine;
				}
				return string.Empty;
			}
			return animationStateMachine;
		}

		public override void Add(global::Kampai.Game.View.ActionableObject obj)
		{
			global::Kampai.Game.View.MinionObject minionObject = obj as global::Kampai.Game.View.MinionObject;
			if (minionObject == null)
			{
				logger.Error("Tried to add an ActionableObject that wasn't a MinionObject to MinionManagerView");
				return;
			}
			minionObject.SetDefaultAnimController(animationControllers["asm_minion_movement"]);
			minionObject.SetAnimController(animationControllers["asm_minion_movement"]);
			base.Add(obj);
			minionObject.EnableRenderers(false);
		}

		public new global::Kampai.Game.View.MinionObject Get(int objectId)
		{
			global::Kampai.Game.View.ActionableObject value;
			objects.TryGetValue(objectId, out value);
			return value as global::Kampai.Game.View.MinionObject;
		}

		internal void StopMinionCamping()
		{
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.View.ActionableObject> @object in objects)
			{
				global::Kampai.Game.View.ActionableObject value = @object.Value;
				if (value is global::Kampai.Game.View.MinionObject)
				{
					value.SetAnimTrigger("OnNewMinionExit");
				}
			}
		}

		internal void playMinionAudio(int minionID, string audioEvent)
		{
			global::Kampai.Game.View.ActionableObject actionableObject = objects[minionID];
			UnityEngine.Debug.Log(string.Format("[MinionManagerView] playMinionAudio: minionID={0}, audioEvent={1}, emitter={2}", 
				minionID, 
				audioEvent, 
				(actionableObject != null && actionableObject.localAudioEmitter != null) ? actionableObject.localAudioEmitter.name : "null"));
			playLocalAudio.Dispatch(actionableObject.localAudioEmitter, audioEvent, new global::System.Collections.Generic.Dictionary<string, float>());
		}

		internal void StartMinionTask(global::Kampai.Game.View.MinionObject mo, global::Kampai.Game.TaskableBuilding building, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.MinionObject, global::Kampai.Game.Building> startSignal, global::strange.extensions.signal.impl.Signal<int> stopSignal, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.CharacterObject> relocateSignal, global::System.Collections.Generic.IList<global::UnityEngine.Vector3> path, float rotation)
		{
			float speed = 4.5f;
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, animationControllers["asm_minion_movement"], logger), true);
			mo.EnqueueAction(new global::Kampai.Game.View.ConstantSpeedPathAction(mo, path, speed, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.RotateAction(mo, rotation, 720f, logger));
			AddRemoveFromBuildingActions(mo, building, mo.GetMinion(), startSignal, stopSignal, relocateSignal);
		}

		private void AddRemoveFromBuildingActions(global::Kampai.Game.View.MinionObject mo, global::Kampai.Game.TaskableBuilding building, global::Kampai.Game.Minion minion, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.MinionObject, global::Kampai.Game.Building> startSignal, global::strange.extensions.signal.impl.Signal<int> stopSignal, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.CharacterObject> relocateSignal)
		{
			global::UnityEngine.RuntimeAnimatorController controller = global::UnityEngine.Resources.Load<global::UnityEngine.RuntimeAnimatorController>(building.Definition.AnimationDefinitions[0].MinionController);
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, controller, logger));
			if (building.GetMinionsInBuilding() > building.Definition.WorkStations)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.EnableRendererAction(mo, false, logger));
			}
			mo.EnqueueAction(new global::Kampai.Game.View.MinionTaskAction(mo, building, startSignal, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.SignalAction(mo, stopSignal, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.EnableRendererAction(mo, true, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.StateChangeAction(minion.ID, stateChangeSignal, global::Kampai.Game.MinionState.Idle, logger));
			if (building is global::Kampai.Game.DebrisBuilding)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.PelvisAnimationCompleteAction(logger, mo, animationControllers["asm_minion_movement"]));
				return;
			}
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, animationControllers["asm_minion_movement"], logger));
			mo.EnqueueAction(new global::Kampai.Game.View.GotoSideWalkAction(mo, building, logger, definitionService, relocateSignal));
		}

		internal void TeleportMinionTask(global::Kampai.Game.Minion minion, global::Kampai.Game.TaskableBuilding building, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.MinionObject, global::Kampai.Game.Building> startSignal, global::strange.extensions.signal.impl.Signal<int> stopSignal, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.CharacterObject> relocateSignal)
		{
			global::Kampai.Game.View.ActionableObject actionableObject = objects[minion.ID];
			global::Kampai.Game.View.MinionObject minionObject = actionableObject as global::Kampai.Game.View.MinionObject;
			if (minionObject == null)
			{
				logger.Error("TeleportMinionTask: ao as MinionObject == null");
			}
			else
			{
				AddRemoveFromBuildingActions(minionObject, building, minion, startSignal, stopSignal, relocateSignal);
			}
		}

		internal void UpdateTaskedMinion(int minionId, global::Kampai.Game.View.MinionTaskInfo taskInfo)
		{
			global::Kampai.Game.View.ActionableObject actionableObject = objects[minionId];
			actionableObject.EnableRenderers(true);
			actionableObject.transform.position = taskInfo.Position;
			actionableObject.transform.rotation = taskInfo.Rotation;
			actionableObject.SetAnimInteger("minionPosition", taskInfo.PositionIndex);
		}

		internal void MinionAppear(int minionID, global::UnityEngine.Vector3 pos)
		{
			if (objects.ContainsKey(minionID))
			{
				objects[minionID].EnqueueAction(new global::Kampai.Game.View.AppearAction(objects[minionID], pos, logger), true);
			}
		}

		public global::Kampai.Game.View.MinionObject GetMinionObject(global::Kampai.Game.Minion minion)
		{
			return (!objects.ContainsKey(minion.ID)) ? null : (objects[minion.ID] as global::Kampai.Game.View.MinionObject);
		}

		public global::Kampai.Game.View.MinionObject GetMinionObjectByID(int minionID)
		{
			if (objects.ContainsKey(minionID))
			{
				return objects[minionID] as global::Kampai.Game.View.MinionObject;
			}
			return null;
		}

		internal void SelectMinion(int minionID, global::Kampai.Game.MinionAnimationDefinition minionAnimDef, global::Kampai.Util.Boxed<global::UnityEngine.Vector3> runLocation, global::Kampai.Game.MinionMoveToSignal minionMoveToSignal, bool muteStatus)
		{
			global::Kampai.Game.View.ActionableObject value;
			if (!objects.TryGetValue(minionID, out value))
			{
				return;
			}
			global::Kampai.Game.View.MinionObject minionObject = value as global::Kampai.Game.View.MinionObject;
			if (minionObject == null)
			{
				logger.Error("SelectMinion: ao as MinionObject == null");
				return;
			}
			if (runLocation != null && (minionObject.transform.position - runLocation.Value).sqrMagnitude < 0.0001f)
			{
				runLocation = null;
			}
			EnqueueAnimation(minionObject, minionAnimDef, muteStatus);
			minionObject.EnqueueAction(new global::Kampai.Game.View.SelectedAction(minionID, runLocation, minionMoveToSignal, logger));
			if (muteStatus)
			{
				minionObject.EnqueueAction(new global::Kampai.Game.View.MuteAction(minionObject, muteStatus, logger));
			}
		}

		public void AnimateMinion(int minionID, global::Kampai.Game.MinionAnimationDefinition gachaPick, bool muteStatus)
		{
			global::Kampai.Game.View.ActionableObject value;
			if (objects.TryGetValue(minionID, out value))
			{
				global::Kampai.Game.View.MinionObject minionObject = (global::Kampai.Game.View.MinionObject)value;
				value.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(minionObject, global::Kampai.Game.View.MinionObject.MinionGachaState.IndividualTap, logger));
				EnqueueAnimation(value, gachaPick, muteStatus);
				value.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(minionObject, global::Kampai.Game.View.MinionObject.MinionGachaState.Inactive, logger));
			}
		}

		private void EnqueueAnimation(global::Kampai.Game.View.ActionableObject mo, global::Kampai.Game.MinionAnimationDefinition gachaPick, bool muteStatus)
		{
			if (muteStatus)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.MuteAction(mo, muteStatus, logger), true);
			}
			if (gachaPick != null && !mo.IsInAnimatorState(global::UnityEngine.Animator.StringToHash("Base Layer.Init")) && !mo.IsInAnimatorState(global::UnityEngine.Animator.StringToHash("Base Layer.gacha")))
			{
				mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, animationControllers[gachaPick.StateMachine], logger, gachaPick.arguments), !muteStatus);
			}
			mo.EnqueueAction(new global::Kampai.Game.View.RotateAction(mo, global::UnityEngine.Camera.main.transform.eulerAngles.y - 180f, 360f, logger));
			if (gachaPick != null)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(mo, global::UnityEngine.Animator.StringToHash("Base Layer.Exit"), logger));
			}
		}

		internal void SetMinionMute(int minionID, bool mute)
		{
			(objects[minionID] as global::Kampai.Game.View.MinionObject).SetMuteStatus(mute);
		}

		internal void StartGroupGacha(global::Kampai.Game.GachaAnimationDefinition gachaPick, global::System.Collections.Generic.ICollection<int> minionIds, global::UnityEngine.Vector3 centerPoint, global::Kampai.Util.IPathFinder pathFinder)
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> collection = ToActionableObjects(minionIds);
			global::Kampai.Game.View.SyncAction action = new global::Kampai.Game.View.SyncAction(collection, logger);
			foreach (global::Kampai.Game.View.MinionObject item in collection)
			{
				item.EnqueueAction(new global::Kampai.Game.View.StopWalkingAction(item, logger));
				item.EnqueueAction(action);
				item.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(item, global::Kampai.Game.View.MinionObject.MinionGachaState.Active, logger));
			}
			if (coordinatedAnimations.Contains(gachaPick.ID))
			{
				SetupCoordinatedMinionGacha(base.gameObject, definitionService, gachaPick, animationControllers, collection, centerPoint, pathFinder, logger);
			}
			else
			{
				StartNonCoordinatedGacha(gachaPick, collection, minionIds);
			}
			foreach (global::Kampai.Game.View.MinionObject item2 in collection)
			{
				item2.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(item2, global::Kampai.Game.View.MinionObject.MinionGachaState.Inactive, logger));
				item2.EnqueueAction(new global::Kampai.Game.View.IncidentalFinishedAction(item2.ID, stateChangeSignal, logger));
			}
		}

		internal void StartNonCoordinatedGacha(global::Kampai.Game.GachaAnimationDefinition gachaPick, global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> actionableObjects, global::System.Collections.Generic.ICollection<int> minionIds)
		{
			StartNonCoordinatedGacha(gachaPick, actionableObjects, minionIds, null);
		}

		internal void StartNonCoordinatedGacha(global::Kampai.Game.GachaAnimationDefinition gachaPick, global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> actionableObjects, global::System.Collections.Generic.ICollection<int> minionIds, global::Kampai.Util.Boxed<global::UnityEngine.Vector3> buildingPos)
		{
			if (gachaPick == null)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Null gacha pick (CONFIG ERROR)");
				return;
			}
			alternatePlayed = false;
			global::Kampai.Game.View.MinionManagerView.Knuckleheaddedness knuckleheaddedness = new global::Kampai.Game.View.MinionManagerView.Knuckleheaddedness(gachaPick, minionIds, randomService);
			bool mute = false;
			foreach (global::Kampai.Game.View.ActionableObject actionableObject in actionableObjects)
			{
				global::Kampai.Game.View.MinionObject minionObject = actionableObject as global::Kampai.Game.View.MinionObject;
				if (!(minionObject == null))
				{
					float animationDelay = knuckleheaddedness.DelayTime(minionObject.ID, randomService);
					SetupSingleMinionGacha(randomService, definitionService, minionObject, animationControllers, gachaPick, animationDelay, buildingPos, logger, ref mute);
				}
			}
		}

		internal void SetMinionReady(int minionId)
		{
			global::Kampai.Game.View.MinionObject minionObject = objects[minionId] as global::Kampai.Game.View.MinionObject;
			global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
			dictionary.Add("isReady", true);
			global::Kampai.Util.QuantityItem quantityItem = player.GetWeightedInstance(4002).NextPick(randomService);
			minionObject.SetAnimInteger("IdleRandom", quantityItem.ID);
			minionObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minionObject, animationControllers["asm_minion_movement"], logger, dictionary));
			minionObject.EnqueueAction(new global::Kampai.Game.View.DelayAction(minionObject, 1.4f, logger));
		}

		private static int GetClosestRoutingPoint(global::UnityEngine.Vector3 subject, global::UnityEngine.Transform[] routingPoints, global::System.Collections.Generic.IList<global::UnityEngine.Transform> usedRoutingPoints)
		{
			float num = float.MaxValue;
			int result = -1;
			for (int i = 0; i < routingPoints.Length; i++)
			{
				global::UnityEngine.Transform transform = routingPoints[i];
				if (!usedRoutingPoints.Contains(transform))
				{
					float num2 = global::UnityEngine.Vector3.Distance(subject, transform.position);
					if (num2 < num)
					{
						num = num2;
						result = i;
					}
				}
			}
			return result;
		}

		private static bool RoutesAreValid(global::Kampai.Util.IPathFinder pathFinder, global::UnityEngine.Transform[] routes)
		{
			foreach (global::UnityEngine.Transform transform in routes)
			{
				if (!pathFinder.IsOccupiable(new global::Kampai.Game.Location(transform.position)))
				{
					return false;
				}
			}
			return true;
		}

		public static void SetupCoordinatedMinionGacha(global::UnityEngine.GameObject parent, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.GachaAnimationDefinition def, global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> animationControllers, global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> actionableObjects, global::UnityEngine.Vector3 centerPoint, global::Kampai.Util.IPathFinder pathFinder, global::Kampai.Util.IKampaiLogger logger)
		{
			global::Kampai.Game.View.CoordinatedAnimation coordinatedAnimation = parent.AddComponent<global::Kampai.Game.View.CoordinatedAnimation>();
			global::UnityEngine.Vector3 position = global::UnityEngine.Camera.main.transform.position;
			coordinatedAnimation.Init(def, parent.transform, centerPoint, new global::UnityEngine.Vector3(position.x, 0f, position.z), logger);
			global::UnityEngine.Transform[] routingSlots = coordinatedAnimation.GetRoutingSlots();
			if (!RoutesAreValid(pathFinder, routingSlots))
			{
				return;
			}
			int num = routingSlots.Length;
			if (num != actionableObjects.Count)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Too many minions for selected gacha");
				global::System.Collections.Generic.List<global::Kampai.Game.View.ActionableObject> list = new global::System.Collections.Generic.List<global::Kampai.Game.View.ActionableObject>();
				int num2 = 0;
				foreach (global::Kampai.Game.View.ActionableObject actionableObject in actionableObjects)
				{
					if (num2++ == num)
					{
						break;
					}
					list.Add(actionableObject);
				}
				actionableObjects = list;
			}
			global::System.Collections.Generic.IList<global::UnityEngine.Transform> list2 = new global::System.Collections.Generic.List<global::UnityEngine.Transform>();
			global::Kampai.Util.VFXScript vFXScript = coordinatedAnimation.GetVFXScript();
			global::Kampai.Game.View.DestroyObjectAction deallocateAnimationPrefab = new global::Kampai.Game.View.DestroyObjectAction(coordinatedAnimation, logger);
			global::Kampai.Game.View.SyncAction syncActionA = new global::Kampai.Game.View.SyncAction(actionableObjects, logger);
			global::Kampai.Game.View.SyncAction syncActionB = new global::Kampai.Game.View.SyncAction(actionableObjects, logger);
			global::strange.extensions.signal.impl.Signal cancelSignal = new global::Kampai.Util.ActionSignal(CancelGachaAction(actionableObjects), true);
			foreach (global::Kampai.Game.View.ActionableObject actionableObject2 in actionableObjects)
			{
				global::Kampai.Game.View.MinionObject minionObject = actionableObject2 as global::Kampai.Game.View.MinionObject;
				if (minionObject == null)
				{
					continue;
				}
				global::UnityEngine.Vector3 position2 = minionObject.transform.position;
				int closestRoutingPoint = GetClosestRoutingPoint(position2, routingSlots, list2);
				list2.Add(routingSlots[closestRoutingPoint]);
				global::UnityEngine.RuntimeAnimatorController controller = animationControllers[definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(def.AnimationID).StateMachine];
				global::System.Collections.Generic.IList<global::UnityEngine.Vector3> path = pathFinder.FindPath(minionObject.transform.position, routingSlots[closestRoutingPoint].position, 4, true);
				global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
				global::Kampai.Game.MinionAnimationDefinition minionAnimationDefinition = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(def.AnimationID);
				if (minionAnimationDefinition.arguments != null)
				{
					foreach (string key in minionAnimationDefinition.arguments.Keys)
					{
						if (!key.Equals("actor"))
						{
							dictionary.Add(key, minionAnimationDefinition.arguments[key]);
							continue;
						}
						logger.Warning("Ignoring actor attribute for {0}", minionAnimationDefinition.ID);
					}
				}
				dictionary.Add("actor", closestRoutingPoint);
				minionObject.StopLocalAudio();
				EnqueueActions(exitState: def.SoloExit ? string.Format("Base Layer.Exit_{0}", closestRoutingPoint) : "Base Layer.Exit", mo: minionObject, animationControllers: animationControllers, vfxScript: vFXScript, path: path, rotation: routingSlots[closestRoutingPoint].rotation.eulerAngles.y, controller: controller, args: dictionary, deallocateAnimationPrefab: deallocateAnimationPrefab, logger: logger, syncActionA: syncActionA, syncActionB: syncActionB, cancelSignal: cancelSignal);
			}
		}

		private static global::System.Action CancelGachaAction(global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> objs)
		{
			return delegate
			{
				foreach (global::Kampai.Game.View.ActionableObject obj in objs)
				{
					obj.ClearActionQueue();
				}
			};
		}

		private static void EnqueueActions(global::Kampai.Game.View.MinionObject mo, global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> animationControllers, global::Kampai.Util.VFXScript vfxScript, global::System.Collections.Generic.IList<global::UnityEngine.Vector3> path, float rotation, global::UnityEngine.RuntimeAnimatorController controller, global::System.Collections.Generic.Dictionary<string, object> args, global::Kampai.Game.View.DestroyObjectAction deallocateAnimationPrefab, global::Kampai.Util.IKampaiLogger logger, global::Kampai.Game.View.SyncAction syncActionA, global::Kampai.Game.View.SyncAction syncActionB, string exitState, global::strange.extensions.signal.impl.Signal cancelSignal)
		{
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, animationControllers["asm_minion_movement"], logger));
			if (vfxScript != null)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.TrackVFXAction(mo, vfxScript, logger));
			}
			mo.EnqueueAction(new global::Kampai.Game.View.CancelablePathAction(cancelSignal, 0.2f, mo, path, 0.5f, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.RotateAction(mo, rotation, 720f, logger));
			mo.EnqueueAction(syncActionA);
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, controller, logger, args));
			mo.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(mo, global::UnityEngine.Animator.StringToHash(exitState), logger));
			if (vfxScript != null)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.UntrackVFXAction(mo, logger));
			}
			mo.EnqueueAction(deallocateAnimationPrefab);
			mo.EnqueueAction(new global::Kampai.Game.View.PelvisAnimationCompleteAction(logger, mo, animationControllers["asm_minion_movement"]));
			mo.EnqueueAction(syncActionB);
		}

		public static void SetupSingleMinionGacha(global::Kampai.Common.IRandomService randomService, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.View.MinionObject mo, global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> animationControllers, global::Kampai.Game.GachaAnimationDefinition gachaPick, float animationDelay, global::Kampai.Util.Boxed<global::UnityEngine.Vector3> buildingPos, global::Kampai.Util.IKampaiLogger logger, ref bool mute)
		{
			mo.StopLocalAudio();
			if (animationDelay > 0f)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.DelayAction(mo, animationDelay, logger));
			}
			bool flag = false;
			global::Kampai.Game.AnimationAlternate animationAlternate = gachaPick.AnimationAlternate;
			if (animationAlternate != null && randomService.NextFloat(0f, 1f) < animationAlternate.PercentChance && !alternatePlayed)
			{
				alternatePlayed = true;
				global::Kampai.Game.DefinitionGroup definitionGroup = definitionService.Get<global::Kampai.Game.DefinitionGroup>(animationAlternate.GroupID);
				int index = randomService.NextInt(0, definitionGroup.Group.Count);
				gachaPick = definitionService.Get<global::Kampai.Game.GachaAnimationDefinition>(definitionGroup.Group[index]);
				mo.EnqueueAction(new global::Kampai.Game.View.MuteAction(mo, false, logger));
				mo.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(mo, global::Kampai.Game.View.MinionObject.MinionGachaState.Deviant, logger));
				flag = true;
			}
			if (!flag)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.MuteAction(mo, mute, logger));
				mute = true;
			}
			global::Kampai.Game.MinionAnimationDefinition minionAnimationDefinition = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(gachaPick.AnimationID);
			string stateMachine = minionAnimationDefinition.StateMachine;
			if (!animationControllers.ContainsKey(stateMachine))
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "No state machine {0}", stateMachine);
				return;
			}
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, animationControllers[stateMachine], logger, minionAnimationDefinition.arguments));
			if (minionAnimationDefinition.FaceCamera)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.RotateAction(mo, global::UnityEngine.Camera.main.transform.eulerAngles.y - 180f, 360f, logger));
			}
			else if (buildingPos != null)
			{
				global::UnityEngine.Vector3 forward = buildingPos.Value - mo.transform.position;
				float y = global::UnityEngine.Quaternion.LookRotation(forward).eulerAngles.y;
				mo.EnqueueAction(new global::Kampai.Game.View.RotateAction(mo, y, 360f, logger));
			}
			mo.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(mo, global::UnityEngine.Animator.StringToHash("Base Layer.Exit"), logger));
			mo.EnqueueAction(new global::Kampai.Game.View.MuteAction(mo, false, logger));
		}

		internal global::System.Collections.Generic.Queue<int> GetMinionListSortedByDistanceAndState(global::UnityEngine.Vector3 position, bool needSelected = true)
		{
			global::UnityEngine.Vector3 vector = cameraUtils.GroundPlaneRaycast(position);
			global::System.Collections.Generic.Queue<int> queue = new global::System.Collections.Generic.Queue<int>();
			global::System.Collections.Generic.List<global::Kampai.Util.Tuple<global::Kampai.Game.Minion, float>> list = new global::System.Collections.Generic.List<global::Kampai.Util.Tuple<global::Kampai.Game.Minion, float>>();
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.View.ActionableObject> @object in objects)
			{
				global::Kampai.Game.Minion byInstanceId = player.GetByInstanceId<global::Kampai.Game.Minion>(@object.Key);
				if (byInstanceId.State == global::Kampai.Game.MinionState.Idle || byInstanceId.State == global::Kampai.Game.MinionState.Selectable || byInstanceId.State == global::Kampai.Game.MinionState.Uninitialized || (byInstanceId.State == global::Kampai.Game.MinionState.Selected && needSelected))
				{
					list.Add(global::Kampai.Util.Tuple.Create(byInstanceId, (vector - @object.Value.transform.position).sqrMagnitude));
				}
			}
			list.Sort((global::Kampai.Util.Tuple<global::Kampai.Game.Minion, float> minion1, global::Kampai.Util.Tuple<global::Kampai.Game.Minion, float> minion2) => (minion1.Item1.Level != minion2.Item1.Level) ? minion1.Item1.Level.CompareTo(minion2.Item1.Level) : minion1.Item2.CompareTo(minion2.Item2));
			if (needSelected)
			{
				EnqueueMinionsInCertainStates(list, queue, global::Kampai.Game.MinionState.Selected);
			}
			EnqueueMinionsInCertainStates(list, queue, global::Kampai.Game.MinionState.Idle, global::Kampai.Game.MinionState.Selectable, global::Kampai.Game.MinionState.Uninitialized);
			return queue;
		}

		private void EnqueueMinionsInCertainStates(global::System.Collections.Generic.List<global::Kampai.Util.Tuple<global::Kampai.Game.Minion, float>> minionTuples, global::System.Collections.Generic.Queue<int> minionQueue, params global::Kampai.Game.MinionState[] states)
		{
			for (int i = 0; i < minionTuples.Count; i++)
			{
				if (global::Kampai.Util.ListUtil.Contains(states, minionTuples[i].Item1.State))
				{
					minionQueue.Enqueue(minionTuples[i].Item1.ID);
				}
			}
		}

		internal void StartMinionAnimation(int minionId, global::Kampai.Game.MinionAnimationDefinition def, bool isIncidental)
		{
			global::Kampai.Game.View.ActionableObject actionableObject = objects[minionId];
			actionableObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(actionableObject, animationControllers[def.StateMachine], logger, def.arguments), isIncidental);
			if (def.FaceCamera)
			{
				actionableObject.EnqueueAction(new global::Kampai.Game.View.RotateAction(actionableObject, global::UnityEngine.Camera.main.transform.eulerAngles.y - 180f, 360f, logger));
			}
			actionableObject.EnqueueAction(new global::Kampai.Game.View.DelayAction(actionableObject, def.AnimationSeconds, logger));
		}

		internal void MinionAcknowledgement(int minionID, float rotateTo, global::Kampai.Game.MinionAnimationDefinition def)
		{
			global::Kampai.Game.View.ActionableObject actionableObject = objects[minionID];
			actionableObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(actionableObject, animationControllers[def.StateMachine], logger, def.arguments), true);
			actionableObject.EnqueueAction(new global::Kampai.Game.View.RotateAction(actionableObject, rotateTo, 360f, logger));
			actionableObject.EnqueueAction(new global::Kampai.Game.View.DelayAction(actionableObject, def.AnimationSeconds, logger));
			actionableObject.EnqueueAction(new global::Kampai.Game.View.IncidentalFinishedAction(minionID, stateChangeSignal, logger));
		}

		internal global::Kampai.Game.Minion GetClosestMinionToLocation(global::Kampai.Game.Location location, bool highest)
		{
			float num = float.MaxValue;
			int num2 = -1;
			int num3 = int.MaxValue;
			global::Kampai.Game.Minion result = null;
			global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(location.x, 0f, location.y);
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.View.ActionableObject> @object in objects)
			{
				global::Kampai.Game.Minion byInstanceId = player.GetByInstanceId<global::Kampai.Game.Minion>(@object.Key);
				int level = byInstanceId.Level;
				if (byInstanceId.State != global::Kampai.Game.MinionState.Idle && byInstanceId.State != global::Kampai.Game.MinionState.Selected)
				{
					continue;
				}
				if (highest)
				{
					if (level < num2)
					{
						continue;
					}
				}
				else if (level > num3)
				{
					continue;
				}
				float sqrMagnitude = (vector - @object.Value.transform.position).sqrMagnitude;
				if (highest)
				{
					if (level > num2)
					{
						num = sqrMagnitude;
						num2 = level;
						result = byInstanceId;
					}
					else if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						result = byInstanceId;
					}
				}
				else if (level < num3)
				{
					num = sqrMagnitude;
					num3 = level;
					result = byInstanceId;
				}
				else if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = byInstanceId;
				}
			}
			return result;
		}

		internal void MinionReact(global::Kampai.Game.GachaAnimationDefinition gachaPick, global::System.Collections.Generic.ICollection<int> minionIds, global::Kampai.Util.Boxed<global::UnityEngine.Vector3> buildingPos)
		{
			StartNonCoordinatedGacha(gachaPick, ToActionableObjects(minionIds), minionIds, buildingPos);
		}

		public void MinionIdle(global::Kampai.Game.View.MinionObject minionObject)
		{
			idleMinionSignal.Dispatch(minionObject);
		}

		internal void SeekPosition(int minionID, global::UnityEngine.Vector3 pos, float threshold)
		{
			global::Kampai.Game.View.ActionableObject value;
			if (objects.TryGetValue(minionID, out value))
			{
				global::Kampai.Util.AI.SteerCharacterToSeek component = value.GetComponent<global::Kampai.Util.AI.SteerCharacterToSeek>();
				component.Target = pos;
				component.Threshold = threshold;
				component.enabled = true;
			}
		}

		internal void SetPartyState(int minionID, bool isInParty, bool gameIsStarting, global::Kampai.Game.MinionAnimationDefinition partyStartAnimation)
		{
			if (isInParty && partyLocation == null)
			{
				logger.Warning("Party location is not known.");
			}
			else
			{
				global::Kampai.Game.View.ActionableObject value;
				if (!objects.TryGetValue(minionID, out value))
				{
					return;
				}
				global::Kampai.Game.View.MinionObject minionObject = value as global::Kampai.Game.View.MinionObject;
				if (isInParty)
				{
					minionObject.EnterParty(partyLocation.Value, partyRadius);
					if (gameIsStarting && partyStartAnimation != null)
					{
						minionObject.EnqueueAction(new global::Kampai.Game.View.DelayAction(minionObject, global::UnityEngine.Random.value * 0.3f, logger));
						if (partyStartAnimation.FaceCamera)
						{
							minionObject.EnqueueAction(new global::Kampai.Game.View.RotateAction(minionObject, global::UnityEngine.Camera.main.transform.eulerAngles.y - 180f, 360f, logger));
						}
						minionObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minionObject, animationControllers[partyStartAnimation.StateMachine], logger, partyStartAnimation.arguments));
						minionObject.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(minionObject, global::UnityEngine.Animator.StringToHash("Base Layer.Exit"), logger));
					}
				}
				else
				{
					minionObject.LeaveParty();
				}
			}
		}

		internal void SetPartyLocation(global::Kampai.Util.Boxed<global::UnityEngine.Vector3> partyLocation, float partyRadius)
		{
			this.partyLocation = partyLocation;
			this.partyRadius = partyRadius;
		}

		internal void PlayPartyAnimation(int minionID, global::Kampai.Game.MinionAnimationDefinition def, int allowedAudioMinions)
		{
			global::Kampai.Game.View.ActionableObject value;
			if (objects.TryGetValue(minionID, out value))
			{
				global::Kampai.Game.View.MinionObject mo = value as global::Kampai.Game.View.MinionObject;
				if (minionsPlayingAudio == null)
				{
					minionsPlayingAudio = new global::System.Collections.Generic.List<int>();
				}
				bool flag = minionsPlayingAudio.Contains(minionID);
				if (!flag && minionsPlayingAudio.Count < allowedAudioMinions)
				{
					flag = true;
					minionsPlayingAudio.Add(minionID);
				}
				EnqueueAnimation(mo, def, !flag);
			}
		}

		internal void ResetPartyAnimationCount()
		{
			minionsPlayingAudio = null;
		}
	}
}
