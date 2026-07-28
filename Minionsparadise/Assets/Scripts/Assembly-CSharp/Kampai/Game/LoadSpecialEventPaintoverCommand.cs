namespace Kampai.Game
{
	public class LoadSpecialEventPaintoverCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("LoadSpecialEventPaintoverCommand") as global::Kampai.Util.IKampaiLogger;

		[Inject]
		public global::Kampai.Game.SpecialEventItemDefinition specialEventItemDefinition { get; set; }

		[Inject(global::Kampai.Game.GameElement.SPECIAL_EVENT_PARENT)]
		public global::UnityEngine.GameObject parent { get; set; }

		public override void Execute()
		{
			string paintover = specialEventItemDefinition.Paintover;
			global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] LoadSpecialEventPaintoverCommand.Execute: Event Def={0}, Paintover Prefab Name='{1}'", specialEventItemDefinition.ID, paintover);
			global::UnityEngine.GameObject gameObject = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.GameObject>(paintover);
			if (gameObject == null)
			{
				global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] FAILED to load Special_Event paintover prefab '{0}' via KampaiResources!", paintover);
				logger.Debug("Unable to load Special_Event paintover prefab");
				return;
			}
			global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
			if (gameObject2 == null)
			{
				global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] FAILED to instantiate Special_Event paintover object '{0}'!", paintover);
				logger.Debug("Unable to instantiate Special_Event paintover object");
			}
			else
			{
				global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] SUCCESS! Instantiated paintover object '{0}' and attached to parent!", paintover);
				if (parent != null)
				{
					parent.transform.SetParent(null, false);
					parent.SetActive(true);
				}
				gameObject2.transform.SetParent(parent.transform, false);
				gameObject2.transform.localPosition = global::UnityEngine.Vector3.zero;
				gameObject2.transform.localRotation = global::UnityEngine.Quaternion.identity;
				gameObject2.transform.localScale = global::UnityEngine.Vector3.one;
				gameObject2.SetActive(true);
				foreach (global::UnityEngine.Transform child in gameObject2.transform)
				{
					child.gameObject.SetActive(true);
					global::UnityEngine.Renderer[] renderers = child.GetComponentsInChildren<global::UnityEngine.Renderer>(true);
					foreach (global::UnityEngine.Renderer r in renderers)
					{
						r.enabled = true;
					}
					global::UnityEngine.Debug.LogErrorFormat("[WINTER_DEBUG] WNTR_15 Child: Name='{0}', Active={1}, RenderersCount={2}, Pos={3}", child.name, child.gameObject.activeSelf, renderers.Length, child.position);
				}
			}
		}
	}
}
