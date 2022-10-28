using RasHack.GapOverlap.Main.Inputs;

namespace RasHack.GapOverlap.Main.Result
{
    public struct AllResponseTimes
    {
        public ResponseTime LeftEye;
        public ResponseTime RightEye;

        public bool AllCentralMeasured => LeftEye.CentralResponse.HasValue && RightEye.CentralResponse.HasValue;

        public bool AllPeripheralMeasured =>
            LeftEye.PeripheralResponse.HasValue && RightEye.PeripheralResponse.HasValue;

        public AllResponseTimes CentralMeasured(Eye eye, float measure)
        {
            switch (eye)
            {
                case Eye.Left:
                    return new AllResponseTimes() { LeftEye = LeftEye.CentralMeasured(measure), RightEye = RightEye };
                case Eye.Right:
                    return new AllResponseTimes() { LeftEye = LeftEye, RightEye = RightEye.CentralMeasured(measure) };
                default:
                    return this;
            }
        }

        public AllResponseTimes PeripheralMeasured(Eye eye, float measure)
        {
            switch (eye)
            {
                case Eye.Left:
                    return new AllResponseTimes()
                        { LeftEye = LeftEye.PeripheralMeasured(measure), RightEye = RightEye };
                case Eye.Right:
                    return new AllResponseTimes()
                        { LeftEye = LeftEye, RightEye = RightEye.PeripheralMeasured(measure) };
                default:
                    return this;
            }
        }
    }
}