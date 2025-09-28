using Newtonsoft.Json.Serialization;
using RasHack.GapOverlap.Main.Task;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedPerTask
    {
        #region Fields

        private int countSuccess;

        #endregion

        #region Properties

        public AveragedPerStimulus Central { get; private set; } = new AveragedPerStimulus();
        public AveragedPerStimulus Peripheral { get; private set; } = new AveragedPerStimulus();

        public TaskType Type { get; private set; }

        public int CountTotal { get; private set; } = 0;
        public float SuccessRate => CountTotal == 0 ? 0f : countSuccess / (float)CountTotal;

        #endregion

        #region Constructors

        public AveragedPerTask(TaskType type)
        {
            Type = type;
        }

        #endregion

        #region API

        public void Add(FixationPerTask perTask)
        {
            CountTotal++;
            if (!perTask.IsValid) return;

            countSuccess++;
            Central.Add(perTask.Central);
            Peripheral.Add(perTask.Peripheral);
        }

        #endregion
    }
}