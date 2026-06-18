namespace Kampai.Game.View.Audio
{
	public class PositionalAudioListenerMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private const float CAMERA_HEIGHT_OFFSET = 5f;

		private global::UnityEngine.Camera cameraAnchor;

		private bool characteAudioEnabled = true;

		private global::UnityEngine.Transform audioListenerOriginalParent;

		private global::UnityEngine.Transform newParent;

		[Inject]
		public global::Kampai.Game.View.Audio.PositionalAudioListenerView view { get; set; }

		[Inject(global::Kampai.Main.MainElement.CAMERA)]
		public global::UnityEngine.Camera mainCamera { get; set; }

		[Inject]
		public global::Kampai.Game.View.CameraUtils cameraUtils { get; set; }

		[Inject]
		public global::Kampai.Main.MoveAudioListenerSignal moveCharacterAudio { get; set; }

		public override void OnRegister()
		{
			audioListenerOriginalParent = base.transform.parent;
			cameraAnchor = mainCamera;
			moveCharacterAudio.AddListener(Toggle);
		}

		public override void OnRemove()
		{
			moveCharacterAudio.RemoveListener(Toggle);
		}

		private void Toggle(bool toggle, global::UnityEngine.Transform newParent)
		{
			if (newParent != null)
			{
				this.newParent = newParent;
				base.transform.localPosition = global::UnityEngine.Vector3.zero;
			}
			else
			{
				base.transform.parent = audioListenerOriginalParent;
			}
			characteAudioEnabled = toggle;
			Update();
		}

		private void Update()
		{
			float y = cameraAnchor.gameObject.transform.position.y - 5f;
			global::UnityEngine.Vector3 vector = cameraUtils.CameraCenterRaycast(cameraAnchor);
			if (characteAudioEnabled)
			{
				view.UpdatePosition(new global::UnityEngine.Vector3(vector.x, y, vector.z), cameraAnchor.transform.rotation);
			}
			else if (newParent != null)
			{
				view.UpdatePosition(newParent.position, newParent.rotation);
			}
		}
	}
}
