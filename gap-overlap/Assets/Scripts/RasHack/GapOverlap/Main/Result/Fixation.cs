using System;
using System.Globalization;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    public struct FixationResult
    {
        #region Fields

        public float Duration;

        public bool EndedNow;

        #endregion
    }

    public class Fixation
    {
        #region Fields

        private readonly float start;

        private float? end;

        #endregion

        #region Properties

        public float Duration => end != null ? Mathf.Max(end.Value - start, 0f) : float.NaN;

        #endregion

        #region Constructors

        public Fixation(float start)
        {
            this.start = start;
        }

        #endregion

        #region API

        public static FixationResult BadResult()
        {
            return new FixationResult { Duration = float.NaN, EndedNow = false };
        }

        public FixationResult FixationEnded(float time)
        {
            var endedNow = false;
            if (end == null)
            {
                end = time;
                endedNow = true;
            }
            return new FixationResult { Duration = Duration, EndedNow = endedNow };
        }

        public string ToCSV()
        {
            return end != null ? Duration.ToString("0.000", CultureInfo.InvariantCulture) : "NaN";
        }

        #endregion
    }
}