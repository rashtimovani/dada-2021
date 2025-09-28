using System.Collections.Generic;
using RasHack.GapOverlap.Main.Result.Fixations.CSV;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedPerStimulus
    {
        #region Properties

        public AveragedFixations Both { get; private set; } = new AveragedFixations();
        public AveragedFixations Left { get; private set; } = new AveragedFixations();
        public AveragedFixations Right { get; private set; } = new AveragedFixations();

        #endregion

        #region API

        public void Add(FixationPerStimulus perStimulus)
        {
            Both.Add(perStimulus.Both);
            Left.Add(perStimulus.Left);
            Right.Add(perStimulus.Right);
        }

        #endregion
    }
}