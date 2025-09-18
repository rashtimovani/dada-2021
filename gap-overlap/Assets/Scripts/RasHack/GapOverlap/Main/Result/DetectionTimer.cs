using UnityEngine;
using RasHack.GapOverlap.Main.Inputs;
using System.Collections.Generic;

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
            ObservedAfter = time - StartTime;
            Observed = true;
            return ObservedAfter;
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

        #endregion
    }

    public class DetectionTimer
    {
        #region Properties

        public StimulusTimer Central { get; private set; } = new StimulusTimer();
        public StimulusTimer Peripheral { get; private set; } = new StimulusTimer();

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

        #endregion
    }

    public class Timers
    {
        #region Properties

        public List<DetectionTimer> All { get; private set; } = new List<DetectionTimer>();
        public DetectionTimer Current { get; private set; } = null;

        #endregion

        #region API

        public void StartNewCentral(float time)
        {
            if (Current != null) All.Add(Current);

            Current = new DetectionTimer();
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

        #endregion
    }
}