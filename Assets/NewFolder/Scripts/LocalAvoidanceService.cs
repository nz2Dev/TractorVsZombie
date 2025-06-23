using System;
using System.Collections.Generic;

using UnityEngine;

public class LocalAvoidanceService {

    private int nextId = 0;
    private Dictionary<int, Vector3> agentPositions = new();
    private Vector3 initPosition;

    public int AddAgent(Vector3 initPosition) {
        agentPositions.Add(nextId, initPosition);
        this.initPosition = initPosition;
        return nextId++;
    }

    public Vector3 GetAgentPosition(int agentId) {
        return agentPositions[agentId];
    }
}