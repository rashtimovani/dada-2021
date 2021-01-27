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

        [Header("General")] [SerializeField] private Dropdown background;

        [Header("Pauses")] [SerializeField] private TimeInput pauseBeforeTasks;
        [SerializeField] private TimeInput pauseBetweenTasks;
        [SerializeField] private TimeInput pauseAfterTasks;

        [SerializeField] private InputField gapCentralTime;
        [SerializeField] private InputField gapPauseTime;
        [SerializeField] private InputField gapStimulusTime;

        [SerializeField] private InputField overlapCentralTime;
        [SerializeField] private InputField overlapStimulusTime;

        [SerializeField] private InputField gapsInput;
        [SerializeField] private InputField overlapsInput;

        [SerializeField] private InputField screenDiagonal;
        [SerializeField] private InputField eyeTrackerDistance;

        [SerializeField] private InputField distanceBetweenStimuli;
        [SerializeField] private InputField centralSize;
        [SerializeField] private InputField peripheralSize;

        #endregion

        #region Fields

        private HomeUI home;
        private Simulator simulator;
        private readonly MainSettings defaults = new MainSettings();

        #endregion

        #region API

        private GapTimes GapTimes
        {
            get => new GapTimes
            {
                CentralTime = ParseInput(gapCentralTime, defaults.GapTimes.CentralTime),
                PauseTime = ParseInput(gapPauseTime, defaults.GapTimes.PauseTime),
                StimulusTime = ParseInput(gapStimulusTime, defaults.GapTimes.StimulusTime)
            };

            set
            {
                gapCentralTime.text = $"{value.CentralTime:0.00}";
                gapPauseTime.text = $"{value.PauseTime:0.00}";
                gapStimulusTime.text = $"{value.StimulusTime:0.00}";
            }
        }

        private OverlapTimes OverlapTimes
        {
            get => new OverlapTimes
            {
                CentralTime = ParseInput(overlapCentralTime, defaults.OverlapTimes.CentralTime),
                BothStimuli = ParseInput(overlapStimulusTime, defaults.OverlapTimes.BothStimuli),
            };

            set
            {
                overlapCentralTime.text = $"{value.CentralTime:0.00}";
                overlapStimulusTime.text = $"{value.BothStimuli:0.00}";
            }
        }

        private TaskCount TaskCount
        {
            get => new TaskCount
            {
                Gaps = ParseInput(gapsInput, defaults.TaskCount.Gaps),
                Overlaps = ParseInput(overlapsInput, defaults.TaskCount.Overlaps),
            };

            set
            {
                gapsInput.text = $"{value.Gaps}";
                overlapsInput.text = $"{value.Overlaps}";
            }
        }


        private BackgroundColor BackgroundColor
        {
            get => (BackgroundColor) background.value;
            set => background.value = (int) value;
        }

        private ReferencePoint ReferencePoint
        {
            get => new ReferencePoint
            {
                ScreenDiagonalInInches = ParseInput(screenDiagonal, defaults.ReferencePoint.ScreenDiagonalInInches),
                DistanceFromEyesInCM = ParseInput(eyeTrackerDistance, defaults.ReferencePoint.DistanceFromEyesInCM),
            };
            set
            {
                screenDiagonal.text = $"{value.ScreenDiagonalInInches:0.0}";
                eyeTrackerDistance.text = $"{value.DistanceFromEyesInCM:0.0}";
            }
        }

        private float StimulusDistanceInDegrees
        {
            get => ParseInput(distanceBetweenStimuli, defaults.StimulusDistanceInDegrees);
            set => distanceBetweenStimuli.text = $"{value:0.0}";
        }

        private float CentralStimulusSizeInDegrees
        {
            get => ParseInput(centralSize, defaults.CentralStimulusSizeInDegrees);
            set => centralSize.text = $"{value:0.0}";
        }

        private float PeripheralStimulusSizeInDegrees
        {
            get => ParseInput(peripheralSize, defaults.PeripheralStimulusSizeInDegrees);
            set => peripheralSize.text = $"{value:0.0}";
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
            // simulator.Settings.PauseBetweenTasks = PauseBetweenTasks;
            simulator.Settings.BackgroundColor = BackgroundColor;
            simulator.Settings.ReferencePoint = ReferencePoint;
            simulator.Settings.StimulusDistanceInDegrees = StimulusDistanceInDegrees;
            simulator.Settings.CentralStimulusSizeInDegrees = CentralStimulusSizeInDegrees;
            simulator.Settings.PeripheralStimulusSizeInDegrees = PeripheralStimulusSizeInDegrees;

            simulator.Settings.Store();
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            home = FindObjectOfType<HomeUI>();
            simulator = FindObjectOfType<Simulator>();

            var defaults = new MainSettings();

            pauseBeforeTasks.SetDefault(() => defaults.PauseBeforeTasks);
            pauseBetweenTasks.SetDefault(() => defaults.PauseBetweenTasks);
            pauseAfterTasks.SetDefault(() => defaults.PauseAfterTasks);
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

            simulator.Settings.BackgroundColor = defaults.BackgroundColor;

            simulator.Settings.PauseBeforeTasks = pauseBeforeTasks.Reset();
            simulator.Settings.PauseBetweenTasks = pauseBetweenTasks.Reset();
            simulator.Settings.PauseAfterTasks = pauseAfterTasks.Reset();

            simulator.Settings.GapTimes = defaults.GapTimes;
            simulator.Settings.OverlapTimes = defaults.OverlapTimes;
            simulator.Settings.TaskCount = defaults.TaskCount;
            simulator.Settings.PauseBetweenTasks = defaults.PauseBetweenTasks;
            simulator.Settings.ReferencePoint = defaults.ReferencePoint;
            simulator.Settings.StimulusDistanceInDegrees = defaults.StimulusDistanceInDegrees;
            simulator.Settings.CentralStimulusSizeInDegrees = defaults.CentralStimulusSizeInDegrees;
            simulator.Settings.PeripheralStimulusSizeInDegrees = defaults.PeripheralStimulusSizeInDegrees;

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
            // PauseBetweenTasks = simulator.Settings.PauseBetweenTasks;
            BackgroundColor = simulator.Settings.BackgroundColor;
            ReferencePoint = simulator.Settings.ReferencePoint;
            StimulusDistanceInDegrees = simulator.Settings.StimulusDistanceInDegrees;
            CentralStimulusSizeInDegrees = simulator.Settings.CentralStimulusSizeInDegrees;
            PeripheralStimulusSizeInDegrees = simulator.Settings.PeripheralStimulusSizeInDegrees;
        }

        #endregion
    }
}