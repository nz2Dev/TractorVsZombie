using System;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsBody : MonoBehaviour {
    
    [SerializeField] private Rigidbody thisRigidbody;
    [SerializeField] private CapsuleCollider thisCollider;
    
    public float ExplosionTime { get; private set; } = float.NegativeInfinity;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Vector3 LinearVelocity => thisRigidbody.linearVelocity;
    public bool IsDynamic => !thisRigidbody.isKinematic;

    private void Awake() {
        SetDynamics(false);
    }

    public void AddExplosionForce(float force, Vector3 position, float radius, float upwardsModifier, ForceMode mode) {
        thisRigidbody.AddExplosionForce(force, position, radius, upwardsModifier, mode);
        ExplosionTime = Time.time;
    }

    public void SetDynamics(bool active) {
        thisRigidbody.isKinematic = !active;
        thisRigidbody.useGravity = active;
        thisCollider.isTrigger = !active;
    }

    internal void Teleport(Vector3 position) {
        transform.position = position;
        thisRigidbody.position = position;
    }

    internal void Set(Vector3 position, Quaternion rotation) {
        transform.SetPositionAndRotation(position, rotation);
        thisRigidbody.position = position;
        thisRigidbody.rotation = rotation;
    }

}