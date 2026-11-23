using System;

public enum SelectionEntityType {
    Truck,
    Platform
}

public readonly struct SelectionOption : IEquatable<SelectionOption> {
    
    public readonly int entityId;
    public readonly SelectionEntityType entityType;

    public static readonly SelectionOption TruckOption = new(-1, SelectionEntityType.Truck);

    public SelectionOption(int entityId, SelectionEntityType entityType) {
        this.entityId = entityId;
        this.entityType = entityType;
    }

    public bool Equals(SelectionOption other) => entityId == other.entityId && entityType == other.entityType;
    public override bool Equals(object obj) => obj is SelectionOption other && Equals(other);
    public override int GetHashCode() => (entityId * 397) ^ (int)entityType;
}
