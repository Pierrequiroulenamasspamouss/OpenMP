namespace Kampai.UI.View
{
	public abstract class AbstractWayFinderView : global::Kampai.UI.View.WorldToGlassView, global::Kampai.UI.View.IWayFinderView, global::Kampai.UI.View.IWorldToGlassView
	{
		public global::strange.extensions.signal.impl.Signal RemoveWayFinderSignal = new global::strange.extensions.signal.impl.Signal();

		public global::strange.extensions.signal.impl.Signal UpdateWayFinderPrioritySignal = new global::strange.extensions.signal.impl.Signal();

		public global::strange.extensions.signal.impl.Signal SimulateClickSignal = new global::strange.extensions.signal.impl.Signal();

		private bool _snappable;

		private bool isTargetObjectVisible;

		private bool isFTUEWayFinder;

		protected global::Kampai.Game.IZoomCameraModel zoomCameraModel;

		protected global::Kampai.Game.WayFinderDefinition wayFinderDefinition;

		protected global::Kampai.Game.ITikiBarService tikiBarService;

		protected global::UnityEngine.Animator m_Animator;

		protected global::Kampai.UI.View.ButtonView m_GoToButton;

		protected global::Kampai.UI.View.KampaiImage m_CenterImage;

		private string m_CurrentIcon;

		protected global::UnityEngine.Transform m_NoRotationTransform;

		protected global::Kampai.Game.Prestige m_Prestige;

		public global::Kampai.Game.Prestige Prestige
		{
			get
			{
				return m_Prestige;
			}
		}

		public bool ClickedOnce { get; set; }

		public bool AvoidsHUD { get; set; }

		public virtual bool Snappable
		{
			get
			{
				return _snappable;
			}
			set
			{
				_snappable = value;
				AvoidsHUD = value;
				if (_snappable)
				{
					base.transform.SetAsLastSibling();
				}
			}
		}

		protected abstract string WayFinderDefaultIcon { get; }

		internal void Init(global::Kampai.UI.IPositionService positionService, global::strange.extensions.context.api.ICrossContextCapable gameContext, global::Kampai.Util.IKampaiLogger logger, global::Kampai.Game.IZoomCameraModel zoomCameraModel, global::Kampai.Game.ITikiBarService tikiBarService, global::Kampai.Game.IPlayerService playerService, global::Kampai.Main.ILocalizationService localizationService, global::Kampai.Game.IDefinitionService definitionService)
		{
			wayFinderDefinition = definitionService.Get<global::Kampai.Game.WayFinderDefinition>(1000008086);
			this.zoomCameraModel = zoomCameraModel;
			this.tikiBarService = tikiBarService;
			if (playerService.GetHighestFtueCompleted() < 9)
			{
				isFTUEWayFinder = true;
				Snappable = true;
			}
			Init(positionService, gameContext, logger, playerService, localizationService);
		}

		public override void OnDefinitionsHotSwap(global::Kampai.Game.IDefinitionService definitionService)
		{
			wayFinderDefinition = definitionService.Get<global::Kampai.Game.WayFinderDefinition>(1000008086);
			base.OnDefinitionsHotSwap(definitionService);
		}

		protected virtual void OnLoadWayFinderModal(global::Kampai.UI.View.WayFinderModal wayFinderModal)
		{
		}

		protected override void LoadModalData(global::Kampai.UI.View.WorldToGlassUIModal modal)
		{
			global::Kampai.UI.View.WayFinderModal wayFinderModal = modal as global::Kampai.UI.View.WayFinderModal;
			if (wayFinderModal == null)
			{
				logger.Error("Way Finder Modal doesn't exist!");
				return;
			}
			m_Animator = wayFinderModal.Animator;
			m_GoToButton = wayFinderModal.GoToButton;
			m_Prestige = wayFinderModal.Prestige;
			m_CenterImage = wayFinderModal.SpecificModel.CenterImage;
			m_NoRotationTransform = wayFinderModal.SpecificModel.NoRotationTransform;
			wayFinderModal.GenericModel.gameObject.SetActive(false);
			wayFinderModal.SpecificModel.gameObject.SetActive(true);
			OnLoadWayFinderModal(wayFinderModal);
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(m_trackedId);
			if (byInstanceId != null)
			{
				global::Kampai.Game.BuildingDefinition definition = byInstanceId.Definition;
				if (definition != null)
				{
					UIOffset = definition.QuestIconOffset;
				}
				if (definition.ID == 3015 && playerService.GetCountByDefinitionId(definition.ID) == 1)
				{
					isFTUEWayFinder = true;
					Snappable = true;
				}
			}
			UpdateIcon(WayFinderDefaultIcon);
			InitSubView();
		}

		protected override void OnHide()
		{
			base.OnHide();
			if (m_Animator != null)
			{
				m_Animator.enabled = false;
			}
		}

		protected override void OnShow()
		{
			base.OnShow();
			if (m_Animator != null)
			{
				m_Animator.enabled = true;
			}
		}

		protected virtual void InitSubView()
		{
		}

		internal virtual void Clear()
		{
		}

		public void SimulateClick()
		{
			if (!isHidden)
			{
				SimulateClickSignal.Dispatch();
			}
		}

		internal void UpdateIcon(string icon)
		{
			if (!(icon == m_CurrentIcon))
			{
				ClickedOnce = false;
				m_CurrentIcon = icon;
				m_CenterImage.maskSprite = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Sprite>(icon);
			}
		}

		public bool IsTargetObjectVisible()
		{
			return isTargetObjectVisible;
		}

		protected abstract bool OnCanUpdate();

		internal override bool CanUpdate()
		{
			if (targetObject == null && TrackedId != 1000008087)
			{
				RemoveWayFinderSignal.Dispatch();
				return false;
			}
			if (zoomCameraModel.ZoomInProgress)
			{
				return false;
			}
			if (!OnCanUpdate())
			{
				return false;
			}
			return true;
		}

		internal override void OnUpdatePosition(global::Kampai.UI.PositionData positionData)
		{
			if (Snappable || AvoidsHUD)
			{
				global::Kampai.UI.SnappablePositionData snappablePositionData = positionService.GetSnappablePositionData(positionData, global::Kampai.UI.ViewportBoundary.FULLSCREEN, true);
				positionData = new global::Kampai.UI.PositionData(snappablePositionData);
				if (Snappable)
				{
					m_transform.position = snappablePositionData.ClampedWorldPositionInUI;
				}
				else
				{
					m_transform.position = positionData.WorldPositionInUI;
				}
			}
			else
			{
				m_transform.position = positionData.WorldPositionInUI;
			}
			m_transform.localPosition = VectorUtils.ZeroZ(m_transform.localPosition);
			if (positionData.IsVisible)
			{
				OnVisible();
			}
			else
			{
				OnInvisible(positionData.ViewportDirectionFromCenter);
			}
			m_NoRotationTransform.rotation = global::UnityEngine.Quaternion.identity;
		}

		protected virtual void OnVisible()
		{
			if (!ClickedOnce && isFTUEWayFinder)
			{
				m_Animator.Play("Pulse");
			}
			else
			{
				m_Animator.Play("Idle");
			}
			m_transform.rotation = global::UnityEngine.Quaternion.identity;
			isTargetObjectVisible = true;
		}

		protected virtual void OnInvisible(global::UnityEngine.Vector3 direction)
		{
			isTargetObjectVisible = false;
			if (Snappable)
			{
				global::UnityEngine.Quaternion rotation = global::UnityEngine.Quaternion.LookRotation(direction, global::UnityEngine.Vector3.forward);
				rotation.x = 0f;
				rotation.y = 0f;
				m_transform.rotation = rotation;
			}
			else
			{
				m_transform.rotation = global::UnityEngine.Quaternion.identity;
			}
			m_Animator.Play("Idle");
		}
	}
}
