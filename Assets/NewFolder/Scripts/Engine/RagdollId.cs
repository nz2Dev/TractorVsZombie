using System;

public readonly struct RagdollId : IEquatable<RagdollId> {
    public int Value { get; }
    public RagdollId(int value) => Value = value;
    public bool Equals(RagdollId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is RagdollId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(RagdollId left, RagdollId right) => left.Equals(right);
    public static bool operator !=(RagdollId left, RagdollId right) => !left.Equals(right);
}