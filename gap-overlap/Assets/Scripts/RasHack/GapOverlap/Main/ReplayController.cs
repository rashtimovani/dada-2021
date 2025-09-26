using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Result;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using Tobii.Research;
using UnityEngine;
using UnityEngine.UIElements;

namespace RasHack.GapOverlap.Main
{
    public class ReplayedTest
    {
        #region Fields

        public readonly SampledTest Test;
        private float spentTime;
        private int currentSampleIndex;
        public readonly Timers Timers;
        private readonly string resultsDirectory;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test, string resultsDirectory)
        {
            Test = test;
            currentSampleIndex = 0;
            spentTime = test.Samples.AllSamples[currentSampleIndex].Time;
            Timers = new Timers();
            this.resultsDirectory = resultsDirectory;
        }

        #endregion

        #region Methods

        public float SpentTime => spentTime;

        public bool Tick(float deltaTime, Action<Sample> onNextSample)
        {
            var toTime = spentTime + deltaTime;
            while (currentSampleIndex < Test.Samples.AllSamples.Count)
            {
                var sample = Test.Samples.AllSamples[currentSampleIndex];
                if (toTime < sample.Time) break;

                currentSampleIndex++;
                spentTime = sample.Time;
                onNextSample.Invoke(sample);
            }

            spentTime = toTime;

            var stillRunning = currentSampleIndex < Test.Samples.AllSamples.Count;
            if (!stillRunning) Timers.ToCSV(resultsDirectory, Test.Name, Test.TestId);
            return stillRunning;
        }

        #endregion
    }

    public class ReplayController : MonoBehaviour
    {
        #region Unity fields

        [SerializeField] private bool useRadius;
        [Range(1.0f, 100.0f)][SerializeField] private float analyzeMultiplier = 30f;
        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;

        [SerializeField] private GameObject centralStimulusPrefab;

        [SerializeField] private GameObject stimulusPrefab;

        [SerializeField] private GameObject leftEye;

        [SerializeField] private GameObject rightEye;

        #endregion

        #region Fields

        private MainSettings settings = new MainSettings();

        private Scaler debugScaler;

        private ReplayedTest toReplay;

        private CentralStimulus centralStimulus;

        private PeripheralStimulus peripheralStimulus;

        private TestChooser testChooser;

        private string resultsDirectory;
        private List<string> allJsonsToProcess = new List<string>();

        #endregion

        #region Properties

        private bool ShowPointer => settings.ShowPointer;

        public MainSettings Settings => settings;

        #endregion

        #region API

        private float Degrees => settings.DistanceBetweenPeripheralStimuliInDegrees / 2;

        public void StartReplay(string testToLoad, string directoryForResults = null)
        {
            toReplay = null;
            var bytes = File.ReadAllBytes(testToLoad);

            var deserializer = JsonSerializer.CreateDefault();

            if (directoryForResults != null) resultsDirectory = CreateResultsDirectory(directoryForResults);
            if (!Directory.Exists(resultsDirectory))
            {
                Directory.CreateDirectory(resultsDirectory);
            }

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                toReplay = new ReplayedTest(deserializer.Deserialize<SampledTest>(reader), resultsDirectory);
                Debug.Log($"##### Starting replay of {toReplay.Test.Name} [{toReplay.Test.TestId}]...");
            }

            var mainCamera = Camera.main;
            var screen = ScreenArea.WholeScreen;
            var overlayScreen = new ScreenArea((int)toReplay.Test.ScreenPixelsX, (int)toReplay.Test.ScreenPixelsY);
            screen = screen.Overlay(settings.ReferencePoint.ScreenDiagonalInInches, (float)toReplay.Test.ScreenDiagonalInInches, overlayScreen);
            debugScaler = new Scaler(mainCamera, -2, settings.WithScreenDiagonal((float)toReplay.Test.ScreenDiagonalInInches), screen);
        }

        public void AnalyzeAllTests(string folderPath, string[] jsonFiles)
        {
            Time.timeScale = analyzeMultiplier;
            for (var i = 1; i < jsonFiles.Length; i++)
            {
                allJsonsToProcess.Add(jsonFiles[i]);
            }

            if (jsonFiles.Length > 0)
            {
                var resultsDirectoryToClean = CreateResultsDirectory(folderPath);
                if (Directory.Exists(resultsDirectoryToClean))
                {
                    foreach (var file in Directory.GetFiles(resultsDirectoryToClean))
                    {
                        File.Delete(file);
                    }
                    foreach (var dir in Directory.GetDirectories(resultsDirectoryToClean))
                    {
                        Directory.Delete(dir, true);
                    }
                }

                Debug.Log($"#### {allJsonsToProcess.Count + 1} tests remaining to process...");
                StartReplay(jsonFiles[0], folderPath);
            }
            else FinishReplay();
        }

        #endregion

        #region Methods

        private static string CreateResultsDirectory(string parent)
        {
            return parent + "/detections";
        }

        public void Start()
        {
            testChooser = FindObjectOfType<TestChooser>();
        }

        public void Tick(float deltaTime)
        {
            if (toReplay == null) return;

            var stillRunning = toReplay.Tick(deltaTime, OnNextSample);
            if (!stillRunning)
            {
                if (allJsonsToProcess.Count > 0)
                {
                    Debug.Log($"#### {allJsonsToProcess.Count} tests remaining to process...");
                    var jsonToProcess = allJsonsToProcess[0];
                    allJsonsToProcess.RemoveAt(0);
                    StartReplay(jsonToProcess);
                }
                else
                {
                    FinishReplay();
                    testChooser.Show();
                }
            }
        }

        private void OnNextSample(Sample sample)
        {
            UpdateCentralStimulus(sample.Task, sample.Time);
            UpdatePeripheralStimulus(sample.Task, sample.Time);
            UpdateEyes(sample.Tracker);
        }

        private void UpdateDebugVisibility()
        {
            bottomLeft.enabled = ShowPointer;
            bottomRight.enabled = ShowPointer;
            topLeft.enabled = ShowPointer;
            topRight.enabled = ShowPointer;
        }

        private void Awake()
        {
            var loadedSettings = MainSettings.Load();
            if (loadedSettings != null) settings = loadedSettings;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            Tick(deltaTime);
            if (debugScaler != null) UpdateBounds();
            UpdateDebugVisibility();
            DetectInterruptedReplay();
        }

        private void UpdateBounds()
        {
            topLeft.transform.position = debugScaler.TopLeft;
            bottomLeft.transform.position = debugScaler.BottomLeft;
            bottomRight.transform.position = debugScaler.BottomRight;
            topRight.transform.position = debugScaler.TopRight;
        }

        protected CentralStimulus NewCentralStimulus(TaskType taskType, float rawX, float rawY)
        {
            var area = NextArea.CenterInWorld(debugScaler.FromRaw(rawX, rawY));

            var localWhere = transform.InverseTransformPoint(area.Position);
            var newOne = Instantiate(centralStimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = Name(taskType, area.Side);

            var stimulus = newOne.GetComponent<CentralStimulus>();
            var sizeInDegrees = debugScaler.Settings.CentralStimulusSizeInDegrees;
            var desiredSize = debugScaler.RealWorldSizeFromDegrees(sizeInDegrees, area.OffsetInDegrees);
            stimulus.Scale(desiredSize);

            var circle = stimulus.UseDetectableCircleAndDisableArea();
            circle.RegisterOnDetect(this, eye =>
            {
                if (!useRadius)
                {
                    var after = toReplay.Timers.ObserveCentral(eye, toReplay.SpentTime);
                    Debug.Log($"Central stimulus detected by {eye} eye after {after} second by collision");
                }
            });
            ScaleDetectableArea(stimulus, circle, sizeInDegrees);

            return stimulus;
        }

        private PeripheralStimulus NewPeripheralStimulus(TaskType taskType, StimulusSide side, float x, float y)
        {
            var localWhere = transform.InverseTransformPoint(debugScaler.FromRaw(x, y));
            var newOne = Instantiate(stimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = Name(taskType, side);

            var offsetInDegrees = side == StimulusSide.Right ? Vector3.right.x * Degrees : Vector3.left.x * Degrees;

            var stimulus = newOne.GetComponent<PeripheralStimulus>();
            var sizeInDegrees = debugScaler.Settings.PeripheralStimulusSizeInDegrees;
            var desiredSize = debugScaler.RealWorldSizeFromDegrees(sizeInDegrees, offsetInDegrees);
            stimulus.Scale(desiredSize);

            var circle = stimulus.UseDetectableCircleAndDisableArea();
            circle.RegisterOnDetect(this, eye =>
            {
                if (!useRadius)
                {
                    var after = toReplay.Timers.ObservePeripheral(eye, toReplay.SpentTime);
                    Debug.Log($"Peripheral stimulus detected by {eye} eye after {after} seconds by collision");
                }
            });
            ScaleDetectableArea(stimulus, circle, sizeInDegrees);

            return stimulus;
        }

        private void ScaleDetectableArea(ScalableStimulus stimulus, DetectableCircle circle, float sizeInDegrees)
        {
            var desiredDetectionAreaSize = debugScaler.Settings.DetectionAreaInDegrees;
            var detectionAreaIncrease = desiredDetectionAreaSize / sizeInDegrees;

            var newLocalScaleX = circle.transform.localScale.x * detectionAreaIncrease;
            var newLocalScaleY = circle.transform.localScale.y * detectionAreaIncrease;
            circle.transform.localScale = new Vector3(newLocalScaleX, newLocalScaleY, 1);

            stimulus.ScaleRadius(detectionAreaIncrease);
        }

        private string Name(TaskType taskType, StimulusSide side) => taskType + "_" + side + "_stimulus";

        private void UpdateCentralStimulus(SampledTask task, float time)
        {
            if (task.CenterStimulus.Visible)
            {
                var taskType = Enum.Parse<TaskType>(task.TaskType);
                var taskSide = Enum.Parse<StimulusSide>(task.Side);
                if (centralStimulus == null)
                {
                    centralStimulus = NewCentralStimulus(taskType, task.CenterStimulus.Center.X, task.CenterStimulus.Center.Y);
                    toReplay.Timers.StartNewCentral(time, task.TaskOrder, taskType, taskSide);
                }
                else if (centralStimulus.name != Name(taskType, StimulusSide.Center))
                {
                    Destroy(centralStimulus.gameObject);
                    centralStimulus = NewCentralStimulus(taskType, task.CenterStimulus.Center.X, task.CenterStimulus.Center.Y);
                    toReplay.Timers.StartNewCentral(time, task.TaskOrder, taskType, taskSide);
                }
            }
            else if (centralStimulus != null)
            {
                Destroy(centralStimulus.gameObject);
                centralStimulus = null;
            }
        }

        private void UpdatePeripheralStimulus(SampledTask task, float time)
        {
            if (task.PeripheralStimulus.Visible)
            {
                var taskType = Enum.Parse<TaskType>(task.TaskType);
                var side = Enum.Parse<StimulusSide>(task.Side);
                if (peripheralStimulus == null)
                {
                    peripheralStimulus = NewPeripheralStimulus(taskType, side, task.PeripheralStimulus.Center.X, task.PeripheralStimulus.Center.Y);
                    toReplay.Timers.StartPeripheral(time);
                }
                else if (peripheralStimulus.name != Name(taskType, side))
                {
                    Destroy(peripheralStimulus.gameObject);
                    peripheralStimulus = NewPeripheralStimulus(taskType, side, task.PeripheralStimulus.Center.X, task.PeripheralStimulus.Center.Y);
                    toReplay.Timers.StartPeripheral(time);
                }
            }
            else if (peripheralStimulus != null)
            {
                Destroy(peripheralStimulus.gameObject);
                peripheralStimulus = null;
            }
        }

        private void UpdateEye(Eye eye, SampledGaze eyeSample, GameObject eyeObject)
        {
            eyeObject.SetActive(eyeSample.Validity == Validity.Valid.ToString());
            if (eyeObject.activeSelf)
            {
                eyeObject.transform.position = transform.InverseTransformPoint(debugScaler.FromRaw(eyeSample.PositionOnDisplayArea.X, eyeSample.PositionOnDisplayArea.Y));
            }

            if (useRadius)
            {
                if (centralStimulus != null) centralStimulus.TryFocusInRadius(eye, eyeObject.transform, e =>
                {
                    var after = toReplay.Timers.ObserveCentral(e, toReplay.SpentTime);
                    Debug.Log($"Central stimulus detected by {e} eye after {after} seconds by radius");
                }, e => Process(toReplay.Timers.StopObservingCentral(e, toReplay.SpentTime), e, "Central"));
                else Process(toReplay.Timers.StopObservingCentral(eye, toReplay.SpentTime), eye, "Central");
            }

            if (useRadius)
            {
                if (peripheralStimulus != null) peripheralStimulus.TryFocusInRadius(eye, eyeObject.transform, e =>
                {
                    var after = toReplay.Timers.ObservePeripheral(e, toReplay.SpentTime);
                    Debug.Log($"Peripheral stimulus detected by {e} eye after {after} seconds by radius");
                }, e => Process(toReplay.Timers.StopObservingPeripheral(e, toReplay.SpentTime), e, "Peripheral"));
                else Process(toReplay.Timers.StopObservingPeripheral(eye, toReplay.SpentTime), eye, "Peripheral");
            }
        }

        private void Process(FixationResult fixation, Eye eye, string modifier)
        {
            if (fixation.EndedNow) Debug.Log($"{modifier} stimulus fixation by {eye} eye ended after {fixation.Duration} seconds by radius");
        }

        private void UpdateEyes(SampledTracker tracker)
        {
            UpdateEye(Eye.Left, tracker.LeftEye, leftEye);
            UpdateEye(Eye.Right, tracker.RightEye, rightEye);
        }

        private void DetectInterruptedReplay()
        {
            if (toReplay != null && Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.LogWarning("Replay aborted by user!");
                allJsonsToProcess.Clear();
                FinishReplay();
                testChooser.Show();
            }
        }

        private void FinishReplay()
        {
            if (toReplay != null) Debug.Log($"===== Replay of {toReplay.Test.Name} [{toReplay.Test.TestId}] finished!");
            else Debug.Log("Finishing without active test.");
            toReplay = null;
            Time.timeScale = 1;
            if (centralStimulus != null)
            {
                Destroy(centralStimulus.gameObject);
                centralStimulus = null;
            }
            if (peripheralStimulus != null)
            {
                Destroy(peripheralStimulus.gameObject);
                peripheralStimulus = null;
            }
        }

        #endregion
    }
}