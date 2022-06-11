using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class FloatInput : MonoBehaviour
    {
        #region Serialized fields

        [FormerlySerializedAs("time")] [SerializeField]
        private InputField valueInput;

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
            get => ParseInput();
            set => valueInput.text = value.ToString(format);
        }

        private float Default => defaultValue?.Invoke() ?? 0f;

        public bool HasValue => !string.IsNullOrWhiteSpace(valueInput.text);

        #endregion

        #region Helpers

        private float ParseInput()
        {
            return HasValue ? float.Parse(valueInput.text) : Default;
        }

        #endregion
    }
}