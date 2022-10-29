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
        public StimulusSide Side;
        public AllResponseTimes Responses;
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

        public const string RESULTS_DIRECTORY = "Results";

        private string filename;
        private readonly List<TestRun> results = new();
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

        public void AttachMeasurement(TaskType taskType, StimuliType stimuliType, StimulusSide side,
            AllResponseTimes responses)
        {
            activeRun?.Measurements.Add(new TestMeasurement
            {
                TaskType = taskType,
                StimuliType = stimuliType,
                Side = side,
                Responses = responses
            });
        }

        public void FlushToDisk()
        {
            if (results.Count == 0) return;
            var fullFilename = RESULTS_DIRECTORY + "/" + filename + ".csv";

            Directory.CreateDirectory(RESULTS_DIRECTORY);

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
            header.Append(",").Append(Quote("left_eye_central_noticed_seconds"));
            header.Append(",").Append(Quote("left_eye_peripheral_noticed_seconds"));
            header.Append(",").Append(Quote("right_eye_central_noticed_seconds"));
            header.Append(",").Append(Quote("right_eye_peripheral_noticed_seconds"));

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
                csv.Append(",").Append(Quote(measurement.Side.ToString()));

                csv.Append(",").Append(FormatNullable(measurement.Responses.LeftEye.CentralResponse));
                csv.Append(",").Append(FormatNullable(measurement.Responses.LeftEye.PeripheralResponse));
                csv.Append(",").Append(FormatNullable(measurement.Responses.RightEye.CentralResponse));
                csv.Append(",").Append(FormatNullable(measurement.Responses.RightEye.PeripheralResponse));

                destination[index + i] = csv.ToString();
            }

            return index + results.Measurements.Count;
        }

        private static string Quote(string text)
        {
            return $"\"{text}\"";
        }

        private static string FormatNullable(float? value)
        {
            return value.HasValue ? Quote(value.Value.ToString("0.000")) : "null";
        }

        #endregion
    }
}