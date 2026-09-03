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

public class AvoidanceService {

    private int nextId = 0;
    private readonly Dictionary<int, Agent> agentRegistry = new();

    private int obstacleIdCounter;
    private readonly Dictionary<AvoidanceObstacleId, Obstacle> obstacleRegistry = new();

    private readonly List<float3> verticesReadBuffer = new (32);

    public AvoidanceService() {
    }

    public AvoidanceObstacleId AddObstacle(Vector3 position, Quaternion rotation, ORCAObstacleVertices verticesPrefab) {
        var nextObstacleId = new AvoidanceObstacleId(++obstacleIdCounter);
        
        verticesPrefab.ReadWorldVertices(verticesReadBuffer);
        var obstacle = ORCASystem.Instance.AddObstacle(isStatic: false, verticesPrefab.InverseORCAOrder, verticesReadBuffer);
        obstacleRegistry[nextObstacleId] = obstacle;
        return nextObstacleId;
    }

    public void RemoveObstacle(AvoidanceObstacleId obstacleId) {
        obstacleRegistry.Remove(obstacleId, out var orcaObstacle);
        ORCASystem.Instance.RemoveObstacle(orcaObstacle);
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
        var newAgent = ORCASystem.Instance.AddAgent(initPosition);
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
        ORCASystem.Instance.RemoveAgent(agent);
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