using UnityEngine;

namespace RasHack.GapOverlap.Main.Result
{
    public struct ResponseTime
    {
        public float? CentralResponse;
        public float? CentralDistance;
        public float? PeripheralResponse;
        public float? PeripheralDistance;

        private bool IsCentralMeasured()
        {
            return CentralResponse.HasValue;
        }

        private bool IsPeripheralMeasured()
        {
            return PeripheralResponse.HasValue;
        }

        public ResponseTime CentralMeasured(float central)
        {
            return IsCentralMeasured()
                ? this
                : new ResponseTime
                {
                    CentralResponse = central,
                    CentralDistance = CentralDistance,
                    PeripheralResponse = PeripheralResponse,
                    PeripheralDistance = PeripheralDistance
                };
        }

        public ResponseTime PeripheralMeasured(float peripheral)
        {
            return IsPeripheralMeasured()
                ? this
                : new ResponseTime
                {
                    CentralResponse = CentralResponse,
                    CentralDistance = CentralDistance,
                    PeripheralResponse = peripheral,
                    PeripheralDistance = PeripheralDistance
                };
        }

        public ResponseTime CentralGotCloser(float central)
        {
            return !IsCentralMeasured()
                ? this
                : new ResponseTime
                {
                    CentralResponse = CentralResponse,
                    CentralDistance = Mathf.Min(central, CentralDistance.GetValueOrDefault(float.MaxValue)),
                    PeripheralResponse = PeripheralResponse,
                    PeripheralDistance = PeripheralDistance
                };
        }

        public ResponseTime PeripheralGotCloser(float peripheral)
        {
            return !IsPeripheralMeasured()
                ? this
                : new ResponseTime
                {
                    CentralResponse = CentralResponse,
                    CentralDistance = CentralDistance,
                    PeripheralResponse = PeripheralResponse,
                    PeripheralDistance = Mathf.Min(peripheral, PeripheralDistance.GetValueOrDefault(float.MaxValue)),
                };
        }
    }
}