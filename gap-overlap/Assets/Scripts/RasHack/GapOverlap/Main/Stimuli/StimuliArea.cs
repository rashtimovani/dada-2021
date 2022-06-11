using System;
using RasHack.GapOverlap.Common;
using RasHack.GapOverlap.Main.Task;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public struct NextArea
    {
        public Vector3 Position;
        public float OffsetInDegrees;
        public string Side;
    }

    public class Side
    {
        #region Internals

        private readonly StimuliArea area;
        private readonly Vector3 direction;

        #endregion

        #region API

        public Side(StimuliArea area, Vector3 direction)
        {
            this.area = area;
            this.direction = direction;
        }

        public NextArea Point => area.Point(direction);

        #endregion
    }

    public class StimuliArea : MonoBehaviour
    {
        #region Internals

        private readonly Side left;
        private readonly Side right;
        private readonly ManagedRandom<Side> gapSides;
        private readonly ManagedRandom<Side> overlapSides;
        private readonly ManagedRandom<Side> baselineSides;

        private Simulator simulator;

        #endregion

        #region API

        public StimuliArea()
        {
            left = new Side(this, Vector3.left);
            right = new Side(this, Vector3.right);

            gapSides = new ManagedRandom<Side>();
            overlapSides = new ManagedRandom<Side>();
            baselineSides = new ManagedRandom<Side>();
        }

        public NextArea CenterInWorld =>
            new NextArea { Position = Scaler.Center, OffsetInDegrees = 0f, Side = "central" };

        public NextArea NextInWorld(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.Overlap:
                    return overlapSides.Next().Point;
                case TaskType.Baseline:
                    return baselineSides.Next().Point;
                default:
                    return gapSides.Next().Point;
            }
        }

        public void Reset(TaskCount taskCount)
        {
            var leftGaps = Math.Max(0, Math.Min(taskCount.Gaps, taskCount.LeftGaps));
            var rightGaps = Math.Max(taskCount.Gaps - leftGaps, 0);
            gapSides.SetOptions(new RandomOption<Side>(left, leftGaps), new RandomOption<Side>(right, rightGaps));

            var leftOverlaps = Math.Max(0, Math.Min(taskCount.Overlaps, taskCount.LeftOverlaps));
            var rightOverlaps = Math.Max(taskCount.Overlaps - leftOverlaps, 0);
            overlapSides.SetOptions(new RandomOption<Side>(left, leftOverlaps),
                new RandomOption<Side>(right, rightOverlaps));

            var leftBaselines = Math.Max(0, Math.Min(taskCount.Baselines, taskCount.LeftBaselines));
            var rightBaseline = Math.Max(taskCount.Baselines - leftBaselines, 0);
            baselineSides.SetOptions(new RandomOption<Side>(left, leftBaselines),
                new RandomOption<Side>(right, rightBaseline));
        }

        #endregion

        #region Mono methods

        private void Awake()
        {
            simulator = GetComponent<Simulator>();
        }

        #endregion

        #region Helpers

        private float Degrees => simulator.Settings.DistanceBetweenPeripheralStimuliInDegrees / 2;

        private Scaler Scaler => simulator.Scaler;

        private Vector3 Center => Scaler.ScreenCenter;

        public NextArea Point(Vector3 direction)
        {
            var position = Scaler.Point(Scaler.ScreenPosition(Center, Degrees, direction));
            var offsetInDegrees = direction.x * Degrees;
            var side = direction.x > 0 ? "right" : "left";
            return new NextArea { Position = position, OffsetInDegrees = offsetInDegrees, Side = side };
        }

        #endregion
    }
}