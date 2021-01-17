﻿using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    public abstract class Task : MonoBehaviour
    {
        #region Prefabs

        [SerializeField] private GameObject centralStimulusPrefab;

        [SerializeField] private GameObject stimulusPrefab;

        #endregion

        #region Internals

        protected Simulator owner;
        protected StimuliType stimulusType;

        #endregion

        #region API

        protected Scaler Scaler => owner.Scaler;

        public void StartTask(Simulator owner, StimuliType stimulusType)
        {
            this.owner = owner;
            this.stimulusType = stimulusType;
        }

        public abstract void ReportFocusedOn(Stimulus stimulus, float after);

        public abstract void ReportCentralStimulusDied(CentralStimulus central);

        public abstract void ReportStimulusDied(Stimulus active);

        #endregion

        #region Instantiators

        protected CentralStimulus NewCentralStimulus()
        {
            var localWhere = transform.InverseTransformPoint(Scaler.Center);
            var newOne = Instantiate(centralStimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = name + "_central_stimulus";
            return newOne.GetComponent<CentralStimulus>();
        }

        protected Stimulus NewStimulus()
        {
            var localWhere = transform.InverseTransformPoint(owner.Area.NextInWorld);
            var newOne = Instantiate(stimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = name + "_" + stimulusType + "_stimulus";
            return newOne.GetComponent<Stimulus>();
        }

        #endregion
    }
}