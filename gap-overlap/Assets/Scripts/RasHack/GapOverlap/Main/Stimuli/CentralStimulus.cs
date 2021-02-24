using RasHack.GapOverlap.Main.Stimuli.Animaition;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class CentralStimulus : ScalableStimulus
    {
        #region Provided fields from simulator

        private Task.Task owner;
        private float? lifetime;

        #endregion

        #region API

        public void StartSimulating(Task.Task owner, float lifetime)
        {
            this.owner = owner;
            this.lifetime = lifetime;
            
            DoFadeIn(lifetime, owner.FadeInOut);
        }

        #endregion

        #region Mono methods
        
        private void Start() 
        {
            GetComponent<AudioSource>().Play();
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