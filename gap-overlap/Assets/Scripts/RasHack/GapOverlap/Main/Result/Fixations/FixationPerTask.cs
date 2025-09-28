using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class FixationPerTask
    {
        #region Properties

        public int TaskOrder { get; private set; }
        public TaskType Type { get; private set; }
        public StimulusSide Side { get; private set; }

        public FixationPerStimulus Central { get; private set; }
        public FixationPerStimulus Peripheral { get; private set; }

        public bool IsValid => Central.Both.IsDetected && Peripheral.Both.IsDetected && Central.Both.After < Peripheral.Both.After;

        public bool IsFinishedProperly => Central != null && Peripheral != null;

        #endregion

        #region Fields

        private readonly MainSettings settings;

        #endregion

        #region Constructors

        public FixationPerTask(int taskOrder, TaskType taskType, StimulusSide side, MainSettings settings)
        {
            TaskOrder = taskOrder;
            Type = taskType;
            Side = side;

            this.settings = settings;
        }

        #endregion

        #region API

        public void CentralCreated(ScalableStimulus stimulus, float time)
        {
            Central = new FixationPerStimulus($"{TaskOrder}. central", stimulus, settings, time);
        }

        public void CentralDestroyed(float time)
        {
            Central?.StimulusDestroyed(time);
        }

        public void PeripheralCreated(ScalableStimulus stimulus, float time)
        {
            Peripheral = new FixationPerStimulus($"{TaskOrder}. peripheral", stimulus, settings, time);
        }

        public void PeripheralDestroyed(float time)
        {
            Peripheral?.StimulusDestroyed(time);
        }

        public void TaskDone(float time)
        {
            CentralDestroyed(time);
            PeripheralDestroyed(time);
        }

        public void Update(Vector3 leftEyePosition, Vector3 rightEyePosition, float time)
        {
            Central?.Update(leftEyePosition, rightEyePosition, time);
            Peripheral?.Update(leftEyePosition, rightEyePosition, time);
        }

        #endregion
    }
}
