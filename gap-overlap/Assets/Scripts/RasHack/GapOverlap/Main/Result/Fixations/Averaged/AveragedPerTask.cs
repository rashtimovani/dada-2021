using Newtonsoft.Json.Serialization;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedPerTask
    {
        #region Fields

        private int countSuccess;
        private int countTotal;

        #endregion

        #region Properties

        public AveragedPerStimulus Central { get; private set; } = new AveragedPerStimulus();
        public AveragedPerStimulus Peripheral { get; private set; } = new AveragedPerStimulus();

        public float SuccessRate => (float)countSuccess / (float)countTotal;

        #endregion

        #region API

        public void Add(FixationPerTask perTask)
        {
            countTotal++;
            if (!perTask.IsValid) return;

            countSuccess++;
            Central.Add(perTask.Central);
            Peripheral.Add(perTask.Peripheral);
        }

        #endregion
    }
}