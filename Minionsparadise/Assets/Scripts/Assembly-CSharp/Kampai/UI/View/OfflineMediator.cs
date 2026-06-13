namespace Kampai.UI.View
{
	public class OfflineMediator : global::strange.extensions.mediation.impl.Mediator
	{
		private global::System.Collections.Generic.List<global::UnityEngine.UI.Button> retryButtons = new global::System.Collections.Generic.List<global::UnityEngine.UI.Button>();
		private global::System.Collections.Generic.List<global::Kampai.UI.View.ButtonView> retryButtonViews = new global::System.Collections.Generic.List<global::Kampai.UI.View.ButtonView>();
		private global::System.Collections.Generic.List<global::Kampai.UI.View.ButtonView> playOfflineButtonViews = new global::System.Collections.Generic.List<global::Kampai.UI.View.ButtonView>();

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
			retryButtons.Clear();
			retryButtonViews.Clear();
			playOfflineButtonViews.Clear();

			global::UnityEngine.Transform[] transforms = view.GetComponentsInChildren<global::UnityEngine.Transform>(true);
			global::UnityEngine.GameObject cta1 = null;
			global::UnityEngine.GameObject cta2 = null;
			foreach (global::UnityEngine.Transform t in transforms)
			{
				if (t.name == "panel_CTA_1up") cta1 = t.gameObject;
				else if (t.name == "panel_CTA_2up") cta2 = t.gameObject;
			}
			if (cta1 != null && cta2 != null)
			{
				cta1.SetActive(false);
				cta2.SetActive(true);
			}

			foreach (global::Kampai.UI.View.ButtonView b in view.GetComponentsInChildren<global::Kampai.UI.View.ButtonView>(true))
			{
				bool isRetry = false;
				bool isPlayOffline = false;

				if (b.transform.parent != null)
				{
					string parentName = b.transform.parent.name;
					if (parentName == "panel_CTA_1up")
					{
						if (b.name == "btn_01") isRetry = true;
						else if (b.name == "btn_playOffline") isPlayOffline = true;
					}
					else if (parentName == "panel_CTA_2up")
					{
						if (b.name == "btn_01") isRetry = true;
						else if (b.name == "btn_02") isPlayOffline = true;
					}
				}

				if (isRetry)
				{
					b.ClickedSignal.AddListener(OnRetry);
					retryButtonViews.Add(b);
					global::UnityEngine.UI.Button uiBtn = b.GetComponent<global::UnityEngine.UI.Button>();
					if (uiBtn != null)
					{
						retryButtons.Add(uiBtn);
					}
					global::UnityEngine.UI.Text txt = b.GetComponentInChildren<global::UnityEngine.UI.Text>(true);
					if (txt != null)
					{
						txt.text = locService.GetString("OfflineRetry");
					}
				}
				else if (isPlayOffline)
				{
					b.ClickedSignal.AddListener(OnPlayOffline);
					playOfflineButtonViews.Add(b);
					global::UnityEngine.UI.Text txt = b.GetComponentInChildren<global::UnityEngine.UI.Text>(true);
					if (txt != null)
					{
						txt.text = locService.GetString("OfflinePlayOffline");
					}
				}
			}

			view.title.text = locService.GetString("OfflineTitle");
			view.description.text = locService.GetString("OfflineDescription");
			
			if (view.retryButtonText != null)
			{
				view.retryButtonText.text = locService.GetString("OfflineRetry");
			}
			if (view.playOfflineButtonText != null)
			{
				view.playOfflineButtonText.text = locService.GetString("OfflinePlayOffline");
			}

			view.OnMenuClose.AddListener(OnMenuClose);
			view.Init();
			view.Open();
			openSignal.Dispatch();
		}

		public override void OnRemove()
		{
			if (view != null)
			{
				foreach (global::Kampai.UI.View.ButtonView b in retryButtonViews)
				{
					if (b != null && b.ClickedSignal != null)
					{
						b.ClickedSignal.RemoveListener(OnRetry);
					}
				}
				foreach (global::Kampai.UI.View.ButtonView b in playOfflineButtonViews)
				{
					if (b != null && b.ClickedSignal != null)
					{
						b.ClickedSignal.RemoveListener(OnPlayOffline);
					}
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
			foreach (global::UnityEngine.UI.Button btn in retryButtons)
			{
				if (btn != null)
				{
					btn.interactable = false;
				}
			}
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
			foreach (global::UnityEngine.UI.Button btn in retryButtons)
			{
				if (btn != null)
				{
					btn.interactable = true;
				}
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
