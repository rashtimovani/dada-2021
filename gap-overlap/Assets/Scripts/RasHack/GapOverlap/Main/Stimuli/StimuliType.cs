namespace RasHack.GapOverlap.Main.Stimuli
{
    public enum StimuliType
    {
        Bee = 0,
        Octopus = 1,
        Rainbow = 2,
        RainCloud = 3,
        Umbrella = 4,
        YellowBird = 5
    }

    public static class StimuliTypeExtensions
    {
        public static StimuliType next(this StimuliType current)
        {
            if (current >= StimuliType.YellowBird || current < StimuliType.Bee) return StimuliType.Bee;
            return current + 1;
        }
    }
}