using System.Globalization;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class FixationUtil
    {
        #region API

        public static string ToCSV(float value)
        {
            return value.ToString("0.000", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}