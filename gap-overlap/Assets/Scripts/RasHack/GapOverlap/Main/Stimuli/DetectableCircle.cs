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

        private bool wasFocusedOnByLeftEye;
        private bool wasFocusedOnByRightEye;

        private ReplayController owner;

        private Action<Eye> onDetected;

        #endregion

        #region API

        public void RegisterOnDetect(ReplayController whoDetects, Action<Eye> onDetectedAction)
        {
            owner = whoDetects;
            onDetected = onDetectedAction;
            wasFocusedOnByLeftEye = false;
            wasFocusedOnByRightEye = false;
        }

        #endregion

        #region Unity methods

        private void LateUpdate()
        {
            debugArea.enabled = owner.Settings.ShowPointer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var pointer = other.gameObject.GetComponent<ReplayPointer>();
            if (pointer == null) return;

            if (pointer.Eye == Eye.Left)
            {
                if (wasFocusedOnByLeftEye) return;
                wasFocusedOnByLeftEye = true;
            }

            if (pointer.Eye == Eye.Right)
            {
                if (wasFocusedOnByRightEye) return;
                wasFocusedOnByRightEye = true;
            }

            onDetected.Invoke(pointer.Eye);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerEnter2D(other);
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerEnter2D(other);
        }

        private void OnDestroy()
        {
            onDetected = null;
        }

        #endregion
    }
}