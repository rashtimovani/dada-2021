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

        [SerializeField] private InputField distanceBetweenStimuli;
        [SerializeField] private InputField centralSize;
        [SerializeField] private InputField peripheralSize;

        #endregion

        #region Fields

        private HomeUI home;
        private Simulator simulator;
        private readonly MainSettings defaults = new MainSettings();

        #endregion

        #region General

        [Header("General")] [SerializeField] private Dropdown background;
        
        private BackgroundColor Background
        {
            get => (BackgroundColor) background.value;
            set => background.value = (int) value;
        }
        
        #endregion

        #region Pauses

        [Header("Pauses")] [SerializeField] private FloatInput pauseBeforeTasks;
        [SerializeField] private FloatInput pauseBetweenTasks;
        [SerializeField] private FloatInput pauseAfterTasks;

        #endregion

        #region Screen
        
        [Header("Screen")] [SerializeField] private FloatInput screenDiagonal;
        [SerializeField] private IntInput eyeTrackerDistance;

        private ReferencePoint ReferencePoint
        {
            get => new ReferencePoint
            {
                ScreenDiagonalInInches = screenDiagonal.Value,
                DistanceFromEyesInCM = ToCM(eyeTrackerDistance.Value)
            };
            set
            {
                screenDiagonal.Value = value.ScreenDiagonalInInches;
                eyeTrackerDistance.Value = ToMM(value.DistanceFromEyesInCM);
            }
        }

        private ReferencePoint ResetReferencePoint()
        {
            screenDiagonal.Reset();
            eyeTrackerDistance.Reset();
            return ReferencePoint;
        }

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

            simulator.Settings.Background = Background;

            simulator.Settings.PauseBeforeTasks = pauseBeforeTasks.Value;
            simulator.Settings.PauseBetweenTasks = pauseBetweenTasks.Value;
            simulator.Settings.PauseAfterTasks = pauseAfterTasks.Value;

            simulator.Settings.ReferencePoint = ReferencePoint;
            
            simulator.Settings.GapTimes = GapTimes;
            simulator.Settings.OverlapTimes = OverlapTimes;
            simulator.Settings.TaskCount = TaskCount;
            
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

            pauseBeforeTasks.SetDefault(() => defaults.PauseBeforeTasks);
            pauseBetweenTasks.SetDefault(() => defaults.PauseBetweenTasks);
            pauseAfterTasks.SetDefault(() => defaults.PauseAfterTasks);

            screenDiagonal.SetDefault(() => defaults.ReferencePoint.ScreenDiagonalInInches);
            eyeTrackerDistance.SetDefault(() => ToMM(defaults.ReferencePoint.DistanceFromEyesInCM));
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

            simulator.Settings.Background = defaults.Background;

            simulator.Settings.PauseBeforeTasks = pauseBeforeTasks.Reset();
            simulator.Settings.PauseBetweenTasks = pauseBetweenTasks.Reset();
            simulator.Settings.PauseAfterTasks = pauseAfterTasks.Reset();

            simulator.Settings.ReferencePoint = ResetReferencePoint();
            
            simulator.Settings.GapTimes = defaults.GapTimes;
            simulator.Settings.OverlapTimes = defaults.OverlapTimes;
            simulator.Settings.TaskCount = defaults.TaskCount;
            simulator.Settings.PauseBetweenTasks = defaults.PauseBetweenTasks;
            
            simulator.Settings.StimulusDistanceInDegrees = defaults.StimulusDistanceInDegrees;
            simulator.Settings.CentralStimulusSizeInDegrees = defaults.CentralStimulusSizeInDegrees;
            simulator.Settings.PeripheralStimulusSizeInDegrees = defaults.PeripheralStimulusSizeInDegrees;

            Display();
        }

        #endregion

        #region Helpers

        private static int ToMM(float cm)
        {
            return Mathf.RoundToInt(cm * 10f);
        }

        private static float ToCM(int mm)
        {
            return mm / 10f;
        }

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
            Background = simulator.Settings.Background;

            pauseBeforeTasks.Value = simulator.Settings.PauseBeforeTasks;
            pauseBetweenTasks.Value = simulator.Settings.PauseBetweenTasks;
            pauseAfterTasks.Value = simulator.Settings.PauseAfterTasks;

            ReferencePoint = simulator.Settings.ReferencePoint;
            
            GapTimes = simulator.Settings.GapTimes;
            OverlapTimes = simulator.Settings.OverlapTimes;
            TaskCount = simulator.Settings.TaskCount;
            // PauseBetweenTasks = simulator.Settings.PauseBetweenTasks;
            
            StimulusDistanceInDegrees = simulator.Settings.StimulusDistanceInDegrees;
            CentralStimulusSizeInDegrees = simulator.Settings.CentralStimulusSizeInDegrees;
            PeripheralStimulusSizeInDegrees = simulator.Settings.PeripheralStimulusSizeInDegrees;
        }

        #endregion
    }
}