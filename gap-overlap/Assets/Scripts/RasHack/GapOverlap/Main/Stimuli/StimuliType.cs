using Random = UnityEngine.Random;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public enum StimuliType
    {
        Bee = 0,
        Bird = 1,
        Owl = 2,
        Rainbow = 3,
        RainCloud = 4,
        Umbrella = 5
    }

    public static class StimuliTypeExtensions
    {
        public static StimuliType Next()
        {
            return (StimuliType) Random.Range((int) StimuliType.Bee, (int) StimuliType.Umbrella + 1);
        }
    }
}