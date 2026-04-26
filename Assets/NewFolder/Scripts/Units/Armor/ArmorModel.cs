
using UnityEngine;

public class ArmorModel {

    public ArmorConfig Config { get; }

    public ArmorModel(int id, Vector3 position, ArmorConfig config) {
        Id = id;
        Position = position;
        this.Config = config;
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

    public VehicleDrivingConfig DrivingConfig => Config.drivingConfig;
    public ArmorVisuals VisualsPrefab => Config.visualsPrefab;
    public VehiclePhysics PhysicsPrefab => Config.physicsPrefab;
    public AudioClip EngineLoopSFX => Config.engineLoopSFX;

    public WeaponConfig WeaponConfig => Config.weaponConfig;
    public RamEffectConfig RamConfig => Config.ramConfig;

}