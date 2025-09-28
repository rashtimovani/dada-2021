using System;
using System.IO;
using RasHack.GapOverlap.Main.Result.Fixations;
using RasHack.GapOverlap.Main.Settings;

namespace RasHack.GapOverlap.Main.Result
{
    public class ResultDirectory
    {
        #region Properties

        public string Parent { get; private set; }
        public string Results { get; private set; }

        #endregion

        #region Constructors

        public ResultDirectory(string parent)
        {
            Parent = parent;
            Results = ResultsFromParent(parent);
        }

        #endregion

        #region API

        public void Prepare()
        {
            if (!Directory.Exists(Results)) Directory.CreateDirectory(Results);
        }

        public void WriteRawResults(string subject, string testId, string header, string csv)
        {
            File.AppendAllText(PrepareAllRawResults(header), csv);

            var single = $"{Results}/{subject}_{testId}_detection.csv";
            File.WriteAllText(single, header + "\n");
            File.AppendAllText(single, csv);
        }

        public void WriteAveragedResults(string header, string csv)
        {
            File.AppendAllText(PrepareAveragedResults(header), csv);
        }

        private string PrepareAllRawResults(string header)
        {
            var allResults = Results + "/all_raw_detections.csv";
            if (!File.Exists(allResults)) File.WriteAllText(allResults, $"{header}\n");
            return allResults;
        }

        private string PrepareAveragedResults(string header)
        {
            var allResults = Results + "/averaged_detections.csv";
            if (!File.Exists(allResults)) File.WriteAllText(allResults, $"{header}\n");
            return allResults;
        }

        public void DeleteAll()
        {
            if (!Directory.Exists(Results)) return;

            foreach (var file in Directory.GetFiles(Results))
            {
                File.Delete(file);
            }

            foreach (var dir in Directory.GetDirectories(Results))
            {
                Directory.Delete(dir, true);
            }
        }

        #endregion

        #region Helpers

        private static string ResultsFromParent(string parent)
        {
            return parent + "/detections";
        }

        #endregion
    }
}