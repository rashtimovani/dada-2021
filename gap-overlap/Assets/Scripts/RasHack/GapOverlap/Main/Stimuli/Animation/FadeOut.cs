using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli.Animation
{
    public class FadeOut : StimuliAnimation
    {
        #region Fields

        private GameObject shrink;
        private SpriteRenderer hide;

        private Vector3 initialScale;
        private float initialAlpha;

        #endregion

        #region API

        public static FadeOut ForStimuli(GameObject shrink, float duration)
        {
            var fadeOut = shrink.AddComponent<FadeOut>();

            fadeOut.totalLife = duration;

            fadeOut.shrink = shrink;
            fadeOut.initialScale = shrink.transform.localScale;

            fadeOut.hide = shrink.GetComponent<SpriteRenderer>();
            fadeOut.initialAlpha = fadeOut.hide.color.a;

            if (duration <= 0f)
            {
                shrink.transform.localScale = Vector3.zero;
                fadeOut.hide.color = fadeOut.ColorWithAlpha(0f);
            }

            return fadeOut;
        }

        #endregion

        #region Mono methods

        private void OnDestroy()
        {
            shrink.transform.localScale = Vector3.zero;
            hide.color = ColorWithAlpha(0f);
        }

        #endregion

        #region Implementation

        protected override void Animate()
        {
            var clamp = Clamp;
            shrink.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, clamp);
            hide.color = ColorWithAlpha(Mathf.Lerp(initialAlpha, 0f, clamp));
        }

        private Color ColorWithAlpha(float alpha)
        {
            var transparent = hide.color;
            transparent.a = alpha;
            return transparent;
        }

        #endregion
    }
}