using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli.Animaition
{
    public abstract class StimuliAnimation : MonoBehaviour
    {
        #region Fields

        private float spentLife;
        protected float totalLife;

        #endregion

        #region API

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
                return;
            }

            Animate();
        }

        #endregion
    }
}