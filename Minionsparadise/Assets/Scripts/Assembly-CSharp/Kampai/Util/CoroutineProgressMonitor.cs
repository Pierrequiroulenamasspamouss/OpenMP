namespace Kampai.Util
{
	public class CoroutineProgressMonitor : global::Kampai.Util.ICoroutineProgressMonitor
	{
		private sealed class Task
		{
			public global::System.Collections.Generic.List<global::System.Collections.IEnumerator> CoroutinesStack = new global::System.Collections.Generic.List<global::System.Collections.IEnumerator>(2);

			public string Tag = string.Empty;

			public object Current;

			public void Reset(global::System.Collections.IEnumerator coroutine, string tag)
			{
				CoroutinesStack.Add(coroutine);
				Tag = tag;
			}

			public bool NextStep()
			{
				int num = CoroutinesStack.Count - 1;
				global::System.Collections.IEnumerator enumerator = CoroutinesStack[num];
				if (enumerator.MoveNext())
				{
					Current = enumerator.Current;
					return true;
				}
				Current = null;
				CoroutinesStack.RemoveAt(num);
				return num > 0 && NextStep();
			}
		}

		private const float allowedTime = 3f;

		private static object _waitForPreviousTaskToComplete = new object();

		private static object _waitForNextFrame = new object();

		private global::System.Collections.Generic.List<global::Kampai.Util.CoroutineProgressMonitor.Task> tasksPool = new global::System.Collections.Generic.List<global::Kampai.Util.CoroutineProgressMonitor.Task>(4);

		private global::System.Collections.Generic.Queue<global::Kampai.Util.CoroutineProgressMonitor.Task> tasksQueue = new global::System.Collections.Generic.Queue<global::Kampai.Util.CoroutineProgressMonitor.Task>(4);

		private global::System.Collections.Generic.Queue<global::Kampai.Util.CoroutineProgressMonitor.Task> waitQueue = new global::System.Collections.Generic.Queue<global::Kampai.Util.CoroutineProgressMonitor.Task>(1);

		private global::System.Collections.Generic.List<global::Kampai.Util.CoroutineProgressMonitor.Task> tasksWaitingForNextFrame = new global::System.Collections.Generic.List<global::Kampai.Util.CoroutineProgressMonitor.Task>(1);

		private global::System.Diagnostics.Stopwatch timer = global::System.Diagnostics.Stopwatch.StartNew();

		private long spentTime;

		private bool updateRegistered;

		[Inject]
		public global::Kampai.Util.IUpdateRunner updateRunner { get; set; }

		public object waitForPreviousTaskToComplete
		{
			get
			{
				return _waitForPreviousTaskToComplete;
			}
		}

		public object waitForNextFrame
		{
			get
			{
				return _waitForNextFrame;
			}
		}

		public bool HasRunningTasks()
		{
			int count = GetRunningTasksCount();
			if (count > 0)
			{
				string tasks = "";
				foreach (var t in tasksQueue) tasks += t.Tag + ", ";
				foreach (var t in waitQueue) tasks += t.Tag + " (waiting), ";
				foreach (var t in tasksWaitingForNextFrame) tasks += t.Tag + " (next frame), ";
				UnityEngine.Debug.Log("CoroutineProgressMonitor: Waiting for tasks: " + tasks);
			}
			return count > 0;
		}

		public int GetRunningTasksCount()
		{
			return tasksQueue.Count + waitQueue.Count + tasksWaitingForNextFrame.Count;
		}

		public void StartTask(global::System.Collections.IEnumerator enumerator, string tag)
		{
			if (!updateRegistered)
			{
				updateRunner.Subscribe(Update);
				updateRegistered = true;
			}
			global::Kampai.Util.CoroutineProgressMonitor.Task newTask = GetNewTask();
			newTask.Reset(enumerator, tag);
			spentTime += IntegrateTask(newTask);
		}

		private global::Kampai.Util.CoroutineProgressMonitor.Task GetNewTask()
		{
			if (tasksPool.Count == 0)
			{
				return new global::Kampai.Util.CoroutineProgressMonitor.Task();
			}
			int index = tasksPool.Count - 1;
			global::Kampai.Util.CoroutineProgressMonitor.Task result = tasksPool[index];
			tasksPool.RemoveAt(index);
			return result;
		}

		private void ReleaseTask(global::Kampai.Util.CoroutineProgressMonitor.Task task)
		{
			tasksPool.Add(task);
		}

		private void Update()
		{
			float unscaledTime = global::UnityEngine.Time.unscaledTime;
			int num = 64;
			for (int i = 0; i < tasksWaitingForNextFrame.Count; i++)
			{
				tasksQueue.Enqueue(tasksWaitingForNextFrame[i]);
			}
			tasksWaitingForNextFrame.Clear();
			while (global::UnityEngine.Time.realtimeSinceStartup - unscaledTime < 3f && num > 0 && tasksQueue.Count > 0)
			{
				spentTime += Integrate();
				num--;
			}
			spentTime = 0L;
			if (!HasRunningTasks())
			{
				updateRegistered = false;
				updateRunner.Unsubscribe(Update);
			}
		}

		private long IntegrateTask(global::Kampai.Util.CoroutineProgressMonitor.Task task)
		{
			long elapsedMilliseconds = timer.ElapsedMilliseconds;
			if (!task.NextStep())
			{
				ReleaseTask(task);
			}
			else
			{
				object current = task.Current;
				if (current == _waitForPreviousTaskToComplete)
				{
					waitQueue.Enqueue(task);
				}
				else if (current == _waitForNextFrame)
				{
					tasksWaitingForNextFrame.Add(task);
				}
				else if (current != null)
				{
					global::System.Collections.IEnumerator enumerator = current as global::System.Collections.IEnumerator;
					if (enumerator != null)
					{
						task.CoroutinesStack.Add(enumerator);
						IntegrateTask(task);
					}
					else
					{
						tasksWaitingForNextFrame.Add(task);
					}
				}
				else
				{
					tasksQueue.Enqueue(task);
				}
			}
			return timer.ElapsedMilliseconds - elapsedMilliseconds;
		}

		private long Integrate()
		{
			if (tasksQueue.Count == 0)
			{
				return 0L;
			}
			long result = IntegrateTask(tasksQueue.Dequeue());
			if (tasksQueue.Count == 0 && waitQueue.Count > 0)
			{
				tasksWaitingForNextFrame.Add(waitQueue.Dequeue());
			}
			return result;
		}
	}
}
