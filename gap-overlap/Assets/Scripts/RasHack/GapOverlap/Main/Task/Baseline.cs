using System;
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
    }

    public class Baseline : Task
    {
        #region Serialized fields

        [SerializeField] private BaselineTimes times = new BaselineTimes
            { CentralTime = 1f, CentralOutStimulusIn = 0.5f, StimulusTime = 2f };

        #endregion

        #region Fields

        private float? centralTimeOnly;
        private float? outInTime;
        private float? measurement;
        private CentralStimulus centralStimulus;
        private Stimulus activeStimulus;

        #endregion

        #region API

        protected override TaskType TaskType => TaskType.Baseline;

        private BaselineTimes Times => owner.Settings?.BaselineTimes ?? times;

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

        public override void ReportStimulusDied(Stimulus active)
        {
            if (active != activeStimulus)
            {
                Debug.LogError($"{active} stimulus is not the active one, don't care if it died!");
                return;
            }

            Debug.Log($"{activeStimulus} has finished");
            Destroy(activeStimulus.gameObject);
            activeStimulus = null;
            owner.ReportTaskFinished(this, measurement);
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