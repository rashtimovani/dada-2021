using System.Xml.Schema;

namespace RasHack.GapOverlap.Main.Result.Fixations
{
    public class AveragedPerStimulus
    {
        #region Fields

        private readonly Bucket duration = new Bucket();

        #endregion

        #region Properties

        public AveragedFixations Both { get; private set; } = new AveragedFixations();
        public AveragedFixations Left { get; private set; } = new AveragedFixations();
        public AveragedFixations Right { get; private set; } = new AveragedFixations();

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