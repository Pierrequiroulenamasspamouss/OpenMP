namespace Kampai.Util
{
	public class TrailRendererSortOrder : global::UnityEngine.MonoBehaviour
	{
		public int SortingLayer = -1;

		private void Start()
		{
			global::UnityEngine.TrailRenderer component = base.gameObject.GetComponent<global::UnityEngine.TrailRenderer>();
			if (component != null)
			{
                component.Clear();
                component.sortingOrder = SortingLayer;
			}
		}
	}
}
