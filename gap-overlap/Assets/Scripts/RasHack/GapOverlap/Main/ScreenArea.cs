using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class ScreenArea
    {
        #region Fields

        private readonly int x1;
        private readonly int x2;
        private readonly int y1;
        private readonly int y2;

        #endregion

        #region Constructors

        public ScreenArea(int x1, int x2, int y1, int y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        #endregion

        #region API

        public static ScreenArea WholeScreen => new ScreenArea(0, Screen.width, 0, Screen.height);

        public Vector3 BottomLeft => new Vector3(x1, y1);
        public Vector3 BottomRight => new Vector3(x2, y1);
        public Vector3 TopLeft => new Vector3(x1, y2);
        public Vector3 TopRight => new Vector3(x2, y2);

        public Vector3 Center => Vector3.Lerp(BottomLeft, TopRight, 0.5f);

        public int Width => x2 - x1;
        public int Height => y2 - y1;

        public float Ratio => (float)Height / Width;

        #endregion
    }
}