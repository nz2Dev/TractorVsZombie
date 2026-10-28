
using UnityEngine;

public struct InfantryState {
    public bool isAlive;
    public bool isGrounded;
    public Vector3 position;
    public float maxSpeed;
    public float activationRadius;
    public bool attackActivated;
    public HealthId healthId;
    public bool factionIsAlie;

    public InfantryState(bool isAlive, bool isGrounded, Vector3 position, float maxSpeed, float activationRadius, HealthId healthId, bool factionIsAlie, bool attackActivated) {
        this.isAlive = isAlive;
        this.isGrounded = isGrounded;
        this.position = position;
        this.maxSpeed = maxSpeed;
        this.activationRadius = activationRadius;
        this.healthId = healthId;
        this.factionIsAlie = factionIsAlie;
        this.attackActivated = attackActivated;
    }
}
