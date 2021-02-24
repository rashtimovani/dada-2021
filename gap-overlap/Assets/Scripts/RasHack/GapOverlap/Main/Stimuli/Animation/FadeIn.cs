using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli.Animaition
{
    public class FadeIn : StimuliAnimation
    {
        #region Fields

        private GameObject grow;
        private SpriteRenderer show;

        private Vector3 initialScale;
        private float initialAlpha;

        #endregion

        #region API

        public static FadeIn ForStimuli(GameObject grow, float duration)
        {
            var fadeIn = grow.AddComponent<FadeIn>();

            fadeIn.totalLife = duration;
            
            fadeIn.grow = grow;
            fadeIn.initialScale = grow.transform.localScale;
            grow.transform.localScale = Vector3.zero;
            
            fadeIn.show = grow.GetComponent<SpriteRenderer>();
            fadeIn.initialAlpha = fadeIn.show.color.a;
            fadeIn.show.color = fadeIn.ColorWithAlpha(0f);

            return fadeIn;
        }

        #endregion

        #region Mono methods

        private void OnDestroy()
        {
            grow.transform.localScale = initialScale;
            show.color = ColorWithAlpha(initialAlpha);
        }

        #endregion

        #region Implementation
        
        protected override void Animate()
        {
            var clamp = Clamp;
            show.transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, clamp);
            show.color = ColorWithAlpha(Mathf.Lerp(0f, initialAlpha, clamp));
        }

        private Color ColorWithAlpha(float alpha)
        {
            var transparent = show.color;
            transparent.a = alpha;
            return transparent;
        }

        #endregion
    }
}