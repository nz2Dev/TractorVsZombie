using System;
using UnityEngine;

public struct FootstepGridCell {
    public Vector2Int index;
    public Vector3 averagePosition;
    public int requestsCount;
    public float minSpeed;
    public float maxSpeed;

    public void Clear() {
        averagePosition = Vector3.zero;
        requestsCount = 0;
        minSpeed = float.MaxValue;
        maxSpeed = float.MinValue;
    }

    public void AddUnit(Vector3 position, float speed) {
        averagePosition = (averagePosition * requestsCount + position) / (requestsCount + 1);
        requestsCount++;

        if (speed < minSpeed)
            minSpeed = speed;
        if (speed > maxSpeed)
            maxSpeed = speed;
    }
}

public class SpatialFootstepGrid {

    private readonly float cellSize;
    private readonly int gridWidth;
    private readonly int gridHeight;
    private readonly Vector3 worldOrigin;

    private readonly FootstepGridCell[] cells;
    private readonly int[] activeCellIndices;
    private readonly float[] activeCellDistancesSqr;

    private int activeCellCount;

    public SpatialFootstepGrid(float cellSize, int gridWidth, int gridHeight, Vector3 worldOrigin = default) {
        this.cellSize = cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        this.worldOrigin = worldOrigin;

        int totalCells = gridWidth * gridHeight;
        cells = new FootstepGridCell[totalCells];
        activeCellIndices = new int[totalCells];
        activeCellDistancesSqr = new float[totalCells];
        activeCellCount = 0;
    }

    public void ClearActiveRecords() {
        for (int i = 0; i < activeCellCount; i++) {
            int cellIdx = activeCellIndices[i];
            cells[cellIdx].Clear();
        }
        activeCellCount = 0;
    }

    public Vector2Int GetGridIndex(Vector3 position) {
        Vector3 localPos = position - worldOrigin;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int z = Mathf.FloorToInt(localPos.z / cellSize);
        return new Vector2Int(x, z);
    }

    public void AddRecord(Vector3 position, float speed) {
        var gridIdx = GetGridIndex(position);
        if (gridIdx.x < 0 || gridIdx.x >= gridWidth || gridIdx.y < 0 || gridIdx.y >= gridHeight)
            return;

        int flatIndex = gridIdx.x * gridHeight + gridIdx.y;
        if (cells[flatIndex].requestsCount == 0) {
            cells[flatIndex].index = gridIdx;
            activeCellIndices[activeCellCount] = flatIndex;
            activeCellCount++;
        }

        cells[flatIndex].AddUnit(position, speed);
    }

    public int GetSortedCells(Vector3 referencePoint, Span<FootstepGridCell> resultBuffer) {
        for (int i = 0; i < activeCellCount; i++) {
            int cellIdx = activeCellIndices[i];
            activeCellDistancesSqr[i] = (cells[cellIdx].averagePosition - referencePoint).sqrMagnitude;
        }

        Array.Sort(activeCellDistancesSqr, activeCellIndices, 0, activeCellCount);

        int countToCopy = Mathf.Min(resultBuffer.Length, activeCellCount);
        for (int i = 0; i < countToCopy; i++) {
            int cellIdx = activeCellIndices[i];
            resultBuffer[i] = cells[cellIdx];
        }

        return countToCopy;
    }

    public int ActiveCellCount => activeCellCount;
}
