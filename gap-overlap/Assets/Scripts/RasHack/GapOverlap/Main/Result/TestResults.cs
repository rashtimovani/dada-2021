using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RasHack.GapOverlap.Main.Result
{
    public struct TestMeasurement
    {
        public string TestName;
        public float? ResponseTime;
    }

    public struct TestRun
    {
        public string Name;
        public List<TestMeasurement> Measurements;
    }

    public class TestResults
    {
        #region Fields

        private string filename;
        private readonly List<TestRun> Results = new List<TestRun>();
        private TestRun? ActiveRun;

        #endregion

        #region API

        public void StartTest(string name)
        {
            filename ??= $"{DateTime.Now:yyyy-MM-dd_HH-mm}";

            ActiveRun = new TestRun
            {
                Name = name,
                Measurements = new List<TestMeasurement>()
            };
        }

        public void EndActiveTest()
        {
            if (!ActiveRun.HasValue) return;

            Results.Add(ActiveRun.Value);
            ActiveRun = null;

            FlushToDisk();
        }

        public void AbortActiveTest()
        {
            ActiveRun = null;
            
            FlushToDisk();
        }

        public void AttachMeasurement(string testName, float? responseTime)
        {
            ActiveRun?.Measurements.Add(new TestMeasurement
            {
                TestName = testName,
                ResponseTime = responseTime
            });
        }

        public void FlushToDisk()
        {
            if (Results.Count == 0) return;
            var fullFilename = filename + ".csv";

            if (!File.Exists(@fullFilename))
            {
                var headerLines = new string[1];
                headerLines[0] = Header();
                File.WriteAllLines(@fullFilename, headerLines);
            }

            var lines = new string[Results.Count];

            for (var i = 0; i < Results.Count; i++)
            {
                lines[i] = ResultsCsv(i);
            }

            File.AppendAllLines(@fullFilename, lines);

            Results.Clear();
        }

        #endregion

        #region Helpers

        private string Header()
        {
            var header = new StringBuilder(Quote("name"));

            for (var i = 0; i < Results[0].Measurements.Count; i++)
            {
                header.Append(",").Append(Quote(Results[0].Measurements[i].TestName));
            }

            return header.ToString();
        }


        private string ResultsCsv(int index)
        {
            var results = Results[index];
            var csv = new StringBuilder(Quote(results.Name));

            for (var i = 0; i < Results[0].Measurements.Count; i++)
            {
                var current = results.Measurements[i].ResponseTime;
                var formatted = current.HasValue ? current.Value.ToString("0.000") : "N/A";
                csv.Append(",").Append(Quote(formatted));
            }

            return csv.ToString();
        }

        private static string Quote(string text)
        {
            return $"\"{text}\"";
        }

        #endregion
    }
}