using System;

public readonly struct HealthId : IEquatable<HealthId> {
    public int Value { get; }
    public HealthId(int value) => Value = value;
    public bool Equals(HealthId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is HealthId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(HealthId left, HealthId right) => left.Equals(right);
    public static bool operator !=(HealthId left, HealthId right) => !left.Equals(right);
}
