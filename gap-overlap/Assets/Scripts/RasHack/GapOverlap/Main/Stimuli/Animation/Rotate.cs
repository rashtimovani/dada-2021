using System;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli.Animaition
{
    public class Rotate : StimuliAnimation
    {
        #region Fields

        private GameObject rotate;
        private float cycles;
        private Quaternion toLeft;
        private Quaternion toRight;

        #endregion

        #region API

        public static Rotate ForStimuli(GameObject toRotate, float duration, int rotationFactor, Action onFinish)
        {
            var rotate = toRotate.AddComponent<Rotate>();

            rotate.totalLife = duration;
            rotate.onFinish = onFinish;

            rotate.cycles = Mathf.PI * rotationFactor;
            rotate.rotate = toRotate;
            rotate.toLeft = Quaternion.Euler(new Vector3(0, 0, 20f));
            rotate.toRight = Quaternion.Euler(new Vector3(0, 0, -20f));

            return rotate;
        }

        #endregion

        #region Implementation

        protected override void Animate()
        {
            var lerp = 0.5F * (1.0F + Mathf.Sin(cycles * Spent));
            rotate.transform.localRotation = Quaternion.Lerp(toLeft, toRight, lerp);
        }

        #endregion
    }
}