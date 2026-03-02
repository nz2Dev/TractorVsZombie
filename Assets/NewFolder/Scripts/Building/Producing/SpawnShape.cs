using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class SpawnShape : MonoBehaviour {
    
    public int width = 1;
    public int height = 1;
    public float spaceScale = 0.25f;
    public Vector3 centerOffset;
    public bool randomOffset = true;
    public float offsetScale = 0.5f;

    public int GetTotalPoints() {
        return width * height;
    }

    public void CalculateSpawnPoints(List<Vector3> outputBuffer) {
        outputBuffer.Clear();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var gridPosition = new Vector3(x * spaceScale, 0, y * spaceScale);
                if (randomOffset) {
                    gridPosition += Vector3.ProjectOnPlane(Random.onUnitSphere * offsetScale, Vector3.up);
                }
                outputBuffer.Add(centerOffset + gridPosition);
            };
        }
    }

    private void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;
        var halfWidth = width * 0.5f * spaceScale;
        var halfHeight = height * 0.5f * spaceScale;
        Handles.DrawPolyLine(new Vector3[] {
            centerOffset + new Vector3(-halfWidth, 0, halfHeight),
            centerOffset + new Vector3(halfWidth, 0, halfHeight),
            centerOffset + new Vector3(halfWidth, 0, -halfHeight),
            centerOffset + new Vector3(-halfWidth, 0, -halfHeight),
            centerOffset + new Vector3(-halfWidth, 0, halfHeight),
        });
    }

}
