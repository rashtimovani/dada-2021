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

        [Header("Task times")] public GapTimes GapTimes = new GapTimes
            {CentralTime = 1f, PauseTime = 0.5f, StimulusTime = 2f};

        public OverlapTimes OverlapTimes = new OverlapTimes {CentralTime = 1f, BothStimuli = 2.5f};

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
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(FILENAME, json, Encoding.UTF8);
        }

        #endregion
    }
}