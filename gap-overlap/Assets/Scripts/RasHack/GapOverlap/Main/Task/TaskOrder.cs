using System.Collections.Generic;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    public enum TaskType
    {
        Gap = 0,
        Overlap = 1,
    }

    public class TaskOrder : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private GameObject gapPrefab;
        [SerializeField] private GameObject overlapPrefab;

        [SerializeField] private List<TaskType> tasks = new List<TaskType>
        {
            TaskType.Gap, TaskType.Gap,
            TaskType.Overlap, TaskType.Overlap
        };

        #endregion

        #region Fields

        private int currentIndex;

        #endregion

        #region API

        public Task CreateNext()
        {
            if (currentIndex >= tasks.Count) return null;

            var next = tasks[currentIndex];
            currentIndex++;

            switch (next)
            {
                case TaskType.Overlap:
                    var newOverlap = Instantiate(gapPrefab, Vector3.zero, Quaternion.identity);
                    newOverlap.name = "Overlap_" + currentIndex;
                    return newOverlap.GetComponent<Gap>();
                default:
                    var newGap = Instantiate(gapPrefab, Vector3.zero, Quaternion.identity);
                    newGap.name = "Gap_" + currentIndex;
                    return newGap.GetComponent<Gap>();
            }
        }

        #endregion
    }
}