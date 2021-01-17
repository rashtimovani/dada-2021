using System.Collections;
using System.Collections.Generic;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Task
{
    public class Gap : MonoBehaviour
    {
        [SerializeField] private GameObject stimulusPrefab;

        

        private Simulator simulator;
        private float spentTime;
        private Stimulus activeStimulus;

        // Start is called before the first frame update
        private void Start()
        {
            simulator = GetComponent<Simulator>();
        }

        // Update is called once per frame
        private void Update()
        {
            spentTime += Time.deltaTime;
            if (spentTime > 1 && activeStimulus == null)
            {
                activeStimulus = Instantiate(stimulusPrefab, Vector3.Lerp(simulator.Scaler.TopLeft, simulator.Scaler.BottomRight, 0.33f),
                    Quaternion.identity).GetComponent<Stimulus>();
            }
        }
    }
}