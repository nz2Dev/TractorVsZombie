using System;

using UnityEngine;

public readonly struct TowableVehicleId : IEquatable<TowableVehicleId> {
    public readonly int data;
    internal TowableVehicleId(int value) => data = value;
    public bool Equals(TowableVehicleId other) => data == other.data;
    public override bool Equals(object obj) => obj is TowableVehicleId other && Equals(other);
    public override int GetHashCode() => data.GetHashCode();
    public override string ToString() => data.ToString();
    public static bool operator ==(TowableVehicleId left, TowableVehicleId right) => left.Equals(right);
    public static bool operator !=(TowableVehicleId left, TowableVehicleId right) => !left.Equals(right);
}

public class TowableVehicleModel {

    private readonly TowableVehicleConfig config;

    public TowableVehicleId Id { get; }
    public int RamCombatId { get; set; }
    public Vector3 Position { get; set; }
    public int PhysicsId { get; set; }
    public VehicleState PhysicsPose { get; set; }

    public TowableVehicleModel(TowableVehicleId id, Vector3 position, TowableVehicleConfig config) {
        Id = id;
        Position = position;
        this.config = config;
    }

    public VehiclePhysics PhysicsPrefab => config.physicsPrefab;
    public TowableVehicleVisuals VisualsPrefab => config.visualsPrefab;
    
}