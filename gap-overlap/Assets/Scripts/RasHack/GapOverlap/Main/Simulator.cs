using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class Simulator : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private Pointer pointer;

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
        private TaskOrder tasks;
        private StimuliArea area;

        private float? waitingTime;
        private Task.Task currentTask;
        private StimuliType nextStimulus;

        #endregion

        #region API

        public Scaler Scaler => scaler;
        public Scaler DebugScaler => debugScaler;
        public StimuliArea Area => area;

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

            scaler = new Scaler(mainCamera, -1);
            debugScaler = new Scaler(mainCamera, -2);

            tasks = GetComponent<TaskOrder>();
            area = GetComponent<StimuliArea>();

            newTask();
        }

        private void Update()
        {
            UpdateBounds();
            UpdateDebugVisibility();
            UpdatePause();
        }

        #endregion

        #region Helpers

        private void UpdateBounds()
        {
            topLeft.transform.position = debugScaler.TopLeft;
            bottomLeft.transform.position = debugScaler.BottomLeft;
            bottomRight.transform.position = debugScaler.BottomRight;
            topRight.transform.position = debugScaler.TopRight;
        }

        private void UpdateDebugVisibility()
        {
            pointer.ShowPointer(showPointer);

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
            currentTask = tasks.CreateNext();
            if (currentTask == null)
            {
                Debug.Log("All tasks finished!");
                return;
            }

            currentTask.StartTask(this, nextStimulus);
            nextStimulus = nextStimulus.next();
        }

        #endregion
    }
}