namespace Kampai.Game.View
{
	public abstract class NamedCharacterObject : global::Kampai.Game.View.CharacterObject
	{
		private bool isIdle;

		private bool isAtAttention;

		private float spreadMin;

		private float spreadMax;

		private float attentionDuration;

		private int idleCount;

		private float currentTime;

		private float timer;

		private bool randomize;

		private bool transitionComplete;

		private global::System.Collections.Generic.List<int> knuthDeck = new global::System.Collections.Generic.List<int>();

		[Inject]
		public global::Kampai.Common.IRandomService randomService { get; set; }

		protected virtual string ActionsLayer
		{
			get
			{
				return "Base Layer.Actions";
			}
		}

		protected virtual string RandomizerString
		{
			get
			{
				return "randomizer";
			}
		}

		protected virtual string AttentionString
		{
			get
			{
				return "IsGetAttention";
			}
		}

		protected virtual string PlayAlternateString
		{
			get
			{
				return "PlayAlternate";
			}
		}

		protected virtual string AttentionStartString
		{
			get
			{
				return "AttentionStart";
			}
		}

		protected bool IsIdle
		{
			get
			{
				return isIdle;
			}
			set
			{
				isIdle = value;
				if (isIdle)
				{
					timer = randomService.NextFloat(spreadMin, spreadMax);
				}
			}
		}

		protected bool IsAtAttention
		{
			get
			{
				return isAtAttention;
			}
			set
			{
				isAtAttention = value;
				if (isAtAttention)
				{
					StartCoroutine(ReturnToIdle());
				}
				else
				{
					StopCoroutine(ReturnToIdle());
				}
			}
		}

		public abstract global::strange.extensions.signal.impl.Signal<string, global::System.Type, object> AnimSignal { get; }

		public virtual global::Kampai.Game.View.NamedCharacterObject Build(global::Kampai.Game.NamedCharacter character, global::UnityEngine.GameObject parent, global::Kampai.Util.IKampaiLogger logger, global::Kampai.Util.IMinionBuilder minionBuilder)
		{
			if (parent != null)
			{
				base.gameObject.transform.parent = parent.transform;
			}
			base.gameObject.transform.localPosition = global::UnityEngine.Vector3.zero;
			base.gameObject.transform.localEulerAngles = global::UnityEngine.Vector3.zero;
			global::Kampai.Game.NamedCharacterDefinition definition = character.Definition;
			global::Kampai.Game.Location location = definition.Location;
			if (location != null)
			{
				base.gameObject.transform.position = (global::UnityEngine.Vector3)location;
			}
			global::UnityEngine.Vector3 rotationEulers = definition.RotationEulers;
			if (rotationEulers != global::UnityEngine.Vector3.zero)
			{
				base.gameObject.transform.localEulerAngles = rotationEulers;
			}
			SetupLODs();
			string stateMachine = definition.CharacterAnimations.StateMachine;
			if (animators == null || animators.Count == 0)
			{
				animators = new global::System.Collections.Generic.List<global::UnityEngine.Animator>(base.gameObject.GetComponentsInChildren<global::UnityEngine.Animator>());
			}
			global::UnityEngine.Animator component = base.gameObject.GetComponent<global::UnityEngine.Animator>();
			if (component == null && animators.Count > 0)
			{
				component = animators[0];
			}
			global::UnityEngine.RuntimeAnimatorController runtimeAnimatorController = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(stateMachine);
			if (component != null)
			{
				component.applyRootMotion = true;
				component.cullingMode = global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms;
				SetAnimController(runtimeAnimatorController);
			}
			if (runtimeAnimatorController == null)
			{
				logger.Error("Failed to get default runtime animator controller for {0}: asm name: {1}", character.Name, stateMachine);
			}
			global::UnityEngine.Transform transform = base.gameObject.transform.Find(GetRootJointPath(character));
			global::UnityEngine.SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<global::UnityEngine.SkinnedMeshRenderer>();
			foreach (global::UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.rootBone = transform;
			}
			SetupCharacterObject(transform, runtimeAnimatorController, minionBuilder);
			return this;
		}

		protected virtual void SetupLODs()
		{
			global::UnityEngine.Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<global::UnityEngine.Renderer>();
			global::System.Array.Sort(componentsInChildren, (global::UnityEngine.Renderer x, global::UnityEngine.Renderer y) => x.gameObject.name.CompareTo(y.gameObject.name));
			global::UnityEngine.LOD[] array = new global::UnityEngine.LOD[componentsInChildren.Length];
			for (int num = 0; num < componentsInChildren.Length; num++)
			{
				array[num] = new global::UnityEngine.LOD(global::Kampai.Util.GameConstants.GetLODHeightsArray()[num], new global::UnityEngine.Renderer[1] { componentsInChildren[num] });
				componentsInChildren[num].shadowCastingMode = global::UnityEngine.Rendering.ShadowCastingMode.Off;
				componentsInChildren[num].receiveShadows = false;
			}
			global::UnityEngine.LODGroup lODGroup = base.gameObject.AddComponent<global::UnityEngine.LODGroup>();
			lODGroup.SetLODs(array);
			lODGroup.RecalculateBounds();
		}

		private void SetupCharacterObject(global::UnityEngine.Transform pelvis, global::UnityEngine.RuntimeAnimatorController defaultController, global::Kampai.Util.IMinionBuilder minionBuilder)
		{
			global::UnityEngine.Rigidbody rigidbody = base.gameObject.AddComponent<global::UnityEngine.Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
			if (minionBuilder.GetLOD() != global::Kampai.Util.TargetPerformance.LOW && minionBuilder.GetLOD() != global::Kampai.Util.TargetPerformance.VERYLOW)
			{
				global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("MinionBlobShadow") as global::UnityEngine.GameObject;
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.GetComponent<global::Kampai.Util.MinionBlobShadowView>().SetToTrack(pelvis);
				SetBlobShadow(gameObject);
			}
			SetDefaultAnimController(defaultController);
		}

		public void SetupRandomizer(global::Kampai.Game.NamedCharacterAnimationDefinition animDefinition)
		{
			spreadMin = animDefinition.SpreadMin;
			spreadMax = animDefinition.SpreadMax;
			idleCount = animDefinition.IdleCount;
			attentionDuration = animDefinition.AttentionDuration;
		}

		protected virtual void SetNextAnimation(int next)
		{
			SetAnimInteger(RandomizerString, next);
			AnimSignal.Dispatch(RandomizerString, typeof(int), next);
		}

		private global::System.Collections.IEnumerator ReturnToIdle()
		{
			yield return new global::UnityEngine.WaitForSeconds(attentionDuration);
			SetAnimBool(AttentionString, false);
			AnimSignal.Dispatch(AttentionString, typeof(bool), false);
		}

		public override void Update()
		{
			base.Update();
			if (!isIdle)
			{
				return;
			}
			string playAlternateString = PlayAlternateString;
			if (!randomize)
			{
				currentTime += global::UnityEngine.Time.deltaTime;
				if (currentTime > timer)
				{
					randomize = true;
					if (knuthDeck.Count == 0)
					{
						PopulateKnuthDeck();
					}
					int nextAnimation = DrawFromKnuthDeck();
					SetNextAnimation(nextAnimation);
					SetAnimTrigger(playAlternateString);
					AnimSignal.Dispatch(playAlternateString, typeof(bool), true);
				}
			}
			else if (!transitionComplete && !IsInAnimatorState(global::UnityEngine.Animator.StringToHash(ActionsLayer)))
			{
				transitionComplete = true;
				TransitionComplete();
			}
			else if (!GetAnimBool(playAlternateString) && IsInAnimatorState(global::UnityEngine.Animator.StringToHash(ActionsLayer)))
			{
				currentTime = 0f;
				randomize = false;
				transitionComplete = false;
				timer = randomService.NextFloat(spreadMin, spreadMax);
			}
		}

		private void PopulateKnuthDeck()
		{
			for (int i = 0; i < idleCount; i++)
			{
				knuthDeck.Add(i);
			}
			for (int j = 0; j < knuthDeck.Count; j++)
			{
				int index = randomService.NextInt(j, knuthDeck.Count);
				int value = knuthDeck[j];
				knuthDeck[j] = knuthDeck[index];
				knuthDeck[index] = value;
			}
		}

		private int DrawFromKnuthDeck()
		{
			int result = knuthDeck[knuthDeck.Count - 1];
			knuthDeck.RemoveAt(knuthDeck.Count - 1);
			return result;
		}

		protected virtual void TransitionComplete()
		{
		}

		protected override void SetName(global::Kampai.Game.Character character)
		{
			if (base.gameObject.name.Length == 0)
			{
				base.gameObject.name = string.Format("NamedCharacter_{0}_{1}", character.Name, character.ID);
			}
		}
	}
}
