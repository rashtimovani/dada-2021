using UnityEngine;
using UnityEngine.XR;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class TouchPointer : PointerDevice
    {
        public Vector3? Position =>
            Input.touchSupported && Input.touchCount > 0 ? Input.touches[0].position : (Vector3?) null;
    }
}