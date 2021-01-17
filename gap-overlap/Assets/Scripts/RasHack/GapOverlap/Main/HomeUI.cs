using UnityEngine;
using UnityEngine.UI;

namespace RasHack.GapOverlap.Main
{
    public class HomeUI : MonoBehaviour
    {
        #region Components

        [SerializeField] private GameObject homeMenu;
        [SerializeField] private InputField nameInput;

        #endregion

        #region Fields

        private Simulator simulator;

        #endregion

        #region Mono methods

        private void Start()
        {
            simulator = FindObjectOfType<Simulator>();
        }

        private void Update()
        {
            homeMenu.gameObject.SetActive(!simulator.IsActive);
        }

        public void OnStart()
        {
            simulator.StartTests(nameInput.text);
        }

        public void OnExit()
        {
#if UNITY_EDITOR
            simulator.FlushToDisk();
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion
    }
}