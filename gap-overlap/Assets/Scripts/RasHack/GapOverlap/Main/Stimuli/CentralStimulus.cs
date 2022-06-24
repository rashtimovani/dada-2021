using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class CentralStimulus : ScalableStimulus
    {
        #region Detection

        [SerializeField] private DetectableArea detectable;

        #endregion

        #region Provided fields from simulator

        private Task.Task owner;
        private float? lifetime;

        #endregion

        #region API

        public void StartSimulating(Task.Task owner, float lifetime)
        {
            detectable.RegisterOnDetect(owner.Owner, OnCentralPointerDetection);

            this.owner = owner;
            this.lifetime = lifetime;

            DoFadeIn(lifetime, owner.FadeInOut, owner.RotationFactor);
        }

        public void StartSimulating(Task.Task owner, float lifetime, float fadeOut)
        {
            detectable.RegisterOnDetect(owner.Owner, OnCentralPointerDetection);

            this.owner = owner;
            this.lifetime = lifetime;

            DoFadeIn(lifetime, owner.FadeInOut, fadeOut, owner.RotationFactor);
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            GetComponent<AudioSource>().Play();
        }

        private void OnCentralPointerDetection(Pointer pointer)
        {
            owner.ReportFocusedOnCentral(this, lifetime.Value);
        }

        private void Update()
        {
            if (!lifetime.HasValue) return;

            lifetime -= Time.deltaTime;
            if (lifetime > 0f) return;

            lifetime = null;
            owner.ReportCentralStimulusDied(this);
        }

        #endregion
    }
}