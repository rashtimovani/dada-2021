using System;
using System.Collections.Generic;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RasHack.GapOverlap.Main.Task
{
    public enum TaskType
    {
        Gap = 0,
        Overlap = 1,
    }

    [Serializable]
    public struct TaskCount
    {
        public int Gaps;
        public int Overlaps;
    }

    public class TaskOrder : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private GameObject gapPrefab;
        [SerializeField] private GameObject overlapPrefab;

        #endregion

        #region Fields

        private int currentIndex;
        private int remainingGaps;
        private int remainingOverlaps;

        #endregion

        #region API

        public void Reset(TaskCount taskCount)
        {
            currentIndex = 0;
            remainingGaps = taskCount.Gaps;
            remainingOverlaps = taskCount.Overlaps;
        }

        public Task CreateNext(StimuliType type)
        {
            if (remainingGaps <= 0 && remainingOverlaps <= 0) return null;

            TaskType next;
            if (remainingGaps == 0) next = TaskType.Overlap;
            else
            {
                var weighted = remainingGaps + remainingOverlaps;
                var rounded = Mathf.FloorToInt(Random.value * weighted);
                next = rounded < remainingGaps ? TaskType.Gap : TaskType.Overlap;
            }

            currentIndex++;

            switch (next)
            {
                case TaskType.Overlap:
                    remainingOverlaps--;
                    var newOverlap = Instantiate(overlapPrefab, Vector3.zero, Quaternion.identity);
                    newOverlap.name = $"Overlap_{currentIndex}_{type}";
                    return newOverlap.GetComponent<Overlap>();
                default:
                    remainingGaps--;
                    var newGap = Instantiate(gapPrefab, Vector3.zero, Quaternion.identity);
                    newGap.name = $"Gap_{currentIndex}_{type}";
                    return newGap.GetComponent<Gap>();
            }
        }

        #endregion
    }
}