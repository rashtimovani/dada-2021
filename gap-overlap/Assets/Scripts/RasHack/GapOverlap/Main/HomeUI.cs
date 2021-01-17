using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class HomeUI : MonoBehaviour
    {
        #region Components

        [SerializeField] private GameObject homeMenu;

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
            simulator.StartTests();
        }

        #endregion
    }
}