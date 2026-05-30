namespace Kampai.UI.View
{
	public class WayFinderPanelView : global::Kampai.Util.KampaiView
	{
		private delegate bool PriorityFunction(global::Kampai.UI.View.IWayFinderView wayFinderView);

		private global::System.Collections.Generic.Dictionary<int, global::Kampai.UI.View.IWayFinderView> trackedWayFinders;

		private global::System.Collections.Generic.Dictionary<int, global::Kampai.UI.View.IParentWayFinderView> trackedParentWayFinders;

		private global::Kampai.Util.IKampaiLogger logger;

		private global::Kampai.Game.ITikiBarService tikiBarService;

		private global::Kampai.Game.IPlayerService playerService;

		private global::Kampai.Game.IPrestigeService prestigeService;

		private global::Kampai.UI.View.WayFinderSettings tikiBarParentWayFinderSettings;

		private global::Kampai.UI.View.WayFinderSettings cabanaParentWayFinderSettings;

		private global::Kampai.UI.View.WayFinderSettings orderBoardWayFinderSettings;

		private global::Kampai.UI.View.WayFinderSettings storageBuildingWayFinderSettings;

		private global::Kampai.UI.View.WayFinderSettings stageBuildingWayFinderSettings;

		private int specialEventCharacterId;

		private int tsmWayFinderTrackedId;

		private bool isForceHideEnabled;

		private global::Kampai.Common.PickControllerModel pickControllerModel;

		private global::Kampai.Game.VillainLairModel lairModel;

		private global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction>> allPriorityFunctions;

		internal void Init(global::Kampai.Util.IKampaiLogger logger, global::Kampai.Game.ITikiBarService tikiBarService, global::Kampai.Game.IPlayerService playerService, global::Kampai.Game.IPrestigeService prestigeService, global::Kampai.UI.IPositionService positionService, global::Kampai.Common.PickControllerModel pickControllerModel, global::Kampai.Game.VillainLairModel lairModel)
		{
			this.logger = logger;
			this.tikiBarService = tikiBarService;
			this.playerService = playerService;
			this.prestigeService = prestigeService;
			this.pickControllerModel = pickControllerModel;
			this.lairModel = lairModel;
			tikiBarParentWayFinderSettings = new global::Kampai.UI.View.WayFinderSettings(313);
			cabanaParentWayFinderSettings = new global::Kampai.UI.View.WayFinderSettings(1000008087);
			orderBoardWayFinderSettings = new global::Kampai.UI.View.WayFinderSettings(309);
			storageBuildingWayFinderSettings = new global::Kampai.UI.View.WayFinderSettings(314);
			stageBuildingWayFinderSettings = new global::Kampai.UI.View.WayFinderSettings(370);
			global::Kampai.Game.SpecialEventCharacter firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.SpecialEventCharacter>(70009);
			if (firstInstanceByDefinitionId != null)
			{
				specialEventCharacterId = firstInstanceByDefinitionId.ID;
			}
			tsmWayFinderTrackedId = 301;
			trackedWayFinders = new global::System.Collections.Generic.Dictionary<int, global::Kampai.UI.View.IWayFinderView>();
			trackedParentWayFinders = new global::System.Collections.Generic.Dictionary<int, global::Kampai.UI.View.IParentWayFinderView>();
			allPriorityFunctions = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction>>();
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeKevinWayfinder });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeVillainEntrance });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeSpecialEvent });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeQuestComplete, PrioritizeAtTikiBar });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeQuestComplete, PrioritizeAtCabana });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeNewQuest, PrioritizeAtTikiBar });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeNewQuest, PrioritizeAtCabana });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeTaskComplete, PrioritizeAtTikiBar });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeTaskComplete, PrioritizeAtCabana });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeBob });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeQuestAvailable, PrioritizeAtTikiBar });
			allPriorityFunctions.Add(new global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> { PrioritizeQuestAvailable, PrioritizeAtCabana });
			UpdateHUDSnap(positionService);
			UpdateWayFinderPriority();
		}

		internal void UpdateHUDSnap(global::Kampai.UI.IPositionService positionService)
		{
			global::System.Collections.Generic.List<global::UnityEngine.GameObject> list = GenerateListOfObjectsToSnapAround();
			foreach (global::UnityEngine.GameObject item in list)
			{
				positionService.AddHUDElementToAvoid(item);
			}
		}

		private global::System.Collections.Generic.List<global::UnityEngine.GameObject> GenerateListOfObjectsToSnapAround()
		{
			global::System.Collections.Generic.List<global::UnityEngine.GameObject> list = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();
			list.Add(global::UnityEngine.GameObject.Find("btn_OpenStore"));
			list.Add(global::UnityEngine.GameObject.Find("btn_Settings"));
			list.Add(global::UnityEngine.GameObject.Find("group_Storage"));
			list.Add(global::UnityEngine.GameObject.Find("group_Currency_Grind"));
			list.Add(global::UnityEngine.GameObject.Find("group_Shopping"));
			global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find("sale_snapTarget");
			if (gameObject != null)
			{
				list.Add(gameObject);
			}
			return list;
		}

		private bool PrioritizeBob(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			return IsBobPointsAtStuffWayFinder(wayFinderView.Prestige);
		}

		private bool PrioritizeAtCabana(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			return IsCabanaChildWayFinder(wayFinderView.Prestige);
		}

		private bool PrioritizeAtTikiBar(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			return IsTikiBarChildWayFinder(wayFinderView.Prestige);
		}

		private bool PrioritizeVillainEntrance(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			return wayFinderView.TrackedId == 374;
		}

		private bool PrioritizeKevinWayfinder(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			int num = 0;
			global::Kampai.Game.KevinCharacter firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.KevinCharacter>(70003);
			if (firstInstanceByDefinitionId != null)
			{
				num = firstInstanceByDefinitionId.ID;
			}
			if (wayFinderView.TrackedId != num)
			{
				return false;
			}
			return true;
		}

		private bool PrioritizeSpecialEvent(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			return wayFinderView as global::Kampai.UI.View.SpecialEventWayFinderView != null;
		}

		private bool PrioritizeQuestComplete(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			global::Kampai.UI.View.AbstractQuestWayFinderView abstractQuestWayFinderView = wayFinderView as global::Kampai.UI.View.AbstractQuestWayFinderView;
			if (abstractQuestWayFinderView != null)
			{
				return abstractQuestWayFinderView.IsQuestComplete();
			}
			return false;
		}

		private bool PrioritizeQuestAvailable(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			global::Kampai.UI.View.AbstractQuestWayFinderView abstractQuestWayFinderView = wayFinderView as global::Kampai.UI.View.AbstractQuestWayFinderView;
			if (abstractQuestWayFinderView != null)
			{
				return abstractQuestWayFinderView.IsQuestAvailable();
			}
			return false;
		}

		private bool PrioritizeNewQuest(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			global::Kampai.UI.View.AbstractQuestWayFinderView abstractQuestWayFinderView = wayFinderView as global::Kampai.UI.View.AbstractQuestWayFinderView;
			if (abstractQuestWayFinderView != null)
			{
				return abstractQuestWayFinderView.IsNewQuestAvailable();
			}
			return false;
		}

		private bool PrioritizeTaskComplete(global::Kampai.UI.View.IWayFinderView wayFinderView)
		{
			global::Kampai.UI.View.AbstractQuestWayFinderView abstractQuestWayFinderView = wayFinderView as global::Kampai.UI.View.AbstractQuestWayFinderView;
			if (abstractQuestWayFinderView != null)
			{
				return abstractQuestWayFinderView.IsTaskReady();
			}
			return false;
		}

		internal void Cleanup()
		{
			if (trackedWayFinders != null)
			{
				trackedWayFinders.Clear();
			}
			if (trackedParentWayFinders != null)
			{
				trackedParentWayFinders.Clear();
			}
			if (allPriorityFunctions != null)
			{
				allPriorityFunctions.Clear();
			}
		}

		private bool IsTikiBarParentWayFinder(int trackedId)
		{
			return trackedId == tikiBarParentWayFinderSettings.TrackedId;
		}

		private bool IsTikiBarChildWayFinder(global::Kampai.Game.Prestige prestige)
		{
			return prestige != null && tikiBarService.IsCharacterSitting(prestige);
		}

		private bool IsOrderBoardWayFinder(int trackedId)
		{
			return trackedId == orderBoardWayFinderSettings.TrackedId;
		}

		private bool IsCabanaParentWayFinder(int trackedId)
		{
			return trackedId == cabanaParentWayFinderSettings.TrackedId;
		}

		private bool IsStorageBuildingWayFinder(int trackedId)
		{
			return trackedId == storageBuildingWayFinderSettings.TrackedId;
		}

		private bool IsStageBuildingWayFinder(int trackedId)
		{
			return trackedId == stageBuildingWayFinderSettings.TrackedId;
		}

		private bool IsTSMWayFinder(int trackedId)
		{
			return trackedId == tsmWayFinderTrackedId;
		}

		private bool IsMoveBuildingWayFinder(int trackedId)
		{
			if (trackedId == -1)
			{
				return true;
			}
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(trackedId);
			if (byInstanceId != null && (byInstanceId is global::Kampai.Game.BridgeBuilding || byInstanceId is global::Kampai.Game.FountainBuilding || byInstanceId is global::Kampai.Game.StageBuilding || byInstanceId is global::Kampai.Game.WelcomeHutBuilding || byInstanceId.State == global::Kampai.Game.BuildingState.Complete))
			{
				return false;
			}
			return byInstanceId != null && pickControllerModel.CurrentMode == global::Kampai.Common.PickControllerModel.Mode.DragAndDrop;
		}

		private bool IsLairEntranceWayfinder(int trackedId)
		{
			if (trackedId != 374)
			{
				return false;
			}
			global::Kampai.Game.VillainLairEntranceBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.VillainLairEntranceBuilding>(374);
			return byInstanceId.State != global::Kampai.Game.BuildingState.Inaccessible;
		}

		private bool IsCabanaChildWayFinder(global::Kampai.Game.Prestige prestige)
		{
			if (prestige != null)
			{
				global::Kampai.Game.PrestigeDefinition definition = prestige.Definition;
				return definition.Type == global::Kampai.Game.PrestigeType.Villain && definition.ID != 40001;
			}
			return false;
		}

		private bool IsBobPointsAtStuffWayFinder(global::Kampai.Game.Prestige prestige)
		{
			if (prestige != null)
			{
				return prestige.Definition.ID == 40002;
			}
			return false;
		}

		private bool IsFluxLairWayFinder(global::Kampai.Game.Prestige prestige)
		{
			if (prestige != null)
			{
				return prestige.Definition.ID == 40001;
			}
			return false;
		}

		private bool IsSpecialEventMinionWayFinder(int trackedId)
		{
			return specialEventCharacterId > 0 && trackedId == specialEventCharacterId;
		}

		private bool IsMignetteWayFinder(int trackedId)
		{
			return playerService.GetByInstanceId<global::Kampai.Game.MignetteBuilding>(trackedId) != null;
		}

		internal global::Kampai.UI.View.IWayFinderView CreateWayFinder(global::Kampai.UI.View.WayFinderSettings settings, bool updatePriority = true)
		{
			int trackedId = settings.TrackedId;
			global::Kampai.Game.Prestige prestigeForWayFinder = GetPrestigeForWayFinder(trackedId);
			if (IsFluxLairWayFinder(prestigeForWayFinder) && (lairModel == null || lairModel.currentActiveLair == null))
			{
				return null;
			}
			global::Kampai.UI.View.IWayFinderView wayFinderView = null;
			if ((wayFinderView = GetWayFinder(trackedId)) != null)
			{
				return wayFinderView;
			}
			wayFinderView = SetupWayFinder(settings);
			trackedWayFinders[trackedId] = wayFinderView;
			PostCreateWayFinder(wayFinderView, updatePriority);
			return wayFinderView;
		}

		private global::Kampai.UI.View.IWayFinderView SetupWayFinder(global::Kampai.UI.View.WayFinderSettings settings)
		{
			int trackedId = settings.TrackedId;
			bool isQuest = settings.QuestDefId > 0;
			global::Kampai.Game.Prestige prestigeForWayFinder = GetPrestigeForWayFinder(trackedId);
			bool flag = IsCabanaChildWayFinder(prestigeForWayFinder);
			bool flag2 = IsTikiBarChildWayFinder(prestigeForWayFinder);
			global::Kampai.UI.View.IParentWayFinderView parentWayFinderView = null;
			if (flag)
			{
				parentWayFinderView = CreateWayFinder(cabanaParentWayFinderSettings) as global::Kampai.UI.View.IParentWayFinderView;
			}
			else if (flag2)
			{
				parentWayFinderView = CreateWayFinder(tikiBarParentWayFinderSettings) as global::Kampai.UI.View.IParentWayFinderView;
			}
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(global::Kampai.Util.KampaiResources.Load("cmp_WayFinder")) as global::UnityEngine.GameObject;
			gameObject.transform.SetParent(base.transform, false);
			gameObject.SetActive(true);
			global::Kampai.UI.View.WayFinderModal component = gameObject.GetComponent<global::Kampai.UI.View.WayFinderModal>();
			component.Settings = settings;
			component.Prestige = prestigeForWayFinder;
			return AddWayFinderViewComponent(gameObject, trackedId, prestigeForWayFinder, parentWayFinderView, isQuest, flag, flag2);
		}

		private global::Kampai.UI.View.IWayFinderView AddWayFinderViewComponent(global::UnityEngine.GameObject wayFinderGO, int trackedId, global::Kampai.Game.Prestige prestige, global::Kampai.UI.View.IParentWayFinderView parentWayFinderView, bool isQuest, bool isCabanaChildWayFinder, bool isTikiBarChildWayFinder)
		{
			global::Kampai.UI.View.IWayFinderView wayFinderView = null;
			if (IsCabanaParentWayFinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.CabanaParentWayFinderView>();
				trackedParentWayFinders.Add(trackedId, wayFinderView as global::Kampai.UI.View.CabanaParentWayFinderView);
			}
			else if (isCabanaChildWayFinder)
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.CabanaChildWayFinderView>();
				global::Kampai.UI.View.CabanaChildWayFinderView childWayFinderView = wayFinderView as global::Kampai.UI.View.CabanaChildWayFinderView;
				parentWayFinderView.AddChildWayFinder(childWayFinderView);
			}
			else if (IsTikiBarParentWayFinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.TikiBarParentWayFinderView>();
				trackedParentWayFinders.Add(trackedId, wayFinderView as global::Kampai.UI.View.TikiBarParentWayFinderView);
			}
			else if (isTikiBarChildWayFinder)
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.TikiBarChildWayFinderView>();
				global::Kampai.UI.View.TikiBarChildWayFinderView childWayFinderView2 = wayFinderView as global::Kampai.UI.View.TikiBarChildWayFinderView;
				parentWayFinderView.AddChildWayFinder(childWayFinderView2);
			}
			else if (IsStageBuildingWayFinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.StageBuildingWayFinderView>();
			}
			else if (IsOrderBoardWayFinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.OrderBoardWayFinderView>();
			}
			else if (IsLairEntranceWayfinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.LairEntranceWayfinderView>();
			}
			else if (IsMignetteWayFinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.MignetteWayFinderView>();
			}
			else if (isQuest)
			{
				wayFinderView = (IsTSMWayFinder(trackedId) ? wayFinderGO.AddComponent<global::Kampai.UI.View.TSMWayFinderView>() : ((!IsSpecialEventMinionWayFinder(trackedId)) ? ((global::Kampai.UI.View.AbstractQuestWayFinderView)wayFinderGO.AddComponent<global::Kampai.UI.View.QuestWayFinderView>()) : ((global::Kampai.UI.View.AbstractQuestWayFinderView)wayFinderGO.AddComponent<global::Kampai.UI.View.SpecialEventWayFinderView>())));
			}
			else if (IsBobPointsAtStuffWayFinder(prestige))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.BobPointsAtStuffWayFinderView>();
			}
			else if (IsFluxLairWayFinder(prestige))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.VolcanoLairWayfinderView>();
			}
			else if (IsStorageBuildingWayFinder(trackedId))
			{
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.StorageBuildingWayFinderView>();
			}
			else if (!IsTSMWayFinder(trackedId))
			{
				wayFinderView = ((!IsMoveBuildingWayFinder(trackedId)) ? ((global::Kampai.UI.View.AbstractWayFinderView)wayFinderGO.AddComponent<global::Kampai.UI.View.WayFinderView>()) : ((global::Kampai.UI.View.AbstractWayFinderView)wayFinderGO.AddComponent<global::Kampai.UI.View.MoveBuildingWayFinderView>()));
			}
			else
			{
				logger.Info("Way finder with tracking id: {0} is on tsm for a trigger", trackedId);
				wayFinderView = wayFinderGO.AddComponent<global::Kampai.UI.View.TSMTriggerWayFinderView>();
			}
			return wayFinderView;
		}

		private void PostCreateWayFinder(global::Kampai.UI.View.IWayFinderView wayFinderView, bool updatePriority)
		{
			if (playerService.GetHighestFtueCompleted() < 3 && trackedWayFinders.Count > 2)
			{
				ForceHideTikiBarWayFinders(true);
			}
			wayFinderView.SetForceHide(isForceHideEnabled);
			if (updatePriority)
			{
				UpdateWayFinderPriority();
			}
		}

		private global::Kampai.Game.Prestige GetPrestigeForWayFinder(int trackedId)
		{
			global::Kampai.Game.Character byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Character>(trackedId);
			if (byInstanceId != null)
			{
				return tikiBarService.GetPrestigeForSeatableCharacter(byInstanceId);
			}
			global::Kampai.Game.CabanaBuilding byInstanceId2 = playerService.GetByInstanceId<global::Kampai.Game.CabanaBuilding>(trackedId);
			if (byInstanceId2 != null)
			{
				global::System.Collections.Generic.List<global::Kampai.Game.Villain> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Villain>();
				foreach (global::Kampai.Game.Villain item in instancesByType)
				{
					if (item.CabanaBuildingId == byInstanceId2.ID)
					{
						return prestigeService.GetPrestigeFromMinionInstance(item);
					}
				}
			}
			return null;
		}

		internal void RemoveWayFinder(int trackedId, bool updatePriority = true)
		{
			global::Kampai.UI.View.IWayFinderView wayFinderView = null;
			if ((wayFinderView = GetWayFinder(trackedId)) == null)
			{
				return;
			}
			trackedWayFinders.Remove(trackedId);
			global::Kampai.UI.View.IChildWayFinderView childWayFinderView = wayFinderView as global::Kampai.UI.View.IChildWayFinderView;
			if (childWayFinderView != null && childWayFinderView.ParentWayFinderTrackedId > 0)
			{
				global::Kampai.UI.View.IParentWayFinderView parentWayFinderView = GetWayFinder(childWayFinderView.ParentWayFinderTrackedId) as global::Kampai.UI.View.IParentWayFinderView;
				if (parentWayFinderView != null)
				{
					parentWayFinderView.RemoveChildWayFinder(trackedId);
					global::System.Collections.Generic.Dictionary<int, global::Kampai.UI.View.IChildWayFinderView> childrenWayFinders = parentWayFinderView.ChildrenWayFinders;
					if (childrenWayFinders != null && childrenWayFinders.Count == 0)
					{
						RemoveWayFinder(parentWayFinderView.TrackedId, false);
					}
				}
			}
			else
			{
				global::Kampai.UI.View.IParentWayFinderView parentWayFinderView2 = wayFinderView as global::Kampai.UI.View.IParentWayFinderView;
				if (parentWayFinderView2 != null)
				{
					global::System.Collections.Generic.Dictionary<int, global::Kampai.UI.View.IChildWayFinderView> childrenWayFinders2 = parentWayFinderView2.ChildrenWayFinders;
					if (childrenWayFinders2 != null && childrenWayFinders2.Count > 0)
					{
						foreach (global::Kampai.UI.View.IChildWayFinderView value in childrenWayFinders2.Values)
						{
							RemoveWayFinder(value.TrackedId, false);
						}
						return;
					}
					trackedParentWayFinders.Remove(trackedId);
				}
			}
			global::UnityEngine.Object.Destroy(wayFinderView.GameObject);
			if (updatePriority)
			{
				UpdateWayFinderPriority();
			}
		}

		internal global::Kampai.UI.View.IWayFinderView GetWayFinder(int trackedId)
		{
			if (trackedWayFinders != null && trackedWayFinders.ContainsKey(trackedId))
			{
				return trackedWayFinders[trackedId];
			}
			return null;
		}

		internal void AddQuestToExistingWayFinder(int questDefId, int trackedId)
		{
			global::Kampai.UI.View.IWayFinderView wayFinder = GetWayFinder(trackedId);
			if (wayFinder != null)
			{
				global::Kampai.UI.View.IQuestWayFinderView questWayFinderView = wayFinder as global::Kampai.UI.View.IQuestWayFinderView;
				if (questWayFinderView != null)
				{
					questWayFinderView.AddQuest(questDefId);
				}
			}
		}

		internal void RemoveQuestFromExistingWayFinder(int questDefId, int trackedId)
		{
			global::Kampai.UI.View.IWayFinderView wayFinder = GetWayFinder(trackedId);
			if (wayFinder != null)
			{
				global::Kampai.UI.View.IQuestWayFinderView questWayFinderView = wayFinder as global::Kampai.UI.View.IQuestWayFinderView;
				if (questWayFinderView != null)
				{
					questWayFinderView.RemoveQuest(questDefId);
				}
			}
		}

		private void SetForceHideForAllWayFinders()
		{
			foreach (global::Kampai.UI.View.IWayFinderView value in trackedWayFinders.Values)
			{
				value.SetForceHide(isForceHideEnabled);
			}
		}

		internal void HideAllWayFinders()
		{
			isForceHideEnabled = true;
			SetForceHideForAllWayFinders();
		}

		internal void ShowAllWayFinders()
		{
			isForceHideEnabled = false;
			SetForceHideForAllWayFinders();
		}

		internal void SetLimitTikiBarWayFinders(bool limitTikiBarWayFinders)
		{
			ForceHideTikiBarWayFinders(limitTikiBarWayFinders);
		}

		internal void UpdateWayFinderPriority()
		{
			if (lairModel == null || lairModel.currentActiveLair == null)
			{
				global::System.Collections.Generic.List<int> toRemove = new global::System.Collections.Generic.List<int>();
				foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.UI.View.IWayFinderView> pair in trackedWayFinders)
				{
					if (pair.Value != null)
					{
						global::Kampai.Game.Prestige prestige = GetPrestigeForWayFinder(pair.Key);
						if (IsFluxLairWayFinder(prestige))
						{
							toRemove.Add(pair.Key);
						}
					}
				}
				foreach (int id in toRemove)
				{
					RemoveWayFinder(id, false);
				}
			}
			foreach (global::Kampai.UI.View.IParentWayFinderView value in trackedParentWayFinders.Values)
			{
				value.UpdateWayFinderIcon();
			}
			if (playerService.GetHighestFtueCompleted() < 9)
			{
				return;
			}
			global::Kampai.UI.View.IWayFinderView prioritizedWayFinder = GetPrioritizedWayFinder();
			if (prioritizedWayFinder != null)
			{
				SetWayFinderSnappable(prioritizedWayFinder);
				if (prioritizedWayFinder.TrackedId != orderBoardWayFinderSettings.TrackedId && GetWayFinder(orderBoardWayFinderSettings.TrackedId) != null)
				{
					RemoveWayFinder(orderBoardWayFinderSettings.TrackedId, false);
				}
			}
			else
			{
				global::Kampai.UI.View.IWayFinderView wayFinder = GetWayFinder(orderBoardWayFinderSettings.TrackedId);
				if (wayFinder == null)
				{
					wayFinder = CreateWayFinder(orderBoardWayFinderSettings, false);
				}
			}
		}

		private global::Kampai.UI.View.IWayFinderView GetPrioritizedWayFinder()
		{
			int count = trackedWayFinders.Count;
			if (count <= 0)
			{
				return null;
			}
			foreach (global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> allPriorityFunction in allPriorityFunctions)
			{
				foreach (global::Kampai.UI.View.IWayFinderView value in trackedWayFinders.Values)
				{
					if (value.TrackedId == 301 || !PassesPriorityFunctions(value, allPriorityFunction))
					{
						continue;
					}
					return value;
				}
			}
			return null;
		}

		private bool PassesPriorityFunctions(global::Kampai.UI.View.IWayFinderView wayFinderView, global::System.Collections.Generic.List<global::Kampai.UI.View.WayFinderPanelView.PriorityFunction> priorityFunctions)
		{
			foreach (global::Kampai.UI.View.WayFinderPanelView.PriorityFunction priorityFunction in priorityFunctions)
			{
				if (!priorityFunction(wayFinderView))
				{
					return false;
				}
			}
			return true;
		}

		private void SetWayFinderSnappable(global::Kampai.UI.View.IWayFinderView snappableWayFinderView)
		{
			foreach (global::Kampai.UI.View.IWayFinderView value in trackedWayFinders.Values)
			{
				value.Snappable = false;
			}
			global::Kampai.UI.View.IChildWayFinderView childWayFinderView = snappableWayFinderView as global::Kampai.UI.View.IChildWayFinderView;
			if (childWayFinderView != null)
			{
				int parentWayFinderTrackedId = childWayFinderView.ParentWayFinderTrackedId;
				global::Kampai.UI.View.IParentWayFinderView parentWayFinderView = GetWayFinder(parentWayFinderTrackedId) as global::Kampai.UI.View.IParentWayFinderView;
				if (parentWayFinderTrackedId == 313)
				{
					snappableWayFinderView.Snappable = true;
					return;
				}
				if (parentWayFinderView != null)
				{
					parentWayFinderView.Snappable = true;
					return;
				}
				logger.Warning("Parent way finder with tracked id: {0} does not exist, child with id:{1} has no parent!", parentWayFinderTrackedId, childWayFinderView.TrackedId);
			}
			else
			{
				snappableWayFinderView.Snappable = true;
			}
		}

		private void ForceHideTikiBarWayFinders(bool hide)
		{
			global::Kampai.UI.View.IWayFinderView wayFinder = GetWayFinder(78);
			global::Kampai.UI.View.IWayFinderView wayFinder2 = GetWayFinder(313);
			if (wayFinder != null && wayFinder2 != null)
			{
				wayFinder.SetForceHide(hide);
				wayFinder2.SetForceHide(hide);
			}
		}
	}
}
