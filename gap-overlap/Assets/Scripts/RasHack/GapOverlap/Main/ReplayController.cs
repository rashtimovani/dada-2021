using System;
using System.IO;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Result;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class ReplayedTest
    {
        #region Fields

        private readonly SampledTest toReplay;
        private float spentTime;
        private int currentSampleIndex;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test)
        {
            toReplay = test;
            currentSampleIndex = 0;
            spentTime = toReplay.Samples.AllSamples[currentSampleIndex].Time;
        }

        #endregion

        #region Methods

        public void Tick(float deltaTime)
        {
            var toTime = spentTime + deltaTime;
            while (currentSampleIndex < toReplay.Samples.AllSamples.Count)
            {
                var sample = toReplay.Samples.AllSamples[currentSampleIndex];
                if (toTime < sample.Time) break;
                Debug.Log($"Replaying sample at {sample.Time}s: {sample.Task.TaskType}");

                currentSampleIndex++;
            }

            spentTime = toTime;
        }

        #endregion
    }

    public class ReplayController
    {
        #region Fields

        private ReplayedTest toReplay;

        #endregion

        #region API

        public void StartReplay()
        {
            toReplay = null;
            var bytes = File.ReadAllBytes("../gap-overlap-results/ANIKA IZDA/ANIKAIZDA_57946701-fd62-444c-b4af-43570d1c0b56.json");

            var deserializer = JsonSerializer.CreateDefault();

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                toReplay = new ReplayedTest(deserializer.Deserialize<SampledTest>(reader));
            }
        }

        #endregion

        #region Methods

        public void Tick(float deltaTime)
        {
            if (toReplay == null) return;

            toReplay.Tick(deltaTime);
        }

        #endregion

    }
}