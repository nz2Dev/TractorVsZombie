using System;

public readonly struct MovementId : IEquatable<MovementId> {
    public int Value { get; }

    public MovementId(int value) => Value = value;

    public bool Equals(MovementId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is MovementId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(MovementId left, MovementId right) => left.Equals(right);
    public static bool operator !=(MovementId left, MovementId right) => !left.Equals(right);
}
