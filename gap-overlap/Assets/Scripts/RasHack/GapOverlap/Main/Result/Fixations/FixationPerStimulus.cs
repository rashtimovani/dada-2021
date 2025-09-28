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

        private SingleGaze currentLeft;
        private readonly List<SingleGaze> allLeft = new List<SingleGaze>();

        private SingleGaze currentRight;
        private readonly List<SingleGaze> allRight = new List<SingleGaze>();

        private SingleGaze currentBoth;
        private readonly List<SingleGaze> allBoth = new List<SingleGaze>();

        #endregion

        #region Properties

        public float Duration => destroyedTime - createdTime;
        public SingleGaze Both => allBoth.Count > 0 ? allBoth[0] : currentBoth;
        public SingleGaze Left => allLeft.Count > 0 ? allLeft[0] : currentLeft;
        public SingleGaze Right => allRight.Count > 0 ? allRight[0] : currentRight;

        #endregion

        #region Constructors

        public FixationPerStimulus(string classifier, ScalableStimulus stimulus, MainSettings settings, float absoluteTime)
        {
            createdTime = absoluteTime;

            currentLeft = new SingleGaze($"{classifier} left eye gaze", stimulus, settings);
            currentRight = new SingleGaze($"{classifier} right eye gaze", stimulus, settings);
            currentBoth = new SingleGaze($"{classifier} both eyes gaze", stimulus, settings);
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

        private static SingleGaze Update(SingleGaze gaze, Vector3 eyePosition, float time, List<SingleGaze> all)
        {
            return CollectDetected(gaze, gaze.Update(eyePosition, time), all);
        }

        private static SingleGaze ForceFinish(SingleGaze gaze, float time, List<SingleGaze> all)
        {
            return CollectDetected(gaze, gaze.ForceFinish(time), all);
        }

        private static SingleGaze CollectDetected(SingleGaze gaze, SingleGaze newGaze, List<SingleGaze> all)
        {
            if (newGaze != gaze && gaze.IsDetected) all.Add(gaze);
            return newGaze;
        }

        #endregion
    }
}
