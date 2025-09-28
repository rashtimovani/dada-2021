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

        private Vector3? anchor;

        #endregion

        #region Properties

        private bool IsInactive => !startTime.HasValue;
        private bool IsActive => startTime.HasValue && !endTime.HasValue;
        public bool IsDetected => startTime.HasValue && endTime.HasValue;

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

            var inRadius = false;
            if (anchor == null)
            {
                if (stimulus.IsInRadius(eyePosition)) {
                    anchor = eyePosition;
                    inRadius = true;
                }
            }
            else
            {
                var distanceInDegrees = DistanceInDegrees(eyePosition);
                inRadius = distanceInDegrees <= settings.FixationToleranceInDegrees;
            }

            if (inRadius)
            {
                if (IsInactive) return OnStarted(time);

                return this;
            }

            if (IsActive) return OnEnded(eyePosition, time);

            return this;
        }

        public Fixation ForceFinish(float time)
        {
            if (IsActive)
            {
                endTime = time;
                Debug.Log($"{Classifier} lasted {Duration.ToCSV()}s untill the end");
                return new Fixation(Classifier, stimulus, settings);
            }

            return this;
        }

        #endregion

        #region Helper methods
        private float DistanceInDegrees(Vector3 position)
        {
            return stimulus.DistanceBetweenInDegrees(position, anchor);
        }

        private Fixation OnStarted(float time)
        {
            startTime = time;
            Debug.Log($"{Classifier} after {time.ToCSV()}s");
            return this;
        }


        private Fixation OnEnded(Vector3 eyePosition, float time)
        {
            endTime = time;
            Debug.Log($"{Classifier} lasted {Duration.ToCSV()}s");
            return new Fixation(Classifier, stimulus, settings).Update(eyePosition, time);
        }

        #endregion
    }
}