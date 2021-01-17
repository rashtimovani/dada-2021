using RasHack.GapOverlap.Main.Stimuli;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class Simulator : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private SpriteRenderer pointer;
        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;

        [SerializeField] private bool showPointer;

        #endregion

        #region Tasks

        private Gap gap;

        #endregion

        #region Fields

        private Scaler scaler;
        private Camera mainCamera;

        #endregion

        #region API

        public Scaler Scaler => scaler;

        public void ReportStimulusDied(Stimulus active)
        {
            if (gap.enabled) gap.ReportStimulusDied(active);
        }

        public void ReportCentralStimulusDied(CentralStimulus central)
        {
            if (gap.enabled) gap.ReportCentralStimulusDied(central);
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            mainCamera = Camera.main;
            gap = GetComponent<Gap>();

            scaler = new Scaler(mainCamera, -1);

            topLeft.transform.position = transform.InverseTransformPoint(scaler.TopLeft);
            bottomLeft.transform.position = transform.InverseTransformPoint(scaler.BottomLeft);
            bottomRight.transform.position = transform.InverseTransformPoint(scaler.BottomRight);
            topRight.transform.position = transform.InverseTransformPoint(scaler.TopRight);
        }

        private void Update()
        {
            UpdateShownState();

            pointer.transform.position = scaler.point(Input.mousePosition);
        }

        #endregion

        #region Helpers

        private void UpdateShownState()
        {
            pointer.enabled = showPointer;

            bottomLeft.enabled = showPointer;
            bottomRight.enabled = showPointer;
            topLeft.enabled = showPointer;
            topRight.enabled = showPointer;
        }

        #endregion
    }
}