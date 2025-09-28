using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class SingleGazeCSV : DumpToCSV
    {
        #region API

        public string CSVHeader(string classifier)
        {
            return $"{Quote($"{classifier} gaze after s")},{Quote($"{classifier} gaze duration s")},{Quote($"{classifier} p25 distance in degrees")},{Quote($"{classifier} p50 distance in degrees")},{Quote($"{classifier} p95 distance in degrees")}";
        }

        public string ToCSV(SingleGaze fixation)
        {
            if (!fixation.IsDetected) return NaN(2);

            var after = fixation.After;
            var duration = fixation.Duration;
            var p25Distance = fixation.Distances.P25;
            var p50Distance = fixation.Distances.P50;
            var p95Distance = fixation.Distances.P95;
            return $"{Quote(after)},{Quote(duration)},{Quote(p25Distance)},{Quote(p50Distance)},{Quote(p95Distance)}";
        }

        public string CSVHeaderAveraged(string classifier)
        {
            return $"{Quote($"{classifier} gaze after s on average")},{Quote($"{classifier} average gaze duration s")},{Quote($"{classifier} p25 distance in degrees")},{Quote($"{classifier} p50 distance in degrees")},{Quote($"{classifier} p95 distance in degrees")}";
        }

        public string ToCSV(AveragedFixations fixation)
        {
            var after = fixation.AverageAfter;
            var duration = fixation.AverageDuration;
            var p25Distance = fixation.P25Distance;
            var p50Distance = fixation.P50Distance;
            var p95Distance = fixation.P95Distance;
            return $"{Quote(after)},{Quote(duration)},{Quote(p25Distance)},{Quote(p50Distance)},{Quote(p95Distance)}";
        }

        #endregion
    }
}