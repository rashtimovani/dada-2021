﻿using System;
using RasHack.GapOverlap.Main.Settings;
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

        public Vector3 ScreenInCM
        {
            get
            {
                var ratio = (float)Screen.height / Screen.width;
                var sizeInCM = settings.ReferencePoint.ScreenDiagonalInInches * 2.54f;

                var width = Mathf.Sqrt(sizeInCM * sizeInCM / (1 + ratio * ratio));
                var height = ratio * width;

                return new Vector3(width, height, 0);
            }
        }

        public Vector3 BottomLeft => Point(new Vector3(0, 0));

        public Vector3 BottomRight => Point(new Vector3(Screen.width, 0));

        public Vector3 TopLeft => Point(new Vector3(0, Screen.height));

        public Vector3 TopRight => Point(new Vector3(Screen.width, Screen.height));

        public Vector3 Center => Vector3.Lerp(BottomLeft, TopRight, 0.5f);

        public Vector3 CenterLeftThird => new(Mathf.Lerp(BottomLeft.x, TopRight.x, 0.35f), BottomLeft.y, BottomLeft.z);

        public Vector3 CenterRightThird => new(Mathf.Lerp(BottomLeft.x, TopRight.x, 0.65f), TopRight.y, TopRight.z);
        
        public Vector3 LeftThird => new(Mathf.Lerp(BottomLeft.x, TopRight.x, 0.3f), TopRight.y, TopRight.z);

        public Vector3 RightThird => new(Mathf.Lerp(BottomLeft.x, TopRight.x, 0.7f), BottomRight.y, BottomRight.z);

        public static Vector3 ScreenCenter => new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        public Vector3 Point(Vector3 inputPosition)
        {
            return InDepth(mainCamera.ScreenToWorldPoint(inputPosition));
        }

        public Vector3 ScreenSizeFromDegrees(float sizeInDegrees, float offsetInDegrees)
        {
            var halfSizeInRadians = Mathf.Deg2Rad * sizeInDegrees / 2f;
            var offsetInRadians = Mathf.Deg2Rad * offsetInDegrees;

            var farFromCenter = Mathf.Tan(offsetInRadians + halfSizeInRadians);

            var nearFromCenter = Mathf.Tan(offsetInRadians - halfSizeInRadians);

            var sizeInCM = (farFromCenter - nearFromCenter) * settings.ReferencePoint.DistanceFromEyesInCM;

            return CMToPixel(new Vector3(sizeInCM, sizeInCM, 1));
        }

        public Vector3 RealWorldSizeFromDegrees(float sizeInDegrees, float offsetInDegrees)
        {
            var screenSize = ScreenSizeFromDegrees(sizeInDegrees, offsetInDegrees);

            var size = Point(screenSize) - Point(Vector3.zero);
            return new Vector3(size.x, size.y, 1);
        }

        public Vector3 ScreenPosition(Vector3 anchor, float degrees, Vector3 direction)
        {
            var radians = Mathf.Deg2Rad * degrees;

            var distanceInCM = Mathf.Tan(radians) * settings.ReferencePoint.DistanceFromEyesInCM;
            var distance = CMToPixel(direction * distanceInCM);

            return anchor + distance;
        }

        #endregion

        #region Helpers

        private Vector3 InDepth(Vector3 original)
        {
            return new Vector3(original.x, original.y, depth);
        }

        private Vector3 CMToPixel(Vector3 inCm)
        {
            var screenInCM = ScreenInCM;

            var widthRatio = inCm.x / screenInCM.x;
            var heightRatio = inCm.y / screenInCM.y;

            var width = widthRatio * Screen.width;
            var height = heightRatio * Screen.height;

            return new Vector3(width, height, 1);
        }

        #endregion
    }
}