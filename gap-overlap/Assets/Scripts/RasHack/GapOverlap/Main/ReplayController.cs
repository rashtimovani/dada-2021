using System;
using System.IO;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Result;

namespace RasHack.GapOverlap.Main
{
    public class ReplayController
    {
        #region Fields

        private SampledTest toReplay;

        #endregion

        #region API

        public void StartReplay()
        {
            toReplay = null;
            var bytes = File.ReadAllBytes("../gap-overlap-results/ANIKA IZDA/ANIKAIZDA_57946701-fd62-444c-b4af-43570d1c0b56.json");

            var deserializer = JsonSerializer.CreateDefault();

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                toReplay = deserializer.Deserialize<SampledTest>(reader);
            }
        }

        #endregion

        #region Methods

        public void Tick(float deltaTime)
        {
            if (toReplay == null) return;
        }

        #endregion

    }
}