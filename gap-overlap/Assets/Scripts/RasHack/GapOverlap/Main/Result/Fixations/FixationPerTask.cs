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

        #endregion

        #region Fields

        private readonly MainSettings settings;

        private FixationPerStimulus central;
        private FixationPerStimulus peripheral;

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
            central = new FixationPerStimulus(stimulus, settings, time);
        }

        public void CentralDestroyed(float time)
        {
            central?.StimulusDestroyed(time);
            central = null;
        }

        public void PeripheralCreated(ScalableStimulus stimulus, float time)
        {
            peripheral = new FixationPerStimulus(stimulus, settings, time);
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
