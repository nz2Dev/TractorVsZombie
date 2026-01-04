using System;

using UnityEngine;

public readonly struct MotorVehicleId : IEquatable<MotorVehicleId> {
    public readonly int data;
    internal MotorVehicleId(int value) => data = value;
    public bool Equals(MotorVehicleId other) => data == other.data;
    public override bool Equals(object obj) => obj is MotorVehicleId other && Equals(other);
    public override int GetHashCode() => data.GetHashCode();
    public override string ToString() => data.ToString();
    public static bool operator ==(MotorVehicleId left, MotorVehicleId right) => left.Equals(right);
    public static bool operator !=(MotorVehicleId left, MotorVehicleId right) => !left.Equals(right);
}

public class MotorVehicleModel {
    
    private readonly MotorVehicleConfig config;

    public MotorVehicleId Id { get; }
    public Vector3 Position { get; set; }
    public int PhysicsId { get; set; }
    public int SoundSourceId { get; set; }
    public VehicleState PhysicsPose { get; set; }
    public float MotorPower { get; set; }
    public float SteeringDegrees { get; set; }
    public float BreaksPower { get; set; }

    public MotorVehicleModel(MotorVehicleId id, Vector3 position, MotorVehicleConfig config) {
        Id = id;
        Position = position;
        this.config = config;
    }

    public VehiclePhysics PhysicsPrefab => config.physicsPrefab;
    public MotorVehicleVisuals VisualsPrefab => config.visualsPrefab;
    public MotorVehicleConfig.DrivingData DrivingData => config.drivingData;

}