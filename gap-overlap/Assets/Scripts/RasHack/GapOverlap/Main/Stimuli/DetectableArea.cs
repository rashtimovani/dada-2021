using System;
using System.Collections;
using System.Collections.Generic;
using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;


namespace RasHack.GapOverlap.Main.Stimuli
{
    public class DetectableArea : MonoBehaviour
    {
        #region Internal fields

        [SerializeField] private BoxCollider2D collider;

        [SerializeField] private SpriteRenderer debugArea;

        #endregion

        #region

        private bool wasFocusedOn;

        private Simulator owner;

        private Action<Pointer> onDetected;

        #endregion

        #region API

        public void RegisterOnDetect(Simulator whoDetects, Action<Pointer> onDetectedAction)
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
            transform.rotation = Quaternion.identity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (wasFocusedOn) return;
            var pointer = other.gameObject.GetComponent<Pointer>();
            if (pointer == null) return;
            wasFocusedOn = true;
            onDetected.Invoke(pointer);
        }

        private void OnDestroy()
        {
            onDetected = null;
        }

        #endregion
    }
}