using System.Numerics;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class FixationPerTask
    {
        #region Properties

        public int TaskOrder { get; private set; }
        public TaskType Type { get; private set; }
        public Side Side { get; private set; }

        #endregion

        #region Fields

        private readonly Scaler scaler;

        private FixationPerStimulus central;
        private FixationPerStimulus peripheral;

        #endregion

        #region Constructors

        public FixationPerTask(int taskOrder, TaskType taskType, Side side, Scaler scaler)
        {
            TaskOrder = taskOrder;
            Type = taskType;
            Side = side;

            this.scaler = scaler;
        }

        #endregion

        #region API

        public void CentralCreated(Vector3 position, float time)
        {
            central = new FixationPerStimulus(scaler, position, time);
        }

        public void CentralDestroyed(float time)
        {
            central?.StimulusDestroyed(time);
            central = null;
        }

        public void PeripheralCreated(Vector3 position, float time)
        {
            peripheral = new FixationPerStimulus(scaler, position, time);
        }

        public void PeripheralDestroyed(float time)
        {
            peripheral?.StimulusDestroyed(time);
            peripheral = null;
        }

        public void TaskDone(float time)
        {
            CentralDestroyed(time);
            PeripheralDestroyed(time);
        }

        public void Update(Vector3 leftEyePosition, Vector3 rightEyePosition, float time)
        {
            central?.Update(leftEyePosition, rightEyePosition, time);
            peripheral?.Update(leftEyePosition, rightEyePosition, time);
        }

        #endregion
    }
}
