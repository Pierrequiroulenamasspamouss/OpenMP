namespace GooglePlayGames
{
	internal class PlayGamesAchievement : global::UnityEngine.SocialPlatforms.IAchievementDescription, global::UnityEngine.SocialPlatforms.IAchievement
	{
		private readonly global::GooglePlayGames.ReportProgress mProgressCallback;

		private string mId = string.Empty;

		private bool mIsIncremental;

		private int mCurrentSteps;

		private int mTotalSteps;

		private double mPercentComplete;

		private bool mCompleted;

		private bool mHidden;

		private global::System.DateTime mLastModifiedTime = new global::System.DateTime(1970, 1, 1, 0, 0, 0, 0);

		private string mTitle = string.Empty;

		private string mRevealedImageUrl = string.Empty;

		private string mUnlockedImageUrl = string.Empty;

		private global::UnityEngine.Networking.UnityWebRequest mImageFetcher;

		private global::UnityEngine.Texture2D mImage;

		private string mDescription = string.Empty;

		private ulong mPoints;

		public string id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public bool isIncremental
		{
			get
			{
				return mIsIncremental;
			}
		}

		public int currentSteps
		{
			get
			{
				return mCurrentSteps;
			}
		}

		public int totalSteps
		{
			get
			{
				return mTotalSteps;
			}
		}

		public double percentCompleted
		{
			get
			{
				return mPercentComplete;
			}
			set
			{
				mPercentComplete = value;
			}
		}

		public bool completed
		{
			get
			{
				return mCompleted;
			}
		}

		public bool hidden
		{
			get
			{
				return mHidden;
			}
		}

		public global::System.DateTime lastReportedDate
		{
			get
			{
				return mLastModifiedTime;
			}
		}

		public string title
		{
			get
			{
				return mTitle;
			}
		}

		public global::UnityEngine.Texture2D image
		{
			get
			{
				return LoadImage();
			}
		}

		public string achievedDescription
		{
			get
			{
				return mDescription;
			}
		}

		public string unachievedDescription
		{
			get
			{
				return mDescription;
			}
		}

		public int points
		{
			get
			{
				return (int)mPoints;
			}
		}

		internal PlayGamesAchievement()
			: this(global::GooglePlayGames.PlayGamesPlatform.Instance.ReportProgress)
		{
		}

		internal PlayGamesAchievement(global::GooglePlayGames.ReportProgress progressCallback)
		{
			mProgressCallback = progressCallback;
		}

		internal PlayGamesAchievement(global::GooglePlayGames.BasicApi.Achievement ach)
			: this()
		{
			mId = ach.Id;
			mIsIncremental = ach.IsIncremental;
			mCurrentSteps = ach.CurrentSteps;
			mTotalSteps = ach.TotalSteps;
			if (ach.IsIncremental)
			{
				if (ach.TotalSteps > 0)
				{
					mPercentComplete = (double)ach.CurrentSteps / (double)ach.TotalSteps * 100.0;
				}
				else
				{
					mPercentComplete = 0.0;
				}
			}
			else
			{
				mPercentComplete = ((!ach.IsUnlocked) ? 0.0 : 100.0);
			}
			mCompleted = ach.IsUnlocked;
			mHidden = !ach.IsRevealed;
			mLastModifiedTime = ach.LastModifiedTime;
			mTitle = ach.Name;
			mDescription = ach.Description;
			mPoints = ach.Points;
			mRevealedImageUrl = ach.RevealedImageUrl;
			mUnlockedImageUrl = ach.UnlockedImageUrl;
		}

		public void ReportProgress(global::System.Action<bool> callback)
		{
			mProgressCallback(mId, mPercentComplete, callback);
		}

		private global::UnityEngine.Texture2D LoadImage()
		{
			if (hidden)
			{
				return null;
			}
			string text = ((!completed) ? mRevealedImageUrl : mUnlockedImageUrl);
			if (!string.IsNullOrEmpty(text))
			{
				if (mImageFetcher == null || mImageFetcher.url != text)
				{
					mImageFetcher = global::UnityEngine.Networking.UnityWebRequestTexture.GetTexture(text);
					mImageFetcher.SendWebRequest();
					mImage = null;
				}
				if (mImage != null)
				{
					return mImage;
				}
				if (mImageFetcher.isDone)
				{
					if (mImageFetcher.result == global::UnityEngine.Networking.UnityWebRequest.Result.Success)
					{
						mImage = global::UnityEngine.Networking.DownloadHandlerTexture.GetContent(mImageFetcher);
					}
					return mImage;
				}
			}
			return null;
		}
	}
}
