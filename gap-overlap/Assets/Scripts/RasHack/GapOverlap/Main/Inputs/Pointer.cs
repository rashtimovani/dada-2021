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

        #region API

        public void ShowPointer(bool show)
        {
            spriteRenderer.enabled = show;
        }

        #endregion

        #region Mono methods

        private void Update()
        {
            transform.position = simulator.DebugScaler.point(Input.mousePosition);
        }

        #endregion
    }
}