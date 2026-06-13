namespace Kampai.Game
{
	public class TimedSocialEventService : global::Kampai.Game.ITimedSocialEventService
	{
		public const string SOCIAL_EVENT_TEAM_BY_USER_ENDPOINT = "/rest/tse/event/{0}/team/user/{1}";

		public const string SOCIAL_EVENT_INVITE_FRIENDS_ENDPOINT = "/rest/tse/event/{0}/team/{1}/user/{2}/invite";

		public const string SOCIAL_EVENT_REJECT_INVITE_ENDPOINT = "/rest/tse/event/{0}/team/{1}/user/{2}/reject";

		public const string SOCIAL_EVENT_JOIN_TEAM_ENDPOINT = "/rest/tse/event/{0}/team/{1}/user/{2}/join";

		public const string SOCIAL_EVENT_LEAVE_TEAM_ENDPOINT = "/rest/tse/event/{0}/team/{1}/user/{2}/leave";

		public const string SOCIAL_EVENT_FILL_ORDER_ENDPOINT = "/rest/tse/event/{0}/team/{1}/user/{2}/order";

		public const string SOCIAL_EVENT_CLAIM_REWARD_ENDPOINT = "/rest/tse/event/{0}/team/{1}/user/{2}/reward";

		public const string SOCIAL_EVENT_TEAMS_ENDPOINT = "/rest/tse/event/{0}/teams";

		public const int CLAIM_REWARD_LIMIT = 259200;

		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("TimedSocialEventService") as global::Kampai.Util.IKampaiLogger;

		private global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.SocialTeamResponse> socialEventCache;

		private bool rewardCutscene;

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Game.IUserSessionService userSessionService { get; set; }

		[Inject]
		public global::Kampai.Game.IPlayerService playerService { get; set; }

		[Inject("game.server.host")]
		public string ServerUrl { get; set; }

		[Inject]
		public global::Kampai.Splash.IDownloadService downloadService { get; set; }

		[Inject]
		public global::Ea.Sharkbite.HttpPlugin.Http.Api.IRequestFactory requestFactory { get; set; }

		public TimedSocialEventService()
		{
			socialEventCache = new global::System.Collections.Generic.Dictionary<int, global::Kampai.Game.SocialTeamResponse>();
		}

		public void ClearCache()
		{
			socialEventCache.Clear();
		}

		public global::Kampai.Game.TimedSocialEventDefinition GetCurrentSocialEvent()
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> all = definitionService.GetAll<global::Kampai.Game.TimedSocialEventDefinition>();
			if (all == null || all.Count == 0) return null;
			
			int num = timeService.CurrentTime();
			
			// First pass: look for a truly active event
			foreach (global::Kampai.Game.TimedSocialEventDefinition item in all)
			{
				if (item.StartTime <= num && item.FinishTime >= num)
				{
					return item;
				}
			}
			
			// Second pass: if all events are in the past, loop them!
			// Find the total span of all events to determine the cycle length
			int minStart = int.MaxValue;
			int maxFinish = int.MinValue;
			foreach (var item in all)
			{
				if (item.OriginalStartTime < minStart) minStart = item.OriginalStartTime;
				if (item.OriginalFinishTime > maxFinish) maxFinish = item.OriginalFinishTime;
			}
			
			if (num > maxFinish && maxFinish > minStart)
			{
				int cycleDuration = maxFinish - minStart + 3600; // Add 1 hour gap between cycles
				int offset = ((num - minStart) / cycleDuration) * cycleDuration;
				
				foreach (var item in all)
				{
					if (item.OriginalStartTime + offset <= num && item.OriginalFinishTime + offset >= num)
					{
						item.StartTime = item.OriginalStartTime + offset;
						item.FinishTime = item.OriginalFinishTime + offset;
						return item;
					}
				}
			}
			
			return null;
		}

		public global::Kampai.Game.TimedSocialEventDefinition GetNextSocialEvent()
		{
			global::Kampai.Game.TimedSocialEventDefinition result = null;
			int num = int.MaxValue;
			global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> all = definitionService.GetAll<global::Kampai.Game.TimedSocialEventDefinition>();
			if (all == null || all.Count == 0) return null;

			int currentTime = timeService.CurrentTime();
			
			// First pass: standard logic
			foreach (global::Kampai.Game.TimedSocialEventDefinition item in all)
			{
				int startTime = item.StartTime;
				int timeUntilStart = startTime - currentTime;
				if (startTime > currentTime && timeUntilStart < num)
				{
					num = timeUntilStart;
					result = item;
				}
			}
			
			if (result != null) return result;

			// Second pass: loop logic
			int minStart = int.MaxValue;
			int maxFinish = int.MinValue;
			foreach (var item in all)
			{
				if (item.OriginalStartTime < minStart) minStart = item.OriginalStartTime;
				if (item.OriginalFinishTime > maxFinish) maxFinish = item.OriginalFinishTime;
			}

			if (maxFinish > minStart)
			{
				int cycleDuration = maxFinish - minStart + 3600;
				int offset = ((currentTime - minStart) / cycleDuration + 1) * cycleDuration;
				
				num = int.MaxValue;
				foreach (var item in all)
				{
					int shiftedStart = item.OriginalStartTime + offset;
					int timeUntilStart = shiftedStart - currentTime;
					if (shiftedStart > currentTime && timeUntilStart < num)
					{
						num = timeUntilStart;
						result = item;
						result.StartTime = shiftedStart;
						result.FinishTime = item.OriginalFinishTime + offset;
					}
				}
			}

			return result;
		}

		public global::Kampai.Game.TimedSocialEventDefinition GetSocialEvent(int id)
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> all = definitionService.GetAll<global::Kampai.Game.TimedSocialEventDefinition>();
			if (all == null)
			{
				logger.Warning("GetSocialEvent not found");
				return null;
			}
			foreach (global::Kampai.Game.TimedSocialEventDefinition item in all)
			{
				if (item.ID == id)
				{
					return item;
				}
			}
			logger.Warning("GetSocialEvent not found with id {0}", id);
			return null;
		}

		public void GetSocialEventState(int eventID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/user/{1}", eventID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithResponseSignal(signal));
		}

		public global::Kampai.Game.SocialTeamResponse GetSocialEventStateCached(int eventID)
		{
			if (socialEventCache != null)
			{
				if (socialEventCache.ContainsKey(eventID))
				{
					return socialEventCache[eventID];
				}
				logger.Error("Social event not found in cache {0}", eventID);
			}
			return null;
		}

		public void CreateSocialTeam(int eventID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/user/{1}", eventID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithResponseSignal(signal));
		}

		public void JoinSocialTeam(int eventID, long teamID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/{1}/user/{2}/join", eventID, teamID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithResponseSignal(signal));
		}

		public void LeaveSocialTeam(int eventID, long teamID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/{1}/user/{2}/leave", eventID, teamID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithResponseSignal(signal));
		}

		public void InviteFriends(int eventID, long teamID, global::Kampai.Game.IdentityType identityType, global::System.Collections.Generic.IList<string> externalIDs, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			global::Kampai.Game.InviteFriendsRequest inviteFriendsRequest = new global::Kampai.Game.InviteFriendsRequest();
			inviteFriendsRequest.IdentityType = identityType;
			inviteFriendsRequest.ExternalIds = externalIDs;
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/{1}/user/{2}/invite", eventID, teamID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithEntity(inviteFriendsRequest)
				.WithResponseSignal(signal));
		}

		public void RejectInvitation(int eventID, long teamID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/{1}/user/{2}/reject", eventID, teamID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithResponseSignal(signal));
		}

		public void FillOrder(int eventID, long teamID, int orderID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnFillOrderResponse(resultSignal, response);
			});
			global::Kampai.Game.FillOrderRequest fillOrderRequest = new global::Kampai.Game.FillOrderRequest();
			fillOrderRequest.OrderID = orderID;
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/{1}/user/{2}/order", eventID, teamID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithEntity(fillOrderRequest)
				.WithResponseSignal(signal));
		}

		public void ClaimReward(int eventID, long teamID, global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			if (userSession == null)
			{
				logger.Error("User is not logged in. Can't get social team for event {0}", eventID);
				return;
			}
			string userID = userSession.UserID;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamResponse(resultSignal, response);
			});
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/team/{1}/user/{2}/reward", eventID, teamID, userID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithResponseSignal(signal));
		}

		public void GetSocialTeams(int eventID, global::System.Collections.Generic.IList<long> teamIds, global::strange.extensions.signal.impl.Signal<global::System.Collections.Generic.Dictionary<long, global::Kampai.Game.SocialTeam>> resultSignal)
		{
			global::Kampai.Game.UserSession userSession = userSessionService.UserSession;
			global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse> signal = new global::strange.extensions.signal.impl.Signal<global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse>();
			signal.AddListener(delegate(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
			{
				OnGetTeamsResponse(resultSignal, response);
			});
			global::Kampai.Game.GetTeamsRequest getTeamsRequest = new global::Kampai.Game.GetTeamsRequest();
			getTeamsRequest.TeamIDs = teamIds;
			downloadService.Perform(requestFactory.Resource(ServerUrl + string.Format("/rest/tse/event/{0}/teams", eventID)).WithHeaderParam("user_id", userSession.UserID).WithHeaderParam("session_key", userSession.SessionID)
				.WithContentType("application/json")
				.WithMethod("POST")
				.WithEntity(getTeamsRequest)
				.WithResponseSignal(signal));
		}

		private void OnGetTeamResponse(global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal, global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			if (response.Success)
			{
				string body = response.Body;
				global::Kampai.Game.SocialTeamResponse socialTeamResponse = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.SocialTeamResponse>(body, new global::Newtonsoft.Json.JsonConverter[1]
				{
					new global::Kampai.Game.SocialTeamConverter(definitionService)
				});
				socialEventCache[socialTeamResponse.EventId] = socialTeamResponse;
				UpdateClaimRewardForPastEvent(socialTeamResponse);
				if (resultSignal != null)
				{
					resultSignal.Dispatch(socialTeamResponse, null);
				}
			}
			else
			{
				logger.Warning("Failed to get social team", response.Code);
				if (resultSignal != null)
				{
					global::Kampai.Game.ErrorResponse errorResponse = GetErrorResponse(response);
					resultSignal.Dispatch(null, errorResponse);
				}
			}
		}

		private void OnFillOrderResponse(global::strange.extensions.signal.impl.Signal<global::Kampai.Game.SocialTeamResponse, global::Kampai.Game.ErrorResponse> resultSignal, global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			string body = response.Body;
			if (response.Success)
			{
				global::Kampai.Game.SocialTeamResponse socialTeamResponse = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.SocialTeamResponse>(body, new global::Newtonsoft.Json.JsonConverter[1]
				{
					new global::Kampai.Game.SocialTeamConverter(definitionService)
				});
				socialEventCache[socialTeamResponse.EventId] = socialTeamResponse;
				if (resultSignal != null)
				{
					resultSignal.Dispatch(socialTeamResponse, null);
				}
			}
			else
			{
				logger.Warning("Failed to fill order in social event");
				if (resultSignal != null)
				{
					global::Kampai.Game.ErrorResponse errorResponse = GetErrorResponse(response);
					resultSignal.Dispatch(null, errorResponse);
				}
			}
		}

		private void OnGetTeamsResponse(global::strange.extensions.signal.impl.Signal<global::System.Collections.Generic.Dictionary<long, global::Kampai.Game.SocialTeam>> resultSignal, global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			if (response.Success)
			{
				if (resultSignal != null)
				{
					string body = response.Body;
					global::System.Collections.Generic.Dictionary<long, global::Kampai.Game.SocialTeam> type = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::System.Collections.Generic.Dictionary<long, global::Kampai.Game.SocialTeam>>(body, new global::Newtonsoft.Json.JsonConverter[1]
					{
						new global::Kampai.Game.SocialTeamConverter(definitionService)
					});
					resultSignal.Dispatch(type);
				}
			}
			else
			{
				logger.Warning("Failed to get list of social teams", response.Code);
				if (resultSignal != null)
				{
					resultSignal.Dispatch(null);
				}
			}
		}

		private global::Kampai.Game.ErrorResponse GetErrorResponse(global::Ea.Sharkbite.HttpPlugin.Http.Api.IResponse response)
		{
			string body = response.Body;
			global::Kampai.Game.ErrorResponse errorResponse = null;
			try
			{
				errorResponse = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.ErrorResponse>(body);
			}
			catch (global::System.Exception)
			{
				errorResponse = new global::Kampai.Game.ErrorResponse();
				global::Kampai.Game.ErrorResponseContent errorResponseContent = new global::Kampai.Game.ErrorResponseContent();
				errorResponseContent.ResponseCode = response.Code;
				errorResponseContent.Code = 0;
				errorResponseContent.Message = "unknown";
				errorResponse.Error = errorResponseContent;
			}
			return errorResponse;
		}

		public global::Kampai.Game.TimedSocialEventDefinition GetCurrentTimedSocialEventDefinition()
		{
			global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> all = definitionService.GetAll<global::Kampai.Game.TimedSocialEventDefinition>();
			int num = timeService.CurrentTime();
			foreach (global::Kampai.Game.TimedSocialEventDefinition item in all)
			{
				int startTime = item.StartTime;
				int finishTime = item.FinishTime;
				if (num >= startTime && num < finishTime)
				{
					return item;
				}
			}
			return null;
		}

		public void setRewardCutscene(bool cutscene)
		{
			rewardCutscene = cutscene;
		}

		public bool isRewardCutscene()
		{
			return rewardCutscene;
		}

		public global::System.Collections.Generic.IList<int> GetPastEventsWithUnclaimedReward()
		{
			global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
			global::System.Collections.Generic.List<int> list2 = new global::System.Collections.Generic.List<int>();
			int num = timeService.CurrentTime();
			global::System.Collections.Generic.IList<global::Kampai.Game.TimedSocialEventDefinition> all = definitionService.GetAll<global::Kampai.Game.TimedSocialEventDefinition>();
			foreach (global::Kampai.Game.TimedSocialEventDefinition item in all)
			{
				int iD = item.ID;
				if (item.FinishTime >= num)
				{
					list2.Add(iD);
				}
				else
				{
					if (num - item.FinishTime >= 259200)
					{
						continue;
					}
					list2.Add(iD);
					switch (playerService.GetSocialClaimReward(iD))
					{
					case global::Kampai.Game.SocialClaimRewardItem.ClaimState.EVENT_COMPLETED_REWARD_NOT_CLAIMED:
						list.Add(iD);
						if (!socialEventCache.ContainsKey(iD))
						{
							GetSocialEventState(iD, null);
						}
						break;
					case global::Kampai.Game.SocialClaimRewardItem.ClaimState.UNKNOWN:
						GetSocialEventState(iD, null);
						break;
					}
				}
			}
			playerService.CleanupSocialClaimReward(list2);
			return list;
		}

		private void UpdateClaimRewardForPastEvent(global::Kampai.Game.SocialTeamResponse teamResponse)
		{
			global::Kampai.Game.TimedSocialEventDefinition timedSocialEventDefinition = definitionService.Get<global::Kampai.Game.TimedSocialEventDefinition>(teamResponse.EventId);
			int num = timeService.CurrentTime();
			if (timedSocialEventDefinition == null || timedSocialEventDefinition.FinishTime > num)
			{
				return;
			}
			if (teamResponse.UserEvent == null)
			{
				playerService.AddSocialClaimReward(teamResponse.EventId, global::Kampai.Game.SocialClaimRewardItem.ClaimState.EVENT_NOT_COMPLETED);
			}
			else if (teamResponse.UserEvent.RewardClaimed)
			{
				playerService.AddSocialClaimReward(teamResponse.EventId, global::Kampai.Game.SocialClaimRewardItem.ClaimState.REWARD_CLAIMED);
			}
			else if (teamResponse.Team != null && teamResponse.Team.OrderProgress != null)
			{
				if (teamResponse.Team.OrderProgress.Count == timedSocialEventDefinition.Orders.Count)
				{
					playerService.AddSocialClaimReward(teamResponse.EventId, global::Kampai.Game.SocialClaimRewardItem.ClaimState.EVENT_COMPLETED_REWARD_NOT_CLAIMED);
				}
				else
				{
					playerService.AddSocialClaimReward(teamResponse.EventId, global::Kampai.Game.SocialClaimRewardItem.ClaimState.EVENT_NOT_COMPLETED);
				}
			}
		}
	}
}
