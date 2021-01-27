using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public struct NextArea
    {
        public Vector3 Position;
        public float OffsetInDegrees;
        public string Side;
    }

    public class StimuliArea : MonoBehaviour
    {
        #region Internals

        private Simulator simulator;

        #endregion

        #region API

        private NextArea LeftInWorld => Point(Vector3.left);

        private NextArea RightInWorld => Point(Vector3.right);

        public NextArea NextInWorld => Random.Range(0, 2) == 0 ? LeftInWorld : RightInWorld;

        public NextArea CenterInWorld => new NextArea {Position = Scaler.Center, OffsetInDegrees = 0f, Side = "central"};

        #endregion

        #region Mono methods

        private void Awake()
        {
            simulator = GetComponent<Simulator>();
        }

        #endregion

        #region Helpers

        private float Degrees => simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees / 2;

        private Scaler Scaler => simulator.Scaler;

        private Vector3 Center => Scaler.ScreenCenter;

        private NextArea Point(Vector3 direction)
        {
            var position = Scaler.Point(Scaler.ScreenPosition(Center, Degrees, direction));
            var offsetInDegrees = direction.x * Degrees;
            var side = direction.x > 0 ? "right" : "left";
            return new NextArea {Position = position, OffsetInDegrees = offsetInDegrees, Side = side};
        }

        #endregion
    }
}