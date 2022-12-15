using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Stimuli;
using Tobii.Research;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    public class TestSampler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Simulator simulator;

        private float sampleTime;

        private float sampleTimePassed;

        private float referenceTime;

        private bool doCollecting;

        private IEyeTracker subscribedTo;

        private SampledTest sampled;

        private Task.Task currentTask;

        private CentralStimulus currentCentral;

        private PeripheralStimulus currentPeripheral;

        private GazeDataEventArgs currentGaze;

        #endregion

        #region API

        public void StartTest(string name, string testId, float sampleRate)
        {
            sampled = new SampledTest { Name = name, TestId = testId };
            doCollecting = true;
            referenceTime = 0;

            sampleTime = 1f / sampleRate;
            sampleTimePassed = 0;

            currentTask = null;
            currentCentral = null;
            currentPeripheral = null;
            currentGaze = null;

            var eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            foreach (IEyeTracker eyeTracker in eyeTrackers)
            {
                subscribedTo = eyeTracker;
                subscribedTo.GazeDataReceived += GazeDataReceived;
                Debug.Log($"Subscribed to using {subscribedTo.Model}({subscribedTo.DeviceName}) on address {subscribedTo.Address}, started to receive gaze events from it");
                break;
            }
        }

        public void CompleteTest(bool allCompleted)
        {
            if (!doCollecting) return;

            sampled.TestCompleted = allCompleted;
            doCollecting = false;

            if (subscribedTo != null) subscribedTo.GazeDataReceived -= GazeDataReceived;

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
            Directory.CreateDirectory(TestResults.RESULTS_DIRECTORY);
            var filename = $"{TestResults.RESULTS_DIRECTORY}{Path.DirectorySeparatorChar}{sampled.Name}_{sampled.TestId}.json";

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
                    CenterStimulus = currentCentral?.RawPosition,
                    PeripheralStimulus = currentPeripheral?.RawPosition

                };
            }

            if (currentGaze != null)
                sample.Tracker = new SampledTracker { LeftEye = new SampledGaze(currentGaze.LeftEye.GazePoint), RightEye = new SampledGaze(currentGaze.RightEye.GazePoint) };

            if (Input.mousePresent)
            {
                var mouse = Input.mousePosition;

                var leftEye = new SampledGaze(new GazePoint(new NormalizedPoint2D(mouse.x / Screen.width, mouse.y / Screen.height), new Point3D(1, 1, 1), Validity.Valid));
                var rightEye = new SampledGaze(new GazePoint(new NormalizedPoint2D(mouse.x / Screen.width, mouse.y / Screen.height), new Point3D(1, 1, 1), Validity.Valid));

                sample.Tracker = new SampledTracker { LeftEye = leftEye, RightEye = rightEye };
            }

            sampled.Samples.AllSamples.Add(sample);
        }

        private void GazeDataReceived(object sender, GazeDataEventArgs gazeEvent)
        {
            currentGaze = gazeEvent;
        }

        #endregion
    }
}