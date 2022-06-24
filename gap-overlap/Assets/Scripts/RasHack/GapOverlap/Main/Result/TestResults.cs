using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEditor;

namespace RasHack.GapOverlap.Main.Result
{
    public struct TestMeasurement
    {
        public TaskType TaskType;
        public StimuliType StimuliType;
        public string Side;
        public float? ResponseTime;
        public float? CentralResponseTime;
    }

    public struct TestRun
    {
        public string Name;
        public string Id;
        public List<TestMeasurement> Measurements;
    }

    public class TestResults
    {
        #region Fields

        private string filename;
        private readonly List<TestRun> results = new List<TestRun>();
        private TestRun? activeRun;

        #endregion

        #region API

        public void StartTest(string name)
        {
            filename ??= $"{DateTime.Now:yyyy-MM-dd_HH-mm}";

            activeRun = new TestRun
            {
                Name = name,
                Id = GUID.Generate().ToString(),
                Measurements = new List<TestMeasurement>()
            };
        }

        public void EndActiveTest()
        {
            if (!activeRun.HasValue) return;

            results.Add(activeRun.Value);
            activeRun = null;

            FlushToDisk();
        }

        public void AbortActiveTest()
        {
            activeRun = null;

            FlushToDisk();
        }

        public void AttachMeasurement(TaskType taskType, StimuliType stimuliType, string side, float? responseTime,
            float? centralResponseTime)
        {
            activeRun?.Measurements.Add(new TestMeasurement
            {
                TaskType = taskType,
                StimuliType = stimuliType,
                Side = side,
                ResponseTime = responseTime,
                CentralResponseTime = centralResponseTime
            });
        }

        public void FlushToDisk()
        {
            if (results.Count == 0) return;
            var fullFilename = filename + ".csv";

            if (!File.Exists(@fullFilename))
            {
                var headerLines = new string[1];
                headerLines[0] = Header();
                File.WriteAllLines(@fullFilename, headerLines);
            }

            var totalLines = 0;
            for (var i = 0; i < results.Count; i++)
            {
                totalLines += results[i].Measurements.Count;
            }

            var lines = new string[totalLines];

            var nextLineIndex = 0;
            for (var i = 0; i < results.Count; i++)
            {
                nextLineIndex = ResultsCsv(results[i], lines, nextLineIndex);
            }

            File.AppendAllLines(@fullFilename, lines);

            results.Clear();
        }

        #endregion

        #region Helpers

        private string Header()
        {
            var header = new StringBuilder(Quote("name"));

            header.Append(",").Append(Quote("test_id"));
            header.Append(",").Append(Quote("task_type"));
            header.Append(",").Append(Quote("stimuli_type"));
            header.Append(",").Append(Quote("side"));
            header.Append(",").Append(Quote("reaction_time"));
            header.Append(",").Append(Quote("central_noticed_time"));

            return header.ToString();
        }


        private static int ResultsCsv(TestRun results, IList<string> destination, int index)
        {
            for (var i = 0; i < results.Measurements.Count; i++)
            {
                var csv = new StringBuilder(Quote(results.Name)).Append(",").Append(Quote(results.Id));

                var measurement = results.Measurements[i];
                csv.Append(",").Append(Quote(measurement.TaskType.ToString()));
                csv.Append(",").Append(Quote(measurement.StimuliType.ToString()));
                csv.Append(",").Append(Quote(measurement.Side));

                var current = measurement.ResponseTime;
                var formatted = current.HasValue ? Quote(current.Value.ToString("0.000")) : "null";
                csv.Append(",").Append(formatted);

                var central = measurement.CentralResponseTime;
                var centralFormatted = central.HasValue ? Quote(central.Value.ToString("0.000")) : "null";
                csv.Append(",").Append(centralFormatted);
                
                destination[index + i] = csv.ToString();
            }

            return index + results.Measurements.Count;
        }

        private static string Quote(string text)
        {
            return $"\"{text}\"";
        }

        #endregion
    }
}