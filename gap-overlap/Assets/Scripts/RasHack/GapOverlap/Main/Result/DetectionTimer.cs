using UnityEngine;
using RasHack.GapOverlap.Main.Inputs;
using System.Collections.Generic;
using System.Globalization;
using RasHack.GapOverlap.Main.Task;
using RasHack.GapOverlap.Main.Stimuli;

namespace RasHack.GapOverlap.Main.Result
{
    public class EyeObservation
    {
        #region Properties
        private float StartTime { get; set; } = float.NaN;
        public bool Observed { get; private set; } = false;
        public float ObservedAfter { get; private set; } = float.NaN;

        #endregion

        #region API

        public void Start(float time)
        {
            StartTime = time;
            Observed = false;
        }

        public float ObservedAt(float time)
        {
            if (!Observed)
            {
                ObservedAfter = time - StartTime;
                Observed = true;
            }
            return ObservedAfter;
        }

        public string ToCSV()
        {
            return Observed ? ObservedAfter.ToString("0.00", CultureInfo.InvariantCulture) : "NaN";
        }

        #endregion
    }

    public class StimulusTimer
    {
        #region Properties

        public EyeObservation LeftEye { get; private set; } = new EyeObservation();
        public EyeObservation RightEye { get; private set; } = new EyeObservation();

        public bool Observed => LeftEye.Observed || RightEye.Observed;

        #endregion

        #region API

        public void Start(float startTime)
        {
            LeftEye.Start(startTime);
            RightEye.Start(startTime);
        }

        public float ObservedAt(Eye eye, float time)
        {
            switch (eye)
            {
                case Eye.Left:
                    return LeftEye.ObservedAt(time);
                case Eye.Right:
                    return RightEye.ObservedAt(time);
                default:
                    throw new System.Exception($"Unknown eye: {eye}");
            }
        }

        public string ToCSV()
        {
            return $"{LeftEye.ToCSV()},{RightEye.ToCSV()}";
        }

        public static string ToCSVHeader(string modifier)
        {
            return $"\"{modifier} stimulus detection by left eye\",\"{modifier} stimulus detection by right eye\"";
        }

        #endregion
    }

    public class DetectionTimer
    {
        #region Properties

        public StimulusTimer Central { get; private set; } = new StimulusTimer();
        public StimulusTimer Peripheral { get; private set; } = new StimulusTimer();
        public int Order { get; private set; }
        public TaskType Type { get; private set; }
        public StimulusSide Side { get; private set; }

        #endregion

        #region Constructor
        public DetectionTimer(int order, TaskType type, StimulusSide side)
        {
            Type = type;
            Order = order;
            Side = side;
        }

        #endregion

        #region API

        public void StartCentral(float currentTime)
        {
            Central.Start(currentTime);
        }

        public void StartPeripheral(float currentTime)
        {
            Peripheral.Start(currentTime);
        }

        public string ToCSV(string subject, string testId)
        {
            return $"\"{subject}\",\"{testId}\",\"{Order}\",\"{Type}\",\"{Side}\",{Central.ToCSV()},{Peripheral.ToCSV()}";
        }

        public static string ToCSVHeader()
        {
            return $"\"Subject\",\"Test ID\",\"Task Order\",\"Task Type\",\"Task Side\",{StimulusTimer.ToCSVHeader("Central")},{StimulusTimer.ToCSVHeader("Peripheral")}";
        }

        #endregion
    }

    public class Timers
    {
        #region Properties

        public List<DetectionTimer> All { get; private set; } = new List<DetectionTimer>();
        public DetectionTimer Current { get; private set; } = null;

        #endregion

        #region API

        public void StartNewCentral(float time, int order, TaskType type, StimulusSide side)
        {
            if (Current != null) All.Add(Current);

            Current = new DetectionTimer(order, type, side);
            Current.StartCentral(time);
        }

        public void StartPeripheral(float time)
        {
            Current.StartPeripheral(time);
        }

        public float ObserveCentral(Eye eye, float time)
        {
            return Current.Central.ObservedAt(eye, time);
        }

        public float ObservePeripheral(Eye eye, float time)
        {
            return Current.Peripheral.ObservedAt(eye, time);
        }

        public void ToCSV(string resultsDirectory, string subject, string testId)
        {
            if (Current != null)
            {
                All.Add(Current);
                Current = null;
            }

            var lines = new List<string>
            {
                DetectionTimer.ToCSVHeader()
            };
            foreach (var timer in All)
            {
                lines.Add(timer.ToCSV(subject, testId));
            }

            var csv = string.Join("\n", lines) + "\n";
            System.IO.File.WriteAllText($"{resultsDirectory}/{subject}_{testId}_detection.csv", csv);
        }

        #endregion
    }
}