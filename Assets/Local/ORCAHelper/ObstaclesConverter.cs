using Unity.Mathematics;

using UnityEngine;

public static class ObstaclesConverter {
    
    public static Vector3[] ComputeBoxVerticies(Vector3 position, Quaternion rotation, Vector3 halfSize) {
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

    public static float3[] ToFloat3Vertices(Vector3[] vectorVertices) {
        var vertices3 = new float3[vectorVertices.Length];
        for (int i = 0; i < vectorVertices.Length; i++)
            vertices3[i] = vectorVertices[i];
        
        return vertices3;
    }
}