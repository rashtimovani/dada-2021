using System.Collections.Generic;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AllFixations
    {
        #region Fields
        private readonly List<FixationPerTask> tasks = new List<FixationPerTask>();
        private readonly MainSettings settings;

        private FixationPerTask current;

        #endregion

        #region Constructors

        public AllFixations(MainSettings settings)
        {
            this.settings = settings;
        }

        #endregion

        #region API

        public void NewTask(int order, TaskType type, StimulusSide side, float time)
        {
            if (current == null || current.TaskOrder != order)
            {
                TaskDone(time);
                current = new FixationPerTask(order, type, side, settings);
            }
        }

        public void TaskDone(float time)
        {
            if (current == null) return;

            current.TaskDone(time);
            tasks.Add(current);
            current = null;
        }

        public void CentralCreated(ScalableStimulus stimulus, float time)
        {
            current?.CentralCreated(stimulus, time);
        }

        public void CentralDestroyed(float time)
        {
            current?.CentralDestroyed(time);
        }

        public void PeripheralCreated(ScalableStimulus stimulus, float time)
        {
            current?.PeripheralCreated(stimulus, time);
        }

        public void PeripheralDestroyed(float time)
        {
            current?.PeripheralDestroyed(time);
        }

        public void Update(Vector3 leftEyePosition, Vector3 rightEyePosition, float time)
        {
            current?.Update(leftEyePosition, rightEyePosition, time);
        }

        public void ToCSV(string resultsDirectory, string name, string testId)
        {
            // TODO: sort this method out
        }

        #endregion
    }
}
