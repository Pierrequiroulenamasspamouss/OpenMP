namespace Kampai.Tools.AnimationToolKit
{
	public class AnimationToolKit
	{
		public class SimplePathFinder : global::Kampai.Util.IPathFinder
		{
			public global::System.Collections.Generic.IList<global::UnityEngine.Vector3> FindPath(global::UnityEngine.Vector3 startPos, global::UnityEngine.Vector3 goalPos, int modifier, bool forceDestination = false)
			{
				global::System.Collections.Generic.IList<global::UnityEngine.Vector3> list = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();
				list.Add(startPos);
				list.Add(goalPos);
				return list;
			}

			public bool IsOccupiable(global::Kampai.Game.Location location)
			{
				return true;
			}
		}

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("AnimationToolKit") as global::Kampai.Util.IKampaiLogger;

		private int activeDefintionId;

		private int activeBuildingId;

		private int animatingMinions;

		private global::System.Collections.Generic.IList<int> coordinatedAnimations;

		private global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.View.CharacterObject> characterDict;

		private global::System.Collections.Generic.IDictionary<int, global::Kampai.Game.View.BuildingObject> buildingDict;

		private global::strange.extensions.signal.impl.Signal<int> minionDoneSignal;

		private global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.CharacterObject, int> addToTikiBarSignal;

		private global::UnityEngine.RuntimeAnimatorController walk;

		private global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> animationControllers;

		private readonly global::UnityEngine.Vector3 CENTER = new global::UnityEngine.Vector3(3.4688f, 0f, -3.4687f);

		private readonly global::UnityEngine.Vector3 RIGHT = new global::UnityEngine.Vector3(8.4136f, 0f, 1.4761f);

		private readonly global::UnityEngine.Vector3 LEFT = new global::UnityEngine.Vector3(-1.306f, 0f, -8.2435f);

		[Inject(global::strange.extensions.context.api.ContextKeys.CONTEXT_VIEW)]
		public global::UnityEngine.GameObject ContextView { get; set; }

		[Inject(global::Kampai.Tools.AnimationToolKit.AnimationToolKitElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable Context { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService DefinitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService PlayerService { get; set; }

		[Inject]
		public global::Kampai.Common.IRandomService randomService { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.AnimationToolkitModel model { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.LoadInterfaceSignal LoadInterfaceSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.GenerateMinionSignal generateMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.MinionCreatedSignal minionCreatedSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.AddMinionSignal AddMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.RemoveMinionSignal RemoveMinionSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.PlayMinionAnimationSignal playGachaSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.GenerateVillainSignal generateVillainSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.VillainCreatedSignal villainCreatedSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.GenerateCharacterSignal generateCharacterSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.CharacterCreatedSignal characterCreatedSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.AddCharacterSignal addCharacterSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.RemoveCharacterSignal removeCharacterSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.EnableInterfaceSignal enableInterfaceSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal audioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StartLoopingAudioSignal startLoopingAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StopLocalAudioSignal stopAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayMinionStateAudioSignal minionStateAudioSignal { get; set; }

		public AnimationToolKit()
		{
			coordinatedAnimations = new global::System.Collections.Generic.List<int>();
			characterDict = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.CharacterObject>();
			buildingDict = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.BuildingObject>();
			animationControllers = new global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController>();
			walk = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>("asm_minion_movement");
			animationControllers.Add("asm_minion_movement", walk);
			minionDoneSignal = new global::strange.extensions.signal.impl.Signal<int>();
			addToTikiBarSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.CharacterObject, int>();
		}

		[PostConstruct]
		public void PostConstruct()
		{
			global::UnityEngine.Object[] array = global::UnityEngine.Object.FindObjectsByType(typeof(global::UnityEngine.Transform), global::UnityEngine.FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				global::UnityEngine.Transform transform = (global::UnityEngine.Transform)array[i];
				if (!(transform.parent != null) && !(transform == ContextView.transform))
				{
					model.Mode = global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Building;
					AddBuilding(transform);
				}
			}
			LoadInterfaceSignal.Dispatch();
			playGachaSignal.AddListener(PlayMinionAnimation);
			minionDoneSignal.AddListener(GachaFinished);
			minionCreatedSignal.AddListener(MinionCreated);
			villainCreatedSignal.AddListener(VillainCreated);
			characterCreatedSignal.AddListener(CharacterCreated);
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("The Light");
			global::UnityEngine.Light light = gameObject.AddComponent<global::UnityEngine.Light>();
			gameObject.GetComponent<global::UnityEngine.Light>().color = global::UnityEngine.Color.white;
			gameObject.transform.position = new global::UnityEngine.Vector3(4.52576f, 0.539923f, -2.41174f);
			gameObject.transform.rotation = global::UnityEngine.Quaternion.Euler(new global::UnityEngine.Vector3(50f, 330f, 0f));
			light.intensity = 0.4f;
			light.type = global::UnityEngine.LightType.Directional;
		}

		public void ToggleOn(int toggleId)
		{
			global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode mode = model.Mode;
			global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(toggleId);
			if (byInstanceId is global::Kampai.Game.StageBuilding)
			{
				model.Mode = global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage;
			}
			else if (byInstanceId is global::Kampai.Game.TikiBarBuilding)
			{
				model.Mode = global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar;
			}
			else if (byInstanceId is global::Kampai.Game.DebrisBuilding)
			{
				model.Mode = global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Debris;
			}
			else if (byInstanceId != null)
			{
				model.Mode = global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Building;
			}
			if (mode != model.Mode)
			{
				LoadInterfaceSignal.Dispatch();
			}
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Building || model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar || model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage || model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Debris)
			{
				activeBuildingId = toggleId;
			}
			else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Villain || model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
			{
				activeDefintionId = toggleId;
			}
		}

		public void AddMinion()
		{
			global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(activeBuildingId);
			if (byInstanceId != null)
			{
				int routeIndex = GetRouteIndex(byInstanceId);
				int stationCount = GetStationCount(buildingDict[activeBuildingId]);
				if (routeIndex < 0 || routeIndex >= stationCount)
				{
					return;
				}
			}
			generateMinionSignal.Dispatch();
		}

		public void AddVillain()
		{
			generateVillainSignal.Dispatch(activeDefintionId);
		}

		public void AddCharacter()
		{
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
			{
				generateCharacterSignal.Dispatch(activeDefintionId);
			}
			else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage)
			{
				generateCharacterSignal.Dispatch(70001);
			}
			else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(activeBuildingId);
				int routeIndex = GetRouteIndex(byInstanceId);
				int stationCount = GetStationCount(buildingDict[activeBuildingId]);
				if (routeIndex >= 0 && routeIndex < stationCount)
				{
					int type = ((routeIndex != 0) ? 70003 : 70000);
					generateCharacterSignal.Dispatch(type);
				}
			}
		}

		private void AddMinionToBuilding(global::Kampai.Game.View.MinionObject minionObj)
		{
			global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(activeBuildingId);
			int routeIndex = GetRouteIndex(byInstanceId);
			int stationCount = GetStationCount(buildingDict[activeBuildingId]);
			if (routeIndex >= 0 && routeIndex < stationCount)
			{
				AddMinionSignal.Dispatch(buildingDict[activeBuildingId], minionObj, routeIndex);
				if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Debris)
				{
					minionObj.EnqueueAction(new global::Kampai.Game.View.DelegateAction(RemoveMinionFromBuilding, logger));
				}
			}
		}

		private void AddCharacterToBuilding(global::Kampai.Game.View.NamedCharacterObject characterObj)
		{
			global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(activeBuildingId);
			int routeIndex = GetRouteIndex(byInstanceId);
			int stationCount = GetStationCount(buildingDict[activeBuildingId]);
			if (routeIndex < 0 || routeIndex >= stationCount)
			{
				return;
			}
			addCharacterSignal.Dispatch(buildingDict[activeBuildingId], characterObj, routeIndex);
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				global::Kampai.Game.View.PhilView philView = characterObj as global::Kampai.Game.View.PhilView;
				if (philView != null)
				{
					philView.AnimSignal.AddListener(PlayTikiBarAnimation);
					global::Kampai.Game.TeleportCharacterToTikiBarSignal instance = Context.injectionBinder.GetInstance<global::Kampai.Game.TeleportCharacterToTikiBarSignal>();
					instance.AddListener(AddCharacterToTikiBarActions);
					philView.SitAtBar(true, instance);
				}
			}
		}

		private void AddCharacterToTikiBarActions(global::Kampai.Game.View.CharacterObject characterObject, int routeIndex)
		{
			global::Kampai.Game.View.TikiBarBuildingObjectView tikiBarBuildingObjectView = buildingDict[activeBuildingId] as global::Kampai.Game.View.TikiBarBuildingObjectView;
			if (tikiBarBuildingObjectView != null)
			{
				addToTikiBarSignal.Dispatch(characterObject, routeIndex);
			}
		}

		private void PlayTikiBarAnimation(string animation, global::System.Type type, object obj)
		{
			global::Kampai.Game.View.TikiBarBuildingObjectView tikiBarBuildingObjectView = buildingDict[activeBuildingId] as global::Kampai.Game.View.TikiBarBuildingObjectView;
			if (tikiBarBuildingObjectView != null)
			{
				tikiBarBuildingObjectView.PlayAnimation(animation, type, obj);
			}
		}

		private void MinionCreated(global::Kampai.Game.View.MinionObject minionObj)
		{
			minionObj.ShelveActionQueue();
			characterDict.Add(minionObj.ID, minionObj);
			global::System.Collections.Generic.ICollection<global::Kampai.Game.Building> instancesByType = PlayerService.GetInstancesByType<global::Kampai.Game.Building>();
			if (instancesByType.Count > 0)
			{
				AddMinionToBuilding(minionObj);
			}
			else
			{
				ResetCharacterPosition();
			}
		}

		private void VillainCreated(global::Kampai.Game.View.VillainView villainObj)
		{
			villainObj.ShelveActionQueue();
			characterDict.Add(villainObj.ID, villainObj);
			ResetCharacterPosition();
		}

		private void CharacterCreated(global::Kampai.Game.View.NamedCharacterObject characterObj)
		{
			characterObj.ShelveActionQueue();
			characterDict.Add(characterObj.ID, characterObj);
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage || model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				AddCharacterToBuilding(characterObj);
			}
			else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
			{
				ResetCharacterPosition();
			}
		}

		public void RemoveMinion()
		{
			if (buildingDict.Count > 0)
			{
				RemoveMinionFromBuilding();
				return;
			}
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>(characterDict.Keys);
			if (list.Count > 0)
			{
				global::UnityEngine.Object.DestroyImmediate(characterDict[list[0]].gameObject);
				characterDict.Remove(list[0]);
				ResetCharacterPosition();
			}
		}

		public void RemoveVillain()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(activeDefintionId);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.CharacterObject characterObject = characterDict[item.ID];
				global::UnityEngine.Object.DestroyImmediate(characterObject.gameObject);
				PlayerService.Remove(item);
				characterDict.Remove(item.ID);
			}
		}

		public void RemoveCharacter()
		{
			int key = -1;
			int num = -1;
			int num2 = -1;
			global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(activeBuildingId);
			if (buildingDict.ContainsKey(activeBuildingId))
			{
				num = GetRouteIndex(byInstanceId);
				if (model.Mode != global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage)
				{
					num--;
				}
				num2 = GetStationCount(buildingDict[activeBuildingId]);
				if (num < 0 || num >= num2)
				{
					return;
				}
			}
			int definitionId = ((model.Mode != global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character) ? 70001 : activeDefintionId);
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				definitionId = ((num != 0) ? 70003 : 70000);
			}
			global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId = PlayerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>(definitionId);
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage || model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				key = GetMinionId(byInstanceId, num);
			}
			else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character && firstInstanceByDefinitionId != null)
			{
				key = firstInstanceByDefinitionId.ID;
			}
			if (!characterDict.ContainsKey(key))
			{
				return;
			}
			global::Kampai.Game.View.CharacterObject characterObject = characterDict[key];
			if (buildingDict.ContainsKey(activeBuildingId))
			{
				removeCharacterSignal.Dispatch(buildingDict[activeBuildingId], characterObject);
			}
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				global::Kampai.Game.View.TikiBarBuildingObjectView tikiBarBuildingObjectView = buildingDict[activeBuildingId] as global::Kampai.Game.View.TikiBarBuildingObjectView;
				if (tikiBarBuildingObjectView != null)
				{
					switch (num)
					{
					case 0:
						tikiBarBuildingObjectView.SetAnimBool("bartender_IsSeated", false);
						break;
					case 1:
						tikiBarBuildingObjectView.SetAnimBool("pos1_IsSeated", false);
						break;
					case 2:
						tikiBarBuildingObjectView.SetAnimBool("pos2_IsSeated", false);
						break;
					}
				}
			}
			PlayerService.Remove(firstInstanceByDefinitionId);
			global::UnityEngine.Object.Destroy(characterObject.gameObject);
			characterDict.Remove(characterObject.ID);
		}

		public void RemoveMinionFromBuilding()
		{
			global::Kampai.Game.Building byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.Building>(activeBuildingId);
			int num = GetRouteIndex(byInstanceId) - 1;
			int stationCount = GetStationCount(buildingDict[activeBuildingId]);
			if (num >= 0 && num < stationCount)
			{
				int minionId = GetMinionId(byInstanceId, num);
				if (characterDict.ContainsKey(minionId))
				{
					global::Kampai.Game.View.MinionObject minionObject = characterDict[minionId] as global::Kampai.Game.View.MinionObject;
					RemoveMinionSignal.Dispatch(buildingDict[activeBuildingId], minionObject);
					global::UnityEngine.Object.Destroy(minionObject.gameObject);
					characterDict.Remove(minionObject.ID);
				}
			}
		}

		private void ResetCharacterPosition()
		{
			float num = 1f / global::System.Convert.ToSingle(characterDict.Count + 1);
			float num2 = num;
			foreach (global::Kampai.Game.View.CharacterObject value in characterDict.Values)
			{
				value.gameObject.transform.position = global::UnityEngine.Vector3.Lerp(LEFT, RIGHT, num2);
				num2 += num;
				if (model.Mode != global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Villain)
				{
					global::UnityEngine.Vector3 position = global::UnityEngine.Camera.main.transform.position;
					value.gameObject.transform.LookAt(new global::UnityEngine.Vector3(position.x, 0f, position.z));
				}
			}
		}

		public void LoopAnimation()
		{
			if (buildingDict.ContainsKey(activeBuildingId))
			{
				global::Kampai.Game.View.AnimatingBuildingObject animatingBuildingObject = buildingDict[activeBuildingId] as global::Kampai.Game.View.AnimatingBuildingObject;
				if (animatingBuildingObject != null)
				{
					animatingBuildingObject.StartAnimating();
				}
			}
		}

		public void PlayMinionAnimation(global::Kampai.Game.AnimationDefinition def)
		{
			if (animatingMinions > 0)
			{
				logger.Error("Wait for the previous animation to finish.");
				return;
			}
			if (characterDict.Count == 0)
			{
				logger.Error("Add minions first.");
				return;
			}
			global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = def as global::Kampai.Game.GachaAnimationDefinition;
			if (gachaAnimationDefinition != null)
			{
				if (gachaAnimationDefinition.Minions <= 0)
				{
					if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Minion)
					{
						StartNonCoordinatedGacha(gachaAnimationDefinition, characterDict.Keys);
					}
					else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
					{
						StartNonCoordinatedGacha(gachaAnimationDefinition, characterDict.Keys);
					}
					return;
				}
				global::System.Collections.Generic.List<global::Kampai.Game.View.ActionableObject> list = new global::System.Collections.Generic.List<global::Kampai.Game.View.ActionableObject>();
				if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Minion)
				{
					foreach (global::Kampai.Game.View.MinionObject value in characterDict.Values)
					{
						list.Add(value);
					}
				}
				else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
				{
					foreach (global::Kampai.Game.View.NamedCharacterObject value2 in characterDict.Values)
					{
						list.Add(value2);
					}
				}
				StartCoordinatedGacha(gachaAnimationDefinition, list);
			}
			else
			{
				global::Kampai.Game.MinionAnimationDefinition definition = def as global::Kampai.Game.MinionAnimationDefinition;
				if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Minion)
				{
					StartMinionAnimation(definition, characterDict.Keys);
				}
				else if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
				{
					StartMinionAnimation(definition, characterDict.Keys);
				}
			}
		}

		public void GagAnimation()
		{
			if (buildingDict.ContainsKey(activeBuildingId))
			{
				global::Kampai.Game.View.GaggableBuildingObject gaggableBuildingObject = buildingDict[activeBuildingId] as global::Kampai.Game.View.GaggableBuildingObject;
				if (gaggableBuildingObject != null)
				{
					gaggableBuildingObject.TriggerGagAnimation();
				}
			}
		}

		public void WaitAnimation()
		{
			if (!buildingDict.ContainsKey(activeBuildingId))
			{
				return;
			}
			global::Kampai.Game.TaskableBuilding byInstanceId = PlayerService.GetByInstanceId<global::Kampai.Game.TaskableBuilding>(activeBuildingId);
			if (byInstanceId == null)
			{
				return;
			}
			global::Kampai.Game.View.TaskableBuildingObject taskableBuildingObject = buildingDict[activeBuildingId] as global::Kampai.Game.View.TaskableBuildingObject;
			if (!(taskableBuildingObject == null))
			{
				int num = byInstanceId.GetMinionsInBuilding() - 1;
				if (num >= 0 && num < taskableBuildingObject.GetNumberOfStations())
				{
					int minionByIndex = byInstanceId.GetMinionByIndex(num);
					taskableBuildingObject.RestMinion(minionByIndex);
				}
			}
		}

		public void VillainIntroAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(activeDefintionId);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.VillainView villainView = characterDict[item.ID] as global::Kampai.Game.View.VillainView;
				villainView.PlayWelcome();
			}
		}

		public void VillainCabanaAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(activeDefintionId);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.VillainView villainView = characterDict[item.ID] as global::Kampai.Game.View.VillainView;
				villainView.GotoCabana(0, villainView.transform);
			}
		}

		public void VillainFarewellAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(activeDefintionId);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.VillainView villainView = characterDict[item.ID] as global::Kampai.Game.View.VillainView;
				villainView.PlayFarewell();
			}
		}

		public void TikiBarCelebrateAnimation()
		{
			global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId = PlayerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>(70000);
			global::Kampai.Game.View.PhilView philView = characterDict[firstInstanceByDefinitionId.ID] as global::Kampai.Game.View.PhilView;
			if (!(philView == null))
			{
				philView.Celebrate();
			}
		}

		public void TikiBarAttentionAnimation()
		{
			global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId = PlayerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>(70000);
			global::Kampai.Game.View.PhilView philView = characterDict[firstInstanceByDefinitionId.ID] as global::Kampai.Game.View.PhilView;
			if (!(philView == null))
			{
				bool flag = philView.GetAnimBool("AttentionStart") || philView.GetAnimBool("bartender_IsGetAttention");
				philView.GetAttention(!flag);
			}
		}

		public void TikiBarAlternateAnimation(int alternateIndex)
		{
			global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId = PlayerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>(70000);
			global::Kampai.Game.View.PhilView philView = characterDict[firstInstanceByDefinitionId.ID] as global::Kampai.Game.View.PhilView;
			if (!(philView == null))
			{
				philView.SetAnimInteger("randomizer", alternateIndex);
				philView.AnimSignal.Dispatch("randomizer", typeof(int), alternateIndex);
				philView.SetAnimBool("PlayAlternate", true);
				philView.AnimSignal.Dispatch("PlayAlternate", typeof(bool), true);
			}
		}

		public void StuartStageIdleAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(70001);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.StuartView stuartView = characterDict[item.ID] as global::Kampai.Game.View.StuartView;
				if (stuartView != null)
				{
					stuartView.GetOnStage(!stuartView.GetAnimBool("goToStage"));
				}
			}
		}

		public void StuartPerformAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(70001);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.StuartView stuartView = characterDict[item.ID] as global::Kampai.Game.View.StuartView;
				if (stuartView != null)
				{
					stuartView.Perform(new global::Kampai.Util.SignalCallback<global::strange.extensions.signal.impl.Signal>(new global::strange.extensions.signal.impl.Signal()));
				}
			}
		}

		public void StuartCelebrateAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(70001);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.StuartView stuartView = characterDict[item.ID] as global::Kampai.Game.View.StuartView;
				if (stuartView != null)
				{
					stuartView.StartingState(global::Kampai.Game.StuartStageAnimationType.CELEBRATE);
				}
			}
		}

		public void StuartAttentionAnimation()
		{
			global::System.Collections.Generic.ICollection<global::Kampai.Game.NamedCharacter> byDefinitionId = PlayerService.GetByDefinitionId<global::Kampai.Game.NamedCharacter>(70001);
			foreach (global::Kampai.Game.NamedCharacter item in byDefinitionId)
			{
				global::Kampai.Game.View.StuartView stuartView = characterDict[item.ID] as global::Kampai.Game.View.StuartView;
				if (stuartView != null)
				{
					stuartView.GetAttention(!stuartView.GetAnimBool("isGetAttention"));
				}
			}
		}

		private void AddBuilding(global::UnityEngine.Transform transform)
		{
			global::Kampai.Game.BuildingDefinition buildingDefinition = GetBuildingDefinition(transform.name);
			if (buildingDefinition == null)
			{
				logger.Error(string.Format("Building ({0}) not found! Does the prefab name match the JSON?", transform.name));
				return;
			}
			global::Kampai.Game.TaskableBuildingDefinition taskableBuildingDefinition = buildingDefinition as global::Kampai.Game.TaskableBuildingDefinition;
			if (taskableBuildingDefinition != null)
			{
				taskableBuildingDefinition.RouteToSlot = true;
			}
			LoadAnimationControllers(buildingDefinition);
			LoadAnimationEventHandler(transform);
			global::Kampai.Game.Building building = buildingDefinition.BuildBuilding();
			global::Kampai.Game.View.BuildingObject buildingObject = building.AddBuildingObject(transform.gameObject);
			building.Location = new global::Kampai.Game.Location((int)buildingObject.transform.position.x, (int)buildingObject.transform.position.z);
			PlayerService.Add(building);
			buildingObject.Init(building, logger, animationControllers, DefinitionService);
			buildingDict.Add(building.ID, buildingObject);
		}

		private global::Kampai.Game.BuildingDefinition GetBuildingDefinition(string prefabName)
		{
			global::Kampai.Game.BuildingDefinition buildingDefinition = null;
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.Definition> allDefinitions = DefinitionService.GetAllDefinitions();
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.Definition> item in allDefinitions)
			{
				global::Kampai.Game.Definition value = item.Value;
				buildingDefinition = value as global::Kampai.Game.BuildingDefinition;
				if (buildingDefinition != null && buildingDefinition.GetPrefab().Contains(prefabName))
				{
					return buildingDefinition;
				}
			}
			return buildingDefinition;
		}

		private void LoadAnimationControllers(global::Kampai.Game.BuildingDefinition buildingDefinition)
		{
			global::Kampai.Game.AnimatingBuildingDefinition animatingBuildingDefinition = buildingDefinition as global::Kampai.Game.AnimatingBuildingDefinition;
			if (animatingBuildingDefinition == null)
			{
				return;
			}
			foreach (string item in animatingBuildingDefinition.AnimationControllerKeys())
			{
				if (!animationControllers.ContainsKey(item))
				{
					global::UnityEngine.RuntimeAnimatorController value = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(item);
					animationControllers.Add(item, value);
				}
			}
		}

		private void LoadAnimationEventHandler(global::UnityEngine.Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				global::UnityEngine.Transform child = transform.GetChild(i);
				global::UnityEngine.GameObject gameObject = child.gameObject;
				global::UnityEngine.Animator component = gameObject.GetComponent<global::UnityEngine.Animator>();
				global::Kampai.Game.View.AnimEventHandler component2 = gameObject.GetComponent<global::Kampai.Game.View.AnimEventHandler>();
				if (component != null && component2 == null)
				{
					global::Kampai.Game.View.AnimEventHandler animEventHandler = gameObject.AddComponent<global::Kampai.Game.View.AnimEventHandler>();
					animEventHandler.Init(gameObject, audioSignal, stopAudioSignal, minionStateAudioSignal, startLoopingAudioSignal);
				}
			}
		}

		private void GachaFinished(int minionId)
		{
			if (characterDict.ContainsKey(minionId))
			{
				global::Kampai.Game.View.CharacterObject characterObject = characterDict[minionId];
				characterObject.ClearActionQueue();
				animatingMinions--;
				if (animatingMinions == 0)
				{
					enableInterfaceSignal.Dispatch(true);
				}
				ResetCharacterPosition();
			}
		}

		private void CacheAnimators(global::Kampai.Game.AnimationDefinition animationDef, bool cycle = false)
		{
			global::Kampai.Game.MinionAnimationDefinition minionAnimationDefinition = animationDef as global::Kampai.Game.MinionAnimationDefinition;
			if (minionAnimationDefinition != null && !animationControllers.ContainsKey(minionAnimationDefinition.StateMachine))
			{
				animationControllers.Add(minionAnimationDefinition.StateMachine, global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(minionAnimationDefinition.StateMachine));
			}
			global::Kampai.Game.GachaAnimationDefinition gachaAnimationDefinition = animationDef as global::Kampai.Game.GachaAnimationDefinition;
			if (gachaAnimationDefinition == null)
			{
				return;
			}
			global::Kampai.Game.AnimationDefinition definition;
			if (!DefinitionService.TryGet<global::Kampai.Game.AnimationDefinition>(gachaAnimationDefinition.AnimationID, out definition))
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Undefined animation ID {0} for animation {1}", gachaAnimationDefinition.AnimationID, animationDef.ID);
			}
			else
			{
				CacheAnimators(definition);
			}
			if (gachaAnimationDefinition.AnimationAlternate == null || cycle)
			{
				return;
			}
			global::Kampai.Game.DefinitionGroup definition2 = null;
			if (!DefinitionService.TryGet<global::Kampai.Game.DefinitionGroup>(gachaAnimationDefinition.AnimationAlternate.GroupID, out definition2))
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Undefined group ID {0} for animation {1}", definition2.Group, animationDef.ID);
				return;
			}
			foreach (int item in definition2.Group)
			{
				global::Kampai.Game.AnimationDefinition animationDef2 = DefinitionService.Get<global::Kampai.Game.AnimationDefinition>(item);
				CacheAnimators(animationDef2, true);
			}
		}

		private void StartMinionAnimation(global::Kampai.Game.MinionAnimationDefinition definition, global::System.Collections.Generic.ICollection<int> minionIds)
		{
			CacheAnimators(definition);
			foreach (int minionId in minionIds)
			{
				global::Kampai.Game.View.CharacterObject characterObject = characterDict[minionId];
				characterObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(characterObject, animationControllers[definition.StateMachine], logger, definition.arguments));
				characterObject.EnqueueAction(new global::Kampai.Game.View.DelayAction(characterObject, definition.AnimationSeconds, logger));
				characterObject.EnqueueAction(new global::Kampai.Game.View.SendIDSignalAction(characterObject, minionDoneSignal, logger));
				animatingMinions++;
				if (animatingMinions == 1)
				{
					enableInterfaceSignal.Dispatch(false);
				}
			}
		}

		private void StartNonCoordinatedGacha(global::Kampai.Game.GachaAnimationDefinition animationDef, global::System.Collections.Generic.ICollection<int> minionIds)
		{
			CacheAnimators(animationDef);
			global::Kampai.Game.View.MinionManagerView.Knuckleheaddedness knuckleheaddedness = new global::Kampai.Game.View.MinionManagerView.Knuckleheaddedness(animationDef, minionIds, randomService);
			bool mute = false;
			foreach (int minionId in minionIds)
			{
				global::Kampai.Game.View.CharacterObject characterObject = characterDict[minionId];
				global::Kampai.Game.View.MinionObject minionObject = characterObject as global::Kampai.Game.View.MinionObject;
				if (minionObject != null)
				{
					float animationDelay = knuckleheaddedness.DelayTime(minionId, randomService);
					characterObject.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(minionObject, global::Kampai.Game.View.MinionObject.MinionGachaState.Active, logger));
					global::Kampai.Game.View.MinionManagerView.SetupSingleMinionGacha(randomService, DefinitionService, minionObject, animationControllers, animationDef, animationDelay, null, logger, ref mute);
					characterObject.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(minionObject, global::Kampai.Game.View.MinionObject.MinionGachaState.Inactive, logger));
				}
				characterObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(characterObject, walk, logger));
				characterObject.EnqueueAction(new global::Kampai.Game.View.SendIDSignalAction(characterObject, minionDoneSignal, logger));
				animatingMinions++;
				if (animatingMinions == 1)
				{
					enableInterfaceSignal.Dispatch(false);
				}
			}
		}

		private void StartCoordinatedGacha(global::Kampai.Game.GachaAnimationDefinition gacha, global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> minions)
		{
			CacheAnimators(gacha);
			if (!coordinatedAnimations.Contains(gacha.ID))
			{
				global::UnityEngine.Object obj = global::Kampai.Util.KampaiResources.Load(gacha.Prefab);
				if (obj == null)
				{
					logger.Error("Can't load prefab: " + gacha.Prefab);
				}
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(obj) as global::UnityEngine.GameObject;
				if (gameObject == null)
				{
					logger.Error("Can't instantiate prefab: " + gacha.Prefab);
				}
				global::UnityEngine.Object.Destroy(gameObject);
				coordinatedAnimations.Add(gacha.ID);
			}
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Minion)
			{
				global::Kampai.Game.View.MinionManagerView.SetupCoordinatedMinionGacha(ContextView, DefinitionService, gacha, animationControllers, minions, CENTER, new global::Kampai.Tools.AnimationToolKit.AnimationToolKit.SimplePathFinder(), logger);
			}
			foreach (global::Kampai.Game.View.CharacterObject minion in minions)
			{
				global::Kampai.Game.View.MinionObject minionObject = minion as global::Kampai.Game.View.MinionObject;
				if (minionObject != null)
				{
					minionObject.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(minionObject, global::Kampai.Game.View.MinionObject.MinionGachaState.Active, logger));
					minionObject.EnqueueAction(new global::Kampai.Game.View.SetMinionGachaState(minionObject, global::Kampai.Game.View.MinionObject.MinionGachaState.Inactive, logger));
				}
				minion.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minion, walk, logger));
				minion.EnqueueAction(new global::Kampai.Game.View.SendIDSignalAction(minion, minionDoneSignal, logger));
				animatingMinions++;
				if (animatingMinions == 1)
				{
					enableInterfaceSignal.Dispatch(false);
				}
			}
		}

		private int GetRouteIndex(global::Kampai.Game.Building building)
		{
			global::Kampai.Game.TaskableBuilding taskableBuilding = building as global::Kampai.Game.TaskableBuilding;
			global::Kampai.Game.LeisureBuilding leisureBuilding = building as global::Kampai.Game.LeisureBuilding;
			global::Kampai.Game.StageBuilding stageBuilding = building as global::Kampai.Game.StageBuilding;
			global::Kampai.Game.TikiBarBuilding tikiBarBuilding = building as global::Kampai.Game.TikiBarBuilding;
			int result = -1;
			if (taskableBuilding != null)
			{
				result = taskableBuilding.GetMinionsInBuilding();
			}
			else if (leisureBuilding != null)
			{
				result = leisureBuilding.GetMinionsInBuilding();
			}
			else if (tikiBarBuilding != null)
			{
				result = tikiBarBuilding.GetMinionsInBuilding();
			}
			else if (stageBuilding != null)
			{
				result = 0;
			}
			return result;
		}

		private int GetStationCount(global::Kampai.Game.View.BuildingObject buildingObject)
		{
			global::Kampai.Game.View.RoutableBuildingObject routableBuildingObject = buildingObject as global::Kampai.Game.View.RoutableBuildingObject;
			int num = -1;
			if (routableBuildingObject != null)
			{
				num = routableBuildingObject.GetNumberOfStations();
			}
			if (buildingObject is global::Kampai.Game.View.TikiBarBuildingObjectView)
			{
				num -= 3;
			}
			return num;
		}

		private int GetMinionId(global::Kampai.Game.Building building, int routeIndex)
		{
			global::Kampai.Game.TaskableBuilding taskableBuilding = building as global::Kampai.Game.TaskableBuilding;
			global::Kampai.Game.LeisureBuilding leisureBuilding = building as global::Kampai.Game.LeisureBuilding;
			global::Kampai.Game.StageBuilding stageBuilding = building as global::Kampai.Game.StageBuilding;
			int definitionId = 70001;
			if (model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				definitionId = ((routeIndex != 0) ? 70003 : 70000);
			}
			global::Kampai.Game.NamedCharacter firstInstanceByDefinitionId = PlayerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.NamedCharacter>(definitionId);
			int result = -1;
			if (taskableBuilding != null)
			{
				result = taskableBuilding.GetMinionByIndex(routeIndex);
			}
			else if (leisureBuilding != null)
			{
				result = leisureBuilding.GetMinionByIndex(routeIndex);
			}
			else if (stageBuilding != null && firstInstanceByDefinitionId != null)
			{
				result = firstInstanceByDefinitionId.ID;
			}
			return result;
		}
	}
}
