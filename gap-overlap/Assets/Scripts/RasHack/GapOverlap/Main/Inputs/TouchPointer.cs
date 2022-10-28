using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class TouchPointer : Pointer
    {
        #region API

        protected override Vector3 Position =>
            Input.touchSupported && Input.touchCount > 0 ? Input.touches[0].position : NOT_DETECTED;

        public override Eye Eye => Eye.Left;

        #endregion
    }
}