using System.Collections.Generic;
using System.Numerics;

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

        #region Constructors

        public FixationPerStimulus(Scaler scaler, Vector3 anchor, float time)
        {
            createdTime = time;

            currentLeft = new Fixation(scaler, anchor);
            currentRight = new Fixation(scaler, anchor);
            currentBoth = new Fixation(scaler, anchor);
        }

        #endregion

        #region API

        public void Update(Vector3 leftEyePosition, Vector3 rightEyePosition, float time)
        {
            currentLeft = Update(currentLeft, leftEyePosition, time, allLeft);
            currentBoth = Update(currentBoth, leftEyePosition, time, allBoth);

            currentRight = Update(currentRight, rightEyePosition, time, allRight);
            currentBoth = Update(currentBoth, rightEyePosition, time, allRight);
        }

        public void StimulusDestroyed(float time)
        {
            destroyedTime = time;

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
