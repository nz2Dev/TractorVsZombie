using System;

public readonly struct HitboxId : IEquatable<HitboxId> {
    public int Value { get; }

    public HitboxId(int value) => Value = value;

    public bool Equals(HitboxId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is HitboxId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(HitboxId left, HitboxId right) => left.Equals(right);
    public static bool operator !=(HitboxId left, HitboxId right) => !left.Equals(right);
}
