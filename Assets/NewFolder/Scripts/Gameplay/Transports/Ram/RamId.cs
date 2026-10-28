using System;

public readonly struct RamId : IEquatable<RamId> {
    public int Value { get; }

    public RamId(int value) => Value = value;

    public bool Equals(RamId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is RamId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(RamId left, RamId right) => left.Equals(right);
    public static bool operator !=(RamId left, RamId right) => !left.Equals(right);
}
