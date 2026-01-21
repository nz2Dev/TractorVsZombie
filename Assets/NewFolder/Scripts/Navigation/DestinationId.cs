using System;

public readonly struct DestinationId : IEquatable<DestinationId> {
    public readonly int data;
    internal DestinationId(int value) => data = value;
    public bool Equals(DestinationId other) => data == other.data;
    public override bool Equals(object obj) => obj is DestinationId other && Equals(other);
    public override int GetHashCode() => data.GetHashCode();
    public override string ToString() => data.ToString();
    public static bool operator ==(DestinationId left, DestinationId right) => left.Equals(right);
    public static bool operator !=(DestinationId left, DestinationId right) => !left.Equals(right);
}