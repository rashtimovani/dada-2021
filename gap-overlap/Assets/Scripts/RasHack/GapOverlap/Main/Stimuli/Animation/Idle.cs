using System;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli.Animation
{
    public class Idle : StimuliAnimation
    {
        #region API

        public static Idle ForStimuli(GameObject stimuli, float duration, Action onFinish)
        {
            var idle = stimuli.AddComponent<Idle>();

            idle.totalLife = duration;
            idle.onFinish = onFinish;

            return idle;
        }

        #endregion

        #region Implementation

        protected override void Animate()
        {
        }

        #endregion
    }
}