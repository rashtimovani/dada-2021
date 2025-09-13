using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public class FileChooser : MonoBehaviour
    {
        #region Unity fields

        [SerializeField] private GameObject panel;

        [SerializeField] private TMP_Dropdown dropdown;

        [SerializeField] private InputField folderInput;

        private HomeUI home;

        #endregion

        #region Fields

        private int selectedItem;

        #endregion

        #region Methods

        private void Start()
        {
            home = FindObjectOfType<HomeUI>();
        }

        public void OnBack()
        {
            Hide();
            home.Show();
        }

        public void Show()
        {
            LoadFiles();
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }

        public void LoadFiles()
        {
            string folderPath = folderInput.text;
            string[] jsonFiles = System.IO.Directory.GetFiles(folderPath, "*.json", System.IO.SearchOption.AllDirectories);

            selectedItem = dropdown.value;

            dropdown.ClearOptions();
            // Example: print file paths to console
            foreach (string file in jsonFiles)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData() { text = file });
            }
            if (dropdown.options.Count > selectedItem)
            {
                dropdown.value = selectedItem;
            }
        }

        #endregion
    }
}