using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    [System.Serializable]
    public class RawPoint
    {
        public float X;
        public float Y;
    }

    [System.Serializable]
    public class RawPosition
    {
        public RawPoint Center;
        // public Vector2 TopLeft;
        // public Vector2 TopRight;
        // public Vector2 BottomLeft;
        // public Vector2 BottomRight;
    }
}