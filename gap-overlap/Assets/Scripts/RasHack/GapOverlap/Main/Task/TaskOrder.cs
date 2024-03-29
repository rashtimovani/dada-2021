﻿using System;
using RasHack.GapOverlap.Common;
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
        public int Baselines;
        public int LeftBaselines;
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

        private readonly ManagedRandom<TaskType> randomTasks = new ManagedRandom<TaskType>();

        #endregion

        #region API

        public int CurrentTaskOrder { get; private set;}

        public bool HasNext => randomTasks.HasNext;

        public void Reset(TaskCount taskCount)
        {
            currentIndex = 0;
            CurrentTaskOrder = 0;
            randomTasks.SetOptions(new RandomOption<TaskType>(TaskType.Gap, taskCount.Gaps),
                new RandomOption<TaskType>(TaskType.Overlap, taskCount.Overlaps),
                new RandomOption<TaskType>(TaskType.Baseline, taskCount.Baselines));
        }

        public void End()
        {
            randomTasks.End();
        }

        public Task CreateNext(StimuliType type)
        {
            if (!HasNext) return null;

            currentIndex++;
            CurrentTaskOrder++;
            var next = randomTasks.Next();

            switch (next)
            {
                case TaskType.Overlap:
                    var newOverlap = Instantiate(overlapPrefab, Vector3.zero, Quaternion.identity);
                    newOverlap.name = $"Overlap_{currentIndex}_{type}";
                    return newOverlap.GetComponent<Overlap>();
                case TaskType.Baseline:
                    var newBaseline = Instantiate(baselinePrefab, Vector3.zero, Quaternion.identity);
                    newBaseline.name = $"Baseline_{currentIndex}_{type}";
                    return newBaseline.GetComponent<Baseline>();
                default:
                    var newGap = Instantiate(gapPrefab, Vector3.zero, Quaternion.identity);
                    newGap.name = $"Gap_{currentIndex}_{type}";
                    return newGap.GetComponent<Gap>();
            }
        }

        #endregion
    }
}