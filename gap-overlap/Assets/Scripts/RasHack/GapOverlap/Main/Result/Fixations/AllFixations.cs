using System.Collections.Generic;
using RasHack.GapOverlap.Main.Result.Fixations.CSV;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AllFixations
    {
        #region Fields
        private readonly AllFixationsCSV toCSV = new AllFixationsCSV();
        private readonly MainSettings settings;
        private FixationPerTask current;

        #endregion

        #region Properties
        public List<FixationPerTask> Tasks { get; private set; } = new List<FixationPerTask>();

        public string Subject { get; private set; }
        public string TestId { get; private set; }

        #endregion

        #region Constructors

        public AllFixations(string subject, string testId, MainSettings settings)
        {
            Subject = subject;
            TestId = testId;
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
            Tasks.Add(current);
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

        public string ToCSVHeader()
        {
            return toCSV.CSVHeader();
        }

        public string ToCSV()
        {
            return toCSV.ToCSV(this);
        }

        public string ToCSVHeaderAveraged()
        {
            return toCSV.CSVHeaderAveraged();
        }
        
        public string ToCSVAveraged()
        {
            var averaged = new AveragedAllFixations();
            averaged.Process(this);
            return toCSV.ToCSV(this, averaged);
        }

        #endregion
    }
}
