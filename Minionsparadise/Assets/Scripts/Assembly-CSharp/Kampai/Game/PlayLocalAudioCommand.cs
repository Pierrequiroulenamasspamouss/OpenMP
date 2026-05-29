namespace Kampai.Game
{
	public class PlayLocalAudioCommand
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("PlayLocalAudioCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		public void Execute(CustomFMOD_StudioEventEmitter emitter, string audioEvent, global::System.Collections.Generic.Dictionary<string, float> eventParameters)
		{
			global::FMOD.Studio.PLAYBACK_STATE state = global::FMOD.Studio.PLAYBACK_STATE.STOPPED;
			if (emitter.evt.isValid())
			{
				emitter.evt.getPlaybackState(out state);
			}
			string guid = fmodService.GetGuid(audioEvent);
			if (string.IsNullOrEmpty(guid))
			{
				//UnityEngine.Debug.LogWarning(string.Format("[PlayLocalAudioCommand] Failed to find GUID for audioEvent: {0}", audioEvent));
				logger.Error("Failed to find event {0}", audioEvent);
			}
			else
			{
				if (state == global::FMOD.Studio.PLAYBACK_STATE.PLAYING || state == global::FMOD.Studio.PLAYBACK_STATE.STARTING)
				{
					//UnityEngine.Debug.Log(string.Format("[PlayLocalAudioCommand] Skipping audioEvent {0} because emitter {1} is already in state {2}", audioEvent, (emitter != null) ? emitter.name : "null", state));
					return;
				}
				if (emitter.path != null && emitter.path != guid)
				{
					if (emitter.evt.isValid())
					{
						emitter.evt.release();
						emitter.evt = default(global::FMOD.Studio.EventInstance);
					}
					emitter.path = guid;
				}
				emitter.SetEventParameters(eventParameters);
				if (emitter.path != null)
				{
					//UnityEngine.Debug.Log(string.Format("[PlayLocalAudioCommand] Starting audioEvent: {0} (guid: {1}) on emitter: {2}", audioEvent, guid, (emitter != null) ? emitter.name : "null"));
					emitter.StartEvent();
				}
			}
		}
	}
}
