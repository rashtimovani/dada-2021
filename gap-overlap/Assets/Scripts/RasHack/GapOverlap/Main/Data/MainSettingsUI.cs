using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class MainSettingsUI : MonoBehaviour
    {
        #region Fileds

        [SerializeField] private GameObject panel;

        [SerializeField] private InputField gapCentralTime;
        [SerializeField] private InputField gapPauseTime;
        [SerializeField] private InputField gapStimulusTime;

        #endregion

        #region Fields

        private HomeUI home;

        #endregion

        #region API

        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }
        
        #endregion

        #region Unity methods

        private void Start()
        {
            home = FindObjectOfType<HomeUI>();
        }

        public void OnClose()
        {
            Hide();
            home.Show();
        }

        #endregion
    }
}