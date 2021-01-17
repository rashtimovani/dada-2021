using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class CentralStimulus : MonoBehaviour
    {
        #region Internal fields

        private float spentLifetime;

        #endregion

        #region Provided fields from simulator

        private Task.Task owner;
        private float lifetime;

        #endregion

        #region API

        public void StartSimulating(Task.Task owner, float lifetime)
        {
            this.owner = owner;
            this.lifetime = lifetime;
        }

        #endregion

        #region Mono methods

        private void Update()
        {
            spentLifetime += Time.deltaTime;
            if (spentLifetime < lifetime) return;

            owner.ReportCentralStimulusDied(this);
            Destroy(gameObject);
        }

        #endregion
    }
}