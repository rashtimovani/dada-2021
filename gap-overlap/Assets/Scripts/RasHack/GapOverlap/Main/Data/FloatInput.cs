using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class FloatInput : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private InputField time;

        [SerializeField] private string format = "0.00";

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
            Value = resetValue;
            return resetValue;
        }

        public float Value
        {
            get => ParseInput(time);
            set => time.text = value.ToString(format);
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