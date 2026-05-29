namespace Kampai.Game.View
{
	public abstract class TaskableBuildingObject : global::Kampai.Game.View.AnimatingBuildingObject
	{
		protected global::System.Collections.Generic.Dictionary<int, global::UnityEngine.RuntimeAnimatorController> minionControllers;

		private int[] layerIndicies;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.TaskingMinionObject> childAnimators = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.TaskingMinionObject>();

		protected global::System.Collections.Generic.List<global::Kampai.Game.View.TaskingMinionObject> childQueue = new global::System.Collections.Generic.List<global::Kampai.Game.View.TaskingMinionObject>();

		internal override void Init(global::Kampai.Game.Building building, global::Kampai.Util.IKampaiLogger logger, global::System.Collections.Generic.IDictionary<string, global::UnityEngine.RuntimeAnimatorController> controllers, global::Kampai.Game.IDefinitionService definitionService)
		{
			base.Init(building, logger, controllers, definitionService);
			global::Kampai.Game.TaskableBuildingDefinition taskableBuildingDefinition = building.Definition as global::Kampai.Game.TaskableBuildingDefinition;
			if (taskableBuildingDefinition == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.BV_ILLEGAL_TASKABLE_DEFINITION, building.Definition.ID.ToString());
			}
			minionControllers = new global::System.Collections.Generic.Dictionary<int, global::UnityEngine.RuntimeAnimatorController>();
			if (this is global::Kampai.Game.View.MignetteBuildingObject)
			{
				return;
			}
			foreach (global::Kampai.Game.BuildingAnimationDefinition animationDefinition in taskableBuildingDefinition.AnimationDefinitions)
			{
				if (controllers.ContainsKey(animationDefinition.MinionController))
				{
					minionControllers.Add(animationDefinition.CostumeId, controllers[animationDefinition.MinionController]);
				}
			}
			if (!minionControllers.ContainsKey(-1))
			{
				logger.Fatal(global::Kampai.Util.FatalCode.BV_NO_DEFAULT_ANIMATION_CONTROLLER, taskableBuildingDefinition.ID.ToString());
			}
		}

		internal void SetupLayers()
		{
			if (animators.Count == 0)
			{
				return;
			}
			int layerCount = animators[0].layerCount;
			layerIndicies = new int[stations];
			for (int i = 0; i < layerIndicies.Length; i++)
			{
				layerIndicies[i] = -1;
			}
			for (int j = 0; j < layerCount; j++)
			{
				string layerName = animators[0].GetLayerName(j);
				if (layerName.StartsWith("Pos"))
				{
					string value = layerName.Substring("Pos".Length);
					int num = global::System.Convert.ToInt32(value) - 1;
					if (num < 0 || num >= stations)
					{
						logger.Fatal(global::Kampai.Util.FatalCode.BV_NO_SUCH_WEIGHT_FOR_STATION);
					}
					layerIndicies[num] = j;
				}
			}
			if (stations <= 1 || !SupportsMultipleLayers())
			{
				return;
			}
			for (int k = 0; k < layerIndicies.Length; k++)
			{
				if (layerIndicies[k] == -1)
				{
					logger.Fatal(global::Kampai.Util.FatalCode.BV_MISSING_LAYER);
				}
			}
		}

		public void SetEnabledAllStations(bool isEnabled)
		{
			for (int i = 0; i < routes.Length; i++)
			{
				SetEnabledStation(i, isEnabled);
			}
		}

		public bool SupportsMultipleLayers()
		{
			return !(this is global::Kampai.Game.View.MignetteBuildingObject);
		}

		protected void SetEnabledStation(int station, bool isEnabled)
		{
			if (animators == null)
			{
				logger.Info("animators is null for building {0} ({1}), {2}s since startup", ID, definitionService.Get(base.DefinitionID).LocalizedKey, global::UnityEngine.Time.realtimeSinceStartup);
			}
			foreach (global::UnityEngine.Animator animator in animators)
			{
				SetStationState(animator, station, isEnabled);
			}
		}

		private void SetStationState(global::UnityEngine.Animator animator, int station, bool isEnabled)
		{
			if (GetNumberOfStations() > 1 && SupportsMultipleLayers() && station < GetNumberOfStations() && animator != null && layerIndicies != null && station >= 0)
			{
				animator.SetLayerWeight(layerIndicies[station], (!isEnabled) ? 0f : 1f);
			}
		}

		protected void SetMinionTriggers(string name)
		{
			foreach (global::Kampai.Game.View.TaskingMinionObject value in childAnimators.Values)
			{
				global::Kampai.Game.View.MinionObject minion = value.Minion;
				minion.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minion, null, name, logger));
			}
		}

		public override void Update()
		{
			base.Update();
			global::UnityEngine.AnimatorCullingMode animatorCullingMode = ((!(this is global::Kampai.Game.View.MignetteBuildingObject) && !HasVisibleRenderers()) ? global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms : global::UnityEngine.AnimatorCullingMode.AlwaysAnimate);
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.TaskingMinionObject>.Enumerator enumerator = childAnimators.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					global::Kampai.Game.View.TaskingMinionObject value = enumerator.Current.Value;
					value.Minion.SetAnimatorCullingMode(animatorCullingMode);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		internal virtual void RestMinion(int minionId)
		{
			for (int i = 0; i < childQueue.Count; i++)
			{
				global::Kampai.Game.View.TaskingMinionObject taskingMinionObject = childQueue[i];
				global::Kampai.Game.View.MinionObject minion = taskingMinionObject.Minion;
				if (minion.ID == minionId)
				{
					if (taskingMinionObject.RoutingIndex < GetNumberOfStations())
					{
						minion.EnqueueAction(new global::Kampai.Game.View.SetLayerAction(minion, 10, logger), true);
						string paramName = "OnWait";
						minion.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minion, null, paramName, logger));
						minion.EnqueueAction(new global::Kampai.Game.View.SetLayerAction(minion, 8, logger, 2));
						SetEnabledStation(i, false);
					}
					taskingMinionObject.IsResting = true;
					break;
				}
			}
			RestBuildingIfNeeded();
		}

		protected int GetActiveMinionCount()
		{
			int num = 0;
			foreach (global::Kampai.Game.View.TaskingMinionObject item in childQueue)
			{
				if (item.RoutingIndex < GetNumberOfStations() && !item.IsResting)
				{
					num++;
				}
			}
			return num;
		}

		private void RestBuildingIfNeeded()
		{
			if (GetActiveMinionCount() == 0 && !IsInAnimatorState(GetHashAnimationState("Base Layer.Wait")))
			{
				RestBuilding();
			}
		}

		private void RestBuilding()
		{
			EnqueueAction(new global::Kampai.Game.View.TriggerBuildingAnimationAction(this, OnlyStateEnabled("OnWait"), logger), true);
		}

		public void TrackChild(global::Kampai.Game.View.MinionObject child, global::UnityEngine.RuntimeAnimatorController controller, bool alreadyRushed)
		{
			int routeForChild = GetRouteForChild();
			global::Kampai.Game.View.TaskingMinionObject taskingMinionObject = new global::Kampai.Game.View.TaskingMinionObject(child, routeForChild);
			taskingMinionObject.IsResting = alreadyRushed;
			if (!childAnimators.ContainsKey(child.ID))
			{
				childAnimators.Add(child.ID, taskingMinionObject);
			}
			if (!childQueue.Contains(taskingMinionObject))
			{
				childQueue.Add(taskingMinionObject);
			}
			child.ShelveActionQueue();
			child.ExecuteAction(new global::Kampai.Game.View.MuteAction(child, false, logger));
			if (!IsAnimating)
			{
				StartAnimating();
			}
			SetSiblingVFXScript(child.gameObject, vfxScript);
			SetupChild(routeForChild, taskingMinionObject, controller);
		}

		private void SetSiblingVFXScript(global::UnityEngine.GameObject sibling, global::Kampai.Util.VFXScript vfxScript)
		{
			global::Kampai.Game.View.AnimEventHandler component = sibling.GetComponent<global::Kampai.Game.View.AnimEventHandler>();
			if (component != null)
			{
				component.SetSiblingVFXScript(vfxScript);
			}
		}

		protected int GetRouteForChild()
		{
			int result = childQueue.Count;
			int[] array = new int[routes.Length];
			foreach (global::Kampai.Game.View.TaskingMinionObject item in childQueue)
			{
				if (item.RoutingIndex < routes.Length)
				{
					array[item.RoutingIndex] = 1;
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public virtual void UntrackChild(int minionId, global::Kampai.Game.TaskableBuilding building)
		{
			if (childAnimators.ContainsKey(minionId))
			{
				global::Kampai.Game.View.TaskingMinionObject taskingMinionObject = childAnimators[minionId];
				global::Kampai.Game.View.MinionObject minion = taskingMinionObject.Minion;
				minion.ApplyRootMotion(true);
				minion.UnshelveActionQueue();
				minion.EnableBlobShadow(true);
				minion.SetAnimatorCullingMode(global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms);
				UnlinkChild(minionId);
				FillEmptyStation(taskingMinionObject.RoutingIndex);
				if (childAnimators.Count == 0)
				{
					EnqueueAction(new global::Kampai.Game.View.TriggerBuildingAnimationAction(this, OnlyStateEnabled("OnStop"), logger), true);
				}
				else
				{
					RestBuildingIfNeeded();
				}
				SetSiblingVFXScript(minion.gameObject, null);
				SetEnabledStation(taskingMinionObject.RoutingIndex, false);
			}
		}

		public void ReleaseMinionsForParty(global::Kampai.Game.TaskableBuilding building)
		{
			foreach (int minion in building.MinionList)
			{
				UntrackChild(minion, building);
			}
		}

		public virtual void FadeMinions(global::Kampai.Game.ToggleMinionRendererSignal toggleMinionSignal, bool fadeIn)
		{
			foreach (int key in childAnimators.Keys)
			{
				toggleMinionSignal.Dispatch(key, fadeIn);
			}
		}

		protected global::UnityEngine.Vector3 GetRandomExit(global::Kampai.Game.Building building)
		{
			string buildingFootprint = definitionService.GetBuildingFootprint(building.Definition.FootprintID);
			global::Kampai.Util.Point point = new global::Kampai.Util.Point(building.Location.x, building.Location.y);
			global::System.Collections.Generic.List<global::UnityEngine.Vector3> list = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();
			int num = 0;
			int num2 = 0;
			string text = buildingFootprint;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '.':
				{
					global::Kampai.Util.Point point2 = new global::Kampai.Util.Point(point.x + num, point.y + num2);
					if (!IsOccupiedByMinion(point2))
					{
						list.Add(new global::UnityEngine.Vector3(point2.x, 0f, point2.y));
					}
					break;
				}
				case '|':
					num = 0;
					num2--;
					break;
				}
				num++;
			}
			if (list.Count == 0)
			{
				return base.gameObject.transform.position;
			}
			int index = global::UnityEngine.Random.Range(0, list.Count - 1);
			return list[index];
		}

		private bool IsOccupiedByMinion(global::Kampai.Util.Point point)
		{
			global::UnityEngine.Collider[] array = global::UnityEngine.Physics.OverlapSphere(new global::UnityEngine.Vector3(point.x, 1f, point.y), 1f);
			global::UnityEngine.Collider[] array2 = array;
			foreach (global::UnityEngine.Collider collider in array2)
			{
				if (collider.gameObject.GetComponent<global::Kampai.Game.View.MinionObject>() != null)
				{
					return true;
				}
			}
			return false;
		}

		private void FillEmptyStation(int vacant)
		{
			int numberOfStations = GetNumberOfStations();
			if (numberOfStations > childQueue.Count)
			{
				return;
			}
			foreach (global::Kampai.Game.View.TaskingMinionObject item in childQueue)
			{
				if (item.RoutingIndex >= numberOfStations)
				{
					SetupChild(vacant, item);
					break;
				}
			}
		}

		protected void UnlinkChild(int minionId)
		{
			int num = -1;
			for (int i = 0; i < childQueue.Count; i++)
			{
				if (childQueue[i].ID == minionId)
				{
					num = i;
					break;
				}
			}
			if (num > -1)
			{
				childQueue.RemoveAt(num);
			}
			else
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Not found");
			}
			childAnimators.Remove(minionId);
		}

		protected virtual void SetupChild(int routingIndex, global::Kampai.Game.View.TaskingMinionObject taskingChild, global::UnityEngine.RuntimeAnimatorController controller = null)
		{
			taskingChild.RoutingIndex = routingIndex;
			global::Kampai.Game.View.MinionObject minion = taskingChild.Minion;
			if (controller != null)
			{
				minion.SetAnimController(controller);
			}
			int numberOfStations = GetNumberOfStations();
			if (routingIndex < numberOfStations)
			{
				minion.ApplyRootMotion(true);
				minion.ExecuteAction(new global::Kampai.Game.View.SetAnimatorAction(minion, null, logger, new global::System.Collections.Generic.Dictionary<string, object> { { "minionPosition", routingIndex } }));
				global::System.Collections.Generic.Dictionary<string, object> dictionary = OnlyStateEnabled("OnLoop");
				int hashAnimationState;
				int hashAnimationState2;
				if (taskingChild.IsResting)
				{
					dictionary["OnWait"] = true;
					hashAnimationState = GetHashAnimationState("Base Layer.Wait_Pos" + (routingIndex + 1));
					hashAnimationState2 = GetHashAnimationState("Base Layer.Wait");
				}
				else
				{
					hashAnimationState = GetHashAnimationState("Base Layer.Loop_Pos" + (routingIndex + 1));
					hashAnimationState2 = GetHashAnimationState("Base Layer.Loop");
				}
				minion.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minion, null, logger, OnlyStateEnabled("OnStop")), true);
				minion.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(minion, GetHashAnimationState("Base Layer.Idle"), logger));
				minion.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minion, null, logger, dictionary));
				minion.EnqueueAction(new global::Kampai.Game.View.SkipToTimeAction(minion, new global::Kampai.Game.View.SkipToTime(0, hashAnimationState, GetCurrentAnimationTimeForState(hashAnimationState2)), logger));
				MoveToRoutingPosition(minion, routingIndex);
				SetEnabledStation(routingIndex, !taskingChild.IsResting);
			}
			else
			{
				minion.EnableRenderers(false);
			}
		}

		protected global::Kampai.Game.View.TaskingMinionObject GetByRouteSlot(int routingIndex)
		{
			foreach (global::Kampai.Game.View.TaskingMinionObject item in childQueue)
			{
				if (item.RoutingIndex == routingIndex)
				{
					return item;
				}
			}
			return null;
		}
	}
}
