using Tobii.Research;
using Tobii.Research.Unity;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class TobiiEyePointer : Pointer
    {
        #region Fields

        [SerializeField] private Eye gazeEye;

        [SerializeField] private EyeTracker eyeTracker;

        private Vector3 lastDetectedPosition = NOT_DETECTED;

        #endregion

        #region API

        public override PointerStatus Status
        {
            get
            {
                if (!PointerEnabled) return new() { Enabled = false, Message = $"Tracker for {Eye} is disabled!" };

                var subscribedTo = eyeTracker?.EyeTrackerInterface;
                if (subscribedTo == null)
                    return new() { Enabled = false, Message = $"No eye tracker detected for {Eye} eye." };

                return new()
                {
                    Enabled = true,
                    Message =
                        $"Using {subscribedTo.Model}({subscribedTo.DeviceName}) on address {subscribedTo.Address} for {Eye} eye."
                };
            }
        }

        protected override Vector3 Position => lastDetectedPosition;

        public override Eye Eye => gazeEye;

        #endregion

        #region Mono methods

        protected override void Start()
        {
            base.Start();
            PointerEnabled = true;
        }

        protected override void Update()
        {
            base.Update();
            if (!PointerEnabled) return;

            var eye = GetEye(eyeTracker.LatestGazeData);
            if (!eye.GazePointValid) lastDetectedPosition = NOT_DETECTED;
            var x = eye.GazePointOnDisplayArea.x * Screen.width;
            var y = (1 - eye.GazePointOnDisplayArea.y) * Screen.height;
            lastDetectedPosition = new Vector2(x, y);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region Helpers

        private IGazeDataEye GetEye(IGazeData gazeData)
        {
            switch (Eye)
            {
                case Eye.Left: return gazeData.Left;
                case Eye.Right:
                default: return gazeData.Right;
            }
        }

        #endregion
    }
}