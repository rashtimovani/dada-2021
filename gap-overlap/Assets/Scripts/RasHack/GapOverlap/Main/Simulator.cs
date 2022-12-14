using System;
using System.Collections.Generic;
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Result;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class Simulator : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private Pointer[] pointers;

        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;

        [SerializeField] private RawDataCollector collector;

        #endregion

        #region Fields

        private MainSettings settings = new MainSettings();

        private Scaler scaler;
        private Scaler debugScaler;
        private Camera mainCamera;
        private TestResults results;
        private Background background;

        private TaskOrder tasks;
        private StimuliArea area;
        private StimuliType nextStimulus;

        private float? waitingTime;
        private Task.Task currentTask;

        #endregion

        #region API

        public bool IsActive { get; private set; }

        public List<TobiiEyePointer> TobiiPointers
        {
            get
            {
                var tobiiPointers = new List<TobiiEyePointer>();
                foreach (var pointer in pointers)
                {
                    var tobiiPointer = pointer as TobiiEyePointer;
                    if (tobiiPointer != null) tobiiPointers.Add(tobiiPointer);
                }

                return tobiiPointers;
            }
        }

        private List<Pointer> NonTobiiPointers
        {
            get
            {
                var nonTobiiPointers = new List<Pointer>();
                foreach (var pointer in pointers)
                {
                    var tobiiPointer = pointer as TobiiEyePointer;
                    if (pointer != null && tobiiPointer == null) nonTobiiPointers.Add(pointer);
                }

                return nonTobiiPointers;
            }
        }

        public MainSettings Settings => settings;

        public Scaler Scaler => scaler;
        public Scaler DebugScaler => debugScaler;
        public StimuliArea Area => area;

        private bool ShowPointer => settings.ShowPointer;

        public void ReportTaskFinished(Task.Task task, AllResponseTimes responses)
        {
            if (task != currentTask)
            {
                Debug.LogError($"{task} reported as finished, but that ${currentTask} is currently active");
                return;
            }


            results.AttachMeasurement(task.TaskType, task.StimulusType, task.Side, responses);
            Debug.Log($"{currentTask} has finished");
            collector.TaskCompleted(currentTask);
            currentTask = null;
            waitingTime = tasks.HasNext ? settings.PauseBetweenTasks : settings.PauseAfterTasks;
        }

        public void UpdateBackground()
        {
            background.SetBackground(settings.Background);
        }

        public void StartTests(string usingName)
        {
            tasks.Reset(settings.TaskCount);
            area.Reset(settings.TaskCount);
            nextStimulus = StimuliTypeExtensions.Next();
            IsActive = true;

            var runName = string.IsNullOrWhiteSpace(usingName) ? $"{Guid.NewGuid()}" : usingName;

            UpdateBackground();

            settings.LastUsedName = usingName;
            settings.Store();

            FlushToDisk();
            AudioListener.volume = settings.SoundVolume;

            var testId = Guid.NewGuid().ToString();
            results.StartTest(runName, testId);
            collector.TasksStarted(runName, testId, settings.SamplesPerSecond);
            waitingTime = settings.PauseBeforeTasks;
        }

        public void FlushToDisk()
        {
            results.FlushToDisk();
        }

        #endregion

        #region Mono methods

        private void Awake()
        {
            var loadedSettings = MainSettings.Load();
            if (loadedSettings != null) settings = loadedSettings;
        }

        private void Start()
        {
            Application.targetFrameRate = 120;
            mainCamera = Camera.main;

            background = FindObjectOfType<Background>();
            UpdateBackground();

            scaler = new Scaler(mainCamera, -1, settings);
            debugScaler = new Scaler(mainCamera, -2, settings);
            results = new TestResults();

            tasks = GetComponent<TaskOrder>();
            area = GetComponent<StimuliArea>();
        }

        private void Update()
        {
            UpdateBounds();
            UpdateDebugVisibility();
            UpdatePause();
            DetectInterruptedTest();
            UpdatePointers();
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
            foreach (var pointer in pointers)
            {
                pointer.ShowPointer(settings.ShowPointer);
            }

            bottomLeft.enabled = ShowPointer;
            bottomRight.enabled = ShowPointer;
            topLeft.enabled = ShowPointer;
            topRight.enabled = ShowPointer;
        }

        private void UpdatePause()
        {
            if (!waitingTime.HasValue) return;
            waitingTime -= Time.deltaTime;
            if (waitingTime > 0f) return;
            waitingTime = null;
            NewTask();
        }

        private void NewTask()
        {
            var taskOrder = tasks.CurrentTaskOrder;
            currentTask = tasks.CreateNext(nextStimulus);
            if (currentTask == null)
            {
                results.EndActiveTest();
                collector.TasksCompleted();
                Debug.Log("All tasks finished!");
                IsActive = false;
                return;
            }

            currentTask.StartTask(this, nextStimulus, taskOrder);
            collector.TaskStarted(currentTask);
            nextStimulus = StimuliTypeExtensions.Next();
        }

        private void DetectInterruptedTest()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.LogWarning("Test aborted by user!");
                tasks.End();
                if (currentTask != null)
                {
                    Destroy(currentTask.gameObject);
                    currentTask = null;
                }

                results.AbortActiveTest();
                collector.TasksCompleted();
                IsActive = false;
                waitingTime = 0.01f;
            }
        }

        private void UpdatePointers()
        {
            var tobiiWorking = true;
            foreach (var tobiiPointer in TobiiPointers)
            {
                tobiiWorking &= tobiiPointer.Status.Enabled;
            }

            foreach (var pointer in NonTobiiPointers)
            {
                pointer.PointerEnabled = !tobiiWorking;
            }
        }

        #endregion
    }
}