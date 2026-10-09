using System;

public readonly struct PoseId : IEquatable<PoseId> {
    public int Value { get; }

    public PoseId(int value) => Value = value;

    public bool Equals(PoseId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is PoseId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(PoseId left, PoseId right) => left.Equals(right);
    public static bool operator !=(PoseId left, PoseId right) => !left.Equals(right);
}
