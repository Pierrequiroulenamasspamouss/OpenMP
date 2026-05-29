namespace Kampai.Game
{
	public class MignetteMusicEmitter : global::UnityEngine.MonoBehaviour
	{
		private global::FMOD.Studio.EventInstance eventInstance;

		private static bool isShuttingDown;

		public virtual void Update()
		{
			if (!eventInstance.isValid() || HasFinished())
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public bool IsValid()
		{
			return eventInstance.isValid();
		}

		public bool HasFinished()
		{
			if (!IsValid())
			{
				return true;
			}
			return getPlaybackState() == global::FMOD.Studio.PLAYBACK_STATE.STOPPED;
		}

		public global::FMOD.Studio.PLAYBACK_STATE getPlaybackState()
		{
			if (!IsValid())
			{
				return global::FMOD.Studio.PLAYBACK_STATE.STOPPED;
			}
			global::FMOD.Studio.PLAYBACK_STATE state = global::FMOD.Studio.PLAYBACK_STATE.STOPPED;
			if (ERRCHECK(eventInstance.getPlaybackState(out state)) == global::FMOD.RESULT.OK)
			{
				return state;
			}
			return global::FMOD.Studio.PLAYBACK_STATE.STOPPED;
		}

		public void SetEventGUID(string guid)
		{
			if (!string.IsNullOrEmpty(guid))
			{
				eventInstance = FMOD_StudioSystem.instance.GetEvent(guid);
			}
		}

		public void StartEvent()
		{
			if (IsValid())
			{
				ERRCHECK(eventInstance.start());
			}
		}

		public void StopEvent()
		{
			if (IsValid())
			{
				ERRCHECK(eventInstance.stop(global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT));
			}
		}

		private void OnApplicationQuit()
		{
			isShuttingDown = true;
		}

		private void OnDestroy()
		{
			if (!isShuttingDown && eventInstance.isValid())
			{
				if (getPlaybackState() != global::FMOD.Studio.PLAYBACK_STATE.STOPPED)
				{
					ERRCHECK(eventInstance.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE));
				}
				ERRCHECK(eventInstance.release());
				eventInstance = default(global::FMOD.Studio.EventInstance);
			}
		}

		private global::FMOD.RESULT ERRCHECK(global::FMOD.RESULT result)
		{
			global::FMOD.Studio.UnityUtil.ERRCHECK(result);
			return result;
		}
	}
}
