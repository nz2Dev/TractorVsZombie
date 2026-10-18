using System;

public readonly struct DropId : IEquatable<DropId> {
    public int Value { get; }
    public DropId(int value) => Value = value;
    public bool Equals(DropId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is DropId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(DropId left, DropId right) => left.Equals(right);
    public static bool operator !=(DropId left, DropId right) => !left.Equals(right);
}
