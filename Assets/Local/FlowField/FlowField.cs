using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Flags assigned to integration cells during tile construction.
/// These flags control how the flow field and LOS pass interact.
/// </summary>
[Flags]
public enum CellFlags : byte {
    None = 0,
    // 1 << 0 is vacant, reserved for future use
    /// <summary>Cell has unobstructed line-of-sight to the goal.</summary>
    HasLineOfSight = 1 << 1,
    /// <summary>Cell was reached by a Bresenham ray that was then blocked (corner shadow boundary).</summary>
    WaveFrontBlocked = 1 << 2,
}

public class Cell {

    public int cost;
    public int integratedCost;
    public CellFlags flags;
    public Vector2Int flowVector;

    internal bool NoFlags() {
        return flags == CellFlags.None;
    }

    internal bool HasFlag(CellFlags flag) {
        return (flags & flag) != 0;
    }

    internal bool FlagsAre(CellFlags flag) {
        return flags == flag;
    }

    internal void SetBlockedCost() {
        cost = 255;
    }

    internal void SetDefaultCost() {
        cost = 1;
    }

    internal bool IsBlocked() {
        return cost == 255;
    }
}

public class FlowField : Grid2D<Cell> {

    public FlowField(int gridSize, IEnumerable<Vector2Int> blockedCells) : base(gridSize) {
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                this[x, y].SetDefaultCost();
            }
        }

        if (blockedCells != null) {
            foreach (var blocked in blockedCells) {
                if (IsInBounds(blocked)) {
                    this[blocked].SetBlockedCost();
                }
            }
        }
    }

    public void UpdateBlockedCells(IEnumerable<Vector2Int> cells) {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                this[x, y].SetDefaultCost();


        if (cells == null)
            return;

        foreach (var cell in cells)
            if (IsInBounds(cell))
                this[cell].SetBlockedCost();
    }

}
