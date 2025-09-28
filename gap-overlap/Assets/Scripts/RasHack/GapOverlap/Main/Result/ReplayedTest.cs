using System;
using RasHack.GapOverlap.Main.Result.Fixations;
using RasHack.GapOverlap.Main.Settings;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    public class ReplayedTest
    {
        #region Fields

        public readonly SampledTest Test;
        private float spentTime;
        private int currentSampleIndex;
        public readonly AllFixations AllFixations;
        private readonly ResultDirectory results;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test, ResultDirectory results, MainSettings settings)
        {
            Test = test;
            currentSampleIndex = 0;
            spentTime = test.Samples.AllSamples[currentSampleIndex].Time;

            this.results = results;

            AllFixations = new AllFixations(test.Name, test.TestId, settings);
        }

        #endregion

        #region Methods

        public float SpentTime => spentTime;

        public bool IsValid => AllFixations.IsValid;

        public bool Tick(float deltaTime, Action<Sample> onNextSample)
        {
            var toTime = spentTime + deltaTime;
            while (currentSampleIndex < Test.Samples.AllSamples.Count)
            {
                var sample = Test.Samples.AllSamples[currentSampleIndex];
                if (toTime < sample.Time) break;

                currentSampleIndex++;
                spentTime = sample.Time;
                onNextSample.Invoke(sample);
            }

            var stillRunning = currentSampleIndex < Test.Samples.AllSamples.Count;
            if (!stillRunning)
            {
                if (IsValid)
                {
                    AllFixations.TaskDone(spentTime);
                    AllFixationsToCSV();
                }
                else Debug.LogWarning($"Test of {Test.Name} with id {Test.TestId} is not valid and results will be ignored!");
            }

            spentTime = toTime;
            return stillRunning;
        }

        private void AllFixationsToCSV()
        {
            var header = AllFixations.ToCSVHeader();
            var csv = AllFixations.ToCSV();
            results.WriteRawResults(AllFixations.Subject, AllFixations.TestId, header, csv);

            var averagedHeader = AllFixations.ToCSVHeaderAveraged();
            var averagedCsv = AllFixations.ToCSVAveraged();
            results.WriteAveragedResults(averagedHeader, averagedCsv);
        }

        #endregion
    }
}