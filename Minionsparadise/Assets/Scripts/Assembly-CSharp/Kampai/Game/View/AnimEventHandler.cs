namespace Kampai.Game.View
{
	public class AnimEventHandler : global::UnityEngine.MonoBehaviour
	{
		private enum AudioEventType
		{
			Default = 0,
			MinionState = 1,
			Looping = 2
		}

		protected const string MISSING_PARENT = "Missing parent for ";

		private global::Kampai.Main.PlayLocalAudioSignal audioSignal;

		private global::Kampai.Main.StopLocalAudioSignal stopAudioSignal;

		private global::Kampai.Main.PlayMinionStateAudioSignal minionStateAudioSignal;

		private global::Kampai.Main.StartLoopingAudioSignal startLoopingAudioSignal;

		private global::Kampai.Util.VFXScript siblingVFXScript;

		private global::System.Action<global::Kampai.Game.View.AnimEventHandler> binder;

		private static global::System.Collections.Generic.HashSet<string> loopingEvents = new global::System.Collections.Generic.HashSet<string>
		{
			"MasterChargingStation", "MasterIceMountain", "MasterSwampGas", "MasterCraftingTool", "MasterCraftingGadget", "MasterHotTub", "Play_leisure_firepit_fire_01", "Play_crafting_forge_01", "Play_crafting_gizmos_01", "Play_villainLair_component_thermalBattery_01",
			"Play_villainLair_component_ultravioletEmitter_01", "Play_villainLair_component_freqSpacer_01", "Play_crafting_dessert_01"
		};

		private static global::System.Collections.Generic.HashSet<string> minionStateEvents = new global::System.Collections.Generic.HashSet<string> { "Paired_Awareness_State", "Paired_Gacha_State", "Solo_Awareness_State", "Solo_Gacha_State", "Solo_Idle_State", "TownSquare_State", "Trio_Awareness_State", "Trio_Gacha_State" };

		private CustomFMOD_StudioEventEmitter localEmitter;

		private global::Kampai.Game.View.ActionableObject parentObject;

		private global::System.Collections.Generic.Dictionary<string, CustomFMOD_StudioEventEmitter> secondaryEmitters = new global::System.Collections.Generic.Dictionary<string, CustomFMOD_StudioEventEmitter>();

		private global::System.Collections.Generic.Dictionary<string, float> loopingEventParameters = new global::System.Collections.Generic.Dictionary<string, float>(1);

		private global::strange.extensions.signal.impl.Signal stopBuildingAudioInIdleStateSignal;

		public bool IsStopBuildingAudioSignalSet
		{
			get
			{
				return stopBuildingAudioInIdleStateSignal != null;
			}
		}

		public bool mute { get; set; }

		public void SetStopBuildingAudioInIdleStateSignal(global::strange.extensions.signal.impl.Signal signal)
		{
			if (stopBuildingAudioInIdleStateSignal != null)
			{
				stopBuildingAudioInIdleStateSignal.RemoveListener(OnStopAudio);
				stopBuildingAudioInIdleStateSignal = null;
			}
			if (signal != null)
			{
				stopBuildingAudioInIdleStateSignal = signal;
				stopBuildingAudioInIdleStateSignal.AddListener(OnStopAudio);
			}
		}

		public void Init(global::Kampai.Game.View.ActionableObject parent, CustomFMOD_StudioEventEmitter emitter, global::Kampai.Main.PlayLocalAudioSignal audioSignal, global::Kampai.Main.StopLocalAudioSignal stopAudioSignal, global::Kampai.Main.PlayMinionStateAudioSignal minionStateAudioSignal, global::Kampai.Main.StartLoopingAudioSignal startLoopingAudioSignal)
		{
			this.audioSignal = audioSignal;
			this.stopAudioSignal = stopAudioSignal;
			this.minionStateAudioSignal = minionStateAudioSignal;
			this.startLoopingAudioSignal = startLoopingAudioSignal;
			parentObject = parent;
			localEmitter = emitter;
		}

		public void Init(global::UnityEngine.GameObject go, global::Kampai.Main.PlayLocalAudioSignal audioSignal, global::Kampai.Main.StopLocalAudioSignal stopAudioSignal, global::Kampai.Main.PlayMinionStateAudioSignal minionStateAudioSignal, global::Kampai.Main.StartLoopingAudioSignal startLoopingAudioSignal)
		{
			this.audioSignal = audioSignal;
			this.stopAudioSignal = stopAudioSignal;
			this.minionStateAudioSignal = minionStateAudioSignal;
			this.startLoopingAudioSignal = startLoopingAudioSignal;
			parentObject = null;
			localEmitter = global::Kampai.Util.Audio.GetAudioEmitter.Get(go, "LocalAudio");
		}

		public void OnDestroy()
		{
			if (stopBuildingAudioInIdleStateSignal != null)
			{
				stopBuildingAudioInIdleStateSignal.RemoveListener(OnStopAudio);
			}
		}

		public virtual void OnPlayAudio(global::UnityEngine.AnimationEvent animationEvent)
		{
			string stringParameter = animationEvent.stringParameter;
			if (stringParameter != null)
			{
				string[] tempArray = stringParameter.Split(new char[1] { '|' }, global::System.StringSplitOptions.RemoveEmptyEntries);
				if (tempArray.Length > 0 && minionStateEvents.Contains(tempArray[0]))
				{
					//UnityEngine.Debug.Log(string.Format("[AnimEventHandler] OnPlayAudio (MinionState Event): event={0}, mute={1}, gameObject={2}", tempArray[0], mute, base.gameObject.name));
				}
			}

			if (mute)
			{
				return;
			}
			if (stringParameter == null)
			{
				return;
			}
			string[] array = stringParameter.Split(new char[1] { '|' }, global::System.StringSplitOptions.RemoveEmptyEntries);
			string clipName = array[0];
			string text = string.Empty;
			CustomFMOD_StudioEventEmitter emitter = localEmitter;
			if (array.Length > 1)
			{
				text = array[1];
				if (!secondaryEmitters.ContainsKey(text))
				{
					secondaryEmitters.Add(text, global::Kampai.Util.Audio.GetAudioEmitter.Get(base.gameObject, text));
				}
				emitter = secondaryEmitters[text];
			}
			global::Kampai.Game.View.AnimEventHandler.AudioEventType eventType = GetEventType(clipName);
			if (audioSignal != null)
			{
				switch (eventType)
				{
				case global::Kampai.Game.View.AnimEventHandler.AudioEventType.Default:
					HandleDefaultEventType(clipName, emitter);
					break;
				case global::Kampai.Game.View.AnimEventHandler.AudioEventType.MinionState:
					HandleMinionStateEventType(animationEvent, clipName, emitter, text);
					break;
				case global::Kampai.Game.View.AnimEventHandler.AudioEventType.Looping:
					HandleLoopingEventType(animationEvent, clipName, emitter);
					break;
				default:
					throw new global::System.ComponentModel.InvalidEnumArgumentException("audioEventType", (int)eventType, typeof(global::Kampai.Game.View.AnimEventHandler.AudioEventType));
				}
			}
		}

		public virtual void ShowProp(string propName)
		{
			parentObject.AddProp(propName, base.gameObject);
		}

		public virtual void HideProp(string propName)
		{
			parentObject.RemoveProp(propName);
		}

		private void HandleDefaultEventType(string clipName, CustomFMOD_StudioEventEmitter emitter)
		{
			stopAudioSignal.Dispatch(emitter);
			audioSignal.Dispatch(emitter, clipName, null);
		}

		private void HandleMinionStateEventType(global::UnityEngine.AnimationEvent animationEvent, string clipName, CustomFMOD_StudioEventEmitter emitter, string emitterKey)
		{
			stopAudioSignal.Dispatch(emitter);
			float floatParameter = animationEvent.floatParameter;
			MinionStateAudioArgs minionStateAudioArgs = new MinionStateAudioArgs();
			minionStateAudioArgs.source = parentObject;
			minionStateAudioArgs.audioEvent = clipName;
			minionStateAudioArgs.emitterKey = emitterKey;
			minionStateAudioArgs.cueId = floatParameter;
			MinionStateAudioArgs type = minionStateAudioArgs;
			minionStateAudioSignal.Dispatch(type);
		}

		private void HandleLoopingEventType(global::UnityEngine.AnimationEvent animationEvent, string clipName, CustomFMOD_StudioEventEmitter emitter)
		{
			loopingEventParameters["Cue"] = animationEvent.floatParameter;
			startLoopingAudioSignal.Dispatch(emitter, clipName, loopingEventParameters);
		}

		private global::Kampai.Game.View.AnimEventHandler.AudioEventType GetEventType(string clipName)
		{
			if (loopingEvents.Contains(clipName))
			{
				return global::Kampai.Game.View.AnimEventHandler.AudioEventType.Looping;
			}
			if (minionStateEvents.Contains(clipName))
			{
				return global::Kampai.Game.View.AnimEventHandler.AudioEventType.MinionState;
			}
			return global::Kampai.Game.View.AnimEventHandler.AudioEventType.Default;
		}

		public void SetSiblingVFXScript(global::Kampai.Util.VFXScript vfxScript)
		{
			siblingVFXScript = vfxScript;
		}

		public void OnStopAudio()
		{
			if (mute || stopAudioSignal == null)
			{
				return;
			}
			stopAudioSignal.Dispatch(localEmitter);
			foreach (global::System.Collections.Generic.KeyValuePair<string, CustomFMOD_StudioEventEmitter> secondaryEmitter in secondaryEmitters)
			{
				stopAudioSignal.Dispatch(secondaryEmitter.Value);
			}
		}

		public void StopAllFX()
		{
			foreach (global::UnityEngine.Transform item in base.transform)
			{
				if (!(item.name == "fx"))
				{
					continue;
				}
				foreach (global::UnityEngine.Transform item2 in item.transform)
				{
					global::UnityEngine.ParticleSystem component = item2.gameObject.GetComponent<global::UnityEngine.ParticleSystem>();
					if (component.isPlaying)
					{
						component.Stop();
					}
				}
			}
		}

		public void OnPlayFX(string fxName)
		{
			string[] array = fxName.Split(',');
			foreach (global::UnityEngine.Transform item in base.transform)
			{
				if (!(item.name == "fx"))
				{
					continue;
				}
				foreach (global::UnityEngine.Transform item2 in item.transform)
				{
					string[] array2 = array;
					foreach (string text in array2)
					{
						if (item2.name == text)
						{
							global::UnityEngine.ParticleSystem component = item2.gameObject.GetComponent<global::UnityEngine.ParticleSystem>();
							if (!component.isPlaying)
							{
								component.Play();
							}
						}
					}
				}
			}
		}

		public void animEvent(string state)
		{
			AnimVFX(state);
		}

		public void AnimVFX(string state)
		{
			ResolveBindings();
			global::Kampai.Game.View.MinionObject component = GetComponent<global::Kampai.Game.View.MinionObject>();
			global::Kampai.Util.VFXScript vFXScript = null;
			vFXScript = ((!base.name.Contains("_LOD")) ? GetComponent<global::Kampai.Util.VFXScript>() : GetComponentInParent<global::Kampai.Util.VFXScript>());
			if (component != null)
			{
				component.AnimVFX(state);
			}
			else if (vFXScript != null)
			{
				vFXScript.AnimVFX(state);
			}
			if (siblingVFXScript != null)
			{
				siblingVFXScript.AnimVFX(state);
			}
		}

		private void ResolveBindings()
		{
			if (binder != null)
			{
				if (siblingVFXScript == null)
				{
					binder(this);
				}
				else
				{
					binder = null;
				}
			}
		}

		public void SetVFXScriptBinder(global::System.Action<global::Kampai.Game.View.AnimEventHandler> binder)
		{
			this.binder = binder;
		}
	}
}
