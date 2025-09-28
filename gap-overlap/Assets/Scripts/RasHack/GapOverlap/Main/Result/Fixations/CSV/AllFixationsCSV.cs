using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RasHack.GapOverlap.Main.Task;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class AllFixationsCSV : DumpToCSV
    {
        #region Fields

        private readonly FixationPerTaskCSV subCSV = new FixationPerTaskCSV();

        #endregion

        #region API

        public string CSVHeader()
        {
            return $"{Quote("Subject")},{Quote("Test id")},{subCSV.CSVHeader()}";
        }

        public string ToCSV(AllFixations allFixations)
        {
            var entries = new List<string>();
            foreach (var task in allFixations.Tasks)
            {
                entries.Add($"{Quote(allFixations.Subject)},{Quote(allFixations.TestId)},{subCSV.ToCSV(task)}");
            }

            return string.Join("\n", entries);
        }

        public string CSVHeaderAveraged()
        {
            return $"{Quote("Subject")},{Quote("Test id")},{subCSV.CSVHeaderAveraged(TaskType.Baseline)},{subCSV.CSVHeaderAveraged(TaskType.Gap)},{subCSV.CSVHeaderAveraged(TaskType.Overlap)}";
        }

        public string ToCSV(AllFixations allFixations, AveragedAllFixations averaged)
        {
            return ($"{Quote(allFixations.Subject)},{Quote(allFixations.TestId)},{subCSV.ToCSV(averaged.AveragedBaseline)},{subCSV.ToCSV(averaged.AveragedGap)},{subCSV.ToCSV(averaged.AveragedOverlap)}");
        }

        #endregion
    }
}