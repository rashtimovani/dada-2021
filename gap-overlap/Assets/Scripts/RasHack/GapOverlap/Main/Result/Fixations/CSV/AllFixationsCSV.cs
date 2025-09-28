using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

            return String.Join("\n", entries) + "\n";
        }

        #endregion
    }
}