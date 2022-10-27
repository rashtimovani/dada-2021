using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class MousePointer : Pointer
    {
        public override Vector3 Position => Input.mousePresent ? Input.mousePosition : NotDetected;

        public override Eye Eye => Eye.Right;
    }
}