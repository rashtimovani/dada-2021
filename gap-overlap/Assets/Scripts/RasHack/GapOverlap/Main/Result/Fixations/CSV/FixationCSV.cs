using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class FixationCSV : DumpToCSV
    {
        #region API

        public string CSVHeader(string classifier)
        {
            return $"{Quote($"{classifier} fixated after s")},{Quote($"{classifier} fixation duration s")}";
        }

        public string ToCSV(Fixation fixation)
        {
            if (!fixation.IsDetected) return NaN(2);

            var after = fixation.After;
            var duration = fixation.Duration;
            return $"{Quote(after)},{Quote(duration)}";
        }

        #endregion
    }
}