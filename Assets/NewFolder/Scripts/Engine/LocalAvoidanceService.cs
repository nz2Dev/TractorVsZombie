using System;
using System.Collections.Generic;

using Nebukam.ORCA;

using Unity.Mathematics;

using UnityEngine;

[Serializable]
public struct AgentAvoidanceConfig {
    public float height; // = 0.5;
    public float radius; // = 0.5f;
    public float radiusObst; // = 0.5f;
    public float maxSpeed; // = 20.0f;
    public int maxNeighbors; // = 15;
    public float neighborDist; // = 20.0f;
    public float timeHorizon; // = 15.0f;
    public float timeHorizonObst; // = 1.2f;
}

public class LocalAvoidanceService {

    private int nextId = 0;
    private readonly Dictionary<int, Agent> agentRegistry = new();

    private int obstacleIdCounter;
    private readonly Dictionary<int, Obstacle> obstacleRegistry = new();

    public LocalAvoidanceService() {
    }

    public int AddObstacle(Vector3 position, Quaternion rotation, PhysicsObstacle prefab) {
        var nextObstacleId = ++obstacleIdCounter;
        
        var computedVerticies = ObstaclesConverter.ComputeBoxVerticies(position, rotation, prefab.bakedSize * 0.5f);
        var boxObstacle = ORCASystem.Instance.DynamicObstacles.Add(ObstaclesConverter.ToFloat3Vertices(computedVerticies), inverseOrder: true);
        obstacleRegistry[nextObstacleId] = boxObstacle;

        return nextObstacleId;
    }

    public void RemoveObstacle(int obstacleId) {
        obstacleRegistry.Remove(obstacleId, out var orcaObstacle);
        ORCASystem.Instance.DynamicObstacles.Remove(orcaObstacle);
    }

    public virtual int AddAgent(Vector3 initPosition) {
        return AddAgent(initPosition, new AgentAvoidanceConfig {
            height = 0.5f,
            radius = 0.3f,
            radiusObst = 0.5f,
            maxSpeed = 20.0f,
            maxNeighbors = 15,
            neighborDist = 20,
            timeHorizon = 1.5f,
            timeHorizonObst = 2.5f
        });
    }
    
    public virtual int AddAgent(Vector3 initPosition, AgentAvoidanceConfig config) {
        var newAgent = ORCASystem.Instance.Agents.Add(initPosition);
        agentRegistry.Add(nextId, newAgent);
        var id = nextId++;
        UpdateAgent(id, config);
        return id;
    }

    public void UpdateAgent(int agentId, AgentAvoidanceConfig config) {
        var newAgent = agentRegistry[agentId];
        newAgent.height = config.height;
        newAgent.radius = config.radius;
        newAgent.radiusObst = config.radiusObst;
        newAgent.maxSpeed = config.maxSpeed;
        newAgent.maxNeighbors = config.maxNeighbors;
        newAgent.neighborDist = config.neighborDist;
        newAgent.timeHorizon = config.timeHorizon;
        newAgent.timeHorizonObst = config.timeHorizonObst;
    }

    public void RemoveAgent(int agentId) {
        var agent = agentRegistry[agentId];
        ORCASystem.Instance.Agents.Remove(agent);
        agentRegistry.Remove(agentId);
    }

    public virtual void SetAgentPosition(int agentId, Vector3 position) {
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

    public virtual Vector3 GetVelocity(int agentId) {
        return agentRegistry[agentId].velocity;
    }

    public virtual void SetPreferedVelocity(int agentId, Vector3 preferedVelocity) {
        var agent = agentRegistry[agentId];
        agent.prefVelocity = preferedVelocity;
    }

    public void SetMaxSpeed(int agentId, float maxSpeed) {
        var agent = agentRegistry[agentId];
        agent.maxSpeed = maxSpeed;
    }

}