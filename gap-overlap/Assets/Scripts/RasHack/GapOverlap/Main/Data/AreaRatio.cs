using System;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class AreaRatio : MonoBehaviour
    {
        #region Fileds

        [SerializeField] private Slider slider;
        [SerializeField] private Text label;
        [SerializeField] private IntInput valueInput;

        #endregion

        #region Internals

        private int oldMax;

        #endregion

        #region API

        public int Left => Mathf.RoundToInt(slider.value);
        public int Right => oldMax - Left;

        public void Initial(float value)
        {
            var max = valueInput.Value;
            oldMax = max;

            slider.maxValue = max;
            slider.value = Math.Min(value, max);
        }

        #endregion

        #region Mono methods

        private void Update()
        {
            var max = valueInput.Value;
            if (oldMax == max) return;

            var percent = slider.value / slider.maxValue;
            slider.maxValue = valueInput.Value;

            var newValue = Mathf.RoundToInt(percent * max);
            slider.value = newValue;

            oldMax = max;
        }

        public void OnSliderValueChanged()
        {
            label.text = $"Left/Right ratio: {Left}/{Right}";
        }

        #endregion
    }
}