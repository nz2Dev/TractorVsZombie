using System;
using System.Collections.Generic;
using System.Numerics;

using Nebukam.ORCA;

using Unity.Mathematics;

using UnityEditor;

/*
    ORCA - optimal reciprocal collision avoidance runtime for algorithm implemented by https://github.com/Nebukam/com.nebukam.orca.git
*/
public class ORCASystem {

    public static ORCASystem Instance { get; set; }
    
    private ORCA orca;
    internal AgentGroup<Agent> Agents { get; }
    internal ObstacleGroup StaticObstacles { get; }
    internal ObstacleGroup DynamicObstacles { get; }

    private bool staticIsDirty;
    private bool firstTick;

    public ORCASystem() {
        Agents = new();
        StaticObstacles = new();
        DynamicObstacles = new();
        Recreate();
    }

    public Obstacle AddObstacle(bool isStatic, bool inverseOrder, IList<float3> vertices) {
        var targetGroup = isStatic ? StaticObstacles : DynamicObstacles;
        if (isStatic) staticIsDirty = true;
        return targetGroup.Add(vertices, inverseOrder);
    }

    public void RemoveObstacle(Obstacle obstacle) {
        // might be in one of those lists
        StaticObstacles.Remove(obstacle);
        DynamicObstacles.Remove(obstacle);
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