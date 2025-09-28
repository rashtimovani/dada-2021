using System.Numerics;

namespace RasHack.GapOverlap.Main.Result.Fixations.CSV
{
    public class FixationPerTaskCSV : DumpToCSV
    {
        #region Fields

        private readonly FixationPerStimulusCSV subCSV = new FixationPerStimulusCSV();

        #endregion

        #region API

        public string CSVHeader()
        {
            return $"{Quote("Order")},{Quote("Type")},{Quote("Side")},{subCSV.CSVHeader("Central")},{subCSV.CSVHeader("Peripheral")}";
        }

        public string ToCSV(FixationPerTask perTask)
        {
            var order = perTask.TaskOrder;
            var type = perTask.Type.ToString();
            var side = perTask.Side.ToString();
            var central = subCSV.ToCSV(perTask.Central);
            var peripheral =subCSV.ToCSV(perTask.Peripheral);
            return $"{order},{Quote(type)},{Quote(side)},{central},{peripheral}";
        }

        #endregion
    }
}