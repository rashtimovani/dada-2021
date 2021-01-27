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


        [SerializeField] private InputField overlapCentralTime;
        [SerializeField] private InputField overlapStimulusTime;

        [SerializeField] private InputField overlapsInput;

        #endregion

        #region Fields

        private readonly MainSettings defaults = new MainSettings();

        private HomeUI home;
        private Simulator simulator;

        #endregion

        #region General

        [Header("General")] [SerializeField] private Dropdown background;

        private BackgroundColor Background
        {
            get => (BackgroundColor) background.value;
            set => background.value = (int) value;
        }

        private BackgroundColor ResetBackground()
        {
            Background = defaults.Background;
            return Background;
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

        #region Stimuli dimensions

        [Header("Stimuli dimensions")] [SerializeField]
        private FloatInput distanceBetweenPeripheralStimuli;

        [SerializeField] private FloatInput centralStimulusSize;
        [SerializeField] private FloatInput peripheralStimulusSize;

        #endregion

        #region Gaps

        [Header("Gap settings")] [SerializeField]
        private FloatInput gapCentralStimulusTime;

        [SerializeField] private FloatInput gapPauseBetweenStimuli;
        [SerializeField] private FloatInput gapPeripheralStimulusTime;
        [SerializeField] private IntInput gapTaskCount;
        
        private GapTimes GapTimes
        {
            get => new GapTimes
            {
                CentralTime = gapCentralStimulusTime.Value,
                PauseTime = gapPauseBetweenStimuli.Value,
                StimulusTime = gapPeripheralStimulusTime.Value
            };

            set
            {
                gapCentralStimulusTime.Value = value.CentralTime;
                gapPauseBetweenStimuli.Value = value.PauseTime;
                gapPeripheralStimulusTime.Value = value.StimulusTime;
            }
        }
        
        private GapTimes ResetGapTimes()
        {
            gapCentralStimulusTime.Reset();
            gapPauseBetweenStimuli.Reset();
            gapPeripheralStimulusTime.Reset();
            return GapTimes;
        }

        #endregion

        #region Task count

        private TaskCount TaskCount
        {
            get => new TaskCount
            {
                Gaps = gapTaskCount.Value,
                Overlaps = ParseInput(overlapsInput, defaults.TaskCount.Overlaps),
            };

            set
            {
                gapTaskCount.Value = value.Gaps;
                overlapsInput.text = $"{value.Overlaps}";
            }
        }
        
        private TaskCount ResetTaskCount()
        {
            gapTaskCount.Reset();
            gapPauseBetweenStimuli.Reset();
            gapPeripheralStimulusTime.Reset();
            return TaskCount;
        }

        #endregion

        #region API

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

            simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees = distanceBetweenPeripheralStimuli.Value;
            simulator.Settings.CentralStimulusSizeInDegrees = centralStimulusSize.Value;
            simulator.Settings.PeripheralStimulusSizeInDegrees = peripheralStimulusSize.Value;

            simulator.Settings.GapTimes = GapTimes;
            simulator.Settings.OverlapTimes = OverlapTimes;
            simulator.Settings.TaskCount = TaskCount;

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
            
            gapCentralStimulusTime.SetDefault(() => defaults.GapTimes.CentralTime);
            gapPauseBetweenStimuli.SetDefault(() => defaults.GapTimes.PauseTime);
            gapPeripheralStimulusTime.SetDefault(() => defaults.GapTimes.StimulusTime);
            
            
            
            gapTaskCount.SetDefault(() => defaults.TaskCount.Gaps);
        }

        public void OnClose()
        {
            Hide();
            home.Show();
            simulator.UpdateBackground();
        }

        public void OnReset()
        {
            simulator.Settings.Background = ResetBackground();

            simulator.Settings.PauseBeforeTasks = pauseBeforeTasks.Reset();
            simulator.Settings.PauseBetweenTasks = pauseBetweenTasks.Reset();
            simulator.Settings.PauseAfterTasks = pauseAfterTasks.Reset();

            simulator.Settings.ReferencePoint = ResetReferencePoint();

            simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees = distanceBetweenPeripheralStimuli.Reset();
            simulator.Settings.CentralStimulusSizeInDegrees = centralStimulusSize.Reset();
            simulator.Settings.PeripheralStimulusSizeInDegrees = peripheralStimulusSize.Reset();

            simulator.Settings.GapTimes = ResetGapTimes();
            simulator.Settings.OverlapTimes = defaults.OverlapTimes;
            
            simulator.Settings.TaskCount = ResetTaskCount();

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

            distanceBetweenPeripheralStimuli.Value = simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees;
            centralStimulusSize.Value = simulator.Settings.CentralStimulusSizeInDegrees;
            peripheralStimulusSize.Value = simulator.Settings.PeripheralStimulusSizeInDegrees;

            GapTimes = simulator.Settings.GapTimes;

            OverlapTimes = simulator.Settings.OverlapTimes;
           
            TaskCount = simulator.Settings.TaskCount;
        }

        #endregion
    }
}