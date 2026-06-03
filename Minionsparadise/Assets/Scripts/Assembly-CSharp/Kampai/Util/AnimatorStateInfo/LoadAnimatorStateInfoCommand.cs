namespace Kampai.Util.AnimatorStateInfo
{
	public class LoadAnimatorStateInfoCommand : global::strange.extensions.command.impl.Command
	{
		private const string dataResourcePath = "Debug/animator_state_info";

		private const string viewResourcePath = "Debug/UI/AnimatorStateView";

		[Inject(global::Kampai.Main.MainElement.UI_GLASSCANVAS)]
		public global::UnityEngine.GameObject glassCanvas { get; set; }

		public override void Execute()
		{
			global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>("Debug/animator_state_info");
			global::System.Collections.Generic.Dictionary<int, string> o = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::System.Collections.Generic.Dictionary<int, string>>(textAsset.text);
			base.injectionBinder.Bind<global::System.Collections.Generic.Dictionary<int, string>>().ToValue(o).ToName(global::Kampai.Util.UtilElement.ANIMATOR_STATE_DEBUG_INFO)
				.CrossContext()
				.Weak();
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Animator State Views");
			gameObject.transform.parent = glassCanvas.transform;
			gameObject.transform.localScale = global::UnityEngine.Vector3.one;
			global::UnityEngine.GameObject original = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Debug/UI/AnimatorStateView");
			global::System.Collections.Generic.List<global::UnityEngine.Transform> list = new global::System.Collections.Generic.List<global::UnityEngine.Transform>();
			global::UnityEngine.Animator[] array = global::UnityEngine.Object.FindObjectsByType<global::UnityEngine.Animator>(global::UnityEngine.FindObjectsSortMode.None);
			foreach (global::UnityEngine.Animator animator in array)
			{
				if (!list.Contains(animator.transform))
				{
					global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(original);
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.GetComponent<global::Kampai.Util.AnimatorStateInfo.AnimatorStateView>().Initialize(animator);
					list.Add(animator.transform);
				}
			}
			gameObject.transform.localPosition = new global::UnityEngine.Vector3((float)global::UnityEngine.Screen.width / -2f, (float)global::UnityEngine.Screen.height / -2f, 100f);
		}
	}
}
