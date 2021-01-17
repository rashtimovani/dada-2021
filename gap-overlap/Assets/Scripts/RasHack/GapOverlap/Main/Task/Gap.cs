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

    public class Gap : MonoBehaviour, Task
    {
        #region Serialized fields

        [SerializeField] private GameObject centralStimulusPrefab;

        [SerializeField] private GameObject stimulusPrefab;

        [SerializeField] private GapTimes times = new GapTimes {CentralTime = 1f, PauseTime = 0.5f, StimulusTime = 2f};

        #endregion

        #region Fields

        private Simulator simulator;
        private float? waitingTime;

        private CentralStimulus centralStimulus;
        private Stimulus activeStimulus;
        private StimuliType stimulusType;

        #endregion

        #region API

        public Scaler Scaler => simulator.Scaler;

        public void ReportStimulusDied(Stimulus active)
        {
            if (active != activeStimulus)
            {
                Debug.LogError($"{active} stimulus is not the active one, don't care if it died!");
                return;
            }

            activeStimulus = null;
        }

        public void ReportCentralStimulusDied(CentralStimulus central)
        {
            if (central != centralStimulus)
            {
                Debug.LogError($"{central} stimulus is not the central one, don't care if it died!");
                return;
            }

            centralStimulus = null;
            waitingTime = times.PauseTime;
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            simulator = GetComponent<Simulator>();
            StartWithCentralStimulus();
        }

        private void Update()
        {
            if (!waitingTime.HasValue) return;
            waitingTime -= Time.deltaTime;
            if (waitingTime > 0f) return;
            waitingTime = null;
            StartWithStimulus();
        }

        #endregion

        #region Helpers

        private void StartWithCentralStimulus()
        {
            var newOne = Instantiate(centralStimulus, Scaler.Center, Quaternion.identity);
            centralStimulus = newOne.GetComponent<CentralStimulus>();
            centralStimulus.StartSimulating(this, times.CentralTime);
        }

        private void StartWithStimulus()
        {
            var where = Vector3.Lerp(simulator.Scaler.TopLeft, simulator.Scaler.BottomRight, 0.33f);
            var newOne = Instantiate(stimulusPrefab, where, Quaternion.identity);
            activeStimulus = newOne.GetComponent<Stimulus>();
            activeStimulus.StartSimulating(stimulusType, this, times.StimulusTime);
        }

        #endregion
    }
}