using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    #region Helper structures

    public struct PointerStatus
    {
        public bool Enabled;
        public string Message;
    }

    #endregion

    public abstract class Pointer : MonoBehaviour
    {
        #region Constants

        protected static readonly Vector3 NOT_DETECTED = new(-20000, -20000, 0);

        #endregion

        #region Serialized fields

        [SerializeField] private Simulator simulator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region API

        public bool PointerEnabled { get; set; }

        public virtual PointerStatus Status => new()
            { Enabled = PointerEnabled, Message = PointerEnabled ? "Enabled" : "Disabled" };

        protected abstract Vector3 Position { get; }

        public abstract Eye Eye { get; }

        public void ShowPointer(bool show)
        {
            spriteRenderer.enabled = show;
        }

        #endregion

        #region Mono methods

        protected virtual void Start()
        {
            PointerEnabled = true;
        }

        protected virtual void Update()
        {
            var position = PointerEnabled ? Position : NOT_DETECTED;
            transform.position = simulator.Scaler.Point(position);
            spriteRenderer.transform.position = simulator.DebugScaler.Point(position);
        }

        protected virtual void OnDestroy()
        {
            PointerEnabled = false;
        }

        #endregion
    }
}