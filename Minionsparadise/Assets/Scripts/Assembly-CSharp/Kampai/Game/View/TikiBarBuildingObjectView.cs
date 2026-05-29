namespace Kampai.Game.View
{
	public class TikiBarBuildingObjectView : global::Kampai.Game.View.AnimatingBuildingObject, global::strange.extensions.mediation.api.IView
	{
		public global::Kampai.Game.TikiBarBuilding tikiBar;

		private global::UnityEngine.Renderer glowRenderer;

		private global::UnityEngine.Animation glowAnimation;

		private int routeIndex;

		internal bool didSkipParty;

		private global::System.Collections.Generic.Dictionary<int, global::UnityEngine.RuntimeAnimatorController> unlockedMinionControllers;

		private global::UnityEngine.RuntimeAnimatorController bartenderStateMachine;

		private global::UnityEngine.RuntimeAnimatorController minionWalkStateMachine;

		private int[] layerIndicies;

		private global::UnityEngine.Renderer[] renderers;

		protected global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.TaskingCharacterObject> childAnimators = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.TaskingCharacterObject>();

		protected global::System.Collections.Generic.List<global::Kampai.Game.View.TaskingCharacterObject> childQueue = new global::System.Collections.Generic.List<global::Kampai.Game.View.TaskingCharacterObject>();

		private global::Kampai.Game.MinionStateChangeSignal stateChangeSignal;

		private global::Kampai.Game.NamedCharacterRemovedFromTikiBarSignal removedFromTikibarSignal;

		private global::Kampai.Game.CharacterIntroCompleteSignal introCompleteSignal;

		private global::Kampai.Game.CharacterDrinkingCompleteSignal drinkingCompelteSignal;

		private bool _requiresContext = true;

		protected bool registerWithContext = true;

		private global::strange.extensions.context.api.IContext currentContext;

		public bool requiresContext
		{
			get
			{
				return _requiresContext;
			}
			set
			{
				_requiresContext = value;
			}
		}

		public bool registeredWithContext { get; set; }

		public virtual bool autoRegisterWithContext
		{
			get
			{
				return registerWithContext;
			}
			set
			{
				registerWithContext = value;
			}
		}

		internal override void Init(global::Kampai.Game.Building building, global::Kampai.Util.IKampaiLogger logger, global::System.Collections.Generic.IDictionary<string, global::UnityEngine.RuntimeAnimatorController> controllers, global::Kampai.Game.IDefinitionService definitionService)
		{
			base.Init(building, logger, controllers, definitionService);
			tikiBar = building as global::Kampai.Game.TikiBarBuilding;
			global::Kampai.Game.TaskableBuildingDefinition taskableBuildingDefinition = building.Definition as global::Kampai.Game.TaskableBuildingDefinition;
			if (taskableBuildingDefinition == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.BV_ILLEGAL_TASKABLE_DEFINITION, building.Definition.ID.ToString());
			}
			if (building.IsBuildingRepaired())
			{
				if (tikiBar.State != global::Kampai.Game.BuildingState.MissingTikiSign)
				{
					global::UnityEngine.Transform transform = base.transform.Find("Unique_TikiBar_LOD0/Unique_TikiBar:Unique_TikiBar/Unique_TikiBar:Unique_TikiBar_Glow_mesh");
					glowRenderer = transform.GetComponent<global::UnityEngine.Renderer>();
					glowRenderer.enabled = false;
					glowAnimation = transform.GetComponent<global::UnityEngine.Animation>();
				}
				SetupAnimationControllers();
			}
			renderers = base.gameObject.GetComponentsInChildren<global::UnityEngine.Renderer>();
		}

		internal void SetupInjections(global::Kampai.Game.MinionStateChangeSignal stateChangeSignal, global::Kampai.Game.NamedCharacterRemovedFromTikiBarSignal removedFromTikibarSignal, global::Kampai.Game.CharacterIntroCompleteSignal introCompleteSingal, global::Kampai.Game.CharacterDrinkingCompleteSignal drinkingCompleteSingal)
		{
			this.stateChangeSignal = stateChangeSignal;
			this.removedFromTikibarSignal = removedFromTikibarSignal;
			introCompleteSignal = introCompleteSingal;
			drinkingCompelteSignal = drinkingCompleteSingal;
		}

		private void SetupAnimationControllers()
		{
			minionWalkStateMachine = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>("asm_minion_movement");
			bartenderStateMachine = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>("asm_unique_tikibar_bartender");
			unlockedMinionControllers = new global::System.Collections.Generic.Dictionary<int, global::UnityEngine.RuntimeAnimatorController>();
			for (int i = 0; i < 3; i++)
			{
				unlockedMinionControllers.Add(i, global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(string.Format("{0}{1}", "asm_animIntro_newMinion", i + 1)));
				unlockedMinionControllers.Add(i + 3, global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(string.Format("{0}{1}", "asm_unique_tikibar_newMinion_Fun", i + 1)));
			}
		}

		public override void ResetAnimationParameters()
		{
			if (!didSkipParty)
			{
				base.ResetAnimationParameters();
			}
			else
			{
				SetAnimTrigger("SkipParty");
			}
			didSkipParty = false;
		}

		internal void SetupLayers()
		{
			if (animators.Count == 0)
			{
				return;
			}
			int layerCount = animators[0].layerCount;
			layerIndicies = new int[3];
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
			if (stations <= 1)
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

		internal void SetupCharacter(global::Kampai.Game.View.CharacterObject characterObject, global::Kampai.Game.IPlayerService playerService, global::Kampai.Game.IPrestigeService prestigeService)
		{
			int num = routeIndex % 3;
			routeIndex++;
			MoveToRoutingPosition(characterObject, num + 3);
			characterObject.ShelveActionQueue();
			characterObject.SetAnimController(GetMinionArrivalAnimation(characterObject, playerService, prestigeService, num));
			characterObject.SetAnimatorCullingMode(global::UnityEngine.AnimatorCullingMode.AlwaysAnimate);
			characterObject.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(characterObject, GetHashAnimationState("Base Layer.NewMinionIntro"), logger));
			characterObject.EnqueueAction(new global::Kampai.Game.View.SetCullingModeAction(characterObject, global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms, logger));
			stateChangeSignal.Dispatch(characterObject.ID, global::Kampai.Game.MinionState.Questing);
		}

		internal global::UnityEngine.RuntimeAnimatorController GetMinionArrivalAnimation(global::Kampai.Game.View.CharacterObject characterObject, global::Kampai.Game.IPlayerService playerService, global::Kampai.Game.IPrestigeService prestigeService, int routeIndex)
		{
			global::Kampai.Game.Character byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Character>(characterObject.ID);
			if (byInstanceId != null)
			{
				global::Kampai.Game.Prestige prestigeFromMinionInstance = prestigeService.GetPrestigeFromMinionInstance(byInstanceId);
				if (prestigeFromMinionInstance != null)
				{
					global::Kampai.Game.PrestigeDefinition definition = prestigeFromMinionInstance.Definition;
					if (definition != null && definition.UniqueArrivalStateMachine != null)
					{
						return global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(prestigeFromMinionInstance.Definition.UniqueArrivalStateMachine);
					}
				}
			}
			int minionCount = playerService.GetMinionCount();
			if (minionCount > 4)
			{
				return unlockedMinionControllers[routeIndex];
			}
			return unlockedMinionControllers[routeIndex + 3];
		}

		internal void BeginCharacterIntroLoop(bool waitForLoop, global::Kampai.Game.View.CharacterObject characterObject)
		{
			if (waitForLoop)
			{
				SetAnimTrigger("OnNewMinionAppear");
				characterObject.SetAnimTrigger("OnNewMinionAppear");
			}
			characterObject.SetAnimatorCullingMode(global::UnityEngine.AnimatorCullingMode.AlwaysAnimate);
		}

		internal void BeginCharacterIntro(bool waitForLoop, global::Kampai.Game.View.CharacterObject characterObject, int minionRouteIndex)
		{
			routeIndex = 0;
			characterObject.ClearActionQueue();
			characterObject.EnqueueAction(new global::Kampai.Game.View.SetCullingModeAction(characterObject, global::UnityEngine.AnimatorCullingMode.AlwaysAnimate, logger));
			if (waitForLoop)
			{
				SetAnimTrigger("OnNewMinionIntro");
				characterObject.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(characterObject, GetHashAnimationState("Base Layer.Loop"), logger));
				global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
				dictionary.Add("OnNewMinionIntro", true);
				characterObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(characterObject, null, logger, dictionary));
			}
			characterObject.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(characterObject, GetHashAnimationState("Base Layer.Exit"), logger));
			characterObject.EnqueueAction(new global::Kampai.Game.View.CharacterIntroCompleteAction(characterObject, minionRouteIndex, minionWalkStateMachine, introCompleteSignal, logger));
			characterObject.EnqueueAction(new global::Kampai.Game.View.SetCullingModeAction(characterObject, global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms, logger));
		}

		internal void EndCharacterIntro(global::Kampai.Game.View.CharacterObject characterObject, int slotIndex)
		{
			characterObject.ApplyRootMotion(true);
			characterObject.EnableBlobShadow(true);
			characterObject.SetAnimatorCullingMode(global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms);
			characterObject.UnshelveActionQueue();
			if (slotIndex < 0)
			{
				characterObject.EnqueueAction(new global::Kampai.Game.View.StateChangeAction(characterObject.ID, stateChangeSignal, global::Kampai.Game.MinionState.Idle, logger), true);
			}
		}

		internal void RemoveCharacterFromTikiBar(int minionId)
		{
			if (childAnimators.ContainsKey(minionId))
			{
				global::Kampai.Game.View.TaskingCharacterObject taskingCharacterObject = childAnimators[minionId];
				int routingIndex = taskingCharacterObject.RoutingIndex;
				global::Kampai.Game.View.CharacterObject character = taskingCharacterObject.Character;
				switch (routingIndex)
				{
				case 1:
					SetAnimBool("pos1_IsSeated", false);
					break;
				case 2:
					SetAnimBool("pos2_IsSeated", false);
					break;
				}
				character.SetAnimBool("isSeated", false);
				character.IsSeatedInTikiBar = false;
				character.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(character, character.GetCurrentAnimController(), logger), true);
				character.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(character, GetHashAnimationState("Base Layer.Idle"), logger));
				character.EnqueueAction(new global::Kampai.Game.View.CharacterDrinkingCompleteAction(character, drinkingCompelteSignal, logger));
			}
		}

		internal void UntrackChild(int minionId)
		{
			if (childAnimators.ContainsKey(minionId))
			{
				global::Kampai.Game.View.TaskingCharacterObject taskingCharacterObject = childAnimators[minionId];
				int routingIndex = taskingCharacterObject.RoutingIndex;
				global::Kampai.Game.View.CharacterObject character = taskingCharacterObject.Character;
				character.ApplyRootMotion(true);
				character.UnshelveActionQueue();
				character.EnableBlobShadow(true);
				character.SetAnimatorCullingMode(global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms);
				UnlinkChild(minionId);
				SetEnabledStation(routingIndex, false);
				global::Kampai.Game.View.NamedCharacterObject namedCharacterObject = character as global::Kampai.Game.View.NamedCharacterObject;
				if (namedCharacterObject != null)
				{
					character.ResetAnimationController();
					removedFromTikibarSignal.Dispatch(namedCharacterObject);
				}
				else if (character is global::Kampai.Game.View.MinionObject)
				{
					character.EnqueueAction(new global::Kampai.Game.View.StateChangeAction(character.ID, stateChangeSignal, global::Kampai.Game.MinionState.Idle, logger), true);
					character.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(character, minionWalkStateMachine, logger));
					character.EnqueueAction(new global::Kampai.Game.View.GotoSideWalkAction(character, tikiBar, logger, definitionService, null));
				}
			}
		}

		internal void UnlinkChild(int minionId)
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
				childAnimators.Remove(minionId);
			}
			else
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Not found");
			}
		}

		internal void PathCharacterToTikiBar(global::Kampai.Game.View.CharacterObject characterObject, global::System.Collections.Generic.IList<global::UnityEngine.Vector3> path, float rotation, int routeIndex, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.View.CharacterObject, int> addSignal)
		{
			float speed = 4.5f;
			characterObject.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(characterObject, minionWalkStateMachine, logger), true);
			characterObject.EnqueueAction(new global::Kampai.Game.View.ConstantSpeedPathAction(characterObject, path, speed, logger));
			characterObject.EnqueueAction(new global::Kampai.Game.View.RotateAction(characterObject, rotation, 720f, logger));
			characterObject.EnqueueAction(new global::Kampai.Game.View.PathToBuildingCompleteAction(characterObject, routeIndex, addSignal, logger));
		}

		internal bool ContainsCharacter(int instanceID)
		{
			return childAnimators.ContainsKey(instanceID);
		}

		internal void AddCharacterToBuildingActions(global::Kampai.Game.View.CharacterObject characterObject, global::Kampai.Game.IPlayerService playerService, int routeIndex, global::Kampai.Game.IPrestigeService prestigeService, global::Kampai.Game.GetNewQuestSignal getNewQuestSignal)
		{
			switch (routeIndex)
			{
			case 1:
				SetAnimBool("pos1_IsSeated", true);
				break;
			case 2:
				SetAnimBool("pos2_IsSeated", true);
				break;
			}
			EnqueueAction(new global::Kampai.Game.View.TikibarTrackChildAction(this, characterObject, routeIndex, GetMinionBarstoolAnimation(characterObject, playerService, routeIndex, prestigeService), getNewQuestSignal, logger));
			if (!(characterObject is global::Kampai.Game.View.PhilView))
			{
				EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(characterObject, logger, "isSeated", true));
			}
			characterObject.IsSeatedInTikiBar = true;
			global::Kampai.Util.AI.Agent component = characterObject.GetComponent<global::Kampai.Util.AI.Agent>();
			if (component != null)
			{
				component.MaxSpeed = 0f;
			}
		}

		internal global::UnityEngine.RuntimeAnimatorController GetMinionBarstoolAnimation(global::Kampai.Game.View.CharacterObject characterObject, global::Kampai.Game.IPlayerService playerService, int routeIndex, global::Kampai.Game.IPrestigeService prestigeService)
		{
			if (routeIndex == 0)
			{
				return bartenderStateMachine;
			}
			global::Kampai.Game.Character byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Character>(characterObject.ID);
			global::Kampai.Game.Prestige prestigeFromMinionInstance = prestigeService.GetPrestigeFromMinionInstance(byInstanceId);
			if (prestigeFromMinionInstance == null)
			{
				logger.Fatal(global::Kampai.Util.FatalCode.PS_NO_SUCH_PRESTIGE, characterObject.ID);
				return null;
			}
			if (routeIndex == 1)
			{
				return global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(prestigeFromMinionInstance.Definition.UniqueTikiBarstoolASMPatron1);
			}
			return global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(prestigeFromMinionInstance.Definition.UniqueTikiBarstoolASMPatron2);
		}

		public void TrackChild(global::Kampai.Game.View.CharacterObject child, global::UnityEngine.RuntimeAnimatorController controller, int routeIndex)
		{
			global::Kampai.Game.View.TaskingCharacterObject taskingCharacterObject = new global::Kampai.Game.View.TaskingCharacterObject(child, routeIndex);
			if (!childAnimators.ContainsKey(child.ID))
			{
				childAnimators.Add(child.ID, taskingCharacterObject);
				childQueue.Add(taskingCharacterObject);
				child.ShelveActionQueue();
			}
			SetupChild(routeIndex, taskingCharacterObject, controller);
		}

		protected virtual void SetupChild(int routingIndex, global::Kampai.Game.View.TaskingCharacterObject taskingChild, global::UnityEngine.RuntimeAnimatorController controller = null)
		{
			taskingChild.RoutingIndex = routingIndex;
			global::Kampai.Game.View.CharacterObject character = taskingChild.Character;
			if (controller != null)
			{
				character.SetAnimController(controller);
			}
			character.ApplyRootMotion(true);
			character.EnableRenderers(true);
			MoveToRoutingPosition(character, routingIndex);
			SetEnabledStation(routingIndex, true);
		}

		public override void Update()
		{
			base.Update();
			bool flag = false;
			for (int i = 0; i < renderers.Length; i++)
			{
				global::UnityEngine.Renderer renderer = renderers[i];
				if (renderer.isVisible)
				{
					flag = true;
					break;
				}
			}
			global::UnityEngine.AnimatorCullingMode animatorCullingMode = ((!flag) ? global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms : global::UnityEngine.AnimatorCullingMode.AlwaysAnimate);
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.View.TaskingCharacterObject>.Enumerator enumerator = childAnimators.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					global::Kampai.Game.View.TaskingCharacterObject value = enumerator.Current.Value;
					value.Character.SetAnimatorCullingMode(animatorCullingMode);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		protected void SetEnabledStation(int station, bool isEnabled)
		{
			foreach (global::UnityEngine.Animator animator in animators)
			{
				SetStationState(animator, station, isEnabled);
			}
		}

		private void SetStationState(global::UnityEngine.Animator animator, int station, bool isEnabled)
		{
			if (GetNumberOfStations() > 1 && station < GetNumberOfStations())
			{
				animator.SetLayerWeight(layerIndicies[station], (!isEnabled) ? 0f : 1f);
			}
		}

		public void ToggleHitbox(bool enable)
		{
			global::UnityEngine.Collider[] components = GetComponents<global::UnityEngine.Collider>();
			foreach (global::UnityEngine.Collider collider in components)
			{
				collider.enabled = enable;
			}
		}

		internal void ToggleStickerbookGlow(bool enable)
		{
			if (enable)
			{
				glowRenderer.enabled = true;
				glowAnimation.Play();
			}
			else
			{
				glowAnimation.Stop();
				glowRenderer.enabled = false;
			}
		}

		protected override global::UnityEngine.Vector3 GetIndicatorPosition(bool centerY)
		{
			if (tikiBar != null && tikiBar.State == global::Kampai.Game.BuildingState.MissingTikiSign)
			{
				return global::Kampai.Util.GameConstants.TIKI_BAR_MISSING_SIGN_INDICATOR_POSITION;
			}
			return base.GetIndicatorPosition(centerY);
		}

		protected void Awake()
		{
			if (autoRegisterWithContext && !registeredWithContext)
			{
				global::Kampai.Util.KampaiView.BubbleToContext(this, true, false, ref currentContext);
			}
		}

		protected void Start()
		{
			if (autoRegisterWithContext && !registeredWithContext)
			{
				global::Kampai.Util.KampaiView.BubbleToContext(this, true, true, ref currentContext);
			}
		}

		protected void OnDestroy()
		{
			global::Kampai.Util.KampaiView.BubbleToContext(this, false, false, ref currentContext);
		}

		internal void PlayAnimation(string animation, global::System.Type type, object obj)
		{
			if (type == typeof(int))
			{
				SetAnimInteger(animation, (int)obj);
			}
			else if (type == typeof(float))
			{
				SetAnimFloat(animation, (float)obj);
			}
			else if (type == typeof(bool))
			{
				SetAnimBool(animation, (bool)obj);
			}
		}

		public override bool CanFadeGFX()
		{
			return false;
		}

		public override bool CanFadeSFX()
		{
			return false;
		}
	}
}
