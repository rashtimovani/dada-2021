using System;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Data
{
    [Serializable]
    public class MainSettings
    {
        #region Settings related to times

        [Header("Task times")]
        public GapTimes GapTimes = new GapTimes {CentralTime = 1f, PauseTime = 0.5f, StimulusTime = 2f};

        public OverlapTimes OverlapTimes = new OverlapTimes {CentralTime = 1f, BothStimuli = 2.5f};

        #endregion
    }
}