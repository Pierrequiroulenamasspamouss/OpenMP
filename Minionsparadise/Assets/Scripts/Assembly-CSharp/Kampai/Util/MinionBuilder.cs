namespace Kampai.Util
{
	public class MinionBuilder : global::Kampai.Util.IMinionBuilder
	{
		private global::Kampai.Util.Boxed<global::Kampai.Util.TargetPerformance> FORCE_LOD;

		private global::Kampai.Util.TargetPerformance TargetLOD = global::Kampai.Util.TargetPerformance.MED;

		private bool restart;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("MinionBuilder") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal audioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StartLoopingAudioSignal startLoopingAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.StopLocalAudioSignal stopAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayMinionStateAudioSignal minionStateAudioSignal { get; set; }

		public MinionBuilder()
		{
			SetLOD(TargetLOD);
		}

		public global::Kampai.Game.View.MinionObject BuildMinion(global::Kampai.Game.CostumeItemDefinition costume, string animatorStateMachine, global::UnityEngine.GameObject parent, bool showShadow)
		{
			if (FORCE_LOD != null)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Warning, "Forced LOD to " + FORCE_LOD);
				restart = false;
				SetLOD(FORCE_LOD.Value);
				FORCE_LOD = null;
			}
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.SkinnedMeshAggregator.CreateAggregateObject("NEW_MINION", costume.Skeleton, costume.MeshList, TargetLOD.ToString());
			if (parent != null)
			{
				gameObject.transform.parent = parent.transform;
			}
			gameObject.transform.localPosition = global::UnityEngine.Vector3.zero;
			gameObject.transform.localEulerAngles = global::UnityEngine.Vector3.zero;
			RebuildMinion(gameObject);
			global::UnityEngine.Animator component = gameObject.GetComponent<global::UnityEngine.Animator>();
			component.applyRootMotion = true;
			component.runtimeAnimatorController = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.RuntimeAnimatorController>(animatorStateMachine);
			component.cullingMode = global::UnityEngine.AnimatorCullingMode.CullUpdateTransforms;
			global::UnityEngine.Transform pelvis = gameObject.transform.Find("minion:ROOT/minion:pelvis_jnt");
			return SetupMinionObject(gameObject, pelvis, showShadow);
		}

		public void RebuildMinion(global::UnityEngine.GameObject minion)
		{
			global::UnityEngine.Renderer[] componentsInChildren = minion.GetComponentsInChildren<global::UnityEngine.Renderer>();
			global::System.Collections.Generic.SortedDictionary<string, global::System.Collections.Generic.List<global::UnityEngine.Renderer>> sortedDictionary = new global::System.Collections.Generic.SortedDictionary<string, global::System.Collections.Generic.List<global::UnityEngine.Renderer>>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				string text = ExtractLODKey(componentsInChildren[i]);
				if (text != null)
				{
					if (!sortedDictionary.ContainsKey(text))
					{
						sortedDictionary.Add(text, new global::System.Collections.Generic.List<global::UnityEngine.Renderer>());
					}
					sortedDictionary[text].Add(componentsInChildren[i]);
				}
			}
			float[] lODHeightsArray = global::Kampai.Util.GameConstants.GetLODHeightsArray();
			int num = lODHeightsArray.Length - 1;
			float screenRelativeTransitionHeight = lODHeightsArray[num];
			global::UnityEngine.LOD[] array = new global::UnityEngine.LOD[lODHeightsArray.Length];
			using (global::System.Collections.Generic.SortedDictionary<string, global::System.Collections.Generic.List<global::UnityEngine.Renderer>>.Enumerator enumerator = sortedDictionary.GetEnumerator())
			{
				int num2 = 0;
				while (enumerator.MoveNext())
				{
					global::System.Collections.Generic.List<global::UnityEngine.Renderer> value = enumerator.Current.Value;
					global::UnityEngine.Renderer[] array2 = value.ToArray();
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j].shadowCastingMode = global::UnityEngine.Rendering.ShadowCastingMode.Off;
						array2[j].receiveShadows = false;
					}
					if (num2 < array.Length)
					{
						array[num2] = new global::UnityEngine.LOD(lODHeightsArray[num2], array2);
					}
					else
					{
						global::UnityEngine.LOD lOD = array[num];
						global::System.Collections.Generic.List<global::UnityEngine.Renderer> list = new global::System.Collections.Generic.List<global::UnityEngine.Renderer>(array2);
						if (lOD.renderers != null && lOD.renderers.Length > 0)
						{
							list.AddRange(lOD.renderers);
						}
						array[num] = new global::UnityEngine.LOD(screenRelativeTransitionHeight, list.ToArray());
					}
					num2++;
				}
			}
			global::UnityEngine.LODGroup lODGroup = minion.GetComponent<global::UnityEngine.LODGroup>();
			if (lODGroup == null)
			{
				lODGroup = minion.AddComponent<global::UnityEngine.LODGroup>();
			}
			lODGroup.SetLODs(array);
			lODGroup.RecalculateBounds();
			global::UnityEngine.Transform rootBone = minion.transform.Find("minion:ROOT/minion:pelvis_jnt");
			global::UnityEngine.SkinnedMeshRenderer[] componentsInChildren2 = minion.GetComponentsInChildren<global::UnityEngine.SkinnedMeshRenderer>();
			foreach (global::UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
			{
				skinnedMeshRenderer.rootBone = rootBone;
			}
			minion.SetLayerRecursively(8);
		}

		private global::Kampai.Game.View.MinionObject SetupMinionObject(global::UnityEngine.GameObject minion, global::UnityEngine.Transform pelvis, bool showShadow)
		{
			global::Kampai.Game.View.MinionObject minionObject = minion.AddComponent<global::Kampai.Game.View.MinionObject>();
			if (showShadow && GetLOD() != global::Kampai.Util.TargetPerformance.LOW && GetLOD() != global::Kampai.Util.TargetPerformance.VERYLOW)
			{
				global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("MinionBlobShadow") as global::UnityEngine.GameObject;
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
				gameObject.transform.parent = minion.transform;
				gameObject.GetComponent<global::Kampai.Util.MinionBlobShadowView>().SetToTrack(pelvis);
				minionObject.SetBlobShadow(gameObject);
			}
			minion.AddComponent<global::Kampai.Util.AI.SteerToAvoidCollisions>();
			minion.AddComponent<global::Kampai.Util.AI.SteerToAvoidEnvironment>();
			minion.AddComponent<global::Kampai.Util.AI.SteerToTether>();
			minion.AddComponent<global::Kampai.Util.AI.SteerMinionToWander>();
			global::Kampai.Util.AI.SteerCharacterToSeek steerCharacterToSeek = minion.AddComponent<global::Kampai.Util.AI.SteerCharacterToSeek>();
			steerCharacterToSeek.enabled = false;
			steerCharacterToSeek.Threshold = 0.1f;
			global::Kampai.Util.AI.Agent agent = minion.GetComponent<global::Kampai.Util.AI.Agent>();
			if (agent == null)
			{
				agent = minion.AddComponent<global::Kampai.Util.AI.Agent>();
			}
			agent.Radius = 0.5f;
			agent.Mass = 1f;
			agent.MaxForce = 8f;
			agent.MaxSpeed = 1f;
			global::Kampai.Game.View.AnimEventHandler animEventHandler = minion.AddComponent<global::Kampai.Game.View.AnimEventHandler>();
			animEventHandler.Init(minionObject, minionObject.localAudioEmitter, audioSignal, stopAudioSignal, minionStateAudioSignal, startLoopingAudioSignal);
			return minionObject;
		}

		public global::Kampai.Util.TargetPerformance GetLOD()
		{
			return TargetLOD;
		}

		public void SetLOD(global::Kampai.Util.TargetPerformance targetPerformance)
		{
			if (targetPerformance != global::Kampai.Util.TargetPerformance.UNKNOWN && targetPerformance != global::Kampai.Util.TargetPerformance.UNSUPPORTED)
			{
				TargetLOD = targetPerformance;
			}
			else
			{
				TargetLOD = global::Kampai.Util.TargetPerformance.MED;
				logger.Error("Unsupported/Unknown device: {0}, setting to MED", targetPerformance);
			}
			if (restart)
			{
				FORCE_LOD = new global::Kampai.Util.Boxed<global::Kampai.Util.TargetPerformance>(targetPerformance);
			}
			restart = true;
		}

		private string ExtractLODKey(global::UnityEngine.Renderer renderer)
		{
			string result = null;
			string name = renderer.name;
			if (name.Length > 4)
			{
				string text = name.Substring(name.Length - 4);
				if (text.Contains("LOD"))
				{
					result = text;
				}
			}
			return result;
		}
	}
}
