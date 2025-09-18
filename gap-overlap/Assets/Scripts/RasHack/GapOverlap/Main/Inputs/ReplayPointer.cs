using UnityEngine;

namespace RasHack.GapOverlap.Main.Inputs
{
    public class ReplayPointer : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private Eye eye;

        #endregion

        #region API

        public Eye Eye => eye;

        #endregion
    }
}