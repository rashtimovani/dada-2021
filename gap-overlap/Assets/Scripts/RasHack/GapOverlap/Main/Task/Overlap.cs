using System;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    [Serializable]
    public struct OverlapTimes
    {
        public float CentralTime;
        public float BothStimuli;
    }

    public class Overlap : Task
    {
        #region Serialized fields

        [SerializeField] private OverlapTimes times = new OverlapTimes { CentralTime = 1f, BothStimuli = 2.5f };

        #endregion

        #region Fields

        private float? waitingTime;
        private float? measurement;
        private float? centralMeasurement;
        private CentralStimulus centralStimulus;
        private Stimulus activeStimulus;

        #endregion

        #region API

        public override TaskType TaskType => TaskType.Overlap;

        private OverlapTimes Times => owner.Settings?.OverlapTimes ?? times;

        public override void ReportFocusedOn(Stimulus stimulus, float after)
        {
            if (stimulus != activeStimulus)
            {
                Debug.LogError($"{stimulus} stimulus is not the active one, don't care if it reported focused!");
                return;
            }

            measurement = after;
            Debug.Log($"{stimulus} reported focused after {after:0.000}s!");
        }

        public override void ReportFocusedOnCentral(CentralStimulus stimulus, float after)
        {
            if (stimulus != centralStimulus)
            {
                Debug.LogError($"{stimulus} is not the central stimulus, don't care if it reported focused!");
                return;
            }

            centralMeasurement = after;
            Debug.Log($"{stimulus} reported focused on central after {after:0.000}s!");
        }

        public override void ReportStimulusDied(Stimulus active)
        {
            if (active != activeStimulus)
            {
                Debug.LogError($"{active} stimulus is not the active one, don't care if it died!");
                return;
            }

            Debug.Log($"{activeStimulus} has finished");
            Destroy(activeStimulus.gameObject);
            Destroy(centralStimulus.gameObject);
            activeStimulus = null;
            owner.ReportTaskFinished(this, measurement, centralMeasurement);
            Destroy(gameObject);
        }

        public override void ReportCentralStimulusDied(CentralStimulus central)
        {
            if (central != centralStimulus)
            {
                Debug.LogError($"{central} stimulus is not the central one, don't care if it died!");
                return;
            }

            Debug.Log($"Only {centralStimulus} has finished");
            StartWithStimulus();
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            StartWithCentralStimulus();
        }

        protected override void OnDestroy()
        {
            if (centralStimulus != null) Destroy(centralStimulus.gameObject);
            if (activeStimulus != null) Destroy(activeStimulus.gameObject);
        }

        #endregion

        #region Helpers

        private void StartWithCentralStimulus()
        {
            centralStimulus = NewCentralStimulus();
            centralStimulus.StartSimulating(this, Times.CentralTime);
        }

        private void StartWithStimulus()
        {
            activeStimulus = NewStimulus();
            activeStimulus.StartSimulating(stimulusType, this, Times.BothStimuli);
        }

        #endregion
    }
}