namespace Kampai.Game
{
	public class PlayGlobalSoundFXCommand
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("PlayGlobalSoundFXCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Common.Service.Audio.IFMODService fmodService { get; set; }

		[Inject(global::Kampai.Main.MainElement.AUDIO_LISTENER)]
		public global::UnityEngine.GameObject audioListener { get; set; }

		public void Execute(string audioSource)
		{
			FMOD_StudioSystem instance = FMOD_StudioSystem.instance;
			string guid = fmodService.GetGuid(audioSource);
			global::FMOD.Studio.EventInstance eventInstance = instance.GetEvent(guid);
			if (!eventInstance.isValid())
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Failed to Load Audio Source: " + audioSource);
				return;
			}
			eventInstance.set3DAttributes(global::FMOD.Studio.UnityUtil.to3DAttributes(audioListener));
			global::FMOD.RESULT rESULT = eventInstance.start();
			if (!global::FMOD.Studio.UnityUtil.ERRCHECK(rESULT))
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Error, "Failed to Play (Error Code: " + rESULT.ToString() + ") Audio Source: " + audioSource);
			}
		}
	}
}
