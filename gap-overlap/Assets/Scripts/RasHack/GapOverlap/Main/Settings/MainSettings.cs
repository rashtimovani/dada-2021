using System;
using System.IO;
using System.Text;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Settings
{
    [Serializable]
    public class MainSettings
    {
        #region Settings related to times

        [HideInInspector] public string LastUsedName = "";

        [Header("General")] public float SoundVolume = 1f;
        public float FadeInOut = 0.1f;
        public int RotationFactor = 4;

        [Header("Debug")] public bool ShowPointer;

        [Header("Pauses")] public float PauseBeforeTasks = 1f;
        public float PauseBetweenTasks = 2.7f;
        public float PauseAfterTasks = 1f;

        [Header("Screen")] public ReferencePoint ReferencePoint = new()
            { DistanceFromEyesInCM = 60f, ScreenDiagonalInInches = 15.6f };

        public BackgroundColor Background = BackgroundColor.Black;

        [Header("Stimulus dimension specification")]
        public float DistanceBetweenPeripheralStimuliInDegrees = 20f;

        public float CentralStimulusSizeInDegrees = 3.5f;
        public float PeripheralStimulusSizeInDegrees = 3f;

        [Header("Task specification")] public TaskCount TaskCount = new()
            { Gaps = 18, LeftGaps = 9, Overlaps = 18, LeftOverlaps = 9, Baselines = 18, LeftBaselines = 9 };

        [Header("Task times")] public GapTimes GapTimes = new()
            { CentralTime = 5.0f, PauseTime = 0.0f, StimulusTime = 5.0f, ShortenOnFocusTime = 0.2f };

        public OverlapTimes OverlapTimes = new() { CentralTime = 5.0f, BothStimuli = 5.0f, ShortenOnFocusTime = 0.2f };

        public BaselineTimes BaselineTimes = new()
            { CentralTime = 5.0f, CentralOutStimulusIn = 5.0f, StimulusTime = 5.0f, ShortenOnFocusTime = 0.2f };

        [Header("Sound")]
        public bool SoundEnabled;

        [Header("Results")]
        public float SamplesPerSecond = 25f;

        #endregion

        #region API

        public static readonly string DIRECTORY =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}GapOverlap";

        public static readonly string FILENAME = $"{DIRECTORY}{Path.DirectorySeparatorChar}settings.json";

        public static MainSettings Load()
        {
            if (!File.Exists(FILENAME)) return null;
            var json = File.ReadAllText(FILENAME, Encoding.UTF8);
            return JsonUtility.FromJson<MainSettings>(json);
        }

        public void Store()
        {
            var json = JsonUtility.ToJson(this, true);
            Directory.CreateDirectory(DIRECTORY);
            File.WriteAllText(FILENAME, json, Encoding.UTF8);
        }

        #endregion
    }
}