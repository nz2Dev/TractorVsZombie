using System;

namespace Combat {
    public readonly struct CombatId : IEquatable<CombatId> {
        public int Value { get; }
        public CombatId(int value) => Value = value;
        public bool Equals(CombatId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CombatId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"Id {{ Value = {Value} }}";
        public static bool operator ==(CombatId left, CombatId right) => left.Equals(right);
        public static bool operator !=(CombatId left, CombatId right) => !left.Equals(right);
    }
}
