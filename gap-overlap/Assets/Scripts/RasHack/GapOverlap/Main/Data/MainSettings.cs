using System;
using System.IO;
using System.Text;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Data
{
    [Serializable]
    public class MainSettings
    {
        #region Settings related to times

        [HideInInspector] public string LastUsedName = "";

        [Header("General")] public float SoundVolume = 1f;
        public float FadeInOut = 0.1f;
        public int RotationFactor = 4;

        [Header("Pauses")] public float PauseBeforeTasks = 1f;
        public float PauseBetweenTasks = 2.7f;
        public float PauseAfterTasks = 1f;

        [Header("Screen")]
        public ReferencePoint ReferencePoint = new ReferencePoint
            {DistanceFromEyesInCM = 60f, ScreenDiagonalInInches = 15.6f};
        public BackgroundColor Background = BackgroundColor.Black;

        [Header("Stimulus dimension specification")]
        public float DistanceBetweenPeripheralStimuliInDegrees = 20f;
        public float CentralStimulusSizeInDegrees = 3.5f;
        public float PeripheralStimulusSizeInDegrees = 3f;

        [Header("Task specification")] public TaskCount TaskCount = new TaskCount
            {Gaps = 18, LeftGaps = 9, Overlaps = 18, LeftOverlaps = 9};


        [Header("Task times")] public GapTimes GapTimes = new GapTimes
            {CentralTime = 1f, PauseTime = 0.2f, StimulusTime = 1.5f};

        public OverlapTimes OverlapTimes = new OverlapTimes {CentralTime = 1.2f, BothStimuli = 1.5f};

        public BaselineTimes BaselineTimes = new BaselineTimes { CentralTime = 1f, CentralOutStimulusIn = 1f, StimulusTime = 1f };
        
        #endregion

        #region API

        public const string FILENAME = "settings.json";

        public static MainSettings Load()
        {
            if (!File.Exists(FILENAME)) return null;
            var json = File.ReadAllText(FILENAME, Encoding.UTF8);
            return JsonUtility.FromJson<MainSettings>(json);
        }

        public void Store()
        {
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(FILENAME, json, Encoding.UTF8);
        }

        #endregion
    }
}