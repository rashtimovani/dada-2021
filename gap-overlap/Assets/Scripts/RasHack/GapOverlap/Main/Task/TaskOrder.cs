using System;
using System.Collections.Generic;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    public enum TaskType
    {
        Gap = 0,
        Overlap = 1,
        Baseline = 2,
    }

    [Serializable]
    public struct TaskCount
    {
        public int Gaps;
        public int LeftGaps;
        public int Overlaps;
        public int LeftOverlaps;
    }

    public class TaskOrder : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private GameObject gapPrefab;
        [SerializeField] private GameObject overlapPrefab;
        [SerializeField] private GameObject baselinePrefab;

        #endregion

        #region Fields

        private int currentIndex;

        private readonly ManagedRandom<TaskType> randomTasks =
            new ManagedRandom<TaskType>(TaskType.Gap, TaskType.Overlap);

        #endregion

        #region API

        public bool HasNext => randomTasks.HasNext;

        public void Reset(TaskCount taskCount)
        {
            currentIndex = 0;
            randomTasks.Reset(taskCount.Gaps, taskCount.Overlaps);
        }

        public void End()
        {
            randomTasks.End();
        }

        public Task CreateNext(StimuliType type)
        {
            if (!HasNext) return null;

            currentIndex++;
            var next = randomTasks.Next();

            switch (next)
            {
                case TaskType.Overlap:
                    var newOverlap = Instantiate(overlapPrefab, Vector3.zero, Quaternion.identity);
                    newOverlap.name = $"Overlap_{currentIndex}_{type}";
                    return newOverlap.GetComponent<Overlap>();
                default:
                    var newGap = Instantiate(gapPrefab, Vector3.zero, Quaternion.identity);
                    newGap.name = $"Gap_{currentIndex}_{type}";
                    return newGap.GetComponent<Gap>();
            }
        }

        #endregion
    }
}