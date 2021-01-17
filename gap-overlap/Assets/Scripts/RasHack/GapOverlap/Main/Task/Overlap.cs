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

        [SerializeField] private OverlapTimes times = new OverlapTimes {CentralTime = 1f, BothStimuli = 2.5f};

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
            Destroy(centralStimulus.gameObject);
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

            Debug.Log($"Only {centralStimulus} has finished");
            StartWithStimulus();
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            StartWithCentralStimulus();
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
            var where = Vector3.Lerp(Scaler.TopLeft, Scaler.BottomRight, 0.33f);
            activeStimulus = NewStimulus(where);
            activeStimulus.StartSimulating(stimulusType, this, times.BothStimuli);
        }

        #endregion
    }
}