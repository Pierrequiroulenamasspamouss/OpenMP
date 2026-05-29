namespace Kampai.UI.View
{
	public class MysteryMinionTeaserSelectionView : global::Kampai.UI.View.PopupMenuView
	{
		public global::Kampai.UI.View.ButtonView confirmButton;

		[global::UnityEngine.Header("Minion Effects")]
		public global::Kampai.UI.View.KampaiImage Minion2DImage;

		public global::UnityEngine.ParticleSystem RevealVFX;

		public global::Kampai.UI.View.MinionSlotModal MinionSlot;

		[global::UnityEngine.Header("Reward 1 Panel")]
		public global::Kampai.UI.View.ButtonView choice1Button;

		public global::UnityEngine.Animator choice1PulseAnimator;

		public global::Kampai.UI.View.KampaiImage choice1_icon1;

		public global::UnityEngine.UI.Text choice1_icon1_amt;

		public global::Kampai.UI.View.KampaiImage choice1_icon2;

		public global::UnityEngine.UI.Text choice1_icon2_amt;

		[global::UnityEngine.Header("Reward 1 Selected")]
		public global::UnityEngine.GameObject choice1_SelectedActivePanel;

		[global::UnityEngine.Header("Reward 1 DeSelected")]
		public global::UnityEngine.GameObject choice1_ButtonDeselectedInitialColor;

		public global::UnityEngine.GameObject choice1_DeSelectedActivePanel;

		[global::UnityEngine.Header("Reward 2 Panel")]
		public global::Kampai.UI.View.ButtonView choice2Button;

		public global::UnityEngine.Animator choice2PulseAnimator;

		public global::Kampai.UI.View.KampaiImage choice2_icon1;

		public global::UnityEngine.UI.Text choice2_icon1_amt;

		public global::Kampai.UI.View.KampaiImage choice2_icon2;

		public global::UnityEngine.UI.Text choice2_icon2_amt;

		[global::UnityEngine.Header("Reward 2 Selected")]
		public global::UnityEngine.GameObject choice2_SelectedActivePanel;

		[global::UnityEngine.Header("Reward 2 DeSelected")]
		public global::UnityEngine.GameObject choice2_ButtonDeselectedInitialColor;

		public global::UnityEngine.GameObject choice2_DeSelectedActivePanel;

		private bool initialSelectionMade;

		private global::Kampai.UI.IFancyUIService fancyUIService;

		private global::Kampai.Main.PlayGlobalSoundFXSignal playAudioSignal;

		private global::Kampai.Game.View.DummyCharacterObject dummyCharacterObject;

		public void Initialize(global::Kampai.UI.IFancyUIService fancyService, global::Kampai.Main.PlayGlobalSoundFXSignal playSFXSignal)
		{
			Minion2DImage.gameObject.SetActive(true);
			base.Init();
			base.Open();
			fancyUIService = fancyService;
			playAudioSignal = playSFXSignal;
			confirmButton.gameObject.SetActive(false);
		}

		public void SetUpRewardIconDisplayable(global::Kampai.UI.View.KampaiImage icon, global::Kampai.Game.DisplayableDefinition def, global::UnityEngine.UI.Text amtText, uint amt)
		{
			icon.sprite = UIUtils.LoadSpriteFromPath(def.Image);
			icon.maskSprite = UIUtils.LoadSpriteFromPath(def.Mask);
			amtText.text = amt.ToString();
		}

		public void PlayerSelectedFirstReward(bool oneSelected)
		{
			if (!initialSelectionMade)
			{
				SetInitialSelection();
			}
			EnableSelectedButtonOnFirstReward(oneSelected);
		}

		internal void SetInitialSelection()
		{
			initialSelectionMade = true;
			choice1_ButtonDeselectedInitialColor.SetActive(false);
			choice2_ButtonDeselectedInitialColor.SetActive(false);
			DisableSelectPulse();
			confirmButton.gameObject.SetActive(true);
			confirmButton.GetComponent<global::UnityEngine.UI.Button>().interactable = true;
		}

		internal void PulseSelectButtons()
		{
			choice1Button.StartButtonPulse(true);
			choice2Button.StartButtonPulse(true);
		}

		internal void DisableSelectPulse()
		{
			choice1PulseAnimator.enabled = false;
			choice2PulseAnimator.enabled = false;
		}

		public void TriggerVFXReveal()
		{
			playAudioSignal.Dispatch("Play_captain_poofReveal_01");
			RevealVFX.Play();
		}

		public void SpawnMinion()
		{
			Minion2DImage.gameObject.SetActive(false);
			dummyCharacterObject = fancyUIService.CreateCharacter(global::Kampai.UI.DummyCharacterType.NamedCharacter, global::Kampai.UI.DummyCharacterAnimationState.SelectedHappy, MinionSlot.transform, MinionSlot.VillainScale, MinionSlot.VillainPositionOffset, 40014);
			playAudioSignal.Dispatch("Play_captainReveal_stinger_01");
		}

		internal void Release()
		{
			if (dummyCharacterObject != null && dummyCharacterObject.gameObject != null)
			{
				global::UnityEngine.Object.Destroy(dummyCharacterObject.gameObject);
				dummyCharacterObject = null;
			}
		}

		private void EnableSelectedButtonOnFirstReward(bool enableFirstReward)
		{
			choice1_SelectedActivePanel.SetActive(enableFirstReward);
			choice2_SelectedActivePanel.SetActive(!enableFirstReward);
			choice1_DeSelectedActivePanel.SetActive(!enableFirstReward);
			choice2_DeSelectedActivePanel.SetActive(enableFirstReward);
		}
	}
}
