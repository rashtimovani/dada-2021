namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedFixations
    {
        #region Fields

        private readonly Bucket after = new Bucket();
        private readonly Bucket duration = new Bucket();
        
        #endregion

        #region Properties

        public float AverageAfter => after.Average;
        public float AverageDuration => duration.Average;

        #endregion

        #region API

        public void Add(Fixation fixation)
        {
            if (!fixation.IsDetected) return;

            after.Add(fixation.After);
            duration.Add(fixation.Duration);
        }

        #endregion
    }
}