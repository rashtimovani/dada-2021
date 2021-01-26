using System;
using RasHack.GapOverlap.Main.Data;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    [Serializable]
    public struct ReferencePoint
    {
        public float DistanceFromEyesInCM;
        public float ScreenDiagonalInInches;
    }

    public class Scaler
    {
        #region Internal fields

        private const float TARGET_WIDTH = 1920;
        private const float TARGET_HEIGHT = 1080;

        private readonly Camera mainCamera;
        private readonly float depth;
        private readonly MainSettings settings;

        #endregion

        #region API

        public Scaler(Camera mainCamera, float depth, MainSettings settings)
        {
            this.mainCamera = mainCamera;
            this.depth = depth;
            this.settings = settings;
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

        public Vector3 ScreenCenter => new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        public Vector3 InWorld(Vector2 screenPercent)
        {
            return inDepth(point(
                new Vector3(screenPercent.x * Screen.width, screenPercent.y * Screen.height, depth)));
        }

        public Vector3 point(Vector3 inputPosition)
        {
            return inDepth(mainCamera.ScreenToWorldPoint(inputPosition));
        }

        public Vector3 ScaleSize(Vector3 currentScale, float sizeInDegrees, float offsetInDegrees)
        {
            var halfSizeInRadians = Mathf.Deg2Rad * sizeInDegrees / 2f;
            var offsetInRadians = Mathf.Deg2Rad * offsetInDegrees;

            var farFromCenter = Mathf.Tan(offsetInRadians + halfSizeInRadians);

            var nearFromCenter = Mathf.Tan(offsetInRadians - halfSizeInRadians);

            var sizeInCM = (farFromCenter - nearFromCenter) * settings.ReferencePoint.DistanceFromEyesInCM;

            return new Vector3(sizeInCM, sizeInCM, 1);
        }

        public Vector3 ScreenPosition(Vector3 anchor, float degrees, Vector3 direction)
        {
            var radians = Mathf.Deg2Rad * degrees;

            var distance = Mathf.Tan(radians) * settings.ReferencePoint.DistanceFromEyesInCM;

            return anchor + direction * distance;
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