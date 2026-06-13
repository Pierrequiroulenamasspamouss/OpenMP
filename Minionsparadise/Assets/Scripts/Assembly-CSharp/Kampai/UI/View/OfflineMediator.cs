namespace Kampai.UI.View
{
	public class OfflineMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private global::UnityEngine.UI.Button button;

		[Inject]
		public global::Kampai.UI.View.OfflineView view { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService locService { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkModel networkModel { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowOfflinePopupSignal showOfflinePopupSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ResumeNetworkOperationSignal resumeNetworkOperationSignal { get; set; }

		[Inject]
		public global::Kampai.Game.NetworkLostOpenSignal openSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.TransitionToOfflineModeSignal transitionToOfflineModeSignal { get; set; }

		[Inject]
		public global::Kampai.Game.NetworkLostCloseSignal closeSignal { get; set; }

		public override void OnRegister()
		{
			view.retryButton.ClickedSignal.AddListener(OnRetry);
			
			// Resolve the playOfflineButton instance to ensure we are listening to the correct one
			foreach (global::Kampai.UI.View.ButtonView b in view.GetComponentsInChildren<global::Kampai.UI.View.ButtonView>(true))
			{
				if (b.name == "btn_playOffline")
				{
					view.playOfflineButton = b;
					break;
				}
			}

			view.title.text = locService.GetString("OfflineTitle");
			view.description.text = locService.GetString("OfflineDescription");
			view.retryButtonText.text = locService.GetString("OfflineRetry");
			
			if (view.playOfflineButton != null)
			{
				view.playOfflineButton.ClickedSignal.AddListener(OnPlayOffline);
				view.playOfflineButtonText.text = locService.GetString("OfflinePlayOffline");

				// Disable the Animator on btn_playOffline to prevent Unity 6's WriteDefaultValues
				// from resetting its anchoredPosition to btn_01's defaults (both share the same
				// AnimatorController asm_buttonClick_Tertiary which continuously resets position).
				global::UnityEngine.Animator playOfflineAnimator = view.playOfflineButton.GetComponent<global::UnityEngine.Animator>();
				if (playOfflineAnimator != null)
				{
					playOfflineAnimator.enabled = false;
				}
			}
			
			view.OnMenuClose.AddListener(OnMenuClose);
			view.Init();
			view.Open();
			button = view.retryButton.GetComponent<global::UnityEngine.UI.Button>();
			openSignal.Dispatch();

			// Force correct button layout after the popup opens and animators initialize.
			StartCoroutine(ForceButtonLayout());
		}

		private global::System.Collections.IEnumerator ForceButtonLayout()
		{
			// Wait one frame so the popup's Open animation and button animators have initialized
			yield return null;

			global::UnityEngine.RectTransform retryRT = view.retryButton.transform as global::UnityEngine.RectTransform;
			if (retryRT != null)
			{
				// btn_01: stretch to fill parent panel_CTA_1up
				retryRT.anchorMin = new global::UnityEngine.Vector2(0f, 0f);
				retryRT.anchorMax = new global::UnityEngine.Vector2(1f, 1f);
				retryRT.anchoredPosition = new global::UnityEngine.Vector2(0f, -1.95f);
				retryRT.sizeDelta = new global::UnityEngine.Vector2(0f, -3.9f);
			}

			if (view.playOfflineButton != null)
			{
				global::UnityEngine.RectTransform offlineRT = view.playOfflineButton.transform as global::UnityEngine.RectTransform;
				if (offlineRT != null)
				{
					// btn_playOffline: stretch to fill parent, offset 88.2 pixels below btn_01
					offlineRT.anchorMin = new global::UnityEngine.Vector2(0f, 0f);
					offlineRT.anchorMax = new global::UnityEngine.Vector2(1f, 1f);
					offlineRT.anchoredPosition = new global::UnityEngine.Vector2(0f, -88.2f);
					offlineRT.sizeDelta = new global::UnityEngine.Vector2(0f, -3.9f);
				}
			}
		}

		public override void OnRemove()
		{
			if (view != null)
			{
				if (view.retryButton != null && view.retryButton.ClickedSignal != null)
				{
					view.retryButton.ClickedSignal.RemoveListener(OnRetry);
				}
				if (view.playOfflineButton != null && view.playOfflineButton.ClickedSignal != null)
				{
					view.playOfflineButton.ClickedSignal.RemoveListener(OnPlayOffline);
				}
				view.OnMenuClose.RemoveListener(OnMenuClose);
			}
			if (closeSignal != null)
			{
				closeSignal.Dispatch();
			}
		}

		private void OnRetry()
		{
			button.interactable = false;
			StartCoroutine(WaitForRetry());
			networkModel.isConnectionLost = !global::Kampai.Util.NetworkUtil.IsConnected();
			if (!networkModel.isConnectionLost)
			{
				Close();
				resumeNetworkOperationSignal.Dispatch();
			}
		}

		private void OnPlayOffline()
		{
			transitionToOfflineModeSignal.Dispatch();
		}

		private global::System.Collections.IEnumerator WaitForRetry()
		{
			yield return new global::UnityEngine.WaitForSeconds(2f);
			if (view.retryButton != null)
			{
				button.interactable = true;
			}
		}

		private void OnMenuClose()
		{
			if (!networkModel.isConnectionLost)
			{
				showOfflinePopupSignal.Dispatch(false);
			}
			else
			{
				view.Open();
			}
		}

		private void Close()
		{
			view.Close();
		}
	}
}
