using UnityEngine;
using Nebukam.ORCA;
using Unity.Mathematics;
using System;
using System.Collections.Generic;

[Serializable]
public struct ObstacleData {
    public bool inverseOrder;
    public Vector3[] vertices;

    public readonly float3[] ToFloat3Vertices() {
        var vertices3 = new float3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            vertices3[i] = vertices[i];
        
        return vertices3;
    }
}

public class ORCAEnvironment : MonoBehaviour {
    
    [SerializeField] List<ObstacleData> bakedObstacleData;

    private ORCA orca;
    private AgentGroup<Agent> agentsGroup;
    private ObstacleGroup staticObstacles;

    private void Awake() {
        agentsGroup = new();
        staticObstacles = new();
        orca = new ORCA() {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = agentsGroup,
            staticObstacles = staticObstacles,
        };

        ORCADebuger.Debug(orca);
    }

    private void Start() {
        staticObstacles.Clear();
        foreach (var bakedData in bakedObstacleData) {
            staticObstacles.Add(bakedData.ToFloat3Vertices(), bakedData.inverseOrder);
        }
    }

    internal void BakeObstacles() {
        bakedObstacleData.Clear();
        var obstacles = FindObjectsByType<ORCAObstacle>(FindObjectsSortMode.None);
        foreach (var obstacle in obstacles) {
            bakedObstacleData.Add(obstacle.ComputeObstacleData());
        }
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

}