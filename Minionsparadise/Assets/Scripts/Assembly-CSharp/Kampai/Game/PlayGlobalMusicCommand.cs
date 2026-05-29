namespace Kampai.Game
{
	public class PlayGlobalMusicCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public string audioSource { get; set; }

		[Inject]
		public global::System.Collections.Generic.Dictionary<string, float> eventParameters { get; set; }

		[Inject(global::Kampai.Main.MainElement.AUDIO_LISTENER)]
		public global::UnityEngine.GameObject audioListener { get; set; }

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner runner { get; set; }

		public override void Execute()
		{
			CustomFMOD_StudioEventEmitter customFMOD_StudioEventEmitter = audioListener.GetComponent<CustomFMOD_StudioEventEmitter>();
			if (customFMOD_StudioEventEmitter == null)
			{
				customFMOD_StudioEventEmitter = audioListener.AddComponent<CustomFMOD_StudioEventEmitter>();
				if (customFMOD_StudioEventEmitter == null)
				{
					return;
				}
				customFMOD_StudioEventEmitter.startEventOnAwake = false;
			}
			bool flag = false;
			if (customFMOD_StudioEventEmitter.path != null && customFMOD_StudioEventEmitter.path != audioSource)
			{
				string guid = fmodService.GetGuid(audioSource);
				if (guid != customFMOD_StudioEventEmitter.path)
				{
					flag = true;
					customFMOD_StudioEventEmitter.Stop();
					if (customFMOD_StudioEventEmitter.evt.isValid())
					{
						customFMOD_StudioEventEmitter.evt.release();
						customFMOD_StudioEventEmitter.evt = default(global::FMOD.Studio.EventInstance);
					}
					customFMOD_StudioEventEmitter.path = fmodService.GetGuid(audioSource);
				}
			}
			customFMOD_StudioEventEmitter.SetEventParameters(eventParameters);
			if (flag)
			{
				runner.StartCoroutine(WaitAFrame(customFMOD_StudioEventEmitter));
			}
			else
			{
				customFMOD_StudioEventEmitter.UpdateEventParameters();
			}
		}

		private global::System.Collections.IEnumerator WaitAFrame(CustomFMOD_StudioEventEmitter emitter)
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			if (emitter.path != null)
			{
				emitter.StartEvent();
			}
		}
	}
}
