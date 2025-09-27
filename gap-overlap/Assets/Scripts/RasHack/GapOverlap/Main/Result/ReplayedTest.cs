using System;

namespace RasHack.GapOverlap.Main.Result
{
    public class ReplayedTest
    {
        #region Fields

        public readonly SampledTest Test;
        private float spentTime;
        private int currentSampleIndex;
        public readonly Timers Timers;
        private readonly string resultsDirectory;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test, string resultsDirectory)
        {
            Test = test;
            currentSampleIndex = 0;
            spentTime = test.Samples.AllSamples[currentSampleIndex].Time;
            Timers = new Timers();
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
            if (!stillRunning) Timers.ToCSV(resultsDirectory, Test.Name, Test.TestId);
            return stillRunning;
        }

        #endregion
    }
}