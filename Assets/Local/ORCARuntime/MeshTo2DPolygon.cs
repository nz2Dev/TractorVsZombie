using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

public static class MeshTo2DPolygon {
    public static List<float3> ExtractXZHull(Mesh mesh) {
        var points = new List<float3>();

        // 1. Collect all vertices projected to XZ (y = 0)
        var vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++) {
            float3 vertex = vertices[i];
            points.Add(new float3(vertex.x, 0f, vertex.z));
        }

        // 2. Compute convex hull
        return ComputeHull(points);
    }

    // Monotone Chain Convex Hull
    private static List<float3> ComputeHull(List<float3> points) {
        if (points.Count <= 3)
            return new List<float3>(points);

        points.Sort((a, b) =>
            a.x == b.x ? a.z.CompareTo(b.z) : a.x.CompareTo(b.x));

        var lower = new List<float3>();
        foreach (var p in points) {
            while (lower.Count >= 2 &&
                   Cross(lower[^2], lower[^1], p) <= 0) {
                lower.RemoveAt(lower.Count - 1);
            }
            lower.Add(p);
        }

        var upper = new List<float3>();
        for (int i = points.Count - 1; i >= 0; i--) {
            var p = points[i];
            while (upper.Count >= 2 &&
                   Cross(upper[^2], upper[^1], p) <= 0) {
                upper.RemoveAt(upper.Count - 1);
            }
            upper.Add(p);
        }

        // Remove duplicates
        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);

        lower.AddRange(upper);
        return lower;
    }

    // Cross product on XZ plane
    private static float Cross(float3 a, float3 b, float3 c) {
        float abx = b.x - a.x;
        float abz = b.z - a.z;
        float acx = c.x - a.x;
        float acz = c.z - a.z;

        return abx * acz - abz * acx;
    }
}