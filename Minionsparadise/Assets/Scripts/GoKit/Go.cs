public class Go : global::UnityEngine.MonoBehaviour
{
	public static GoEaseType defaultEaseType = GoEaseType.Linear;

	public static GoLoopType defaultLoopType = GoLoopType.RestartFromBeginning;

	public static GoUpdateType defaultUpdateType = GoUpdateType.Update;

	public static GoDuplicatePropertyRuleType duplicatePropertyRule = GoDuplicatePropertyRuleType.None;

	public static GoLogLevel logLevel = GoLogLevel.Warn;

	public static bool validateTargetObjectsEachTick = true;

	private static bool _applicationIsQuitting = false;

	private static global::System.Collections.Generic.List<AbstractGoTween> _tweens = new global::System.Collections.Generic.List<AbstractGoTween>();

	private bool _timeScaleIndependentUpdateIsRunning;

	private static Go _instance = null;

	public static Go instance
	{
		get
		{
			if (!_instance && !_applicationIsQuitting)
			{
				_instance = global::UnityEngine.Object.FindFirstObjectByType(typeof(Go)) as Go;
				if (!_instance)
				{
					global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("GoKit");
					_instance = gameObject.AddComponent<Go>();
					global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
			return _instance;
		}
	}

	private void handleUpdateOfType(GoUpdateType updateType, float deltaTime)
	{
		for (int num = _tweens.Count - 1; num >= 0; num--)
		{
			AbstractGoTween abstractGoTween = _tweens[num];
			if (abstractGoTween.state == GoTweenState.Destroyed)
			{
				removeTween(abstractGoTween);
			}
			else if (abstractGoTween.updateType == updateType && abstractGoTween.state == GoTweenState.Running && abstractGoTween.update(deltaTime * abstractGoTween.timeScale) && (abstractGoTween.state == GoTweenState.Destroyed || abstractGoTween.autoRemoveOnComplete))
			{
				removeTween(abstractGoTween);
				abstractGoTween.destroy();
			}
		}
	}

	private void Update()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.Update, global::UnityEngine.Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.LateUpdate, global::UnityEngine.Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.FixedUpdate, global::UnityEngine.Time.deltaTime);
		}
	}

	private void OnApplicationQuit()
	{
		_instance = null;
		global::UnityEngine.Object.Destroy(base.gameObject);
		_applicationIsQuitting = true;
	}

	private global::System.Collections.IEnumerator timeScaleIndependentUpdate()
	{
		_timeScaleIndependentUpdateIsRunning = true;
		float time = global::UnityEngine.Time.realtimeSinceStartup;
		while (_tweens.Count > 0)
		{
			float elapsed = global::UnityEngine.Time.realtimeSinceStartup - time;
			time = global::UnityEngine.Time.realtimeSinceStartup;
			handleUpdateOfType(GoUpdateType.TimeScaleIndependentUpdate, elapsed);
			yield return null;
		}
		_timeScaleIndependentUpdateIsRunning = false;
	}

	private static bool handleDuplicatePropertiesInTween(GoTween tween)
	{
		global::System.Collections.Generic.List<GoTween> list = tweensWithTarget(tween.target);
		global::System.Collections.Generic.List<AbstractTweenProperty> list2 = tween.allTweenProperties();
		for (int i = 0; i < list.Count; i++)
		{
			GoTween goTween = list[i];
			for (int j = 0; j < list2.Count; j++)
			{
				AbstractTweenProperty property = list2[j];
				if (goTween.containsTweenProperty(property))
				{
					if (duplicatePropertyRule == GoDuplicatePropertyRuleType.DontAddCurrentProperty)
					{
						return true;
					}
					if (duplicatePropertyRule == GoDuplicatePropertyRuleType.RemoveRunningProperty)
					{
						goTween.removeTweenProperty(property);
					}
					return false;
				}
			}
		}
		return false;
	}

	[global::System.Diagnostics.Conditional("UNITY_EDITOR")]
	private static void log(object format, params object[] paramList)
	{
		if (format is string)
		{
			global::UnityEngine.Debug.Log(string.Format(format as string, paramList));
		}
		else
		{
			global::UnityEngine.Debug.Log(format);
		}
	}

	[global::System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void warn(object format, params object[] paramList)
	{
		if (logLevel != GoLogLevel.None && logLevel != GoLogLevel.Info)
		{
			if (format is string)
			{
				global::UnityEngine.Debug.LogWarning(string.Format(format as string, paramList));
			}
			else
			{
				global::UnityEngine.Debug.LogWarning(format);
			}
		}
	}

	[global::System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void error(object format, params object[] paramList)
	{
		if (logLevel != GoLogLevel.None && logLevel != GoLogLevel.Info && logLevel != GoLogLevel.Warn)
		{
			if (format is string)
			{
				global::UnityEngine.Debug.LogError(string.Format(format as string, paramList));
			}
			else
			{
				global::UnityEngine.Debug.LogError(format);
			}
		}
	}

	public static void killAllTweens()
	{
		for (int i = 0; i < _tweens.Count; i++)
		{
			AbstractGoTween abstractGoTween = _tweens[i];
			abstractGoTween.destroy();
		}
	}

	public static GoTween to(object target, float duration, GoTweenConfig config)
	{
		config.setIsTo();
		GoTween goTween = new GoTween(target, duration, config);
		addTween(goTween);
		return goTween;
	}

	public static GoTween to(object target, GoSpline path, float speed, GoTweenConfig config)
	{
		config.setIsTo();
		path.buildPath();
		float duration = path.pathLength / speed;
		GoTween goTween = new GoTween(target, duration, config);
		addTween(goTween);
		return goTween;
	}

	public static GoTween from(object target, float duration, GoTweenConfig config)
	{
		config.setIsFrom();
		GoTween goTween = new GoTween(target, duration, config);
		addTween(goTween);
		return goTween;
	}

	public static GoTween from(object target, GoSpline path, float speed, GoTweenConfig config)
	{
		config.setIsFrom();
		path.buildPath();
		float duration = path.pathLength / speed;
		GoTween goTween = new GoTween(target, duration, config);
		addTween(goTween);
		return goTween;
	}

	public static void addTween(AbstractGoTween tween)
	{
		if (tween.isValid() && !_tweens.Contains(tween) && (duplicatePropertyRule == GoDuplicatePropertyRuleType.None || !(tween is GoTween) || (!handleDuplicatePropertiesInTween(tween as GoTween) && tween.isValid())))
		{
			_tweens.Add(tween);
			if (!instance.enabled)
			{
				_instance.enabled = true;
			}
			if (tween is GoTween && ((GoTween)tween).isFrom && tween.state != GoTweenState.Paused)
			{
				tween.update(0f);
			}
			if (!_instance._timeScaleIndependentUpdateIsRunning && tween.updateType == GoUpdateType.TimeScaleIndependentUpdate)
			{
				_instance.StartCoroutine(_instance.timeScaleIndependentUpdate());
			}
		}
	}

	public static bool removeTween(AbstractGoTween tween)
	{
		if (_tweens.Contains(tween))
		{
			_tweens.Remove(tween);
			if (_instance != null && _tweens.Count == 0)
			{
				_instance.enabled = false;
			}
			return true;
		}
		return false;
	}

	public static void removeTweenWithTag(string tag)
	{
		global::System.Collections.Generic.List<AbstractGoTween> list = tweensWithTag(tag);
		if (list == null)
		{
			return;
		}
		foreach (AbstractGoTween item in list)
		{
			removeTween(item);
		}
	}

	public static global::System.Collections.Generic.List<AbstractGoTween> tweensWithTag(string tag)
	{
		global::System.Collections.Generic.List<AbstractGoTween> list = null;
		foreach (AbstractGoTween tween in _tweens)
		{
			if (tween.tag == tag)
			{
				if (list == null)
				{
					list = new global::System.Collections.Generic.List<AbstractGoTween>();
				}
				list.Add(tween);
			}
		}
		return list;
	}

	public static global::System.Collections.Generic.List<AbstractGoTween> tweensWithId(int id)
	{
		global::System.Collections.Generic.List<AbstractGoTween> list = null;
		for (int i = 0; i < _tweens.Count; i++)
		{
			AbstractGoTween abstractGoTween = _tweens[i];
			if (abstractGoTween.id == id)
			{
				if (list == null)
				{
					list = new global::System.Collections.Generic.List<AbstractGoTween>();
				}
				list.Add(abstractGoTween);
			}
		}
		return list;
	}

	public static global::System.Collections.Generic.List<GoTween> tweensWithTarget(object target, bool traverseCollections = false)
	{
		global::System.Collections.Generic.List<GoTween> list = new global::System.Collections.Generic.List<GoTween>();
		for (int i = 0; i < _tweens.Count; i++)
		{
			AbstractGoTween abstractGoTween = _tweens[i];
			GoTween goTween = abstractGoTween as GoTween;
			if (goTween != null && goTween.target == target)
			{
				list.Add(goTween);
			}
			if (!traverseCollections || goTween != null)
			{
				continue;
			}
			AbstractGoTweenCollection abstractGoTweenCollection = abstractGoTween as AbstractGoTweenCollection;
			if (abstractGoTweenCollection != null)
			{
				global::System.Collections.Generic.List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public static void killAllTweensWithTarget(object target)
	{
		global::System.Collections.Generic.List<GoTween> list = tweensWithTarget(target, true);
		for (int i = 0; i < list.Count; i++)
		{
			GoTween goTween = list[i];
			goTween.destroy();
		}
	}
}
