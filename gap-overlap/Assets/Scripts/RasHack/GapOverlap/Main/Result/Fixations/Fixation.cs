using System.Collections.Generic;
using UnityEngine;

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

        private bool IsInactive => !startTime.HasValue;
        private bool IsActive => startTime.HasValue && !endTime.HasValue;
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
            if (IsDetected) return this;

            var distanceInDegrees = DistanceInDegrees(eyePosition);
            var inRadius = IsInRadius(distanceInDegrees);
            if (inRadius)
            {
                if (IsInactive) return OnStarted(distanceInDegrees, time);

                return OnContinued(distanceInDegrees);
            }

            if (IsActive) return OnEnded(distanceInDegrees, time, scaler.Settings.FixationEndCooldown);

            return this;
        }

        public Fixation ForceFinish(float time)
        {
            if (IsActive)
            {
                endTime = tentativeEndTime ?? time;
                ClearTentative();
                return new Fixation(scaler, anchor);
            }

            return this;
        }

        #endregion

        #region Helper methods
        private float DistanceInDegrees(Vector3 position)
        {
            // TODO: calcuate distance in degrees
            return 0f;
        }

        private bool IsInRadius(float distanceInDegrees)
        {
            return scaler.Settings.FixationAreaInDegrees >= distanceInDegrees;
        }

        private void ClearTentative()
        {
            tentativeDistances.Clear();
            tentativeEndTime = null;
        }

        private Fixation OnStarted(float distance, float time)
        {
            ClearTentative();

            distances.Add(distance);
            startTime = time;
            return this;
        }

        private Fixation OnContinued(float distance)
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

        private Fixation OnEnded(float distance, float time, float cooldown)
        {
            tentativeDistances.Add(distance);
            if (!tentativeEndTime.HasValue) tentativeEndTime = time;

            var endCooldown = time - tentativeEndTime;
            if (cooldown <= endCooldown)
            {
                endTime = tentativeEndTime;
                ClearTentative();
                return new Fixation(scaler, anchor);
            }

            return this;
        }

        #endregion
    }
}