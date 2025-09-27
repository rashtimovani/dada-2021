using System;
using System.Runtime.ConstrainedExecution;
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli.Animation;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class ScalableStimulus : MonoBehaviour
    {
        #region Serialized field

        [SerializeField] private Transform bottomLeft;
        [SerializeField] private Transform topRight;
        [SerializeField] private Transform radiusMarker;
        [SerializeField] protected GameObject imageToRotate;
        [SerializeField] protected DetectableCircle circle;

        #endregion

        #region Fields

        private StimuliAnimation currentAnimation;
        private StimuliAnimation rotation;

        private float fadeOut;
        private float idleDuration;

        private float sizeInDegrees;

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

        public float Radius => Vector2.Distance(transform.position, radiusMarker.position);

        public bool IsInRadius(Vector3 other)
        {
            var distance = Vector2.Distance(other, transform.position);
            return distance <= Radius;
        }

        public RawPosition RawPosition
        {
            get
            {
                var rawCenter = AsRaw(transform.position);
                return new RawPosition
                {
                    Visible = true,
                    Center = rawCenter
                };
            }
        }

        public void Scale(Vector3 desiredSize, float sizeInDegrees)
        {
            this.sizeInDegrees = sizeInDegrees;
            var size = Size;
            var desiredFixedZ = new Vector3(desiredSize.x, desiredSize.y, 1);
            var scaleChange = new Vector3(desiredFixedZ.x / size.x, desiredFixedZ.y / size.y, desiredFixedZ.z / size.z);

            var originalScale = transform.localScale;
            transform.localScale = new Vector3(originalScale.x * scaleChange.x, originalScale.y * scaleChange.y,
                originalScale.z * scaleChange.z);
        }

        public void ScaleRadius(float factor)
        {
            var currentMarker = radiusMarker.transform.localPosition;
            var radiusMarkerX = currentMarker.x * factor;
            var radiusMarkerAdjusted = new Vector3(radiusMarkerX, currentMarker.y, currentMarker.z);
            radiusMarker.transform.localPosition = radiusMarkerAdjusted;
        }

        public float DistanceBetweenInDegrees(Vector3 what)
        {
            var halfSize = sizeInDegrees / 2;
            var distanceToWhat = Vector3.Distance(transform.position, what);
            return distanceToWhat / Radius * halfSize;
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

        private RawPoint AsRaw(Vector3 inWorld)
        {
            var inScreen = Camera.main.WorldToScreenPoint(inWorld);
            var rawX = inScreen.x / Screen.width;
            var rawY = (Screen.height - inScreen.y) / Screen.height;
            return new RawPoint { X = rawX, Y = rawY };
        }

        public DetectableCircle UseDetectableCircleAndDisableArea()
        {
            circle.gameObject.SetActive(true);
            return circle;
        }

        #endregion
    }
}