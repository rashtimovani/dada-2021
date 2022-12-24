using System;
using Tobii.Research;
using Tobii.Research.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public class TobiiTroubleShooter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private EyeTracker eyeTracker;
        [SerializeField] private Text text;
        [SerializeField] private Image image;

        #endregion

        #region Mono methods

        private void Update()
        {
            if (eyeTracker == null)
            {
                ShowMessage("No eye tracker prefab detected!");
                return;
            }

            if (eyeTracker.EyeTrackerInterface == null)
            {
                ShowMessage("No eyetrackers detected!");
                return;
            }

            if (!eyeTracker.SubscribeToGazeData)
            {
                ShowMessage("Eye tracker is not subscribet to getting gaze data1");
                return;
            }

            if (eyeTracker.GazeDataCount == 0)
            {
                ShowMessage("Eye tracker is not receiving gaze data!");
                return;
            }

            HideMessage();
        }

        #endregion

        #region Helper methods

        private void HideMessage()
        {
            text.text = "";
            text.enabled = false;
            image.enabled = false;
        }

        private void ShowMessage(string message)
        {
            text.text = message;
            text.enabled = true;
            image.enabled = true;
        }

        #endregion
    }
}