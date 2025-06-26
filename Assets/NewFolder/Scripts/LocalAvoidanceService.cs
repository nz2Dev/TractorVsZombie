using System;
using System.Collections.Generic;

using Nebukam.ORCA;

using Unity.Mathematics;

using UnityEngine;

public class LocalAvoidanceService {

    private int nextId = 0;
    private readonly ORCA orca;
    private readonly AgentGroup<Agent> agentsGroup = new();
    private readonly ObstacleGroup staticObstacles = new();
    private readonly Dictionary<int, Agent> agentRegistry = new();

    public LocalAvoidanceService() {
        orca = new ORCA() {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = agentsGroup,
            staticObstacles = staticObstacles
        };
    }

    public IEnumerable<int> AgentIds => agentRegistry.Keys;

    public int AddAgent(Vector3 initPosition) {
        var newAgent = agentsGroup.Add(initPosition);
        agentRegistry.Add(nextId, newAgent);
        return nextId++;
    }

    public Vector3 GetAgentPosition(int agentId) {
        return agentRegistry[agentId].pos;
    }

    public Vector3 GetVelocity(int agentId) {
        return agentRegistry[agentId].velocity;
    }

    public void SetPreferedVelocity(int agentId, Vector3 preferedVelocity) {
        var agent = agentRegistry[agentId];
        agent.prefVelocity = preferedVelocity;
    }

    public void SetMaxSpeed(int agentId, float maxSpeed) {
        var agent = agentRegistry[agentId];
        agent.maxSpeed = maxSpeed;
    }

    public void AddStaticBoxObstacle(Vector3 position, Quaternion rotation, Vector2 boxExtents) {
        var computedVerticies = ComputeBoxVerticies(position, boxExtents);
        staticObstacles.Add(computedVerticies, inverseOrder: true);
    }

    private float3[] ComputeBoxVerticies(float3 position, float2 boxExtents) {
        var verticies = new float3[4];
        var left = -boxExtents.x;
        var right = boxExtents.x;
        var forward = boxExtents.y;
        var backward = -boxExtents.y;
        verticies[0] = position + new float3(left, 0, backward);
        verticies[1] = position + new float3(left, 0, forward);
        verticies[2] = position + new float3(right, 0, forward);
        verticies[3] = position + new float3(right, 0, backward);
        return verticies;
    }

    public void CopyBoxObstaclePose(float3[] poseArray) {
        var obstacle = staticObstacles[0];
        for (int i = 0; i < obstacle.Count; i++) {
            poseArray[i] = obstacle[i];
        }
    }

    public void SimulateMovement(float deltaTime) {
        orca.Schedule(deltaTime);
        orca.Complete();
    }

    public void Release() {
        orca.DisposeAll();
    }

}