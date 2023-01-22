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

        public AllResponseTimes CentralGotCloser(Eye eye, float distance)
        {
            switch (eye)
            {
                case Eye.Left:
                    return new AllResponseTimes()
                        { LeftEye = LeftEye.CentralGotCloser(distance), RightEye = RightEye };
                case Eye.Right:
                    return new AllResponseTimes()
                        { LeftEye = LeftEye, RightEye = RightEye.CentralGotCloser(distance) };
                default:
                    return this;
            }
        }

        public AllResponseTimes PeripheralGotCloser(Eye eye, float distance)
        {
            switch (eye)
            {
                case Eye.Left:
                    return new AllResponseTimes()
                        { LeftEye = LeftEye.PeripheralGotCloser(distance), RightEye = RightEye };
                case Eye.Right:
                    return new AllResponseTimes()
                        { LeftEye = LeftEye, RightEye = RightEye.PeripheralGotCloser(distance) };
                default:
                    return this;
            }
        }
    }
}