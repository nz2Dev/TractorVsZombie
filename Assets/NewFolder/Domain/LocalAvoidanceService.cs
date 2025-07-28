using System;
using System.Collections.Generic;

using Nebukam.ORCA;

using Unity.Mathematics;

using UnityEngine;

public class LocalAvoidanceService {

    private int nextId = 0;
    private readonly ORCAEnvironment environment;
    
    private readonly Dictionary<int, Agent> agentRegistry = new();

    public LocalAvoidanceService(ORCAEnvironment environment) {
        this.environment = environment;
    }

    public IEnumerable<int> AgentIds => agentRegistry.Keys;

    public int AddAgent(Vector3 initPosition) {
        var newAgent = environment.AddAgent(initPosition);
        newAgent.timeHorizon = 1.5f;
        newAgent.timeHorizonObst = 2.5f;
        newAgent.radius = 0.3f;
        agentRegistry.Add(nextId, newAgent);
        return nextId++;
    }

    public void RemoveAgent(int agentId) {
        var agent = agentRegistry[agentId];
        environment.RemoveAgent(agent);
        agentRegistry.Remove(agentId);
    }

    public void SetAgentPosition(int agentId, Vector3 position) {
        var agent = agentRegistry[agentId];
        agent.pos = position;
    }

    public void SetAgentCollisionEnabled(int agentId, bool enabled) {
        var agent = agentRegistry[agentId];
        agent.collisionEnabled = enabled;
    }

    public Vector3 GetAgentPosition(int agentId) {
        return agentRegistry[agentId].pos;
    }

    public Quaternion GetAgentRotation(int agentId) {
        return Quaternion.LookRotation(agentRegistry[agentId].velocity);
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

}