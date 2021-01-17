﻿using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class CentralStimulus : MonoBehaviour
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
        }

        #endregion

        #region Mono methods

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