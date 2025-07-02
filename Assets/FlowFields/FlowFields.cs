using System;

using UnityEngine;

public class FlowFields {
    
    private int size;
    private bool[,] blockersGrid;

    public int CellCount => blockersGrid.Length;

    public FlowFields() {
    }

    public void SetGrid(int size) {
        this.size = size;
        blockersGrid = new bool[size, size];
    }

    public Vector3 GetFlowVector(int x, int y) {
        return Vector3.zero;
    }

    public void SetCellBlocked(int x, int y, bool blocked) {
        blockersGrid[x, y] = blocked; 
    }

    public int GetCellCost(int x, int y) {
        return blockersGrid[x, y] ? 255 : 1; 
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
}
