namespace Prime31
{
	public abstract class AbstractManager : global::UnityEngine.MonoBehaviour
	{
		private static global::Prime31.LifecycleHelper _prime31LifecycleHelperRef;

		private static global::Prime31.ThreadingCallbackHelper _threadingCallbackHelper;

		private static global::UnityEngine.GameObject _prime31GameObject;

		public static global::Prime31.LifecycleHelper coroutineSurrogate
		{
			get
			{
				if (_prime31LifecycleHelperRef == null)
				{
					global::UnityEngine.GameObject prime31ManagerGameObject = getPrime31ManagerGameObject();
					_prime31LifecycleHelperRef = prime31ManagerGameObject.AddComponent<global::Prime31.LifecycleHelper>();
				}
				return _prime31LifecycleHelperRef;
			}
		}

		public static global::Prime31.LifecycleHelper lifecycleHelper
		{
			get
			{
				return coroutineSurrogate;
			}
		}

		public static global::Prime31.ThreadingCallbackHelper getThreadingCallbackHelper()
		{
			return _threadingCallbackHelper;
		}

		public static void createThreadingCallbackHelper()
		{
			if (!(_threadingCallbackHelper != null))
			{
				_threadingCallbackHelper = global::UnityEngine.Object.FindFirstObjectByType(typeof(global::Prime31.ThreadingCallbackHelper)) as global::Prime31.ThreadingCallbackHelper;
				if (!(_threadingCallbackHelper != null))
				{
					global::UnityEngine.GameObject prime31ManagerGameObject = getPrime31ManagerGameObject();
					_threadingCallbackHelper = prime31ManagerGameObject.AddComponent<global::Prime31.ThreadingCallbackHelper>();
				}
			}
		}

		public static global::UnityEngine.GameObject getPrime31ManagerGameObject()
		{
			if (_prime31GameObject != null)
			{
				return _prime31GameObject;
			}
			_prime31GameObject = global::UnityEngine.GameObject.Find("prime[31]");
			if (_prime31GameObject == null)
			{
				_prime31GameObject = new global::UnityEngine.GameObject("prime[31]");
				global::UnityEngine.Object.DontDestroyOnLoad(_prime31GameObject);
			}
			return _prime31GameObject;
		}

		public static void initialize(global::System.Type type)
		{
			try
			{
				global::UnityEngine.MonoBehaviour monoBehaviour = global::UnityEngine.Object.FindFirstObjectByType(type) as global::UnityEngine.MonoBehaviour;
				if (!(monoBehaviour != null))
				{
					global::UnityEngine.GameObject prime31ManagerGameObject = getPrime31ManagerGameObject();
					global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject(type.Name);
					gameObject.AddComponent(type);
					gameObject.transform.parent = prime31ManagerGameObject.transform;
				}
			}
			catch (global::UnityEngine.UnityException)
			{
				global::UnityEngine.Debug.LogWarning(string.Concat("It looks like you have the ", type, " on a GameObject in your scene. Our new prefab-less manager system does not require the ", type, " to be on a GameObject.\nIt will be added to your scene at runtime automatically for you. Please remove the script from your scene."));
			}
		}

		private void Awake()
		{
			base.gameObject.name = GetType().Name;
			global::UnityEngine.Object.DontDestroyOnLoad(base.transform.root.gameObject);
		}
	}
}
