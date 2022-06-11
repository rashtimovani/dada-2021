using System;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Data
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
        private int Right => valueInput.HasValue ? oldMax - Left : 0;

        public void Initial(float value)
        {
            var max = valueInput.Value;
            oldMax = max;

            slider.maxValue = max;
            slider.value = Math.Min(value, max);
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            OnSliderValueChanged();
        }

        private void Update()
        {
            if (!valueInput.HasValue)
            {
                slider.value = 0;
                return;
            }
            
            var max = valueInput.Value;
            if (oldMax == max) return;

            var percent = slider.value / slider.maxValue;
            slider.maxValue = valueInput.Value;

            var newValue = Mathf.RoundToInt(percent * max);

            oldMax = max;
            slider.value = newValue;
            OnSliderValueChanged();
        }

        public void OnSliderValueChanged()
        {
            var left = valueInput.HasValue ? Left.ToString() : "-";
            var right = valueInput.HasValue ? Right.ToString() : "-";
            label.text = $"Left/Right ratio: {left}/{right}";
        }

        #endregion
    }
}