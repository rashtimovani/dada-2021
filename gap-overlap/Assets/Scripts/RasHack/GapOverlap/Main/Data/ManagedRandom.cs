using System;
using System.Collections.Generic;
using System.Linq;

namespace RasHack.GapOverlap.Common
{
    #region Helper structure

    public class RandomOption<T>
    {
        #region Properties

        public T Option { get; }
        public int Count { get; private set; }

        #endregion

        #region API

        public RandomOption(T option, int count)
        {
            Option = option;
            Count = count;
        }

        public bool IsDone => Count == 0;

        public T Drain()
        {
            Count--;
            return Option;
        }

        public void Stop()
        {
            Count = 0;
        }

        #endregion
    }

    #endregion

    public class ManagedRandom<T>
    {
        #region Fields

        private readonly Random random = new Random();
        private readonly List<RandomOption<T>> initial = new List<RandomOption<T>>();
        private readonly List<RandomOption<T>> remaining = new List<RandomOption<T>>();

        #endregion

        #region API

        public ManagedRandom(params RandomOption<T>[] options)
        {
            SetOptions(options);
        }

        public bool HasNext => Maintain();

        public void SetOptions(params RandomOption<T>[] options)
        {
            foreach (var option in options)
            {
                initial.Add(new RandomOption<T>(option.Option, option.Count));
            }

            Reset();
        }

        public void End()
        {
            remaining.Clear();
        }

        public T Next()
        {
            if (!HasNext) Reset();

            var randomized = remaining.OrderBy(o => random.Next()).GetEnumerator();
            randomized.MoveNext();
            var next = randomized.Current.Drain();
            randomized.Dispose();
            Maintain();
            return next;
        }

        #endregion

        #region Helpers

        private bool Maintain()
        {
            remaining.RemoveAll(o => o.IsDone);
            return remaining.Count > 0;
        }

        private void Reset()
        {
            remaining.Clear();
            foreach (var option in initial)
            {
                if (!option.IsDone) remaining.Add(new RandomOption<T>(option.Option, option.Count));
            }
        }

        #endregion
    }
}