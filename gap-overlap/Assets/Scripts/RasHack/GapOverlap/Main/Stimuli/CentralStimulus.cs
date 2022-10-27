using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class CentralStimulus : ScalableStimulus
    {
        #region Detection

        [SerializeField] private DetectableArea detectable;

        #endregion

        #region Internal fields

        private float? spentLifetime;

        #endregion

        #region Provided fields from simulator

        private Task.Task owner;
        private float lifetime;

        #endregion

        #region API

        public void StartSimulating(Task.Task owner, float lifetime)
        {
            this.owner = owner;
            spentLifetime = 0;
            this.lifetime = lifetime;

            detectable.RegisterOnDetect(owner.Owner, OnCentralPointerDetection);
            DoFadeIn(lifetime, owner.FadeInOut, owner.RotationFactor);
        }

        public void StartSimulating(Task.Task owner, float lifetime, float fadeOut)
        {
            this.owner = owner;
            spentLifetime = 0;
            this.lifetime = lifetime;

            detectable.RegisterOnDetect(owner.Owner, OnCentralPointerDetection);
            DoFadeIn(lifetime, owner.FadeInOut, fadeOut, owner.RotationFactor);
        }

        public override float ShortenAnimation(float shorterLifetime, bool keepIdling)
        {
            var newLifetime = base.ShortenAnimation(shorterLifetime, keepIdling);
            lifetime = newLifetime;
            return lifetime;
        }

        public override float ShortenIdleAnimationOnly(float shorterLifetime)
        {
            var newLifetime = base.ShortenIdleAnimationOnly(shorterLifetime);
            lifetime = newLifetime;
            return lifetime;
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            GetComponent<AudioSource>().Play();
        }

        private void OnCentralPointerDetection(Pointer pointer)
        {
            owner.ReportFocusedOnCentral(this, spentLifetime.GetValueOrDefault(lifetime));
        }

        private void Update()
        {
            if (!spentLifetime.HasValue) return;

            spentLifetime += Time.deltaTime;
            if (spentLifetime < lifetime) return;

            spentLifetime = null;
            owner.ReportCentralStimulusDied(this);
        }

        #endregion
    }
}