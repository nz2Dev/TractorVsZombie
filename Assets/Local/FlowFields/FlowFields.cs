using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class FlowFields {

    public class Cell {
        public int cost;
        public int integratedCost;
        public Vector2Int flowVector;
    }
    
    public static readonly Vector2Int[] CostNeighborsOffsets = new Vector2Int[] {
        new(0, -1),
        // new(+1, -1),
        new(+1, 0),
        // new(+1, +1),
        new(0, +1),
        // new(-1, +1),
        new(-1, 0),
        // new(-1, -1),
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

    private Grid2D<Cell> grid;

    public int CellCount => grid?.CellCount ?? 0;
    public int Size => grid?.Size ?? 0;

    public FlowFields() {
    }

    public void SetGrid(int size) {
        grid = new Grid2D<Cell>(size);
        for (int x = 0; x < grid.Size; x++) {
            for (int y = 0; y < grid.Size; y++) {
                grid[x, y].cost = 1;
            }
        }
    }

    public void SetCellBlocked(int x, int y, bool blocked) {
        grid[x, y].cost = blocked ? 255 : 0;
    }



    public bool IsCellBlocked(int x, int y) {
        var cellCost = grid[x, y].cost;
        return cellCost == 255;
    }

    public void ComputeCosts(Vector2Int goal) {
        // TODO: Bug - integratedCost != 0 check fails for cells with legitimate 0 cost
        // TODO: Performance - List.RemoveAt(0) is O(n), consider Queue<Vector2Int>
        grid[goal].integratedCost = 0;
        var inSearch = new List<Vector2Int>(capacity: 64) { goal };
        int safeCounter = 0;
        
        while (inSearch.Count > 0) {
            if (++safeCounter > grid.Size * 100) 
                throw new Exception("iterating over 1k times");
            
            var nextLocation = inSearch[0];
            var nextCell = grid[nextLocation];
            inSearch.RemoveAt(0);

            foreach (var offset in CostNeighborsOffsets) {
                var neighborLocation = nextLocation + offset;
                if (IsLocationOutsideBounds(neighborLocation) 
                    || IsCellBlocked(neighborLocation.x, neighborLocation.y)
                    || neighborLocation == goal)
                    continue;

                var neighborCell = grid[neighborLocation];
                if (neighborCell.integratedCost != 0)
                    continue;

                neighborCell.integratedCost = neighborCell.cost + nextCell.integratedCost;
                inSearch.Add(neighborLocation);
            }
        }
    }

    private bool IsLocationOutsideBounds(Vector2Int gridLocation) {
        return !grid.IsInBounds(gridLocation);
    }

    public int GetCost(int x, int y) {
        return grid[x, y].cost;
    }

    public int GetIntegratedCost(int x, int y) {
        return grid[x, y].integratedCost;
    }

    public void ComputeFlow() {
        for (int row = 0; row < grid.Size; row++) {
            for (int column = 0; column < grid.Size; column++) {
                var cellLocation = new Vector2Int(row, column);
                if (IsCellBlocked(row, column)) {
                    grid[row, column].flowVector = Vector2Int.zero;
                    continue;
                }
                
                var lowestCost = int.MaxValue;
                Vector2Int lowestCostLocation = cellLocation;
                foreach (var offset in FlowNeighborsOffsets) {
                    var neighborLocation = cellLocation + offset;
                    if (IsLocationOutsideBounds(neighborLocation)
                        || IsCellBlocked(neighborLocation.x, neighborLocation.y))
                        continue;

                    var neighborCell = grid[neighborLocation];
                    if (neighborCell.integratedCost < lowestCost) {
                        lowestCost = neighborCell.integratedCost;
                        lowestCostLocation = neighborLocation;
                    }
                }

                grid[row, column].flowVector = lowestCostLocation - cellLocation;
            }
        }
    }

    public Vector2Int GetFlowVector(int x, int y) {
        return grid[x, y].flowVector;
    }

}
