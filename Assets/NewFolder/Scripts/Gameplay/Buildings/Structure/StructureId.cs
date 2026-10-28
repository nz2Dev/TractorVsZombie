using System;

public readonly struct StructureId : IEquatable<StructureId> {
    public int Value { get; }

    public StructureId(int value) => Value = value;

    public bool Equals(StructureId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is StructureId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";

    public static bool operator ==(StructureId left, StructureId right) => left.Equals(right);
    public static bool operator !=(StructureId left, StructureId right) => !left.Equals(right);
}
