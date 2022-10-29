using Tobii.Research;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class TobiiEyePointer : Pointer
    {
        #region Fields

        [SerializeField] private Eye gazeEye;
        [SerializeField] private float acceptableLastValidReadingTimeout = 10f;

        private Vector3 lastDetectedPosition = NOT_DETECTED;
        private float timeSinceLastValidReading;
        private IEyeTracker subscribedTo;

        #endregion

        #region API

        public override PointerStatus Status
        {
            get
            {
                if (!PointerEnabled) return new() { Enabled = false, Message = $"Tracker for {Eye} is disabled!" };

                if (subscribedTo == null)
                    return new() { Enabled = false, Message = $"No eye tracker detected for {Eye} eye." };

                if (timeSinceLastValidReading > acceptableLastValidReadingTimeout)
                    return new()
                    {
                        Enabled = false, Message =
                            $"No valid gaze points from {subscribedTo.Model}({subscribedTo.DeviceName}) on address {subscribedTo.Address} for {Eye} eye."
                    };

                return new()
                {
                    Enabled = true, Message =
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
            subscribedTo = null;
            PointerEnabled = true;
        }

        protected override void Update()
        {
            base.Update();
            timeSinceLastValidReading += Time.deltaTime;

            if (subscribedTo != null) return;

            var eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            foreach (IEyeTracker eyeTracker in eyeTrackers)
            {
                subscribedTo = eyeTracker;
                subscribedTo.GazeDataReceived += GazeDataReceived;
                Debug.Log($"Subscribed to {Status}, started to receive gaze events from it");
                break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (subscribedTo != null) subscribedTo.GazeDataReceived -= GazeDataReceived;
            PointerEnabled = false;
        }

        #endregion

        #region Helpers

        private void GazeDataReceived(object sender, GazeDataEventArgs gazeEvent)
        {
            var eye = GetEye(gazeEvent);

            if (eye.GazePoint.Validity == Validity.Invalid) lastDetectedPosition = NOT_DETECTED;

            timeSinceLastValidReading = 0.0f;
            var x = eye.GazePoint.PositionOnDisplayArea.X * Screen.width;
            var y = (1 - eye.GazePoint.PositionOnDisplayArea.Y) * Screen.height;
            lastDetectedPosition = new Vector2(x, y);
        }

        private EyeData GetEye(GazeDataEventArgs gazeEvent)
        {
            switch (Eye)
            {
                case Eye.Left: return gazeEvent.LeftEye;
                case Eye.Right:
                default: return gazeEvent.RightEye;
            }
        }

        #endregion
    }
}