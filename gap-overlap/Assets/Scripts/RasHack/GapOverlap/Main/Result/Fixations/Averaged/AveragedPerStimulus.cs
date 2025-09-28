using System.Xml.Schema;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedPerStimulus
    {
        #region Fields

        private readonly Bucket duration = new Bucket();

        #endregion

        #region Properties

        public AveragedGazes Both { get; private set; } = new AveragedGazes();
        public AveragedGazes Left { get; private set; } = new AveragedGazes();
        public AveragedGazes Right { get; private set; } = new AveragedGazes();

        public float Duration => duration.Average;

        #endregion

        #region API

        public void Add(FixationPerStimulus perStimulus)
        {
            Both.Add(perStimulus.Both);
            Left.Add(perStimulus.Left);
            Right.Add(perStimulus.Right);

            duration.Add(perStimulus.Duration);
            
        }

        #endregion
    }
}