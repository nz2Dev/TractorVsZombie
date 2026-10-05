using System;
using System.Collections.Generic;

using Nebukam.Common;
using Nebukam.ORCA;

using Unity.Mathematics;
using UnityEngine;

/*
    ORCA - optimal reciprocal collision avoidance runtime for algorithm implemented by https://github.com/Nebukam/com.nebukam.orca.git
*/
public class ORCASystem {

    public static ORCASystem Instance { get; set; }
    
    private ORCA orca;
    internal AgentGroup<Agent> Agents { get; }
    internal ObstacleGroup StaticObstacles { get; }
    internal ObstacleGroup DynamicObstacles { get; }

    private readonly Dictionary<Obstacle, Vector3> dynamicObstaclePositions = new();

    private bool staticIsDirty;
    private bool firstTick;

    public ORCASystem() {
        Agents = new();
        StaticObstacles = new();
        DynamicObstacles = new();
        Recreate();
    }

    public Obstacle AddStaticObstacle(bool inverseOrder, IList<float3> vertices) {
        staticIsDirty = true;
        return StaticObstacles.Add(vertices, inverseOrder);
    }
    
    public void RemoveStaticObstacle(Obstacle obstacle) {
        StaticObstacles.Remove(obstacle);
        obstacle.Release();
        staticIsDirty = true;
    }

    public Obstacle AddDynamicObstacle(IList<float3> vertices, bool inverseOrder, Vector3 centerPosition) {
        var obstacle = DynamicObstacles.Add(vertices, inverseOrder);
        dynamicObstaclePositions.Add(obstacle, centerPosition);
        return obstacle;
    }

    public void MoveDynamicObstacle(Obstacle obstacle, Vector3 newPosition) {
        if (!dynamicObstaclePositions.TryGetValue(obstacle, out var lastPosition))
            return;

        var offset = (float3)(newPosition - lastPosition);
        obstacle.Offset(offset);
        dynamicObstaclePositions[obstacle] = newPosition;
    }

    public void RemoveDynamicObstacle(Obstacle obstacle) {
        if (!dynamicObstaclePositions.Remove(obstacle))
            return;

        DynamicObstacles.Remove(obstacle);
        obstacle.Release();
    }

    public Agent AddAgent(float3 position) {
        return Agents.Add(position);
    }

    public void RemoveAgent(Agent agent) {
        Agents.Remove(agent);
    }

    public void Tick(float deltaTime) {
        if (firstTick && staticIsDirty)
            Recreate();

        firstTick = true;
        staticIsDirty = false;
        orca.Run(deltaTime);
    }

    private void Recreate() {
        orca?.Dispose();
        orca = new ORCA {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = Agents,
            staticObstacles = StaticObstacles,
            dynamicObstacles = DynamicObstacles
        };
    }

    public void Dispose() {
        orca?.Dispose();
        orca = null;
    }

}
