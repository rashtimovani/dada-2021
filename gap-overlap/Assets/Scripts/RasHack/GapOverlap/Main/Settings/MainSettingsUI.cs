using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Settings
{
    public class MainSettingsUI : MonoBehaviour
    {
        #region Fileds

        [SerializeField] private GameObject panel;

        private readonly MainSettings defaults = new MainSettings();

        private HomeUI home;
        private Simulator simulator;

        #endregion

        #region General

        [Header("General")] [SerializeField] private Slider soundVolume;

        [SerializeField] private FloatInput fadeInOut;
        [SerializeField] private Slider rotationFactor;

        private float SoundVolume
        {
            get => Mathf.Clamp01(soundVolume.value);
            set => soundVolume.value = Mathf.Clamp01(value);
        }

        private int RotationFactor
        {
            get => Mathf.RoundToInt(rotationFactor.value);
            set => rotationFactor.value = value;
        }

        private int ResetRotationFactor()
        {
            RotationFactor = defaults.RotationFactor;
            return RotationFactor;
        }

        private float ResetSoundVolume()
        {
            SoundVolume = defaults.SoundVolume;
            return SoundVolume;
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

        [SerializeField] private Dropdown background;

        private BackgroundColor Background
        {
            get => (BackgroundColor)background.value;
            set => background.value = (int)value;
        }

        private BackgroundColor ResetBackground()
        {
            Background = defaults.Background;
            return Background;
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
        [SerializeField] private AreaRatio gapSides;

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

        #region Overlap

        [Header("Overlap settings")] [SerializeField]
        private FloatInput overlapCentralStimulusTime;

        [SerializeField] private FloatInput overlapBothStimuliTime;
        [SerializeField] private IntInput overlapTaskCount;
        [SerializeField] private AreaRatio overlapSides;

        private OverlapTimes OverlapTimes
        {
            get => new OverlapTimes
            {
                CentralTime = overlapCentralStimulusTime.Value,
                BothStimuli = overlapBothStimuliTime.Value
            };

            set
            {
                overlapCentralStimulusTime.Value = value.CentralTime;
                overlapBothStimuliTime.Value = value.BothStimuli;
            }
        }

        private OverlapTimes ResetOverlapTimes()
        {
            overlapCentralStimulusTime.Reset();
            overlapBothStimuliTime.Reset();
            return OverlapTimes;
        }

        #endregion

        #region Baseline

        [Header("Baseline settings")] [SerializeField]
        private FloatInput baselineCentralStimulusTime;

        [SerializeField] private FloatInput baselineCentralOutStimulusInTime;
        [SerializeField] private FloatInput baselineStimulusTime;
        [SerializeField] private IntInput baselineTaskCount;
        [SerializeField] private AreaRatio baselineSides;

        private BaselineTimes BaselineTimes
        {
            get => new BaselineTimes
            {
                CentralTime = baselineCentralStimulusTime.Value,
                CentralOutStimulusIn = baselineCentralOutStimulusInTime.Value,
                StimulusTime = baselineStimulusTime.Value
            };

            set
            {
                baselineCentralStimulusTime.Value = value.CentralTime;
                baselineCentralOutStimulusInTime.Value = value.CentralOutStimulusIn;
                baselineStimulusTime.Value = value.StimulusTime;
            }
        }

        private BaselineTimes ResetBaselineTimes()
        {
            baselineCentralStimulusTime.Reset();
            baselineCentralOutStimulusInTime.Reset();
            baselineStimulusTime.Reset();
            return BaselineTimes;
        }

        #endregion

        #region Task count

        private TaskCount TaskCount
        {
            get => new TaskCount
            {
                Gaps = gapTaskCount.Value,
                LeftGaps = gapSides.Left,
                Overlaps = overlapTaskCount.Value,
                LeftOverlaps = overlapSides.Left,
                Baselines = baselineTaskCount.Value,
                LeftBaselines = baselineSides.Left,
            };

            set
            {
                gapTaskCount.Value = value.Gaps;
                gapSides.Initial(value.LeftGaps);
                overlapTaskCount.Value = value.Overlaps;
                overlapSides.Initial(value.LeftOverlaps);
                baselineTaskCount.Value = value.Baselines;
                baselineSides.Initial(value.LeftBaselines);
            }
        }

        private TaskCount ResetTaskCount()
        {
            gapTaskCount.Reset();
            gapSides.Initial(defaults.TaskCount.LeftGaps);
            overlapTaskCount.Reset();
            overlapSides.Initial(defaults.TaskCount.LeftOverlaps);
            baselineTaskCount.Reset();
            baselineSides.Initial(defaults.TaskCount.LeftBaselines);
            return TaskCount;
        }

        #endregion


        #region API

        public void Show()
        {
            panel.SetActive(true);

            Display();
        }

        public void Hide(bool dontStore = false)
        {
            panel.SetActive(false);
            if (dontStore) return;

            simulator.Settings.SoundVolume = SoundVolume;
            simulator.Settings.FadeInOut = fadeInOut.Value;
            simulator.Settings.RotationFactor = RotationFactor;

            simulator.Settings.PauseBeforeTasks = pauseBeforeTasks.Value;
            simulator.Settings.PauseBetweenTasks = pauseBetweenTasks.Value;
            simulator.Settings.PauseAfterTasks = pauseAfterTasks.Value;

            simulator.Settings.ReferencePoint = ReferencePoint;
            simulator.Settings.Background = Background;

            simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees = distanceBetweenPeripheralStimuli.Value;
            simulator.Settings.CentralStimulusSizeInDegrees = centralStimulusSize.Value;
            simulator.Settings.PeripheralStimulusSizeInDegrees = peripheralStimulusSize.Value;

            simulator.Settings.GapTimes = GapTimes;

            simulator.Settings.OverlapTimes = OverlapTimes;

            simulator.Settings.BaselineTimes = BaselineTimes;

            simulator.Settings.TaskCount = TaskCount;

            simulator.Settings.Store();
        }

        #endregion

        #region Unity methods

        private void Start()
        {
            home = FindObjectOfType<HomeUI>();
            simulator = FindObjectOfType<Simulator>();

            fadeInOut.SetDefault(() => defaults.FadeInOut);

            pauseBeforeTasks.SetDefault(() => defaults.PauseBeforeTasks);
            pauseBetweenTasks.SetDefault(() => defaults.PauseBetweenTasks);
            pauseAfterTasks.SetDefault(() => defaults.PauseAfterTasks);

            screenDiagonal.SetDefault(() => defaults.ReferencePoint.ScreenDiagonalInInches);
            eyeTrackerDistance.SetDefault(() => ToMM(defaults.ReferencePoint.DistanceFromEyesInCM));

            distanceBetweenPeripheralStimuli.SetDefault(() => defaults.DistanceBetweenPeripheralStimuliInDegrees);
            centralStimulusSize.SetDefault(() => defaults.CentralStimulusSizeInDegrees);
            peripheralStimulusSize.SetDefault(() => defaults.PeripheralStimulusSizeInDegrees);

            gapCentralStimulusTime.SetDefault(() => defaults.GapTimes.CentralTime);
            gapPauseBetweenStimuli.SetDefault(() => defaults.GapTimes.PauseTime);
            gapPeripheralStimulusTime.SetDefault(() => defaults.GapTimes.StimulusTime);

            overlapCentralStimulusTime.SetDefault(() => defaults.OverlapTimes.CentralTime);
            overlapBothStimuliTime.SetDefault(() => defaults.OverlapTimes.BothStimuli);

            baselineStimulusTime.SetDefault(() => defaults.BaselineTimes.CentralTime);
            baselineCentralOutStimulusInTime.SetDefault(() => defaults.BaselineTimes.CentralOutStimulusIn);
            baselineStimulusTime.SetDefault(() => defaults.BaselineTimes.StimulusTime);

            gapTaskCount.SetDefault(() => defaults.TaskCount.Gaps);
            overlapTaskCount.SetDefault(() => defaults.TaskCount.Overlaps);
        }

        public void OnClose()
        {
            Hide();
            home.Show();
            simulator.UpdateBackground();
        }

        public void OnReset()
        {
            simulator.Settings.SoundVolume = ResetSoundVolume();
            simulator.Settings.FadeInOut = fadeInOut.Reset();
            simulator.Settings.RotationFactor = ResetRotationFactor();

            simulator.Settings.PauseBeforeTasks = pauseBeforeTasks.Reset();
            simulator.Settings.PauseBetweenTasks = pauseBetweenTasks.Reset();
            simulator.Settings.PauseAfterTasks = pauseAfterTasks.Reset();

            simulator.Settings.ReferencePoint = ResetReferencePoint();
            simulator.Settings.Background = ResetBackground();

            simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees = distanceBetweenPeripheralStimuli.Reset();
            simulator.Settings.CentralStimulusSizeInDegrees = centralStimulusSize.Reset();
            simulator.Settings.PeripheralStimulusSizeInDegrees = peripheralStimulusSize.Reset();

            simulator.Settings.GapTimes = ResetGapTimes();

            simulator.Settings.OverlapTimes = ResetOverlapTimes();

            simulator.Settings.BaselineTimes = ResetBaselineTimes();

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

        #endregion

        private void Display()
        {
            SoundVolume = simulator.Settings.SoundVolume;
            fadeInOut.Value = simulator.Settings.FadeInOut;
            RotationFactor = simulator.Settings.RotationFactor;

            pauseBeforeTasks.Value = simulator.Settings.PauseBeforeTasks;
            pauseBetweenTasks.Value = simulator.Settings.PauseBetweenTasks;
            pauseAfterTasks.Value = simulator.Settings.PauseAfterTasks;

            ReferencePoint = simulator.Settings.ReferencePoint;
            Background = simulator.Settings.Background;

            distanceBetweenPeripheralStimuli.Value = simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees;
            centralStimulusSize.Value = simulator.Settings.CentralStimulusSizeInDegrees;
            peripheralStimulusSize.Value = simulator.Settings.PeripheralStimulusSizeInDegrees;

            GapTimes = simulator.Settings.GapTimes;

            OverlapTimes = simulator.Settings.OverlapTimes;

            BaselineTimes = simulator.Settings.BaselineTimes;

            TaskCount = simulator.Settings.TaskCount;
        }
    }
}