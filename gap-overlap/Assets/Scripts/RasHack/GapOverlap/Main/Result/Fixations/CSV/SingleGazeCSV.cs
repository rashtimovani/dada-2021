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

        public string ToCSV(SingleGaze gaze)
        {
            if (!gaze.IsDetected) return NaN(2);

            var after = gaze.After;
            var duration = gaze.Duration;
            var p25Distance = gaze.Distances.P25;
            var p50Distance = gaze.Distances.P50;
            var p95Distance = gaze.Distances.P95;
            return $"{Quote(after)},{Quote(duration)},{Quote(p25Distance)},{Quote(p50Distance)},{Quote(p95Distance)}";
        }

        public string CSVHeaderAveraged(string classifier)
        {
            return $"{Quote($"{classifier} gaze after s on average")},{Quote($"{classifier} average gaze duration s")},{Quote($"{classifier} p25 distance in degrees")},{Quote($"{classifier} p50 distance in degrees")},{Quote($"{classifier} p95 distance in degrees")}";
        }

        public string ToCSV(AveragedGazes gaze)
        {
            var after = gaze.AverageAfter;
            var duration = gaze.AverageDuration;
            var p25Distance = gaze.P25Distance;
            var p50Distance = gaze.P50Distance;
            var p95Distance = gaze.P95Distance;
            return $"{Quote(after)},{Quote(duration)},{Quote(p25Distance)},{Quote(p50Distance)},{Quote(p95Distance)}";
        }

        #endregion
    }
}