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

        private bool wasFocusedOn;

        private ReplayController owner;

        private Action<Pointer> onDetected;

        #endregion

        #region API

        public void RegisterOnDetect(ReplayController whoDetects, Action<Pointer> onDetectedAction)
        {
            owner = whoDetects;
            onDetected = onDetectedAction;
            wasFocusedOn = false;
        }

        #endregion

        #region Unity methods

        private void LateUpdate()
        {
            debugArea.enabled = owner.Settings.ShowPointer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (wasFocusedOn) return;
            var pointer = other.gameObject.GetComponent<Pointer>();
            if (pointer == null) return;
            wasFocusedOn = true;
            onDetected.Invoke(pointer);
        }

        private void OnTriggerStay2D(Collider2D other)
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