using System;

public readonly struct PathfindingObstacleId : IEquatable<PathfindingObstacleId> {
    public int Value { get; }
    public PathfindingObstacleId(int value) => Value = value;
    public bool Equals(PathfindingObstacleId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is PathfindingObstacleId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(PathfindingObstacleId left, PathfindingObstacleId right) => left.Equals(right);
    public static bool operator !=(PathfindingObstacleId left, PathfindingObstacleId right) => !left.Equals(right);
}