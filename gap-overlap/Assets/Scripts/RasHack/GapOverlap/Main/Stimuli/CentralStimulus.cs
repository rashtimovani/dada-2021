using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class CentralStimulus : ScalableStimulus
    {
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
            detectable.AreaScreenSide = StimulusSide.Center;
            this.owner = owner;
            this.lifetime = lifetime;
            spentLifetime = 0;

            detectable.RegisterOnDetect(owner.Owner, OnCentralPointerDetection, OnCentralPointerGettingCloser);
            DoFadeIn(lifetime, owner.FadeInOut, owner.RotationFactor);
        }

        public void StartSimulating(Task.Task owner, float lifetime, float fadeOut)
        {
            detectable.AreaScreenSide = StimulusSide.Center;
            this.owner = owner;
            this.lifetime = lifetime;
            spentLifetime = 0;

            detectable.RegisterOnDetect(owner.Owner, OnCentralPointerDetection, OnCentralPointerGettingCloser);
            DoFadeIn(lifetime, owner.FadeInOut, fadeOut, owner.RotationFactor);
        }

        public override float ShortenAnimation(float shorterLifetime, bool keepIdling)
        {
            if (!spentLifetime.HasValue) return 0f;
            var remaining = base.ShortenAnimation(shorterLifetime, keepIdling);
            lifetime = spentLifetime.Value + remaining;
            return remaining;
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
            owner.ReportFocusedOnCentral(this, pointer.Eye, spentLifetime.GetValueOrDefault(lifetime));
        }
        
        private void OnCentralPointerGettingCloser(Pointer pointer)
        {
            owner.ReportCentralGotCloser(this, pointer.Eye, pointer);
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