using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    [System.Serializable]
    public struct RawPosition
    {
        public Vector2 Center;
        public Vector2 TopLeft;
        public Vector2 TopRight;
        public Vector2 BottomLeft;
        public Vector2 BottomRight;
    }
}