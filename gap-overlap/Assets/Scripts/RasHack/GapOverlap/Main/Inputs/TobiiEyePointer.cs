using Tobii.Research;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class TobiiEyePointer : Pointer
    {
        #region Fields

        [SerializeField] private Eye gazeEye;

        private IEyeTracker subscribedTo;

        #endregion

        #region API

        private string Status =>
            subscribedTo == null
                ? $"No eye tracker detected for {Eye} eye."
                : $"Using {subscribedTo.Model}({subscribedTo.DeviceName}) on address {subscribedTo.Address} for {Eye} eye.";

        protected override Vector3 Position { get; }

        public override Eye Eye => gazeEye;

        #endregion

        #region Mono methods

        protected override void Start()
        {
            base.Start();
            subscribedTo = null;
        }

        protected override void Update()
        {
            base.Update();

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
        }

        #endregion

        #region Helpers

        private void GazeDataReceived(object sender, GazeDataEventArgs gazeEvent)
        {
            var eye = GetEye(gazeEvent);
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