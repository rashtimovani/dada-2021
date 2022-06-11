using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEditor;
using UnityEngine;
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
#if UNITY_EDITOR
            simulator.FlushToDisk();
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

        #endregion
    }
}