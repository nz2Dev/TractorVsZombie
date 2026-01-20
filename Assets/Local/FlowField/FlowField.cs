using System;
using System.Collections.Generic;
using UnityEngine;

public class FlowField {

    internal class Cell {
        
        public int cost;
        public int integratedCost;
        public Vector2Int flowVector;

        internal void SetBlocked() {
            cost = 255;
        }

        internal bool IsBlocked() {
            return cost == 255;
        }
    }

    public static readonly Vector2Int[] CostNeighborsOffsets = new Vector2Int[] {
        new(0, -1),
        new(+1, 0),
        new(0, +1),
        new(-1, 0),
    };

    public static readonly Vector2Int[] FlowNeighborsOffsets = new Vector2Int[] {
        new(0, -1),
        new(+1, -1),
        new(+1, 0),
        new(+1, +1),
        new(0, +1),
        new(-1, +1),
        new(-1, 0),
        new(-1, -1),
    };

    private readonly Grid2D<Cell> grid;
    private Vector2Int nextGoal;

    public int Size => grid.Size;
    public Vector2Int NextGoal => nextGoal;

    public FlowField(int gridSize, IEnumerable<Vector2Int> blockedCells, Vector2Int goal) {
        this.grid = new Grid2D<Cell>(gridSize);
        this.nextGoal = goal;
        
        for (int x = 0; x < grid.Size; x++) {
            for (int y = 0; y < grid.Size; y++) {
                grid[x, y].cost = 1;
            }
        }

        if (blockedCells != null) {
            foreach (var blocked in blockedCells) {
                if (grid.IsInBounds(blocked)) {
                    grid[blocked].SetBlocked();
                }
            }
        }
    }

    public void SetNextGoal(Vector2Int location) {
        this.nextGoal = location;
    }

    public Vector2Int GetFlowVector(int x, int y) {
        return grid[x, y].flowVector;
    }

    public int GetIntegratedCost(int x, int y) {
        return grid[x, y].integratedCost;
    }

    public bool IsCellBlocked(int x, int y) {
        return grid[x, y].IsBlocked();
    }

    public void ComputeCosts() {
        grid[nextGoal].integratedCost = 0;

        var inSearch = new Queue<Vector2Int>();
        inSearch.Enqueue(nextGoal);
        
        int safeCounter = 0;
        int maxIterations = Size * Size * 2; 

        while (inSearch.Count > 0) {
            if (++safeCounter > maxIterations) 
                throw new Exception("Infinite loop detected in ComputeCosts");
            
            var nextLocation = inSearch.Dequeue();
            var nextCell = grid[nextLocation];

            foreach (var offset in CostNeighborsOffsets) {
                var neighborLocation = nextLocation + offset;
                if (!grid.IsInBounds(neighborLocation) || neighborLocation == nextGoal)
                    continue;

                var neighborCell = grid[neighborLocation];
                if (neighborCell.IsBlocked() || neighborCell.integratedCost != 0)
                    continue;

                neighborCell.integratedCost = neighborCell.cost + nextCell.integratedCost;
                inSearch.Enqueue(neighborLocation);
            }
        }
    }

    public void ComputeFlow() {
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                var cellLocation = new Vector2Int(x, y);
                var cell = grid[cellLocation];
                if (cell.IsBlocked()) {
                    grid[x, y].flowVector = Vector2Int.zero;
                    continue;
                }
                
                var lowestCost = int.MaxValue;
                Vector2Int lowestCostLocation = new Vector2Int(x, y);
                foreach (var offset in FlowNeighborsOffsets) {
                    var neighborLocation = cellLocation + offset;
                    if (!grid.IsInBounds(neighborLocation))
                        continue;
                    
                    var neighborCell = grid[neighborLocation];
                    if (neighborCell.IsBlocked() || neighborCell.integratedCost == 0 && neighborLocation != nextGoal)
                        continue;

                    if (neighborCell.integratedCost < lowestCost) {
                        lowestCost = neighborCell.integratedCost;
                        lowestCostLocation = neighborLocation;
                    }
                }

                grid[x, y].flowVector = lowestCostLocation - cellLocation;
            }
        }
    }

}
