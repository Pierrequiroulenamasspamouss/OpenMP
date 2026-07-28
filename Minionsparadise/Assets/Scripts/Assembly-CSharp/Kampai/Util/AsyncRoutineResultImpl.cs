namespace Kampai.Util
{
	internal sealed class AsyncRoutineResultImpl : global::Kampai.Util.AsyncRoutineResult
	{
		public static global::Kampai.Util.AsyncRoutineResultImpl Start(global::Kampai.Util.IInvokerService invoker, global::System.Action action, global::System.Action onComplete)
		{
			global::Kampai.Util.AsyncRoutineResultImpl result = new global::Kampai.Util.AsyncRoutineResultImpl();
			global::System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					if (action != null) action();
				}
				catch (global::System.Exception ex)
				{
					global::UnityEngine.Debug.LogErrorFormat("[AsyncRoutineResultImpl] Exception in action task: {0}\n{1}", ex.Message, ex.StackTrace);
				}
				finally
				{
					result.IsDone = true;
					if (onComplete != null && invoker != null)
					{
						invoker.Add(onComplete);
					}
				}
			});
			return result;
		}

		public static global::Kampai.Util.AsyncRoutineResultImpl Start(global::Kampai.Util.IInvokerService invoker, global::Kampai.Util.ContidionTask task, global::System.Action onComplete)
		{
			global::Kampai.Util.AsyncRoutineResultImpl result = new global::Kampai.Util.AsyncRoutineResultImpl();
			global::System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					if (task != null) task();
				}
				catch (global::System.Exception ex)
				{
					global::UnityEngine.Debug.LogErrorFormat("[AsyncRoutineResultImpl] Exception in condition task: {0}\n{1}", ex.Message, ex.StackTrace);
				}
				finally
				{
					result.IsDone = true;
					if (onComplete != null && invoker != null)
					{
						invoker.Add(onComplete);
					}
				}
			});
			return result;
		}
	}
}
