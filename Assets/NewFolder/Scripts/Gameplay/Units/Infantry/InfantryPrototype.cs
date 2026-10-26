
using UnityEngine;

public struct InfantryPrototype {
    public Vector3 position;
    public MotionPrototype motionPrototype;
    public MovementPrototype movementPrototype;
    public InfantryVisuals visualsPrefab;
    public DropPrototype dropPrototype;

    public RagdollBody physicsBodyPrefab;
    public HitboxPrototype hitboxPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public CombatPrototype combatPrototype;
}
