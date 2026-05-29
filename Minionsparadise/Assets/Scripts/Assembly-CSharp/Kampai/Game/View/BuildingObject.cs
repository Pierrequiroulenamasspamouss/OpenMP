using UnityEngine;

namespace Kampai.Game.View
{
	public abstract class BuildingObject : global::Kampai.Game.View.BuildingDefinitionObject
	{
		private global::strange.extensions.signal.impl.Signal<int, global::Kampai.Game.View.MinionTaskInfo> updateSignal;

		private readonly global::strange.extensions.signal.impl.Signal stopBuildingAudioInIdleStateSignal = new global::strange.extensions.signal.impl.Signal();

		protected global::UnityEngine.Collider minColliderY;

		private global::UnityEngine.Renderer maxRendererY;

		private global::UnityEngine.Bounds combinedRendererBounds;

		private global::UnityEngine.Color highlightColor = global::UnityEngine.Color.grey;

		private global::UnityEngine.Animation cachedAnimation;

		protected global::Kampai.Game.IDefinitionService definitionService;

		private global::strange.extensions.signal.impl.Signal networkOpenSignal;

		private global::strange.extensions.signal.impl.Signal netowrkCloseSignal;

		public global::UnityEngine.Vector3 IndicatorPosition
		{
			get
			{
				return GetIndicatorPosition(false);
			}
		}

		public global::UnityEngine.Vector3 Center
		{
			get
			{
				return GetCenter(true);
			}
		}

		public global::UnityEngine.Vector3 ZoomCenter
		{
			get
			{
				return GetZoomCenterPosition();
			}
		}

		public global::strange.extensions.signal.impl.Signal StopBuildingAudioInIdleStateSignal
		{
			get
			{
				return stopBuildingAudioInIdleStateSignal;
			}
		}

		public global::UnityEngine.GameObject MinionPartyDecorations { get; set; }

		protected global::strange.extensions.signal.impl.Signal<int, global::Kampai.Game.View.MinionTaskInfo> GetUpdateSignal()
		{
			if (updateSignal == null)
			{
				updateSignal = base.transform.parent.GetComponent<global::Kampai.Game.View.BuildingManagerView>().updateMinionSignal;
			}
			return updateSignal;
		}

		internal virtual void Highlight(bool enabled)
		{
			global::UnityEngine.Color materialColor = ((!enabled) ? global::UnityEngine.Color.white : highlightColor);
			SetMaterialColor(materialColor);
		}

		internal virtual void Init(global::Kampai.Game.Building building, global::Kampai.Util.IKampaiLogger logger, global::System.Collections.Generic.IDictionary<string, global::UnityEngine.RuntimeAnimatorController> controllers, global::Kampai.Game.IDefinitionService definitionService)
		{
			base.logger = logger;
			this.definitionService = definitionService;
			ID = building.ID;
			base.IsRotated = building.IsRotated;
			Init(building.Definition, definitionService);
			UpdateColliderState(building.State);
			if (building.IsRotated)
			{
				base.transform.localEulerAngles = new global::UnityEngine.Vector3(0f, 90f, 0f);
				base.transform.localScale = new global::UnityEngine.Vector3(1f, 1f, -1f);
			}
			cachedAnimation = GetComponent<global::UnityEngine.Animation>();
			if (base.colliders.Length > 0)
			{
				minColliderY = base.colliders[0];
				global::UnityEngine.Collider[] array = base.colliders;
				foreach (global::UnityEngine.Collider collider in array)
				{
					if (collider.bounds.min.y < minColliderY.bounds.min.y)
					{
						minColliderY = collider;
					}
				}
			}
			if (base.objectRenderers.Length <= 0)
			{
				return;
			}
			maxRendererY = base.objectRenderers[0];
			global::UnityEngine.Renderer[] array2 = base.objectRenderers;
			foreach (global::UnityEngine.Renderer renderer in array2)
			{

                if (renderer.GetComponent<ParticleSystem>() != null)
                {
                    continue;
                }

                if (renderer.bounds.max.y > maxRendererY.bounds.max.y)
				{
					maxRendererY = renderer;
				}
				combinedRendererBounds.Encapsulate(renderer.bounds);
			}
		}

		public virtual void UpdateColliderState(global::Kampai.Game.BuildingState state)
		{
			base.IsInteractable = state != global::Kampai.Game.BuildingState.Disabled;
			UpdateColliders(base.IsInteractable);
		}

		private global::UnityEngine.Vector3 GetCenter(bool centerY)
		{
			global::UnityEngine.Vector3 position = base.transform.position;
			if (minColliderY != null)
			{
				return new global::UnityEngine.Vector3(minColliderY.bounds.center.x, (!centerY) ? minColliderY.bounds.max.y : minColliderY.bounds.center.y, minColliderY.bounds.center.z);
			}
			return new global::UnityEngine.Vector3(position.x, 0f, position.z);
		}

		protected virtual global::UnityEngine.Vector3 GetIndicatorPosition(bool centerY)
		{
			if (maxRendererY != null)
			{
				return new global::UnityEngine.Vector3(maxRendererY.bounds.center.x, (!centerY) ? maxRendererY.bounds.max.y : maxRendererY.bounds.center.y, maxRendererY.bounds.center.z);
			}
			return GetCenter(centerY);
		}

		protected virtual global::UnityEngine.Vector3 GetZoomCenterPosition()
		{
			global::UnityEngine.Vector3 position = base.transform.position;
			if (combinedRendererBounds.extents.sqrMagnitude > 0f)
			{
				return new global::UnityEngine.Vector3(position.x + combinedRendererBounds.center.x, combinedRendererBounds.center.y, position.z + combinedRendererBounds.center.z);
			}
			return GetCenter(true);
		}

		public void SetUpWifiListeners(global::strange.extensions.signal.impl.Signal openSignal, global::strange.extensions.signal.impl.Signal closeSignal)
		{
			networkOpenSignal = openSignal;
			netowrkCloseSignal = closeSignal;
			networkOpenSignal.AddListener(Hide);
			netowrkCloseSignal.AddListener(Show);
		}

		protected virtual void Hide()
		{
			base.gameObject.SetActive(false);
		}

		protected virtual void Show()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void Bounce()
		{
			if (cachedAnimation != null)
			{
				cachedAnimation.Play();
			}
		}

		public virtual void Cleanup()
		{
			if (networkOpenSignal != null)
			{
				networkOpenSignal.RemoveListener(Hide);
			}
			if (netowrkCloseSignal != null)
			{
				netowrkCloseSignal.RemoveListener(Show);
			}
		}

		public override bool CanFadeGFX()
		{
			return true;
		}

		public override bool CanFadeSFX()
		{
			return true;
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			if (IsRotated)
			{
				global::UnityEngine.Vector3 localScale = base.transform.localScale;
				if (localScale.z > 0f)
				{
					base.transform.localScale = new global::UnityEngine.Vector3(global::UnityEngine.Mathf.Abs(localScale.x), global::UnityEngine.Mathf.Abs(localScale.y), 0f - global::UnityEngine.Mathf.Abs(localScale.z));
				}
				global::UnityEngine.Vector3 localEulerAngles = base.transform.localEulerAngles;
				if (global::UnityEngine.Mathf.Abs(localEulerAngles.y - 90f) > 0.1f)
				{
					base.transform.localEulerAngles = new global::UnityEngine.Vector3(localEulerAngles.x, 90f, localEulerAngles.z);
				}
			}
		}
	}
}
