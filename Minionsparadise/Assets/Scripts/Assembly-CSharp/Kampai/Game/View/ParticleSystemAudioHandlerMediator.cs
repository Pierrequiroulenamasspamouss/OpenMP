namespace Kampai.Game.View
{
	public class ParticleSystemAudioHandlerMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("ParticleSystemAudioHandlerMediator") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.View.ParticleSystemAudioHandlerView view { get; set; }

		[Inject]
		public global::Kampai.Main.PlayLocalAudioSignal playLocalAudioSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal playGlobalAudioSignal { get; set; }

		public override void OnRegister()
		{
			PlayAudio();
		}

		private void PlayAudio()
		{
			if (view.system == null)
			{
				logger.Warning("No particle system found.  Audio will not play.");
			}
			else if (string.IsNullOrEmpty(view.audioEventName))
			{
				logger.Warning("No audio event specified.  Audio will not play.");
			}
			else if (view.syncStart)
			{
				StartCoroutine(PlayAudioWithDelay());
			}
			else
			{
				PlayAudio(view.audioEventName);
			}
		}

		private global::System.Collections.IEnumerator PlayAudioWithDelay()
		{
			yield return new global::UnityEngine.WaitForSeconds(view.system.main.startDelay.constant);
			PlayAudio(view.audioEventName);
		}

		private void PlayAudio(string eventName)
		{
			if (view.isLocal)
			{
				playLocalAudioSignal.Dispatch(global::Kampai.Util.Audio.GetAudioEmitter.Get(base.gameObject, "ParticleSystem"), eventName, null);
			}
			else
			{
				playGlobalAudioSignal.Dispatch(eventName);
			}
		}
	}
}
