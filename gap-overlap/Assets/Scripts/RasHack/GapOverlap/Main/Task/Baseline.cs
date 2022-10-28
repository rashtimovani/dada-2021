using System;
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    [Serializable]
    public struct BaselineTimes
    {
        public float CentralTime;
        public float CentralOutStimulusIn;
        public float StimulusTime;
        public float ShortenOnFocusTime;
    }

    public class Baseline : Task
    {
        #region Serialized fields

        [SerializeField] private BaselineTimes times = new()
            { CentralTime = 5.0f, CentralOutStimulusIn = 0.0f, StimulusTime = 5.0f, ShortenOnFocusTime = 0.2f };

        #endregion

        #region Fields

        private float? centralTimeOnly;
        private float? outInTime;
        private CentralStimulus centralStimulus;
        private PeripheralStimulus activeStimulus;

        #endregion

        #region API

        public override TaskType TaskType => TaskType.Baseline;

        private BaselineTimes Times => owner.Settings?.BaselineTimes ?? times;

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

            Debug.Log($"{stimulus} reported {eye} eye focused after {after:0.000}s!");
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
                stimulus.ShortenIdleAnimationOnly(Times.ShortenOnFocusTime);
                if (centralTimeOnly.HasValue)
                {
                    centralTimeOnly = Mathf.Min(centralTimeOnly.Value, Times.ShortenOnFocusTime);
                }
            }

            Debug.Log($"{stimulus} reported {eye} eye focused on central after {after:0.000}s!");
        }

        public override void ReportPeripheralStimulusDied(PeripheralStimulus active)
        {
            if (active != activeStimulus)
            {
                Debug.LogError($"{active} stimulus is not the active one, don't care if it died!");
                return;
            }

            Debug.Log($"{activeStimulus} has finished");
            Destroy(activeStimulus.gameObject);
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

            Debug.Log($"{centralStimulus} has finished");
            Destroy(centralStimulus.gameObject);
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            StartWithCentralStimulus();
        }

        private void Update()
        {
            if (!centralTimeOnly.HasValue) return;
            centralTimeOnly -= Time.deltaTime;
            if (centralTimeOnly > 0f) return;
            centralTimeOnly = null;
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
            centralStimulus.StartSimulating(this, Times.CentralTime + Times.CentralOutStimulusIn,
                Times.CentralOutStimulusIn);
            centralTimeOnly = Times.CentralTime;
        }

        private void StartWithStimulus()
        {
            activeStimulus = NewStimulus();
            activeStimulus.StartSimulating(stimulusType, this, Times.StimulusTime + Times.CentralOutStimulusIn,
                Times.CentralOutStimulusIn);
        }

        #endregion
    }
}