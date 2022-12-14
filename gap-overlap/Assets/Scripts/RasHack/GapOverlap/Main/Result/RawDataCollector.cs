using Tobii.Research;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;
using RasHack.GapOverlap.Main.Inputs;

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


        #endregion

        #region API

        public void TasksStarted(float sampleRate)
        {
            doCollecting = true;
            referenceTime = 0;

            sampleTime = 1f / sampleRate;
            sampleTimePassed = 0;

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
            Debug.Log($"Sampling done at {referenceTime}s");
        }

        private void GazeDataReceived(object sender, GazeDataEventArgs gazeEvent)
        {

        }

        #endregion
    }
}