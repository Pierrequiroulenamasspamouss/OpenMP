namespace Kampai.Util
{
	public class UpdateRunnerBehaviour : global::UnityEngine.MonoBehaviour
	{
		private global::System.Collections.Generic.List<global::System.Action> actionList = new global::System.Collections.Generic.List<global::System.Action>();

		private global::System.Action updateAction;

		public void Subscribe(global::System.Action action)
		{
			updateAction = (global::System.Action)global::System.Delegate.Combine(updateAction, action);
			if (!actionList.Contains(action))
			{
				actionList.Add(action);
			}
		}

		public void Unsubscribe(global::System.Action action)
		{
			updateAction = (global::System.Action)global::System.Delegate.Remove(updateAction, action);
			if (actionList.Contains(action))
			{
				actionList.Remove(action);
			}
		}

		private void Update()
		{
			if (updateAction != null)
			{
				updateAction();
			}
		}

		private void OnDestroy()
		{
			foreach (global::System.Action action in actionList)
			{
				updateAction = (global::System.Action)global::System.Delegate.Remove(updateAction, action);
			}
		}
	}
}
