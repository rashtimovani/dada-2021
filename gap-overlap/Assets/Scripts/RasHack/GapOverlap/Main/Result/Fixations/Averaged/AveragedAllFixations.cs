using System;
using RasHack.GapOverlap.Main.Task;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedAllFixations
    {
        #region Properties

        public AveragedPerTask AveragedGap { get; private set; } = new AveragedPerTask(TaskType.Gap);
        public AveragedPerTask AveragedOverlap { get; private set; } = new AveragedPerTask(TaskType.Overlap);
        public AveragedPerTask AveragedBaseline { get; private set; } = new AveragedPerTask(TaskType.Baseline);

        #endregion

        #region API

        public void Process(AllFixations allFixations)
        {
            foreach (var task in allFixations.Tasks)
            {
                Get(task.Type).Add(task);
            }
        }

        #endregion

        #region Helpers

        private AveragedPerTask Get(TaskType type)
        {
            return type switch
            {
                TaskType.Gap => AveragedGap,
                TaskType.Overlap => AveragedOverlap,
                TaskType.Baseline => AveragedBaseline,
                _ => throw new Exception("Unknown task type: " + type)
            };
        }

        #endregion
    }
}