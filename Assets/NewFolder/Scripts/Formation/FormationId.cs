using System;

public readonly struct FormationId : IEquatable<FormationId> {
    public int Value { get; }
    public FormationId(int value) => Value = value;
    public bool Equals(FormationId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is FormationId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(FormationId left, FormationId right) => left.Equals(right);
    public static bool operator !=(FormationId left, FormationId right) => !left.Equals(right);
}