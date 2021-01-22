using RasHack.GapOverlap.Main.Data;
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

        private GapTimes GapTimes
        {
            get => new GapTimes
            {
                CentralTime = ParseInput(gapCentralTime, 1f),
                PauseTime = ParseInput(gapPauseTime, 0.5f),
                StimulusTime = ParseInput(gapStimulusTime, 2f)
            };

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

            Display();
        }

        public void Hide(bool dontStore = false)
        {
            panel.SetActive(false);

            simulator.Settings.GapTimes = GapTimes;

            if (!dontStore) simulator.Settings.Store();
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

        public void OnReset()
        {
            var defaults = new MainSettings();

            simulator.Settings.GapTimes = defaults.GapTimes;
            
            Display();
        }

        #endregion

        #region Helpers

        private static float ParseInput(InputField field, float defaultValue)
        {
            return string.IsNullOrWhiteSpace(field.text) ? defaultValue : float.Parse(field.text);
        }

        private void Display()
        {
            GapTimes = simulator.Settings.GapTimes;
        }

        #endregion
    }
}