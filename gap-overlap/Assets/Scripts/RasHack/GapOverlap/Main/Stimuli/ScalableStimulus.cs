using RasHack.GapOverlap.Main.Stimuli.Animation;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class ScalableStimulus : MonoBehaviour
    {
        #region Serialized field

        [SerializeField] private Transform bottomLeft;
        [SerializeField] private Transform topRight;

        #endregion

        #region Fields

        private StimuliAnimation currentAnimation;
        private StimuliAnimation rotation;

        private float fadeOut;
        private float idleDuration;

        #endregion

        #region API

        private Vector3 Size
        {
            get
            {
                var realSizeDiff = topRight.position - bottomLeft.position;
                return new Vector3(realSizeDiff.x, realSizeDiff.y, 1);
            }
        }

        public void Scale(Vector3 desiredSize)
        {
            var size = Size;
            var desiredFixedZ = new Vector3(desiredSize.x, desiredSize.y, 1);
            var scaleChange = new Vector3(desiredFixedZ.x / size.x, desiredFixedZ.y / size.y, desiredFixedZ.z / size.z);

            var originalScale = transform.localScale;
            transform.localScale = new Vector3(originalScale.x * scaleChange.x, originalScale.y * scaleChange.y,
                originalScale.z * scaleChange.z);
        }

        public virtual float ShortenAnimation(float shorterLifetime, bool keepIdling)
        {
            switch (currentAnimation)
            {
                case FadeIn:
                {
                    var remainingFadeIn = currentAnimation.ShortenAnimation(shorterLifetime);
                    if (!keepIdling)
                    {
                        idleDuration = 0;
                        fadeOut = Mathf.Min(fadeOut, Mathf.Max(0f, shorterLifetime - remainingFadeIn));
                    }

                    return remainingFadeIn + fadeOut;
                }
                case Idle:
                    if (!keepIdling)
                    {
                        fadeOut = Mathf.Min(fadeOut, shorterLifetime);
                        idleDuration = currentAnimation.ShortenAnimation(Mathf.Max(0f, shorterLifetime - fadeOut));
                    }

                    return fadeOut + idleDuration;
                case FadeOut:
                    if (!keepIdling) fadeOut = currentAnimation.ShortenAnimation(shorterLifetime);
                    return fadeOut;
            }

            return shorterLifetime;
        }

        public virtual float ShortenIdleAnimationOnly(float shorterLifetime)
        {
            switch (currentAnimation)
            {
                case FadeIn:
                {
                    idleDuration = shorterLifetime;
                    return idleDuration + fadeOut;
                }
                case Idle:
                    idleDuration = currentAnimation.ShortenAnimation(shorterLifetime);
                    return fadeOut + idleDuration;
                case FadeOut:
                    return fadeOut;
            }

            return shorterLifetime;
        }

        #endregion

        #region Helpers

        protected void DoFadeIn(float totalDuration, float fadeIn, float fadeOut, int rotationFactor)
        {
            this.fadeOut = fadeOut;
            idleDuration = totalDuration - fadeIn - fadeOut;
            StartAnimation(FadeIn.ForStimuli(gameObject, fadeIn, WaitToFadeOut));
            StartRotation(Rotate.ForStimuli(gameObject, totalDuration, rotationFactor, () => { }));
        }

        protected void DoFadeIn(float totalDuration, float fadeInOut, int rotationFactor)
        {
            DoFadeIn(totalDuration, fadeInOut, fadeInOut, rotationFactor);
        }

        private void WaitToFadeOut()
        {
            StartAnimation(Idle.ForStimuli(gameObject, idleDuration, DoFadeOut));
        }

        private void DoFadeOut()
        {
            StartAnimation(FadeOut.ForStimuli(gameObject, fadeOut));
        }

        private void StartAnimation(StimuliAnimation animation)
        {
            Destroy(currentAnimation);
            currentAnimation = animation;
        }

        private void StartRotation(StimuliAnimation animation)
        {
            Destroy(rotation);
            rotation = animation;
        }

        #endregion
    }
}