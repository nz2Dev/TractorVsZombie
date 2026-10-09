using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct SpawnShape {
    
    public int width;
    public int height;
    public float spaceScale;
    public bool randomOffset;
    public float randomScale;

    public readonly int GetTotalPoints() {
        return width * height;
    }

    public readonly void CalculateSpawnPoints(List<Vector3> outputBuffer) {
        outputBuffer.Clear();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var gridPosition = new Vector3(x * spaceScale, 0, y * spaceScale);
                if (randomOffset) {
                    gridPosition += Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere * randomScale, Vector3.up);
                }
                outputBuffer.Add(gridPosition);
            };
        }
    }

}
