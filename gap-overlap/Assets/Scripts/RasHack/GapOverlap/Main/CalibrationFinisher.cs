using Tobii.Research.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public enum CalibrationState
    {
        Initialized,
        Running,
        Message,
        Done
    }

    public class CalibrationFinisher : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private Calibration calibration;

        [SerializeField]
        private EyeTracker tracker;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private GameObject messageObject;

        [SerializeField]
        private float messageDuration = 5f;

        private CalibrationState state = CalibrationState.Initialized;
        private float messageRemaining;

        #endregion

        #region Mono methods

        private void Start()
        {
            HideMessage();
        }

        private void Update()
        {
            switch (state)
            {
                case CalibrationState.Initialized:
                    if (tracker.EyeTrackerInterface == null) DisplayMessage("No eye tracker detected!");
                    else if (calibration.StartCalibration()) state = CalibrationState.Running;
                    else DisplayMessage("Calibration is already running");
                    break;
                case CalibrationState.Running:
                    if (!calibration.CalibrationInProgress) DisplayMessage(calibration.LatestCalibrationSuccessful ? "Calibration was succcessful" : "Calibration failed!");
                    break;
                case CalibrationState.Message:
                    messageRemaining -= Time.deltaTime;
                    if (messageRemaining <= 0)
                    {
                        state = CalibrationState.Done;
                        HideMessage();
                    }
                    break;
                default:
                    SceneManager.LoadScene(0); // Main scene
                    break;
            }
        }

        #endregion 

        #region Helper methods

        private void HideMessage()
        {
            messageRemaining = 0f;
            messageText.text = "";
            messageObject.SetActive(false);
        }

        private void DisplayMessage(string message)
        {
            state = CalibrationState.Message;
            messageRemaining = messageDuration;
            Debug.Log(message);
            messageText.text = message;
            messageObject.SetActive(true);
        }

        #endregion
    }
}