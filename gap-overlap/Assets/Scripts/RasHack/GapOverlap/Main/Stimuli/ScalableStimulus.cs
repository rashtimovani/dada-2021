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

        private float fadeOut;
        private int rotationFactor;
        private float rotateDuration;

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

        public virtual void Scale(Vector3 desiredSize)
        {
            var size = Size;
            var desiredFixedZ = new Vector3(desiredSize.x, desiredSize.y, 1);
            var scaleChange = new Vector3(desiredFixedZ.x / size.x, desiredFixedZ.y / size.y, desiredFixedZ.z / size.z);

            var originalScale = transform.localScale;
            transform.localScale = new Vector3(originalScale.x * scaleChange.x, originalScale.y * scaleChange.y,
                originalScale.z * scaleChange.z);
        }

        #endregion

        #region Helpers
        
        protected void DoFadeIn(float totalDuration, float fadeIn, float fadeOut, int rotationFactor)
        {
            this.fadeOut = fadeOut;
            this.rotationFactor = rotationFactor;
            rotateDuration = totalDuration - fadeIn - fadeOut;
            StartAnimation(FadeIn.ForStimuli(gameObject, fadeIn, DoRotate));
        }

        protected void DoFadeIn(float totalDuration, float fadeInOut, int rotationFactor)
        {
            DoFadeIn(totalDuration, fadeInOut, fadeInOut, rotationFactor);
        }

        private void DoRotate()
        {
            StartAnimation(Rotate.ForStimuli(gameObject, rotateDuration, rotationFactor, DoFadeOut));
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

        #endregion
    }
}