public enum CombatEvent {
    None,
    ChargeActivated,
    AttackActivated,
    Canceled
}

public struct CombatState {
    public CombatEvent attackEvent;
    public bool attackActivated;
    public float attackDuration;
    public float activationRadius;
}
