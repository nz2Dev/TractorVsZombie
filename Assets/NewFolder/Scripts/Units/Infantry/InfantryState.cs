using Combat;

using UnityEngine;

public struct InfantryState {
    public bool isAlive;
    public bool isGrounded;
    public Vector3 position;
    public Vector3 movementVelocity;
    public float maxSpeed;
    public float activationRadius;
    public bool attackActivated;
    public CombatId combatId;
    public bool combatIsAlie;

    public InfantryState(bool isAlive, bool isGrounded, Vector3 position, Vector3 movementVelocity, float maxSpeed, float activationRadius, CombatId combatId, bool combatIsAlie, bool attackActivated) {
        this.isAlive = isAlive;
        this.isGrounded = isGrounded;
        this.position = position;
        this.movementVelocity = movementVelocity;
        this.maxSpeed = maxSpeed;
        this.activationRadius = activationRadius;
        this.combatId = combatId;
        this.combatIsAlie = combatIsAlie;
        this.attackActivated = attackActivated;
    }
}
