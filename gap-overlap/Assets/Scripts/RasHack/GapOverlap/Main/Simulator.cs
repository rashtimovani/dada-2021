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

        [SerializeField] private Pointer pointer;

        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;

        [SerializeField] private MainSettings settings;

        #endregion

        #region Fields

        private Scaler scaler;
        private Scaler debugScaler;
        private Camera mainCamera;
        private TestResults results;
        private Background background;

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

        public MainSettings Settings => settings;

        public Scaler Scaler => scaler;
        public Scaler DebugScaler => debugScaler;
        public StimuliArea Area => area;

        private bool ShowPointer => settings.ShowPointer;

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
            waitingTime = tasks.HasNext ? settings.PauseBetweenTasks : settings.PauseAfterTasks;
        }

        public void UpdateBackground()
        {
            background.SetBackground(settings.Background);
        }

        public void StartTests(string usingName)
        {
            if (lastEnteredName != usingName)
            {
                lastEnteredName = usingName;
                testId = 1;
            }

            tasks.Reset(settings.TaskCount);
            area.Reset(settings.TaskCount);
            nextStimulus = StimuliTypeExtensions.Next();
            IsActive = true;

            string runName;
            if (string.IsNullOrWhiteSpace(usingName)) runName = $"Run {testId}";
            else if (testId > 1) runName = $"{usingName} - run {testId}";
            else runName = usingName;
            testId++;

            UpdateBackground();

            settings.LastUsedName = usingName;
            settings.Store();

            FlushToDisk();
            AudioListener.volume = settings.SoundVolume;

            results.StartTest(runName);
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
            pointer.ShowPointer(settings.ShowPointer);

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
            currentTask = tasks.CreateNext(nextStimulus);
            if (currentTask == null)
            {
                Debug.Log("All tasks finished!");
                IsActive = false;
                return;
            }

            currentTask.StartTask(this, nextStimulus);
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

                IsActive = false;
                waitingTime = 0.01f;
            }
        }

        #endregion
    }
}