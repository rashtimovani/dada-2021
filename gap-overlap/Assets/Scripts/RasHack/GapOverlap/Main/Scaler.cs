using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class Scaler
    {
        #region Internal fields

        private const float TARGET_WIDTH = 1920;
        private const float TARGET_HEIGHT = 1080;

        private readonly Camera mainCamera;
        private readonly float depth;

        #endregion

        #region API

        public Scaler(Camera mainCamera, float depth)
        {
            this.mainCamera = mainCamera;
            this.depth = depth;
        }

        public Vector3 BottomLeft => inDepth(mainCamera.ScreenToWorldPoint(
            new Vector3(0, 0)));

        public Vector3 BottomRight => inDepth(mainCamera.ScreenToWorldPoint(
            new Vector3(Screen.width, 0)));

        public Vector3 TopLeft => inDepth(mainCamera.ScreenToWorldPoint(
            new Vector3(0, Screen.height)));

        public Vector3 TopRight => inDepth(mainCamera.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height)));

        public Vector3 Center => Vector3.Lerp(BottomLeft, TopRight, 0.5f);

        public Vector3 point(Vector3 inputPosition)
        {
            return inDepth(mainCamera.ScreenToWorldPoint(inputPosition));
        }

        #endregion

        #region Helpers

        private Vector3 inDepth(Vector3 original)
        {
            return new Vector3(original.x, original.y, depth);
        }

        #endregion
    }
}