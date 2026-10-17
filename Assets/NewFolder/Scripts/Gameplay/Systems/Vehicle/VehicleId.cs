using System;

public readonly struct VehicleId : IEquatable<VehicleId> {
    public int Value { get; }

    public VehicleId(int value) => Value = value;

    public bool Equals(VehicleId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is VehicleId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(VehicleId left, VehicleId right) => left.Equals(right);
    public static bool operator !=(VehicleId left, VehicleId right) => !left.Equals(right);
}
