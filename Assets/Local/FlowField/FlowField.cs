using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
        
    public int cost;
    public int integratedCost;
    public Vector2Int flowVector;

    internal void SetBlocked() {
        cost = 255;
    }

    internal void ClearBlocked() {
        cost = 1;
    }

    internal bool IsBlocked() {
        return cost == 255;
    }
}

public class FlowField : Grid2D<Cell> {

    public FlowField(int gridSize, IEnumerable<Vector2Int> blockedCells) : base(gridSize){
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                this[x, y].cost = 1;
            }
        }

        if (blockedCells != null) {
            foreach (var blocked in blockedCells) {
                if (IsInBounds(blocked)) {
                    this[blocked].SetBlocked();
                }
            }
        }
    }

    public void UpdateBlockedCells(IEnumerable<Vector2Int> cells) {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                this[x, y].ClearBlocked();    
        

        if (cells == null)
            return;

        foreach (var cell in cells) 
            if (IsInBounds(cell)) 
                this[cell].SetBlocked();
    }    

}
