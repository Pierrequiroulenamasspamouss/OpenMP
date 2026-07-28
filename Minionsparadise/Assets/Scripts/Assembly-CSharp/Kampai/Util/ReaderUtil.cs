namespace Kampai.Util
{
	public static class ReaderUtil
	{
		public static int SafeInt(object val)
		{
			if (val == null) return 0;
			try { return global::System.Convert.ToInt32(val); } catch { return 0; }
		}

		public static uint SafeUInt(object val)
		{
			if (val == null) return 0u;
			try { return global::System.Convert.ToUInt32(val); } catch { return 0u; }
		}

		public static float SafeFloat(object val)
		{
			if (val == null) return 0f;
			try { return global::System.Convert.ToSingle(val); } catch { return 0f; }
		}

		public static bool SafeBool(object val)
		{
			if (val == null) return false;
			try { return global::System.Convert.ToBoolean(val); } catch { return false; }
		}

		public static string SafeString(object val)
		{
			if (val == null) return null;
			return val.ToString();
		}

		public static T ReadObject<T>(global::Newtonsoft.Json.JsonReader reader, global::System.Func<T, string, T> parseProp)
		{
			if (reader == null) return default(T);
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None) { try { reader.Read(); } catch { return default(T); } }
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null) return default(T);
			T result = default(T);
			try
			{
				try { result = global::System.Activator.CreateInstance<T>(); }
				catch
				{
					try { result = (T)global::System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T)); } catch { }
				}
				while (reader.Read())
				{
					if (reader.TokenType == global::Newtonsoft.Json.JsonToken.PropertyName)
					{
						string valStr = reader.Value as string;
						string prop = (valStr != null) ? valStr.ToUpper() : null;
						reader.Read();
						if (prop != null) { try { result = parseProp(result, prop); } catch { reader.Skip(); } }
						else { reader.Skip(); }
					}
					else if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
					{
						return result;
					}
				}
			}
			catch { }
			return result;
		}

				public static global::Kampai.Game.LegalDocumentURL ReadLegalDocumentURL(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.LegalDocumentURL>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LANGUAGE": res.language = ReadString(reader, converters); break;
				case "URL": res.url = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.NotificationReminder ReadNotificationReminder(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.NotificationReminder>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LEVEL": res.level = SafeInt(reader.Value); break;
				case "MESSAGELOCALIZEDKEY": res.messageLocalizedKey = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.CharacterPrestigeLevelDefinition ReadCharacterPrestigeLevelDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.CharacterPrestigeLevelDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "UNLOCKLEVEL": res.UnlockLevel = SafeUInt(reader.Value); break;
				case "UNLOCKQUESTID": res.UnlockQuestID = SafeInt(reader.Value); break;
				case "POINTSNEEDED": res.PointsNeeded = SafeUInt(reader.Value); break;
				case "ATTACHEDQUESTID": res.AttachedQuestID = SafeInt(reader.Value); break;
				case "WELCOMEPANELMESSAGELOCALIZEDKEY": res.WelcomePanelMessageLocalizedKey = ReadString(reader, converters); break;
				case "FAREWELLPANELMESSAGELOCALIZEDKEY": res.FarewellPanelMessageLocalizedKey = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.AchievementID ReadAchievementID(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.AchievementID>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "GAMECENTERID": res.GameCenterID = ReadString(reader, converters); break;
				case "GOOGLEPLAYID": res.GooglePlayID = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.ScreenPosition ReadScreenPosition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.ScreenPosition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeFloat(reader.Value); break;
				case "Z": res.z = SafeFloat(reader.Value); break;
				case "ZOOM": res.zoom = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::UnityEngine.Vector3 ReadVector3(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::UnityEngine.Vector3>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeFloat(reader.Value); break;
				case "Y": res.y = SafeFloat(reader.Value); break;
				case "Z": res.z = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.ConnectablePiecePrefabDefinition ReadConnectablePiecePrefabDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.ConnectablePiecePrefabDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "STRAIGHT": res.straight = ReadString(reader, converters); break;
				case "CROSS": res.cross = ReadString(reader, converters); break;
				case "POST": res.post = ReadString(reader, converters); break;
				case "TSHAPE": res.tshape = ReadString(reader, converters); break;
				case "ENDCAP": res.endcap = ReadString(reader, converters); break;
				case "CORNER": res.corner = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.SlotUnlock ReadSlotUnlock(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.SlotUnlock>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "SLOTUNLOCKLEVELS": res.SlotUnlockLevels = PopulateListInt32(reader, res.SlotUnlockLevels); break;
				case "SLOTUNLOCKCOSTS": res.SlotUnlockCosts = PopulateListInt32(reader, res.SlotUnlockCosts); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.UserSegment ReadUserSegment(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.UserSegment>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LEVELGREATERTHANOREQUALTO": res.LevelGreaterThanOrEqualTo = SafeInt(reader.Value); break;
				case "FIRSTXRETURNREWARDSWEIGHTEDDEFINITIONID": res.FirstXReturnRewardsWeightedDefinitionId = SafeInt(reader.Value); break;
				case "SECONDXRETURNREWARDSWEIGHTEDDEFINITIONID": res.SecondXReturnRewardsWeightedDefinitionId = SafeInt(reader.Value); break;
				case "AFTERXRETURNREWARDS": res.AfterXReturnRewards = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Location ReadLocation(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Location>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeInt(reader.Value); break;
				case "Y": res.y = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static MignetteRuleDefinition ReadMignetteRuleDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<MignetteRuleDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "CAUSEIMAGE": res.CauseImage = ReadString(reader, converters); break;
				case "CAUSEIMAGEMASK": res.CauseImageMask = ReadString(reader, converters); break;
				case "EFFECTIMAGE": res.EffectImage = ReadString(reader, converters); break;
				case "EFFECTIMAGEMASK": res.EffectImageMask = ReadString(reader, converters); break;
				case "EFFECTAMOUNT": res.EffectAmount = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MignetteChildObjectDefinition ReadMignetteChildObjectDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MignetteChildObjectDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "PREFAB": res.Prefab = ReadString(reader, converters); break;
				case "POSITION": res.Position = ReadVector3(reader, converters); break;
				case "ISLOCAL": res.IsLocal = SafeBool(reader.Value); break;
				case "ROTATION": res.Rotation = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MinionPartyPrefabDefinition ReadMinionPartyPrefabDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MinionPartyPrefabDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "EVENTTYPE": res.EventType = ReadString(reader, converters); break;
				case "PREFAB": res.Prefab = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Area ReadArea(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Area>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "A": res.a = ReadLocation(reader, converters); break;
				case "B": res.b = ReadLocation(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.StorageUpgradeDefinition ReadStorageUpgradeDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.StorageUpgradeDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LEVEL": res.Level = SafeInt(reader.Value); break;
				case "STORAGECAPACITY": res.StorageCapacity = SafeUInt(reader.Value); break;
				case "TRANSACTIONID": res.TransactionId = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.PlatformDefinition ReadPlatformDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.PlatformDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "BUILDINGREMOVALANIMCONTROLLER": res.buildingRemovalAnimController = ReadString(reader, converters); break;
				case "CUSTOMCAMERAPOSID": res.customCameraPosID = SafeInt(reader.Value); break;
				case "DESCRIPTION": res.description = ReadString(reader, converters); break;
				case "OFFSET": res.offset = ReadVector3(reader, converters); break;
				case "PLACEMENTLOCATION": res.placementLocation = ReadLocation(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.ResourcePlotDefinition ReadResourcePlotDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.ResourcePlotDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "DESCRIPTIONKEY": res.descriptionKey = ReadString(reader, converters); break;
				case "ISAUTOMATICALLYUNLOCKED": res.isAutomaticallyUnlocked = SafeBool(reader.Value); break;
				case "LOCATION": res.location = ReadLocation(reader, converters); break;
				case "UNLOCKTRANSACTIONID": res.unlockTransactionID = SafeInt(reader.Value); break;
				case "ROTATION": res.rotation = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.CharacterUIAnimationDefinition ReadCharacterUIAnimationDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.CharacterUIAnimationDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "STATEMACHINE": res.StateMachine = ReadString(reader, converters); break;
				case "IDLEWEIGHTEDANIMATIONID": res.IdleWeightedAnimationID = SafeInt(reader.Value); break;
				case "IDLECOUNT": res.IdleCount = SafeInt(reader.Value); break;
				case "HAPPYWEIGHTEDANIMATIONID": res.HappyWeightedAnimationID = SafeInt(reader.Value); break;
				case "HAPPYCOUNT": res.HappyCount = SafeInt(reader.Value); break;
				case "SELECTEDWEIGHTEDANIMATIONID": res.SelectedWeightedAnimationID = SafeInt(reader.Value); break;
				case "SELECTEDCOUNT": res.SelectedCount = SafeInt(reader.Value); break;
				case "USELEGACY": res.UseLegacy = SafeBool(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.FloatLocation ReadFloatLocation(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.FloatLocation>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeFloat(reader.Value); break;
				case "Y": res.y = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Angle ReadAngle(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Angle>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "DEGREES": res.Degrees = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.CollectionReward ReadCollectionReward(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.CollectionReward>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "REQUIREDPOINTS": res.RequiredPoints = SafeInt(reader.Value); break;
				case "TRANSACTIONID": res.TransactionID = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.FlyOverNode ReadFlyOverNode(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.FlyOverNode>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeFloat(reader.Value); break;
				case "Y": res.y = SafeFloat(reader.Value); break;
				case "Z": res.z = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.BridgeScreenPosition ReadBridgeScreenPosition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.BridgeScreenPosition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeFloat(reader.Value); break;
				case "Y": res.y = SafeFloat(reader.Value); break;
				case "Z": res.z = SafeFloat(reader.Value); break;
				case "ZOOM": res.zoom = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Util.KampaiColor ReadKampaiColor(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Util.KampaiColor>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "R": res.r = SafeFloat(reader.Value); break;
				case "G": res.g = SafeFloat(reader.Value); break;
				case "B": res.b = SafeFloat(reader.Value); break;
				case "A": res.a = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Reward ReadReward(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Reward>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "REQUIREDQUANTITY": res.requiredQuantity = SafeUInt(reader.Value); break;
				case "PREMIUMREWARD": res.premiumReward = SafeUInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MiniGameScoreReward ReadMiniGameScoreReward(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MiniGameScoreReward>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "MINIGAMEID": res.MiniGameId = SafeInt(reader.Value); break;
				case "REWARDTABLE": res.rewardTable = PopulateList<global::Kampai.Game.Reward>(reader, converters, ReadReward, res.rewardTable); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MiniGameScoreRange ReadMiniGameScoreRange(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MiniGameScoreRange>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "MINIGAMEID": res.MiniGameId = SafeInt(reader.Value); break;
				case "SCORERANGEMAX": res.ScoreRangeMax = SafeInt(reader.Value); break;
				case "SCORERANGEMIN": res.ScoreRangeMin = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MasterPlanComponentRewardDefinition ReadMasterPlanComponentRewardDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MasterPlanComponentRewardDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "REWARDITEMID": res.rewardItemId = SafeInt(reader.Value); break;
				case "REWARDQUANTITY": res.rewardQuantity = SafeUInt(reader.Value); break;
				case "GRINDREWARD": res.grindReward = SafeUInt(reader.Value); break;
				case "PREMIUMREWARD": res.premiumReward = SafeUInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MasterPlanComponentTaskDefinition ReadMasterPlanComponentTaskDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MasterPlanComponentTaskDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "REQUIREDITEMID": res.requiredItemId = SafeInt(reader.Value); break;
				case "REQUIREDQUANTITY": res.requiredQuantity = SafeUInt(reader.Value); break;
				case "SHOWWAYFINDER": res.ShowWayfinder = SafeBool(reader.Value); break;
				case "TYPE": res.Type = ReadEnum<global::Kampai.Game.MasterPlanComponentTaskType>(reader); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.GhostFunctionDefinition ReadGhostFunctionDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.GhostFunctionDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "STARTTYPE": res.startType = ReadEnum<global::Kampai.UI.GhostComponentFunctionType>(reader); break;
				case "CLOSETYPE": res.closeType = ReadEnum<global::Kampai.UI.GhostFunctionCloseType>(reader); break;
				case "COMPONENTBUILDINGDEFID": res.componentBuildingDefID = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.KnuckleheadednessInfo ReadKnuckleheadednessInfo(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.KnuckleheadednessInfo>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "KNUCKLEHEADDEDNESSMIN": res.KnuckleheaddednessMin = SafeFloat(reader.Value); break;
				case "KNUCKLEHEADDEDNESSMAX": res.KnuckleheaddednessMax = SafeFloat(reader.Value); break;
				case "KNUCKLEHEADDEDNESSSCALE": res.KnuckleheaddednessScale = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.AnimationAlternate ReadAnimationAlternate(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.AnimationAlternate>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "GROUPID": res.GroupID = SafeInt(reader.Value); break;
				case "PERCENTCHANCE": res.PercentChance = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.CameraControlSettings ReadCameraControlSettings(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.CameraControlSettings>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "CUSTOMCAMERAPOSTIKI": res.customCameraPosTiki = SafeInt(reader.Value); break;
				case "CUSTOMCAMERAPOSSTAGE": res.customCameraPosStage = SafeInt(reader.Value); break;
				case "CUSTOMCAMERAPOSTOWNHALL": res.customCameraPosTownHall = SafeInt(reader.Value); break;
				case "CUSTOMCAMERAPOSPARTYDEFAULT": res.customCameraPosPartyDefault = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.VFXAssetDefinition ReadVFXAssetDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.VFXAssetDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LOCATION": res.location = ReadLocation(reader, converters); break;
				case "PREFAB": res.Prefab = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MinionBenefit ReadMinionBenefit(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MinionBenefit>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LOCALIZEDKEY": res.localizedKey = ReadString(reader, converters); break;
				case "ITEMICONID": res.itemIconId = SafeInt(reader.Value); break;
				case "TYPE": res.type = ReadEnum<global::Kampai.UI.View.Benefit>(reader); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MinionBenefitLevel ReadMinionBenefitLevel(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MinionBenefitLevel>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "DOUBLEDROPPERCENTAGE": res.doubleDropPercentage = SafeFloat(reader.Value); break;
				case "DOUBLEDROPLEVEL": res.doubleDropLevel = SafeInt(reader.Value); break;
				case "PREMIUMDROPPERCENTAGE": res.premiumDropPercentage = SafeFloat(reader.Value); break;
				case "PREMIUMDROPLEVEL": res.premiumDropLevel = SafeInt(reader.Value); break;
				case "RAREDROPPERCENTAGE": res.rareDropPercentage = SafeFloat(reader.Value); break;
				case "RAREDROPLEVEL": res.rareDropLevel = SafeInt(reader.Value); break;
				case "TOKENSTOLEVEL": res.tokensToLevel = SafeInt(reader.Value); break;
				case "COSTUMEID": res.costumeId = SafeInt(reader.Value); break;
				case "IMAGE": res.image = ReadString(reader, converters); break;
				case "MASK": res.mask = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.ImageMaskCombo ReadImageMaskCombo(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.ImageMaskCombo>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "IMAGE": res.image = ReadString(reader, converters); break;
				case "MASK": res.mask = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Transaction.TransactionInstance ReadTransactionInstance(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Transaction.TransactionInstance>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "ID": res.ID = SafeInt(reader.Value); break;
				case "INPUTS": res.Inputs = PopulateList<global::Kampai.Util.QuantityItem>(reader, converters, res.Inputs); break;
				case "OUTPUTS": res.Outputs = PopulateList<global::Kampai.Util.QuantityItem>(reader, converters, res.Outputs); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.QuestStepDefinition ReadQuestStepDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.QuestStepDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "TYPE": res.Type = ReadEnum<global::Kampai.Game.QuestStepType>(reader); break;
				case "ITEMAMOUNT": res.ItemAmount = SafeInt(reader.Value); break;
				case "ITEMDEFINITIONID": res.ItemDefinitionID = SafeInt(reader.Value); break;
				case "COSTUMEDEFINITIONID": res.CostumeDefinitionID = SafeInt(reader.Value); break;
				case "SHOWWAYFINDER": res.ShowWayfinder = SafeBool(reader.Value); break;
				case "QUESTSTEPCOMPLETEPLAYERTRAININGCATEGORYITEMID": res.QuestStepCompletePlayerTrainingCategoryItemId = SafeInt(reader.Value); break;
				case "UPGRADELEVEL": res.UpgradeLevel = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.QuestChainStepDefinition ReadQuestChainStepDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.QuestChainStepDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "INTRO": res.Intro = ReadString(reader, converters); break;
				case "VOICE": res.Voice = ReadString(reader, converters); break;
				case "OUTRO": res.Outro = ReadString(reader, converters); break;
				case "XP": res.XP = SafeInt(reader.Value); break;
				case "GRIND": res.Grind = SafeInt(reader.Value); break;
				case "PREMIUM": res.Premium = SafeInt(reader.Value); break;
				case "TASKS": res.Tasks = PopulateList<global::Kampai.Game.QuestChainTask>(reader, converters, ReadQuestChainTask, res.Tasks); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.QuestChainTask ReadQuestChainTask(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.QuestChainTask>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "TYPE": res.Type = ReadEnum<global::Kampai.Game.QuestChainTaskType>(reader); break;
				case "ITEM": res.Item = SafeInt(reader.Value); break;
				case "COUNT": res.Count = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.PlatformStoreSkuDefinition ReadPlatformStoreSkuDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.PlatformStoreSkuDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "APPLEAPPSTORE": res.appleAppstore = ReadString(reader, converters); break;
				case "GOOGLEPLAY": res.googlePlay = ReadString(reader, converters); break;
				case "DEFAULTSTORE": res.defaultStore = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Util.Vector3Serialize ReadVector3Serialize(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Util.Vector3Serialize>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "X": res.x = SafeInt(reader.Value); break;
				case "Y": res.y = SafeInt(reader.Value); break;
				case "Z": res.z = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.SocialEventOrderDefinition ReadSocialEventOrderDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.SocialEventOrderDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "ORDERID": res.OrderID = SafeInt(reader.Value); break;
				case "TRANSACTION": res.Transaction = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Trigger.TriggerRewardLayout ReadTriggerRewardLayout(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Trigger.TriggerRewardLayout>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "INDEX": res.index = SafeInt(reader.Value); break;
				case "ITEMIDS": res.itemIds = PopulateListInt32(reader, res.itemIds); break;
				case "LAYOUT": res.layout = ReadEnum<global::Kampai.Game.Trigger.TriggerRewardLayout.Layout>(reader); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.KampaiPendingTransaction ReadKampaiPendingTransaction(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.KampaiPendingTransaction>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "EXTERNALIDENTIFIER": res.ExternalIdentifier = ReadString(reader, converters); break;
				case "TRANSACTION": res.Transaction = ((converters.transactionDefinitionConverter == null) ? global::Kampai.Util.FastJSONDeserializer.Deserialize<global::Kampai.Game.Transaction.TransactionDefinition>(reader, converters) : converters.transactionDefinitionConverter.ReadJson(reader, converters)); break;
				case "TRANSACTIONINSTANCE": res.TransactionInstance = ReadTransactionInstance(reader, converters); break;
				case "STOREITEMDEFINITIONID": res.StoreItemDefinitionId = SafeInt(reader.Value); break;
				case "UTCTIMECREATED": res.UTCTimeCreated = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.UnlockedItem ReadUnlockedItem(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.UnlockedItem>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "DEFID": res.defID = SafeInt(reader.Value); break;
				case "QUANTITY": res.quantity = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.TrackedSale ReadTrackedSale(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.TrackedSale>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "DEFID": res.defID = SafeInt(reader.Value); break;
				case "NUMBERPURCHASED": res.numberPurchased = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.SocialClaimRewardItem ReadSocialClaimRewardItem(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.SocialClaimRewardItem>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "EVENTID": res.eventID = SafeInt(reader.Value); break;
				case "CLAIMSTATE": res.claimState = ReadEnum<global::Kampai.Game.SocialClaimRewardItem.ClaimState>(reader); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.Player.HelpTipTrackingItem ReadHelpTipTrackingItem(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.Player.HelpTipTrackingItem>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "TIPDIFINITIONID": res.tipDifinitionId = SafeInt(reader.Value); break;
				case "SHOWSCOUNT": res.showsCount = SafeInt(reader.Value); break;
				case "LASTSHOWNTIME": res.lastShownTime = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.QuestStep ReadQuestStep(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.QuestStep>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "STATE": res.state = ReadEnum<global::Kampai.Game.QuestStepState>(reader); break;
				case "AMOUNTCOMPLETED": res.AmountCompleted = SafeInt(reader.Value); break;
				case "AMOUNTREADY": res.AmountReady = SafeInt(reader.Value); break;
				case "TRACKEDID": res.TrackedID = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.OrderBoardTicket ReadOrderBoardTicket(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.OrderBoardTicket>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "TRANSACTIONINST": res.TransactionInst = ReadTransactionInstance(reader, converters); break;
				case "STARTGAMETIME": res.StartGameTime = SafeInt(reader.Value); break;
				case "BOARDINDEX": res.BoardIndex = SafeInt(reader.Value); break;
				case "ORDERNAMETABLEINDEX": res.OrderNameTableIndex = SafeInt(reader.Value); break;
				case "STARTTIME": res.StartTime = SafeInt(reader.Value); break;
				case "CHARACTERDEFINITIONID": res.CharacterDefinitionId = SafeInt(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.UserIdentity ReadUserIdentity(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.UserIdentity>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "id": res.ID = ReadString(reader, converters); break;
				case "externalId": res.ExternalID = ReadString(reader, converters); break;
				case "userId": res.UserID = ReadString(reader, converters); break;
				case "type": res.Type = ReadEnum<global::Kampai.Game.IdentityType>(reader); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.SocialOrderProgress ReadSocialOrderProgress(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.SocialOrderProgress>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "ORDERID": res.OrderId = SafeInt(reader.Value); break;
				case "COMPLETEDBYUSERID": res.CompletedByUserId = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MasterPlanComponentReward ReadMasterPlanComponentReward(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MasterPlanComponentReward>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "DEFINITION": res.Definition = ReadMasterPlanComponentRewardDefinition(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.MasterPlanComponentTask ReadMasterPlanComponentTask(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.MasterPlanComponentTask>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "ISCOMPLETE": res.isComplete = SafeBool(reader.Value); break;
				case "EARNEDQUANTITY": res.earnedQuantity = SafeUInt(reader.Value); break;
				case "DEFINITION": res.Definition = ReadMasterPlanComponentTaskDefinition(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.GachaConfig ReadGachaConfig(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.GachaConfig>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "GATCHAANIMATIONDEFINITIONS": res.GatchaAnimationDefinitions = PopulateList<global::Kampai.Game.GachaAnimationDefinition>(reader, converters, res.GatchaAnimationDefinitions); break;
				case "DISTRIBUTIONTABLES": res.DistributionTables = PopulateList<global::Kampai.Game.GachaWeightedDefinition>(reader, converters, res.DistributionTables); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Game.TaskDefinition ReadTaskDefinition(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Game.TaskDefinition>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "LEVELBANDS": res.levelBands = PopulateList<global::Kampai.Game.TaskLevelBandDefinition>(reader, converters, res.levelBands); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Splash.BucketAssignment ReadBucketAssignment(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Splash.BucketAssignment>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "BUCKETID": res.BucketId = SafeInt(reader.Value); break;
				case "TIME": res.Time = SafeFloat(reader.Value); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

				public static global::Kampai.Main.PreloadableAsset ReadPreloadableAsset(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadObject<global::Kampai.Main.PreloadableAsset>(reader, (res, prop) =>
			{
				switch (prop)
				{
				case "NAME": res.name = ReadString(reader, converters); break;
				case "TYPE": res.type = ReadString(reader, converters); break;
				default: reader.Skip(); break;
				}
				return res;
			});
		}

		public static string ReadString(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			return global::System.Convert.ToString(reader.Value);
		}

		public static bool ReadBool(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			return SafeBool(reader.Value);
		}

		public static global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<string, string>> ReadDictionaryDictionaryString(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			return ReadDictionary<global::System.Collections.Generic.Dictionary<string, string>>(reader, converters, ReadDictionaryString);
		}

		public static global::System.Collections.Generic.Dictionary<string, string> ReadDictionaryString(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			return ReadDictionary<string>(reader, converters, ReadString);
		}

		public static T ReadEnum<T>(global::Newtonsoft.Json.JsonReader reader)
		{
			if (reader == null || reader.Value == null) return default(T);
			try
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				case global::Newtonsoft.Json.JsonToken.String:
				{
					string text = reader.Value.ToString();
					if (global::System.Enum.IsDefined(typeof(T), text))
					{
						return (T)global::System.Enum.Parse(typeof(T), text, true);
					}
					try { return (T)global::System.Enum.Parse(typeof(T), text, true); } catch { return default(T); }
				}
				case global::Newtonsoft.Json.JsonToken.Integer:
					return (T)global::System.Enum.ToObject(typeof(T), reader.Value);
				}
			}
			catch { }
			return default(T);
		}

		public static global::System.Collections.Generic.Dictionary<string, string> ReadStringDictionary(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			return ReadDictionary<string>(reader, converters, (global::Newtonsoft.Json.JsonReader r, JsonConverters c) => (string)r.Value);
		}

		public static global::System.Collections.Generic.Dictionary<string, object> ReadDictionary(global::Newtonsoft.Json.JsonReader reader)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::Newtonsoft.Json.Linq.JObject jObject = global::Newtonsoft.Json.Linq.JObject.Load(reader);
			global::Newtonsoft.Json.JsonSerializer jsonSerializer = new global::Newtonsoft.Json.JsonSerializer();
			return jsonSerializer.Deserialize<global::System.Collections.Generic.Dictionary<string, object>>(jObject.CreateReader());
		}

		public static global::System.Collections.Generic.Dictionary<string, object> ReadNestedDictionary(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			global::Newtonsoft.Json.Linq.JObject token = global::Newtonsoft.Json.Linq.JObject.Load(reader);
			return (global::System.Collections.Generic.Dictionary<string, object>)ReadNestedObject(token);
		}

		private static object ReadNestedObject(global::Newtonsoft.Json.Linq.JToken token)
		{
			switch (token.Type)
			{
			case global::Newtonsoft.Json.Linq.JTokenType.Object:
			{
				global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
				{
					foreach (global::Newtonsoft.Json.Linq.JProperty item in token.Children<global::Newtonsoft.Json.Linq.JProperty>())
					{
						dictionary.Add(item.Name, ReadNestedObject(item.Value));
					}
					return dictionary;
				}
			}
			case global::Newtonsoft.Json.Linq.JTokenType.Array:
			{
				global::System.Collections.Generic.List<object> list = new global::System.Collections.Generic.List<object>();
				{
					foreach (global::Newtonsoft.Json.Linq.JProperty item2 in token.Children<global::Newtonsoft.Json.Linq.JProperty>())
					{
						list.Add(ReadNestedObject(item2.Value));
					}
					return list;
				}
			}
			default:
				return ((global::Newtonsoft.Json.Linq.JValue)token).Value;
			}
		}

		public static global::System.Collections.Generic.Dictionary<string, T> ReadDictionary<T>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null) where T : global::Kampai.Util.IFastJSONDeserializable, new()
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.Dictionary<string, T> dictionary = new global::System.Collections.Generic.Dictionary<string, T>();
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					string key = (string)reader.Value;
					reader.Read();
					T value = global::Kampai.Util.FastJSONDeserializer.Deserialize<T>(reader, converters);
					dictionary.Add(key, value);
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					return dictionary;
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return dictionary;
		}

		public static global::System.Collections.Generic.Dictionary<string, T> ReadDictionary<T>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters, global::System.Func<global::Newtonsoft.Json.JsonReader, JsonConverters, T> valueReader)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.Dictionary<string, T> dictionary = new global::System.Collections.Generic.Dictionary<string, T>();
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					string key = (string)reader.Value;
					reader.Read();
					T value = valueReader(reader, converters);
					dictionary.Add(key, value);
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					return dictionary;
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return dictionary;
		}

		public static global::System.Collections.Generic.Dictionary<K, V> ReadDictionary<K, V>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters, global::System.Func<global::Newtonsoft.Json.JsonReader, JsonConverters, K> keyReader, global::System.Func<global::Newtonsoft.Json.JsonReader, JsonConverters, V> valueReader)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.Dictionary<K, V> dictionary = new global::System.Collections.Generic.Dictionary<K, V>();
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					K key = keyReader(reader, converters);
					reader.Read();
					V value = valueReader(reader, converters);
					dictionary.Add(key, value);
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					return dictionary;
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return dictionary;
		}

		public static global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>> ReadListOfIntLists(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>> list = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<int>>();
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Comment:
					continue;
				}
				global::System.Collections.Generic.List<int> item = PopulateListInt32(reader);
				list.Add(item);
			}
			return list;
		}

		public static global::System.Collections.Generic.List<T> PopulateList<T>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters, global::System.Func<global::Newtonsoft.Json.JsonReader, JsonConverters, T> elementReader, global::System.Collections.Generic.IEnumerable<T> existingValue = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<T> list = ((existingValue == null) ? new global::System.Collections.Generic.List<T>() : new global::System.Collections.Generic.List<T>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Comment:
					continue;
				}
				T item = elementReader(reader, converters);
				list.Add(item);
			}
			return list;
		}

		public static global::System.Collections.Generic.List<T> PopulateList<T>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters, global::Kampai.Util.FastJsonConverter<T> converter, global::System.Collections.Generic.IEnumerable<T> existingValue = null) where T : class, global::Kampai.Util.IFastJSONDeserializable
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<T> list = ((existingValue == null) ? new global::System.Collections.Generic.List<T>() : new global::System.Collections.Generic.List<T>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Comment:
					continue;
				}
				T item = converter.ReadJson(reader, converters);
				list.Add(item);
			}
			return list;
		}

		public static global::System.Collections.Generic.List<T> PopulateList<T>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null, global::System.Collections.Generic.IEnumerable<T> existingValue = null) where T : global::Kampai.Util.IFastJSONDeserializable, new()
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			global::System.Collections.Generic.List<T> list = ((existingValue == null) ? new global::System.Collections.Generic.List<T>() : new global::System.Collections.Generic.List<T>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Comment:
					continue;
				}
				T item = new T();
				item.Deserialize(reader, converters);
				list.Add(item);
			}
			return list;
		}

		public static global::System.Collections.Generic.List<string> PopulateListString(global::Newtonsoft.Json.JsonReader reader, global::System.Collections.Generic.IEnumerable<string> existingValue = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<string> list = ((existingValue == null) ? new global::System.Collections.Generic.List<string>() : new global::System.Collections.Generic.List<string>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.String:
				{
					string item = (string)reader.Value;
					list.Add(item);
					break;
				}
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return list;
		}

		public static global::System.Collections.Generic.List<int> PopulateListInt32(global::Newtonsoft.Json.JsonReader reader, global::System.Collections.Generic.IEnumerable<int> existingValue = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<int> list = ((existingValue == null) ? new global::System.Collections.Generic.List<int>() : new global::System.Collections.Generic.List<int>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Integer:
				{
					int item = SafeInt(reader.Value);
					list.Add(item);
					break;
				}
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return list;
		}

		public static global::System.Collections.Generic.List<bool> PopulateListBoolean(global::Newtonsoft.Json.JsonReader reader, global::System.Collections.Generic.IEnumerable<bool> existingValue = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<bool> list = ((existingValue == null) ? new global::System.Collections.Generic.List<bool>() : new global::System.Collections.Generic.List<bool>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Boolean:
				{
					bool item2 = SafeBool(reader.Value);
					list.Add(item2);
					break;
				}
				case global::Newtonsoft.Json.JsonToken.Integer:
				{
					bool item = SafeBool(reader.Value);
					list.Add(item);
					break;
				}
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return list;
		}

		public static global::System.Collections.Generic.List<float> PopulateListSingle(global::Newtonsoft.Json.JsonReader reader, global::System.Collections.Generic.IEnumerable<float> existingValue = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::System.Collections.Generic.List<float> list = ((existingValue == null) ? new global::System.Collections.Generic.List<float>() : new global::System.Collections.Generic.List<float>(existingValue));
			EnsureToken(global::Newtonsoft.Json.JsonToken.StartArray, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.EndArray:
					return list;
				case global::Newtonsoft.Json.JsonToken.Float:
				{
					float item2 = SafeFloat(reader.Value);
					list.Add(item2);
					break;
				}
				case global::Newtonsoft.Json.JsonToken.Integer:
				{
					float item = SafeFloat(reader.Value);
					list.Add(item);
					break;
				}
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return list;
		}

		public static void EnsureToken(global::Newtonsoft.Json.JsonToken token, global::Newtonsoft.Json.JsonReader reader) { }

		public static T ReaderNotImplemented<T>(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null) { return default(T); }

		public static string GetPositionInSource(global::Newtonsoft.Json.JsonReader reader)
		{
			global::Newtonsoft.Json.JsonTextReader jsonTextReader = reader as global::Newtonsoft.Json.JsonTextReader;
			if (jsonTextReader != null)
			{
				return string.Format("Line number: {0}, Line position: {1}", jsonTextReader.LineNumber, jsonTextReader.LinePosition);
			}
			return "Line number: -, Line position: -";
		}

		public static global::System.Collections.Generic.Dictionary<global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent, bool> ReadRateAppTriggerConfig(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			return ReadDictionary<global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent, bool>(reader, converters, ReadRateAppAfterEvent, ReadBool);
		}

		public static global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent ReadRateAppAfterEvent(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			return ReadEnum<global::Kampai.Game.ConfigurationDefinition.RateAppAfterEvent>(reader);
		}

		public static global::Kampai.Game.KillSwitch ReadKillSwitch(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			return ReadEnum<global::Kampai.Game.KillSwitch>(reader);
		}
	}

}
