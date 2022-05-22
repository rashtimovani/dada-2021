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

        #region API

        protected override TaskType TaskType => TaskType.Baseline;
        
        private BaselineTimes Times => owner.Settings?.BaselineTimes ?? times;

        public override void ReportFocusedOn(Stimulus stimulus, float after)
        {
            throw new NotImplementedException();
        }

        public override void ReportCentralStimulusDied(CentralStimulus central)
        {
            throw new NotImplementedException();
        }

        public override void ReportStimulusDied(Stimulus active)
        {
            throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}