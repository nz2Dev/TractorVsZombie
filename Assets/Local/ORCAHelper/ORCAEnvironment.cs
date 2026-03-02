using UnityEngine;
using Nebukam.ORCA;
using Unity.Mathematics;
using System;
using System.Collections.Generic;

[Serializable]
public struct ObstacleData {
    public bool inverseOrder;
    public Vector3[] vertices;
}

public class ORCAEnvironment : MonoBehaviour {
    
    [SerializeField] List<ObstacleData> bakedObstacleData;

    public IReadOnlyList<ObstacleData> BakedData => bakedObstacleData;

    private void Start() {
        var system = ORCASystem.Instance;
        
        foreach (var bakedData in bakedObstacleData) {
            system.StaticObstacles.Add(ToFloat3Vertices(bakedData.vertices), bakedData.inverseOrder);
        }

        system.Recreate();
    }

    void OnDestroy() {
        // if domain reload disabled for enter/exit play mode
        ORCASystem.Instance.StaticObstacles.Clear();
        ORCASystem.Instance.DynamicObstacles.Clear();
    }

    internal void BakeObstacles() {
        bakedObstacleData.Clear();
        var obstacles = FindObjectsByType<ORCABoxObstacleTag>(FindObjectsSortMode.None);
        foreach (var obstacle in obstacles) {
            obstacle.GetBoxInfo(out var position, out var rotation, out var boxSize);
            bakedObstacleData.Add(new ObstacleData {
                vertices = ComputeBoxVerticies(position, rotation, boxSize * 0.5f),
                inverseOrder = true,
            });
        }
    }

    public Obstacle AddTemporalBoxObstacle(Vector3 position, Quaternion rotation, Vector3 boxSize) {
        var computedVerticies = ComputeBoxVerticies(position, rotation, boxSize * 0.5f);
        var boxObstacle = ORCASystem.Instance.DynamicObstacles.Add(ToFloat3Vertices(computedVerticies), inverseOrder: true);
        return boxObstacle;
    }

    public void RemoveTemporalObstacle(Obstacle orcaObstacle) {
        ORCASystem.Instance.DynamicObstacles.Remove(orcaObstacle);
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

    private static float3[] ToFloat3Vertices(Vector3[] vectorVertices) {
        var vertices3 = new float3[vectorVertices.Length];
        for (int i = 0; i < vectorVertices.Length; i++)
            vertices3[i] = vectorVertices[i];
        
        return vertices3;
    }

}