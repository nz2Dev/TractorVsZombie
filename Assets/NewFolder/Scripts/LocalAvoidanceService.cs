using System;
using System.Collections.Generic;

using UnityEngine;

public class LocalAvoidanceService {

    class AgentState {
        public Vector3 Position { get; set; }
        public Vector3 PreferedVelocity { get; set; }
    }

    private int nextId = 0;
    private Dictionary<int, AgentState> agentStates = new();

    public int AddAgent(Vector3 initPosition) {
        agentStates.Add(nextId, new AgentState {
            Position = initPosition,
            PreferedVelocity = Vector3.zero,
        });
        return nextId++;
    }

    public Vector3 GetAgentPosition(int agentId) {
        return agentStates[agentId].Position;
    }

    public void SetPreferedVelocity(int agentId, Vector3 preferedVelocity) {
        var agentState = agentStates[agentId];
        agentState.PreferedVelocity = preferedVelocity;
    }

    public void SimulateMovement(float deltaTime) {
        foreach (var agentState in agentStates.Values) {
            agentState.Position += agentState.PreferedVelocity * deltaTime;
        }
    }
}