using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class TimeInput : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private InputField time;

        #endregion

        #region Internals

        private DefaultValueProvider defaultValue;

        #endregion

        #region API

        public delegate float DefaultValueProvider();

        public void SetDefault(DefaultValueProvider newDefault)
        {
            defaultValue = newDefault;
        }

        public float Reset()
        {
            var resetValue = Default;
            Time = resetValue;
            return resetValue;
        }

        public float Time
        {
            get => ParseInput(time);
            set => time.text = $"{value:0.00}";
        }

        private float Default => defaultValue?.Invoke() ?? 0f;

        #endregion

        #region Helpers

        private float ParseInput(InputField field)
        {
            return string.IsNullOrWhiteSpace(field.text) ? Default : float.Parse(field.text);
        }

        #endregion
    }
}