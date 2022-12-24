using System.Collections.Generic;
using System.Numerics;
using RasHack.GapOverlap.Main.Stimuli;
using Tobii.Research;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    [System.Serializable]
    public class SampledTest
    {

        public string Name;

        public string TestId;

        public bool TestCompleted;

        public Samples Samples = new Samples();
    }


    [System.Serializable]
    public class Samples
    {
        public List<Sample> AllSamples = new List<Sample>();
    }

    [System.Serializable]
    public class Sample
    {
        public float Time;

        public SampledTask Task;

        public SampledTracker Tracker;
    }

    [System.Serializable]
    public class SampledTask
    {
        public int TaskOrder;

        public string TaskType;

        public string Side;

        public string StimulusType;

        public RawPosition CenterStimulus;

        public RawPosition PeripheralStimulus;
    }


    [System.Serializable]
    public class SampledTracker
    {
        public SampledGaze LeftEye;

        public SampledGaze RightEye;
    }

    [System.Serializable]
    public class SampledGaze
    {
        public string Validity;

        public RawPoint PositionOnDisplayArea;

        public SampledGaze(GazePoint point)
        {
            PositionOnDisplayArea = new RawPoint { X = point.PositionOnDisplayArea.X, Y = point.PositionOnDisplayArea.Y };
            Validity = point.Validity.ToString();
        }

        public SampledGaze()
        {
        }
    }
}