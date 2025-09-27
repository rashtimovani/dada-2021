using System;
using System.Collections.Generic;
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
        private readonly List<float> distances = new List<float>();

        private float? tentativeEndTime;
        private readonly List<float> tentativeDistances = new List<float>();

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

            var distanceInDegrees = DistanceInDegrees(eyePosition);
            var inRadius = IsInRadius(distanceInDegrees);
            if (inRadius)
            {
                if (!IsActive) return OnStarted(distanceInDegrees, time);

                return OnContinued(distanceInDegrees, time);
            }

            if (IsActive) return OnEnded(distanceInDegrees, time);

            return this;
        }

        #endregion

        #region Helper methods

        private bool IsInRadius(float distanceInDegrees)
        {
            return scaler.Settings.FixationAreaInDegrees >= distanceInDegrees;
        }

        private float DistanceInDegrees(Vector3 position)
        {
            return 0f;
        }

        private Fixation OnStarted(float distance, float time)
        {
            ClearTentative();

            distances.Add(distance);
            startTime = time;
            return this;
        }

        private Fixation OnContinued(float distance, float time)
        {
            if (tentativeEndTime.HasValue)
            {
                foreach (var tentative in tentativeDistances)
                {
                    distances.Add(tentative);
                }
                ClearTentative();
            }

            distances.Add(distance);
            return this;
        }

        private Fixation OnEnded(float distance, float time)
        {
            tentativeDistances.Add(distance);
            if (!tentativeEndTime.HasValue) tentativeEndTime = time;

            var endCooldown = time - tentativeEndTime;
            if (scaler.Settings.FixationEndCooldown <= endCooldown)
            {
                endTime = tentativeEndTime;
                ClearTentative();
                return new Fixation(scaler, anchor);
            }

            return this;
        }

        private void ClearTentative()
        {
            tentativeDistances.Clear();
            tentativeEndTime = null;
        }

        #endregion
    }
}