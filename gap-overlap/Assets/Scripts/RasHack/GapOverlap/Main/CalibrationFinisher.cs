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
        private Color okColor = Color.white;

        [SerializeField]
        private Color errorColor = Color.red;


        [SerializeField]
        private float messageDuration = 5f;

        private Vector2[] points = new Vector2[] { new Vector2(0.1f, 0.1f), new Vector2(0.1f, 0.1f), new Vector2(0.1f, 0.1f), new Vector2(0.1f, 0.1f), new Vector2(0.1f, 0.1f) };

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
                    if (tracker.EyeTrackerInterface == null) DisplayMessage("No eye tracker detected!", errorColor);
                    else if (calibration.StartCalibration()) state = CalibrationState.Running;
                    else DisplayMessage("Calibration is already running", errorColor);
                    break;
                case CalibrationState.Running:
                    if (!calibration.CalibrationInProgress)
                    {
                        if (calibration.LatestCalibrationSuccessful) DisplayMessage("Calibration was succcessful", okColor);
                        else DisplayMessage("Calibration failed!", errorColor);
                    }
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

        private void DisplayMessage(string message, Color color)
        {
            state = CalibrationState.Message;
            messageRemaining = messageDuration;
            Debug.Log(message);
            messageText.text = message;
            messageText.color = color;
            messageObject.SetActive(true);
        }

        #endregion
    }
}