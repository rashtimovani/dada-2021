namespace RasHack.GapOverlap.Main.Result
{
    public struct ResponseTime
    {
        public float? CentralResponse;
        public float? PeripheralResponse;

        public ResponseTime CentralMeasured(float central)
        {
            return new ResponseTime { CentralResponse = central, PeripheralResponse = PeripheralResponse };
        }

        public ResponseTime PeripheralMeasured(float peripheral)
        {
            return new ResponseTime { CentralResponse = CentralResponse, PeripheralResponse = peripheral };
        }
    }
}