
using UnityEngine;

public class ArmorModel {

    private readonly ArmorConfig config;

    public ArmorModel(int id, Vector3 position, ArmorConfig config) {
        Id = id;
        Position = position;
        this.config = config;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; set; }
    
    public int VehiclePhysicsId { get; set; }
    public VehicleState VehiclePhysicsState { get; set; }
    
    public float MotorTorque { get; set; }
    public float SteeringDegrees { get; set; }
    public float BrakesTorque { get; set; }

    public int CombatId { get; set; }
    public int WeaponId { get; set; }
    public bool Destroyed { get; set; }
    public int RamId { get; set; }

    public int MaxHealthConfig => config.maxHealth;
    
    public VehicleDrivingConfig DrivingConfig => config.drivingConfig;
    public ArmorVisuals VisualsPrefab => config.visualsPrefab;
    public VehiclePhysics PhysicsPrefab => config.physicsPrefab;
    public AudioClip EngineLoopSFX => config.engineLoopSFX;

    public WeaponConfig WeaponConfig => config.weaponConfig;
    public RamConfig RamConfig => config.ramConfig;

}