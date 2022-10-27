using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public abstract class Pointer : MonoBehaviour
    {
        #region Constants

        protected readonly Vector3 NotDetected = new Vector3(-20000, -20000, 0);

        #endregion

        #region Serialized fields

        [SerializeField] private Simulator simulator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region API

        public abstract Vector3 Position { get; }

        public abstract Eye Eye { get; }

        public void ShowPointer(bool show)
        {
            spriteRenderer.enabled = show;
        }

        #endregion

        #region Mono methods

        private void Update()
        {
            var position = Position;
            transform.position = simulator.Scaler.Point(position);
            spriteRenderer.transform.position = simulator.DebugScaler.Point(position);
        }

        #endregion
    }
}