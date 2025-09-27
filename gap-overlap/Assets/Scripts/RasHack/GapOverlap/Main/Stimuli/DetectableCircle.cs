using System;
using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class DetectableCircle : MonoBehaviour
    {
        #region Internal fields

        [SerializeField] private SpriteRenderer debugArea;

        #endregion

        #region

        private ReplayController owner;


        #endregion

        #region API

        public void RegisterOnDetect(ReplayController whoDetects)
        {
            owner = whoDetects;
        }

        #endregion

        #region Unity methods

        private void LateUpdate()
        {
            debugArea.enabled = owner.Settings.ShowPointer;
        }

        #endregion
    }
}