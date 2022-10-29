using System;
using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class DetectableArea : MonoBehaviour
    {
        #region Internal fields

        [SerializeField] private SpriteRenderer debugArea;

        #endregion

        #region

        private bool wasFocusedOn;

        private Simulator owner;

        private Action<Pointer> onDetected;

        #endregion

        #region API

        public StimulusSide AreaScreenSide { get; set; }

        private Scaler Scaler => owner.Scaler;

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

            gameObject.transform.rotation = Quaternion.identity;
            switch (AreaScreenSide)
            {
                case StimulusSide.Center:
                    ScaleToFill(Scaler.CenterLeftThird, Scaler.CenterRightThird, gameObject);
                    break;
                case StimulusSide.Left:
                    ScaleToFill(Scaler.BottomLeft, Scaler.LeftThird, gameObject);
                    break;
                case StimulusSide.Right:
                    ScaleToFill(Scaler.RightThird, Scaler.TopRight, gameObject);
                    break;
            }
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

        #region Helpers

        private void ScaleToFill(Vector3 bottomLeft, Vector3 topRight, GameObject what)
        {
            var scale = new Vector3(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y, 1);
            what.transform.localScale = what.transform.parent.InverseTransformVector(scale);
            what.transform.position = Vector3.Lerp(bottomLeft, topRight, 0.5f);
        }

        #endregion
    }
}