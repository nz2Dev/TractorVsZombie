using System;

public readonly struct FactionId : IEquatable<FactionId> {
    public int Value { get; }
    public FactionId(int value) => Value = value;
    public bool Equals(FactionId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is FactionId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(FactionId left, FactionId right) => left.Equals(right);
    public static bool operator !=(FactionId left, FactionId right) => !left.Equals(right);
}
