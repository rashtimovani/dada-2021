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
        public static StimuliType next(this StimuliType current)
        {
            if (current >= StimuliType.Umbrella || current < StimuliType.Bee) return StimuliType.Bee;
            return current + 1;
        }
    }
}