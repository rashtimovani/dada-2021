using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class TouchPointer : Pointer
    {
        public override Vector3 Position =>
            Input.touchSupported && Input.touchCount > 0 ? Input.touches[0].position : NotDetected;

        public override Eye Eye => Eye.Left;
    }
}