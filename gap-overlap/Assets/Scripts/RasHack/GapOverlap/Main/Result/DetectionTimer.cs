using UnityEngine;
using RasHack.GapOverlap.Main.Inputs;
using System.Collections.Generic;
using System.Globalization;
using RasHack.GapOverlap.Main.Task;
using RasHack.GapOverlap.Main.Stimuli;
using System;

namespace RasHack.GapOverlap.Main.Result
{
    public class EyeObservation
    {
        #region Properties
        private float StartTime { get; set; } = float.NaN;
        public bool Observed { get; private set; } = false;
        public float ObservedAfter { get; private set; } = float.NaN;

        public Fixation Fixation { get; private set; }

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
                Fixation = new Fixation(time);
            }
            return ObservedAfter;
        }

        public FixationResult ObservationStopped(float time)
        {
            return Fixation?.FixationEnded(time) ?? Fixation.BadResult();
        }

        public float ObservedAfterTime(float observedAfter)
        {
            if (!Observed)
            {
                ObservedAfter = observedAfter;
                Observed = true;
            }
            return ObservedAfter;
        }

        public string ToCSV()
        {
            return Observed ? ObservedAfter.ToString("0.000", CultureInfo.InvariantCulture) + "," + Fixation.ToCSV() : "NaN,NaN";
        }

        public static string ToCSVHeader(string modifier)
        {
            return $"\"{modifier} fixation after\",\"{modifier} fixation duration\"";
        }

        #endregion
    }

    public class StimulusTimer
    {
        #region Properties

        public EyeObservation LeftEye { get; private set; } = new EyeObservation();
        public EyeObservation RightEye { get; private set; } = new EyeObservation();

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
                    throw new Exception($"Unknown eye: {eye}");
            }
        }

        public FixationResult ObservationStopped(Eye eye, float time)
        {
            switch (eye)
            {
                case Eye.Left:
                    return LeftEye.ObservationStopped(time);
                case Eye.Right:
                    return RightEye.ObservationStopped(time);
                default:
                    throw new Exception($"Unknown eye: {eye}");
            }
        }

        public EyeObservation Unified
        {
            get
            {
                var unified = new EyeObservation();
                if (LeftEye.Observed && RightEye.Observed) unified.ObservedAfterTime(Math.Min(LeftEye.ObservedAfter, RightEye.ObservedAfter));
                if (!LeftEye.Observed && RightEye.Observed) unified.ObservedAfterTime(RightEye.ObservedAfter);
                if (LeftEye.Observed && !RightEye.Observed) unified.ObservedAfterTime(LeftEye.ObservedAfter);
                return unified;
            }
        }

        public string ToCSV()
        {
            return $"{LeftEye.ToCSV()},{RightEye.ToCSV()}";
        }

        public static string ToCSVHeader(string modifier)
        {
            return $"{EyeObservation.ToCSVHeader($"{modifier} by left eye")},{EyeObservation.ToCSVHeader($"{modifier} by right eye")}";
        }

        public void EndFixation(float currentTime)
        {
            ObservationStopped(Eye.Left, currentTime);
            ObservationStopped(Eye.Right, currentTime);
        }

        #endregion
    }

    public class EyeAggregator
    {
        #region Properties
        public int Total { get; private set; } = 0;
        public int Valid { get; private set; } = 0;
        public float TotalTime { get; private set; } = 0f;

        #endregion

        #region API

        public void Add(EyeObservation timer)
        {
            Total++;
            if (timer.Observed)
            {
                TotalTime += timer.ObservedAfter;
                Valid++;
            }
        }

        public string ToCSV()
        {
            var averageTime = Valid > 0 ? TotalTime / Valid : float.NaN;
            var successRate = Total > 0 && Valid > 0 ? (int)Math.Round((float)Valid / Total * 100f) : 0.0f;
            return $"{averageTime.ToString("0.000", CultureInfo.InvariantCulture)},{successRate}%";
        }

        public static string ToCSVHeader(string modifier)
        {
            return $"\"{modifier} average response time\",\"{modifier} success rate\"";
        }

        #endregion
    }

    public class Aggregator
    {
        #region Properties
        public EyeAggregator Unified { get; private set; } = new EyeAggregator();
        public EyeAggregator Left { get; private set; } = new EyeAggregator();
        public EyeAggregator Right { get; private set; } = new EyeAggregator();

        #endregion

        #region API

        public void Add(DetectionTimer timer)
        {
            if (timer.Central.Unified.Observed)
            {
                Unified.Add(timer.Peripheral.Unified);
                Left.Add(timer.Peripheral.LeftEye);
                Right.Add(timer.Peripheral.RightEye);
            }
        }

        public string ToCSV()
        {
            return $"{Unified.ToCSV()},{Left.ToCSV()},{Right.ToCSV()}";
        }

        public static string ToCSVHeader(TaskType modifier)
        {
            return $"{EyeAggregator.ToCSVHeader("Both eyes " + modifier)},{EyeAggregator.ToCSVHeader("Left eye " + modifier)},{EyeAggregator.ToCSVHeader("Right eye " + modifier)}";
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

        public DetectionTimer EndFixations(float currentTime)
        {
            Central.EndFixation(currentTime);
            Peripheral.EndFixation(currentTime);
            return this;
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
            if (Current != null) All.Add(Current.EndFixations(time));

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

        public FixationResult StopObservingCentral(Eye eye, float time)
        {
            return Current?.Central.ObservationStopped(eye, time) ?? Fixation.BadResult();
        }

        public FixationResult StopObservingPeripheral(Eye eye, float time)
        {
            return Current?.Peripheral.ObservationStopped(eye, time) ?? Fixation.BadResult();
        }

        public void ToCSV(string resultsDirectory, string subject, string testId)
        {
            if (Current != null)
            {
                All.Add(Current);
                Current = null;
            }

            var lines = new List<string>();

            var baseline = new Aggregator();
            var gap = new Aggregator();
            var overlap = new Aggregator();
            foreach (var timer in All)
            {
                lines.Add(timer.ToCSV(subject, testId));

                switch (timer.Type)
                {
                    case TaskType.Baseline:
                        baseline.Add(timer);
                        break;
                    case TaskType.Gap:
                        gap.Add(timer);
                        break;
                    case TaskType.Overlap:
                        overlap.Add(timer);
                        break;
                }
            }

            var all = CreateAllDetectionsCsv(resultsDirectory);
            var csv = string.Join("\n", lines) + "\n";
            System.IO.File.AppendAllText(all, csv);

            var single = $"{resultsDirectory}/{subject}_{testId}_detection.csv";
            System.IO.File.WriteAllText(single, DetectionTimer.ToCSVHeader() + "\n");
            System.IO.File.AppendAllText(single, csv);

            var entry = $"{subject},{testId},{baseline.ToCSV()},{gap.ToCSV()},{overlap.ToCSV()}\n";
            var aggregated = CreateAggregatedDetectionsCsv(resultsDirectory);
            System.IO.File.AppendAllText(aggregated, entry);
        }

        private string CreateAggregatedDetectionsCsv(string resultsDirectory)
        {
            var file = resultsDirectory + "/aggregated_detections.csv";
            if (!System.IO.File.Exists(file))
            {
                var header = $"\"Subject\",\"Test ID\",{Aggregator.ToCSVHeader(TaskType.Baseline)},{Aggregator.ToCSVHeader(TaskType.Gap)},{Aggregator.ToCSVHeader(TaskType.Overlap)}\n";
                System.IO.File.WriteAllText(file, header);
            }
            return file;
        }

        private string CreateAllDetectionsCsv(string resultsDirectory)
        {
            var file = resultsDirectory + "/all_detections.csv";
            if (!System.IO.File.Exists(file))
            {
                var header = DetectionTimer.ToCSVHeader() + "\n";
                System.IO.File.WriteAllText(file, header);
            }
            return file;
        }

        #endregion
    }
}