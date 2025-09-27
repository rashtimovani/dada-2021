using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Result;
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

        protected int taskOrder;

        protected Simulator owner;
        protected StimuliType stimulusType;
        private NextArea area;
        protected CentralStimulus centralStimulus;
        protected PeripheralStimulus peripheralStimulus;

        #endregion

        #region API

        public Simulator Owner => owner;

        public abstract TaskType TaskType { get; }

        public StimuliType StimulusType => stimulusType;

        public StimulusSide Side => area.Side;

        public int TaskOrder => taskOrder;

        public float FadeInOut => owner.Settings.FadeInOut;

        public int RotationFactor => owner.Settings.RotationFactor;

        public void StartTask(Simulator owner, StimuliType stimulusType, int taskOrder)
        {
            this.owner = owner;
            this.stimulusType = stimulusType;
            this.taskOrder = taskOrder;

            area = owner.Area.NextInWorld(TaskType);
            gameObject.name = gameObject.name + "_" + area.Side;
        }

        protected abstract void OnSuccessfulCentralFocus();

        protected abstract void OnSuccessfulPeripheralFocus();

        public abstract void ReportCentralStimulusDied(CentralStimulus central);

        public abstract void ReportPeripheralStimulusDied(PeripheralStimulus active);

        #endregion

        #region Mono methods

        protected abstract void OnDestroy();

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
            stimulus.Scale(desiredSize, sizeInDegrees);

            return stimulus;
        }

        protected PeripheralStimulus NewPeripheralStimulus()
        {
            var localWhere = transform.InverseTransformPoint(area.Position);
            var newOne = Instantiate(stimulusPrefab, localWhere, Quaternion.identity, transform);
            newOne.name = name + "_" + stimulusType + "_" + area.Side + "_stimulus";

            var stimulus = newOne.GetComponent<PeripheralStimulus>();
            var sizeInDegrees = owner.Settings.PeripheralStimulusSizeInDegrees;
            var desiredSize = owner.Scaler.RealWorldSizeFromDegrees(sizeInDegrees, area.OffsetInDegrees);
            stimulus.Scale(desiredSize, sizeInDegrees);

            return stimulus;
        }

        #endregion
    }
}