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

        [SerializeField] private InputField overlapCentralTime;
        [SerializeField] private InputField overlapStimulusTime;

        [SerializeField] private InputField gapsInput;
        [SerializeField] private InputField overlapsInput;

        [SerializeField] private InputField pauseInput;
        
        [SerializeField] private Dropdown backgroundInput;

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

        private OverlapTimes OverlapTimes
        {
            get => new OverlapTimes
            {
                CentralTime = ParseInput(overlapCentralTime, 1f),
                BothStimuli = ParseInput(overlapStimulusTime, 2.5f),
            };

            set
            {
                overlapCentralTime.text = $"{value.CentralTime:0.000}";
                overlapStimulusTime.text = $"{value.BothStimuli:0.000}";
            }
        }

        private TaskCount TaskCount
        {
            get => new TaskCount
            {
                Gaps = ParseInput(gapsInput, 18),
                Overlaps = ParseInput(overlapsInput, 18),
            };

            set
            {
                gapsInput.text = $"{value.Gaps}";
                overlapsInput.text = $"{value.Overlaps}";
            }
        }

        private float PauseBetweenTasks
        {
            get => ParseInput(pauseInput, 3.5f);
            set => pauseInput.text = $"{value:0.000}";
        }
        
        private BackgroundColor BackgroundColor
        {
            get => (BackgroundColor) backgroundInput.value;
            set => backgroundInput.value = (int) value;
        }

        public void Show()
        {
            panel.SetActive(true);

            Display();
        }

        public void Hide(bool dontStore = false)
        {
            panel.SetActive(false);
            if (dontStore) return;

            simulator.Settings.GapTimes = GapTimes;
            simulator.Settings.OverlapTimes = OverlapTimes;
            simulator.Settings.TaskCount = TaskCount;
            simulator.Settings.PauseBetweenTasks = PauseBetweenTasks;
            simulator.Settings.BackgroundColor = BackgroundColor;

            simulator.Settings.Store();
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
            simulator.UpdateBackground();
        }

        public void OnReset()
        {
            var defaults = new MainSettings();

            simulator.Settings.GapTimes = defaults.GapTimes;
            simulator.Settings.OverlapTimes = defaults.OverlapTimes;
            simulator.Settings.TaskCount = defaults.TaskCount;
            simulator.Settings.PauseBetweenTasks = defaults.PauseBetweenTasks;
            simulator.Settings.BackgroundColor = defaults.BackgroundColor;

            Display();
        }

        #endregion

        #region Helpers

        private static float ParseInput(InputField field, float defaultValue)
        {
            return string.IsNullOrWhiteSpace(field.text) ? defaultValue : float.Parse(field.text);
        }

        private static int ParseInput(InputField field, int defaultValue)
        {
            return string.IsNullOrWhiteSpace(field.text) ? defaultValue : int.Parse(field.text);
        }

        private void Display()
        {
            GapTimes = simulator.Settings.GapTimes;
            OverlapTimes = simulator.Settings.OverlapTimes;
            TaskCount = simulator.Settings.TaskCount;
            PauseBetweenTasks = simulator.Settings.PauseBetweenTasks;
            BackgroundColor = simulator.Settings.BackgroundColor;
        }

        #endregion
    }
}