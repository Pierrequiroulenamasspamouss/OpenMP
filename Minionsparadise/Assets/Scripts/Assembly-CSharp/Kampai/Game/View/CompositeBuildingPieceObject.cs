namespace Kampai.Game.View
{
	public class CompositeBuildingPieceObject : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Renderer BoundsRenderer;

		public global::UnityEngine.GameObject[] TopPieceEnabledObjects;

		public global::UnityEngine.GameObject[] TopPieceDisabledObjects;

		public global::UnityEngine.GameObject[] AllCollectedEnabledObjects;

		public global::UnityEngine.GameObject[] AllCollectedDisabledObjects;

		private global::UnityEngine.Animator animator;

		public int PieceID { get; set; }

		private void Awake()
		{
			animator = GetComponent<global::UnityEngine.Animator>();
		}

		public global::UnityEngine.Vector3 GetMaxBounds()
		{
			return BoundsRenderer.bounds.max;
		}

		public void PlayFallInAnimation()
		{
			animator.Play("FallIn", 0, 0f);
		}

		public void PlayFallInShuffleTopAnimation()
		{
			animator.Play("FallInShuffleTop", 0, 0f);
		}

		public void PlayFallInShuffleNotTopAnimation()
		{
			animator.Play("FallInShuffleNotTop", 0, 0f);
		}

		public void PlayJumpAnimation()
		{
			animator.Play("Jump", 0, 0f);
		}

		public void RefreshAppearance(bool isOnTop, bool allPiecesCollected)
		{
			animator.SetBool("IsTopPiece", isOnTop);
			animator.SetBool("AllPiecesCollected", allPiecesCollected);
			SetGameObjectsActive(TopPieceEnabledObjects, isOnTop);
			SetGameObjectsActive(AllCollectedEnabledObjects, allPiecesCollected);
			SetGameObjectsActive(TopPieceDisabledObjects, !isOnTop);
			SetGameObjectsActive(AllCollectedDisabledObjects, !allPiecesCollected);
		}

		private void SetGameObjectsActive(global::UnityEngine.GameObject[] gameObjects, bool isActive)
		{
			for (int i = 0; i < gameObjects.Length; i++)
			{
				global::UnityEngine.ParticleSystem component = gameObjects[i].GetComponent<global::UnityEngine.ParticleSystem>();
				if (component != null)
				{
					if (isActive)
					{
						gameObjects[i].SetActive(true);
						component.Play();
					}
					else
					{
						component.Stop();
						StartCoroutine(waitThenDisableParticleSystem(gameObjects[i], component.main.startLifetime.constant));
					}
				}
				else
				{
					gameObjects[i].SetActive(isActive);
				}
			}
		}

		private global::System.Collections.IEnumerator waitThenDisableParticleSystem(global::UnityEngine.GameObject gameObject, float secondsToWait)
		{
			yield return new global::UnityEngine.WaitForSeconds(secondsToWait);
			if (gameObject != null)
			{
				global::UnityEngine.ParticleSystem particleSystem = gameObject.GetComponent<global::UnityEngine.ParticleSystem>();
				if (particleSystem != null && particleSystem.isStopped)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
