using System;

public readonly struct RaycastId : IEquatable<RaycastId> {
    public int Value { get; }
    public RaycastId(int value) => Value = value;
    public bool Equals(RaycastId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is RaycastId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(RaycastId left, RaycastId right) => left.Equals(right);
    public static bool operator !=(RaycastId left, RaycastId right) => !left.Equals(right);
}