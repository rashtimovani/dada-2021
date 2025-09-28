using System.Collections.Generic;
using RasHack.GapOverlap.Main.Result.Fixations.CSV;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class Fixation
    {
        #region Fields

        private readonly ScalableStimulus stimulus;

        private readonly MainSettings settings;

        public readonly string Classifier;

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

        public float After => startTime.Value;
        public float Duration => endTime.Value - startTime.Value;

        #endregion

        #region Constructors

        public Fixation(string classifier, ScalableStimulus stimulus, MainSettings settings)
        {
            Classifier = classifier;

            this.stimulus = stimulus;
            this.settings = settings;
        }

        #endregion

        #region API

        public Fixation Update(Vector3 eyePosition, float time)
        {
            if (stimulus == null || IsDetected) return this;

            var distanceInDegrees = DistanceInDegrees(eyePosition);
            var inRadius = stimulus.IsInRadius(eyePosition);
            if (inRadius)
            {
                if (IsInactive) return OnStarted(distanceInDegrees, time);

                return OnContinued(distanceInDegrees);
            }

            if (IsActive) return OnEnded(distanceInDegrees, time, settings.FixationEndCooldown);

            return this;
        }

        public Fixation ForceFinish(float time)
        {
            if (IsActive)
            {
                endTime = tentativeEndTime ?? time;
                ClearTentative();
                Debug.Log($"{Classifier} fixation lasted {Duration.ToCSV()}s untill the end");
                return new Fixation(Classifier, stimulus, settings);
            }

            return this;
        }

        #endregion

        #region Helper methods
        private float DistanceInDegrees(Vector3 position)
        {
            return stimulus.DistanceBetweenInDegrees(position);
        }

        private bool IsInRadius(float distanceInDegrees)
        {
            return settings.FixationAreaInDegrees >= distanceInDegrees;
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
            Debug.Log($"{Classifier} fixation after {time.ToCSV()}s");
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

        private Fixation OnEnded(float distance, float time, float allowedCooldown)
        {
            tentativeDistances.Add(distance);
            if (!tentativeEndTime.HasValue) tentativeEndTime = time;

            var endCooldown = time - tentativeEndTime;
            if (allowedCooldown <= endCooldown)
            {
                endTime = tentativeEndTime;
                Debug.Log($"{Classifier} fixation lasted {Duration.ToCSV()}s");
                ClearTentative();
                return new Fixation(Classifier, stimulus, settings);
            }

            return this;
        }

        #endregion
    }
}