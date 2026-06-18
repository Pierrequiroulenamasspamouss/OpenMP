using System;
using Kampai.Game;
using Kampai.Common;
using Kampai.UI.View;
using strange.extensions.command.impl;

namespace Kampai.UI.Controller
{
    public class TransitionToOfflineModeCommand : Command
    {
        [Inject]
        public IUserSessionService userSessionService { get; set; }

        [Inject]
        public ShowOfflinePopupSignal showOfflinePopupSignal { get; set; }

        [Inject]
        public IConfigurationsService configurationsService { get; set; }

        [Inject]
        public ILocalPersistanceService localPersistService { get; set; }

        [Inject]
        public LoadPlayerSignal loadPlayerSignal { get; set; }

        [Inject]
        public LoginUserSignal loginUserSignal { get; set; }

        [Inject]
        public ICoppaService coppaService { get; set; }

        [Inject]
        public NetworkModel networkModel { get; set; }

        public override void Execute()
        {
            if (userSessionService.IsOffline)
            {
                UnityEngine.Debug.Log("[OfflineMode] Already in offline mode. Skipping duplicate transition.");
                return;
            }
            
            userSessionService.IsOffline = true;

            // In offline mode, we are deliberately disconnected — this is not a "connection lost" scenario.
            // Clear the flag so gameplay systems (minigames, etc.) don't pause.
            networkModel.isConnectionLost = false;

            // Close the popup
            showOfflinePopupSignal.Dispatch(false);

            // If we don't have a UserID yet (first launch offline), create a dummy one
            string userId = localPersistService.GetData("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                string offlineUid = "OFFLINE_" + DateTime.Now.Ticks.ToString();
                localPersistService.PutData("UserID", offlineUid);
            }

            // In offline mode, ensure COPPA birthdate is set so marketplace can initialize.
            // Without this, IsBirthdateKnown() returns false, InitMarketplaceSlotsIfNeeded() is skipped,
            // and the marketplace panels remain empty.
            if (!coppaService.IsBirthdateKnown())
            {
                // Set an adult birthdate (year=2000, month=1) to bypass COPPA gate in offline mode
                coppaService.SetUserBirthdate(new DateTime(2000, 1, 1));
                UnityEngine.Debug.Log("[OfflineMode] Auto-set COPPA birthdate for offline marketplace access.");
            }

            // Continue the loading flow using local data
            if (configurationsService.GetConfigurations() == null)
            {
                configurationsService.LoadLocalConfiguration();
            }
            else
            {
                loginUserSignal.Dispatch();
            }
        }
    }
}
