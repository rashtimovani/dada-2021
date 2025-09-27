using System;
using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class Fixation
    {
        #region Fields

        private readonly Scaler scaler;

        private readonly Vector3 anchor;

        #endregion

        #region Fields

        private float? startTime;

        private float? endTime;

        #endregion

        #region Properties

        public bool IsActive => startTime.HasValue && !endTime.HasValue;
        public bool IsDetected => startTime.HasValue && endTime.HasValue;

        #endregion

        #region Constructors

        public Fixation(Scaler scaler, Vector3 anchor)
        {
            this.scaler = scaler;
            this.anchor = anchor;
        }

        #endregion

        #region API

        public Fixation Update(Vector3 eyePosition, float time)
        {
            if (IsDetected) throw new Exception("Fixation is already detected, use new one for the update");

            var inRadius = IsInRadius(eyePosition);
            if (inRadius)
            {
                if (!IsActive) return OnStarted(time);

                return OnContinued(time);
            }

            if (IsActive) return OnEnded(time);

            return this;
        }

        #endregion

        #region Helper methods

        private bool IsInRadius(Vector3 position)
        {
            return true;
        }

        private Fixation OnStarted(float time)
        {
            startTime = time;
            return this;
        }

        private Fixation OnContinued(float time)
        {
            return this;
        }

        private Fixation OnEnded(float time)
        {
            return this;
        }

        #endregion
    }
}