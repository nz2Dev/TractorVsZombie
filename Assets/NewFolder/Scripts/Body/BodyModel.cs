using UnityEngine;

public class BodyModel {
    
    private readonly BodyConfig config;

    public BodyModel(int id, Vector3 position, BodyConfig config) {
        Id = id;
        Position = position;
        this.config = config;
        Alive = true;
    }

    public int Id { get; private set; }
    public int AvoidanceId { get; set; }
    public int PhysicsId { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 PreferedVelocity { get; set; }
    public bool Grounded { get; set; }
    public bool Alive { get; set; }

    public BodyVisuals VisualsPrefab => config.visualsPrefab;
    public BodyConfig.PhysicsData PhysicsData => config.physicsData;
    public AgentAvoidanceConfig AvoidanceConfig => config.agentAvoidanceConfig;

}