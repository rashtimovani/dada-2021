using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class FixationPerStimulusCSV : DumpToCSV
    {
        #region Fields

        private readonly SingleGazeCSV subCSV = new SingleGazeCSV();

        #endregion

        #region API

        public string CSVHeader(string classifier)
        {
            return $"{subCSV.CSVHeader($"{classifier} both eyes")},{subCSV.CSVHeader($"{classifier} left eye")},{subCSV.CSVHeader($"{classifier} right eye")},{Quote($"{classifier} duration s")}";
        }

        public string ToCSV(FixationPerStimulus perStimulus)
        {
            var both = perStimulus.Both;
            var left = perStimulus.Left;
            var right = perStimulus.Right;
            var duration = perStimulus.Duration;
            return $"{subCSV.ToCSV(both)},{subCSV.ToCSV(left)},{subCSV.ToCSV(right)},{Quote(duration)}";
        }

        public string CSVHeaderAveraged(string classifier)
        {
            return $"{subCSV.CSVHeaderAveraged($"{classifier} both eyes")},{subCSV.CSVHeaderAveraged($"{classifier} left eye")},{subCSV.CSVHeaderAveraged($"{classifier} right eye")},{Quote($"{classifier} average duration s")}";
        }

        public string ToCSV(AveragedPerStimulus perStimulus)
        {
            var both = perStimulus.Both;
            var left = perStimulus.Left;
            var right = perStimulus.Right;
            var duration = perStimulus.Duration;
            return $"{subCSV.ToCSV(both)},{subCSV.ToCSV(left)},{subCSV.ToCSV(right)},{Quote(duration)}";
        }

        #endregion
    }
}