using UnityEngine.InputSystem;

namespace Kampai.UI.View
{
	public class SkrimMediator : global::Kampai.UI.View.KampaiMediator, global::Kampai.Util.KampaiDisposable
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("SkrimMediator") as global::Kampai.Util.IKampaiLogger;

		private bool triedToCloseTheNextFrame;

		private float fadeDuration;

		private bool useFade;

		private global::UnityEngine.GameObject partyScreenVFX;

		private global::Kampai.UI.View.SkrimCallback externalCallback;

		[Inject]
		public global::Kampai.UI.View.SkrimView view { get; set; }

		[Inject]
		public global::Kampai.UI.View.UIModel model { get; set; }

		[Inject]
		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideSkrimSignal hideSkrim { get; set; }

		[Inject]
		public global::Kampai.UI.View.EnableSkrimButtonSignal enableSkrimSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.HideItemPopupSignal hideGenericPopupsSignal { get; set; }

		[Inject]
		public global::Kampai.Common.PickControllerModel PickControllerModel { get; set; }

		[Inject]
		public global::Kampai.UI.View.OnClickSkrimSignal onClickSkrimSignal { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.Util.IInvokerService invoker { get; set; }

		[Inject]
		public global::Kampai.UI.View.MoveSkrimTopLayerSignal moveSkrimTopLayerSignal { get; set; }

		public override void OnRegister()
		{
			PickControllerModel.IncreaseSkrimCounter();
			enableSkrimSignal.AddListener(EnableSkrimButton);
			moveSkrimTopLayerSignal.AddListener(OnMoveSkirmToTopLevelCallback);
		}

		public override void OnRemove()
		{
			view.ClickButton.ClickedSignal.RemoveListener(Close);
			hideSkrim.RemoveListener(HideSkrim);
			enableSkrimSignal.RemoveListener(EnableSkrimButton);
			moveSkrimTopLayerSignal.RemoveListener(OnMoveSkirmToTopLevelCallback);
		}

		public override void Initialize(global::Kampai.UI.View.GUIArguments args)
		{
			float alpha = view.DarkSkrimImage.color.a;
			bool flag = args.Get<bool>();
			global::Kampai.UI.View.SkrimBehavior skrimBehavior = args.Get<global::Kampai.UI.View.SkrimBehavior>();
			fadeDuration = 0.13f;
			if (skrimBehavior == global::Kampai.UI.View.SkrimBehavior.partyEffectsAndFade)
			{
				useFade = true;
				if (partyScreenVFX == null)
				{
					global::UnityEngine.GameObject original = global::Kampai.Util.KampaiResources.Load("cmp_StartPartySkrimEffects") as global::UnityEngine.GameObject;
					global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original);
					gameObject.transform.SetParent(base.gameObject.transform, false);
					gameObject.transform.SetSiblingIndex(view.DarkSkrimImage.transform.GetSiblingIndex() + 1);
					partyScreenVFX = gameObject;
				}
			}
			float num = global::UnityEngine.Mathf.Clamp(args.Get<float>(), 0f, 1f);
			if (num > 0f)
			{
				alpha = num;
			}
			hideSkrim.AddListener(HideSkrim);
			enableSkrimSignal.AddListener(EnableSkrimButton);
			view.Init(alpha, useFade);
			global::Kampai.UI.View.SkrimCallback skrimCallback = args.Get<global::Kampai.UI.View.SkrimCallback>();
			if (skrimCallback != null)
			{
				externalCallback = skrimCallback;
				view.ClickButton.ClickedSignal.AddListener(CallbackDelegate);
			}
			else
			{
				view.ClickButton.ClickedSignal.AddListener(Close);
			}
			view.ClickButton.gameObject.SetActive(true);
			view.SetDarkSkrimActive(flag, fadeDuration);
		}

		private void CallbackDelegate()
		{
			PickControllerModel.DecreaseSkrimCounter();
			externalCallback.Callback.Dispatch(this);
		}

		private void OnMoveSkirmToTopLevelCallback(string skrimName)
		{
			if (guiLabel.Equals(skrimName))
			{
				view.transform.SetAsFirstSibling();
			}
		}

		private void EnableSkrimButton(bool enable)
		{
			view.EnableSkrimButton(enable);
		}

		private void Close()
		{
			if (model.PopupAnimationIsPlaying)
			{
				return;
			}
			if (view.genericPopupSkrim)
			{
				hideGenericPopupsSignal.Dispatch();
				HideSkrim(guiLabel);
			}
			else if (model.UIOpen && !model.DisableBack)
			{
				onClickSkrimSignal.Dispatch();
				global::System.Action action = model.RemoveTopUI();
				if (!(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame))
				{
					if (view.singleSkrimClose)
					{
						action();
						return;
					}
					while (action != null)
					{
						action();
						action = model.RemoveTopUI();
					}
				}
				else if (action != null)
				{
					action();
				}
				else
				{
					HideSkrim(guiLabel);
				}
			}
			else if (!model.DisableBack)
			{
				global::System.Action action2 = model.RemoveTopUI();
				if (action2 != null)
				{
					action2();
				}
				HideSkrim(guiLabel);
			}
			else if (!triedToCloseTheNextFrame)
			{
				logger.Debug("Postponing skrim close with empty ui stack");
				triedToCloseTheNextFrame = true;
				invoker.Add(Close);
			}
			else
			{
				logger.Debug("Closing skrim with empty ui stack");
				HideSkrim(guiLabel);
			}
		}

		private void HideSkrim(string skrimName)
		{
			if (guiLabel == null || guiLabel.Equals(skrimName))
			{
				if (useFade)
				{
					view.FadeDarkSkrim(0f, fadeDuration);
				}
				StartCoroutine(DelayFinishHidingSkrim(fadeDuration));
			}
		}

		private global::System.Collections.IEnumerator DelayFinishHidingSkrim(float duration)
		{
			yield return new global::UnityEngine.WaitForSeconds(duration);
			FinishHidingSkrim();
		}

		private void FinishHidingSkrim()
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.ShowHiddenBuildingsSignal>().Dispatch();
			global::Kampai.UI.View.IGUICommand command = guiService.BuildCommand(global::Kampai.UI.View.GUIOperation.Unload, "Skrim", guiLabel);
			guiService.Execute(command);
			PickControllerModel.DecreaseSkrimCounter();
		}

		public void KDispose()
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
