using System;
using System.Collections.Generic;

using Nebukam.ORCA;

using UnityEngine;

public class LocalAvoidanceService {

    private int nextId = 0;
    private readonly ORCA orca;
    private readonly AgentGroup<Agent> agentsGroup = new();
    private readonly Dictionary<int, Agent> agentRegistry = new();

    public LocalAvoidanceService() {
        orca = new ORCA() {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = agentsGroup
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

    public void SimulateMovement(float deltaTime) {
        orca.Schedule(deltaTime);
        orca.Complete();
    }

    public void Release() {
        orca.DisposeAll();
    }

}