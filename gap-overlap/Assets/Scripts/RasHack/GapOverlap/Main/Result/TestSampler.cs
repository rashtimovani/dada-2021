using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Stimuli;
using Tobii.Research;
using Tobii.Research.Unity;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    public class TestSampler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Simulator simulator;

        [SerializeField] private EyeTracker eyeTracker;

        private static readonly RawPosition NO_STIMULUS = new RawPosition
        {
            Visible = false,
            Center = new RawPoint
            {
                X = float.NaN,
                Y = float.NaN
            }
        };

        private static readonly SampledTask NO_TASK = new SampledTask
        {
            TaskOrder = -1,
            TaskType = "Empty",
            Side = "Unknown",
            StimulusType = "None",
            CenterStimulus = NO_STIMULUS,
            PeripheralStimulus = NO_STIMULUS
        };

        public static readonly string RESULTS_DIRECTORY =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}GapOverlap{Path.DirectorySeparatorChar}Results";

        private float sampleTime;

        private float sampleTimePassed;

        private float referenceTime;

        private bool doCollecting;

        private SampledTest sampled;

        private Task.Task currentTask;

        private CentralStimulus currentCentral;

        private PeripheralStimulus currentPeripheral;

        #endregion

        #region API

        public void StartTest(string name, string testId, float sampleRate)
        {
            sampled = new SampledTest
            {
                Name = name,
                TestId = testId,
                ScreenDiagonalInInches = simulator.Settings.ReferencePoint.ScreenDiagonalInInches,
                ScreenPixelsX = Screen.width,
                ScreenPixelsY = Screen.height
            };
            doCollecting = true;
            referenceTime = 0;

            sampleTime = 1f / sampleRate;
            sampleTimePassed = 0;

            currentTask = null;
            currentCentral = null;
            currentPeripheral = null;
        }

        public void CompleteTest(bool allCompleted)
        {
            if (!doCollecting) return;

            sampled.TestCompleted = allCompleted;
            doCollecting = false;

            Store();

            sampled = null;
        }

        public void StartTask(Task.Task task)
        {
            currentTask = task;
        }

        public void CompleteTask(Task.Task task)
        {
            currentTask = null;
        }

        public void StartCentral(CentralStimulus centralStimulus)
        {
            currentCentral = centralStimulus;
        }

        public void CompleteCentral()
        {
            currentCentral = null;
        }

        public void StartPeripheral(PeripheralStimulus peripheralStimulus)
        {
            currentPeripheral = peripheralStimulus;
        }

        public void CompletePeripheral()
        {
            currentPeripheral = null;
        }

        public void Store()
        {
            Directory.CreateDirectory(RESULTS_DIRECTORY);
            var filename = $"{RESULTS_DIRECTORY}{Path.DirectorySeparatorChar}{sampled.Name}_{sampled.TestId}.json";

            var serializer = JsonSerializer.CreateDefault();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(@filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, sampled);
            }
            Debug.Log($"Stored {filename}");
        }

        #endregion

        #region Mono methods

        private void OnDestroy()
        {
            CompleteTest(false);
        }


        private void FixedUpdate()
        {
            if (!doCollecting) return;

            sampleTimePassed += Time.fixedDeltaTime;

            while (sampleTimePassed >= sampleTime)
            {
                referenceTime += sampleTime;
                sampleTimePassed -= sampleTime;
                DoSampling();
            }
        }

        #endregion

        #region Helper methods

        private void DoSampling()
        {
            var sample = new Sample() { Time = referenceTime };

            if (currentTask != null)
            {
                sample.Task = new SampledTask
                {
                    TaskOrder = currentTask.TaskOrder,
                    TaskType = currentTask.TaskType.ToString(),
                    Side = currentTask.Side.ToString(),
                    StimulusType = currentTask.StimulusType.ToString(),
                    CenterStimulus = currentCentral?.RawPosition ?? NO_STIMULUS,
                    PeripheralStimulus = currentPeripheral?.RawPosition ?? NO_STIMULUS
                };
            }
            else sample.Task = NO_TASK;

            if (eyeTracker != null && eyeTracker.GazeDataCount > 0)
                sample.Tracker = new SampledTracker { LeftEye = new SampledGaze(eyeTracker.LatestGazeData.Left), RightEye = new SampledGaze(eyeTracker.LatestGazeData.Right) };

            if (!simulator.TobiiWorking)
            {
                var mouse = Input.mousePosition;

                var leftEye = new SampledGaze(new GazePoint(new NormalizedPoint2D(mouse.x / Screen.width, mouse.y / Screen.height), new Point3D(1, 1, 1), Validity.Valid));
                var rightEye = new SampledGaze(new GazePoint(new NormalizedPoint2D(mouse.x / Screen.width, mouse.y / Screen.height), new Point3D(1, 1, 1), Validity.Valid));

                sample.Tracker = new SampledTracker { LeftEye = leftEye, RightEye = rightEye };
            }

            sampled.Samples.AllSamples.Add(sample);
        }

        #endregion
    }
}