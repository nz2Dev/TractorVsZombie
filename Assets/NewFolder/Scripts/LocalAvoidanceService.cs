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
        ORCADebuger.Debug(orca);
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

    public void AddStaticBoxObstacle(Vector3 position, Quaternion rotation, Vector3 boxSize) {
        var computedVerticies = ComputeBoxVerticies(position, rotation, boxSize * 0.5f);
        staticObstacles.Add(computedVerticies, inverseOrder: true);
    }

    private float3[] ComputeBoxVerticies(Vector3 position, Quaternion rotation, float3 halfSize) {
        var verticies = new float3[4];
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

    public void SimulateMovement(float deltaTime) {
        orca.Schedule(deltaTime);
        orca.Complete();
    }

    public void Release() {
        orca.DisposeAll();
    }

}