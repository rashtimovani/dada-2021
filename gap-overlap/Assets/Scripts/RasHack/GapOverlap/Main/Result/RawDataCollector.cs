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


        #endregion

        #region API

        public void TasksStarted(float sampleRate)
        {
            doCollecting = true;
            referenceTime = 0;

            sampleTime = 1f / sampleRate;
            sampleTimePassed = 0;
        }

        public void TasksCompleted()
        {
            doCollecting = false;
        }

        #endregion

        #region Mono methods


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

        #endregion
    }
}