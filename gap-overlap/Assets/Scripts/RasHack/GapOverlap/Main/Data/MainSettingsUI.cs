using RasHack.GapOverlap.Main.Task;
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
        private Simulator simulator;

        #endregion

        #region API

        public GapTimes GapTimes
        {
            get
            {
                return new GapTimes
                {
                    CentralTime = float.Parse(gapCentralTime.text),
                    PauseTime = float.Parse(gapPauseTime.text),
                    StimulusTime = float.Parse(gapStimulusTime.text)
                };
            }
            set
            {
                gapCentralTime.text = $"{value.CentralTime:0.000}";
                gapPauseTime.text = $"{value.PauseTime:0.000}";
                gapStimulusTime.text = $"{value.StimulusTime:0.000}";
            }
        }

        public void Show()
        {
            panel.SetActive(true);
            GapTimes = simulator.Settings.GapTimes;
        }

        public void Hide()
        {
            panel.SetActive(false);
            simulator.Settings.GapTimes = GapTimes;
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            home = FindObjectOfType<HomeUI>();
            simulator = FindObjectOfType<Simulator>();
        }

        public void OnClose()
        {
            Hide();
            home.Show();
        }

        #endregion
    }
}