using System;

public readonly struct CarryingId : IEquatable<CarryingId> {
    public int Value { get; }
    public CarryingId(int value) => Value = value;
    public bool Equals(CarryingId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is CarryingId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(CarryingId left, CarryingId right) => left.Equals(right);
    public static bool operator !=(CarryingId left, CarryingId right) => !left.Equals(right);
}
