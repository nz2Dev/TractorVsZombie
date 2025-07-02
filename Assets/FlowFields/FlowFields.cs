using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class FlowFields {

    public class Cell {
        public int cost;
        public int integratedCost;
    }
    
    private int size;
    private Cell[,] cells;

    public int CellCount => cells.Length;

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

    public Vector3 GetFlowVector(int x, int y) {
        return Vector3.zero;
    }

    public void SetCellBlocked(int x, int y, bool blocked) {
        cells[x, y].cost = blocked ? 255 : 0;
    }

    public bool IsCellBlocked(int x, int y) {
        var cellCost = cells[x, y].cost;
        return cellCost == 255;
    }

    public Vector2Int[] GetNeightbors(int x, int y) {
        return new Vector2Int[] {
            new(x, y - 1),
            new(x + 1, y - 1),
            new(x + 1, y),
            new(x + 1, y + 1),
            new(x, y + 1),
            new(x - 1, y + 1),
            new(x - 1, y),
            new(x - 1, y - 1),
        };
    }

    public void ComputeCosts(Vector2Int goal) {
        cells[goal.x, goal.y].integratedCost = 0;
        var inSearch = new List<Vector2Int> { goal };
        int safeCounter = 0;
        
        while (inSearch.Count > 0) {
            if (++safeCounter > size * 100) 
                throw new Exception("iterating over 1k times");
            
            var nextLocation = inSearch[0];
            var nextCell = cells[nextLocation.x, nextLocation.y];
            inSearch.RemoveAt(0);

            var neighbors = GetNeightbors(nextLocation.x, nextLocation.y);
            foreach (var neighborLocation in neighbors) {
                if (neighborLocation.x < 0 || neighborLocation.x >= size
                    || neighborLocation.y < 0 || neighborLocation.y >= size
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

    public int GetIntegratedCost(int x, int y) {
        return cells[x, y].integratedCost;
    }
}
