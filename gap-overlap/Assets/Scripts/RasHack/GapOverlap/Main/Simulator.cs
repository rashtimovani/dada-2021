using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class Simulator : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private GameObject gapPrefab;
        [SerializeField] private GameObject overlapPrefab;

        [SerializeField] private SpriteRenderer pointer;
        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;

        [SerializeField] private bool showPointer;
        [SerializeField] private float pauseBetweenTasks = 3.5f;

        #endregion

        #region Fields

        private Scaler scaler;
        private Scaler debugScaler;
        private Camera mainCamera;

        private int taskId = 1;
        private float? waitingTime;
        private Task.Task currentTask;
        private StimuliType nextStimulus;

        #endregion

        #region API

        public Scaler Scaler => scaler;

        public void ReportTaskFinished(Task.Task task)
        {
            if (task != currentTask)
            {
                Debug.LogError($"{task} reported as finished, but that ${currentTask} is currently active");
                return;
            }

            Debug.Log($"{currentTask} has finished");
            currentTask = null;
            waitingTime = pauseBetweenTasks;
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            mainCamera = Camera.main;

            scaler = new Scaler(mainCamera, -2);
            debugScaler = new Scaler(mainCamera, -1);
            
            newTask();
        }

        private void Update()
        {
            UpdateBounds();
            UpdateDebugVisibility();
            UpdatePause();

            pointer.transform.position = scaler.point(Input.mousePosition);
        }

        #endregion

        #region Helpers

        private void UpdateBounds()
        {
            topLeft.transform.position = transform.InverseTransformPoint(debugScaler.TopLeft);
            bottomLeft.transform.position = transform.InverseTransformPoint(debugScaler.BottomLeft);
            bottomRight.transform.position = transform.InverseTransformPoint(debugScaler.BottomRight);
            topRight.transform.position = transform.InverseTransformPoint(debugScaler.TopRight);
        }

        private void UpdateDebugVisibility()
        {
            pointer.enabled = showPointer;

            bottomLeft.enabled = showPointer;
            bottomRight.enabled = showPointer;
            topLeft.enabled = showPointer;
            topRight.enabled = showPointer;
        }

        private void UpdatePause()
        {
            if (!waitingTime.HasValue) return;
            waitingTime -= Time.deltaTime;
            if (waitingTime > 0f) return;
            waitingTime = null;
            newTask();
        }

        private void newTask()
        {
            var newOne = Instantiate(gapPrefab, Vector3.zero, Quaternion.identity);
            newOne.name = "Gap_" + taskId;
            taskId++;
            currentTask = newOne.GetComponent<Gap>();
            
            currentTask.StartTask(this, nextStimulus);
            nextStimulus = nextStimulus.next();
        }

        #endregion
    }
}