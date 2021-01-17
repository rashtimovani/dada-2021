using System;
using System.Net.NetworkInformation;
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
        private TestResults results;

        private TaskOrder tasks;
        private StimuliArea area;
        private StimuliType nextStimulus;

        private int testId;
        private float? waitingTime;
        private Task.Task currentTask;
        private string lastEnteredName;

        #endregion

        #region API

        public bool IsActive { get; private set; }

        public Scaler Scaler => scaler;
        public Scaler DebugScaler => debugScaler;
        public StimuliArea Area => area;

        public void ReportTaskFinished(Task.Task task, float? measurement)
        {
            if (task != currentTask)
            {
                Debug.LogError($"{task} reported as finished, but that ${currentTask} is currently active");
                return;
            }

            results.AttachMeasurement(task.name, measurement);
            Debug.Log($"{currentTask} has finished");
            currentTask = null;
            waitingTime = pauseBetweenTasks;
        }

        public void StartTests(string name)
        {
            if (lastEnteredName != name)
            {
                lastEnteredName = name;
                testId = 1;
            }

            tasks.Reset();
            area.Reset();
            nextStimulus = StimuliType.Bee;
            IsActive = true;

            string runName;
            if (string.IsNullOrWhiteSpace(name)) runName = $"Run {testId}";
            else if (testId > 1) runName = $"{name} - run {testId}";
            else runName = name;
            testId++;

            results.StartTest(runName);
            newTask();
        }

        public void FlushToDisk()
        {
            results.FlushToDisk();
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            Application.targetFrameRate = 120;
            mainCamera = Camera.main;

            scaler = new Scaler(mainCamera, -1);
            debugScaler = new Scaler(mainCamera, -2);
            results = new TestResults();

            tasks = GetComponent<TaskOrder>();
            area = GetComponent<StimuliArea>();
        }

        private void Update()
        {
            UpdateBounds();
            UpdateDebugVisibility();
            UpdatePause();
        }

        private void OnApplicationQuit()
        {
            FlushToDisk();
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
                IsActive = false;
                return;
            }

            currentTask.StartTask(this, nextStimulus);
            nextStimulus = nextStimulus.next();
        }

        #endregion
    }
}