using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class Pointer : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private Simulator simulator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Internals

        private readonly TouchPointer touch = new TouchPointer();
        private readonly MousePointer mouse = new MousePointer();

        #endregion

        #region API

        public void ShowPointer(bool show)
        {
            spriteRenderer.enabled = show;
        }

        #endregion

        #region Mono methods

        private void Update()
        {
            var position = touch.Position ?? mouse.Position;
            if (!position.HasValue) return;

            transform.position = simulator.Scaler.Point(position.Value);
            spriteRenderer.transform.position = simulator.DebugScaler.Point(position.Value);
        }

        #endregion
    }
}