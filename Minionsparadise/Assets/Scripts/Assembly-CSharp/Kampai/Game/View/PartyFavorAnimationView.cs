namespace Kampai.Game.View
{
	public class PartyFavorAnimationView : global::Kampai.Util.KampaiView
	{
		private sealed class MinionArgs
		{
			public global::UnityEngine.GameObject parent;

			public global::Kampai.Game.IDefinitionService definitionService;

			public global::Kampai.Game.PartyFavorAnimationDefinition def;

			public global::UnityEngine.RuntimeAnimatorController walkAnimController;

			public global::UnityEngine.RuntimeAnimatorController animController;

			public global::UnityEngine.Vector3 centerPoint;

			public global::Kampai.Util.IPathFinder pathFinder;

			public global::Kampai.Game.MinionPartyDefinition partyDefinition;
		}

		private global::Kampai.Util.IKampaiLogger logger;

		private global::Kampai.Game.IBuildingUtilities buildingUtilies;

		private global::UnityEngine.RuntimeAnimatorController minionWalkStateMachine;

		private global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.Game.MinionStateChangeSignal stateChangeSignal;

		private global::Kampai.Util.PathFinder pathFinder;

		private global::Kampai.Game.MinionPartyDefinition partyDefinition;

		private global::Kampai.Game.Environment environment;

		private global::Kampai.Game.DebugUpdateGridSignal debugUpdateGridSignal;

		private global::System.Action<int> onFinishAnimCallback;

		protected global::Kampai.Game.View.MinionObject minionObj;

		private string footprint;

		private int[,] tempGrid;

		public global::Kampai.Game.PartyFavorAnimationDefinition PartyFavorDefinition { get; set; }

		public global::Kampai.Game.Location GetLocation()
		{
			global::UnityEngine.Vector3 position = base.transform.position;
			return new global::Kampai.Game.Location(global::UnityEngine.Mathf.RoundToInt(position.x), global::UnityEngine.Mathf.RoundToInt(position.z));
		}

		public void Init(global::Kampai.Game.PartyFavorAnimationDefinition definition, global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Util.PathFinder pathFinder, global::Kampai.Game.DebugUpdateGridSignal debugUpdateGridSignal, global::Kampai.Game.Environment environment, global::System.Action<int> onFinishAnimCallback)
		{
			this.onFinishAnimCallback = onFinishAnimCallback;
			this.pathFinder = pathFinder;
			this.definitionService = definitionService;
			this.debugUpdateGridSignal = debugUpdateGridSignal;
			this.environment = environment;
			footprint = definitionService.GetBuildingFootprint(definition.FootprintID);
			tempGrid = new int[BuildingUtil.GetFootprintWidth(footprint), BuildingUtil.GetFootprintDepth(footprint)];
			partyDefinition = definitionService.Get<global::Kampai.Game.MinionPartyDefinition>(80000);
			PartyFavorDefinition = definition;
			minionWalkStateMachine = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>("asm_minion_movement");
		}

		internal void SetupInjections(global::Kampai.Util.IKampaiLogger logger, global::Kampai.Game.MinionStateChangeSignal minionStateChangeSignal, global::Kampai.Game.IBuildingUtilities buildingUtilies)
		{
			stateChangeSignal = minionStateChangeSignal;
			this.logger = logger;
			this.buildingUtilies = buildingUtilies;
		}

		public void TrackChild(global::Kampai.Game.View.MinionObject minionObj)
		{
			this.minionObj = minionObj;
			base.transform.position = this.minionObj.transform.position;
			UpdatePath(GetLocation(), true);
			global::Kampai.Game.View.PartyFavorAnimationView.MinionArgs minionArgs = new global::Kampai.Game.View.PartyFavorAnimationView.MinionArgs();
			minionArgs.parent = base.gameObject;
			minionArgs.definitionService = definitionService;
			minionArgs.def = PartyFavorDefinition;
			minionArgs.walkAnimController = minionWalkStateMachine;
			minionArgs.animController = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(PartyFavorDefinition.AnimationID).StateMachine);
			minionArgs.centerPoint = base.transform.position;
			minionArgs.pathFinder = pathFinder;
			minionArgs.partyDefinition = partyDefinition;
			global::Kampai.Game.View.PartyFavorAnimationView.MinionArgs args = minionArgs;
			SetupMinionPartyFavorAnimation(args);
		}

		public void UntrackChild()
		{
			if (minionObj != null)
			{
				global::Kampai.Game.MinionState newState = global::Kampai.Game.MinionState.Idle;
				minionObj.ApplyRootMotion(true);
				minionObj.EnableBlobShadow(true);
				minionObj.SetAnimatorCullingMode(global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms);
				if (minionObj.GetCurrentAnimControllerName().Contains("Prop"))
				{
					float time = 1f;
					int layer = -1;
					minionObj.PlayAnimation(global::UnityEngine.Animator.StringToHash("Base Layer.Exit"), layer, time);
				}
				minionObj.EnqueueAction(new global::Kampai.Game.View.StateChangeAction(minionObj.ID, stateChangeSignal, newState, logger));
				minionObj.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(minionObj, minionWalkStateMachine, logger));
			}
		}

		public void FreeAllMinions()
		{
			UpdatePath(GetLocation(), false);
			UntrackChild();
		}

		private void SetupMinionPartyFavorAnimation(global::Kampai.Game.View.PartyFavorAnimationView.MinionArgs args)
		{
			global::Kampai.Game.View.CoordinatedAnimation coordinatedAnimation = args.parent.AddComponent<global::Kampai.Game.View.CoordinatedAnimation>();
			global::UnityEngine.Vector3 position = global::UnityEngine.Camera.main.transform.position;
			coordinatedAnimation.Init(args.def, args.parent.transform, args.centerPoint, new global::UnityEngine.Vector3(position.x, 0f, position.z), logger);
			global::Kampai.Util.VFXScript vFXScript = coordinatedAnimation.GetVFXScript();
			global::Kampai.Game.View.DestroyObjectAction deallocateAnimationPrefab = new global::Kampai.Game.View.DestroyObjectAction(coordinatedAnimation, logger);
			minionObj.StopLocalAudio();
			EnqueueActions(args, minionObj, vFXScript, GetAnimationArgs(args.def.AnimationID), deallocateAnimationPrefab, "Base Layer.Exit");
		}

		private void EnqueueActions(global::Kampai.Game.View.PartyFavorAnimationView.MinionArgs args, global::Kampai.Game.View.MinionObject mo, global::Kampai.Util.VFXScript vfxScript, global::System.Collections.Generic.Dictionary<string, object> animArgs, global::Kampai.Game.View.DestroyObjectAction deallocateAnimationPrefab, string exitState)
		{
			mo.EnqueueAction(new global::Kampai.Game.View.MuteAction(mo, false, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.RotateAction(mo, global::UnityEngine.Camera.main.transform.eulerAngles.y - 180f, 360f, logger));
			mo.EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(mo, args.animController, logger, animArgs));
			mo.EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(mo, global::UnityEngine.Animator.StringToHash(exitState), logger));
			if (vfxScript != null)
			{
				mo.EnqueueAction(new global::Kampai.Game.View.UntrackVFXAction(mo, logger));
			}
			mo.EnqueueAction(deallocateAnimationPrefab);
			mo.EnqueueAction(new global::Kampai.Game.View.PelvisAnimationCompleteAction(logger, mo, args.walkAnimController));
			mo.EnqueueAction(new global::Kampai.Game.View.DelegateAction(delegate
			{
				GetNextController(mo.ID);
			}, logger));
		}

		private void GetNextController(int minionId)
		{
			if (onFinishAnimCallback != null)
			{
				onFinishAnimCallback(minionId);
			}
		}

		private global::System.Collections.Generic.Dictionary<string, object> GetAnimationArgs(int animationId)
		{
			global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
			global::Kampai.Game.MinionAnimationDefinition minionAnimationDefinition = definitionService.Get<global::Kampai.Game.MinionAnimationDefinition>(animationId);
			if (minionAnimationDefinition.arguments != null)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<string, object> argument in minionAnimationDefinition.arguments)
				{
					if (argument.Key.Equals("actor"))
					{
						logger.Warning("Ignoring actor attribute for {0}", minionAnimationDefinition.ID);
					}
					else
					{
						dictionary.Add(argument.Key, argument.Value);
					}
				}
			}
			dictionary.Add("actor", 0);
			return dictionary;
		}

		private void UpdatePath(global::Kampai.Game.Location location, bool isAddingToFootprint)
		{
			int x = location.x;
			int num = x;
			int num2 = location.y;
			int num3 = 0;
			int num4 = 0;
			string text = footprint;
			foreach (char c in text)
			{
				char c2 = c;
				if (c2 != 'X' && c2 != 'x' && c2 == '|')
				{
					num = x;
					num3 = 0;
					num2--;
					num4++;
				}
				else if (buildingUtilies.CheckGridBounds(num, num2))
				{
					if (isAddingToFootprint)
					{
						tempGrid[num3, num4] = environment.PlayerGrid[num, num2].Modifier;
						environment.PlayerGrid[num, num2].Walkable = false;
						environment.PlayerGrid[num, num2].Occupied = true;
					}
					else
					{
						environment.PlayerGrid[num, num2].Modifier = tempGrid[num3, num4];
					}
					num++;
					num3++;
				}
			}
			pathFinder.UpdateWalkableRegion();
			debugUpdateGridSignal.Dispatch();
		}
	}
}
