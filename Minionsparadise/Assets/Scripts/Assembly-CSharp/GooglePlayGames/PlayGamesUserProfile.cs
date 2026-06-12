namespace GooglePlayGames
{
	public class PlayGamesUserProfile : global::UnityEngine.SocialPlatforms.IUserProfile
	{
		private string mDisplayName;

		private string mPlayerId;

		private string mAvatarUrl;

		private volatile bool mImageLoading;

		private global::UnityEngine.Texture2D mImage;

		public string userName
		{
			get
			{
				return mDisplayName;
			}
		}

		public string id
		{
			get
			{
				return mPlayerId;
			}
		}

		public bool isFriend
		{
			get
			{
				return true;
			}
		}

		public global::UnityEngine.SocialPlatforms.UserState state
		{
			get
			{
				return global::UnityEngine.SocialPlatforms.UserState.Online;
			}
		}

		public global::UnityEngine.Texture2D image
		{
			get
			{
				if (!mImageLoading && mImage == null && !string.IsNullOrEmpty(AvatarURL))
				{
					global::UnityEngine.Debug.Log("Starting to load image: " + AvatarURL);
					mImageLoading = true;
					global::GooglePlayGames.OurUtils.PlayGamesHelperObject.RunCoroutine(LoadImage());
				}
				return mImage;
			}
		}

		public string AvatarURL
		{
			get
			{
				return mAvatarUrl;
			}
		}

		internal PlayGamesUserProfile(string displayName, string playerId, string avatarUrl)
		{
			mDisplayName = displayName;
			mPlayerId = playerId;
			mAvatarUrl = avatarUrl;
			mImageLoading = false;
		}

		protected void ResetIdentity(string displayName, string playerId, string avatarUrl)
		{
			mDisplayName = displayName;
			mPlayerId = playerId;
			mAvatarUrl = avatarUrl;
			mImageLoading = false;
		}

		internal global::System.Collections.IEnumerator LoadImage()
		{
			if (!string.IsNullOrEmpty(AvatarURL))
			{
				using (global::UnityEngine.Networking.UnityWebRequest webRequest = global::UnityEngine.Networking.UnityWebRequestTexture.GetTexture(AvatarURL))
				{
					yield return webRequest.SendWebRequest();
					if (webRequest.result == global::UnityEngine.Networking.UnityWebRequest.Result.Success)
					{
						mImage = global::UnityEngine.Networking.DownloadHandlerTexture.GetContent(webRequest);
					}
					else
					{
						mImage = global::UnityEngine.Texture2D.blackTexture;
						global::UnityEngine.Debug.Log("Error downloading image: " + webRequest.error);
					}
				}
				mImageLoading = false;
			}
			else
			{
				global::UnityEngine.Debug.Log("No URL found.");
				mImage = global::UnityEngine.Texture2D.blackTexture;
				mImageLoading = false;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			global::GooglePlayGames.PlayGamesUserProfile playGamesUserProfile = obj as global::GooglePlayGames.PlayGamesUserProfile;
			if (playGamesUserProfile == null)
			{
				return false;
			}
			return global::System.StringComparer.Ordinal.Equals(mPlayerId, playGamesUserProfile.mPlayerId);
		}

		public override int GetHashCode()
		{
			return typeof(global::GooglePlayGames.PlayGamesUserProfile).GetHashCode() ^ mPlayerId.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[Player: '{0}' (id {1})]", mDisplayName, mPlayerId);
		}
	}
}
