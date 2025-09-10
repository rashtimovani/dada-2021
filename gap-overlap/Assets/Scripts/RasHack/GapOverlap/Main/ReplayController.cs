using System.IO;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Result;

namespace RasHack.GapOverlap.Main
{
    public class ReplayController
    {
        #region API

        public void StartReplay()
        {
            var bytes = File.ReadAllBytes("../gap-overlap-results/ANIKA IZDA/ANIKAIZDA_57946701-fd62-444c-b4af-43570d1c0b56.json");

            var deserializer = JsonSerializer.CreateDefault();

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                var result = deserializer.Deserialize<SampledTest>(reader);
                UnityEngine.Debug.Log($"Deserialized {result.Name} with {result.Samples.AllSamples.Count} measurements");
            }
        }

        #endregion

    }
}