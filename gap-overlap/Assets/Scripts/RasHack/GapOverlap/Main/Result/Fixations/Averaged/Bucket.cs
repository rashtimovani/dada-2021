using System;
using System.Collections.Generic;
using System.Linq;
using RasHack.GapOverlap.Main.Result.Fixations.CSV;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Stimuli;
using Unity.Mathematics;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Result.Fixations
{

    public class Bucket
    {
        #region Fields

        private readonly List<float> values = new List<float>();

        private float sum;
        private int p10Index;
        private int p25Index;
        private int p50Index;
        private int p80Index;
        private int p95Index;

        #endregion

        #region Properties

        public float Average => sum / values.Count;

        public float Min => GetOrNaN(0);
        public float P10 => GetOrNaN(p10Index);
        public float P25 => GetOrNaN(p25Index);
        public float P50 => GetOrNaN(p50Index);
        public float P80 => GetOrNaN(p80Index);
        public float P95 => GetOrNaN(p95Index);
        public float Max => GetOrNaN(values.Count - 1);

        #endregion

        #region API

        public void Add(float newValue)
        {
            values.Add(newValue);
            Refresh();
        }

        public void AddRange(List<float> newValues)
        {
            values.AddRange(newValues);
            Refresh();
        }

        public void AddRange(Bucket other)
        {
            values.AddRange(other.values);
            Refresh();
        }

        #endregion

        #region Helpers

        private void Refresh()
        {
            values.Sort();
            sum = values.Sum();

            p25Index = ToIndex(0.10f);
            p25Index = ToIndex(0.25f);
            p50Index = ToIndex(0.50f);
            p80Index = ToIndex(0.80f);
            p95Index = ToIndex(0.95f);
        }

        private int ToIndex(float percentile)
        {
            return Math.Min(values.Count - 1, Mathf.FloorToInt(percentile * values.Count));
        }

        private float GetOrNaN(int index)
        {
            return index < values.Count ? values[index] : float.NaN;
        }

        #endregion
    }
}