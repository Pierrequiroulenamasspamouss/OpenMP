namespace Kampai.UI.View
{
	public class HindsightContentView : global::strange.extensions.mediation.impl.View
	{
		private string campaignUri;

		public global::Kampai.Main.HindsightCampaignDefinition definition { get; set; }

		public global::Kampai.UI.View.HideSkrimSignal hideSkrimSignal { get; set; }

		public global::Kampai.Main.HindsightContentDismissSignal dismissSignal { get; set; }

		public global::Kampai.UI.View.IGUIService guiService { get; set; }

		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		public void Open(global::UnityEngine.GameObject glassCanvas, global::Kampai.Main.HindsightCampaign campaign, string languageKey)
		{
			campaignUri = HindsightUtil.GetUri(campaign.Definition, languageKey);
			string contentCachePath = HindsightUtil.GetContentCachePath(campaign.Definition, languageKey);
			global::UnityEngine.Texture2D texture2D = LoadImage(contentCachePath);
			if (texture2D == null)
			{
				CloseContent();
				return;
			}
			global::Kampai.UI.View.KampaiClickableImage component = GetComponent<global::Kampai.UI.View.KampaiClickableImage>();
			component.material.SetTexture("_MainTex", texture2D);
			component.ClickedSignal.AddListener(OnClick);
			component.EnableClick(true);
			float referenceScale = 1f;
			global::UnityEngine.UI.CanvasScaler component2 = glassCanvas.GetComponent<global::UnityEngine.UI.CanvasScaler>();
			if (component2 != null)
			{
				float num = global::UnityEngine.Mathf.Lerp(component2.referenceResolution.x, component2.referenceResolution.y, component2.matchWidthOrHeight);
				float num2 = global::UnityEngine.Mathf.Lerp(global::UnityEngine.Screen.width, global::UnityEngine.Screen.height, component2.matchWidthOrHeight);
				referenceScale = num / num2;
			}
			ScaleImage(texture2D, component, referenceScale);
			component.SetMaterialDirty();
		}

		public void Close(global::Kampai.Main.HindsightCampaign.DismissType dismissType)
		{
			global::Kampai.Main.HindsightCampaign.Scope type = (global::Kampai.Main.HindsightCampaign.Scope)(int)global::System.Enum.Parse(typeof(global::Kampai.Main.HindsightCampaign.Scope), definition.Scope);
			dismissSignal.Dispatch(type, dismissType);
			telemetryService.Send_Telemetry_EVT_IN_APP_MESSAGE_DISPLAYED(definition.Scope, dismissType);
			CloseContent();
			global::Kampai.UI.View.KampaiClickableImage component = GetComponent<global::Kampai.UI.View.KampaiClickableImage>();
			global::UnityEngine.Object.Destroy(component.mainTexture);
			component.ClickedSignal.RemoveListener(OnClick);
		}

		private global::UnityEngine.Texture2D LoadImage(string filePath)
		{
#if !UNITY_WEBPLAYER
			if (global::System.IO.File.Exists(filePath))
			{
				global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D(1, 1);
				byte[] data = global::System.IO.File.ReadAllBytes(filePath);
				if (global::UnityEngine.ImageConversion.LoadImage(texture2D, data))
				{
					return texture2D;
				}
			}
#endif
			return null;
		}

		private void ScaleImage(global::UnityEngine.Texture2D texture, global::UnityEngine.UI.Image image, float referenceScale)
		{
			int width = texture.width;
			int height = texture.height;
			int num = global::UnityEngine.Screen.width - 100;
			int num2 = global::UnityEngine.Screen.height - 100;
			if (num - width >= 0 && num2 - height >= 0)
			{
				image.rectTransform.sizeDelta = new global::UnityEngine.Vector2((float)width * referenceScale, (float)height * referenceScale);
				return;
			}
			float num3 = (float)num / (float)width;
			float num4 = (float)num2 / (float)height;
			float num5 = ((!(num3 <= num4)) ? num4 : num3);
			image.rectTransform.sizeDelta = new global::UnityEngine.Vector2((float)width * referenceScale * num5, (float)height * referenceScale * num5);
		}

		private void OnClick()
		{
			Close(global::Kampai.Main.HindsightCampaign.DismissType.ACCEPTED);
			global::UnityEngine.Application.OpenURL(campaignUri);
		}

		private void CloseContent()
		{
			hideSkrimSignal.Dispatch("HindsightContentSkrim");
			global::Kampai.UI.View.IGUICommand command = guiService.BuildCommand(global::Kampai.UI.View.GUIOperation.Unload, "HindsightContentView");
			guiService.Execute(command);
		}
	}
}
