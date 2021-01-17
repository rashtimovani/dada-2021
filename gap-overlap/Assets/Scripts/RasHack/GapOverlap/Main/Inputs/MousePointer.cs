using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class MousePointer : PointerDevice
    {
        public Vector3? Position => Input.mousePresent ? Input.mousePosition : (Vector3?) null;
    }
}