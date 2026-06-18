namespace Kampai.Game.View.Audio
{
	public class PositionalAudioListenerView : global::Kampai.Util.KampaiView
	{
		public void UpdatePosition(global::UnityEngine.Vector3 newPosition)
		{
			base.transform.position = newPosition;
		}

		public void UpdatePosition(global::UnityEngine.Vector3 newPosition, global::UnityEngine.Quaternion newRotation)
		{
			base.transform.position = newPosition;
			base.transform.rotation = newRotation;
		}
	}
}
