using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class StimuliArea : MonoBehaviour
    {
        #region Internals

        private Simulator simulator;

        #endregion

        #region API

        private Vector3 LeftInWorld => Point(Vector3.left);

        private Vector3 RightInWorld => Point(Vector3.right);

        public Vector3 NextInWorld => Random.Range(0, 2) == 0 ? LeftInWorld : RightInWorld;

        #endregion

        #region Mono methods

        private void Awake()
        {
            simulator = GetComponent<Simulator>();
        }

        #endregion

        #region Helpers

        private float Degrees => simulator.Settings.StimulusDistanceInDegrees / 2;

        private Scaler Scaler => simulator.Scaler;

        private Vector3 Center => Scaler.ScreenCenter;

        private Vector3 Point(Vector3 direction)
        {
            return Scaler.Point(Scaler.ScreenPosition(Center, Degrees, direction));
        }

        #endregion
    }
}