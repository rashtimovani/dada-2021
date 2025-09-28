using System.Collections.Generic;
using RasHack.GapOverlap.Main.Result.Fixations.CSV;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedFixations
    {
        #region Fields

        #endregion

        #region Fields

        private readonly Bucket after = new Bucket();
        private readonly Bucket duration = new Bucket();
        
        #endregion

        #region Properties

        public float AverageAfter => after.Average;
        public float AverageDuration => duration.Average;

        #endregion

        #region API

        public void Add(Fixation fixation)
        {
            if (!fixation.IsDetected) return;
            
            after.Add(fixation.After);
            duration.Add(fixation.Duration);
        }

        #endregion
    }
}