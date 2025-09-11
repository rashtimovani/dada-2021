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
        private ReplayController replayController;
        private MainSettingsUI mainSettings;

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
            replayController = FindObjectOfType<ReplayController>();
            mainSettings = FindObjectOfType<MainSettingsUI>();
            mainSettings.Hide(true);
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
            replayController.StartReplay();
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