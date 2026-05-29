namespace Kampai.Game
{
	public class StartLoopingAudioCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		public void Execute(CustomFMOD_StudioEventEmitter emitter, string eventName, global::System.Collections.Generic.Dictionary<string, float> parameters)
		{
			string guid = fmodService.GetGuid(eventName);
			if (emitter.path != null && emitter.path != guid)
			{
				if (emitter.evt.isValid())
				{
					emitter.evt.release();
					emitter.evt = default(global::FMOD.Studio.EventInstance);
				}
				emitter.path = guid;
			}
			emitter.SetEventParameters(parameters);
			global::FMOD.Studio.PLAYBACK_STATE playbackState = emitter.getPlaybackState();
			if (playbackState == global::FMOD.Studio.PLAYBACK_STATE.STOPPING || playbackState == global::FMOD.Studio.PLAYBACK_STATE.STOPPED)
			{
				emitter.StartEvent();
			}
			else
			{
				emitter.UpdateEventParameters();
			}
		}
	}
}
