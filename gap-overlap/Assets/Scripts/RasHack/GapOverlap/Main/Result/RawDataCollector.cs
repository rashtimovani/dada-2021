using Tobii.Research;
using UnityEngine;
using RasHack.GapOverlap.Main.Task;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RasHack.GapOverlap.Main.Result
{
    public class RawDataCollector : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Simulator simulator;

        private float sampleTime;

        private float sampleTimePassed;

        private float referenceTime;

        private bool doCollecting;

        private IEyeTracker subscribedTo;

        private RawTestResult result;

        private float currentTaskStartedAt;

        #endregion

        #region API

        public void TasksStarted(string name, string testId, float sampleRate)
        {
            doCollecting = true;
            referenceTime = 0;

            sampleTime = 1f / sampleRate;
            sampleTimePassed = 0;

            result = new RawTestResult { Name = name, TestId = testId, Tasks = new RawTestTasks { List = new List<RawTaskTimes>() } };

            var eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            foreach (IEyeTracker eyeTracker in eyeTrackers)
            {
                subscribedTo = eyeTracker;
                subscribedTo.GazeDataReceived += GazeDataReceived;
                Debug.Log($"Subscribed to using {subscribedTo.Model}({subscribedTo.DeviceName}) on address {subscribedTo.Address}, started to receive gaze events from it");
                break;
            }
        }

        public void TasksCompleted()
        {
            if (!doCollecting) return;

            doCollecting = false;

            if (subscribedTo != null) subscribedTo.GazeDataReceived -= GazeDataReceived;

            Store();

            result = new RawTestResult();
        }

        public void TaskStarted(Task.Task task)
        {
            currentTaskStartedAt = referenceTime;
        }

        public void TaskCompleted(Task.Task task)
        {
            result.Tasks.List.Add(new RawTaskTimes
            {
                EndTime = referenceTime,
                StartTime = currentTaskStartedAt,
                TaskOrder = task.TaskOrder,
                Side = task.Side.ToString(),
                StimulusType = task.StimulusType.ToString(),
                TaskType = task.TaskType.ToString()
            });
            Store();
        }

        public void Store()
        {
            var json = JsonUtility.ToJson(result, true);
            Directory.CreateDirectory(TestResults.RESULTS_DIRECTORY);
            var filename = $"{TestResults.RESULTS_DIRECTORY}{Path.DirectorySeparatorChar}{result.Name}_{result.TestId}.json";
            File.WriteAllText(filename, json, Encoding.UTF8);
            Debug.Log($"Stored {filename}");
        }

        #endregion

        #region Mono methods

        private void OnDestroy()
        {
            TasksCompleted();
        }


        private void FixedUpdate()
        {

            if (!doCollecting) return;

            referenceTime += Time.fixedDeltaTime;
            sampleTimePassed += Time.fixedDeltaTime;

            while (sampleTimePassed >= sampleTime)
            {
                sampleTimePassed -= sampleTime;
                DoSampling();
            }
        }

        #endregion

        #region Helper methods

        private void DoSampling()
        {

        }

        private void GazeDataReceived(object sender, GazeDataEventArgs gazeEvent)
        {

        }

        #endregion
    }
}