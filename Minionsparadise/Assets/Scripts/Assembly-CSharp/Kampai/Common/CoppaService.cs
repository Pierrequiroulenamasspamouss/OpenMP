namespace Kampai.Common
{
	public class CoppaService : global::Kampai.Common.ICoppaService
	{
		[Inject]
		public ILocalPersistanceService localPersistanceService { get; set; }

		public bool IsBirthdateKnown()
		{
			return localPersistanceService.HasKeyPlayer("COPPA_Age_Month") && localPersistanceService.HasKeyPlayer("COPPA_Age_Year");
		}

		public bool GetBirthdate(out global::System.DateTime birthdate)
		{
			if (!IsBirthdateKnown())
			{
				birthdate = global::System.DateTime.Now;
				return false;
			}
			int dataIntPlayer = localPersistanceService.GetDataIntPlayer("COPPA_Age_Year");
			int dataIntPlayer2 = localPersistanceService.GetDataIntPlayer("COPPA_Age_Month");
			if (dataIntPlayer < 1900 || dataIntPlayer > 2100) dataIntPlayer = 2000;
			if (dataIntPlayer2 < 1 || dataIntPlayer2 > 12) dataIntPlayer2 = 1;
			birthdate = new global::System.DateTime(dataIntPlayer, dataIntPlayer2, 1);
			return true;
		}

		public bool Restricted()
		{
			return GetAge() < 13;
		}

		public void SetUserBirthdate(global::System.DateTime birthdate)
		{
			localPersistanceService.PutDataIntPlayer("COPPA_Age_Month", birthdate.Month);
			localPersistanceService.PutDataIntPlayer("COPPA_Age_Year", birthdate.Year);
		}

		public int GetAge()
		{
			global::System.DateTime birthdate;
			if (GetBirthdate(out birthdate))
			{
				return (global::System.DateTime.Now - birthdate).Days / 365;
			}
			return 0;
		}
	}
}
