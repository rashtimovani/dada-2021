using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class ScalableStimulus : MonoBehaviour
    {
        #region Serialized field

        [SerializeField] private Transform bottomLeft;
        [SerializeField] private Transform topRight;

        #endregion

        #region API

        private Vector3 Size
        {
            get
            {
                var realSizeDiff = topRight.position - bottomLeft.position;
                return new Vector3(realSizeDiff.x, realSizeDiff.y, 1);
            }
        }

        public void Scale(Vector3 desiredSize)
        {
            var size = Size;
            var desiredFixedZ = new Vector3(desiredSize.x, desiredSize.y, 1);
            var scaleChange = new Vector3(desiredFixedZ.x / size.x, desiredFixedZ.y / size.y, desiredFixedZ.z / size.z);

            var originalScale = transform.localScale;
            transform.localScale = new Vector3(originalScale.x * scaleChange.x, originalScale.y * scaleChange.y,
                originalScale.z * scaleChange.z);
        }

        #endregion
    }
}