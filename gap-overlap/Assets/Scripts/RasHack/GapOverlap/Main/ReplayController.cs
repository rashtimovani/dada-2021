using System;
using System.IO;
using System.Security.AccessControl;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Result;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using Tobii.Research;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class ReplayedTest
    {
        #region Fields

        public readonly SampledTest test;
        private float spentTime;
        private int currentSampleIndex;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test)
        {
            this.test = test;
            currentSampleIndex = 0;
            spentTime = test.Samples.AllSamples[currentSampleIndex].Time;
        }

        #endregion

        #region Methods

        public bool Tick(float deltaTime, Action<Sample> onNextSample)
        {
            var toTime = spentTime + deltaTime;
            while (currentSampleIndex < test.Samples.AllSamples.Count)
            {
                var sample = test.Samples.AllSamples[currentSampleIndex];
                if (toTime < sample.Time) break;

                currentSampleIndex++;
                spentTime = sample.Time;
                onNextSample.Invoke(sample);
            }

            spentTime = toTime;

            return currentSampleIndex < test.Samples.AllSamples.Count;
        }

        #endregion
    }

    public class ReplayController : MonoBehaviour
    {
        #region Unity fields

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

        private FileChooser fileChooser;

        #endregion

        #region Properties

        private bool ShowPointer => settings.ShowPointer;

        #endregion

        #region API

        private float Degrees => settings.DistanceBetweenPeripheralStimuliInDegrees / 2;

        public void StartReplay(string testToLoad)
        {
            toReplay = null;
            var bytes = File.ReadAllBytes(testToLoad);

            var deserializer = JsonSerializer.CreateDefault();

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                toReplay = new ReplayedTest(deserializer.Deserialize<SampledTest>(reader));
            }

            var mainCamera = Camera.main;
            var screen = ScreenArea.WholeScreen;
            var overlayScreen = new ScreenArea((int)toReplay.test.ScreenPixelsX, (int)toReplay.test.ScreenPixelsY);
            screen = screen.Overlay(settings.ReferencePoint.ScreenDiagonalInInches, (float)toReplay.test.ScreenDiagonalInInches, overlayScreen);
            debugScaler = new Scaler(mainCamera, -2, settings.WithScreenDiagonal((float)toReplay.test.ScreenDiagonalInInches), screen);
        }

        #endregion

        #region Methods

        public void Start()
        {
            fileChooser = FindObjectOfType<FileChooser>();
        }

        public void Tick(float deltaTime)
        {
            if (toReplay == null) return;

            var stillRunning = toReplay.Tick(deltaTime, OnNextSample);
            if (!stillRunning)
            {
                FinishReplay();
                fileChooser.Show();
            }
        }

        private void OnNextSample(Sample sample)
        {
            Debug.Log($"Replaying sample at {sample.Time}s: {sample.Task.TaskType}");

            UpdateCentralStimulus(sample.Task);
            UpdatePeripheralStimulus(sample.Task);
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

        protected CentralStimulus NewCentralStimulus(string taskType, float rawX, float rawY)
        {
            var area = NextArea.CenterInWorld(debugScaler.FromRaw(rawX, rawY));

            var localWhere = transform.InverseTransformPoint(area.Position);
            var newOne = Instantiate(centralStimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = Name(taskType, area.Side);

            var stimulus = newOne.GetComponent<CentralStimulus>();
            var sizeInDegrees = debugScaler.Settings.CentralStimulusSizeInDegrees;
            var desiredSize = debugScaler.RealWorldSizeFromDegrees(sizeInDegrees, area.OffsetInDegrees);
            stimulus.Scale(desiredSize);
            stimulus.DontUseDetectableArea();

            return stimulus;
        }

        private PeripheralStimulus NewPeripheralStimulus(string taskType, StimulusSide side, float x, float y)
        {
            var localWhere = transform.InverseTransformPoint(debugScaler.FromRaw(x, y));
            var newOne = Instantiate(stimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = Name(taskType, side);

            var offsetInDegrees = side == StimulusSide.Right ? Vector3.right.x * Degrees : Vector3.left.x * Degrees;

            var stimulus = newOne.GetComponent<PeripheralStimulus>();
            var sizeInDegrees = debugScaler.Settings.PeripheralStimulusSizeInDegrees;
            var desiredSize = debugScaler.RealWorldSizeFromDegrees(sizeInDegrees, offsetInDegrees);
            stimulus.Scale(desiredSize);
            stimulus.DontUseDetectableArea();

            return stimulus;
        }

        private string Name(string taskType, StimulusSide side) => taskType + "_" + side + "_stimulus";

        private void UpdateCentralStimulus(SampledTask task)
        {
            if (task.CenterStimulus.Visible)
            {
                if (centralStimulus == null) centralStimulus = NewCentralStimulus(task.TaskType, task.CenterStimulus.Center.X, task.CenterStimulus.Center.Y);
                else if (centralStimulus.name != Name(task.TaskType, Enum.Parse<StimulusSide>(task.Side)))
                {
                    Destroy(centralStimulus.gameObject);
                    centralStimulus = NewCentralStimulus(task.TaskType, task.CenterStimulus.Center.X, task.CenterStimulus.Center.Y);
                }
            }
            else if (centralStimulus != null)
            {
                Destroy(centralStimulus.gameObject);
                centralStimulus = null;
            }
        }

        private void UpdatePeripheralStimulus(SampledTask task)
        {
            if (task.PeripheralStimulus.Visible)
            {
                var side = Enum.Parse<StimulusSide>(task.Side);
                if (peripheralStimulus == null) peripheralStimulus = NewPeripheralStimulus(task.TaskType, side, task.PeripheralStimulus.Center.X, task.PeripheralStimulus.Center.Y);
                else if (peripheralStimulus.name != Name(task.TaskType, side))
                {
                    Destroy(peripheralStimulus.gameObject);
                    peripheralStimulus = NewPeripheralStimulus(task.TaskType, side, task.PeripheralStimulus.Center.X, task.PeripheralStimulus.Center.Y);
                }
            }
            else if (peripheralStimulus != null)
            {
                Destroy(peripheralStimulus.gameObject);
                peripheralStimulus = null;
            }
        }

        private void UpdateEye(SampledGaze eye, GameObject eyeObject)
        {
            eyeObject.SetActive(eye.Validity == Validity.Valid.ToString());
            if (eyeObject.activeSelf)
            {
                eyeObject.transform.position = transform.InverseTransformPoint(debugScaler.FromRaw(eye.PositionOnDisplayArea.X, eye.PositionOnDisplayArea.Y));
            }
        }

        private void UpdateEyes(SampledTracker tracker)
        {
            UpdateEye(tracker.LeftEye, leftEye);
            UpdateEye(tracker.RightEye, rightEye);
        }

        private void DetectInterruptedReplay()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.LogWarning("Replay aborted by user!");
                FinishReplay();
                fileChooser.Show();
            }
        }

        private void FinishReplay()
        {
            toReplay = null;
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