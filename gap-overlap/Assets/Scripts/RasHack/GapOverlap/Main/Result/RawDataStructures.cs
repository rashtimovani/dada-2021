using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;

namespace RasHack.GapOverlap.Main.Result
{
    [System.Serializable]
    public class RawTestResult
    {

        public string Name;

        public string TestId;

        public bool TestCompleted;

        public RawTestTasks Tasks = new RawTestTasks();

        public RawStimuli CentralStimuli = new RawStimuli();

        public RawStimuli PeripheralStimuli = new RawStimuli();
    }


    [System.Serializable]
    public class RawTestTasks
    {
        public List<RawTaskTimes> List = new List<RawTaskTimes>();
    }

    [System.Serializable]
    public class RawTaskTimes
    {
        public float StartTime;

        public float EndTime;

        public int TaskOrder;

        public string TaskType;

        public string Side;

        public string StimulusType;
    }

    [System.Serializable]
    public class RawStimuli
    {
        public List<RawStimulus> List = new List<RawStimulus>();
    }

    [System.Serializable]
    public class RawStimulus
    {
        public float Time;

        public int TaskOrder;

        public RawPosition Position;
    }
}