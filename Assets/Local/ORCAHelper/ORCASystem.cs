using Nebukam.ORCA;

using UnityEditor;

public class ORCASystem {

    public static ORCASystem Instance { get; set; }
    
    private ORCA orca;
    public AgentGroup<Agent> Agents { get; }
    public ObstacleGroup StaticObstacles { get; }
    public ObstacleGroup DynamicObstacles { get; }

    public ORCASystem() {
        Agents = new();
        StaticObstacles = new();
        DynamicObstacles = new();
    }

    public void Recreate() {
        orca?.Dispose();
        orca = new ORCA {
            plane = Nebukam.Common.AxisPair.XZ,
            agents = Agents,
            staticObstacles = StaticObstacles,
            dynamicObstacles = DynamicObstacles
        };
    }

    public void Tick(float deltaTime) {
        orca.Run(deltaTime);
    }

    public void Dispose() {
        orca?.Dispose();
        orca = null;
    }

}