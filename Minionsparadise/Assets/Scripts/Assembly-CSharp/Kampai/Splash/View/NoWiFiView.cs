namespace Kampai.Splash.View
{
    public class NoWiFiView : global::Kampai.Util.KampaiView
    {
        public global::Kampai.UI.View.ButtonView continueButton1;
        public global::Kampai.UI.View.ButtonView exitButton1;
        public global::Kampai.UI.View.ButtonView continueButton2;
        public global::Kampai.UI.View.ButtonView exitButton2;
        public global::Kampai.UI.View.ButtonView settingsButton;
        public global::UnityEngine.UI.Text titleText;
        public global::UnityEngine.UI.Text messageText;
        public global::UnityEngine.UI.Text continueText;
        public global::UnityEngine.UI.Text settingsText;
        public global::UnityEngine.GameObject panel_CTA_1up;
        public global::UnityEngine.GameObject panel_CTA_2up;
        
        public void Init(bool twoButtons)
        {
            continueButton1.gameObject.SetActive(twoButtons);
            exitButton1.gameObject.SetActive(twoButtons);
            continueButton2.gameObject.SetActive(!twoButtons);
            exitButton2.gameObject.SetActive(!twoButtons);
            settingsButton.gameObject.SetActive(!twoButtons);
            if (panel_CTA_1up != null)
                panel_CTA_1up.SetActive(twoButtons);
            if (panel_CTA_2up != null)
                panel_CTA_2up.SetActive(!twoButtons);
        }
    }
}
