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

public struct Cell {

    public static int DefaultCost = 1;
    public static int WallCost = 255;

    public int cost;
    public int integratedCost;
    public CellFlags flags;
    public Vector2Int flowVector;

    internal void SetFlag(CellFlags flag) {
        flags |= flag;
    }

    internal void UnsetFlag(CellFlags flag) {
        flags &= ~flag;
    }

    internal readonly bool NoFlags() {
        return flags == CellFlags.None;
    }

    internal readonly bool HasFlag(CellFlags flag) {
        return (flags & flag) != 0;
    }

    internal readonly bool FlagsAre(CellFlags flag) {
        return flags == flag;
    }

    internal void SetBlockedCost() {
        cost = 255;
    }

    internal void SetDefaultCost() {
        cost = DefaultCost;
    }

    internal readonly bool IsBlocked() {
        return cost == 255;
    }

}

public class FlowField : Grid2D<Cell> {

    public FlowField(int gridSize, IEnumerable<Vector2Int> blockedCells) : base(gridSize) {
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                this[x, y] = new Cell {
                    cost = Cell.DefaultCost
                };
            }
        }

        if (blockedCells != null) {
            foreach (var blocked in blockedCells) {
                if (IsInBounds(blocked)) {
                    this[blocked] = new Cell {
                        cost = Cell.WallCost,
                    };
                }
            }
        }
    }

    public void UpdateBlockedCells(IEnumerable<Vector2Int> cells) {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                this[x, y] = new Cell { cost = Cell.DefaultCost };


        if (cells == null)
            return;

        foreach (var cell in cells)
            if (IsInBounds(cell))
                this[cell] = new Cell { cost = Cell.WallCost };
    }

}
