using System;

public readonly struct ProximityId : IEquatable<ProximityId> {
    public int Value { get; }
    public ProximityId(int value) => Value = value;
    public bool Equals(ProximityId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is ProximityId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(ProximityId left, ProximityId right) => left.Equals(right);
    public static bool operator !=(ProximityId left, ProximityId right) => !left.Equals(right);
}