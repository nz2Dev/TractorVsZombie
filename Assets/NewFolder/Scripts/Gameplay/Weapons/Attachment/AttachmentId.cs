using System;

public readonly struct AttachmentId : IEquatable<AttachmentId> {
    public int Value { get; }
    public AttachmentId(int value) => Value = value;
    public bool Equals(AttachmentId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is AttachmentId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => $"Id {{ Value = {Value} }}";
    public static bool operator ==(AttachmentId left, AttachmentId right) => left.Equals(right);
    public static bool operator !=(AttachmentId left, AttachmentId right) => !left.Equals(right);
}
