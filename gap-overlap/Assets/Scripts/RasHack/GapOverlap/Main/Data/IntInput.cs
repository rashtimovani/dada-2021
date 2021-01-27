using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class IntInput : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private InputField valueInput;

        #endregion

        #region Internals

        private DefaultValueProvider defaultValue;

        #endregion

        #region API

        public delegate int DefaultValueProvider();

        public void SetDefault(DefaultValueProvider newDefault)
        {
            defaultValue = newDefault;
        }

        public int Reset()
        {
            var resetValue = Default;
            Value = resetValue;
            return resetValue;
        }

        public int Value
        {
            get => ParseInput();
            set => valueInput.text = value.ToString();
        }

        private int Default => defaultValue?.Invoke() ?? 0;

        public bool HasValue => !string.IsNullOrWhiteSpace(valueInput.text);

        #endregion

        #region Helpers

        private int ParseInput()
        {
            return HasValue ? int.Parse(valueInput.text) : Default;
        }

        #endregion
    }
}