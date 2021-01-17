using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class StimuliArea : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private Vector2 left = new Vector2(0.15f, 0.5f);

        [SerializeField] private Vector2 right = new Vector2(0.85f, 0.5f);

        #endregion

        #region Internals

        private Simulator simulator;
        private int count;

        #endregion

        #region API

        private Vector3 LeftInWorld => simulator.Scaler.InWorld(left);
        
        private Vector3 RightInWorld => simulator.Scaler.InWorld(right);

        public Vector3 NextInWorld => count++ % 2 == 0 ? LeftInWorld : RightInWorld;

        #endregion

        #region Mono methods

        private void Awake()
        {
            simulator = GetComponent<Simulator>();
        }

        #endregion
    }
}