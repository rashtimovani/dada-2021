using System.Collections.Generic;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class FixationPerStimulus
    {
        #region Fields
        
        private readonly float createdTime;
        private float destroyedTime;

        private Fixation currentLeft;
        private readonly List<Fixation> allLeft = new List<Fixation>();

        private Fixation currentRight;
        private readonly List<Fixation> allRight = new List<Fixation>();

        private Fixation currentBoth;
        private readonly List<Fixation> allBoth = new List<Fixation>();

        #endregion

        #region Properties

        public float Duration => destroyedTime - createdTime;
        public Fixation Both => allBoth.Count > 0 ? allBoth[0] : currentBoth;
        public Fixation Left => allLeft.Count > 0 ? allLeft[0] : currentLeft;
        public Fixation Right => allRight.Count > 0 ? allRight[0] : currentRight;

        #endregion

        #region Constructors

        public FixationPerStimulus(string classifier, ScalableStimulus stimulus, MainSettings settings, float absoluteTime)
        {
            createdTime = absoluteTime;

            currentLeft = new Fixation($"{classifier} left eye", stimulus, settings);
            currentRight = new Fixation($"{classifier} right eye", stimulus, settings);
            currentBoth = new Fixation($"{classifier} both eyes", stimulus, settings);
        }

        #endregion

        #region API

        public void Update(Vector3 leftEyePosition, Vector3 rightEyePosition, float absoluteTime)
        {
            var time = absoluteTime - createdTime;

            currentLeft = Update(currentLeft, leftEyePosition, time, allLeft);
            currentBoth = Update(currentBoth, leftEyePosition, time, allBoth);

            currentRight = Update(currentRight, rightEyePosition, time, allRight);
            currentBoth = Update(currentBoth, rightEyePosition, time, allRight);
        }

        public void StimulusDestroyed(float absoluteTime)
        {
            destroyedTime = absoluteTime;
            var time = absoluteTime - createdTime;

            currentLeft = ForceFinish(currentLeft, time, allLeft);
            currentBoth = ForceFinish(currentBoth, time, allBoth);

            currentRight = ForceFinish(currentRight, time, allRight);
            currentBoth = ForceFinish(currentBoth, time, allRight);
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
