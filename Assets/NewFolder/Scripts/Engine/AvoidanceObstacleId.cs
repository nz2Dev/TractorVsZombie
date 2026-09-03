using System;

public readonly struct AvoidanceObstacleId : IEquatable<AvoidanceObstacleId> {
    public int Value { get; }
    public AvoidanceObstacleId(int value) => Value = value;
    public bool Equals(AvoidanceObstacleId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is AvoidanceObstacleId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(AvoidanceObstacleId left, AvoidanceObstacleId right) => left.Equals(right);
    public static bool operator !=(AvoidanceObstacleId left, AvoidanceObstacleId right) => !left.Equals(right);
}