using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class DumpToCSV
    {
        #region API

        public string Quote(string what)
        {
            return $"\"{what}\"";
        }

        public string Quote(int what)
        {
            return Quote(what.ToString());
        }

        public string Quote(float what)
        {
            return Quote(what.ToCSV());
        }

        public string NaN(int times = 1)
        {
            var nans = "";
            var separator = "";
            for (var i = 0; i < times; i++)
            {
                nans += separator;
                nans += Quote("NaN");
                separator = ",";
            }
            return nans;
        }

        #endregion
    }
    public static class DumpToCSVUtils {
        #region Utils

        public static string ToCSV(this float value)
        {
            return value.ToString("0.000", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}