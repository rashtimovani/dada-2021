using System.Collections.Generic;
using RasHack.GapOverlap.Main.Result.Fixations.CSV;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class SingleGaze
    {
        #region Fields

        private readonly ScalableStimulus stimulus;

        private readonly MainSettings settings;

        public readonly string Classifier;

        #endregion

        #region Fields

        private float? startTime;
        private float? endTime;
        private readonly Bucket distances = new Bucket();

        private float? tentativeEndTime;
        private readonly List<float> tentativeDistances = new List<float>();

        private Fixation fixation;
        private readonly List<Fixation> allFixations = new List<Fixation>();

        #endregion

        #region Properties

        private bool IsInactive => !startTime.HasValue;
        private bool IsActive => startTime.HasValue && !endTime.HasValue;
        public bool IsDetected => startTime.HasValue && endTime.HasValue;

        public float After => startTime.Value;
        public float Duration => endTime.Value - startTime.Value;
        public Bucket Distances => distances;

        public float BestFixation
        {
            get
            {
                var bestFixation = 0f;
                foreach (var fixation in allFixations)
                {
                    if (fixation.IsDetected && fixation.Duration > bestFixation) bestFixation = fixation.Duration;
                }
                return bestFixation;
            }
        }

        #endregion

        #region Constructors

        public SingleGaze(string classifier, ScalableStimulus stimulus, MainSettings settings)
        {
            Classifier = classifier;

            this.stimulus = stimulus;
            this.settings = settings;

            fixation = new Fixation(classifier.Replace("gaze", "fixation"), stimulus, settings);
        }

        #endregion

        #region API

        public SingleGaze Update(Vector3 eyePosition, float time)
        {
            fixation = Update(fixation, eyePosition, time, allFixations);

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

        public SingleGaze ForceFinish(float time)
        {
            fixation = ForceFinish(fixation, time, allFixations);
            if (IsActive)
            {
                endTime = tentativeEndTime ?? time;
                ClearTentative();
                Debug.Log($"{Classifier} lasted {Duration.ToCSV()}s untill the end");
                return new SingleGaze(Classifier, stimulus, settings);
            }

            return this;
        }

        #endregion

        #region Helper methods
        private float DistanceInDegrees(Vector3 position)
        {
            return stimulus.DistanceBetweenInDegrees(position);
        }

        private void ClearTentative()
        {
            tentativeDistances.Clear();
            tentativeEndTime = null;
        }

        private SingleGaze OnStarted(float distance, float time)
        {
            ClearTentative();

            distances.Add(distance);
            startTime = time;
            Debug.Log($"{Classifier} after {time.ToCSV()}s");
            return this;
        }

        private SingleGaze OnContinued(float distance)
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

        private SingleGaze OnEnded(float distance, float time, float allowedCooldown)
        {
            tentativeDistances.Add(distance);
            if (!tentativeEndTime.HasValue) tentativeEndTime = time;

            var endCooldown = time - tentativeEndTime;
            if (allowedCooldown <= endCooldown)
            {
                endTime = tentativeEndTime;
                Debug.Log($"{Classifier} lasted {Duration.ToCSV()}s");
                ClearTentative();
                return new SingleGaze(Classifier, stimulus, settings);
            }

            return this;
        }

        #endregion

        #region Helpers

        private static Fixation Update(Fixation fixation, Vector3 eyePosition, float time, List<Fixation> all)
        {
            return CollectDetected(fixation, fixation.Update(eyePosition, time), all);
        }

        private static Fixation ForceFinish(Fixation fixation, float time, List<Fixation> all)
        {
            return CollectDetected(fixation, fixation.ForceFinish(time), all);
        }

        private static Fixation CollectDetected(Fixation fixation, Fixation newFixation, List<Fixation> all)
        {
            if (newFixation != fixation && fixation.IsDetected) all.Add(fixation);
            return newFixation;
        }

        #endregion
    }
}