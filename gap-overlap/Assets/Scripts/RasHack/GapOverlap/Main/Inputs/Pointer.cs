using System.Collections;
using System.Collections.Generic;
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

        private TouchPointer touch = new TouchPointer();
        private MousePointer mouse = new MousePointer();

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
            if (position.HasValue) transform.position = simulator.DebugScaler.point(position.Value);
        }

        #endregion
    }
}