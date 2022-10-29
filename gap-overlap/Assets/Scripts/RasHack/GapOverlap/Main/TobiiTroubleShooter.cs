using System;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public class TobiiTroubleShooter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Simulator simulator;
        [SerializeField] private Text text;
        [SerializeField] private Image image;

        #endregion

        #region Mono methods

        private void Update()
        {
            var fullStatus = "";
            var show = false;
            var separator = "";

            foreach (var pointer in simulator.TobiiPointers)
            {
                var status = pointer.Status;
                fullStatus += separator;
                fullStatus += status.Message;
                show |= !status.Enabled;
                separator = Environment.NewLine;
            }

            text.text = fullStatus;
            text.enabled = show;
            image.enabled = show;
        }

        #endregion
    }
}