using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPro
{
    /// <summary>
    /// Phase C: Cost integration — fast marching wavefront expansion.
    /// </summary>
    public static class CostIntegrationPass
    {

        internal struct PassState 
        {
            public FlowTile Tile;
            public MinHeap<TrialNode> TrialHeap;
            public bool[] Accepted;
            public double[] TravelTimes;
        }

        /// <summary>
        /// Solves the integration field with the fast marching method using
        /// cardinal upwind differences.
        /// </summary>
        public static void IntegrateCosts(FlowTile tile, Queue<Vector2Int> wavefront)
        {
            var state = InitIntegateCosts(tile, wavefront);

            var trialHeap = state.TrialHeap;
            
            while (trialHeap.Count > 0)
            {
                StepIntegrateCosts(state);
            }
        }

        internal static PassState InitIntegateCosts(FlowTile tile, Queue<Vector2Int> wavefront) 
        {
            int w = tile.Width;
            int h = tile.Height;
            int cellCount = w * h;

            var accepted = new bool[cellCount];
            var travelTimes = new double[cellCount];
            var trialHeap = new MinHeap<TrialNode>(cellCount);

            for (int i = 0; i < travelTimes.Length; i++)
                travelTimes[i] = double.PositiveInfinity;

            // Queue entries are the requested FMM sources: goal cells, portal
            // transition cells, and LOS shadow-boundary cells from earlier passes.
            while (wavefront.Count > 0)
            {
                AddSource(tile, accepted, travelTimes, trialHeap, wavefront.Dequeue());
            }
            
            // count all line of sight cells as accepted and set their travel times to their best costs
            // for (int x = 0; x < w; x++)
            //     for (int y = 0; y < h; y++)
            //     {
            //         if (tile.Integration[x, y].Flags.HasFlag(CellFlags.HasLineOfSight))
            //         {
            //             accepted[ToIndex(x, y, w)] = true;
            //             travelTimes[ToIndex(x, y, w)] = tile.Integration[x, y].BestCost;
            //         }
            //     }

            return new PassState
            {
                Tile = tile,
                TrialHeap = trialHeap,
                Accepted = accepted,
                TravelTimes = travelTimes
            };
        }

        internal static void StepIntegrateCosts(PassState state)
        {
            var trialHeap = state.TrialHeap;
            var accepted = state.Accepted;
            var travelTimes = state.TravelTimes;
            var tile = state.Tile;

            int w = tile.Width;
            int h = tile.Height;

            var current = trialHeap.Pop();
            if (!ApproximatelyEqual(current.Cost, travelTimes[current.Index]))
                return;

            accepted[current.Index] = true;

            int currentX = current.Index % w;
            int currentY = current.Index / w;
            ref var currentCell = ref tile.Integration[currentX, currentY];
            currentCell.Flags &= ~CellFlags.ActiveWaveFront;

            foreach (var dir in Directions.Cardinal)
            {
                var offset = Directions.Offset(dir);
                int neighborX = currentX + offset.x;
                int neighborY = currentY + offset.y;

                if (neighborX < 0 || neighborX >= w || neighborY < 0 || neighborY >= h)
                    continue;

                if (tile.Cost.IsWall(neighborX, neighborY))
                    continue;

                int neighborIndex = ToIndex(neighborX, neighborY, w);
                if (accepted[neighborIndex] || (tile.Integration[neighborX, neighborY].Flags & CellFlags.HasLineOfSight) != 0)
                    continue;

                double newCost = ComputeFastMarchingCost(tile, accepted, travelTimes, neighborX, neighborY);
                if (double.IsInfinity(newCost) || newCost >= IntegrationField.Unreachable)
                    continue;

                if (newCost + double.Epsilon < travelTimes[neighborIndex])
                {
                    travelTimes[neighborIndex] = newCost;

                    ref var neighborCell = ref tile.Integration[neighborX, neighborY];
                    // neighborCell.BestCost = QuantizeCost(newCost);
                    neighborCell.BestCost = newCost;
                    neighborCell.Flags |= CellFlags.ActiveWaveFront;

                    trialHeap.Push(new TrialNode(neighborIndex, newCost));
                }
            }
        }

        internal static void FinishIntegrateCosts(PassState state)
        {
            var trialHeap = state.TrialHeap;
            while (trialHeap.Count > 0)
            {
                StepIntegrateCosts(state);
            }
        }

        private static void AddSource(
            FlowTile tile,
            bool[] accepted,
            double[] travelTimes,
            MinHeap<TrialNode> trialHeap,
            Vector2Int seed)
        {
            int w = tile.Width;
            int h = tile.Height;

            if (seed.x < 0 || seed.x >= w || seed.y < 0 || seed.y >= h)
                return;
            if (tile.Cost.IsWall(seed.x, seed.y))
                return;

            var seedCost = tile.Integration[seed.x, seed.y].BestCost;
            if (seedCost == IntegrationField.Unreachable)
                return;

            int seedIndex = ToIndex(seed.x, seed.y, w);
            // accepted[seedIndex] = true;

            if (seedCost < travelTimes[seedIndex])
            {
                travelTimes[seedIndex] = seedCost;
                trialHeap.Push(new TrialNode(seedIndex, seedCost));
            }
        }

        private static double ComputeFastMarchingCost(
            FlowTile tile,
            bool[] accepted,
            double[] travelTimes,
            int x,
            int y)
        {
            int w = tile.Width;
            double xCost = GetAcceptedAxisCost(accepted, travelTimes, w, x - 1, y, x + 1, y);
            double yCost = GetAcceptedAxisCost(accepted, travelTimes, w, x, y - 1, x, y + 1);
            double traversalCost = tile.Cost[x, y];

            if (double.IsInfinity(xCost))
                return double.IsInfinity(yCost) ? double.PositiveInfinity : yCost + traversalCost;
            if (double.IsInfinity(yCost))
                return xCost + traversalCost;

            double lower = Math.Min(xCost, yCost);
            double higher = Math.Max(xCost, yCost);
            double difference = higher - lower;

            if (difference >= traversalCost)
                return lower + traversalCost;

            double discriminant = 2.0 * traversalCost * traversalCost - difference * difference;
            return (lower + higher + Math.Sqrt(discriminant)) * 0.5;
        }

        private static double GetAcceptedAxisCost(
            bool[] accepted,
            double[] travelTimes,
            int width,
            int ax,
            int ay,
            int bx,
            int by)
        {
            double best = double.PositiveInfinity;

            if (IsAccepted(accepted, width, ax, ay))
                best = travelTimes[ToIndex(ax, ay, width)];
            if (IsAccepted(accepted, width, bx, by))
                best = Math.Min(best, travelTimes[ToIndex(bx, by, width)]);

            return best;
        }

        private static bool IsAccepted(bool[] accepted, int width, int x, int y)
        {
            int height = accepted.Length / width;
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            int index = ToIndex(x, y, width);
            return accepted[index];
        }

        internal static int ToIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        // private static ushort QuantizeCost(double cost)
        // {
        //     if (cost >= IntegrationField.Unreachable)
        //         return IntegrationField.Unreachable;

        //     int quantized = Mathf.CeilToInt((float)cost);
        //     if (quantized < 0)
        //         return 0;
        //     if (quantized >= IntegrationField.Unreachable)
        //         return IntegrationField.Unreachable;

        //     return (ushort)quantized;
        // }

        private static bool ApproximatelyEqual(double a, double b)
        {
            return Math.Abs(a - b) <= 0.000001;
        }

        internal readonly struct TrialNode : IHeapNode
        {
            public readonly int Index;
            public readonly double Cost;

            public TrialNode(int index, double cost)
            {
                Index = index;
                Cost = cost;
            }

            readonly double IHeapNode.Cost => Cost;
        }
    }
}
