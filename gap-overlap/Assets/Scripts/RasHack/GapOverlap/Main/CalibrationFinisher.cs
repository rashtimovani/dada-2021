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
        Stopping,
        Stopped
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
        private float warmUpDuration = 5f;

        [SerializeField]
        private float stoppingDuration = 5f;

        private Vector2[] points = new Vector2[]
        {
            new Vector2(0.1f, 0.1f), new Vector2(0.5f, 0.1f), new Vector2(0.9f, 0.1f),
            new Vector2(0.1f, 0.5f), new Vector2(0.5f, 0.5f),new Vector2(0.9f, 0.5f),
            new Vector2(0.1f, 0.9f),new Vector2(0.5f, 0.9f), new Vector2(0.9f, 0.9f),
        };

        private CalibrationState state = CalibrationState.Initialized;

        private float? stoppingRemaining;
        private float? warmUpRemaining;

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
                    if (tracker.EyeTrackerInterface == null)
                    {
                        if (warmUpRemaining == null)
                        {
                            warmUpRemaining = warmUpDuration;
                            DisplayMessage("Waiting to connect to eye tracker", errorColor);
                        }
                        
                        warmUpRemaining -= Time.deltaTime;
                        if (warmUpRemaining <= 0) DisplayMessageAndStop("No eye tracker detected!", errorColor);
                    }
                    else if (calibration.StartCalibration(points)) state = CalibrationState.Running;
                    else DisplayMessageAndStop("Calibration is already running", errorColor);
                    break;
                case CalibrationState.Running:
                    if (!calibration.CalibrationInProgress)
                    {
                        if (calibration.LatestCalibrationSuccessful) DisplayMessageAndStop("Calibration was succcessful", okColor);
                        else DisplayMessageAndStop("Calibration failed!", errorColor);
                    }
                    break;
                case CalibrationState.Stopping:
                    stoppingRemaining -= Time.deltaTime;
                    if (stoppingRemaining <= 0)
                    {
                        state = CalibrationState.Stopped;
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
            stoppingRemaining = null;
            messageText.text = "";
            messageObject.SetActive(false);
        }

        private void DisplayMessageAndStop(string message, Color color)
        {
            state = CalibrationState.Stopping;
            stoppingRemaining = stoppingDuration;
            DisplayMessage(message, color);
        }

        private void DisplayMessage(string message, Color color)
        {
            Debug.Log(message);
            messageText.text = message;
            messageText.color = color;
            messageObject.SetActive(true);
        }

        #endregion
    }
}