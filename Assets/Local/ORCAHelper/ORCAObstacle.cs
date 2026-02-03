using Unity.Mathematics;

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class ORCAObstacle : MonoBehaviour {
    
    private BoxCollider boxCollider;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    public ObstacleData ComputeObstacleData() {
        transform.GetPositionAndRotation(out var position, out var rotation);
        var boxSize = boxCollider.size;
        boxSize.Scale(transform.lossyScale);
        var computedVerticies = ComputeBoxVerticies(position, rotation, boxSize * 0.5f);
        return new ObstacleData {
            inverseOrder = true,
            vertices = computedVerticies,
        };
    }

    private static Vector3[] ComputeBoxVerticies(Vector3 position, Quaternion rotation, Vector3 halfSize) {
        var verticies = new Vector3[4];
        var left = -halfSize.x;
        var right = halfSize.x;
        var forward = halfSize.z;
        var backward = -halfSize.z;
        verticies[0] = position + rotation * new Vector3(left, 0, backward);
        verticies[1] = position + rotation * new Vector3(left, 0, forward);
        verticies[2] = position + rotation * new Vector3(right, 0, forward);
        verticies[3] = position + rotation * new Vector3(right, 0, backward);
        return verticies;
    }

}