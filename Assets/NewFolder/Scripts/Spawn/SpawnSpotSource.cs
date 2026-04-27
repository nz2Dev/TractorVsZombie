using UnityEditor;

using UnityEngine;

public class SpawnSpotSource : MonoBehaviour {
    
    [SerializeField] private SpawnShape shape = new SpawnShape {
        height = 1,
        width = 1,
        spaceScale = 1,
        randomOffset = false,
        randomScale = 1,
    };

    public SpawnSpot Get() {
        return new SpawnSpot {
            position = transform.position,
            rotation = transform.rotation,
            shape = shape,
        };
    }

    private void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;
        var halfWidth = shape.width * 0.5f * shape.spaceScale;
        var halfHeight = shape.height * 0.5f * shape.spaceScale;
        Handles.DrawPolyLine(new Vector3[] {
            new(-halfWidth, 0, halfHeight),
            new(halfWidth, 0, halfHeight),
            new(halfWidth, 0, -halfHeight),
            new(-halfWidth, 0, -halfHeight),
            new(-halfWidth, 0, halfHeight),
        });
    }

}