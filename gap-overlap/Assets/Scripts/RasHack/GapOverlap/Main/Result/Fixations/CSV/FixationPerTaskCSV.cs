using System.Numerics;
using RasHack.GapOverlap.Main.Task;

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
            var peripheral = subCSV.ToCSV(perTask.Peripheral);
            return $"{order},{Quote(type)},{Quote(side)},{central},{peripheral}";
        }

        public string CSVHeaderAveraged(TaskType type)
        {
            return $"{Quote($"{type} total")},{Quote($"{type} succes rate")},{subCSV.CSVHeader($"{type} central")},{subCSV.CSVHeader($"{type} peripheral")}";
        }

        public string ToCSV(AveragedPerTask perTask)
        {
            var total = perTask.CountTotal.ToString();
            var successRate = perTask.SuccessRate.ToPercent();
            var central = subCSV.ToCSV(perTask.Central);
            var peripheral = subCSV.ToCSV(perTask.Peripheral);
            return $"{Quote(total)},{Quote(successRate)},{central},{peripheral}";
        }

        #endregion
    }
}