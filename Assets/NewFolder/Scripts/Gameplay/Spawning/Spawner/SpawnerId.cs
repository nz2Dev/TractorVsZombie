using System;

public readonly struct SpawnerId : IEquatable<SpawnerId> {
    public int Value { get; }
    public SpawnerId(int value) => Value = value;
    public bool Equals(SpawnerId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is SpawnerId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(SpawnerId left, SpawnerId right) => left.Equals(right);
    public static bool operator !=(SpawnerId left, SpawnerId right) => !left.Equals(right);
}