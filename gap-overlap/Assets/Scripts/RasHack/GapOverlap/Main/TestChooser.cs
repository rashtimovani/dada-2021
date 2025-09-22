using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public class TestChooser : MonoBehaviour
    {
        #region Unity fields

        [SerializeField] private GameObject panel;

        [SerializeField] private TMP_Dropdown dropdown;

        [SerializeField] private InputField folderInput;

        private HomeUI home;

        private ReplayController replayController;

        #endregion

        #region Fields

        private int selectedItem;

        #endregion

        #region Methods

        private void Start()
        {
            home = FindObjectOfType<HomeUI>();
            replayController = FindObjectOfType<ReplayController>();
        }

        public void OnBack()
        {
            Hide();
            home.Show();
        }

        public void OnChoose()
        {
            Hide();
            string folderPath = folderInput.text;
            replayController.StartReplay(dropdown.options[dropdown.value].text, folderPath);
        }

        public void OnAnalyzeAll()
        {
            Hide();
            string folderPath = folderInput.text;
            replayController.AnalyzeAllTests(folderPath);
        }

        public void Show()
        {
            LoadFiles();
            panel.SetActive(true);
        }

        public void Hide()
        {
            selectedItem = dropdown.value;
            panel.SetActive(false);
        }

        public void LoadFiles()
        {
            string folderPath = folderInput.text;
            string[] jsonFiles = System.IO.Directory.GetFiles(folderPath, "*.json", System.IO.SearchOption.AllDirectories);

            selectedItem = dropdown.value;

            dropdown.ClearOptions();
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