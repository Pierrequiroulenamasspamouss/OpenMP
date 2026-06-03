namespace Kampai.Game.View
{
	public class DummyCharacterObject : global::Kampai.Game.View.ActionableObject
	{
		private struct LOD
		{
			public int Number;

			private global::System.Collections.Generic.List<global::UnityEngine.GameObject> Renderers;

			internal LOD(int number, global::UnityEngine.GameObject go)
			{
				Number = number;
				Renderers = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>(2);
				Renderers.Add(go);
			}

			internal void addRendererGameObject(global::UnityEngine.GameObject go)
			{
				Renderers.Add(go);
			}

			internal void Enable()
			{
				for (int i = 0; i < Renderers.Count; i++)
				{
					Renderers[i].SetActive(true);
				}
			}
		}

		private const int EXPECTED_LOD_COUNT = 3;

		private const int EXPECTED_RENDERERS_PER_LOD = 2;

		protected global::UnityEngine.RuntimeAnimatorController cachedDefaultController;

		protected global::UnityEngine.RuntimeAnimatorController cachedRuntimeController;

		protected bool animatorControllersAreEqual;

		private global::Kampai.Common.IRandomService randomService;

		private global::Kampai.Game.IDefinitionService definitionService;

		private bool markedForDestory;

		private global::Kampai.UI.View.KampaiImage shadowImage;

		private int idleAnimationCount;

		private global::Kampai.Game.Transaction.WeightedInstance idleWeightedInstance;

		private int selectedAnimationCount;

		private global::Kampai.Game.Transaction.WeightedInstance selectedWeightedInstance;

		private int happyAnimationCount;

		private global::Kampai.Game.Transaction.WeightedInstance happyWeightedInstance;

		private global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedInstance> animationPoolWeightedInstanceList;

		private int lastIndex = -1;

		private global::strange.extensions.signal.impl.Signal networkOpenSignal;

		private global::strange.extensions.signal.impl.Signal networkCloseSignal;

		private global::Kampai.UI.DummyCharacterAnimationState animationState;

		public void Init(global::Kampai.Game.Character character, global::Kampai.Util.IKampaiLogger logger, global::Kampai.Common.IRandomService randomService, global::Kampai.Game.IDefinitionService definitinoService, global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedInstance> weightedInstanceList)
		{
			base.Init();
			ID = character.ID;
			base.name = character.Name;
			base.logger = logger;
			this.randomService = randomService;
			definitionService = definitinoService;
			animationPoolWeightedInstanceList = weightedInstanceList;
			SetName(character);
			animators = new global::System.Collections.Generic.List<global::UnityEngine.Animator>(base.gameObject.GetComponentsInChildren<global::UnityEngine.Animator>());
			global::Kampai.Game.Minion minion = character as global::Kampai.Game.Minion;
			if (minion != null && !minion.HasPrestige)
			{
				global::Kampai.Game.View.MinionObject.SetEyes(base.transform, minion.Definition.Eyes);
				global::Kampai.Game.View.MinionObject.SetBody(base.transform, minion.Definition.Body);
				global::Kampai.Game.View.MinionObject.SetHair(base.transform, minion.Definition.Hair);
			}
			else if (character is global::Kampai.Game.KevinCharacter)
			{
				global::Kampai.Game.View.MinionObject.SetEyes(base.transform, 2u);
				global::Kampai.Game.View.MinionObject.SetBody(base.transform, global::Kampai.Game.MinionBody.TALL);
				global::Kampai.Game.View.MinionObject.SetHair(base.transform, global::Kampai.Game.MinionHair.SPROUT);
			}
		}

		private global::Kampai.Game.Transaction.WeightedInstance CreateWeightedInstance(int weightedCount)
		{
			global::Kampai.Game.Transaction.WeightedDefinition weightedDefinition = new global::Kampai.Game.Transaction.WeightedDefinition();
			weightedDefinition.Entities = new global::System.Collections.Generic.List<global::Kampai.Game.Transaction.WeightedQuantityItem>();
			for (int i = 0; i < weightedCount; i++)
			{
				weightedDefinition.Entities.Add(new global::Kampai.Game.Transaction.WeightedQuantityItem(i, 0u, 1u));
			}
			return new global::Kampai.Game.Transaction.WeightedInstance(weightedDefinition);
		}

		public global::Kampai.Game.View.DummyCharacterObject Build(global::Kampai.Game.Character character, global::Kampai.Game.CharacterUIAnimationDefinition characterUIAnimationDefinition, global::UnityEngine.Transform parent, global::Kampai.Util.IKampaiLogger logger, bool forceHighLOD, global::UnityEngine.Vector3 villainScale, global::UnityEngine.Vector3 villainPositionOffset, global::Kampai.Util.IMinionBuilder minionBuilder)
		{
			if (parent != null)
			{
				base.gameObject.transform.SetParent(parent, false);
			}
			base.gameObject.transform.localEulerAngles = global::UnityEngine.Vector3.zero;
			if (character is global::Kampai.Game.Villain)
			{
				base.gameObject.transform.localPosition = villainPositionOffset;
				base.gameObject.transform.localScale = villainScale;
			}
			else
			{
				base.gameObject.transform.localPosition = global::UnityEngine.Vector3.zero;
				base.gameObject.transform.localScale = global::UnityEngine.Vector3.one;
				SetupLODs(forceHighLOD);
			}
			string stateMachine = characterUIAnimationDefinition.StateMachine;
			global::UnityEngine.Animator component = base.gameObject.GetComponent<global::UnityEngine.Animator>();
			component.applyRootMotion = false;
			component.runtimeAnimatorController = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(stateMachine);
			component.cullingMode = global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms;
			if (component.runtimeAnimatorController == null)
			{
				logger.Error("Failed to get default runtime animator controller for {0}: asm name: {1}", character.Name, stateMachine);
			}
			global::UnityEngine.Transform rootBone = base.gameObject.transform.Find(GetRootJointPath());
			global::UnityEngine.SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<global::UnityEngine.SkinnedMeshRenderer>();
			foreach (global::UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.rootBone = rootBone;
			}
			SetupBlobShadow(minionBuilder);
			base.gameObject.SetLayerRecursively(5);
			SetupCharacterObject(component.runtimeAnimatorController);
			SetupCharacterAnimation(component, characterUIAnimationDefinition);
			return this;
		}

		private void SetupCharacterAnimation(global::UnityEngine.Animator anim, global::Kampai.Game.CharacterUIAnimationDefinition characterUIAnimationDefinition)
		{
			if (characterUIAnimationDefinition.UseLegacy)
			{
				idleAnimationCount = characterUIAnimationDefinition.IdleCount;
				happyAnimationCount = characterUIAnimationDefinition.HappyCount;
				selectedAnimationCount = characterUIAnimationDefinition.SelectedCount;
			}
			else
			{
				SetupOverrideAnimatorController(anim);
			}
			idleWeightedInstance = CreateWeightedInstance(idleAnimationCount);
			selectedWeightedInstance = CreateWeightedInstance(selectedAnimationCount);
			happyWeightedInstance = CreateWeightedInstance(happyAnimationCount);
		}

		private void SetupOverrideAnimatorController(global::UnityEngine.Animator anim)
		{
			global::UnityEngine.AnimatorOverrideController overrideController = new global::UnityEngine.AnimatorOverrideController();
			overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;
			ReplacingClips(ref overrideController);
			anim.runtimeAnimatorController = overrideController;
		}

		private void ReplacingClips(ref global::UnityEngine.AnimatorOverrideController overrideController)
		{
			var overrides = new global::System.Collections.Generic.List<global::System.Collections.Generic.KeyValuePair<global::UnityEngine.AnimationClip, global::UnityEngine.AnimationClip>>();
			overrideController.GetOverrides(overrides);
			for (int i = 0; i < overrides.Count; i++)
			{
				GrabClipBasedOnName(ref overrideController, overrides[i].Key.name);
			}
		}

		private void GrabClipBasedOnName(ref global::UnityEngine.AnimatorOverrideController overrideController, string name)
		{
			if (name.Contains("Idle"))
			{
				idleAnimationCount++;
				overrideController[name] = GetAnimationClip(0);
			}
			else if (name.Contains("Happy"))
			{
				happyAnimationCount++;
				overrideController[name] = GetAnimationClip(1);
			}
			else if (name.Contains("Selected"))
			{
				selectedAnimationCount++;
				overrideController[name] = GetAnimationClip(2);
			}
		}

		private global::UnityEngine.AnimationClip GetAnimationClip(int weightedAnimationIndex)
		{
			int count = animationPoolWeightedInstanceList.Count;
			if (count > weightedAnimationIndex)
			{
				global::Kampai.Util.QuantityItem quantityItem = animationPoolWeightedInstanceList[weightedAnimationIndex].NextPick(randomService);
				global::Kampai.Game.UIAnimationDefinition uIAnimationDefinition = definitionService.Get<global::Kampai.Game.UIAnimationDefinition>(quantityItem.ID);
				return global::Kampai.Util.KampaiResources.Load<global::UnityEngine.AnimationClip>(uIAnimationDefinition.AnimationClipName);
			}
			if (count > 0)
			{
				logger.Error("Index out of range for animationPoolWeightedInstanceList in DummyCharacterObject, picking index 0");
				return GetAnimationClip(0);
			}
			logger.Error("No animation weighted instance found for this character {0}", ID);
			return null;
		}

		public void SetUpWifiListeners(global::strange.extensions.signal.impl.Signal openSignal, global::strange.extensions.signal.impl.Signal closeSignal)
		{
			networkOpenSignal = openSignal;
			networkCloseSignal = closeSignal;
			networkOpenSignal.AddListener(HideDummy);
			networkCloseSignal.AddListener(ShowDummy);
		}

		private void HideDummy()
		{
			base.gameObject.SetActive(false);
		}

		private void ShowDummy()
		{
			base.gameObject.SetActive(true);
			StartingState(animationState, true);
		}

		public void SetStenciledShader()
		{
			int stencilRef = 2;
			int num = 1;
			global::UnityEngine.Renderer componentInChildren = GetComponentInChildren<global::UnityEngine.Renderer>();
			if (componentInChildren != null)
			{
				global::UnityEngine.Material[] materials = componentInChildren.materials;
				foreach (global::UnityEngine.Material material in materials)
				{
					if (!(material == null) && !(material.shader == null) && !string.IsNullOrEmpty(material.shader.name))
					{
						switch (material.shader.name)
						{
						case "Kampai/Standard/Texture":
						case "Kampai/Standard/Minion":
						case "Kampai/Standard/Minion_LOD1":
							global::Kampai.Util.Graphics.ShaderUtils.EnableStencilShader(material, stencilRef, num++);
							break;
						}
					}
				}
			}
			if (shadowImage != null)
			{
				shadowImage.SetStencilMaterial();
			}
		}

		private void SetupBlobShadow(global::Kampai.Util.IMinionBuilder minionBuilder)
		{
			if (minionBuilder.GetLOD() != global::Kampai.Util.TargetPerformance.LOW && minionBuilder.GetLOD() != global::Kampai.Util.TargetPerformance.VERYLOW)
			{
				global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("MinionBlobShadow_UI") as global::UnityEngine.GameObject;
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
				shadowImage = gameObject.GetComponent<global::Kampai.UI.View.KampaiImage>();
				gameObject.transform.SetParent(base.gameObject.transform, false);
				gameObject.transform.localScale = global::UnityEngine.Vector3.one;
				gameObject.transform.localPosition = global::UnityEngine.Vector3.zero;
				gameObject.transform.localEulerAngles = new global::UnityEngine.Vector3(90f, 180f, 0f);
			}
		}

		public void RemoveCoroutine(bool isDestorying = true)
		{
			if (isDestorying)
			{
				markedForDestory = true;
			}
		}

		public virtual void OnDestroy()
		{
			if (networkOpenSignal != null)
			{
				networkOpenSignal.RemoveListener(HideDummy);
			}
			if (networkCloseSignal != null)
			{
				networkCloseSignal.RemoveListener(ShowDummy);
			}
			RemoveCoroutine();
		}

		public void StartingState(global::Kampai.UI.DummyCharacterAnimationState targetState, bool clearQueue = false)
		{
			if (!markedForDestory)
			{
				int num = 0;
				animationState = targetState;
				switch (targetState)
				{
				case global::Kampai.UI.DummyCharacterAnimationState.Idle:
					num = idleWeightedInstance.NextPick(randomService).ID;
					num = ((num == lastIndex) ? idleWeightedInstance.NextPick(randomService).ID : num);
					EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(this, logger, "IsIdle", true, "IsHappy", false), clearQueue);
					EnqueueAction(new global::Kampai.Game.View.DelayAction(this, 0.5f, logger));
					EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(this, global::UnityEngine.Animator.StringToHash("Base Layer.Idle"), logger));
					EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(this, logger, "IdleRandomizer", num));
					EnqueueAction(new global::Kampai.Game.View.PickNextDummyUIAnimationAction(this, targetState, logger));
					lastIndex = num;
					break;
				case global::Kampai.UI.DummyCharacterAnimationState.Happy:
					num = happyWeightedInstance.NextPick(randomService).ID;
					num = ((num == lastIndex) ? happyWeightedInstance.NextPick(randomService).ID : num);
					EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(this, logger, "IsIdle", false, "IsHappy", true), clearQueue);
					EnqueueAction(new global::Kampai.Game.View.DelayAction(this, 0.5f, logger));
					EnqueueAction(new global::Kampai.Game.View.WaitForMecanimStateAction(this, global::UnityEngine.Animator.StringToHash("Base Layer.Happy"), logger));
					EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(this, logger, "HappyRandomizer", num));
					EnqueueAction(new global::Kampai.Game.View.PickNextDummyUIAnimationAction(this, targetState, logger));
					lastIndex = num;
					break;
				case global::Kampai.UI.DummyCharacterAnimationState.SelectedIdle:
					num = selectedWeightedInstance.NextPick(randomService).ID;
					EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(this, logger, "SelectedRandomizer", num, "IsSelected", null, "IsIdle", true, "IsHappy", false), true);
					EnqueueAction(new global::Kampai.Game.View.DelayAction(this, 0.5f, logger));
					EnqueueAction(new global::Kampai.Game.View.PickNextDummyUIAnimationAction(this, global::Kampai.UI.DummyCharacterAnimationState.Idle, logger));
					lastIndex = -1;
					break;
				case global::Kampai.UI.DummyCharacterAnimationState.SelectedHappy:
					num = selectedWeightedInstance.NextPick(randomService).ID;
					EnqueueAction(new global::Kampai.Game.View.SetAnimatorArgumentsAction(this, logger, "SelectedRandomizer", num, "IsSelected", null, "IsIdle", false, "IsHappy", true), true);
					EnqueueAction(new global::Kampai.Game.View.DelayAction(this, 0.5f, logger));
					EnqueueAction(new global::Kampai.Game.View.PickNextDummyUIAnimationAction(this, global::Kampai.UI.DummyCharacterAnimationState.Happy, logger));
					lastIndex = -1;
					break;
				}
			}
		}

		public void StartBundlePackAnimation()
		{
			global::UnityEngine.RuntimeAnimatorController controller = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>("asm_UI_minion_bundle");
			EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(this, controller, logger), true);
		}

		public void MakePhilDance()
		{
			global::UnityEngine.RuntimeAnimatorController controller = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>("asm_UI_minion_solo_dance_loop");
			EnqueueAction(new global::Kampai.Game.View.SetAnimatorAction(this, controller, logger), true);
		}

		private void addRenderToLodList(ref global::System.Collections.Generic.List<global::Kampai.Game.View.DummyCharacterObject.LOD> lodRenderers, global::UnityEngine.Renderer renderer)
		{
			global::UnityEngine.GameObject gameObject = renderer.gameObject;
			string text = gameObject.name;
			int startIndex = text.IndexOf("LOD", global::System.StringComparison.Ordinal) + 3;
			int result = 0;
			int.TryParse(text.Substring(startIndex), out result);
			for (int i = 0; i < lodRenderers.Count; i++)
			{
				if (lodRenderers[i].Number == result)
				{
					lodRenderers[i].addRendererGameObject(gameObject);
					return;
				}
			}
			lodRenderers.Add(new global::Kampai.Game.View.DummyCharacterObject.LOD(result, gameObject));
		}

		protected void SetupLODs(bool forceHigh)
		{
			global::UnityEngine.Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<global::UnityEngine.Renderer>();
			global::System.Collections.Generic.List<global::Kampai.Game.View.DummyCharacterObject.LOD> lodRenderers = new global::System.Collections.Generic.List<global::Kampai.Game.View.DummyCharacterObject.LOD>(3);
			foreach (global::UnityEngine.Renderer renderer in componentsInChildren)
			{
				addRenderToLodList(ref lodRenderers, renderer);
				renderer.gameObject.SetActive(false);
				renderer.shadowCastingMode = global::UnityEngine.Rendering.ShadowCastingMode.Off;
				renderer.receiveShadows = false;
			}
			lodRenderers.Sort((global::Kampai.Game.View.DummyCharacterObject.LOD x, global::Kampai.Game.View.DummyCharacterObject.LOD y) => x.Number - y.Number);
			if (lodRenderers.Count > 0)
			{
				int index = ((!forceHigh) ? (lodRenderers.Count - 1) : 0);
				if (index >= 0 && index < lodRenderers.Count)
				{
					lodRenderers[index].Enable();
				}
			}
		}

		public override void SetAnimController(global::UnityEngine.RuntimeAnimatorController controller)
		{
			animators[0].runtimeAnimatorController = controller;
		}

		private void SetupCharacterObject(global::UnityEngine.RuntimeAnimatorController defaultController)
		{
			SetDefaultAnimController(defaultController);
		}

		protected string GetRootJointPath()
		{
			return "minion:ROOT/minion:pelvis_jnt";
		}

		protected void SetName(global::Kampai.Game.Character character)
		{
			if (base.gameObject.name.Length == 0)
			{
				base.gameObject.name = string.Format("Minion_{0}", character.ID);
			}
		}
	}
}
