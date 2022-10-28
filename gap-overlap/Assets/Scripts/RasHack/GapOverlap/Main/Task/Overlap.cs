using System;
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    [Serializable]
    public struct OverlapTimes
    {
        public float CentralTime;
        public float BothStimuli;
        public float ShortenOnFocusTime;
    }

    public class Overlap : Task
    {
        #region Serialized fields

        [SerializeField] private OverlapTimes times = new()
            { CentralTime = 5.0f, BothStimuli = 5.0f, ShortenOnFocusTime = 0.2f };

        #endregion

        #region Fields

        private float? waitingTime;
        private CentralStimulus centralStimulus;
        private PeripheralStimulus activeStimulus;

        #endregion

        #region API

        public override TaskType TaskType => TaskType.Overlap;

        private OverlapTimes Times => owner.Settings?.OverlapTimes ?? times;

        public override void ReportFocusedOnPeripheral(PeripheralStimulus stimulus, Eye eye, float after)
        {
            if (stimulus != activeStimulus)
            {
                Debug.LogError($"{stimulus} stimulus is not the active one, don't care if it reported focused!");
                return;
            }

            responses = responses.PeripheralMeasured(eye, after);
            if (responses.AllPeripheralMeasured)
            {
                var remaining = stimulus.ShortenAnimation(Times.ShortenOnFocusTime, false);
                centralStimulus?.ShortenAnimation(remaining, false);
            }

            Debug.Log($"{stimulus} reported focused {eye} eye after {after:0.000}s!");
        }

        public override void ReportFocusedOnCentral(CentralStimulus stimulus, Eye eye, float after)
        {
            if (stimulus != centralStimulus)
            {
                Debug.LogError($"{stimulus} is not the central stimulus, don't care if it reported focused!");
                return;
            }

            responses = responses.CentralMeasured(eye, after);
            if (responses.AllCentralMeasured)
            {
                stimulus.ShortenAnimation(Times.ShortenOnFocusTime, true);
                if (waitingTime.HasValue)
                {
                    waitingTime = Mathf.Min(Times.ShortenOnFocusTime, waitingTime.Value);
                }
            }

            Debug.Log($"{stimulus} reported {eye} eye focused on central after {after:0.000}s!");
        }

        public override void ReportStimulusDied(PeripheralStimulus active)
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
            owner.ReportTaskFinished(this, responses);
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
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            waitingTime = times.CentralTime;
            StartWithCentralStimulus();
        }

        private void Update()
        {
            if (!waitingTime.HasValue) return;
            waitingTime -= Time.deltaTime;
            if (waitingTime > 0f) return;
            waitingTime = null;
            StartWithStimulus();
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
            centralStimulus.StartSimulating(this, Times.CentralTime + Times.BothStimuli);
        }

        private void StartWithStimulus()
        {
            activeStimulus = NewStimulus();
            activeStimulus.StartSimulating(stimulusType, this, Times.BothStimuli);
        }

        #endregion
    }
}