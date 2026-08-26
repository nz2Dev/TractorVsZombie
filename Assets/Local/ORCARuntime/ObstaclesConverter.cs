using Unity.Mathematics;

using UnityEngine;

public static class ObstaclesConverter {
    
    public static float3[] ComputeBoxVerticies(Vector3 position, Quaternion rotation, Vector3 halfSize) {
        var verticies = new float3[4];
        var left = -halfSize.x;
        var right = halfSize.x;
        var forward = halfSize.z;
        var backward = -halfSize.z;
        verticies[0] = position + rotation * new float3(left, 0, backward);
        verticies[1] = position + rotation * new float3(left, 0, forward);
        verticies[2] = position + rotation * new float3(right, 0, forward);
        verticies[3] = position + rotation * new float3(right, 0, backward);
        return verticies;
    }

}