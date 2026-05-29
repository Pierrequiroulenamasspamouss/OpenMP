namespace Kampai.Tools.AnimationToolKit
{
	public class LoadInterfaceCommand : global::strange.extensions.command.impl.Command
	{
		[Inject]
		public global::Kampai.Game.IDefinitionService DefinitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService PlayerService { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.AnimationToolkitModel Model { get; set; }

		[Inject]
		public global::UnityEngine.Camera Camera { get; set; }

		[Inject]
		public global::UnityEngine.Canvas Canvas { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.LoadToggleGroupSignal LoadToggleGroupSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.LoadToggleSignal LoadToggleSignal { get; set; }

		[Inject]
		public global::Kampai.Tools.AnimationToolKit.InitToggleSignal InitToggleSignal { get; set; }

		public override void Execute()
		{
			Unload();
			if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Uninitialized)
			{
				LoadPrefab("MinionModeButton");
				LoadPrefab("VillainModeButton");
				LoadPrefab("CharacterModeButton");
			}
			else
			{
				LoadPrefab("ToggleInterfaceButton");
			}
			if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Building)
			{
				LoadBuildingToggles();
				LoadPrefab("ToggleMeshButton");
				LoadPrefab("AddMinionButton");
				LoadPrefab("RemoveMinionButton");
				LoadPrefab("LoopAnimationButton");
				LoadPrefab("GagAnimationButton");
				LoadPrefab("WaitAnimationButton");
			}
			else if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.TikiBar)
			{
				LoadBuildingToggles();
				LoadPrefab("ToggleMeshButton");
				LoadPrefab("AddCharacterButton");
				LoadPrefab("RemoveCharacterButton");
				LoadPrefab("TikiBarCelebrateButton");
				LoadPrefab("TikiBarAttentionButton");
				LoadPrefab("TikiBarSpyGlassButton");
				LoadPrefab("TikiBarMixDrinkButton");
			}
			else if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Stage)
			{
				LoadBuildingToggles();
				LoadPrefab("ToggleMeshButton");
				LoadPrefab("AddCharacterButton");
				LoadPrefab("RemoveCharacterButton");
				LoadPrefab("StuartToggleIdleButton");
				LoadPrefab("StuartToggleAttentionButton");
				LoadPrefab("StuartPerformButton");
				LoadPrefab("StuartCelebrateButton");
			}
			else if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Minion)
			{
				Camera.transform.localPosition = new global::UnityEngine.Vector3(10f, 10f, -10f);
				LoadPrefab("AddMinionButton");
				LoadPrefab("RemoveMinionButton");
				LoadPrefab("ScrollBoxPanel");
			}
			else if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Villain)
			{
				LoadToggleGroupSignal.Dispatch();
				int num = 0;
				global::System.Collections.Generic.IList<global::Kampai.Game.VillainDefinition> all = DefinitionService.GetAll<global::Kampai.Game.VillainDefinition>();
				foreach (global::Kampai.Game.VillainDefinition item in all)
				{
					LoadToggleSignal.Dispatch(num++ == 0, item.ID, item.LocalizedKey);
				}
				LoadPrefab("AddVillainButton");
				LoadPrefab("RemoveVillainButton");
				LoadPrefab("IntroAnimationButton");
				LoadPrefab("BoatAnimationButton");
				LoadPrefab("CabanaAnimationButton");
				LoadPrefab("FarewellAnimationButton");
			}
			else if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Character)
			{
				LoadToggleGroupSignal.Dispatch();
				int num2 = 0;
				global::System.Collections.Generic.IList<global::Kampai.Game.NamedCharacterDefinition> all2 = DefinitionService.GetAll<global::Kampai.Game.NamedCharacterDefinition>();
				foreach (global::Kampai.Game.NamedCharacterDefinition item2 in all2)
				{
					if (item2.Type != global::Kampai.Game.NamedCharacterType.VILLAIN && item2.Type != global::Kampai.Game.NamedCharacterType.TSM)
					{
						LoadToggleSignal.Dispatch(num2++ == 0, item2.ID, item2.LocalizedKey);
					}
				}
				LoadPrefab("AddCharacterButton");
				LoadPrefab("RemoveCharacterButton");
				LoadPrefab("ScrollBoxPanel");
			}
			else if (Model.Mode == global::Kampai.Tools.AnimationToolKit.AnimationToolKitMode.Debris)
			{
				LoadBuildingToggles();
				LoadPrefab("ToggleMeshButton");
				LoadPrefab("AddMinionButton");
				LoadPrefab("RemoveMinionButton");
			}
		}

		private void LoadPrefab(string resourcePath)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>(resourcePath);
			global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
			global::UnityEngine.RectTransform rectTransform = gameObject.transform as global::UnityEngine.RectTransform;
			global::UnityEngine.RectTransform rectTransform2 = gameObject2.transform as global::UnityEngine.RectTransform;
			if (rectTransform2 != null)
			{
				rectTransform2.SetParent(Canvas.transform, false);
				if (rectTransform != null)
				{
					rectTransform2.anchorMin = rectTransform.anchorMin;
					rectTransform2.anchorMax = rectTransform.anchorMax;
					rectTransform2.pivot = rectTransform.pivot;
					rectTransform2.anchoredPosition = rectTransform.anchoredPosition;
					rectTransform2.sizeDelta = rectTransform.sizeDelta;
					rectTransform2.localScale = rectTransform.localScale;
					rectTransform2.localRotation = rectTransform.localRotation;
				}
			}
			else
			{
				gameObject2.transform.SetParent(Canvas.transform, false);
			}
		}

		private void Unload()
		{
			int num = 0;
			while (Canvas.transform.childCount > num)
			{
				global::UnityEngine.GameObject gameObject = Canvas.transform.GetChild(num).gameObject;
				if (gameObject.GetComponent<global::UnityEngine.UI.ToggleGroup>() != null)
				{
					num++;
				}
				else
				{
					global::UnityEngine.Object.DestroyImmediate(gameObject);
				}
			}
		}

		private void LoadBuildingToggles()
		{
			if (global::UnityEngine.Object.FindObjectsOfType<global::UnityEngine.UI.ToggleGroup>().Length > 0)
			{
				return;
			}
			LoadToggleGroupSignal.Dispatch();
			int num = 0;
			global::System.Collections.Generic.ICollection<global::Kampai.Game.Building> instancesByType = PlayerService.GetInstancesByType<global::Kampai.Game.Building>();
			foreach (global::Kampai.Game.Building item in instancesByType)
			{
				LoadToggleSignal.Dispatch(num++ == 0, item.ID, item.Definition.LocalizedKey);
			}
		}
	}
}
