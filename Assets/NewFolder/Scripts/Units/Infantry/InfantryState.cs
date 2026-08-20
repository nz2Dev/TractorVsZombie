using UnityEngine;

public struct InfantryState {
    public bool isAlive;
    public bool isGrounded;
    public Vector3 position;
    public Vector3 movementVelocity;
    public float maxSpeed;
    public int combatId;
    public RagdollId bodyId;
}