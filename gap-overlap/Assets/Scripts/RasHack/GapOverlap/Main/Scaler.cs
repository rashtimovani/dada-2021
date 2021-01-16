using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class Scaler
    {
        private const float TARGET_WIDTH = 1920;
        private const float TARGET_HEIGHT = 1080;

        private readonly Camera mainCamera;
        private readonly float depth;

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


        private Vector3 inDepth(Vector3 original)
        {
            return new Vector3(original.x, original.y, depth);
        }

        public Vector3 point(Vector3 inputPosition)
        {
            return inDepth(mainCamera.ScreenToWorldPoint(inputPosition));
        }
    }
}