using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class FileChooser : MonoBehaviour
    {
        #region Unity fields

        [SerializeField] private GameObject panel;

        #endregion

        #region Methods

        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }

        #endregion
    }
}