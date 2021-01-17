using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RasHack.GapOverlap.Main
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

        #endregion

        #region API

        public void StartTest(string name)
        {
            if (filename == null)
            {
                filename = $"{DateTime.Now:yyyy-MM-dd_HH-mm}";
            }

            Results.Add(new TestRun
            {
                Name = name,
                Measurements = new List<TestMeasurement>()
            });
        }

        public void AttachMeasurement(string testName, float? responseTime)
        {
            Results[Results.Count - 1].Measurements.Add(new TestMeasurement
            {
                TestName = testName,
                ResponseTime = responseTime
            });
        }

        public void FlushToDisk()
        {
            if (Results.Count == 0) return;

            var lines = new string[Results.Count + 1];
            lines[0] = Header();

            for (var i = 0; i < Results.Count; i++)
            {
                lines[i + 1] = ResultsCsv(i);
            }

            var fullFilename = filename + ".csv";
            File.WriteAllLines(@fullFilename, lines);
            filename = null;
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