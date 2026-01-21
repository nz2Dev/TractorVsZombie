using System;

public readonly struct MarkerId : IEquatable<MarkerId> {
    public readonly int data;
    internal MarkerId(int value) => data = value;
    public bool Equals(MarkerId other) => data == other.data;
    public override bool Equals(object obj) => obj is MarkerId other && Equals(other);
    public override int GetHashCode() => data.GetHashCode();
    public override string ToString() => data.ToString();
    public static bool operator ==(MarkerId left, MarkerId right) => left.Equals(right);
    public static bool operator !=(MarkerId left, MarkerId right) => !left.Equals(right);
}