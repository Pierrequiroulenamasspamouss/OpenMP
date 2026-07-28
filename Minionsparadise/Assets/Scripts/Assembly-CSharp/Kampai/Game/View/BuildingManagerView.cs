namespace Kampai.Game.View
{
	public class BuildingManagerView : global::Kampai.Util.KampaiView
	{
		private global::Kampai.Util.IKampaiLogger logger;

		private global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.Game.IMasterPlanService masterPlanService;

		private global::Kampai.Game.View.BuildingObject selectedBuilding;

		private global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.BuildingObjectCollection> buildings = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.BuildingObjectCollection>();

		private global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController> animationControllers;

		private bool isSpecialEventActive;

		internal global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.BuildingObject, global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController>, global::Kampai.Game.Building> initBuildingObject = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.BuildingObject, global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController>, global::Kampai.Game.Building>();

		internal global::strange.extensions.signal.impl.Signal<int, global::Kampai.Game.View.MinionTaskInfo> updateMinionSignal = new global::strange.extensions.signal.impl.Signal<int, global::Kampai.Game.View.MinionTaskInfo>();

		internal global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Building, global::Kampai.Game.Location> addFootprintSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Building, global::Kampai.Game.Location>();

		internal global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Building> updateResourceBuildingSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Building>();

		internal global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Building> setBuildingNumberSignal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.Building>();

		private global::Kampai.Game.View.BuildingObject toInventoryBuildingObject;

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal audioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StartLoopingAudioSignal startLoopingAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StopLocalAudioSignal stopAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayMinionStateAudioSignal minionStateAudioSignal { get; set; }

		[Inject(global::Kampai.Main.MainElement.CAMERA)]
		public global::UnityEngine.Camera mainCamera { get; set; }

		[Inject(global::Kampai.Game.GameElement.LAND_EXPANSION_PARENT)]
		public global::UnityEngine.GameObject landExpansionParent { get; set; }

		[Inject]
		public global::Kampai.Game.ConnectableBuildingPickedUpSignal connectableBuildingPickedUpSignal { get; set; }

		internal void Init(global::Kampai.Util.IKampaiLogger log, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Game.IMasterPlanService masterPlanService, bool isSpecialEventActive)
		{
			selectedBuilding = null;
			animationControllers = new global::System.Collections.Generic.Dictionary<string, global::UnityEngine.RuntimeAnimatorController>();
			logger = log;
			this.definitionService = definitionService;
			this.masterPlanService = masterPlanService;
			this.isSpecialEventActive = isSpecialEventActive;
		}

		internal global::UnityEngine.GameObject CreateBuilding(global::Kampai.Game.Building building, int prefabIndex = 0)
		{
			int iD = building.ID;
			global::Kampai.Game.Location location = building.Location;
			string prefab = building.GetPrefab(prefabIndex);
			global::UnityEngine.GameObject gameObject;
			if (string.IsNullOrEmpty(prefab))
			{
				gameObject = new global::UnityEngine.GameObject();
			}
			else
			{
				global::UnityEngine.GameObject gameObject2 = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(prefab);
				if (gameObject2 != null)
				{
					gameObject = global::UnityEngine.Object.Instantiate(gameObject2);
				}
				else
				{
					logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Trying to instantiate null prefab: {0}", prefab);
					gameObject = new global::UnityEngine.GameObject();
				}
			}
			if (building is global::Kampai.Game.LandExpansionBuilding)
			{
				gameObject.transform.parent = landExpansionParent.transform;
			}
			else
			{
				gameObject.transform.parent = base.transform;
			}
			global::Kampai.Game.View.BuildingObject buildingObject = SetupBuilding(gameObject, building);
			gameObject.transform.position = new global::UnityEngine.Vector3(location.x, 0f, location.y);
			gameObject.transform.rotation = global::UnityEngine.Quaternion.identity;
			if (building is global::Kampai.Game.MasterPlanComponentBuilding)
			{
				gameObject.transform.position += masterPlanService.GetComponentBuildingOffset(building.Definition.ID);
			}
			else if (building.IsFootprintable)
			{
				addFootprintSignal.Dispatch(building, location);
			}
			if (isSpecialEventActive)
			{
				LoadSpecialEventPaintover(building, gameObject);
			}
			buildings[iD] = new global::Kampai.Game.View.BuildingObjectCollection(buildingObject);
			return gameObject;
		}

		internal void HighlightBuilding(int buildingId, bool highlight)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				buildingObject.Highlight(highlight);
			}
		}

		private void LoadSpecialEventPaintover(global::Kampai.Game.Building building, global::UnityEngine.GameObject parent)
		{
			if (!building.IsBuildingRepaired())
			{
				return;
			}
			string paintover = building.GetPaintover();
			if (string.IsNullOrEmpty(paintover))
			{
				return;
			}
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(paintover);
			if (!(gameObject == null))
			{
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
				if (!(gameObject2 == null))
				{
					gameObject2.transform.parent = parent.transform;
					gameObject2.transform.localPosition = global::UnityEngine.Vector3.zero;
				}
			}
		}

		private global::Kampai.Game.View.BuildingObject SetupBuilding(global::UnityEngine.GameObject buildingInstance, global::Kampai.Game.Building building)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = building.AddBuildingObject(buildingInstance);
			if (!(building is global::Kampai.Game.DecorationBuilding) && !(building is global::Kampai.Game.LandExpansionBuilding))
			{
				AddAnimation(buildingObject.gameObject);
			}
			if (building is global::Kampai.Game.ResourceBuilding)
			{
				updateResourceBuildingSignal.Dispatch(building);
			}
			else
			{
				setBuildingNumberSignal.Dispatch(building);
			}
			AddAnimEventHandlersToChildren(buildingInstance.transform, buildingObject);
			if (!(building is global::Kampai.Game.MignetteBuilding))
			{
				LoadAnimationControllers(building);
			}
			global::Kampai.Game.View.IStartAudio startAudio = buildingObject as global::Kampai.Game.View.IStartAudio;
			if (startAudio != null)
			{
				startAudio.InitAudio(building.State, audioSignal);
			}
			initBuildingObject.Dispatch(buildingObject, animationControllers, building);
			buildingInstance.name = string.Format("building_{0}", building.ID);
			return buildingObject;
		}

		private global::Kampai.Game.View.BuildingObjectCollection GetBuildingObjectCollection(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection result = null;
			if (buildings.ContainsKey(buildingId))
			{
				result = buildings[buildingId];
			}
			return result;
		}

		internal void SetBuildingRushed(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection != null)
			{
				buildingObjectCollection.Rushed = true;
			}
		}

		internal bool IsBuildingRushed(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection != null)
			{
				return buildingObjectCollection.Rushed;
			}
			return false;
		}

		internal global::Kampai.Game.View.BuildingObject GetBuildingObject(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection == null)
			{
				return null;
			}
			return buildingObjectCollection.BuildingObject;
		}

		internal global::Kampai.Game.View.ScaffoldingBuildingObject GetScaffoldingBuildingObject(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection == null)
			{
				return null;
			}
			return buildingObjectCollection.ScaffoldingBuildingObject;
		}

		private string GetScaffoldingPrefabName(global::Kampai.Game.BuildingDefinition buildingDefinition)
		{
			return buildingDefinition.ScaffoldingPrefab;
		}

		private string GetPlatformPrefabName(global::Kampai.Game.BuildingDefinition buildingDefinition)
		{
			return buildingDefinition.PlatformPrefab;
		}

		private string GetRibbonPrefabName(global::Kampai.Game.BuildingDefinition buildingDefinition)
		{
			return buildingDefinition.RibbonPrefab;
		}

		internal void RemoveAllScaffoldingParts(int buildingId)
		{
			RemoveScaffoldingBuildingObject(buildingId);
			RemoveRibbonBuildingObject(buildingId);
			RemovePlatformBuildingObject(buildingId);
		}

		internal void RemoveScaffoldingBuildingObject(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection != null && buildingObjectCollection.ScaffoldingBuildingObject != null)
			{
				global::UnityEngine.Object.Destroy(buildingObjectCollection.ScaffoldingBuildingObject.gameObject);
				buildingObjectCollection.ScaffoldingBuildingObject = null;
			}
		}

		internal void RemovePlatformBuildingObject(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection != null && buildingObjectCollection.PlatformBuildingObject != null)
			{
				global::UnityEngine.Object.Destroy(buildingObjectCollection.PlatformBuildingObject.gameObject);
				buildingObjectCollection.PlatformBuildingObject = null;
			}
		}

		internal void RemoveRibbonBuildingObject(int buildingId)
		{
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(buildingId);
			if (buildingObjectCollection != null && buildingObjectCollection.RibbonBuildingObject != null)
			{
				global::UnityEngine.Object.Destroy(buildingObjectCollection.RibbonBuildingObject.gameObject);
				buildingObjectCollection.RibbonBuildingObject = null;
			}
		}

		internal void RemoveBuilding(int buildingId, bool destroyObject = true)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (!(buildingObject == null))
			{
				RemoveAllScaffoldingParts(buildingId);
				buildings.Remove(buildingId);
				if (destroyObject)
				{
					buildingObject.Cleanup();
					global::UnityEngine.Object.Destroy(buildingObject.gameObject);
				}
			}
		}

		private bool CanCreateScaffoldingPartBuildingObject(int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject == null)
			{
				logger.Warning("Building object is null, can't create a scaffolding part object");
				return false;
			}
			global::Kampai.Game.View.IRequiresBuildingScaffolding requiresBuildingScaffolding = buildingObject as global::Kampai.Game.View.IRequiresBuildingScaffolding;
			if (requiresBuildingScaffolding == null)
			{
				logger.Warning("Can't create a scaffolding part object on a building that does not require it.");
				return false;
			}
			return true;
		}

		private T CreateScaffoldingPartPrefab<T>(global::Kampai.Game.Building building, string prefabName, global::UnityEngine.Vector3 position) where T : global::UnityEngine.MonoBehaviour, global::Kampai.Game.View.IScaffoldingPart
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(prefabName));
			gameObject.name = "Building Cover";
			if (gameObject.transform.childCount > 0)
			{
				AddAnimEventHandlersToChildren(gameObject.transform.GetChild(0), null);
			}
			global::UnityEngine.Transform transform = gameObject.transform;
			transform.parent = base.transform;
			transform.position = position;
			transform.eulerAngles = global::UnityEngine.Vector3.zero;
			T result = gameObject.AddComponent<T>();
			result.Init(building, logger, definitionService);
			return result;
		}

		internal global::Kampai.Game.View.ScaffoldingBuildingObject CreateScaffoldingBuildingObject(global::Kampai.Game.Building building, global::UnityEngine.Vector3 position)
		{
			int iD = building.ID;
			if (!CanCreateScaffoldingPartBuildingObject(iD))
			{
				return null;
			}
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(iD);
			if (buildingObjectCollection.ScaffoldingBuildingObject != null)
			{
				return buildingObjectCollection.ScaffoldingBuildingObject;
			}
			global::Kampai.Game.View.ScaffoldingBuildingObject scaffoldingBuildingObject = (buildingObjectCollection.ScaffoldingBuildingObject = CreateScaffoldingPartPrefab<global::Kampai.Game.View.ScaffoldingBuildingObject>(building, GetScaffoldingPrefabName(building.Definition), position));
			AdjustObjectPosition(scaffoldingBuildingObject.transform, building);
			return scaffoldingBuildingObject;
		}

		internal global::Kampai.Game.View.PlatformBuildingObject CreatePlatformBuildingObject(global::Kampai.Game.Building building, global::UnityEngine.Vector3 position)
		{
			int iD = building.ID;
			if (!CanCreateScaffoldingPartBuildingObject(iD))
			{
				return null;
			}
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(iD);
			if (buildingObjectCollection.PlatformBuildingObject != null)
			{
				return buildingObjectCollection.PlatformBuildingObject;
			}
			return buildingObjectCollection.PlatformBuildingObject = CreateScaffoldingPartPrefab<global::Kampai.Game.View.PlatformBuildingObject>(building, GetPlatformPrefabName(building.Definition), position);
		}

		internal global::Kampai.Game.View.RibbonBuildingObject CreateRibbonBuildingObject(global::Kampai.Game.Building building, global::UnityEngine.Vector3 position)
		{
			int iD = building.ID;
			if (!CanCreateScaffoldingPartBuildingObject(iD))
			{
				return null;
			}
			global::Kampai.Game.View.BuildingObjectCollection buildingObjectCollection = GetBuildingObjectCollection(iD);
			if (buildingObjectCollection.RibbonBuildingObject != null)
			{
				return buildingObjectCollection.RibbonBuildingObject;
			}
			global::Kampai.Game.View.RibbonBuildingObject ribbonBuildingObject = (buildingObjectCollection.RibbonBuildingObject = CreateScaffoldingPartPrefab<global::Kampai.Game.View.RibbonBuildingObject>(building, GetRibbonPrefabName(building.Definition), position));
			AdjustObjectPosition(ribbonBuildingObject.transform, building);
			return ribbonBuildingObject;
		}

		internal bool Is8x8Building(global::Kampai.Game.BuildingDefinition buildingDef)
		{
			string platformPrefabName = GetPlatformPrefabName(buildingDef);
			if (string.IsNullOrEmpty(platformPrefabName))
			{
				return false;
			}
			return platformPrefabName.Contains("8x8");
		}

		private void AdjustObjectPosition(global::UnityEngine.Transform transform, global::Kampai.Game.Building building)
		{
			if (Is8x8Building(building.Definition))
			{
				foreach (global::UnityEngine.Transform item in transform)
				{
					if (item.name.Contains("_LOD"))
					{
						item.localPosition = new global::UnityEngine.Vector3(3.5f, 0f, -3.5f);
						break;
					}
				}
				global::UnityEngine.BoxCollider boxCollider = transform.GetComponent<global::UnityEngine.Collider>() as global::UnityEngine.BoxCollider;
				if (boxCollider != null)
				{
					boxCollider.center = new global::UnityEngine.Vector3(3.5f, 0f, -3.5f);
				}
			}
			if (building is global::Kampai.Game.MasterPlanComponentBuilding)
			{
				transform.position += masterPlanService.GetComponentBuildingOffset(building.Definition.ID);
			}
		}

		internal global::Kampai.Game.View.DummyBuildingObject CreateDummyBuilding(global::Kampai.Game.BuildingDefinition buildingDefinition, global::UnityEngine.Vector3 position)
		{
			int index = 0;
			global::Kampai.Game.ConnectableBuildingDefinition connectableBuildingDefinition = buildingDefinition as global::Kampai.Game.ConnectableBuildingDefinition;
			if (connectableBuildingDefinition != null)
			{
				index = connectableBuildingDefinition.GetDefaultPrefabIndex();
			}
			global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(buildingDefinition.GetPrefab(index));
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
			gameObject.name = "Dummy Building";
			global::UnityEngine.Transform transform = gameObject.transform;
			transform.parent = base.transform;
			transform.position = position;
			transform.eulerAngles = global::UnityEngine.Vector3.zero;
			global::Kampai.Game.View.DummyBuildingObject dummyBuildingObject = gameObject.AddComponent<global::Kampai.Game.View.DummyBuildingObject>();
			dummyBuildingObject.Init(buildingDefinition, definitionService);
			return dummyBuildingObject;
		}

		private void AddAnimation(global::UnityEngine.GameObject go)
		{
			global::UnityEngine.Animation animation = go.AddComponent<global::UnityEngine.Animation>();
			animation.playAutomatically = false;
			global::UnityEngine.AnimationClip clip = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.AnimationClip>("AnimBuildingReaction");
			animation.AddClip(clip, "AnimBuildingReaction");
			animation.clip = clip;
		}

		private void AddAnimEventHandlersToChildren(global::UnityEngine.Transform trans, global::Kampai.Game.View.BuildingObject buildingObject)
		{
			AddAnimEventHandler(trans, buildingObject);
			foreach (global::UnityEngine.Transform tran in trans)
			{
				AddAnimEventHandler(tran, buildingObject);
			}
		}

		private void AddAnimEventHandler(global::UnityEngine.Transform transform, global::Kampai.Game.View.BuildingObject buildingObject)
		{
			global::UnityEngine.GameObject gameObject = transform.gameObject;
			global::UnityEngine.Animator component = gameObject.GetComponent<global::UnityEngine.Animator>();
			global::Kampai.Game.View.AnimEventHandler animEventHandler = gameObject.GetComponent<global::Kampai.Game.View.AnimEventHandler>();
			if (component != null && animEventHandler == null)
			{
				global::Kampai.Game.View.AnimEventHandler animEventHandler2 = gameObject.AddComponent<global::Kampai.Game.View.AnimEventHandler>();
				animEventHandler2.Init(gameObject, audioSignal, stopAudioSignal, minionStateAudioSignal, startLoopingAudioSignal);
				animEventHandler = animEventHandler2;
			}
			if (animEventHandler != null && !animEventHandler.IsStopBuildingAudioSignalSet && buildingObject != null)
			{
				animEventHandler.SetStopBuildingAudioInIdleStateSignal(buildingObject.StopBuildingAudioInIdleStateSignal);
			}
		}

		internal void DestroyScaffolding(global::UnityEngine.GameObject scaffolding)
		{
			if (scaffolding != null)
			{
				global::UnityEngine.Object.Destroy(scaffolding);
			}
		}

		internal void SelectBuilding(int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null && buildingObject is global::Kampai.Game.View.ConnectableBuildingObject)
			{
				connectableBuildingPickedUpSignal.Dispatch(buildingId);
			}
			if (buildingObject != null)
			{
				selectedBuilding = buildingObject;
				global::UnityEngine.Vector3 position = selectedBuilding.transform.position;
				global::UnityEngine.Vector3 position2 = new global::UnityEngine.Vector3(position.x, position.y, position.z);
				selectedBuilding.transform.position = position2;
				selectedBuilding.SetBlendedColor(global::Kampai.Util.GameConstants.Building.VALID_PLACEMENT_COLOR);
				selectedBuilding.gameObject.SetLayerRecursively(14);
			}
			else
			{
				selectedBuilding = null;
			}
		}

		internal void DeselectBuilding(int buildingId)
		{
			if (selectedBuilding != null && selectedBuilding.ID == buildingId)
			{
				selectedBuilding.SetBlendedColor(global::UnityEngine.Color.clear);
				global::UnityEngine.Vector3 position = selectedBuilding.transform.position;
				selectedBuilding.transform.position = new global::UnityEngine.Vector3(position.x, 0f, position.z);
				selectedBuilding.gameObject.SetLayerRecursively(9);
				selectedBuilding = null;
			}
		}

		internal void MoveBuilding(int buildingID, global::UnityEngine.Vector3 position, bool isValidPosition)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingID);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.BuildingObject buildingObject2 = buildingObject;
				global::UnityEngine.Vector3 position2 = new global::UnityEngine.Vector3(global::UnityEngine.Mathf.Round(position.x), buildingObject2.transform.position.y, global::UnityEngine.Mathf.Round(position.z));
				buildingObject2.transform.position = position2;
				buildingObject.SetBlendedColor((!isValidPosition) ? global::Kampai.Util.GameConstants.Building.INVALID_PLACEMENT_COLOR : global::Kampai.Util.GameConstants.Building.VALID_PLACEMENT_COLOR);
			}
		}

		internal void SetBuildingPosition(int buildingId, global::UnityEngine.Vector3 position)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				buildingObject.transform.position = position;
			}
		}

		internal void PrepareTaskingMinionForMinionParty(global::Kampai.Game.TaskableBuilding taskableBuilding)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = buildings[taskableBuilding.ID].BuildingObject;
			if (buildingObject != null)
			{
				global::Kampai.Game.View.TaskableBuildingObject taskableBuildingObject = buildingObject as global::Kampai.Game.View.TaskableBuildingObject;
				if (taskableBuildingObject != null)
				{
					taskableBuildingObject.ReleaseMinionsForParty(taskableBuilding);
				}
			}
		}

		internal void StartMinionTask(global::Kampai.Game.TaskableBuilding building, global::Kampai.Game.View.MinionObject minionObject, bool alreadyRushed)
		{
			int iD = building.ID;
			if (building is global::Kampai.Game.MignetteBuilding)
			{
				global::Kampai.Game.View.MignetteBuildingObject component = buildings[iD].BuildingObject.GetComponent<global::Kampai.Game.View.MignetteBuildingObject>();
				component.LoadMignetteAnimationControllers(animationControllers);
			}
			string minionController = building.Definition.AnimationDefinitions[0].MinionController;
			logger.Info("Using controller {0}", minionController);
			global::UnityEngine.RuntimeAnimatorController controller = animationControllers[minionController];
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(iD);
			bool flag = false;
			if (buildingObject != null)
			{
				global::Kampai.Game.View.TaskableBuildingObject taskableBuildingObject = buildingObject as global::Kampai.Game.View.TaskableBuildingObject;
				if (taskableBuildingObject != null)
				{
					taskableBuildingObject.TrackChild(minionObject, controller, alreadyRushed);
					flag = taskableBuildingObject.IsGFXFaded();
				}
			}
			if (selectedBuilding == GetBuildingObject(iD) || flag)
			{
				minionObject.EnableRenderers(false);
			}
			else
			{
				minionObject.EnableRenderers(true);
			}
		}

		internal bool IsGagAnimationPlaying(int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.GaggableBuildingObject gaggableBuildingObject = buildingObject as global::Kampai.Game.View.GaggableBuildingObject;
				if (gaggableBuildingObject != null)
				{
					return gaggableBuildingObject.IsGagAnimationPlaying();
				}
			}
			return false;
		}

		internal void StopGagAnimation(int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.GaggableBuildingObject gaggableBuildingObject = buildingObject as global::Kampai.Game.View.GaggableBuildingObject;
				if (gaggableBuildingObject != null)
				{
					gaggableBuildingObject.StopGagAnimation();
				}
			}
		}

		internal void AppendMinionTaskAnimationCompleteCallback(global::Kampai.Game.View.MinionObject minionObject, global::strange.extensions.signal.impl.Signal<int> callback)
		{
			minionObject.EnqueueAction(new global::Kampai.Game.View.SendIDSignalAction(minionObject, callback, logger));
		}

		internal bool TriggerGagAnimation(int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.GaggableBuildingObject gaggableBuildingObject = buildingObject as global::Kampai.Game.View.GaggableBuildingObject;
				if (gaggableBuildingObject != null)
				{
					return gaggableBuildingObject.TriggerGagAnimation();
				}
			}
			return false;
		}

		internal void HarvestReady(int buildingId, int minionId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.TaskableBuildingObject taskableBuildingObject = buildingObject as global::Kampai.Game.View.TaskableBuildingObject;
				if (taskableBuildingObject != null)
				{
					taskableBuildingObject.RestMinion(minionId);
				}
			}
		}

		internal void UntrackMinion(int buildingId, int minionId, global::Kampai.Game.TaskableBuilding taskableBuilding)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.TaskableBuildingObject taskableBuildingObject = buildingObject as global::Kampai.Game.View.TaskableBuildingObject;
				if (taskableBuildingObject != null)
				{
					taskableBuildingObject.UntrackChild(minionId, taskableBuilding);
				}
			}
		}

		internal void UpdateBuildingState(int buildingId, global::Kampai.Game.BuildingState newState)
		{
			if (!buildings.ContainsKey(buildingId))
			{
				logger.Warning("No such building {0}", buildingId);
				return;
			}
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				global::Kampai.Game.View.AnimatingBuildingObject animatingBuildingObject = buildingObject as global::Kampai.Game.View.AnimatingBuildingObject;
				if (animatingBuildingObject != null)
				{
					animatingBuildingObject.SetState(newState);
				}
			}
		}

		private void LoadAnimationControllers(global::Kampai.Game.Building building)
		{
			global::Kampai.Game.AnimatingBuildingDefinition animatingBuildingDefinition = building.Definition as global::Kampai.Game.AnimatingBuildingDefinition;
			if (animatingBuildingDefinition == null)
			{
				return;
			}
			foreach (string item in animatingBuildingDefinition.AnimationControllerKeys())
			{
				if (!animationControllers.ContainsKey(item))
				{
					global::UnityEngine.RuntimeAnimatorController runtimeAnimatorController = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(item);
					if (runtimeAnimatorController == null)
					{
						logger.Fatal(global::Kampai.Util.FatalCode.BV_ILLEGAL_ANIMATION_CONTROLLER, item);
					}
					animationControllers.Add(item, runtimeAnimatorController);
				}
			}
		}

		public void setAnimParam(global::UnityEngine.GameObject building, string paramName, object value)
		{
			if (building.transform.childCount < 1)
			{
				return;
			}
			foreach (global::UnityEngine.Transform item in building.transform.GetChild(0))
			{
				global::UnityEngine.Animator component = item.gameObject.GetComponent<global::UnityEngine.Animator>();
				if (component != null)
				{
					if (value is bool)
					{
						component.SetBool(paramName, (bool)value);
					}
					else if (value is float)
					{
						component.SetFloat(paramName, (float)value);
					}
					else if (value is int)
					{
						component.SetInteger(paramName, (int)value);
					}
				}
			}
		}

		public void DestroyScaffoldingDelay(float delay, global::UnityEngine.GameObject scaffolding)
		{
			global::System.Action<global::UnityEngine.GameObject> action = DestroyScaffolding;
			StartCoroutine(Delay(delay, action, scaffolding));
		}

		public global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> GetFadedObjects()
		{
			global::System.Collections.Generic.LinkedList<global::Kampai.Game.View.ActionableObject> linkedList = new global::System.Collections.Generic.LinkedList<global::Kampai.Game.View.ActionableObject>();
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.View.BuildingObjectCollection> building in buildings)
			{
				global::Kampai.Game.View.BuildingObject buildingObject = building.Value.BuildingObject;
				if (!(buildingObject == null) && !(buildingObject is global::Kampai.Game.View.DebrisBuildingObject) && !(buildingObject is global::Kampai.Game.View.LandExpansionBuildingObject) && buildingObject.IsFaded())
				{
					linkedList.AddLast(buildingObject);
				}
			}
			return linkedList;
		}

		public global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> GetOccludingObjects(int buildingId)
		{
			global::System.Collections.Generic.LinkedList<global::Kampai.Game.View.ActionableObject> linkedList = new global::System.Collections.Generic.LinkedList<global::Kampai.Game.View.ActionableObject>();
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject == null)
			{
				return linkedList;
			}
			GetOccludingObjects(buildingObject.transform.position, buildingId, linkedList, null);
			return linkedList;
		}

		public void GetOccludingObjects(global::UnityEngine.Vector3 position, int buildingId, global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> occludingObjects, global::System.Collections.Generic.ICollection<global::Kampai.Game.View.ActionableObject> nonOccludingObjects)
		{
			if (buildingId == 313)
			{
				return;
			}
			global::UnityEngine.Vector3 vector = position;
			global::UnityEngine.Vector2 vector2 = new global::UnityEngine.Vector2(vector.x, vector.z);
			global::UnityEngine.Vector3 forward = mainCamera.transform.forward;
			global::UnityEngine.Vector2 rhs = -new global::UnityEngine.Vector2(forward.x, forward.z);
			rhs.Normalize();
			global::UnityEngine.Plane[] planes = global::UnityEngine.GeometryUtility.CalculateFrustumPlanes(mainCamera);
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.View.BuildingObjectCollection> building in buildings)
			{
				if (building.Key == buildingId)
				{
					continue;
				}
				global::Kampai.Game.View.BuildingObject buildingObject = building.Value.BuildingObject;
				if (!(buildingObject == null) && !(buildingObject is global::Kampai.Game.View.DebrisBuildingObject) && !(buildingObject is global::Kampai.Game.View.LandExpansionBuildingObject))
				{
					global::UnityEngine.Vector3 position2 = buildingObject.transform.position;
					global::UnityEngine.Vector2 vector3 = new global::UnityEngine.Vector2(position2.x, position2.z);
					global::UnityEngine.Vector2 lhs = vector3 - vector2;
					bool flag = true;
					if (buildingObject.GetComponent<global::UnityEngine.Collider>() != null)
					{
						flag = global::UnityEngine.GeometryUtility.TestPlanesAABB(planes, buildingObject.GetComponent<global::UnityEngine.Collider>().bounds);
					}
					if (flag && global::UnityEngine.Vector2.Dot(lhs, rhs) > 0f)
					{
						occludingObjects.Add(buildingObject);
					}
					else if (nonOccludingObjects != null)
					{
						nonOccludingObjects.Add(buildingObject);
					}
				}
			}
		}

		private global::System.Collections.IEnumerator Delay(float t, global::System.Action<global::UnityEngine.GameObject> action, global::UnityEngine.GameObject actionArg)
		{
			yield return new global::UnityEngine.WaitForSeconds(t);
			action(actionArg);
		}

		internal void TweenBuildingToMenu(global::UnityEngine.GameObject scaffolding, global::UnityEngine.Vector3 destination, global::System.Action<global::UnityEngine.GameObject> onTweenDone)
		{
			Go.to(scaffolding.transform, 0.5f, new GoTweenConfig().setEaseType(GoEaseType.Linear).scale(0f).rotation(new global::UnityEngine.Vector3(90f, 90f, 90f))
				.position(destination)
				.onComplete(delegate(AbstractGoTween thisTween)
				{
					thisTween.destroy();
					if (onTweenDone != null)
					{
						onTweenDone(scaffolding);
					}
				}));
		}

		internal void ToInventory(int buildingID)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingID);
			if (!(buildingObject == null))
			{
				toInventoryBuildingObject = buildingObject;
				global::UnityEngine.Vector3 destination = BuildingUtil.UIToWorldCoords(mainCamera, global::UnityEngine.Vector3.zero);
				TweenBuildingToMenu(toInventoryBuildingObject.gameObject, destination, DestroyBuilding);
			}
		}

		internal void DestroyBuilding(global::UnityEngine.GameObject inventoryGO)
		{
			CleanupBuilding(inventoryGO.GetComponent<global::Kampai.Game.View.BuildingObject>().ID);
		}

		internal void CleanupBuilding(int buildingId)
		{
			RemoveBuilding(buildingId);
		}

		public global::Kampai.Util.VFXScript GetVFXScriptForBuilding(int buildingId)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingId);
			if (buildingObject != null)
			{
				return buildingObject.GetComponent<global::Kampai.Util.VFXScript>();
			}
			return null;
		}

		public void PreloadBuildingMinionParty(int buildingID, global::Kampai.Game.IMinionPartyBuilding building, global::Kampai.Game.Location buildingPosition, global::Kampai.Game.MinionPartyType partyType)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingID);
			if (buildingObject == null || buildingObject.MinionPartyDecorations != null)
			{
				return;
			}
			string partyPrefab = building.GetPartyPrefab(partyType);
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(partyPrefab);
			if (gameObject != null)
			{
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
				if (gameObject2 != null)
				{
					gameObject2.transform.position = new global::UnityEngine.Vector3(buildingPosition.x, 0f, buildingPosition.y);
					gameObject2.transform.rotation = global::UnityEngine.Quaternion.identity;
					gameObject2.transform.SetParent(buildingObject.transform);
					buildingObject.MinionPartyDecorations = gameObject2;
					gameObject2.SetActive(false);
				}
			}
			else
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Debug, "Trying to instantiate null prefab: {0}", partyPrefab);
			}
		}

		public void StartBuildingMinionParty(int buildingID, global::Kampai.Game.IMinionPartyBuilding building, global::Kampai.Game.Location buildingPosition, global::Kampai.Game.MinionPartyType partyType)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingID);
			if (buildingObject == null)
			{
				logger.Error("Trying to display an Minion Building object that is not there.");
				return;
			}
			if (buildingObject.MinionPartyDecorations == null)
			{
				PreloadBuildingMinionParty(buildingID, building, buildingPosition, partyType);
			}
			buildingObject.MinionPartyDecorations.SetActive(true);
		}

		public void EndBuildingMinionParty(int buildingID)
		{
			global::Kampai.Game.View.BuildingObject buildingObject = GetBuildingObject(buildingID);
			if (!(buildingObject == null) && !(buildingObject.MinionPartyDecorations == null))
			{
				global::UnityEngine.Object.Destroy(buildingObject.MinionPartyDecorations);
				buildingObject.MinionPartyDecorations = null;
			}
		}
	}
}
