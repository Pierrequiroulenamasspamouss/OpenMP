namespace Kampai.Util
{
	public class DebugConsoleController
	{
		private readonly global::System.Text.StringBuilder outBuilder = new global::System.Text.StringBuilder();

		private readonly global::System.Collections.Generic.Dictionary<string, global::Kampai.Util.Tuple<global::Kampai.Util.DebugCommandAttribute, global::Kampai.Util.DebugCommand>> commands = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Util.Tuple<global::Kampai.Util.DebugCommandAttribute, global::Kampai.Util.DebugCommand>>();

		private readonly global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("DebugConsoleController") as global::Kampai.Util.IKampaiLogger;

		private readonly global::Kampai.Game.QuestScriptInstance consoleQuestScript = new global::Kampai.Game.QuestScriptInstance();

		private global::Kampai.Game.View.ZoomView zoomView;

		private global::UnityEngine.Camera cameraComponent;

		private int uniqueID = 90000;

		private global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> localSocialEvents;

		private global::System.Collections.Generic.IList<global::Kampai.Game.SocialEventInvitation> localInvitations;

		public readonly global::strange.extensions.signal.impl.Signal CloseConsoleSignal = new global::strange.extensions.signal.impl.Signal();

		public readonly global::strange.extensions.signal.impl.Signal FlushSignal = new global::strange.extensions.signal.impl.Signal();

		public readonly global::strange.extensions.signal.impl.Signal EnableQuestDebugSignal = new global::strange.extensions.signal.impl.Signal();

		public readonly global::strange.extensions.signal.impl.Signal ToggleRightClickSignal = new global::strange.extensions.signal.impl.Signal();

		private static bool showTransparent = true;

		private global::System.Collections.Generic.List<global::UnityEngine.GameObject> HiddenTransparentObjects;

		private global::System.Collections.Generic.Dictionary<string, global::UnityEngine.GameObject> hiddenGameObjectsMap;

		private global::Kampai.UI.View.HUDView hudView;

		[Inject]
		public global::Kampai.Game.IQuestService questService { get; set; }

		[Inject]
		public global::Kampai.Game.IQuestScriptService questScriptService { get; set; }

		[Inject]
		public global::Kampai.Common.IVideoService videoService { get; set; }

		[Inject]
		public global::Kampai.Util.IMinionBuilder minionBuilder { get; set; }

		[Inject]
		public global::Kampai.Common.LogClientMetricsSignal metricsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ScheduleNotificationSignal notificationSignal { get; set; }

		[Inject]
		public global::Kampai.Common.ITelemetryService telemetryService { get; set; }

		[Inject(global::Kampai.UI.View.UIElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable uiContext { get; set; }

		[Inject(global::Kampai.Game.GameElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable gameContext { get; set; }

		[Inject(global::Kampai.Main.MainElement.CONTEXT)]
		public global::strange.extensions.context.api.ICrossContextCapable mainContext { get; set; }

		[Inject]
		public ILocalPersistanceService localPersistService { get; set; }

		[Inject]
		public IEncryptionService encryptionService { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configurationsService { get; set; }

		[Inject(global::Kampai.Game.SocialServices.FACEBOOK)]
		public global::Kampai.Game.ISocialService facebookService { get; set; }

		[Inject(global::Kampai.Game.SocialServices.GAMECENTER)]
		public global::Kampai.Game.ISocialService gamecenterService { get; set; }

		[Inject(global::Kampai.Game.SocialServices.GOOGLEPLAY)]
		public global::Kampai.Game.ISocialService googlePlayService { get; set; }

		[Inject(global::Kampai.Main.MainElement.CAMERA)]
		public global::UnityEngine.GameObject cameraGO { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject]
		public global::Kampai.Game.IMarketplaceService marketplaceService { get; set; }

		[Inject]
		public global::Kampai.Game.PurchaseLandExpansionSignal purchaseLandExpansionSignal { get; set; }

		[Inject]
		public global::Kampai.Game.EndSaleSignal endSaleSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ILandExpansionService landExpansionService { get; set; }

		[Inject]
		public global::Kampai.Game.ILandExpansionConfigService landExpansionConfigService { get; set; }

		[Inject]
		public global::Kampai.Game.IPrestigeService prestigeService { get; set; }

		[Inject]
		public global::Kampai.Game.LoadAnimatorStateInfoSignal loadAnimatorStateInfoSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UnloadAnimatorStateInfoSignal unloadAnimatorStateInfoSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ABTestSignal ABTestSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IConfigurationsService configService { get; set; }

		[Inject]
		public global::Kampai.Game.SavePlayerSignal savePlayerSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SocialLoginSignal socialLoginSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SocialInitSignal socialInitSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SocialLogoutSignal socialLogoutSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IRoutineRunner routineRunner { get; set; }

		[Inject(global::Kampai.Game.QuestRunnerLanguage.Lua)]
		public global::Kampai.Game.IQuestScriptRunner luaRunner { get; set; }

		[Inject]
		public global::Kampai.Game.IDLCService dlcService { get; set; }

		[Inject]
		public global::Kampai.Game.UnlinkAccountSignal unlinkAccountSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UpdatePlayerDLCTierSignal playerDLCTierSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.MignetteGameModel mignetteGameModel { get; set; }

		[Inject]
		public global::Kampai.Game.MignetteCollectionService mignetteCollectionService { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingCooldownCompleteSignal cooldownCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.Game.BuildingCooldownUpdateViewSignal cooldownUpdateViewSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.SpawnMignetteDooberSignal spawnDooberSignal { get; set; }

		[Inject]
		public global::Kampai.Game.Mignette.ChangeMignetteScoreSignal changeScoreSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ICurrencyService currencyService { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowClientUpgradeDialogSignal showClientUpgradeDialogSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.ShowForcedClientUpgradeScreenSignal showForcedClientUpgradeScreenSignal { get; set; }

		[Inject]
		public global::Kampai.Game.CompositeBuildingPieceAddedSignal compositeBuildingPieceAddedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimedSocialEventService timedSocialEventService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeEventService timeEventService { get; set; }

		[Inject]
		public global::Kampai.Game.SocialOrderBoardCompleteSignal socialOrderBoardCompleteSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.CreateWayFinderSignal createWayFinderSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RemoveWayFinderSignal removeWayFinderSignal { get; set; }

		[Inject]
		public global::Kampai.Common.RepairBuildingSignal repairBuildingSignal { get; set; }

		[Inject]
		public global::Kampai.Game.SetupPushNotificationsSignal setupPushNotificationsSignal { get; set; }

		[Inject]
		public global::Kampai.Game.DebugKeyHitSignal debugKeyHitSignal { get; set; }

		[Inject(global::Kampai.Main.MainElement.UI_GLASSCANVAS)]
		public global::UnityEngine.GameObject glassCanvas { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkModel networkModel { get; set; }

		[Inject]
		public global::Kampai.Common.NetworkConnectionLostSignal networkConnectionLostSignal { get; set; }

		[Inject]
		public global::Kampai.Game.ReconcileSalesSignal reconcileSalesSignal { get; set; }

		[Inject]
		public global::Kampai.UI.View.RushDialogConfirmationSignal dialogConfirmedSignal { get; set; }

		[Inject]
		public global::Kampai.Game.OpenBuildingMenuSignal openBuildingMenuSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IAchievementService achievementService { get; set; }

		[Inject]
		public global::Kampai.UI.View.AddFPSCounterSignal addFPSCounterSignal { get; set; }

		[Inject]
		public global::Kampai.Game.PurchaseLandExpansionSignal purchaseSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IMIBService mibService { get; set; }

		[Inject]
		public global::Kampai.Game.IOrderBoardService orderBoardService { get; set; }

		[Inject]
		public global::Kampai.Game.AwardLevelSignal awardLevelSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IPrestigeService characterService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerDurationService playerDurationService { get; set; }

		[Inject]
		public global::Kampai.UI.View.OpenUpSellModalSignal openUpSellModalSignal { get; set; }

		[Inject]
		public global::Kampai.Game.UnlockVillainLairSignal unlockVillainLairSignal { get; set; }

		[Inject]
		public global::Kampai.Util.IPartyFavorAnimationService partyFavorAnimationService { get; set; }

		[Inject]
		public global::Kampai.Game.IMasterPlanService masterPlanService { get; set; }

		[Inject]
		public global::Kampai.Game.IMasterPlanQuestService masterPlanQuestService { get; set; }

		[Inject]
		public global::Kampai.Main.ILocalizationService localizationService { get; set; }



		[Inject]
		public global::Kampai.Game.IRewardedAdService rewardedAdService { get; set; }

		[Inject]
		public global::Kampai.Game.ResetRewardedAdLimitSignal resetRewardedAdLimitSignal { get; set; }

		[Inject]
		public global::Kampai.Main.GameLoadedModel gameLoadedModel { get; set; }

		public DebugConsoleController()
		{
			try
			{
				global::System.Type typeFromHandle = typeof(global::Kampai.Util.DebugConsoleController);
				global::System.Reflection.MethodInfo[] methods = typeFromHandle.GetMethods(global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.Public);
				int i = 0;
				for (int num = methods.Length; i < num; i++)
				{
					global::System.Reflection.MethodInfo methodInfo = methods[i];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(global::Kampai.Util.DebugCommandAttribute), false);
					if (customAttributes.Length < 1)
					{
						continue;
					}
					global::Kampai.Util.DebugCommand second = null;
					try
					{
						second = global::System.Delegate.CreateDelegate(typeof(global::Kampai.Util.DebugCommand), methodInfo) as global::Kampai.Util.DebugCommand;
					}
					catch (global::System.ArgumentException ex)
					{
						outBuilder.AppendFormat("Failed to grab command method {0}: {1}", methodInfo.Name, ex.Message);
					}
					int j = 0;
					for (int num2 = customAttributes.Length; j < num2; j++)
					{
						global::Kampai.Util.DebugCommandAttribute debugCommandAttribute = customAttributes[j] as global::Kampai.Util.DebugCommandAttribute;
						string text = debugCommandAttribute.Name;
						if (text == null)
						{
							text = methodInfo.Name.ToLower();
						}
						commands.Add(text, new global::Kampai.Util.Tuple<global::Kampai.Util.DebugCommandAttribute, global::Kampai.Util.DebugCommand>(debugCommandAttribute, second));
					}
				}
			}
			catch (global::System.Exception ex2)
			{
				logger.Error("{0}, {1}, {2}", ex2.ToString(), ex2.Message, ex2.StackTrace);
				throw;
			}
		}

		[PostConstruct]
		public void Init()
		{
			zoomView = cameraGO.GetComponent<global::Kampai.Game.View.ZoomView>();
			cameraComponent = cameraGO.GetComponent<global::UnityEngine.Camera>();
			debugKeyHitSignal.AddListener(DebugKeyHit);
		}

		public global::Kampai.Util.DebugCommandError GetCommand(string[] args, out global::Kampai.Util.DebugCommand command)
		{
			int i = 0;
			for (int num = args.Length; i < num; i++)
			{
				string key = string.Join(" ", args, 0, i + 1);
				global::Kampai.Util.Tuple<global::Kampai.Util.DebugCommandAttribute, global::Kampai.Util.DebugCommand> value;
				commands.TryGetValue(key, out value);
				if (value != null)
				{
					if (value.Item1.RequiresAllArgs && args.Length - i - 1 < value.Item1.Args.Length)
					{
						command = null;
						return global::Kampai.Util.DebugCommandError.NotEnoughArguments;
					}
					command = value.Item2;
					return global::Kampai.Util.DebugCommandError.NoError;
				}
			}
			command = null;
			return global::Kampai.Util.DebugCommandError.NotFound;
		}

		public string GetOutput()
		{
			string result = outBuilder.ToString();
			outBuilder.Length = 0;
			return result;
		}

		[global::Kampai.Util.DebugCommand]
		public void unlockLairPortal(string[] args)
		{
			global::Kampai.Game.VillainLairEntranceBuilding byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.VillainLairEntranceBuilding>(374);
			if (byInstanceId.State == global::Kampai.Game.BuildingState.Inaccessible)
			{
				byInstanceId.SetState(global::Kampai.Game.BuildingState.Broken);
				repairBuildingSignal.Dispatch(byInstanceId);
				outBuilder.AppendLine("Villain Lair Portal has been repaired, use debug command 'unlockLairPortal' again to simulate having the neccessary upgraded minions.");
			}
			else if (!byInstanceId.IsUnlocked)
			{
				unlockVillainLairSignal.Dispatch(byInstanceId, 3137);
				outBuilder.AppendLine("Villain Lair Portal should now be unlocked.");
			}
			else
			{
				outBuilder.AppendLine("could not unlock the villain lair portal...building state is not inaccessible or already unlocked through debug.");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void fixBuildings(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Building> buildings = playerService.GetInstancesByType<global::Kampai.Game.Building>();
			int repairCount = 0;
			foreach (global::Kampai.Game.Building building in buildings)
			{
				if (building.State == global::Kampai.Game.BuildingState.Broken || building.State == global::Kampai.Game.BuildingState.Inaccessible)
				{
					building.SetState(global::Kampai.Game.BuildingState.Idle);
					repairBuildingSignal.Dispatch(building);
					repairCount++;
				}
			}
			outBuilder.AppendFormat("Fixed {0} broken or inaccessible buildings.", repairCount);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "command_name" })]
		[global::Kampai.Util.DebugCommand(Name = "?")]
		public void Help(string[] args)
		{
			outBuilder.AppendLine("commands:");
			string text = string.Join(" ", args, 1, args.Length - 1);
			global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<string>> dictionary = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<string>>();
			foreach (string key in commands.Keys)
			{
				if (key.StartsWith(text))
				{
					string commandName;
					string subcommandName;
					SplitCommandKey(key, text, out commandName, out subcommandName);
					global::System.Collections.Generic.List<string> value;
					dictionary.TryGetValue(commandName, out value);
					if (value == null)
					{
						value = new global::System.Collections.Generic.List<string>();
						dictionary.Add(commandName, value);
					}
					if (subcommandName != null)
					{
						value.Add(subcommandName);
					}
				}
			}
			int count = dictionary.Count;
			int num = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<string, global::System.Collections.Generic.List<string>> item in global::System.Linq.Enumerable.OrderBy(dictionary, (global::System.Collections.Generic.KeyValuePair<string, global::System.Collections.Generic.List<string>> key) => key.Key))
			{
				outBuilder.Append(item.Key);
				if (item.Value.Count < 1)
				{
					OutputArguments(commands[item.Key].Item1.Args);
				}
				else
				{
					OutputSubcommands(item.Value);
				}
				if (num++ < count - 1)
				{
					outBuilder.Append(" | ");
				}
			}
			outBuilder.AppendLine("\n Use help {{command_name}} to see more details of a command.");
		}

		[global::Kampai.Util.DebugCommand]
		public void FTUELevel(string[] args)
		{
			int highestFtueCompleted = playerService.GetHighestFtueCompleted();
			outBuilder.Append("FtueLevel: " + highestFtueCompleted + " - " + (global::Kampai.Game.FtueLevel)highestFtueCompleted);
		}

		[global::Kampai.Util.DebugCommand]
		public void ResetQuest(string[] args)
		{
			global::Kampai.Game.IQuestController targetQuest = null;
			bool isPrevious = false;

			var activeQuests = new global::System.Collections.Generic.List<global::Kampai.Game.IQuestController>();
			foreach (var kvp in questService.GetQuestMap())
			{
				if (kvp.Value.State != global::Kampai.Game.QuestState.Complete && kvp.Value.State != global::Kampai.Game.QuestState.Notstarted)
				{
					activeQuests.Add(kvp.Value);
				}
			}

			activeQuests.Sort((a, b) => b.Definition.NarrativeOrder.CompareTo(a.Definition.NarrativeOrder));

			if (activeQuests.Count > 0)
			{
				targetQuest = activeQuests[0];
				if (targetQuest.State == global::Kampai.Game.QuestState.RunningStartScript)
				{
					targetQuest = null; 
				}
			}

			if (targetQuest == null)
			{
				var completedQuests = new global::System.Collections.Generic.List<global::Kampai.Game.IQuestController>();
				foreach (var kvp in questService.GetQuestMap())
				{
					if (kvp.Value.State == global::Kampai.Game.QuestState.Complete)
					{
						completedQuests.Add(kvp.Value);
					}
				}
				completedQuests.Sort((a, b) => b.Definition.NarrativeOrder.CompareTo(a.Definition.NarrativeOrder));

				if (completedQuests.Count > 0)
				{
					targetQuest = completedQuests[0];
					isPrevious = true;
				}
			}

			if (targetQuest == null)
			{
				outBuilder.AppendLine("Could not find any quest to reset.");
				return;
			}

			if (targetQuest.Definition.QuestSteps != null)
			{
				for (int i = 0; i < targetQuest.Definition.QuestSteps.Count; i++)
				{
					var stepDef = targetQuest.Definition.QuestSteps[i];
					if (stepDef.Type == global::Kampai.Game.QuestStepType.StageRepair ||
						stepDef.Type == global::Kampai.Game.QuestStepType.CabanaRepair ||
						stepDef.Type == global::Kampai.Game.QuestStepType.WelcomeHutRepair ||
						stepDef.Type == global::Kampai.Game.QuestStepType.FountainRepair ||
						stepDef.Type == global::Kampai.Game.QuestStepType.StorageRepair ||
						stepDef.Type == global::Kampai.Game.QuestStepType.LairPortalRepair ||
						stepDef.Type == global::Kampai.Game.QuestStepType.MinionUpgradeBuildingRepair)
					{
						int trackedId = stepDef.ItemDefinitionID;
						if (stepDef.Type == global::Kampai.Game.QuestStepType.MinionUpgradeBuildingRepair) trackedId = 375;
						
						var builds = playerService.GetByDefinitionId<global::Kampai.Game.Building>(trackedId);
						if (builds != null)
						{
							foreach (var b in builds)
							{
								if (b.State == global::Kampai.Game.BuildingState.Complete || b.State == global::Kampai.Game.BuildingState.Construction)
								{
									b.SetState(global::Kampai.Game.BuildingState.Broken);
									repairBuildingSignal.Dispatch(b);
								}
							}
						}
					}
					
					if (i < targetQuest.Quest.Steps.Count)
					{
						targetQuest.Quest.Steps[i].state = global::Kampai.Game.QuestStepState.Notstarted;
						targetQuest.Quest.Steps[i].AmountCompleted = 0;
					}
				}
			}

			targetQuest.Quest.state = global::Kampai.Game.QuestState.Notstarted;
			
			if (isPrevious)
			{
				foreach (var kvp in questService.GetQuestMap())
				{
					if (kvp.Value.Definition.QuestLineID == targetQuest.Definition.QuestLineID && 
						kvp.Value.Definition.NarrativeOrder > targetQuest.Definition.NarrativeOrder)
					{
						kvp.Value.Quest.state = global::Kampai.Game.QuestState.Notstarted;
						foreach(var s in kvp.Value.Quest.Steps) 
						{
							s.state = global::Kampai.Game.QuestStepState.Notstarted;
							s.AmountCompleted = 0;
						}
					}
				}
			}

			targetQuest.GoToQuestState(global::Kampai.Game.QuestState.RunningStartScript);
			outBuilder.AppendLine("Reset quest: " + targetQuest.Definition.ID + " (" + (isPrevious ? "regressed to previous" : "restarted current") + ").");
		}

		[global::Kampai.Util.DebugCommand]
		public void Help2(string[] args)
		{
			string text = "quantity items:\n    ";
			int num = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.Definition> allDefinition in definitionService.GetAllDefinitions())
			{
				if (allDefinition.Value.LocalizedKey != null)
				{
					text += string.Format("{0}  |  ", allDefinition.Value.LocalizedKey.Replace(' ', '_').ToLower());
					num++;
					if (num > 4)
					{
						text += "\n    ";
						num = 0;
					}
				}
			}
			outBuilder.Append(text);
		}

		[global::Kampai.Util.DebugCommand]
		public void Exit(string[] args)
		{
			CloseConsoleSignal.Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void AutoOrderBoard(string[] args)
		{
			global::Kampai.Game.View.OrderBoardBuildingObjectView orderBoardBuildingObjectView = global::UnityEngine.Object.FindFirstObjectByType<global::Kampai.Game.View.OrderBoardBuildingObjectView>();
			if (orderBoardBuildingObjectView != null)
			{
				global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(orderBoardBuildingObjectView.ID);
				if (byInstanceId != null)
				{
					openBuildingMenuSignal.Dispatch(orderBoardBuildingObjectView, byInstanceId);
				}
			}
			global::Kampai.Util.DebugButton debugButton = global::UnityEngine.Object.FindFirstObjectByType<global::Kampai.Util.DebugButton>();
			if (debugButton != null)
			{
				debugButton.OnClick(null);
			}
			int result = 1;
			if (args.Length > 1)
			{
				int.TryParse(args[1], out result);
			}
			routineRunner.StartCoroutine(AutoOrderBoardLoop(result));
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "Scope" })]
		public void Hindsight(string[] args)
		{
			if (args.Length < 2)
			{
				outBuilder.AppendLine("Mising argument Scope");
				return;
			}
			global::Kampai.Main.HindsightCampaign.Scope scope = global::Kampai.Main.HindsightCampaign.Scope.unknown;
			try
			{
				scope = (global::Kampai.Main.HindsightCampaign.Scope)(int)global::System.Enum.Parse(typeof(global::Kampai.Main.HindsightCampaign.Scope), args[1]);
			}
			catch (global::System.Exception ex)
			{
				outBuilder.AppendFormat("Failed to convert scope: {0}\n", args[1]);
				outBuilder.AppendFormat("Exception: {0}", ex.Message);
				return;
			}
			if (scope == global::Kampai.Main.HindsightCampaign.Scope.unknown)
			{
				outBuilder.AppendFormat("Scope is unknown");
			}
			else
			{
				mainContext.injectionBinder.GetInstance<global::Kampai.Main.DisplayHindsightContentSignal>().Dispatch(scope);
			}
		}

		private global::System.Collections.IEnumerator AutoOrderBoardLoop(int orderCount)
		{
			yield return new global::UnityEngine.WaitForSeconds(2.5f);
			global::UnityEngine.GameObject go = global::UnityEngine.GameObject.Find("btn_FillOrder_normal");
			if (go == null)
			{
				yield return null;
			}
			FillOrderButtonView fillOrderButtonView = go.GetComponent<FillOrderButtonView>();
			if (fillOrderButtonView == null)
			{
				yield return null;
			}
			for (int i = 0; i < orderCount; i++)
			{
				global::Kampai.UI.View.OrderBoardTicketView[] obtView = global::UnityEngine.Object.FindObjectsByType<global::Kampai.UI.View.OrderBoardTicketView>(global::UnityEngine.FindObjectsSortMode.None);
				if (obtView.Length > 0)
				{
					obtView[global::UnityEngine.Random.Range(0, obtView.Length)].TicketButton.ClickedSignal.Dispatch();
				}
				yield return new global::UnityEngine.WaitForSeconds(0.1f);
				if (fillOrderButtonView.gameObject.activeInHierarchy)
				{
					fillOrderButtonView.OnClickEvent();
				}
				yield return new global::UnityEngine.WaitForSeconds(2f);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void Auth(string[] args)
		{
			outBuilder.AppendLine("unsupported");
		}

		[global::Kampai.Util.DebugCommand]
		public void Stream(string[] args)
		{
			videoService.playVideo("https://archive.org/download/Pbtestfilemp4videotestmp4/video_test.mp4", true, true);
		}

		[global::Kampai.Util.DebugCommand]
		public void Lod(string[] args)
		{
			outBuilder.Append(string.Join(" ", args) + " " + minionBuilder.GetLOD());
		}

		[global::Kampai.Util.DebugCommand]
		public void Throw(string[] args)
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Error, true, "Throwing...");
			if (args.Length < 2 || args[1].Equals("null"))
			{
				string text = null;
				text.IndexOf("a");
				return;
			}
			if (args[1].Equals("bind"))
			{
				gameContext.injectionBinder.Bind<object>().ToValue(this).ToName("DEBUG");
				gameContext.injectionBinder.Bind<object>().ToValue(42).ToName("DEBUG");
				gameContext.injectionBinder.GetInstance<object>("DEBUG");
				return;
			}
			if (args[1].Equals("index"))
			{
				string text2 = args[args.Length + 100];
				logger.Info(text2);
				return;
			}
			throw new global::System.Exception();
		}

		[global::Kampai.Util.DebugCommand]
		public void Crash(string[] args)
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Error, true, "Crashing...");
			global::Kampai.Util.Native.Crash();
		}

		[global::Kampai.Util.DebugCommand]
		public void HardCrash(string[] args)
		{
			logger.Log(global::Kampai.Util.KampaiLogLevel.Error, true, "Crashing hard...");
			global::Kampai.Game.IBuildingUtilities buildingUtilities = null;
			global::UnityEngine.Debug.Log(buildingUtilities.AvailableLandSpaceCount().ToString());
		}

		[global::Kampai.Util.DebugCommand]
		public void Health(string[] args)
		{
			metricsSignal.Dispatch(false);
		}

		[global::Kampai.Util.DebugCommand]
		public void Notify(string[] args)
		{
			global::Kampai.Game.NotificationDefinition notificationDefinition = new global::Kampai.Game.NotificationDefinition();
			notificationDefinition.ID = 11;
			notificationDefinition.Seconds = 30;
			notificationDefinition.Title = "Oh";
			notificationDefinition.Text = "Debug console notification";
			notificationDefinition.Type = global::Kampai.Game.NotificationType.DebugConsole.ToString();
			notificationSignal.Dispatch(notificationDefinition);
		}

		[global::Kampai.Util.DebugCommand]
		public void LocalNote(string[] args)
		{
			int result;
			global::Kampai.Game.NotificationDefinition definition;
			if (args.Length < 2)
			{
				outBuilder.AppendLine("Need arg");
			}
			else if (int.TryParse(args[1], out result) && definitionService.TryGet<global::Kampai.Game.NotificationDefinition>(result, out definition))
			{
				definition.Seconds = 30;
				notificationSignal.Dispatch(definition);
			}
			else
			{
				outBuilder.Append("No such notification ID");
			}
		}

		public void Telemetry(string[] args)
		{
			// telemetryService.Send_Telemetry_EVT_GAME_ERROR_GAMEPLAY("error", "test", true);
		}

		[global::Kampai.Util.DebugCommand]
		public void Fps(string[] args)
		{
			global::Kampai.UI.View.ToggleFPSGraphSignal instance = uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ToggleFPSGraphSignal>();
			instance.Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void Quality(string[] args)
		{
			outBuilder.Append(global::UnityEngine.QualitySettings.names[global::UnityEngine.QualitySettings.GetQualityLevel()]);
		}

		[global::Kampai.Util.DebugCommand]
		public void ap(string[] args)
		{
			global::UnityEngine.Time.timeScale = 1f - global::UnityEngine.Time.timeScale;
		}

		[global::Kampai.Util.DebugCommand]
		public void ToggleHUD(string[] args)
		{
			if (hudView == null)
			{
				hudView = global::UnityEngine.Object.FindObjectOfType<global::Kampai.UI.View.HUDView>();
			}
			if (hudView != null)
			{
				hudView.gameObject.SetActive(!hudView.gameObject.activeSelf);
			}
			else
			{
				logger.Error("Can't find HUD view");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ToggleAnimators(string[] args)
		{
			global::UnityEngine.Animator[] array = global::UnityEngine.Object.FindObjectsByType<global::UnityEngine.Animator>(global::UnityEngine.FindObjectsSortMode.None);
			foreach (global::UnityEngine.Animator animator in array)
			{
				animator.enabled = false;
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void FindByType(string[] args)
		{
			if (args.Length < 2)
			{
				outBuilder.AppendLine("Must include type name to search");
				return;
			}
			global::System.Type typeByName = GetTypeByName(args[1]);
			if (typeByName == null)
			{
				outBuilder.AppendLine(string.Format("Could not find type {0}", args[1]));
				return;
			}
			global::UnityEngine.Object[] array = global::UnityEngine.Object.FindObjectsByType(typeByName, global::UnityEngine.FindObjectsSortMode.None);
			if (array == null)
			{
				return;
			}
			outBuilder.AppendLine(string.Format("Found {0} object", array.Length));
			global::UnityEngine.Object[] array2 = array;
			foreach (global::UnityEngine.Object obj in array2)
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find(obj.name);
				if (gameObject != null)
				{
					outBuilder.AppendLine(string.Format("Name: {0}, activeSelf: {1}", obj.name, gameObject.activeSelf));
				}
				else
				{
					outBuilder.AppendLine(string.Format("Name: {0}", obj.name));
				}
			}
		}

		private global::System.Type GetTypeByName(string typeName)
		{
			global::System.Type type = null;
			string[] array = new string[5]
			{
				string.Empty,
				"Kampai",
				"UnityEngine",
				"UnityEngine.UI",
				"UnityEngine.Rendering"
			};
			int num = 0;
			while (type == null && num < array.Length)
			{
				string text = array[num++];
				string text2 = (string.IsNullOrEmpty(text) ? string.Empty : (text + ".")) + typeName;
				string text3 = (string.IsNullOrEmpty(text) ? string.Empty : ("," + text));
				type = global::System.Type.GetType(text2 + text3);
			}
			return type;
		}

		[global::Kampai.Util.DebugCommand]
		public void ToggleActive(string[] args)
		{
			if (args.Length < 2)
			{
				return;
			}
			global::UnityEngine.GameObject value = null;
			if (hiddenGameObjectsMap == null)
			{
				hiddenGameObjectsMap = new global::System.Collections.Generic.Dictionary<string, global::UnityEngine.GameObject>();
			}
			bool flag = hiddenGameObjectsMap.TryGetValue(args[1], out value);
			if (value == null)
			{
				value = global::UnityEngine.GameObject.Find(args[1]);
			}
			if (!(value == null))
			{
				if (!flag)
				{
					hiddenGameObjectsMap.Add(args[1], value);
				}
				value.SetActive(!value.activeSelf);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ToggleTransparent(string[] args)
		{
			showTransparent = !showTransparent;
			if (!showTransparent)
			{
				HiddenTransparentObjects = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();
				global::UnityEngine.Renderer[] array = global::UnityEngine.Object.FindObjectsByType<global::UnityEngine.Renderer>(global::UnityEngine.FindObjectsSortMode.None);
				foreach (global::UnityEngine.Renderer renderer in array)
				{
					if (renderer.material.renderQueue >= 3000)
					{
						renderer.gameObject.SetActive(false);
						HiddenTransparentObjects.Add(renderer.gameObject);
					}
				}
			}
			if (!showTransparent)
			{
				return;
			}
			foreach (global::UnityEngine.GameObject hiddenTransparentObject in HiddenTransparentObjects)
			{
				hiddenTransparentObject.SetActive(true);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void HideShader(string[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}
			global::UnityEngine.Renderer[] array = global::UnityEngine.Object.FindObjectsByType<global::UnityEngine.Renderer>(global::UnityEngine.FindObjectsSortMode.None);
			foreach (global::UnityEngine.Renderer renderer in array)
			{
				if (renderer.material.shader.name.ToLower().Contains(args[1].ToLower()))
				{
					renderer.gameObject.SetActive(false);
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void System(string[] args)
		{
			global::UnityEngine.Resolution currentResolution = global::UnityEngine.Screen.currentResolution;
			outBuilder.AppendLine("\t Model: " + global::UnityEngine.SystemInfo.deviceModel);
			outBuilder.AppendLine("\t resolution: " + currentResolution.width + "x" + currentResolution.height + ", " + currentResolution.refreshRateRatio.value);
			outBuilder.AppendLine("\t processor count: " + global::UnityEngine.SystemInfo.processorCount);
			outBuilder.AppendLine("\t ram: " + global::UnityEngine.SystemInfo.systemMemorySize);
			outBuilder.AppendLine("\t vram: " + global::UnityEngine.SystemInfo.graphicsMemorySize);
			outBuilder.AppendLine("\t shader level: " + global::UnityEngine.SystemInfo.graphicsShaderLevel);
			outBuilder.AppendLine("\t gpu vendor: " + global::UnityEngine.SystemInfo.graphicsDeviceVendor);
			outBuilder.AppendLine("\t gpu name: " + global::UnityEngine.SystemInfo.graphicsDeviceName);
		}

		[global::Kampai.Util.DebugCommand]
		public void User(string[] args)
		{
			string data = localPersistService.GetData("UserID");
			outBuilder.AppendLine(string.Join(" ", args) + " -> UserID: " + data);
			string plainText = localPersistService.GetData("AnonymousID");
			encryptionService.TryDecrypt(plainText, "Kampai!", out plainText);
			outBuilder.AppendLine("\tAnonynousID: " + plainText);
			string userID = facebookService.userID;
			outBuilder.AppendLine("\tFacebookID: " + userID);
			// string synergyId = NimbleBridge_SynergyIdManager.GetComponent().GetSynergyId();
			// outBuilder.AppendLine("\tSynergyID: " + synergyId);
			// string sdkVersion = NimbleBridge_Base.GetSdkVersion();
			// outBuilder.AppendLine("\tNimbleSDK: " + sdkVersion);
			string userID2 = googlePlayService.userID;
			outBuilder.AppendLine("\tGooglePlayID: " + userID2);
		}

		[global::Kampai.Util.DebugCommand]
		public void GC(string[] args)
		{
		}

		[global::Kampai.Util.DebugCommand]
		public void CBO(string[] args)
		{
			global::UnityEngine.Ray ray = new global::UnityEngine.Ray(cameraGO.transform.position, cameraGO.transform.forward);
			global::UnityEngine.RaycastHit hitInfo;
			if (global::UnityEngine.Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 512))
			{
				global::UnityEngine.Vector3 center = hitInfo.collider.gameObject.GetComponent<global::Kampai.Game.View.BuildingObject>().Center;
				outBuilder.AppendLine("Offset: " + (cameraGO.transform.position - center).ToString() + " Zoom: " + zoomView.GetCurrentPercentage());
				return;
			}
			ray = new global::UnityEngine.Ray(cameraGO.transform.position + new global::UnityEngine.Vector3(2f, 0f, 2f), cameraGO.transform.forward);
			if (global::UnityEngine.Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 512))
			{
				global::UnityEngine.Vector3 center2 = hitInfo.collider.gameObject.GetComponent<global::Kampai.Game.View.BuildingObject>().Center;
				outBuilder.AppendLine("Offset: " + (cameraGO.transform.position - center2).ToString() + " Zoom: " + zoomView.GetCurrentPercentage());
			}
			else
			{
				outBuilder.AppendLine("Unable to determine building");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void locTest(string[] args)
		{
			int time = 10;
			int time2 = 75;
			int time3 = 12345;
			int time4 = 1234567;
			int num = 7500;
			int num2 = 777333;
			int num3 = 123456789;
			float num4 = 1f;
			float num5 = 55.55f;
			float num6 = 1234567.9f;
			global::Kampai.Main.ILocalizationService instance = gameContext.injectionBinder.GetInstance<global::Kampai.Main.ILocalizationService>();
			outBuilder.AppendLine("---  country is " + instance.GetCountry() + " & lang is " + global::Kampai.Util.Native.GetDeviceLanguage());
			outBuilder.AppendLine("time1: " + UIUtils.FormatTime(time, instance));
			outBuilder.AppendLine("time2: " + UIUtils.FormatTime(time2, instance));
			outBuilder.AppendLine("time3: " + UIUtils.FormatTime(time3, instance));
			outBuilder.AppendLine("time4: " + UIUtils.FormatTime(time4, instance));
			outBuilder.AppendLine("date: " + UIUtils.FormatDate(timeService.CurrentTime(), instance));
			outBuilder.AppendLine(num + " = " + UIUtils.FormatLargeNumber(num));
			outBuilder.AppendLine(num2 + " = " + UIUtils.FormatLargeNumber(num2));
			outBuilder.AppendLine(num3 + " = " + UIUtils.FormatLargeNumber(num3));
			outBuilder.AppendLine(string.Format("{0:0.00##}", num4) + " = " + num4.ToString("R", instance.CultureInfo));
			outBuilder.AppendLine(string.Format("{0:0.00##}", num5) + " = " + num5.ToString("R", instance.CultureInfo));
			outBuilder.AppendLine(string.Format("{0:0.00##}", num6) + " = " + num6.ToString("R", instance.CultureInfo));
			outBuilder.AppendLine("-----------");
		}

		[global::Kampai.Util.DebugCommand]
		public void tubes(string[] args)
		{
			int result = 3;
			if (args.Length > 1 && int.TryParse(args[1], out result) && (result > 3 || result < 0))
			{
				result = 3;
			}
			Exit(new string[0]);
			routineRunner.StartCoroutine(CallForTubes(result));
		}

		private global::System.Collections.IEnumerator CallForTubes(int tubeCount)
		{
			yield return new global::UnityEngine.WaitForSeconds(1f);
			for (int i = 0; i < tubeCount; i++)
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.CreateInnerTubeSignal>().Dispatch(i);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "ua")]
		public void UpdateAchievement(string[] args)
		{
			int result;
			int result2;
			if (int.TryParse(args[1], out result) && int.TryParse(args[2], out result2))
			{
				achievementService.UpdateIncrementalAchievement(result, result2);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "ra")]
		public void ResetAchievements(string[] args)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.Achievement> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Achievement>();
			for (int i = 0; i < instancesByType.Count; i++)
			{
				playerService.Remove(instancesByType[i]);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void AddMinion(string[] args)
		{
			int minionsToAdd;
			if (args.Length == 1)
			{
				minionsToAdd = 1;
			}
			else
			{
				int result;
				if (!int.TryParse(args[1], out result))
				{
					result = 1;
				}
				minionsToAdd = result;
			}
			AddMinions(minionsToAdd);
		}

		private void AddMinions(int minionsToAdd)
		{
			for (int i = 0; i < minionsToAdd; i++)
			{
				int id = global::UnityEngine.Random.Range(601, 607);
				global::Kampai.Game.MinionDefinition def = definitionService.Get<global::Kampai.Game.MinionDefinition>(id);
				global::Kampai.Game.Minion minion = new global::Kampai.Game.Minion(def);
				playerService.Add(minion);
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.CreateMinionSignal>().Dispatch(minion);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "addingredients", Args = new string[] { "ingredients_id", "amount" }, RequiresAllArgs = true)]
		public void AddIngredients(string[] args)
		{
			int result;
			uint result2;
			if (int.TryParse(args[1], out result) && uint.TryParse(args[2], out result2))
			{
				global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
				transactionDefinition.Inputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
				transactionDefinition.Outputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
				global::Kampai.Util.QuantityItem item = new global::Kampai.Util.QuantityItem(result, result2);
				transactionDefinition.Outputs.Add(item);
				playerService.RunEntireTransaction(transactionDefinition, global::Kampai.Game.TransactionTarget.INGREDIENT, TransactionCallback);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void LevelUp(string[] args)
		{
			int num;
			if (args.Length == 1)
			{
				num = 1;
			}
			else
			{
				int result;
				if (!int.TryParse(args[1], out result))
				{
					result = 1;
				}
				num = result - (int)playerService.GetQuantity(global::Kampai.Game.StaticItem.LEVEL_ID);
			}
			if (num < 0)
			{
				outBuilder.AppendLine("You can't Level Down silly.");
				return;
			}
			for (int i = 0; i < num; i++)
			{
				Level();
			}
			global::System.Action<int, int> callback = delegate
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.EnableCameraBehaviourSignal>().Dispatch(3);
			};
			gameContext.injectionBinder.GetInstance<global::Kampai.UI.View.PromptReceivedSignal>().AddOnce(callback);
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetXPSignal>().Dispatch();
		}

		private void Level()
		{
			playerService.AlterQuantity(global::Kampai.Game.StaticItem.LEVEL_ID, 1);
			global::Kampai.Game.Transaction.TransactionDefinition rewardTransaction = RewardUtil.GetRewardTransaction(definitionService, playerService);
			awardLevelSignal.Dispatch(rewardTransaction);
			characterService.UpdateEligiblePrestigeList();
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.GetNewQuestSignal>().Dispatch();
			telemetryService.Send_Telemetry_EVT_GP_LEVEL_PROMOTION();
			playerDurationService.MarkLevelUpUTC();
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.UpdateMarketplaceRepairStateSignal>().Dispatch();
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.UpdateForSaleSignsSignal>().Dispatch();
			if (playerService.GetHighestFtueCompleted() >= 999999)
			{
				reconcileSalesSignal.Dispatch(0);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void PlayerTraining(string[] args)
		{
			if (args.Length == 1)
			{
				outBuilder.AppendLine("Invalid call.  You need to specify a PlayerTrainingDefinition ID");
				return;
			}
			int result;
			if (!int.TryParse(args[1], out result))
			{
				outBuilder.AppendLine("Invalid call.  You need to specify a PlayerTrainingDefinition ID");
				return;
			}
			global::Kampai.Game.PlayerTrainingDefinition playerTrainingDefinition = definitionService.Get<global::Kampai.Game.PlayerTrainingDefinition>(result);
			if (playerTrainingDefinition == null)
			{
				outBuilder.AppendLine("Invalid ID.  You need to specify a PlayerTrainingDefinition ID");
			}
			else
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.DisplayPlayerTrainingSignal>().Dispatch(result, true, new global::strange.extensions.signal.impl.Signal<bool>());
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void Experience(string[] args)
		{
			int result;
			if (!int.TryParse(args[1], out result))
			{
				result = 50;
			}
			playerService.CreateAndRunCustomTransaction(2, result, global::Kampai.Game.TransactionTarget.NO_VISUAL);
		}

		[global::Kampai.Util.DebugCommand(Name = "quantity", Args = new string[] { "getQuantity [def id]" }, RequiresAllArgs = true)]
		public void quantity(string[] args)
		{
			int result = 0;
			if (args.Length > 0 && int.TryParse(args[1], out result))
			{
				outBuilder.AppendFormat("quantity of {0} is {1}", result, playerService.GetQuantityByDefinitionId(result));
			}
			else
			{
				outBuilder.AppendFormat("Unable to parse {0}", args[1]);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "availableland")]
		public void AvailableLandSpaceCount(string[] args)
		{
			global::Kampai.Game.IBuildingUtilities instance = gameContext.injectionBinder.GetInstance<global::Kampai.Game.IBuildingUtilities>();
			if (instance == null)
			{
				outBuilder.AppendFormat("Couldn't find type {0}", typeof(global::Kampai.Game.IBuildingUtilities));
				return;
			}
			outBuilder.AppendFormat("AvailableLandSpaceCount: {0}", instance.AvailableLandSpaceCount());
			outBuilder.AppendFormat("Total Saved To Player AvailableLandSpaceCount: {0}", playerService.GetQuantity(global::Kampai.Game.StaticItem.TOTAL_AVAILABLE_LAND_SPACE));
		}

		[global::Kampai.Util.DebugCommand]
		public void PlaySound(string[] args)
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Main.PlayGlobalSoundFXSignal>().Dispatch(args[1]);
		}

		[global::Kampai.Util.DebugCommand(Name = "showfps", Args = new string[] { "sampleSize" }, RequiresAllArgs = true)]
		[global::Kampai.Util.DebugCommand(Name = "hidefps")]
		public void ToggleFps(string[] args)
		{
			bool flag = args[0] == "showfps";
			int result = 0;
			if (flag)
			{
				if (int.TryParse(args[1], out result))
				{
					addFPSCounterSignal.Dispatch(true, result);
				}
				else
				{
					outBuilder.AppendFormat("Invalid sampleSize {0} must be an integer value", args[1]);
				}
			}
			else
			{
				addFPSCounterSignal.Dispatch(false, 0);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void AddPartyFavorItem(string[] args)
		{
			int result;
			if (args.Length < 2)
			{
				outBuilder.AppendLine(string.Format("Invalid Party Favor Id {0}", args));
			}
			else if (int.TryParse(args[1], out result))
			{
				global::Kampai.Game.Definition definition = definitionService.Get(result);
				global::Kampai.Game.PartyFavorAnimationDefinition partyFavorAnimationDefinition = definition as global::Kampai.Game.PartyFavorAnimationDefinition;
				if (partyFavorAnimationDefinition != null)
				{
					playerService.CreateAndRunCustomTransaction(partyFavorAnimationDefinition.UnlockId, 1, global::Kampai.Game.TransactionTarget.NO_VISUAL);
					return;
				}
				global::Kampai.Game.PartyFavorAnimationItemDefinition partyFavorAnimationItemDefinition = definition as global::Kampai.Game.PartyFavorAnimationItemDefinition;
				if (partyFavorAnimationItemDefinition != null)
				{
					playerService.CreateAndRunCustomTransaction(partyFavorAnimationItemDefinition.ID, 1, global::Kampai.Game.TransactionTarget.NO_VISUAL);
					return;
				}
				outBuilder.AppendLine(string.Format("Invalid Definition Type {0}", definition));
				outBuilder.AppendLine("Enter in a Definition id of type PartyFavorAnimationItemDefinition or PartyFavorAnimationDefinition");
			}
			else
			{
				outBuilder.AppendLine(string.Format("Invalid Argument for Party Favor Id {0}", args[1]));
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ListActivePartyFavors(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.PartyFavorAnimationDefinition> all = definitionService.GetAll<global::Kampai.Game.PartyFavorAnimationDefinition>();
			if (all == null)
			{
				return;
			}
			int num = 0;
			foreach (global::Kampai.Game.PartyFavorAnimationDefinition item in all)
			{
				if (playerService.GetQuantityByDefinitionId(item.UnlockId) != 0)
				{
					outBuilder.AppendLine(string.Format("{0}: Name {2}, id {1}, item id: {3}, animation id: {4}", ++num, item.ID, item.LocalizedKey, item.ItemID, item.AnimationID));
					break;
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void PartyPoints(string[] args)
		{
			int result = 0;
			if (args.Length > 1 && int.TryParse(args[1], out result))
			{
				playerService.CreateAndRunCustomTransaction(2, result, global::Kampai.Game.TransactionTarget.NO_VISUAL);
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetXPSignal>().Dispatch();
				return;
			}
			global::Kampai.Game.MinionParty minionPartyInstance = playerService.GetMinionPartyInstance();
			bool flag = playerService.IsMinionPartyUnlocked();
			outBuilder.AppendLine("Minon Party is currently " + ((!flag) ? "Disabled" : "Enabled"));
			outBuilder.AppendLine("You currently have " + minionPartyInstance.CurrentPartyPoints + " party points.");
			outBuilder.AppendLine("You need " + minionPartyInstance.CurrentPartyPointsRequired + " (or another action) to raise Party Meter Tier again.");
		}

		[global::Kampai.Util.DebugCommand(Name = "showpp")]
		[global::Kampai.Util.DebugCommand(Name = "hidepp")]
		public void DisplayPartyPoints(string[] args)
		{
			if (args[0] == "showpp")
			{
				global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("PartyPoints");
				gameObject.transform.parent = glassCanvas.transform;
				gameObject.layer = 5;
				global::UnityEngine.UI.Text text = gameObject.AddComponent<global::UnityEngine.UI.Text>();
				text.transform.localPosition = global::UnityEngine.Vector3.zero;
				text.rectTransform.localScale = global::UnityEngine.Vector3.one;
				text.font = global::Kampai.Util.KampaiResources.Load<global::UnityEngine.Font>("HelveticaLTStd-Cond");
				text.fontSize = 32;
				text.rectTransform.offsetMin = new global::UnityEngine.Vector2(-50f, 0f - (float)global::UnityEngine.Screen.height / 2f);
				text.rectTransform.offsetMax = new global::UnityEngine.Vector2(50f, 100f - (float)global::UnityEngine.Screen.height / 2f);
				routineRunner.StartCoroutine(PartyPointsUpdate(text));
			}
			else
			{
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.GameObject.Find("PartyPoints");
				global::UnityEngine.UI.Text component = gameObject2.GetComponent<global::UnityEngine.UI.Text>();
				routineRunner.StopCoroutine(PartyPointsUpdate(component));
				global::UnityEngine.Object.Destroy(gameObject2);
			}
		}

		private global::System.Collections.IEnumerator PartyPointsUpdate(global::UnityEngine.UI.Text textCmp)
		{
			while (true)
			{
				yield return null;
				textCmp.text = playerService.GetQuantity(global::Kampai.Game.StaticItem.XP_ID).ToString();
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void StopSaving(string[] args)
		{
			savePlayerSignal.Dispatch(new global::Kampai.Util.Tuple<global::Kampai.Game.SaveLocation, string, bool>(global::Kampai.Game.SaveLocation.REMOTE, string.Empty, true));
			global::UnityEngine.PlayerPrefs.SetInt("Debug.StopSaving", 1);
			outBuilder.AppendLine("Last save complete. Turning off remote save. Use the StartSaving command to enable saving again.");
		}

		[global::Kampai.Util.DebugCommand]
		public void StartSaving(string[] args)
		{
			global::UnityEngine.PlayerPrefs.DeleteKey("Debug.StopSaving");
			outBuilder.AppendLine("Turning on remote save.");
		}

		[global::Kampai.Util.DebugCommand]
		public void StartParty(string[] args)
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.StartMinionPartyIntroSignal>().Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void EndParty(string[] args)
		{
			int result = 0;
			if (args.Length > 1 && int.TryParse(args[1], out result))
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.EndPartyBuffTimerSignal>().Dispatch(result);
			}
			else
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.EndPartyBuffTimerSignal>().Dispatch(1);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void HelpMe(string[] args)
		{
			mainContext.injectionBinder.GetInstance<global::Kampai.Main.OpenHelpSignal>().Dispatch(global::Kampai.Main.HelpType.ONLINE_HELP);
		}

		[global::Kampai.Util.DebugCommand(Name = "rc")]
		public void RefreshCatalog(string[] args)
		{
			currencyService.RefreshCatalog();
		}

		[global::Kampai.Util.DebugCommand]
		public void LandExpand(string[] args)
		{
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
			transactionDefinition.Inputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			transactionDefinition.Outputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			transactionDefinition.ID = int.MaxValue;
			global::Kampai.Game.PurchasedLandExpansion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
			global::System.Collections.Generic.IList<global::Kampai.Util.QuantityItem> outputs = transactionDefinition.Outputs;
			if (args.Length < 2)
			{
				foreach (global::Kampai.Game.LandExpansionBuilding allExpansionBuilding in landExpansionService.GetAllExpansionBuildings())
				{
					global::Kampai.Game.LandExpansionConfig expansionConfig = landExpansionConfigService.GetExpansionConfig(allExpansionBuilding.ExpansionID);
					global::Kampai.Util.QuantityItem item = new global::Kampai.Util.QuantityItem(expansionConfig.ID, 1u);
					if (!outputs.Contains(item) && !byInstanceId.HasPurchased(expansionConfig.expansionId))
					{
						outputs.Add(item);
					}
				}
			}
			else
			{
				for (int i = 1; i < args.Length; i++)
				{
					string text = args[i];
					int result;
					if (int.TryParse(text, out result))
					{
						global::Kampai.Game.LandExpansionConfig definition;
						if (definitionService.TryGet<global::Kampai.Game.LandExpansionConfig>(result, out definition))
						{
							if (byInstanceId.PurchasedExpansions.Contains(definition.expansionId))
							{
								outBuilder.AppendFormat("Already owned: {0}\n", result);
							}
							else
							{
								outputs.Add(new global::Kampai.Util.QuantityItem(definition.ID, 1u));
							}
						}
						else
						{
							outBuilder.AppendFormat("Not a land expansion config: {0}\n", result);
						}
					}
					else
					{
						outBuilder.AppendFormat("Not a number: {0}\n", text);
					}
				}
			}
			global::Kampai.Game.Trigger.QuantityItemTriggerRewardDefinition quantityItemTriggerRewardDefinition = new global::Kampai.Game.Trigger.QuantityItemTriggerRewardDefinition();
			quantityItemTriggerRewardDefinition.transaction = transactionDefinition.ToInstance();
			quantityItemTriggerRewardDefinition.RewardPlayer(gameContext);
			outBuilder.AppendFormat("Purchased {0} expansions\n", outputs.Count);
		}

		[global::Kampai.Util.DebugCommand]
		public void ClearDebris(string[] args)
		{
			global::Kampai.Game.CleanupDebrisSignal instance = gameContext.injectionBinder.GetInstance<global::Kampai.Game.CleanupDebrisSignal>();
			global::System.Collections.Generic.List<global::Kampai.Game.DebrisBuilding> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.DebrisBuilding>();
			foreach (global::Kampai.Game.DebrisBuilding item in instancesByType)
			{
				instance.Dispatch(item.ID, false);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void AddBuddy(string[] args)
		{
			global::Kampai.Game.PrestigeDefinition prestigeDefinition = definitionService.Get<global::Kampai.Game.PrestigeDefinition>(40002);
			playerService.AssignNextInstanceId(prestigeDefinition);
			global::Kampai.Game.Prestige prestige = new global::Kampai.Game.Prestige(prestigeDefinition);
			prestige.state = global::Kampai.Game.PrestigeState.Prestige;
			prestige.CurrentPrestigePoints = 100;
			prestigeService.AddPrestige(prestige);
		}

		[global::Kampai.Util.DebugCommand]
		public void CreateLocalSocialEventInvitations(string[] args)
		{
			if (localInvitations == null)
			{
				localInvitations = new global::System.Collections.Generic.List<global::Kampai.Game.SocialEventInvitation>();
			}
			global::Kampai.Game.SocialEventInvitation socialEventInvitation = new global::Kampai.Game.SocialEventInvitation();
			socialEventInvitation.EventID = 101;
			socialEventInvitation.Team = new global::Kampai.Game.SocialTeamInvitationView();
			socialEventInvitation.Team.TeamID = 12421342345234346L;
			socialEventInvitation.inviter = new global::Kampai.Game.UserIdentity();
			socialEventInvitation.inviter.ExternalID = "1374474932861011";
			socialEventInvitation.inviter.ID = "1374474932861011";
			socialEventInvitation.inviter.Type = global::Kampai.Game.IdentityType.discord;
			localInvitations.Add(socialEventInvitation);
		}

		public void OnGetSocialEventStateResponse(global::Kampai.Game.SocialTeamResponse response, global::Kampai.Game.ErrorResponse error)
		{
			if (response.UserEvent != null && !response.UserEvent.RewardClaimed && response.Team.OrderProgress.Count == timedSocialEventService.GetCurrentSocialEvent().Orders.Count)
			{
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShowSocialPartyRewardSignal>().Dispatch(timedSocialEventService.GetCurrentSocialEvent().ID);
			}
			else if (response.Team != null)
			{
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShowSocialPartyFillOrderSignal>().Dispatch(0);
			}
			else if (response.UserEvent != null && response.UserEvent.Invitations != null && response.UserEvent.Invitations.Count > 0 && facebookService.isLoggedIn)
			{
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShowSocialPartyInviteAlertSignal>().Dispatch();
			}
			else
			{
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShowSocialPartyStartSignal>().Dispatch();
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void CheckTriggers(string[] args)
		{
			global::Kampai.Game.TSMCharacter firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.TSMCharacter>(70008);
			if (firstInstanceByDefinitionId == null)
			{
				logger.Error("Failed to find TSM Character in player inventory");
			}
			else
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.CheckTriggersSignal>().Dispatch(firstInstanceByDefinitionId.ID);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ShowSocialPartyFlow(string[] args)
		{
			if (timedSocialEventService.GetCurrentSocialEvent() != null)
			{
				parseSocialEventInvitiations();
				global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse>();
				signal.AddListener(OnGetSocialEventStateResponse);
				timedSocialEventService.GetSocialEventState(timedSocialEventService.GetCurrentSocialEvent().ID, signal);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void AddStageBuilding(string[] args)
		{
			global::Kampai.Game.Location location = new global::Kampai.Game.Location(125, 158);
			global::Kampai.Game.StageBuildingDefinition stageBuildingDefinition = new global::Kampai.Game.StageBuildingDefinition();
			stageBuildingDefinition = definitionService.Get<global::Kampai.Game.StageBuildingDefinition>(3054);
			global::Kampai.Game.Building building = new global::Kampai.Game.StageBuilding(stageBuildingDefinition);
			building.ID = 356568568;
			building.Location = location;
			building.SetState(global::Kampai.Game.BuildingState.Idle);
			playerService.Add(building);
			global::Kampai.Game.CreateInventoryBuildingSignal instance = gameContext.injectionBinder.GetInstance<global::Kampai.Game.CreateInventoryBuildingSignal>();
			instance.Dispatch(building, location);
		}

		[global::Kampai.Util.DebugCommand]
		public void NutRush(string[] args)
		{
			global::Prime31.EtceteraAndroid.showCustomWebView("http://192.168.64.190/~bbangerter/nut_rush/index.htm", true, false);
		}

		[global::Kampai.Util.DebugCommand]
		public void SprintClub(string[] args)
		{
			global::Prime31.EtceteraAndroid.showCustomWebView("http://192.168.64.190/~bbangerter/sprint_club_nitro/index.htm", true, false);
		}

		[global::Kampai.Util.DebugCommand(Name = "createwayfinder", Args = new string[] { "trackedId" }, RequiresAllArgs = true)]
		[global::Kampai.Util.DebugCommand(Name = "removewayfinder", Args = new string[] { "trackedId" }, RequiresAllArgs = true)]
		public void CreateOrRemoveWayFinder(string[] args)
		{
			bool flag = args[0] == "createwayfinder";
			int num = int.Parse(args[1]);
			if (flag)
			{
				createWayFinderSignal.Dispatch(new global::Kampai.UI.View.WayFinderSettings(num));
			}
			else
			{
				removeWayFinderSignal.Dispatch(num);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "add", Args = new string[] { "quantity_item", "ammount" }, RequiresAllArgs = true)]
		[global::Kampai.Util.DebugCommand(Name = "remove", Args = new string[] { "quantity_item", "ammount" }, RequiresAllArgs = true)]
		public void AddOrRemove(string[] args)
		{
			bool add = args[0] == "add";
			int result = 0;
			if (!int.TryParse(args[1], out result) && !IdOf(args[1], out result))
			{
				outBuilder.AppendLine(string.Join(" ", args) + " -> DEFINITION NOT FOUND");
			}
			else
			{
				ProcessTransaction(args[2], result, add);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void Anim(string[] args)
		{
			if (args[1].Equals("on"))
			{
				loadAnimatorStateInfoSignal.Dispatch();
			}
			else if (args[1].Equals("off"))
			{
				unloadAnimatorStateInfoSignal.Dispatch();
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void Transaction(string[] args)
		{
			int result = 0;
			if (int.TryParse(args[1], out result))
			{
				ProcessTransaction(result, true);
			}
			mainContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetGrindCurrencySignal>().Dispatch();
			mainContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetPremiumCurrencySignal>().Dispatch();
			mainContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetStorageCapacitySignal>().Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void Grant(string[] args)
		{
			int result = 0;
			if (int.TryParse(args[1], out result))
			{
				ProcessTransaction(result, false);
			}
			mainContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetGrindCurrencySignal>().Dispatch();
			mainContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetPremiumCurrencySignal>().Dispatch();
			mainContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetStorageCapacitySignal>().Dispatch();
		}

		[global::Kampai.Util.DebugCommand(Name = "set lod")]
		public void SetLod(string[] args)
		{
			if (args[2].Equals("low"))
			{
				minionBuilder.SetLOD(global::Kampai.Util.TargetPerformance.LOW);
			}
			else if (args[2].Equals("medium"))
			{
				minionBuilder.SetLOD(global::Kampai.Util.TargetPerformance.MED);
			}
			else if (args[2].Equals("high"))
			{
				minionBuilder.SetLOD(global::Kampai.Util.TargetPerformance.HIGH);
			}
			outBuilder.AppendLine(string.Join(" ", args) + " -> LOD SET " + minionBuilder.GetLOD());
			mainContext.injectionBinder.GetInstance<global::Kampai.Main.ReloadGameSignal>().Dispatch();
		}

		[global::Kampai.Util.DebugCommand(Name = "set config")]
		public void SetConfig(string[] args)
		{
			if (args[2] != null)
			{
				global::Kampai.Util.ABTestCommand.GameMetaData gameMetaData = new global::Kampai.Util.ABTestCommand.GameMetaData();
				gameMetaData.configurationVariant = args[2];
				gameMetaData.debugConsoleTest = true;
				ABTestSignal.Dispatch(gameMetaData);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "set shaderlod")]
		public void SetShaderLod(string[] args)
		{
			if (args.Length > 2)
			{
				int result = -1;
				int.TryParse(args[2], out result);
				if (result < 0)
				{
					result = int.MaxValue;
				}
				global::UnityEngine.Shader.globalMaximumLOD = result;
			}
			outBuilder.AppendLine(string.Format("Shader LOD: {0}", global::UnityEngine.Shader.globalMaximumLOD));
		}

		[global::Kampai.Util.DebugCommand(Name = "list pf")]
		public void ListAvailablePartyFavors(string[] args)
		{
			global::System.Collections.Generic.List<int> availablePartyFavorItems = partyFavorAnimationService.GetAvailablePartyFavorItems();
			outBuilder.AppendLine("Count: " + availablePartyFavorItems.Count);
			foreach (int item in availablePartyFavorItems)
			{
				outBuilder.AppendLine("  ID: " + item);
			}
			outBuilder.AppendLine("\n");
		}

		[global::Kampai.Util.DebugCommand(Name = "trigger pf", Args = new string[] { "[id id]" })]
		public void TriggerPartyFavorIncidental(string[] args)
		{
			int result = 0;
			if (int.TryParse(args[2], out result))
			{
				partyFavorAnimationService.PlayRandomIncidentalAnimation(result);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "set definition", Args = new string[] { "[id id]|[variants variants]" })]
		public void SetDefinition(string[] args)
		{
			bool flag = false;
			global::Kampai.Util.ABTestCommand.GameMetaData gameMetaData = new global::Kampai.Util.ABTestCommand.GameMetaData();
			gameMetaData.debugConsoleTest = true;
			for (int i = 2; i < args.Length; i += 2)
			{
				if (i + 1 < args.Length && !string.IsNullOrEmpty(args[i]) && !string.IsNullOrEmpty(args[i + 1]))
				{
					string text = args[i];
					string text2 = args[i + 1];
					switch (text)
					{
					case "id":
						gameMetaData.definitionId = text2;
						flag = true;
						break;
					case "variants":
						gameMetaData.definitionVariants = text2;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				ABTestSignal.Dispatch(gameMetaData);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "set pref")]
		public void SetPref(string[] args)
		{
			if (args.Length != 5)
			{
				outBuilder.AppendLine("set pref: Too little arguments.");
				return;
			}
			string text = args[2];
			string text2 = args[3];
			string text3 = args[4];
			string empty = string.Empty;
			if (text.Equals("int"))
			{
				global::UnityEngine.PlayerPrefs.SetInt(text2, int.Parse(text3));
				empty = global::UnityEngine.PlayerPrefs.GetInt(text2).ToString();
			}
			else if (text.Equals("float"))
			{
				global::UnityEngine.PlayerPrefs.SetFloat(text2, float.Parse(text3));
				empty = global::UnityEngine.PlayerPrefs.GetFloat(text2).ToString();
			}
			else
			{
				global::UnityEngine.PlayerPrefs.SetString(text2, text3);
				empty = global::UnityEngine.PlayerPrefs.GetString(text2);
			}
			global::UnityEngine.PlayerPrefs.Save();
			outBuilder.AppendLine(text2 + " = " + empty);
		}

		[global::Kampai.Util.DebugCommand(Name = "get config")]
		public void GetConfig(string[] args)
		{
			outBuilder.AppendLine("Config URL: " + configService.GetConfigURL());
			outBuilder.AppendLine("Definition URL: " + configService.GetConfigurations().definitions);
		}

		[global::Kampai.Util.DebugCommand(Name = "get pref")]
		public void GetPref(string[] args)
		{
			string key = args[2];
			if (global::UnityEngine.PlayerPrefs.HasKey(key))
			{
				string empty = string.Empty;
				float num = global::UnityEngine.PlayerPrefs.GetFloat(key, float.NaN);
				int num2 = global::UnityEngine.PlayerPrefs.GetInt(key, int.MaxValue);
				empty = ((!num.Equals(float.NaN)) ? num.ToString() : (num2.Equals(int.MaxValue) ? global::UnityEngine.PlayerPrefs.GetString(key) : num2.ToString()));
				outBuilder.AppendLine(empty);
			}
			else
			{
				outBuilder.AppendLine("Key not found");
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "get shaders")]
		public void GetShaders(string[] args)
		{
			global::System.Collections.Generic.HashSet<string> hashSet = new global::System.Collections.Generic.HashSet<string>();
			global::UnityEngine.Renderer[] array = global::UnityEngine.Resources.FindObjectsOfTypeAll<global::UnityEngine.Renderer>();
			foreach (global::UnityEngine.Renderer renderer in array)
			{
				global::UnityEngine.Material[] materials = renderer.materials;
				foreach (global::UnityEngine.Material material in materials)
				{
					if (material.shader != null && !string.IsNullOrEmpty(material.shader.name))
					{
						hashSet.Add(material.shader.name);
					}
				}
			}
			outBuilder.AppendLine("Shaders:");
			foreach (string item in hashSet)
			{
				outBuilder.AppendLine("\t" + item);
			}
			outBuilder.AppendLine(string.Format("Found {0} shaders", hashSet.Count));
		}

		[global::Kampai.Util.DebugCommand]
		public void Enable(string[] args)
		{
			outBuilder.AppendLine(string.Join(" ", args) + " TO DO ??? not implemented");
		}

		[global::Kampai.Util.DebugCommand(Name = "load local")]
		public void LoadLocal(string[] args)
		{
			string data = args[2];
			localPersistService.PutData("LoadMode", "local");
			localPersistService.PutData("LocalID", data);
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
		}

		[global::Kampai.Util.DebugCommand(Name = "load file")]
		public void LoadFile(string[] args)
		{
			string data = args[2];
			localPersistService.PutData("LoadMode", "file");
			localPersistService.PutData("LocalFileName", data);
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
		}

		[global::Kampai.Util.DebugCommand(Name = "load remote")]
		public void LoadRemote(string[] args)
		{
			localPersistService.PutData("LoadMode", "remote");
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
		}

		[global::Kampai.Util.DebugCommand(Name = "zeroselltime", Args = new string[] { "on/off" }, RequiresAllArgs = true)]
		public void ZeroSellTime(string[] args)
		{
			if (args[1].Equals("on"))
			{
				localPersistService.PutData("ZeroSellTime", "true");
			}
			else
			{
				localPersistService.PutData("ZeroSellTime", "false");
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "minstrikecost", Args = new string[] { "item_id", "cost" }, RequiresAllArgs = true)]
		public void MinStrikeCost(string[] args)
		{
			int itemID = int.Parse(args[1]);
			int price = int.Parse(args[2]);
			marketplaceService.SetMinStrikePrice(itemID, price);
		}

		[global::Kampai.Util.DebugCommand(Name = "maxstrikecost", Args = new string[] { "item_id", "cost" }, RequiresAllArgs = true)]
		public void MaxStrikeCost(string[] args)
		{
			int itemID = int.Parse(args[1]);
			int price = int.Parse(args[2]);
			marketplaceService.SetMaxStrikePrice(itemID, price);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "corruptionId", "client/server" })]
		public void CorruptMe(string[] args)
		{
			string text = args[1];
			global::Kampai.Game.SaveLocation first = ((args.Length < 3 || args[2].StartsWith("s")) ? global::Kampai.Game.SaveLocation.REMOTE_NOSANITY : global::Kampai.Game.SaveLocation.REMOTE);
			if ("5622" == text)
			{
				global::Kampai.Game.PurchasedLandExpansion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.PurchasedLandExpansion>(354);
				byInstanceId.AdjacentExpansions.Clear();
				byInstanceId.PurchasedExpansions.Clear();
			}
			else
			{
				outBuilder.AppendLine("Invalid corruption id");
			}
			savePlayerSignal.Dispatch(new global::Kampai.Util.Tuple<global::Kampai.Game.SaveLocation, string, bool>(first, string.Empty, false));
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "local/remote" })]
		public void Save(string[] args)
		{
			string text = args[1];
			string second = string.Empty;
			global::Kampai.Game.SaveLocation first = global::Kampai.Game.SaveLocation.REMOTE;
			if (text == "local")
			{
				second = args[2];
				first = global::Kampai.Game.SaveLocation.LOCAL;
			}
			savePlayerSignal.Dispatch(new global::Kampai.Util.Tuple<global::Kampai.Game.SaveLocation, string, bool>(first, second, false));
		}

		[global::Kampai.Util.DebugCommand(Name = "delete local")]
		public void DeleteLocal(string[] args)
		{
			localPersistService.DeleteAll();
		}

		[global::Kampai.Util.DebugCommand(Name = "fb init")]
		public void FBInit(string[] args)
		{
			socialInitSignal.Dispatch(facebookService);
		}

		[global::Kampai.Util.DebugCommand(Name = "fb login")]
		public void FBLogin(string[] args)
		{
			facebookService.LoginSource = "Debug Console";
			socialLoginSignal.Dispatch(facebookService, new global::Kampai.Util.Boxed<global::System.Action>(null));
		}

		[global::Kampai.Util.DebugCommand(Name = "fb logout")]
		public void FBLogout(string[] args)
		{
			socialLogoutSignal.Dispatch(facebookService);
		}

		[global::Kampai.Util.DebugCommand(Name = "camera position")]
		public void CameraPosition(string[] args)
		{
			outBuilder.AppendLine(cameraGO.transform.position.ToString());
		}

		[global::Kampai.Util.DebugCommand(Name = "camera tilt")]
		public void CameraTilt(string[] args)
		{
			outBuilder.AppendLine(cameraGO.transform.eulerAngles.x.ToString());
		}

		[global::Kampai.Util.DebugCommand(Name = "camera fov")]
		public void CameraFOV(string[] args)
		{
			outBuilder.AppendLine(cameraComponent.fieldOfView.ToString());
		}

		[global::Kampai.Util.DebugCommand(Name = "camera initial zoom")]
		public void CameraInitialZoom(string[] args)
		{
			outBuilder.AppendLine(zoomView.InitialFraction.ToString());
		}

		[global::Kampai.Util.DebugCommand(Name = "synergy login")]
		public void SynergyLogin(string[] args)
		{
			logger.Debug("Logging into synergy");
			// NimbleBridge_SynergyIdManager.GetComponent().Login(args[2], "test");
		}

		[global::Kampai.Util.DebugCommand(Name = "purchase a")]
		public void PurchaseApprove(string[] args)
		{
			PurchaseApproval(true);
		}

		[global::Kampai.Util.DebugCommand(Name = "purchase d")]
		public void PurchaseDeny(string[] args)
		{
			PurchaseApproval(false);
		}

		[global::Kampai.Util.DebugCommand(Name = "map view enable")]
		public void MapEnable(string[] args)
		{
			SetMap(true);
		}

		[global::Kampai.Util.DebugCommand(Name = "map view disable")]
		public void MapDisable(string[] args)
		{
			SetMap(false);
		}

		[global::Kampai.Util.DebugCommand(Name = "grid enable")]
		public void GridEnable(string[] args)
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.PopulateEnvironmentSignal>().Dispatch(true);
		}

		[global::Kampai.Util.DebugCommand(Name = "grid disable")]
		public void GridDisable(string[] args)
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.PopulateEnvironmentSignal>().Dispatch(false);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "id" })]
		public void StartQuest(string[] args)
		{
			global::Kampai.Game.QuestDefinition def = definitionService.Get<global::Kampai.Game.QuestDefinition>(int.Parse(args[1]));
			global::Kampai.Game.Quest quest = new global::Kampai.Game.Quest(def);
			quest.Initialize();
			questService.RemoveQuest(quest.GetActiveDefinition().ID);
			questService.AddQuest(quest);
			questScriptService.StartQuestScript(quest, true);
		}

		[global::Kampai.Util.DebugCommand]
		public void Lua(string[] args)
		{
			string scriptText = string.Join(" ", args, 1, args.Length - 1);
			luaRunner.Stop();
			luaRunner.Start(consoleQuestScript, scriptText, "THE ALMIGHTY DEBUG CONSOLE", null);
		}

		[global::Kampai.Util.DebugCommand]
		public void LuaFile(string[] args)
		{
			luaRunner.Stop();
			global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>(args[1]);
			luaRunner.Start(consoleQuestScript, textAsset.text, args[1], null);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "id" })]
		public void ShowQuest(string[] args)
		{
			global::Kampai.Game.QuestDefinition questDefinition = definitionService.Get<global::Kampai.Game.QuestDefinition>(int.Parse(args[1]));
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.IQuestController> questMap = questService.GetQuestMap();
			global::Kampai.Game.IQuestController questController;
			if (questMap.ContainsKey(questDefinition.ID))
			{
				questController = questMap[questDefinition.ID];
			}
			else
			{
				global::Kampai.Game.Quest quest = new global::Kampai.Game.Quest(questDefinition);
				quest.Initialize();
				questController = questService.AddQuest(quest);
			}
			questController.Debug_SetQuestToInProgressIfNotAlready();
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShowQuestPanelSignal>().Dispatch(questController.ID);
		}

		[global::Kampai.Util.DebugCommand(Name = "dlc information")]
		public void DLCInfo(string[] args)
		{
			outBuilder.AppendLine("DLC functionality is disabled/bypassed in this build.");
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "fb/gc/gp" })]
		public void Unlink(string[] args)
		{
			global::System.Collections.Generic.Dictionary<string, global::Kampai.Game.IdentityType> dictionary = new global::System.Collections.Generic.Dictionary<string, global::Kampai.Game.IdentityType>();
			dictionary.Add("fb", global::Kampai.Game.IdentityType.discord);
			dictionary.Add("gc", global::Kampai.Game.IdentityType.gamecenter);
			dictionary.Add("gp", global::Kampai.Game.IdentityType.googleplay);
			global::System.Collections.Generic.Dictionary<string, global::Kampai.Game.IdentityType> dictionary2 = dictionary;
			global::Kampai.Game.IdentityType value;
			if (dictionary2.TryGetValue(args[1], out value))
			{
				unlinkAccountSignal.Dispatch(value);
			}
			else
			{
				outBuilder.AppendLine("Invalid identity type!");
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "displaymib (true|false)" })]
		public void DisplayMIB(string[] args)
		{
			bool type = global::System.Convert.ToBoolean(args[1]);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.DisplayMIBBuildingSignal>().Dispatch(type);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "setmibreturning (true|false)" })]
		public void SetMIBReturning(string[] args)
		{
			if (global::System.Convert.ToBoolean(args[1]))
			{
				mibService.SetReturningKey();
			}
			else
			{
				mibService.ClearReturningKey();
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "resetadlimits")]
		public void ResetRewardedAdLimits(string[] args)
		{
			logger.Info("Debug console: reset rewarded ad limits");
			resetRewardedAdLimitSignal.Dispatch();
		}



		[global::Kampai.Util.DebugCommand(Name = "adstatus")]
		public void RewardedAdStatus(string[] args)
		{
			global::Kampai.Game.RewardedAdService rewardedAdService = this.rewardedAdService as global::Kampai.Game.RewardedAdService;
			if (rewardedAdService == null)
			{
				outBuilder.AppendFormat("Not supported type of rewardedAdService: {0}", this.rewardedAdService.GetType());
			}
			else
			{
				outBuilder.AppendLine(rewardedAdService.GetPlacementsReport());
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "def id" }, RequiresAllArgs = true)]
		public void CreatePrestige(string[] args)
		{
			int result = 0;
			int.TryParse(args[1], out result);
			global::Kampai.Game.Prestige prestige = prestigeService.GetPrestige(result, false);
			if (prestige != null)
			{
				outBuilder.AppendFormat("Prestige already exists for def id {0}: {1}\n", result, prestige);
				return;
			}
			global::Kampai.Game.PrestigeDefinition prestigeDefinition = definitionService.Get<global::Kampai.Game.PrestigeDefinition>(result);
			if (prestigeDefinition == null)
			{
				outBuilder.AppendLine(string.Join(" ", args) + " -> DEFINITION NOT FOUND");
				return;
			}
			outBuilder.AppendLine("Creating new prestige from def id " + result);
			prestige = new global::Kampai.Game.Prestige(prestigeDefinition);
			prestigeService.AddPrestige(prestige);
			prestigeService.ChangeToPrestigeState(prestige, global::Kampai.Game.PrestigeState.Prestige);
			outBuilder.AppendFormat("Prestige created: {0}\n", prestige);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "instance id", "target state" }, RequiresAllArgs = true)]
		[global::Kampai.Util.DebugCommand(Name = "pc", Args = new string[] { "instance id", "target state" }, RequiresAllArgs = true)]
		public void PrestigeCharacter(string[] args)
		{
			int result = 0;
			int.TryParse(args[1], out result);
			global::Kampai.Game.Prestige byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Prestige>(result);
			if (byInstanceId == null)
			{
				outBuilder.AppendFormat("Prestige with id {0} not found!\n", result);
				return;
			}
			global::Kampai.Game.PrestigeState prestigeState;
			try
			{
				prestigeState = (global::Kampai.Game.PrestigeState)(int)global::System.Enum.Parse(typeof(global::Kampai.Game.PrestigeState), args[2], true);
			}
			catch (global::System.ArgumentException)
			{
				outBuilder.AppendFormat("{0} is not a quest state.\n", args[2]);
				return;
			}
			if (byInstanceId.state == global::Kampai.Game.PrestigeState.Prestige && prestigeState == global::Kampai.Game.PrestigeState.Prestige)
			{
				int num = byInstanceId.CurrentPrestigeLevel + 1;
				outBuilder.AppendFormat("Adding 1 to prestige level -> CurrentPrestigeLevel is now {0}\n", num);
				prestigeService.ChangeToPrestigeState(byInstanceId, global::Kampai.Game.PrestigeState.Prestige, num);
			}
			else
			{
				outBuilder.AppendFormat("Changing prestige from {0} state to {1} state.\n", byInstanceId.state, prestigeState);
				prestigeService.ChangeToPrestigeState(byInstanceId, prestigeState);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "lsp")]
		public void ListPrestige(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Prestige> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Prestige>();
			int i = 0;
			for (int count = instancesByType.Count; i < count; i++)
			{
				global::Kampai.Game.Prestige prestige = instancesByType[i];
				int num = 0;
				int num2 = 0;
				if (prestige.Definition.PrestigeLevelSettings != null)
				{
					num = prestige.CurrentPrestigeLevel;
					num2 = prestige.NeededPrestigePoints;
				}
				outBuilder.AppendFormat("{0}: ID:{1} State:{2} Level:{3} Points:{4} Needed:{5}\n", prestige.Definition.LocalizedKey, prestige.ID, num, global::System.Enum.GetName(typeof(global::Kampai.Game.PrestigeState), prestige.state), prestige.CurrentPrestigePoints, num2);
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "character id" })]
		public void UnlockCharacter(string[] args)
		{
			int result = 0;
			int.TryParse(args[1], out result);
			if (result != 0)
			{
				global::Kampai.Game.Prestige prestige = prestigeService.GetPrestige(result);
				if (prestige == null)
				{
					outBuilder.AppendLine(string.Join(" ", args) + " -> CHARACTER NOT FOUND");
				}
				else if (prestige.state != global::Kampai.Game.PrestigeState.Taskable && prestige.state == global::Kampai.Game.PrestigeState.Questing)
				{
					prestigeService.ChangeToPrestigeState(prestige, global::Kampai.Game.PrestigeState.Taskable);
				}
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "fatal id", "fatal code" })]
		public void Fatal(string[] args)
		{
			global::Kampai.Util.FatalCode code = (global::Kampai.Util.FatalCode)0;
			int result = 0;
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			if (args.Length > 1)
			{
				int result2 = 0;
				int.TryParse(args[1], out result2);
				code = (global::Kampai.Util.FatalCode)result2;
				if (args.Length > 2)
				{
					int.TryParse(args[2], out result);
				}
				for (int i = 4; i <= args.Length; i++)
				{
					stringBuilder.Append(args[i - 1]).Append(" ");
				}
			}
			string text = stringBuilder.ToString();
			if (string.IsNullOrEmpty(text))
			{
				logger.Fatal(code, result);
			}
			else
			{
				logger.Fatal(code, result, text);
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "points" })]
		public void AddMignetteScore(string[] args)
		{
			int result2;
			int result3;
			if (args.Length == 2 && mignetteGameModel.IsMignetteActive)
			{
				int result;
				if (int.TryParse(args[1], out result))
				{
					changeScoreSignal.Dispatch(result);
					spawnDooberSignal.Dispatch(glassCanvas.transform.GetComponentInChildren<global::Kampai.UI.View.MignetteHUDView>(), new global::UnityEngine.Vector3(250f, 250f, 0f), result, true);
					return;
				}
			}
			else if (args.Length == 3 && int.TryParse(args[1], out result2) && int.TryParse(args[2], out result3))
			{
				global::Kampai.Game.Trigger.MignetteScoreTriggerRewardDefinition mignetteScoreTriggerRewardDefinition = new global::Kampai.Game.Trigger.MignetteScoreTriggerRewardDefinition();
				mignetteScoreTriggerRewardDefinition.Points = result2;
				mignetteScoreTriggerRewardDefinition.MignetteBuildingId = result3;
				mignetteScoreTriggerRewardDefinition.RewardPlayer(gameContext);
				return;
			}
			outBuilder.AppendLine("Usage: addmignettescore <points> [building id]");
		}

		[global::Kampai.Util.DebugCommand]
		public void AwardNextPrize(string[] args)
		{
			if (!mignetteGameModel.IsMignetteActive)
			{
				outBuilder.AppendLine(args[0] + "-> Can only be called while mignette is active");
				return;
			}
			global::Kampai.Game.RewardCollection activeCollectionForMignette = mignetteCollectionService.GetActiveCollectionForMignette(mignetteGameModel.BuildingId, false);
			int pointTotalForNextReward = activeCollectionForMignette.GetPointTotalForNextReward();
			changeScoreSignal.Dispatch(pointTotalForNextReward);
			spawnDooberSignal.Dispatch(glassCanvas.transform.GetComponentInChildren<global::Kampai.UI.View.MignetteHUDView>(), new global::UnityEngine.Vector3(250f, 250f, 0f), pointTotalForNextReward, true);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "version" })]
		public void Version(string[] args)
		{
			int result;
			if (args.Length > 1 && int.TryParse(args[1], out result))
			{
				localPersistService.PutDataInt("OverrideVersion", result);
				localPersistService.PutData("OverrideVersionPersistState", "keep");
				outBuilder.AppendLine("New version will be active on app restart.");
			}
			else
			{
				outBuilder.AppendLine("Usage: version 1234");
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "upgrade", Args = new string[] { "f/o" })]
		public void UpgradeClient(string[] args)
		{
			switch (args[1])
			{
			case "f":
				showForcedClientUpgradeScreenSignal.Dispatch();
				break;
			case "o":
				showClientUpgradeDialogSignal.Dispatch();
				break;
			default:
				outBuilder.Append("Usage: update f / update o");
				break;
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "idof", Args = new string[] { "item_name" }, RequiresAllArgs = true)]
		public void IdOfCommand(string[] args)
		{
			int id;
			if (!IdOf(args[1], out id))
			{
				outBuilder.AppendLine(string.Join(" ", args) + " -> DEFINITION NOT FOUND");
			}
			else
			{
				outBuilder.AppendLine(id.ToString());
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void UnlockSticker(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				playerService.CreateAndRunCustomTransaction(result, 1, global::Kampai.Game.TransactionTarget.NO_VISUAL);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void CustomCamera(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.CameraMoveToCustomPositionSignal>().Dispatch(result, new global::Kampai.Util.Boxed<global::System.Action>(RestoreCamera));
			}
		}

		private void RestoreCamera()
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.EnableCameraBehaviourSignal>().Dispatch(1);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.EnableCameraBehaviourSignal>().Dispatch(2);
		}

		[global::Kampai.Util.DebugCommand]
		public void AddToTotemPole(string[] args)
		{
			foreach (global::Kampai.Game.CompositeBuildingPieceDefinition item in definitionService.GetAll<global::Kampai.Game.CompositeBuildingPieceDefinition>())
			{
				if (playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.CompositeBuildingPiece>(item.ID) != null)
				{
					continue;
				}
				playerService.CreateAndRunCustomTransaction(item.ID, 1, global::Kampai.Game.TransactionTarget.NO_VISUAL);
				global::Kampai.Game.CompositeBuilding firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.CompositeBuilding>(item.BuildingDefinitionID);
				compositeBuildingPieceAddedSignal.Dispatch(firstInstanceByDefinitionId);
				break;
			}
		}

		private global::System.Collections.IEnumerator UpdateDLCTier()
		{
			yield return new global::UnityEngine.WaitForSeconds(1f);
			playerDLCTierSignal.Dispatch();
		}

		private void ExpansionTransactionCallback(global::Kampai.Game.PendingCurrencyTransaction pct)
		{
			if (!pct.Success || pct.GetPendingTransaction().Outputs == null)
			{
				return;
			}
			foreach (global::Kampai.Util.QuantityItem output in pct.GetPendingTransaction().Outputs)
			{
				global::Kampai.Game.Definition definition = definitionService.Get<global::Kampai.Game.Definition>(output.ID);
				global::Kampai.Game.LandExpansionConfig landExpansionConfig = definition as global::Kampai.Game.LandExpansionConfig;
				if (landExpansionConfig != null)
				{
					purchaseSignal.Dispatch(landExpansionConfig.expansionId, true);
				}
			}
			routineRunner.StartCoroutine(UpdateDLCTier());
		}

		[global::Kampai.Util.DebugCommand]
		public void UM(string[] args)
		{
			playerService.AddUpsellToPurchased(50002);
			global::Kampai.Game.PackDefinition packDefinition = definitionService.Get<global::Kampai.Game.PackDefinition>(50002);
			global::Kampai.Game.Transaction.TransactionDefinition def = packDefinition.TransactionDefinition.ToDefinition();
			playerService.RunEntireTransaction(def, global::Kampai.Game.TransactionTarget.LAND_EXPANSION, ExpansionTransactionCallback);
			int num = 1;
			global::System.Collections.Generic.ICollection<int> animatingBuildingIDs = playerService.GetAnimatingBuildingIDs();
			foreach (int item in animatingBuildingIDs)
			{
				global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(item);
				global::Kampai.Game.MignetteBuilding mignetteBuilding = byInstanceId as global::Kampai.Game.MignetteBuilding;
				if (mignetteBuilding != null)
				{
					byInstanceId.SetState(global::Kampai.Game.BuildingState.Idle);
					if (mignetteBuilding.GetMinionSlotsOwned() > num)
					{
						num = mignetteBuilding.GetMinionSlotsOwned();
					}
					outBuilder.Append(mignetteBuilding.Definition.LocalizedKey + "(" + item + ") set to Idle\n");
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void SetCooldown(string[] args)
		{
			int result;
			if (!int.TryParse(args[1], out result))
			{
				return;
			}
			global::Kampai.Game.Building byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Building>(mignetteGameModel.BuildingId);
			global::Kampai.Game.MignetteBuilding mignetteBuilding = byInstanceId as global::Kampai.Game.MignetteBuilding;
			if (mignetteBuilding != null)
			{
				int cooldown = mignetteBuilding.GetCooldown();
				int num = cooldown - result;
				if ((float)num < 0f)
				{
					num = 0;
					result = cooldown;
				}
				timeEventService.RushEvent(byInstanceId.ID);
				byInstanceId.StateStartTime = timeService.CurrentTime() - num;
				byInstanceId.SetState(global::Kampai.Game.BuildingState.Cooldown);
				int num2 = mignetteBuilding.GetCooldown() / 10;
				for (int i = 0; i < 10; i++)
				{
					timeEventService.AddEvent(byInstanceId.ID, byInstanceId.StateStartTime, i * num2, cooldownUpdateViewSignal);
				}
				timeEventService.AddEvent(byInstanceId.ID, byInstanceId.StateStartTime, mignetteBuilding.GetCooldown(), cooldownCompleteSignal);
				outBuilder.Append("Altered building:" + byInstanceId.Definition.ID + " cooldown to " + result);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ShowSales(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Sale> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Sale>();
			foreach (global::Kampai.Game.Sale item in instancesByType)
			{
				outBuilder.AppendFormat("Sale ID {0} [{1}]: Started - {2} Finished - {3} Viewed - {4} Purchased - {5} TimeRemaining - {6} StartDate - {7} EndDate - {8}\n", item.ID, item.Definition.ID, item.Started, item.Finished, item.Viewed, item.Purchased, timeEventService.GetTimeRemaining(item.ID), item.Definition.UTCStartDate, item.Definition.UTCEndDate);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ResetPurchasedPack(string[] args)
		{
			int result = 0;
			if (!int.TryParse(args[1], out result))
			{
				Help(args);
			}
			else
			{
				playerService.ClearPurchasedUpsells(result);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void TriggerGacha(string[] args)
		{
			global::Kampai.Game.MinionState[] states = new global::Kampai.Game.MinionState[3]
			{
				global::Kampai.Game.MinionState.Idle,
				global::Kampai.Game.MinionState.Selectable,
				global::Kampai.Game.MinionState.WaitingOnMagnetFinger
			};
			global::System.Collections.Generic.List<global::Kampai.Game.Minion> minions = playerService.GetMinions(true, states);
			global::System.Collections.Generic.HashSet<int> hashSet = new global::System.Collections.Generic.HashSet<int>();
			for (int i = 0; i < 3; i++)
			{
				hashSet.Add(minions[i].ID);
			}
			global::Kampai.Util.Boxed<global::UnityEngine.Vector3> center = new global::Kampai.Util.Boxed<global::UnityEngine.Vector3>(new global::UnityEngine.Vector3(150f, 0f, 150f));
			global::Kampai.Game.MinionAnimationInstructions type = new global::Kampai.Game.MinionAnimationInstructions(hashSet, center);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.StartGroupGachaSignal>().Dispatch(type);
		}

		[global::Kampai.Util.DebugCommand]
		public void TsmTrigger(string[] args)
		{
			int result = 0;
			if (!int.TryParse(args[1], out result))
			{
				Help(args);
				return;
			}
			global::Kampai.UI.View.IGUIService instance = uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.IGUIService>();
			global::Kampai.UI.View.IGUICommand iGUICommand = instance.BuildCommand(global::Kampai.UI.View.GUIOperation.Queue, "popup_TSM_Gift_Upsell");
			iGUICommand.skrimScreen = "ProceduralTaskSkrim";
			iGUICommand.darkSkrim = true;
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerDefinition> triggerDefinitions = definitionService.GetTriggerDefinitions();
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggers = playerService.GetTriggers();
			global::Kampai.Game.Trigger.TriggerInstance triggerInstance = null;
			foreach (global::Kampai.Game.Trigger.TriggerInstance item in triggers)
			{
				if (item.ID == result)
				{
					triggerInstance = item;
					break;
				}
			}
			if (triggerInstance == null)
			{
				foreach (global::Kampai.Game.Trigger.TriggerDefinition item2 in triggerDefinitions)
				{
					if (item2.ID == result)
					{
						triggerInstance = item2.Build();
						break;
					}
				}
				if (triggerInstance == null)
				{
					Help(args);
					return;
				}
			}
			iGUICommand.Args.Add(typeof(global::Kampai.Game.Trigger.TriggerInstance), triggerInstance);
			instance.Execute(iGUICommand);
		}

		[global::Kampai.Util.DebugCommand]
		public void ShowUpsellModal(string[] args)
		{
			int result = 0;
			global::Kampai.Game.PackDefinition definition;
			if (!int.TryParse(args[1], out result))
			{
				Help(args);
			}
			else if (definitionService.TryGet<global::Kampai.Game.PackDefinition>(result, out definition))
			{
				openUpSellModalSignal.Dispatch(definition, "Upsell", false);
			}
			else
			{
				outBuilder.AppendLine("Invalid upsell or salepack id");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ResetTriggerReward(string[] args)
		{
			int result = 0;
			if (!int.TryParse(args[1], out result))
			{
				Help(args);
				return;
			}
			global::Kampai.UI.View.IGUIService instance = uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.IGUIService>();
			global::Kampai.UI.View.IGUICommand iGUICommand = instance.BuildCommand(global::Kampai.UI.View.GUIOperation.LoadUntrackedInstance, "popup_TSM_Gift_Upsell");
			iGUICommand.skrimScreen = "ProceduralTaskSkrim";
			iGUICommand.darkSkrim = true;
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggers = playerService.GetTriggers();
			global::Kampai.Game.Trigger.TriggerInstance triggerInstance = null;
			foreach (global::Kampai.Game.Trigger.TriggerInstance item in triggers)
			{
				if (item.ID == result)
				{
					triggerInstance = item;
					break;
				}
			}
			if (triggerInstance == null)
			{
				Help(args);
			}
			else
			{
				triggerInstance.RecievedRewardIds.Clear();
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "Duration", "Percent off", "[Optional]i|in {itemId quantity}...", "o|out {itemId quantity}...", "s|sku" }, RequiresAllArgs = false)]
		public void TriggerSales(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.SalePackDefinition> all = definitionService.GetAll<global::Kampai.Game.SalePackDefinition>();
			global::Kampai.Game.SalePackDefinition salePackDefinition = global::System.Linq.Enumerable.LastOrDefault(all);
			if (salePackDefinition == null)
			{
				return;
			}
			int result2;
			if (args.Length == 3)
			{
				int result;
				if (int.TryParse(args[1], out result) && int.TryParse(args[2], out result2))
				{
					global::Kampai.Game.SalePackDefinition salePackDefinition2 = definitionService.Get<global::Kampai.Game.SalePackDefinition>(result);
					if (salePackDefinition2 != null)
					{
						salePackDefinition2.UTCStartDate = timeService.CurrentTime() + 2;
						salePackDefinition2.UTCEndDate = timeService.CurrentTime() + 2 + result2;
						salePackDefinition2.Duration = result2;
						salePackDefinition2.ID = result + all.Count + timeService.AppTime();
					}
				}
				return;
			}
			bool flag = false;
			if (args.Length < 5)
			{
				return;
			}
			for (int i = 0; i < args.Length; i++)
			{
				if (!(args[i].ToLower() != "s") || !(args[i].ToLower() != "sku"))
				{
					flag = true;
					break;
				}
			}
			int result3;
			if (!int.TryParse(args[1], out result2) || !int.TryParse(args[2], out result3))
			{
				return;
			}
			int num = 3;
			global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem> list = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem> list2 = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
			int result4;
			uint result5;
			if ((!flag && num < args.Length && args[num].ToLower() == "i") || args[num].ToLower() == "in")
			{
				num++;
				outBuilder.AppendLine("Upsell inputs are:");
				while (num + 1 < args.Length && !(args[num].ToLower() == "o") && !(args[num].ToLower() == "out") && int.TryParse(args[num++], out result4) && uint.TryParse(args[num++], out result5))
				{
					list.Add(new global::Kampai.Util.QuantityItem(result4, result5));
					outBuilder.AppendLine(new global::Kampai.Util.QuantityItem(result4, result5).ToString());
				}
				if (list.Count == 0)
				{
					outBuilder.AppendLine("Invalid input Count, remove to make the upsell free");
					return;
				}
				outBuilder.AppendLine();
			}
			if ((num < args.Length && args[num].ToLower() == "o") || args[num].ToLower() == "out")
			{
				num++;
				outBuilder.AppendLine("Upsell outputs are:");
				while (num + 1 < args.Length && int.TryParse(args[num++], out result4) && uint.TryParse(args[num++], out result5))
				{
					list2.Add(new global::Kampai.Util.QuantityItem(result4, result5));
					outBuilder.AppendLine(new global::Kampai.Util.QuantityItem(result4, result5).ToString());
				}
				if (list2.Count == 0)
				{
					outBuilder.AppendLine("Invalid output Count");
					return;
				}
				outBuilder.AppendLine();
				global::Kampai.Game.SalePackDefinition salePackDefinition3 = new global::Kampai.Game.SalePackDefinition();
				salePackDefinition3.ID = salePackDefinition.ID + all.Count + timeService.AppTime();
				salePackDefinition3.Type = global::Kampai.Game.SalePackType.Upsell;
				salePackDefinition3.UTCStartDate = timeService.CurrentTime() + 2;
				salePackDefinition3.UTCEndDate = timeService.CurrentTime() + 2 + result2;
				salePackDefinition3.PercentagePer100 = result3;
				salePackDefinition3.Duration = result2;
				salePackDefinition3.BannerAd = "UpSellTimeLeft";
				salePackDefinition3.ExclusiveItemList = new global::System.Collections.Generic.List<int>(205);
				salePackDefinition3.CurrencyImageID = 0;
				global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
				if (flag)
				{
					salePackDefinition3.TransactionType = global::Kampai.Game.UpsellTransactionType.Cash;
					salePackDefinition3.LocalizedKey = "UpSellTitle";
					salePackDefinition3.Description = "MTXUpSellHeadline";
					salePackDefinition3.PlatformStoreSku = new global::System.Collections.Generic.List<global::Kampai.Game.PlatformStoreSkuDefinition>();
					global::Kampai.Game.PlatformStoreSkuDefinition platformStoreSkuDefinition = new global::Kampai.Game.PlatformStoreSkuDefinition();
					platformStoreSkuDefinition.appleAppstore = "com.ea.mtx.870581";
					platformStoreSkuDefinition.googlePlay = "870582";
					platformStoreSkuDefinition.defaultStore = "SKU_STARTER_PACK";
					global::Kampai.Game.PlatformStoreSkuDefinition item = platformStoreSkuDefinition;
					salePackDefinition3.PlatformStoreSku.Add(item);
					salePackDefinition3.ActiveSKUIndex = 0;
				}
				else if (list.Count >= 1)
				{
					salePackDefinition3.TransactionType = ((result3 <= 0) ? global::Kampai.Game.UpsellTransactionType.ItemPurchase : global::Kampai.Game.UpsellTransactionType.ItemDiscount);
				}
				else
				{
					salePackDefinition3.BannerAd = "Gift";
					salePackDefinition3.TransactionType = global::Kampai.Game.UpsellTransactionType.Free;
				}
				transactionDefinition.Inputs = list;
				transactionDefinition.Outputs = list2;
				salePackDefinition3.TransactionDefinition = transactionDefinition.ToInstance();
				playerService.AssignNextInstanceId(salePackDefinition3.TransactionDefinition);
				definitionService.Add(salePackDefinition3);
				definitionService.Add(salePackDefinition3.TransactionDefinition.ToDefinition());
				reconcileSalesSignal.Dispatch(0);
			}
			else
			{
				outBuilder.AppendLine("Missing output variables");
				FlushSignal.Dispatch();
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void TriggerSales2(string[] args)
		{
			int result;
			int result2;
			if (int.TryParse(args[1], out result) && int.TryParse(args[2], out result2))
			{
				global::Kampai.Game.SalePackDefinition definition;
				if (!definitionService.TryGet<global::Kampai.Game.SalePackDefinition>(result, out definition))
				{
					global::UnityEngine.Debug.LogError("Unable to find: " + result);
				}
				if (definition != null)
				{
					global::Kampai.Game.SalePackDefinition salePackDefinition = definition;
					salePackDefinition.ID = definition.ID + timeService.AppTime();
					salePackDefinition.Type = global::Kampai.Game.SalePackType.Upsell;
					int num = 2;
					salePackDefinition.UTCStartDate = timeService.CurrentTime() + num;
					salePackDefinition.UTCEndDate = timeService.CurrentTime() + num + result2;
					salePackDefinition.Duration = result2;
					playerService.AssignNextInstanceId(salePackDefinition.TransactionDefinition);
					definitionService.Add(salePackDefinition);
					definitionService.Add(salePackDefinition.TransactionDefinition.ToDefinition());
					reconcileSalesSignal.Dispatch(0);
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ReconcileSales(string[] args)
		{
			reconcileSalesSignal.Dispatch(0);
		}

		[global::Kampai.Util.DebugCommand]
		public void TestAppStoreOpen(string[] args)
		{
			string text = "com.ea.gp.pets";
			global::Kampai.Util.Native.OpenAppStoreLink("market://details?id=" + text);
		}

		[global::Kampai.Util.DebugCommand]
		public void TriggerRedemptionSale(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				global::Kampai.Game.SalePackDefinition salePackDefinition = definitionService.Get<global::Kampai.Game.SalePackDefinition>(result);
				if (salePackDefinition != null)
				{
					global::Kampai.Game.Mtx.ReceiptValidationResult validationResult = new global::Kampai.Game.Mtx.ReceiptValidationResult(salePackDefinition.SKU, string.Empty, string.Empty, global::Kampai.Game.Mtx.ReceiptValidationResult.Code.SUCCESS);
					playerService.addPendingRemption(validationResult);
					openUpSellModalSignal.Dispatch(salePackDefinition, "REDEMPTION", true);
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void BuyStarterPack(string[] args)
		{
			global::Kampai.Game.SalePackDefinition salePackDefinition = definitionService.Get<global::Kampai.Game.SalePackDefinition>(50001);
			global::Kampai.Game.TransactionArg transactionArg = new global::Kampai.Game.TransactionArg();
			transactionArg.IsFromPremiumSource = true;
			playerService.RunEntireTransaction(salePackDefinition.TransactionDefinition.ToDefinition(), global::Kampai.Game.TransactionTarget.CURRENCY, null, transactionArg);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.UnlockMinionsSignal>().Dispatch();
			global::Kampai.Game.Sale firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.Sale>(salePackDefinition.ID);
			firstInstanceByDefinitionId.Purchased = true;
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.EndSaleSignal>().Dispatch(firstInstanceByDefinitionId.ID);
		}

		[global::Kampai.Util.DebugCommand]
		public void RemoveSale(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.EndSaleSignal>().Dispatch(result);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ResetRateMyApp(string[] args)
		{
			localPersistService.PutDataPlayer("RateApp", "Enabled");
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShouldRateAppSignal>().Dispatch(global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent.LevelUp);
		}

		[global::Kampai.Util.DebugCommand]
		public void TestRateMyApp(string[] args)
		{
			string dataPlayer = localPersistService.GetDataPlayer("RateApp");
			if (!(dataPlayer == "Disabled"))
			{
				uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShouldRateAppSignal>().Dispatch(global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent.LevelUp);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void SpeedUpTimers(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				result *= 60;
				timeEventService.SpeedUpTimers(result);
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "multiplier|reset" })]
		public void ScaleTimers(string[] args)
		{
			if (args.Length > 1)
			{
				if (args[1].ToLower().Equals("reset"))
				{
					timeEventService.TimerScale = 0f;
					outBuilder.Append("Time scaling reset. Will take effect the next time an event is created.");
					return;
				}
				float result = 0f;
				if (float.TryParse(args[1], out result) && (double)result > 0.01)
				{
					timeEventService.TimerScale = result;
					outBuilder.AppendFormat("All new event timers will be scaled by {0}", result);
				}
				else
				{
					outBuilder.AppendFormat("The multiplier {0} is not a real number greater than 0.01", args[1]);
				}
			}
			else
			{
				outBuilder.Append("usage: \"scaletimers <multiplier>\" where <multiplier> is a real number greater than 0.01");
				outBuilder.Append("You must set the scale before creating a new event. Use \"scaletimers reset\" to reset.");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void SpeedTime(string[] args)
		{
			if (args.Length < 2)
			{
				global::UnityEngine.Time.timeScale = 1f;
				outBuilder.AppendLine("Invalid time argument, reseting to default speed of 1.0");
				return;
			}
			float result = 1f;
			if (float.TryParse(args[1], out result))
			{
				global::UnityEngine.Time.timeScale = result;
				return;
			}
			global::UnityEngine.Time.timeScale = 1f;
			outBuilder.AppendLine("Invalid time argument, reseting to default speed of 1.0");
		}

		[global::Kampai.Util.DebugCommand]
		public void TriggerConfirmation(string[] args)
		{
			global::strange.extensions.signal.impl.Signal<bool> signal = new global::strange.extensions.signal.impl.Signal<bool>();
			signal.AddListener(delegate(bool result)
			{
				outBuilder.Append("User Selected: " + result);
				FlushSignal.Dispatch();
			});
			global::Kampai.UI.View.PopupConfirmationSetting type = new global::Kampai.UI.View.PopupConfirmationSetting("popupConfirmationDefaultTitle", "popupConfirmationDefaultTitle", "img_char_Min_FeedbackChecklist01", signal);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.QueueConfirmationSignal>().Dispatch(type);
		}

		[global::Kampai.Util.DebugCommand]
		public void SMTM(string[] args)
		{
			playerService.CreateAndRunCustomTransaction(0, 90000, global::Kampai.Game.TransactionTarget.CURRENCY);
			playerService.CreateAndRunCustomTransaction(1, 90000, global::Kampai.Game.TransactionTarget.CURRENCY);
		}

		[global::Kampai.Util.DebugCommand]
		public void ST(string[] args)
		{
			global::Kampai.Game.Trigger.TriggerInstance activeTrigger = gameContext.injectionBinder.GetInstance<global::Kampai.Game.ITriggerService>().ActiveTrigger;
			if (activeTrigger != null)
			{
				outBuilder.AppendLine("ID: " + activeTrigger.Definition.ID + " " + activeTrigger.Definition.ToString());
			}
			else
			{
				outBuilder.AppendLine("No trigger exists");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void TriggerState(string[] args)
		{
			int result;
			if (args.Length > 1 && int.TryParse(args[1], out result))
			{
				global::Kampai.Game.Trigger.TriggerDefinition triggerDefinition = definitionService.Get<global::Kampai.Game.Trigger.TriggerDefinition>(result);
				ShowTriggerState(triggerDefinition);
				return;
			}
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggers = playerService.GetTriggers();
			global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerDefinition> all = definitionService.GetAll<global::Kampai.Game.Trigger.TriggerDefinition>();
			foreach (global::Kampai.Game.Trigger.TriggerDefinition item in all)
			{
				if (IsTriggerAboutToActivate(item, triggers))
				{
					ShowTriggerState(item);
				}
			}
		}

		private bool IsTriggerAboutToActivate(global::Kampai.Game.Trigger.TriggerDefinition triggerDef, global::System.Collections.Generic.IList<global::Kampai.Game.Trigger.TriggerInstance> triggerInstances)
		{
			if (triggerDef.IsTriggered(gameContext))
			{
				foreach (global::Kampai.Game.Trigger.TriggerInstance triggerInstance in triggerInstances)
				{
					if (triggerInstance.Definition.ID == triggerDef.ID && triggerInstance.StartGameTime > 0)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private void ShowTriggerState(global::Kampai.Game.Trigger.TriggerDefinition triggerDefinition)
		{
			outBuilder.AppendLine("ID: " + triggerDefinition.ID.ToString() + " Priority: " + triggerDefinition.priority + " CanTrigger: " + triggerDefinition.IsTriggered(gameContext));
			triggerDefinition.PrintTriggerConditions(gameContext, outBuilder);
		}

		[global::Kampai.Util.DebugCommand(Name = "dcn", Args = new string[] { "featured" })]
		public void DCN(string[] args)
		{
			switch (args[1])
			{
			case "featured":
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.DCNFeaturedSignal>().Dispatch();
				break;
			case "reset":
				localPersistService.DeleteKey("DCNStore");
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.DCNMaybeShowContentSignal>().Dispatch();
				break;
			case "show":
				localPersistService.DeleteKey("DCNStoreDoNotShow");
				break;
			case "fake":
			{
				localPersistService.DeleteKey("DCNStore");
				global::Kampai.Game.DCNService dCNService = gameContext.injectionBinder.GetInstance<global::Kampai.Game.IDCNService>() as global::Kampai.Game.DCNService;
				dCNService.SetFeaturedContent(1, "http://www.ea.com/");
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.DCNMaybeShowContentSignal>().Dispatch();
				break;
			}
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "minion ID", "level" }, RequiresAllArgs = false)]
		public void LevelMinion(string[] args)
		{
			try
			{
				if (args.Length == 2)
				{
					int id = global::System.Convert.ToInt32(args[1]);
					global::Kampai.Game.Minion byInstanceId = playerService.GetByInstanceId<global::Kampai.Game.Minion>(id);
					outBuilder.Append(string.Format("Minion: {0} Level: {1}", byInstanceId.ID, byInstanceId.Level));
				}
				else if (args.Length == 3)
				{
					global::Kampai.Game.MinionBenefitLevelBandDefintion minionBenefitLevelBandDefintion = definitionService.Get<global::Kampai.Game.MinionBenefitLevelBandDefintion>(89898);
					int num = global::System.Convert.ToInt32(args[1]);
					global::Kampai.Game.Minion byInstanceId2 = playerService.GetByInstanceId<global::Kampai.Game.Minion>(num);
					byInstanceId2.Level = global::UnityEngine.Mathf.Clamp(global::System.Convert.ToInt32(args[2]), 0, minionBenefitLevelBandDefintion.minionBenefitLevelBands.Count - 1);
					outBuilder.Append(string.Format("Minion: {0} Level: {1}", byInstanceId2.ID, byInstanceId2.Level));
					byInstanceId2.Level--;
					gameContext.injectionBinder.GetInstance<global::Kampai.Game.MinionUpgradeSignal>().Dispatch(num, 0u);
				}
			}
			catch (global::System.Exception ex)
			{
				logger.Error(ex.Message);
				outBuilder.Append("Error: [" + args[0] + "]: " + ex.Message);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "inventory", Args = new string[] { "typename" }, RequiresAllArgs = true)]
		public void Inventory(string[] args)
		{
			if (args[1] == "*")
			{
				args[1] = "Instance";
			}
			global::System.Type type = null;
			global::System.Type[] types = global::System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			foreach (global::System.Type type2 in types)
			{
				if (type2.Name.ToLower() == args[1].ToLower())
				{
					type = type2;
					break;
				}
			}
			if (type == null)
			{
				outBuilder.AppendFormat("Type {0} not found", args[1]);
				return;
			}
			global::System.Reflection.MethodInfo methodInfo = global::System.Linq.Enumerable.First(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(playerService.GetType().GetMethods(), (global::System.Reflection.MethodInfo m) => m.Name == "GetInstancesByType"), (global::System.Reflection.MethodInfo m) => new
			{
				Method = m,
				Params = m.GetParameters(),
				Args = m.GetGenericArguments()
			}), x => x.Params.Length == 0 && x.Args.Length == 1), x => x.Method));
			global::System.Reflection.MethodInfo methodInfo2 = methodInfo.MakeGenericMethod(type);
			if (methodInfo2 == null)
			{
				outBuilder.AppendFormat("Method GetInstancesByType<{0}> not found", type);
				return;
			}
			global::System.Collections.IList list = methodInfo2.Invoke(playerService, null) as global::System.Collections.IList;
			foreach (object item2 in list)
			{
				global::Kampai.Game.Instance instance = item2 as global::Kampai.Game.Instance;
				global::Kampai.Game.Item item = item2 as global::Kampai.Game.Item;
				if (item != null)
				{
					outBuilder.AppendFormat("{0} [{1}] <{2}>: {3} {4}\n", item.ID, item.Definition.ID, item.Definition.LocalizedKey, item.ToString(), item.Quantity);
				}
				else if (instance != null)
				{
					outBuilder.AppendFormat("{0} [{1}] <{2}>: {3}\n", instance.ID, instance.Definition.ID, instance.Definition.LocalizedKey, instance.ToString());
				}
				else
				{
					outBuilder.AppendFormat("{0}\n", item2.ToString());
				}
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "get item")]
		public void GetItem(string[] args)
		{
			int result = 0;
			if (!int.TryParse(args[2], out result) && !IdOf(args[2], out result))
			{
				outBuilder.AppendLine(string.Join(" ", args) + " -> DEFINITION NOT FOUND");
				return;
			}
			foreach (global::Kampai.Game.Instance item2 in playerService.GetInstancesByDefinitionID(result))
			{
				global::Kampai.Game.Item item = item2 as global::Kampai.Game.Item;
				if (item != null)
				{
					outBuilder.AppendFormat("{0} [{1}] <{2}>: {3} {4}\n", item.ID, item.Definition.ID, item.Definition.LocalizedKey, item.ToString(), item.Quantity);
				}
				else if (item2 != null)
				{
					outBuilder.AppendFormat("{0} [{1}] <{2}>: {3}\n", item2.ID, item2.Definition.ID, item2.Definition.LocalizedKey, item2.ToString());
				}
				else
				{
					outBuilder.AppendLine(string.Join(" ", args) + " -> ITEM NOT FOUND");
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void StuartShow(string[] args)
		{
			socialOrderBoardCompleteSignal.Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void AddPhilToTikibar(string[] args)
		{
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.PhilSitAtBarSignal>().Dispatch(true);
		}

		[global::Kampai.Util.DebugCommand]
		public void EnableBuildMenu(string[] args)
		{
			bool type = args.Length <= 1 || !(args[1] == "off");
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.SetBuildMenuEnabledSignal>().Dispatch(type);
		}

		[global::Kampai.Util.DebugCommand]
		public void InitPN(string[] args)
		{
			setupPushNotificationsSignal.Dispatch();
		}

		[global::Kampai.Util.DebugCommand(Name = "pgtcooldown", Args = new string[] { "seconds" }, RequiresAllArgs = true)]
		public void pgtcooldown(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result) && result > 0)
			{
				global::Kampai.Game.TSMCharacter firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.TSMCharacter>(70008);
				firstInstanceByDefinitionId.Definition.CooldownInSeconds = result;
				outBuilder.Append("Altered travelling sales minion's cooldown to " + result);
			}
			else
			{
				outBuilder.Append("Bad cooldown: " + args[1]);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void CompleteQuest(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				global::Kampai.Game.QuestDefinition def = definitionService.Get<global::Kampai.Game.QuestDefinition>(result);
				global::Kampai.Game.Quest quest = new global::Kampai.Game.Quest(def);
				quest.Initialize();
				quest.state = global::Kampai.Game.QuestState.Complete;
				questService.AddQuest(quest);
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.GetNewQuestSignal>().Dispatch();
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void FinishQuest(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				global::Kampai.Game.IQuestController questController = questService.GetQuestMap()[result];
				questController.GoToQuestState(global::Kampai.Game.QuestState.Harvestable);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void FinishQuests(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Quest> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Quest>();
			foreach (global::Kampai.Game.Quest item in instancesByType)
			{
				if (item.state == global::Kampai.Game.QuestState.RunningTasks)
				{
					global::Kampai.Game.IQuestController questController = questService.GetQuestMap()[item.GetActiveDefinition().ID];
					questController.GoToQuestState(global::Kampai.Game.QuestState.Harvestable);
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void FinishTasks(string[] args)
		{
			global::Kampai.Game.MasterPlan currentMasterPlan = masterPlanService.CurrentMasterPlan;
			if (currentMasterPlan != null)
			{
				global::Kampai.Game.MasterPlanComponent activeComponentFromPlanDefinition = masterPlanService.GetActiveComponentFromPlanDefinition(currentMasterPlan.Definition.ID);
				if (activeComponentFromPlanDefinition != null && activeComponentFromPlanDefinition.State == global::Kampai.Game.MasterPlanComponentState.InProgress)
				{
					FinishTasks(activeComponentFromPlanDefinition);
				}
			}
		}

		private void FinishTasks(global::Kampai.Game.MasterPlanComponent component)
		{
			foreach (global::Kampai.Game.MasterPlanComponentTask task in component.tasks)
			{
				task.isComplete = true;
			}
			component.State = global::Kampai.Game.MasterPlanComponentState.TasksComplete;
			global::Kampai.Game.Quest questByInstanceId = masterPlanQuestService.GetQuestByInstanceId(component.ID);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.UpdateQuestWorldIconsSignal>().Dispatch(questByInstanceId);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.MasterPlanComponentTaskUpdatedSignal>().Dispatch(component);
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.CloseQuestBookSignal>().Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void CompleteComponents(string[] args)
		{
			global::Kampai.Game.MasterPlan currentMasterPlan = masterPlanService.CurrentMasterPlan;
			if (currentMasterPlan != null)
			{
				global::System.Collections.Generic.IList<global::Kampai.Game.Instance> instancesByDefinition = playerService.GetInstancesByDefinition<global::Kampai.Game.MasterPlanComponentDefinition>();
				for (int i = 0; i < instancesByDefinition.Count; i++)
				{
					global::Kampai.Game.MasterPlanComponent masterPlanComponent = instancesByDefinition[i] as global::Kampai.Game.MasterPlanComponent;
					if (masterPlanComponent.State == global::Kampai.Game.MasterPlanComponentState.InProgress)
					{
						FinishTasks(masterPlanComponent);
						continue;
					}
					foreach (global::Kampai.Game.MasterPlanComponentTask task in masterPlanComponent.tasks)
					{
						task.isComplete = true;
					}
					masterPlanComponent.State = global::Kampai.Game.MasterPlanComponentState.Complete;
				}
				if (instancesByDefinition.Count > 0)
				{
					outBuilder.AppendLine(string.Format("Components Complete for plan {0}", currentMasterPlan.Definition.ID));
					return;
				}
			}
			outBuilder.AppendLine("Error in completing components.  Is the current master plan null?  Or are components null?");
		}

		[global::Kampai.Util.DebugCommand]
		public void NextMP(string[] args)
		{
			int result;
			if (int.TryParse(args[1], out result))
			{
				if (masterPlanService.ForceNextMPDefinition(result))
				{
					outBuilder.AppendLine("SUCCESS: the next master plan will be from definition " + result);
					outBuilder.AppendLine("Note that this will NOT affect the playlist order: it will resume at the completion of this master plan.");
				}
				else
				{
					outBuilder.AppendLine("FAILURE: " + result + " is not a valid MasterPlanDefinition ID.  No action taken.");
				}
			}
			else
			{
				outBuilder.AppendLine("FAILURE: invalid input.  Please supply a valid Master Plan Definition ID.  No action taken.");
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void MPInfo(string[] args)
		{
			outBuilder.AppendLine("----------Current Master Plan Information:----------");
			global::Kampai.Game.MasterPlan currentMasterPlan = masterPlanService.CurrentMasterPlan;
			global::Kampai.Game.VillainLairDefinition villainLairDefinition = definitionService.Get<global::Kampai.Game.VillainLairDefinition>(global::Kampai.Game.StaticItem.VILLAIN_LAIR_DEFINITION_ID);
			if (currentMasterPlan == null)
			{
				outBuilder.AppendLine("\tNo master plan exists.");
				outBuilder.AppendLine("----------End of Current Plan Information.----------");
				return;
			}
			outBuilder.AppendLine(string.Format("\tMasterPlanDefinition ID: {0}\n\tReward transaction ID: {1}", currentMasterPlan.Definition.ID, currentMasterPlan.Definition.RewardTransactionID));
			for (int i = 0; i < currentMasterPlan.Definition.ComponentDefinitionIDs.Count; i++)
			{
				global::Kampai.Game.MasterPlanComponent firstInstanceByDefinitionId = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.MasterPlanComponent>(currentMasterPlan.Definition.ComponentDefinitionIDs[i]);
				global::Kampai.Game.PlatformDefinition platformDefinition = villainLairDefinition.Platforms[i];
				if (firstInstanceByDefinitionId != null)
				{
					outBuilder.AppendLine(string.Format("\tComponent[{0}] defID: {1} state: {2}", i, currentMasterPlan.Definition.ComponentDefinitionIDs[i], firstInstanceByDefinitionId.State));
					int num = currentMasterPlan.Definition.CompBuildingDefinitionIDs[i];
					global::Kampai.Game.MasterPlanComponentBuilding firstInstanceByDefinitionId2 = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.MasterPlanComponentBuilding>(num);
					if (firstInstanceByDefinitionId2 != null)
					{
						outBuilder.AppendLine(string.Format("\t\tBuilding ID {0} in state:{1},\n\t\tbuilding prefab: {2}", num, firstInstanceByDefinitionId2.State, firstInstanceByDefinitionId2.Definition.Prefab));
					}
					else
					{
						outBuilder.AppendLine(string.Format("\t\tBuilding def id {0} is not yet built.", num));
						outBuilder.AppendLine(string.Format("\t\tbuilding prefab: {0}", definitionService.Get<global::Kampai.Game.MasterPlanComponentBuildingDefinition>().Prefab));
					}
					outBuilder.AppendLine(string.Format("\t\tPlatform base location is {0},{1}, offset is {2}, with final position: {3}", platformDefinition.placementLocation.x, platformDefinition.placementLocation.y, platformDefinition.offset, masterPlanService.GetComponentBuildingPosition(num)));
				}
				else
				{
					outBuilder.AppendLine(string.Format("\tComponent[{0}]: defID: {1} is null.", i, currentMasterPlan.Definition.ComponentDefinitionIDs[i]));
				}
			}
			global::Kampai.Game.PlatformDefinition platformDefinition2 = villainLairDefinition.Platforms[villainLairDefinition.Platforms.Count - 1];
			int buildingDefID = currentMasterPlan.Definition.BuildingDefID;
			global::Kampai.Game.MasterPlanComponentBuilding firstInstanceByDefinitionId3 = playerService.GetFirstInstanceByDefinitionId<global::Kampai.Game.MasterPlanComponentBuilding>(buildingDefID);
			if (firstInstanceByDefinitionId3 != null)
			{
				outBuilder.AppendLine(string.Format("\tMasterPlan Building id {0} is in state {1}", firstInstanceByDefinitionId3.Definition.ID, firstInstanceByDefinitionId3.State));
			}
			else
			{
				outBuilder.AppendLine(string.Format("\tMasterPlan Buiding id {0} is not yet built.", buildingDefID));
			}
			outBuilder.AppendLine(string.Format("\t\tMasterPlan building base location is {0},{1}, offset is {2}, with final position: {3}", platformDefinition2.placementLocation.x, platformDefinition2.placementLocation.y, platformDefinition2.offset, masterPlanService.GetComponentBuildingPosition(buildingDefID)));
			global::Kampai.Game.BuildingDefinition buildingDefinition = definitionService.Get<global::Kampai.Game.BuildingDefinition>(currentMasterPlan.Definition.LeavebehindBuildingDefID);
			outBuilder.AppendLine(string.Format("\t\tLeaveBehind is {0} buildingID: {1}", localizationService.GetString(buildingDefinition.LocalizedKey), currentMasterPlan.Definition.LeavebehindBuildingDefID));
			outBuilder.AppendLine(string.Format("\n----------End of Info for Current Plan {0} ----------", currentMasterPlan.Definition.ID));
		}

		[global::Kampai.Util.DebugCommand]
		public void BackButton(string[] args)
		{
			global::Kampai.UI.View.UIModel instance = uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.UIModel>();
			if (instance.UIOpen)
			{
				global::System.Action action = instance.RemoveTopUI();
				if (action != null)
				{
					action();
				}
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ShowQuests(string[] args)
		{
			global::System.Collections.Generic.List<global::Kampai.Game.Quest> instancesByType = playerService.GetInstancesByType<global::Kampai.Game.Quest>();
			foreach (global::Kampai.Game.Quest item in instancesByType)
			{
				outBuilder.AppendFormat("Quest ID {0}: State - {1} DefID - {2} LineID - {3} SurfaceType - {4} SurfaceID - {5}\n", item.ID, item.state.ToString(), item.GetActiveDefinition().ID, item.GetActiveDefinition().QuestLineID, item.GetActiveDefinition().SurfaceType.ToString(), item.GetActiveDefinition().SurfaceID);
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ShowQuestLines(string[] args)
		{
			global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.QuestLine> questLines = questService.GetQuestLines();
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.QuestLine> item in questLines)
			{
				global::Kampai.Game.QuestLine value = item.Value;
				outBuilder.AppendFormat("First Quest Id - {0} Last QuestId - {1} State - {2} \n", value.Quests[value.Quests.Count - 1].ID, value.Quests[0].ID, value.state.ToString());
			}
		}

		[global::Kampai.Util.DebugCommand]
		public void ShowQuestDebug(string[] args)
		{
			EnableQuestDebugSignal.Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void ToggleRight(string[] args)
		{
			ToggleRightClickSignal.Dispatch();
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "delay" })]
		public void Disconnect(string[] args)
		{
			float result = 0f;
			if (args.Length > 1 && !float.TryParse(args[1], out result))
			{
				result = 0f;
			}
			routineRunner.StartTimer("DebugDisconnectID", result, networkConnectionLostSignal.Dispatch);
		}

		[global::Kampai.Util.DebugCommand]
		public void ResetMignetteScore(string[] args)
		{
			mignetteCollectionService.ResetMignetteProgress();
		}

		[global::Kampai.Util.DebugCommand(Name = "showhud", Args = new string[] { "on/off" }, RequiresAllArgs = true)]
		public void ShowHUD(string[] args)
		{
			bool type = args[1] == "on";
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.ShowHUDSignal>().Dispatch(type);
		}

		[global::Kampai.Util.DebugCommand]
		public void HideWayfinders(string[] args)
		{
			uiContext.injectionBinder.GetInstance<global::Kampai.UI.View.HideAllWayFindersSignal>().Dispatch();
		}

		[global::Kampai.Util.DebugCommand]
		public void RandomFlyOver(string[] args)
		{
			int result = 0;
			if (args.Length > 1)
			{
				int.TryParse(args[1], out result);
			}
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.RandomFlyOverSignal>().Dispatch(result);
		}

		[global::Kampai.Util.DebugCommand]
		public void ClearVideo(string[] args)
		{
			global::UnityEngine.PlayerPrefs.SetString("VideoCache", "INVALID");
			global::UnityEngine.PlayerPrefs.SetInt("intro_video_played", 0);
		}

		[global::Kampai.Util.DebugCommand]
		public void ToggleSellTimers(string[] args)
		{
			marketplaceService.IsDebugMode = !marketplaceService.IsDebugMode;
		}

		[global::Kampai.Util.DebugCommand]
		public void MarketplaceMultiplier(string[] args)
		{
			float result = 1f;
			if (args.Length > 1)
			{
				float.TryParse(args[1], out result);
			}
			marketplaceService.DebugMultiplier = result;
			outBuilder.Append("Marketplace Multiplier set to " + result);
		}

		[global::Kampai.Util.DebugCommand]
		public void MPSelect(string[] args)
		{
			if (args.Length > 1)
			{
				if (args[1].ToLower().Equals("reset"))
				{
					marketplaceService.DebugSelectedItem = 0;
					outBuilder.Append("Marketplace Selected Item reset to normal");
					return;
				}
				int result = 0;
				int.TryParse(args[1], out result);
				global::Kampai.Game.Definition definition;
				if (definitionService.TryGet<global::Kampai.Game.Definition>(result, out definition))
				{
					marketplaceService.DebugSelectedItem = result;
					RefreshSaleItemsSignalArgs refreshSaleItemsSignalArgs = new RefreshSaleItemsSignalArgs();
					refreshSaleItemsSignalArgs.RefreshItems = true;
					refreshSaleItemsSignalArgs.StopSpinning = false;
					RefreshSaleItemsSignalArgs type = refreshSaleItemsSignalArgs;
					gameContext.injectionBinder.GetInstance<global::Kampai.Game.RefreshSaleItemsSignal>().Dispatch(type);
					outBuilder.Append("Marketplace Selected Item set to " + result);
				}
				else
				{
					outBuilder.Append("Item with given id not found: " + result);
				}
			}
			else
			{
				outBuilder.Append("usage: \"mpselect 12345\" to select item 12345 or \"mpselect reset\" to work normally");
			}
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "on/off" })]
		public void MPFacebook(string[] args)
		{
			if (args.Length > 1 && (args[1].Equals("on") || args[1].Equals("off")))
			{
				marketplaceService.DebugFacebook = args[1].Equals("on");
				outBuilder.Append(string.Format("Marketplace Discord features turned {0}", (!marketplaceService.DebugFacebook) ? "off" : "on"));
				gameContext.injectionBinder.GetInstance<global::Kampai.Game.UpdateMarketplaceSlotStateSignal>().Dispatch();
			}
			else
			{
				outBuilder.Append("usage: \"mpfacebook <on|off>\" to switch discord features on/off");
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "killswitch", Args = new string[] { "clear", "[type]", "[type] on/off" }, RequiresAllArgs = false)]
		public void Killswitch(string[] args)
		{
			if (args.Length > 1)
			{
				if (args[1].ToUpper() == "CLEAR")
				{
					configurationsService.ClearAllKillswitchOverrides();
					outBuilder.Append("CLEARED All killswitch overrides.");
					return;
				}
				try
				{
					global::Kampai.Game.KillSwitch killSwitch = (global::Kampai.Game.KillSwitch)(int)global::System.Enum.Parse(typeof(global::Kampai.Game.KillSwitch), args[1].ToUpper());
					if (args.Length > 2)
					{
						switch (args[2].ToLower())
						{
						case "on":
							configurationsService.OverrideKillswitch(killSwitch, true);
							break;
						case "off":
							configurationsService.OverrideKillswitch(killSwitch, false);
							break;
						}
					}
					outBuilder.Append(killSwitch.ToString() + " killswitch state:" + configurationsService.isKillSwitchOn(killSwitch));
					return;
				}
				catch (global::System.Exception ex)
				{
					logger.Info(ex.ToString());
				}
			}
			outBuilder.Append("Invalid killswitch type in second param. Valid types: ");
			foreach (int value in global::System.Enum.GetValues(typeof(global::Kampai.Game.KillSwitch)))
			{
				outBuilder.Append(((global::Kampai.Game.KillSwitch)value).ToString() + ", ");
			}
			outBuilder.Append(" Enter 'killswitch type on/off' to set an override, or 'killswitch clear' to disable all overrides.");
		}

		[global::Kampai.Util.DebugCommand(Name = "clone", Args = new string[] { "User ID" }, RequiresAllArgs = true)]
		public void Clone(string[] args)
		{
			try
			{
				long userId = global::System.Convert.ToInt64(args[1]);
				savePlayerSignal.Dispatch(new global::Kampai.Util.Tuple<global::Kampai.Game.SaveLocation, string, bool>(global::Kampai.Game.SaveLocation.REMOTE, string.Empty, true));
				routineRunner.StartCoroutine(ClonePlayer(userId));
			}
			catch (global::System.Exception ex)
			{
				logger.Error(ex.Message);
				outBuilder.Append("Error: [" + args[0] + "]: " + ex.Message);
			}
		}

		[global::Kampai.Util.DebugCommand(Name = "liberate", Args = new string[] { "Minimum Level" }, RequiresAllArgs = false)]
		public void LiberateLeveledUpMinions(string[] args)
		{
			int type = 1;
			if (args.Length > 1)
			{
				type = int.Parse(args[1]);
			}
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.LiberateLeveledUpMinionsSignal>().Dispatch(type);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "Order ID" }, RequiresAllArgs = false)]
		public void SocialOrder(string[] args)
		{
			if (args.Length > 1)
			{
				int result;
				if (int.TryParse(args[1], out result))
				{
					global::Kampai.Game.Trigger.SocialOrderTriggerRewardDefinition socialOrderTriggerRewardDefinition = new global::Kampai.Game.Trigger.SocialOrderTriggerRewardDefinition();
					socialOrderTriggerRewardDefinition.OrderId = result;
					socialOrderTriggerRewardDefinition.RewardPlayer(gameContext);
				}
				else
				{
					outBuilder.Append("Bad order ID: " + args[1]);
				}
				return;
			}
			global::Kampai.Game.TimedSocialEventDefinition currentSocialEvent = timedSocialEventService.GetCurrentSocialEvent();
			if (currentSocialEvent == null)
			{
				outBuilder.Append("No social order available.\n");
				return;
			}
			global::Kampai.Game.SocialTeamResponse socialEventStateCached = timedSocialEventService.GetSocialEventStateCached(currentSocialEvent.ID);
			if (socialEventStateCached == null || socialEventStateCached.Team == null)
			{
				outBuilder.Append("No social team available.\n");
				return;
			}
			foreach (global::Kampai.Game.SocialEventOrderDefinition order in currentSocialEvent.Orders)
			{
				string text = null;
				foreach (global::Kampai.Game.SocialOrderProgress item in socialEventStateCached.Team.OrderProgress)
				{
					if (item.OrderId == order.OrderID)
					{
						text = item.CompletedByUserId;
						break;
					}
				}
				outBuilder.Append(string.Format("Order {0} : {1}\n", order.OrderID, (!string.IsNullOrEmpty(text)) ? ("COMPLETED BY " + text) : "OPEN"));
			}
		}

		private global::System.Collections.IEnumerator ClonePlayer(long userId)
		{
			yield return new global::UnityEngine.WaitForSeconds(1f);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.CloneUserSignal>().Dispatch(userId);
		}

		[global::Kampai.Util.DebugCommand(Args = new string[] { "Source Environment", "User ID" }, RequiresAllArgs = true)]
		public void CloneFrom(string[] args)
		{
			try
			{
				string sourceEnvironment = args[1].ToLower();
				long userId = global::System.Convert.ToInt64(args[2]);
				savePlayerSignal.Dispatch(new global::Kampai.Util.Tuple<global::Kampai.Game.SaveLocation, string, bool>(global::Kampai.Game.SaveLocation.REMOTE, string.Empty, true));
				routineRunner.StartCoroutine(ClonePlayer(sourceEnvironment, userId));
			}
			catch (global::System.Exception ex)
			{
				logger.Error(ex.Message);
				outBuilder.Append("Error: [" + args[0] + "]: " + ex.Message);
			}
		}

		private global::System.Collections.IEnumerator ClonePlayer(string sourceEnvironment, long userId)
		{
			yield return new global::UnityEngine.WaitForSeconds(1f);
			gameContext.injectionBinder.GetInstance<global::Kampai.Game.CloneUserFromEnvSignal>().Dispatch(sourceEnvironment, userId);
		}

		[global::Kampai.Util.DebugCommand]
		public void Stats(string[] args)
		{
			if (gameLoadedModel.coldStartTime > 0)
			{
				outBuilder.AppendFormat("Last Start Time: {0}", ((float)gameLoadedModel.coldStartTime / 1000f).ToString());
			}
		}

		private void DebugKeyHit(global::Kampai.Util.DebugArgument arg)
		{
			switch (arg)
			{
			case global::Kampai.Util.DebugArgument.RUSH_ALL_QUESTS:
				FinishQuests(new string[0]);
				break;
			case global::Kampai.Util.DebugArgument.RUSH_ALL_TASKS:
				FinishTasks(new string[0]);
				break;
			case global::Kampai.Util.DebugArgument.BACK_BUTTON:
				BackButton(new string[0]);
				break;
			}
		}

		private void SplitCommandKey(string key, string argString, out string commandName, out string subcommandName)
		{
			int num = -1;
			if (argString.Length > 0)
			{
				if (key.Length > argString.Length)
				{
					num = key.IndexOf(' ', argString.Length + 1);
				}
			}
			else
			{
				num = key.IndexOf(" ");
			}
			if (num != -1)
			{
				commandName = key.Substring(0, num);
				int num2 = key.IndexOf(' ', num + 1);
				if (num2 == -1)
				{
					num2 = key.Length;
				}
				subcommandName = key.Substring(num + 1, num2 - num - 1);
			}
			else
			{
				commandName = key;
				subcommandName = null;
			}
		}

		private void OutputSubcommands(global::System.Collections.Generic.List<string> subcommands)
		{
			if (subcommands == null)
			{
				return;
			}
			outBuilder.Append(" <");
			int i = 0;
			for (int count = subcommands.Count; i < count; i++)
			{
				outBuilder.Append(subcommands[i]);
				if (i < count - 1)
				{
					outBuilder.Append(",");
				}
			}
			outBuilder.Append("> ");
		}

		private void OutputArguments(global::System.Collections.IEnumerable arguments)
		{
			if (arguments == null)
			{
				return;
			}
			foreach (string argument in arguments)
			{
				outBuilder.Append(" {");
				outBuilder.Append(argument);
				outBuilder.Append("} ");
			}
		}

		private void ProcessTransaction(string input, int id, bool add)
		{
			uint result = 0u;
			if (uint.TryParse(input, out result))
			{
				global::Kampai.Util.QuantityItem item = new global::Kampai.Util.QuantityItem(id, result);
				global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = new global::Kampai.Game.Transaction.TransactionDefinition();
				transactionDefinition.ID = uniqueID++;
				if (add)
				{
					transactionDefinition.Outputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
					transactionDefinition.Outputs.Add(item);
				}
				else
				{
					transactionDefinition.Inputs = new global::System.Collections.Generic.List<global::Kampai.Util.QuantityItem>();
					transactionDefinition.Inputs.Add(item);
				}
				playerService.RunEntireTransaction(transactionDefinition, global::Kampai.Game.TransactionTarget.HARVEST, TransactionCallback);
			}
		}

		private void ProcessTransaction(int id, bool processInputs)
		{
			if (processInputs)
			{
				playerService.RunEntireTransaction(id, global::Kampai.Game.TransactionTarget.HARVEST, TransactionCallback);
				return;
			}
			global::Kampai.Game.Transaction.TransactionDefinition transactionDefinition = definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(id).CopyTransaction();
			if (global::System.Linq.Enumerable.Count(transactionDefinition.Inputs) > 0)
			{
				transactionDefinition.Inputs.Clear();
			}
			playerService.RunEntireTransaction(transactionDefinition, global::Kampai.Game.TransactionTarget.HARVEST, TransactionCallback);
		}

		private void TransactionCallback(global::Kampai.Game.PendingCurrencyTransaction pending)
		{
			if (pending.Success)
			{
				ExpansionTransactionCallback(pending);
				outBuilder.AppendLine("TRANSACTION COMPLETE");
			}
			else
			{
				outBuilder.AppendLine("TRANSACTION FAILED");
			}
			FlushSignal.Dispatch();
		}

		private void PurchaseApproval(bool approve)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.KampaiPendingTransaction> pendingTransactions = playerService.GetPendingTransactions();
			if (pendingTransactions.Count > 0)
			{
				string externalIdentifier = pendingTransactions[0].ExternalIdentifier;
				global::Kampai.Game.StoreItemDefinition storeItemDefinition = definitionService.Get<global::Kampai.Game.StoreItemDefinition>(pendingTransactions[0].StoreItemDefinitionId);
				if (approve)
				{
					outBuilder.AppendLine(string.Format("Approving {0} - {1}", externalIdentifier, storeItemDefinition.LocalizedKey));
					currencyService.PurchaseSucceededAndValidatedCallback(externalIdentifier);
				}
				else
				{
					outBuilder.AppendLine(string.Format("Denying {0} - {1}", externalIdentifier, storeItemDefinition.LocalizedKey));
					currencyService.PurchaseCanceledCallback(externalIdentifier, uint.MaxValue);
				}
			}
			else
			{
				outBuilder.AppendLine("No pending transactions.");
			}
		}

		private void SetMap(bool enabled)
		{
			global::Kampai.Game.EnableBuildingAnimatorsSignal instance = gameContext.injectionBinder.GetInstance<global::Kampai.Game.EnableBuildingAnimatorsSignal>();
			global::Kampai.Game.EnableAllMinionRenderersSignal instance2 = gameContext.injectionBinder.GetInstance<global::Kampai.Game.EnableAllMinionRenderersSignal>();
			instance.Dispatch(!enabled);
			instance2.Dispatch(!enabled);
		}

		private bool IdOf(string itemName, out int id)
		{
			foreach (global::System.Collections.Generic.KeyValuePair<int, global::Kampai.Game.Definition> allDefinition in definitionService.GetAllDefinitions())
			{
				if (allDefinition.Value.LocalizedKey != null && itemName.ToLower().Equals(allDefinition.Value.LocalizedKey.Replace(' ', '_').ToLower()))
				{
					id = allDefinition.Key;
					return true;
				}
			}
			id = 0;
			return false;
		}

		private bool parseSocialEventDefinitions()
		{
			int num = timeService.CurrentTime();
			foreach (global::Kampai.Game.TimedSocialEventDefinition localSocialEvent in localSocialEvents)
			{
				int startTime = localSocialEvent.StartTime;
				int finishTime = localSocialEvent.FinishTime;
				if (num >= startTime && num < finishTime)
				{
					return true;
				}
			}
			return false;
		}

		private bool parseSocialEventInvitiations()
		{
			if (localInvitations != null)
			{
				localPersistService.GetData("UserID");
				foreach (global::Kampai.Game.SocialEventInvitation localInvitation in localInvitations)
				{
					if (localInvitation.EventID == timedSocialEventService.GetCurrentSocialEvent().ID)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
