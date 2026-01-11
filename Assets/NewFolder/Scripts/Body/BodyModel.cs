using UnityEngine;

public class BodyModel {
    
    private readonly BodyConfig config;

    public BodyModel(int id, Vector3 position, BodyConfig config) {
        Id = id;
        Position = position;
        this.config = config;
        CanRecover = true;
    }

    public int Id { get; private set; }
    public int PhysicsId { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 DrivenVelocity { get; set; }
    
    public bool Grounded { get; set; }
    public bool CanRecover { get; set; }

    public BodyConfig.PhysicsData PhysicsData => config.physicsData;

}