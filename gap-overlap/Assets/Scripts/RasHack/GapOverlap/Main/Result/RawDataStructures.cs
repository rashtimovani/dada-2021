using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;

namespace RasHack.GapOverlap.Main.Result
{
    [System.Serializable]
    public struct RawTestResult
    {

        public string Name;

        public string TestId;

        public RawTestTasks Tasks;
    }


    [System.Serializable]
    public struct RawTestTasks
    {
        public List<RawTaskTimes> List;
    }

    [System.Serializable]
    public struct RawTaskTimes
    {
        public float StartTime;

        public float EndTime;

        public int TaskOrder;

        public string TaskType;

        public string Side;

        public string StimulusType;
    }
}