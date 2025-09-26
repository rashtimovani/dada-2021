using System.Globalization;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    public class Fixation
    {
        #region Fields

        private readonly float start;

        private float? end;

        #endregion

        #region Properties

        public float Duration => Mathf.Max((end ?? start) - start, 0f);

        #endregion

        #region Constructors

        public Fixation(float start)
        {
            this.start = start;
        }

        #endregion

        #region API

        public float FixationEnded(float time)
        {
            end = time;
            return Duration;
        }

        public string ToCSV()
        {
            return end != null ? Duration.ToString("0.000", CultureInfo.InvariantCulture) : "NaN";
        }

        #endregion
    }
}