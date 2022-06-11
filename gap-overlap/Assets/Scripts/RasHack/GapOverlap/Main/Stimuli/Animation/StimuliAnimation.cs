using System;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli.Animation
{
    public abstract class StimuliAnimation : MonoBehaviour
    {
        #region Fields

        private float spentLife;
        protected float totalLife;

        protected Action onFinish;

        #endregion

        #region API

        protected float Spent => spentLife;
        
        private bool HasEnded => spentLife >= totalLife;

        protected float Clamp => Mathf.Clamp01(spentLife / totalLife);

        protected abstract void Animate();

        #endregion

        #region Mono methods

        protected void Update()
        {
            spentLife += Time.deltaTime;
            if (HasEnded)
            {
                Destroy(this);
                onFinish?.Invoke();
                return;
            }

            Animate();
        }

        #endregion
    }
}