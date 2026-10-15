using System;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RagdollBody : MonoBehaviour {
    
    [SerializeField] private Rigidbody thisRigidbody;
    [SerializeField] private CapsuleCollider thisCollider;
    [SerializeField] private int groundLayer;
    [SerializeField] private LayerMask depenetrationLayers;
    [SerializeField] private LayerMask excludeDynamicsLayer;

    private readonly List<Collider> depenetrationColliders = new();
    
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Vector3 LinearVelocity => thisRigidbody.linearVelocity;
    public bool IsDynamic => !thisRigidbody.isKinematic;
    public bool ContactWithGround { get; private set; }
    public Collider BodyCollider => thisCollider;
    public IReadOnlyList<Collider> DepenetrationColliders => depenetrationColliders;

    private void Awake() {
        SetDynamics(false);
    }

    public void AddExplosionForce(float force, Vector3 position, float radius, float upwardsModifier, ForceMode mode) {
        thisRigidbody.AddExplosionForce(force, position, radius, upwardsModifier, mode);
    }

    public void SetDynamics(bool active) {
        thisRigidbody.isKinematic = !active;
        thisRigidbody.useGravity = active;
        thisCollider.isTrigger = !active;
        var excludedLayers = thisCollider.excludeLayers.value;
        thisCollider.excludeLayers = active
            ? excludedLayers | excludeDynamicsLayer.value
            : excludedLayers & ~excludeDynamicsLayer.value;
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

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == groundLayer) {
            ContactWithGround = true;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == groundLayer) {
            ContactWithGround = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (IsDepenetrationLayer(other) && !depenetrationColliders.Contains(other)) {
            depenetrationColliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other) {
        depenetrationColliders.Remove(other);
    }

    private bool IsDepenetrationLayer(Collider other) {
        return other != null && (depenetrationLayers.value & (1 << other.gameObject.layer)) != 0;
    }

}
