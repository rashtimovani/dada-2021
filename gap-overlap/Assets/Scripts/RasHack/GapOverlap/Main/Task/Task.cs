using RasHack.GapOverlap.Main.Stimuli;
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
            var area = owner.Area.CenterInWorld;

            var localWhere = transform.InverseTransformPoint(area.Position);
            var newOne = Instantiate(centralStimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = name + "_" + area.Side + "_stimulus";

            var stimulus = newOne.GetComponent<CentralStimulus>();
            var sizeInDegrees = owner.Settings.CentralStimulusSizeInDegrees;
            var desiredSize = owner.Scaler.RealWorldSizeFromDegrees(sizeInDegrees, area.OffsetInDegrees);
            stimulus.Scale(desiredSize);

            return stimulus;
        }

        protected Stimulus NewStimulus()
        {
            var area = owner.Area.NextInWorld;
            
            var localWhere = transform.InverseTransformPoint(area.Position);
            var newOne = Instantiate(stimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = name + "_" + stimulusType + "_" + area.Side + "_stimulus";
            
            var stimulus = newOne.GetComponent<Stimulus>();
            var sizeInDegrees = owner.Settings.PeripheralStimulusSizeInDegrees;
            var desiredSize = owner.Scaler.RealWorldSizeFromDegrees(sizeInDegrees, area.OffsetInDegrees);
            stimulus.Scale(desiredSize);
            
            return stimulus;
        }

        #endregion
    }
}