namespace Kampai.Game.View
{
	public class PoolableVFX : global::strange.extensions.pool.api.IPoolable
	{
		public global::UnityEngine.GameObject vfxGO { get; private set; }

		public bool retain { get; private set; }

		public PoolableVFX(global::UnityEngine.GameObject prefab)
		{
			vfxGO = global::UnityEngine.Object.Instantiate(prefab);
		}

		public void Restore()
		{
			vfxGO.SetActive(false);
		}

		public void Retain()
		{
			retain = true;
		}

		public void Release()
		{
			vfxGO.SetActive(false);
		}

		public global::System.Collections.IEnumerator CleanupCoroutine(global::strange.extensions.pool.api.IPool<global::Kampai.Game.View.PoolableVFX> pool)
		{
			yield return new global::UnityEngine.WaitForSeconds(vfxGO.GetComponent<global::UnityEngine.ParticleSystem>().main.duration);
			pool.ReturnInstance(this);
		}
	}
}
