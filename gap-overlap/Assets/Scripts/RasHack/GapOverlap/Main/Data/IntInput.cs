using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class IntInput : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private InputField time;

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
            get => ParseInput(time);
            set => time.text = value.ToString();
        }

        private int Default => defaultValue?.Invoke() ?? 0;

        #endregion

        #region Helpers

        private int ParseInput(InputField field)
        {
            return string.IsNullOrWhiteSpace(field.text) ? Default : int.Parse(field.text);
        }

        #endregion
    }
}