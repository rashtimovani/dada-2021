using RasHack.GapOverlap.Main.Stimuli.Animation;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class ScalableStimulus : MonoBehaviour
    {
        #region Serialized field

        [SerializeField] private Transform bottomLeft;
        [SerializeField] private Transform topRight;
        [SerializeField] protected DetectableArea detectable;
        [SerializeField] protected GameObject imageToRotate;

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


        public RawPosition RawPosition
        {
            get
            {
                var rawCenter = AsRaw(transform.position);
                // var rawBottomLeft = AsRaw(bottomLeft.position);
                // var rawBottomRight = AsRaw(new Vector2(topRight.position.x, bottomLeft.position.y));
                // var rawTopLeft = AsRaw(new Vector2(bottomLeft.position.x, topRight.position.y));
                // var rawTopRight = AsRaw(topRight.position);
                return new RawPosition
                {
                    Center = rawCenter,
                    // BottomLeft = rawBottomLeft,
                    // BottomRight = rawBottomRight,
                    // TopLeft = rawTopLeft,
                    // TopRight = rawTopRight
                };
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
                        var halfShorterTime = keepIdling ? shorterLifetime : shorterLifetime / 2.0f;
                        var remainingFadeIn = currentAnimation.ShortenAnimation(halfShorterTime);
                        if (!keepIdling)
                        {
                            idleDuration = 0;
                            fadeOut = halfShorterTime;
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
            StartAnimation(FadeIn.ForStimuli(imageToRotate, fadeIn, WaitToFadeOut));
            StartRotation(Rotate.ForStimuli(imageToRotate, totalDuration, rotationFactor, () => { }));
        }

        protected void DoFadeIn(float totalDuration, float fadeInOut, int rotationFactor)
        {
            DoFadeIn(totalDuration, fadeInOut, fadeInOut, rotationFactor);
        }

        private void WaitToFadeOut()
        {
            StartAnimation(Idle.ForStimuli(imageToRotate, idleDuration, DoFadeOut));
        }

        private void DoFadeOut()
        {
            StartAnimation(FadeOut.ForStimuli(imageToRotate, fadeOut));
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

        private Vector2 AsRaw(Vector3 inWorld)
        {
            var inScreen = Camera.main.WorldToScreenPoint(inWorld);
            var rawX = inScreen.x / Screen.width;
            var rawY = (Screen.height - inScreen.y) / Screen.height;
            return new Vector2(rawX, rawY);
        }

        #endregion
    }
}