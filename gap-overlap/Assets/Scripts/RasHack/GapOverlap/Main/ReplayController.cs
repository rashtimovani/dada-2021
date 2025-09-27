using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Result;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using Tobii.Research;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class ReplayController : MonoBehaviour
    {
        #region Unity fields

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

            var mainCamera = Camera.main;
            var screen = ScreenArea.WholeScreen;
            var overlayScreen = new ScreenArea((int)toReplay.Test.ScreenPixelsX, (int)toReplay.Test.ScreenPixelsY);
            screen = screen.Overlay(settings.ReferencePoint.ScreenDiagonalInInches, (float)toReplay.Test.ScreenDiagonalInInches, overlayScreen);
            debugScaler = new Scaler(mainCamera, -2, settings.WithScreenDiagonal((float)toReplay.Test.ScreenDiagonalInInches), screen);

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                toReplay = new ReplayedTest(deserializer.Deserialize<SampledTest>(reader), resultsDirectory, debugScaler);
                Debug.Log($"##### Starting replay of {toReplay.Test.Name} [{toReplay.Test.TestId}]...");
            }
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
            UpdateTask(sample.Task, sample.Time);
            UpdateCentralStimulus(sample.Task, sample.Time);
            UpdatePeripheralStimulus(sample.Task, sample.Time);
            UpdateEyes(sample.Tracker, sample.Time);
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
            stimulus.Scale(desiredSize, sizeInDegrees);

            var circle = stimulus.UseDetectableCircleAndDisableArea();
            circle.RegisterOnDetect(this);
            ScaleDetectableArea(stimulus, circle, sizeInDegrees);

            toReplay.AllFixations.CentralCreated(stimulus.transform.position, toReplay.SpentTime);

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
            stimulus.Scale(desiredSize, sizeInDegrees);

            var circle = stimulus.UseDetectableCircleAndDisableArea();
            circle.RegisterOnDetect(this);
            ScaleDetectableArea(stimulus, circle, sizeInDegrees);

            toReplay.AllFixations.PeripheralCreated(stimulus.transform.position, toReplay.SpentTime);

            return stimulus;
        }

        private void ScaleDetectableArea(ScalableStimulus stimulus, DetectableCircle circle, float sizeInDegrees)
        {
            var desiredDetectionAreaSize = debugScaler.Settings.FixationAreaInDegrees;
            var detectionAreaIncrease = desiredDetectionAreaSize / sizeInDegrees;

            var newLocalScaleX = circle.transform.localScale.x * detectionAreaIncrease;
            var newLocalScaleY = circle.transform.localScale.y * detectionAreaIncrease;
            circle.transform.localScale = new Vector3(newLocalScaleX, newLocalScaleY, 1);

            stimulus.ScaleRadius(detectionAreaIncrease);
        }

        private string Name(TaskType taskType, StimulusSide side) => taskType + "_" + side + "_stimulus";

        private void UpdateTask(SampledTask task, float time)
        {
            var taskType = Enum.Parse<TaskType>(task.TaskType);
            var side = Enum.Parse<StimulusSide>(task.Side);
            toReplay.AllFixations.NewTask(task.TaskOrder, taskType, side, time);
        }

        private void UpdateCentralStimulus(SampledTask task, float time)
        {
            if (task.CenterStimulus.Visible)
            {
                var taskType = Enum.Parse<TaskType>(task.TaskType);
                if (centralStimulus == null)
                {
                    centralStimulus = NewCentralStimulus(taskType, task.CenterStimulus.Center.X, task.CenterStimulus.Center.Y);
                }
                else if (centralStimulus.name != Name(taskType, StimulusSide.Center))
                {
                    toReplay.AllFixations.CentralDestroyed(time);
                    Destroy(centralStimulus.gameObject);
                    centralStimulus = NewCentralStimulus(taskType, task.CenterStimulus.Center.X, task.CenterStimulus.Center.Y);
                }
            }
            else if (centralStimulus != null)
            {
                toReplay.AllFixations.CentralDestroyed(time);
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
                }
                else if (peripheralStimulus.name != Name(taskType, side))
                {
                    toReplay.AllFixations.PeripheralDestroyed(time);
                    Destroy(peripheralStimulus.gameObject);
                    peripheralStimulus = NewPeripheralStimulus(taskType, side, task.PeripheralStimulus.Center.X, task.PeripheralStimulus.Center.Y);
                }
            }
            else if (peripheralStimulus != null)
            {
                toReplay.AllFixations.PeripheralDestroyed(time);
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
        }

        private void UpdateEyes(SampledTracker tracker, float time)
        {
            UpdateEye(Eye.Left, tracker.LeftEye, leftEye);
            UpdateEye(Eye.Right, tracker.RightEye, rightEye);
            toReplay.AllFixations.Update(leftEye.transform.position, rightEye.transform.position, time);
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