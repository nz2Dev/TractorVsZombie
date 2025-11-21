using UnityEngine;

public class VehicleModel {
    
    private readonly VehicleConfig config;

    public int Id { get; }
    public int PhysicsId { get; set; }
    public int SoundSourceId { get; set; } = -1;
    public VehicleState PhysicsPose { get; set; }
    public float MotorPower { get; set; }
    public float SteeringDegrees { get; set; }
    public float BreaksPower { get; set; }

    public VehicleModel(int id, Vector3 position, VehicleConfig config) {
        Id = id;
        PhysicsPose = new VehicleState { position = position };
        this.config = config;
    }

    public Vector3 Position => PhysicsPose.position;
    public Quaternion Rotation => PhysicsPose.rotation;
    public Vector3 Velocity => PhysicsPose.velocity;
    public VehiclePhysics PhysicsPrefab => config.physicsPrefab;
    public VehicleVisuals VisualsPrefab => config.visualsPrefab;
    public AudioClip EngineIdleSound => config.soundData.engineIdleSound;
    public DrivingData DrivingData => config.drivingData;
        
}