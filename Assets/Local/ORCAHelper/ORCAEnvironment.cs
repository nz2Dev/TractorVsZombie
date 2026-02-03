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

    private ORCA orca;
    private AgentGroup<Agent> agentsGroup;
    private ObstacleGroup staticObstacles;
    private ObstacleGroup dynamicObstacles;

    private void Awake() {
        agentsGroup = new();
        staticObstacles = new();
        dynamicObstacles = new();
        orca = new ORCA() {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = agentsGroup,
            staticObstacles = staticObstacles,
            dynamicObstacles = dynamicObstacles
        };

        ORCADebuger.Debug(orca);
    }

    private void Start() {
        staticObstacles.Clear();
        foreach (var bakedData in bakedObstacleData) {
            staticObstacles.Add(ToFloat3Vertices(bakedData.vertices), bakedData.inverseOrder);
        }
    }

    internal void BakeObstacles() {
        bakedObstacleData.Clear();
        var obstacles = FindObjectsByType<ORCAObstacle>(FindObjectsSortMode.None);
        foreach (var obstacle in obstacles) {
            obstacle.GetBoxInfo(out var position, out var rotation, out var boxSize);
            bakedObstacleData.Add(new ObstacleData {
                vertices = ComputeBoxVerticies(position, rotation, boxSize * 0.5f),
                inverseOrder = true,
            });
        }
    }

    public Obstacle AddBoxObstacle(Vector3 position, Quaternion rotation, Vector3 boxSize) {
        var computedVerticies = ComputeBoxVerticies(position, rotation, boxSize * 0.5f);
        return dynamicObstacles.Add(ToFloat3Vertices(computedVerticies), inverseOrder: true);
    }

    public void RemoveObstacle(Obstacle orcaObstacle) {
        dynamicObstacles.Remove(orcaObstacle);
    }

    private void OnDestroy() {
        orca.DisposeAll();
    }

    private void Update() {
        orca.Schedule(Time.deltaTime);
        orca.Complete();
    }

    public Agent AddAgent(Vector3 position) {
        return agentsGroup.Add(position);
    }

    public void RemoveAgent(Agent agent) {
        agentsGroup.Remove(agent);
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