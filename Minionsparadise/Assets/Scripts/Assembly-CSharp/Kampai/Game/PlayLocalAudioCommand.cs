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
            if (emitter.HasValidEventInstance())
            {
                emitter.evt.getPlaybackState(out state);
            }
            string guid = fmodService.GetGuid(audioEvent);
            if (string.IsNullOrEmpty(guid))
            {
                logger.Error("Failed to find event {0}", audioEvent);
            }
            else
            {
                if (state == global::FMOD.Studio.PLAYBACK_STATE.PLAYING || state == global::FMOD.Studio.PLAYBACK_STATE.STARTING)
                {
                    return;
                }
                if (emitter.path != null && emitter.path != guid)
                {
                    emitter.ReleaseEventInstance();
                    emitter.path = guid;
                }
                emitter.SetEventParameters(eventParameters);
                if (emitter.path != null)
                {
                    emitter.StartEvent();
                }
            }
        }
    }
}