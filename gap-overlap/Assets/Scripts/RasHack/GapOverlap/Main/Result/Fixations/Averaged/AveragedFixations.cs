namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedGazes
    {
        #region Fields

        private readonly Bucket after = new Bucket();
        private readonly Bucket duration = new Bucket();

        private readonly Bucket distance = new Bucket();

        #endregion

        #region Properties

        public float AverageAfter => after.Average;
        public float AverageDuration => duration.Average;

        public float P25Distance => distance.P25;
        public float P50Distance => distance.P50;
        public float P95Distance => distance.P95;

        #endregion

        #region API

        public void Add(SingleGaze gaze)
        {
            if (!gaze.IsDetected) return;

            after.Add(gaze.After);
            duration.Add(gaze.Duration);
            distance.AddRange(gaze.Distances);
        }

        #endregion
    }
}