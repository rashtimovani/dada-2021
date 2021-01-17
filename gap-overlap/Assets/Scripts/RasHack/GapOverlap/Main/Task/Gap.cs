using System;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    [Serializable]
    public struct GapTimes
    {
        public float CentralTime;
        public float PauseTime;
        public float StimulusTime;
    }

    public class Gap : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private GameObject stimulusPrefab;

        [SerializeField] private GapTimes times = new GapTimes {CentralTime = 1f, PauseTime = 0.5f, StimulusTime = 2f};

        #endregion

        #region Fields

        private Simulator simulator;
        private float spawnLifetime;

        private Stimulus activeStimulus;
        private StimuliType nextStimulusType;

        #endregion

        #region API

        public void ReportStimulusDied(Stimulus active)
        {
            if (active != activeStimulus)
            {
                Debug.LogError($"{active} stimulus is not the active one, don't care if it died!");
                return;
            }

            active = null;
            spawnLifetime = 0f;
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            simulator = GetComponent<Simulator>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (activeStimulus != null) return;

            spawnLifetime += Time.deltaTime;
            if (spawnLifetime > times.PauseTime)
            {
                activeStimulus = newStimulus();
                activeStimulus.StartSimulating(nextStimulusType, simulator, times.StimulusTime);
                nextStimulusType = nextStimulusType.next();
            }
        }

        #endregion

        #region Helpers

        private Stimulus newStimulus()
        {
            var where = Vector3.Lerp(simulator.Scaler.TopLeft, simulator.Scaler.BottomRight, 0.33f);
            var newOne = Instantiate(stimulusPrefab, where, Quaternion.identity);
            return newOne.GetComponent<Stimulus>();
        }

        #endregion
    }
}