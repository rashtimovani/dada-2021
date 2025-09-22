using System.Diagnostics;
using RasHack.GapOverlap.Main.Result;
using RasHack.GapOverlap.Main.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public class HomeUI : MonoBehaviour
    {
        #region Components

        [SerializeField] private GameObject panel;

        [SerializeField] private InputField nameInput;

        #endregion

        #region Fields

        private Simulator simulator;
        private MainSettingsUI mainSettings;

        private TestChooser testChooser;

        private bool hidden;

        #endregion

        #region API

        public void Show()
        {
            hidden = false;
            panel.SetActive(true);
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            simulator = FindObjectOfType<Simulator>();
            mainSettings = FindObjectOfType<MainSettingsUI>();
            testChooser = FindObjectOfType<TestChooser>();
            mainSettings.Hide(true);
            testChooser.Hide();
            nameInput.text = simulator.Settings.LastUsedName;
        }

        private void Update()
        {
            panel.SetActive(!hidden && !simulator.IsActive);
        }

        public void OnStart()
        {
            simulator.StartTests(nameInput.text);
        }

        public void OnExit()
        {
            simulator.FlushToDisk();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OnSettings()
        {
            mainSettings.Show();
            hidden = true;
            panel.SetActive(false);
        }

        public void OnResults()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo("explorer.exe", TestResults.RESULTS_DIRECTORY);
            p.Start();
        }

        public void OnReplayResults()
        {
            testChooser.Show();
            mainSettings.Hide(true);
            hidden = true;
            panel.SetActive(false); 
        }

        public void OnStartCalibration()
        {
            SceneManager.LoadScene(1); // Calibration scene;
        }

        #endregion
    }
}