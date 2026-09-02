using System;

public readonly struct CollisionObstacleId : IEquatable<CollisionObstacleId> {
    public int Value { get; }
    public CollisionObstacleId(int value) => Value = value;
    public bool Equals(CollisionObstacleId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is CollisionObstacleId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(CollisionObstacleId left, CollisionObstacleId right) => left.Equals(right);
    public static bool operator !=(CollisionObstacleId left, CollisionObstacleId right) => !left.Equals(right);
}