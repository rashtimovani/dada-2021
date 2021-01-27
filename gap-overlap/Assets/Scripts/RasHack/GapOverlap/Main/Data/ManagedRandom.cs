using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class ManagedRandom<T>
    {
        #region Internals

        private int initialFirst;
        private int initialSecond;

        private int remainingFirst;
        private int remainingSecond;

        private readonly T first;
        private readonly T second;

        #endregion

        #region API

        public ManagedRandom(T first, T second)
        {
            this.first = first;
            this.second = second;
        }

        public bool HasNext => remainingFirst > 0 || remainingSecond > 0;

        public void Reset(int firstTotal, int secondTotal)
        {
            initialFirst = firstTotal;
            remainingFirst = firstTotal;

            initialSecond = secondTotal;
            remainingSecond = secondTotal;
        }

        public void End()
        {
            Reset(0, 0);
        }

        public T Next()
        {
            if (!HasNext) Reset(initialFirst, initialSecond);

            if (remainingFirst == 0)
            {
                remainingSecond--;
                return second;
            }

            var weighted = remainingFirst + remainingSecond;
            var rounded = Mathf.FloorToInt(Random.value * weighted);
            var next = rounded < remainingFirst ? first : second;

            if (next.Equals(first)) remainingFirst--;
            else remainingSecond--;

            return next;
        }

        #endregion
    }
}