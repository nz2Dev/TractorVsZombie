using System;

public readonly struct MotionId : IEquatable<MotionId> {
    public int Value { get; }

    public MotionId(int value) => Value = value;

    public bool Equals(MotionId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is MotionId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(MotionId left, MotionId right) => left.Equals(right);
    public static bool operator !=(MotionId left, MotionId right) => !left.Equals(right);
}
