namespace Kampai.UI.View
{
	public class XPBarFunMediator : global::strange.extensions.mediation.impl.EventMediator
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("XPBarFunMediator") as global::Kampai.Util.IKampaiLogger;

		private global::Kampai.Game.MinionParty minionParty;

		private int _currentLevel = -1;

		private int _currentLevelTotalPoints;

		private int _prePartyLevel;

		private global::Kampai.Game.EndMinionPartySignal endMinionPartySignal;

		private global::Kampai.Game.ConfirmStartNewMinionPartySignal confirmStartParty;

		private global::Kampai.Game.LevelFunTable levelFunList;

		private bool partyHasStarted;

		private global::System.Collections.IEnumerator m_partyStartDelay;

		[Inject]
		public global::Kampai.UI.View.XPBarFunView view { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetLevelSignal setLevelSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SetXPSignal setXPSignal { get; set; }

		[Inject]
		public global::Kampai.Main.PlayGlobalSoundFXSignal playSFXSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.XPFTUEHighlightSignal ftueHighlightSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.FireXPVFXSignal fireXpSignal { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localService { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject]
		public global::Kampai.UI.IPositionService positionService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.FTUELevelChangedSignal ftueLevelChangedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPartyService partyService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimedSocialEventService timedSocialEventService { get; set; }

		[Inject]
		public global::Kampai.UI.View.CheckIfShouldStartPartySignal checkIfShouldStartPartySignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.DisableLeisureRushButtonSignal disableLeisureRushButtonSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ExitVillainLairSignal exitVillainLairSignal { get; set; }

		[Inject]
		public global::Kampai.Game.VillainLairModel villainLairModel { get; set; }

		public override void OnRegister()
		{
			view.Init(positionService);
			setLevelSignal.AddListener(SetLevel);
			setXPSignal.AddListener(SetXP);
			ftueHighlightSignal.AddListener(ShowFTUEXP);
			fireXpSignal.AddListener(PlayXPVFX);
			ftueLevelChangedSignal.AddListener(OnFTUELevelChanged);
			checkIfShouldStartPartySignal.AddListener(CheckIfShouldStartParty);
			endMinionPartySignal = gameContext.injectionBinder.GetInstance<global::Kampai.Game.EndMinionPartySignal>();
			confirmStartParty = gameContext.injectionBinder.GetInstance<global::Kampai.Game.ConfirmStartNewMinionPartySignal>();
			endMinionPartySignal.AddListener(PartyOver);
			view.animateXP.AddListener(SetXP);
			minionParty = playerService.GetMinionPartyInstance();
			levelFunList = definitionService.Get<global::Kampai.Game.LevelFunTable>(1000009681);
			SetLevel();
			GlowOnStartup();
		}

		private void GlowOnStartup()
		{
			view.PlayInitialVFX();
		}

		public override void OnRemove()
		{
			setLevelSignal.RemoveListener(SetLevel);
			view.animateXP.RemoveListener(SetXP);
			setXPSignal.RemoveListener(SetXP);
			ftueHighlightSignal.RemoveListener(ShowFTUEXP);
			fireXpSignal.RemoveListener(PlayXPVFX);
			checkIfShouldStartPartySignal.RemoveListener(CheckIfShouldStartParty);
			ftueLevelChangedSignal.RemoveListener(OnFTUELevelChanged);
			endMinionPartySignal.RemoveListener(PartyOver);
		}

		private void OnFTUELevelChanged()
		{
			SetXP();
		}

		private void PartyOver(bool ignore)
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "Releasing xp bar to check for minion party being ready again (received callback from party)");
			playerService.UpdateMinionPartyPointValues();
			partyHasStarted = false;
			if (_prePartyLevel != _currentLevel)
			{
				view.ClearBar();
			}
			view.ClearAnnouncement();
			SetXP();
			m_partyStartDelay = null;
		}

		internal void SetLevel()
		{
			SetXP();
		}

		internal void SetXP()
		{
			if (!view.expTweenAudio)
			{
				playSFXSignal.Dispatch("Play_bar_scale_01");
				view.expTweenAudio = true;
			}
			int quantity = (int)playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID);
			if (levelFunList != null && levelFunList.partiesNeededList != null)
			{
				int targetCount = global::System.Math.Max(101, quantity + 1);
				if (levelFunList.partiesNeededList.Count < targetCount)
				{
					global::Kampai.Game.PartyUpDefinition template = null;
					if (levelFunList.partiesNeededList.Count > 0)
					{
						template = levelFunList.partiesNeededList[levelFunList.partiesNeededList.Count - 1];
					}
					while (levelFunList.partiesNeededList.Count < targetCount)
					{
						global::Kampai.Game.PartyUpDefinition newDef = new global::Kampai.Game.PartyUpDefinition();
						newDef.Multiplier = (template != null) ? template.Multiplier : 1f;
						newDef.PartyTransaction = (template != null) ? template.PartyTransaction : null;
						newDef.PointsNeeded = new global::System.Collections.Generic.List<int> { 7200 };
						newDef.ID = levelFunList.partiesNeededList.Count;
						levelFunList.partiesNeededList.Add(newDef);
					}
				}
			}
			if (_currentLevel != quantity)
			{
				_currentLevel = quantity;
				_currentLevelTotalPoints = partyService.GetCumulativePointsRequiredThisLevel(_currentLevel);
				view.SetLevel(levelFunList.partiesNeededList[quantity].PointsNeeded, quantity);
			}
			view.SetXP((uint)partyService.GetCumulativePointsEarnedThisLevel(quantity, minionParty.CurrentPartyIndex, (int)minionParty.CurrentPartyPoints), (uint)_currentLevelTotalPoints);
			if (minionParty.IsPartyReady)
			{
				CheckIfShouldStartParty();
			}
		}

		internal void ShowFTUEXP(bool show)
		{
			view.ShowFTUEXP(show);
		}

		private void PlayXPVFX()
		{
			view.PlayXPVFX();
		}

		private void CheckIfShouldStartParty()
		{
			if (minionParty.IsPartyReady && !partyHasStarted && !timedSocialEventService.isRewardCutscene())
			{
				BeginParty();
				return;
			}
			if (partyHasStarted)
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "In XPBarFunMediator.CheckIfShouldStartParty: blocking start of party because partyHasStarted = true.");
			}
			if (timedSocialEventService.isRewardCutscene())
			{
				logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "In XPBarFunMediator.CheckIfShouldStartParty: blocking start of party because because timedSocialEventService.isRewardCutscene() = true.");
			}
		}

		internal global::System.Collections.IEnumerator StartNewPartyWithDelay()
		{
			yield return new global::UnityEngine.WaitForSeconds(1.5f);
			if (!playerService.GetMinionPartyInstance().CharacterUnlocking)
			{
				ConfirmStartNewParty();
			}
		}

		private void ConfirmStartNewParty()
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Info, "Locking xp bar mediator from sending party signal: will release upon callback from party ending.  Level={0} and index={1} ", _currentLevel, minionParty.CurrentPartyIndex);
			confirmStartParty.Dispatch(true);
		}

		private void BeginParty()
		{
			partyHasStarted = true;
			disableLeisureRushButtonSignal.Dispatch();
			view.SetAnnouncementText(localService.GetString("FunbarPartyPrompt"));
			m_partyStartDelay = StartNewPartyWithDelay();
			_prePartyLevel = _currentLevel;
			if (villainLairModel.currentActiveLair == null)
			{
				StartCoroutine(m_partyStartDelay);
				return;
			}
			exitVillainLairSignal.Dispatch(new global::Kampai.Util.Boxed<global::System.Action>(delegate
			{
				StartCoroutine(m_partyStartDelay);
			}));
		}
	}
}
