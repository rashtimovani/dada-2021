using System;
using RasHack.GapOverlap.Main.Result.Fixations;

namespace RasHack.GapOverlap.Main.Result
{
    public class ReplayedTest
    {
        #region Fields

        public readonly SampledTest Test;
        private float spentTime;
        private int currentSampleIndex;
        public readonly AllFixations AllFixations;
        private readonly string resultsDirectory;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test, string resultsDirectory, Scaler scaler)
        {
            Test = test;
            currentSampleIndex = 0;
            spentTime = test.Samples.AllSamples[currentSampleIndex].Time;
            AllFixations = new AllFixations(scaler);
            this.resultsDirectory = resultsDirectory;
        }

        #endregion

        #region Methods

        public float SpentTime => spentTime;

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

            spentTime = toTime;

            var stillRunning = currentSampleIndex < Test.Samples.AllSamples.Count;
            if (!stillRunning) AllFixations.ToCSV(resultsDirectory, Test.Name, Test.TestId);
            return stillRunning;
        }

        #endregion
    }
}