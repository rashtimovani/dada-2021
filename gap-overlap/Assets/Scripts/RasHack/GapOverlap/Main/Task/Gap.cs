using System;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    [Serializable]
    public struct GapTimes
    {
        public float CentralTime;
        public float PauseTime;
        public float StimulusTime;
    }

    public class Gap : Task
    {
        #region Serialized fields

        [SerializeField] private GapTimes times = new GapTimes {CentralTime = 1f, PauseTime = 0.5f, StimulusTime = 2f};

        #endregion

        #region Fields

        private float? waitingTime;
        private CentralStimulus centralStimulus;
        private Stimulus activeStimulus;

        #endregion

        #region API

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
            owner.ReportTaskFinished(this);
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
            centralStimulus = null;
            waitingTime = times.PauseTime;
        }

        #endregion

        #region Mono methods

        private void Start()
        {
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

        #endregion

        #region Helpers

        private void StartWithCentralStimulus()
        {
            centralStimulus = NewCentralStimulus();
            centralStimulus.StartSimulating(this, times.CentralTime);
        }

        private void StartWithStimulus()
        {
            activeStimulus = NewStimulus();
            activeStimulus.StartSimulating(stimulusType, this, times.StimulusTime);
        }

        #endregion
    }
}