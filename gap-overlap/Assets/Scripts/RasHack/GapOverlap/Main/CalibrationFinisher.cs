using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tobii.Research;
using Tobii.Research.Unity;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RasHack.GapOverlap.Main
{
    public enum CalibrationState
    {
        Initialized,
        Running,
        Done
    }

    public class CalibrationFinisher : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private Calibration calibration;

        private CalibrationState state = CalibrationState.Initialized;

        #endregion

        #region Mono methods
        private void Update()
        {
            switch (state)
            {
                case CalibrationState.Initialized:
                    if (calibration.StartCalibration()) state = CalibrationState.Running;
                    break;
                case CalibrationState.Running:
                    if (!calibration.CalibrationInProgress) state = CalibrationState.Done;
                    break;
                default:
                    break;

            }
        }

        #endregion 
    }
}