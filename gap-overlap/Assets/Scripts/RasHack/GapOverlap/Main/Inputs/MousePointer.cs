using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class MousePointer : Pointer
    {
        #region API

        protected override Vector3 Position => Input.mousePresent ? Input.mousePosition : NOT_DETECTED;

        public override Eye Eye => Eye.Right;

        #endregion
    }
}