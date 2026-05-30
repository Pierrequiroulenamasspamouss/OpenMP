namespace Kampai.Game
{
	public class PartyService : global::Kampai.Game.IPartyService
	{
		private global::Kampai.Game.LevelFunTable levelFunList;

		private bool m_isInspired;

		[Inject]
		public global::Kampai.Game.IDefinitionService DefinitionService { get; set; }

		public bool IsInspiredParty
		{
			get
			{
				return m_isInspired;
			}
			set
			{
				m_isInspired = value;
			}
		}

		private int ClampLevel(int level)
		{
			if (levelFunList == null)
			{
				levelFunList = DefinitionService.Get<global::Kampai.Game.LevelFunTable>(1000009681);
			}
			if (levelFunList != null && levelFunList.partiesNeededList != null)
			{
				int targetCount = global::System.Math.Max(101, level + 1);
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
			level = global::UnityEngine.Mathf.Clamp(level, 0, levelFunList.partiesNeededList.Count - 1);
			return level;
		}

		private int ClampPartyIndex(global::System.Collections.Generic.List<int> pointsNeededList, int partyIndex)
		{
			partyIndex = global::UnityEngine.Mathf.Clamp(partyIndex, 0, pointsNeededList.Count - 1);
			return partyIndex;
		}

		public int GetTotalParties(int level)
		{
			if (levelFunList == null)
			{
				levelFunList = DefinitionService.Get<global::Kampai.Game.LevelFunTable>(1000009681);
			}
			level = ClampLevel(level);
			return levelFunList.partiesNeededList[level].PointsNeeded.Count;
		}

		public uint GetTotalPartyPoints(int level, int partyIndex)
		{
			if (levelFunList == null)
			{
				levelFunList = DefinitionService.Get<global::Kampai.Game.LevelFunTable>(1000009681);
			}
			level = ClampLevel(level);
			global::System.Collections.Generic.List<int> pointsNeeded = levelFunList.partiesNeededList[level].PointsNeeded;
			return (uint)pointsNeeded[ClampPartyIndex(pointsNeeded, partyIndex)];
		}

		public uint GetTotalPartyPoints(int level, int fromPartyIndex, int toPartyIndex)
		{
			uint num = 0u;
			for (int i = fromPartyIndex; i <= toPartyIndex; i++)
			{
				num += GetTotalPartyPoints(level, i);
			}
			return num;
		}

		public bool IsInspirationParty(int level, int currentIndex)
		{
			IsInspiredParty = currentIndex >= levelFunList.partiesNeededList[ClampLevel(level)].PointsNeeded.Count - 1;
			return IsInspiredParty;
		}

		public void GetNewLevelIndexAndPointsAfterParty(int level, int currentIndex, int currentPoints, out int newLevel, out int newIndex, out int newPoints)
		{
			newPoints = currentPoints - (int)GetTotalPartyPoints(level, currentIndex);
			if (IsInspirationParty(level, currentIndex))
			{
				newLevel = level + 1;
				newIndex = 0;
			}
			else
			{
				newLevel = level;
				newIndex = currentIndex + 1;
			}
		}

		public int GetCumulativePointsEarnedThisLevel(int level, int currentIndex, int currentPartyPoints)
		{
			global::System.Collections.Generic.List<int> pointsNeeded = levelFunList.partiesNeededList[ClampLevel(level)].PointsNeeded;
			int count = global::UnityEngine.Mathf.Clamp(currentIndex, 0, pointsNeeded.Count);
			int num = global::System.Linq.Enumerable.Sum(pointsNeeded.GetRange(0, count));
			return num + currentPartyPoints;
		}

		public int GetCumulativePointsRequiredThisLevel(int currentLevel)
		{
			if (levelFunList == null)
			{
				levelFunList = DefinitionService.Get<global::Kampai.Game.LevelFunTable>(1000009681);
			}
			return global::System.Linq.Enumerable.Sum(levelFunList.partiesNeededList[ClampLevel(currentLevel)].PointsNeeded);
		}

		public int GetCumulativePointsNeededForNextParty(int level, int currentIndex)
		{
			return (int)GetTotalPartyPoints(level, 0, currentIndex);
		}

		public global::Kampai.Util.Tuple<int, int> V4toV5UpdatePartyPointsAndIndex(int level, int xp)
		{
			if (levelFunList == null)
			{
				levelFunList = DefinitionService.Get<global::Kampai.Game.LevelFunTable>(1000009681);
			}
			level = ClampLevel(level);
			global::System.Collections.Generic.List<int> pointsNeeded = levelFunList.partiesNeededList[level].PointsNeeded;
			int num = 0;
			int cumulativePointsRequiredThisLevel = GetCumulativePointsRequiredThisLevel(level);
			if (xp >= cumulativePointsRequiredThisLevel)
			{
				num = pointsNeeded.Count - 1;
				return new global::Kampai.Util.Tuple<int, int>(pointsNeeded[num], num);
			}
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int first = xp;
			int second = 0;
			int totalParties = GetTotalParties(level);
			for (int i = 0; i < totalParties; i++)
			{
				int num6 = pointsNeeded[i];
				num4 += num6;
				if (xp < num4)
				{
					second = num2;
					first = xp - (num5 - num3);
					break;
				}
				num2 = i;
				num3 = num6;
				num5 = num4;
			}
			return new global::Kampai.Util.Tuple<int, int>(first, second);
		}
	}
}
