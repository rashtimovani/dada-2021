using System.Collections.Generic;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AllFixations
    {
        #region Fields
        private readonly List<FixationPerTask> tasks = new List<FixationPerTask>();
        private readonly Scaler scaler;

        private FixationPerTask current;

        #endregion

        #region Constructors

        public AllFixations(Scaler scaler)
        {
            this.scaler = scaler;
        }

        #endregion

        #region API

        public void NewTask(int order, TaskType type, StimulusSide side, float time)
        {
            if (current == null || current.TaskOrder != order)
            {
                TaskDone(time);
                current = new FixationPerTask(order, type, side, scaler);
            }
        }

        public void TaskDone(float time)
        {
            if (current == null) return;

            current.TaskDone(time);
            tasks.Add(current);
            current = null;
        }

        public void CentralCreated(Vector3 position, float time)
        {
            current?.CentralCreated(position, time);
        }

        public void CentralDestroyed(float time)
        {
            current?.CentralDestroyed(time);
        }

        public void PeripheralCreated(Vector3 position, float time)
        {
            current?.PeripheralCreated(position, time);
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
