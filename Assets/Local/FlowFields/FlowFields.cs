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

    private int size;
    private Cell[,] cells;

    public int CellCount => cells.Length;
    public int Size => size;

    public FlowFields() {
    }

    public void SetGrid(int size) {
        this.size = size;
        cells = new Cell[size, size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                cells[i, j] = new Cell {
                    cost = 1,
                };
            }
        }
    }

    public void SetCellBlocked(int x, int y, bool blocked) {
        cells[x, y].cost = blocked ? 255 : 0;
    }

    private static Vector2Int[] RadiusCorners = new Vector2Int[] {
        new (-1, -1),
        new (0, -1),
        new (1, -1),
        new (1, 0),
        new (1, 1),
        new (0, 1),
        new (-1, 1),
        new (-1, 0)
    };
    
    // private static Vector2Int[] RadiusCorners = new Vector2Int[] {
    //     new (0, -1),
    //     new (1, 0),
    //     new (0, 1),
    //     new (-1, 0)
    // };

    private static Vector2Int[] RadiusDirections = new Vector2Int[] {
        new (1, 0),
        new (1, 0),
        new (0, 1),
        new (0, 1),
        new (-1, 0),
        new (-1, 0),
        new (0, -1),
        new (0, -1),
    };
    
    // private static Vector2Int[] RadiusDirections = new Vector2Int[] {
    //     new (1, 1),
    //     new (-1, 1),
    //     new (-1, -1),
    //     new (1, -1),
    // };

    public void RaiseNeighborsCost(int x, int y, int radius, int cost) {
        Assert.IsFalse(radius == 0);
        var location = new Vector2Int(x, y);
        for (int segment = 0; segment < RadiusCorners.Length * radius; segment++) {
            var corner = segment / radius;
            var step = segment % radius;
            var offset = RadiusCorners[corner] * radius + RadiusDirections[corner] * step;
            var neighborLocation = location + offset;
            
            if (IsLocationOutsideBounds(neighborLocation) || IsCellBlocked(neighborLocation.x, neighborLocation.y))
                continue;

            var neighborCell = cells[neighborLocation.x, neighborLocation.y];
            neighborCell.cost = Mathf.Max(neighborCell.cost, cost);
        }
    }

    public bool IsCellBlocked(int x, int y) {
        var cellCost = cells[x, y].cost;
        return cellCost == 255;
    }

    public void ComputeCosts(Vector2Int goal) {
        cells[goal.x, goal.y].integratedCost = 0;
        var inSearch = new List<Vector2Int>(capacity: 64) { goal };
        int safeCounter = 0;
        
        while (inSearch.Count > 0) {
            if (++safeCounter > size * 100) 
                throw new Exception("iterating over 1k times");
            
            var nextLocation = inSearch[0];
            var nextCell = cells[nextLocation.x, nextLocation.y];
            inSearch.RemoveAt(0);

            foreach (var offset in CostNeighborsOffsets) {
                var neighborLocation = nextLocation + offset;
                if (IsLocationOutsideBounds(neighborLocation) 
                    || IsCellBlocked(neighborLocation.x, neighborLocation.y)
                    || neighborLocation == goal)
                    continue;

                var neighborCell = cells[neighborLocation.x, neighborLocation.y];
                if (neighborCell.integratedCost != 0)
                    continue;

                neighborCell.integratedCost = neighborCell.cost + nextCell.integratedCost;
                inSearch.Add(neighborLocation);
            }
        }
    }

    private bool IsLocationOutsideBounds(Vector2Int gridLocation) {
        return gridLocation.x < 0 || gridLocation.x >= size
            || gridLocation.y < 0 || gridLocation.y >= size;
    }

    public int GetCost(int x, int y) {
        return cells[x, y].cost;
    }

    public int GetIntegratedCost(int x, int y) {
        return cells[x, y].integratedCost;
    }

    public void ComputeFlow() {
        for (int row = 0; row < size; row++) {
            for (int column = 0; column < size; column++) {
                var cellLocation = new Vector2Int(row, column);
                if (IsCellBlocked(row, column)) {
                    cells[row, column].flowVector = Vector2Int.zero;
                    continue;
                }
                
                var lowestCost = int.MaxValue;
                Vector2Int lowestCostLocation = cellLocation;
                foreach (var offset in FlowNeighborsOffsets) {
                    var neighborLocation = cellLocation + offset;
                    if (IsLocationOutsideBounds(neighborLocation)
                        || IsCellBlocked(neighborLocation.x, neighborLocation.y))
                        continue;

                    var neighborCell = cells[neighborLocation.x, neighborLocation.y];
                    if (neighborCell.integratedCost < lowestCost) {
                        lowestCost = neighborCell.integratedCost;
                        lowestCostLocation = neighborLocation;
                    }
                }

                cells[row, column].flowVector = lowestCostLocation - cellLocation;
            }
        }
    }

    public Vector2Int GetFlowVector(int x, int y) {
        return cells[x, y].flowVector;
    }

}
