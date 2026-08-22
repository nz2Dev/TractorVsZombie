using System;

public readonly struct InteractionId : IEquatable<InteractionId> {
    public int Value { get; }
    public InteractionId(int value) => Value = value;
    public bool Equals(InteractionId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is InteractionId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(InteractionId left, InteractionId right) => left.Equals(right);
    public static bool operator !=(InteractionId left, InteractionId right) => !left.Equals(right);
}