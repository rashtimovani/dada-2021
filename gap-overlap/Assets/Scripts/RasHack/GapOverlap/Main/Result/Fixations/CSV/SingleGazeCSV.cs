using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class SingleGazeCSV : DumpToCSV
    {
        #region API

        public string CSVHeader(string classifier)
        {
            return $"{Quote($"{classifier} gaze after s")},{Quote($"{classifier} gaze duration s")},{Quote($"{classifier} best fixation s")},{Quote($"{classifier} p10 distance in degrees")},{Quote($"{classifier} p50 distance in degrees")},{Quote($"{classifier} p95 distance in degrees")}";
        }

        public string ToCSV(SingleGaze gaze)
        {
            if (!gaze.IsDetected) return NaN(2);

            var after = gaze.After;
            var duration = gaze.Duration;
            var fixation = gaze.BestFixation;
            var p10Distance = gaze.Distances.P10;
            var p50Distance = gaze.Distances.P50;
            var p95Distance = gaze.Distances.P95;
            return $"{Quote(after)},{Quote(duration)},{Quote(fixation)},{Quote(p10Distance)},{Quote(p50Distance)},{Quote(p95Distance)}";
        }

        public string CSVHeaderAveraged(string classifier)
        {
            return $"{Quote($"{classifier} gaze after s on average")},{Quote($"{classifier} average gaze duration s")},{Quote($"{classifier} average best fixation s")},{Quote($"{classifier} p10 distance in degrees")},{Quote($"{classifier} p50 distance in degrees")},{Quote($"{classifier} p95 distance in degrees")}";
        }

        public string ToCSV(AveragedGazes gaze)
        {
            var after = gaze.AverageAfter;
            var duration = gaze.AverageDuration;
            var fixation = gaze.AverageFixation;
            var p10Distance = gaze.P10Distance;
            var p50Distance = gaze.P50Distance;
            var p95Distance = gaze.P95Distance;
            return $"{Quote(after)},{Quote(duration)},{Quote(fixation)},{Quote(p10Distance)},{Quote(p50Distance)},{Quote(p95Distance)}";
        }

        #endregion
    }
}