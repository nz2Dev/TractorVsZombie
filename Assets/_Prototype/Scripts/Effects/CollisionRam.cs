using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CollisionRam : MonoBehaviour {

    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float forceMultiplier = 1;
    [SerializeField] private float explosionRadius = 1;
    [SerializeField] private float upwardsModifier = 1;
    [SerializeField] private float explosionPositionSideOffset = 1;

    public event Action<Rigidbody> OnRamCollision;

    private void OnTriggerStay(Collider other) {
        if (other.attachedRigidbody == null) {
            return;
        }

        var stability = other.attachedRigidbody.GetComponent<PhysicMember>();
        if (stability != null) {
            if (stability.IsStable) {
                stability.Destabilize();
            } else {
                return;
            }
        }

        var sideSign = Random.Range(-1, 1);
        var ramToTarget = other.attachedRigidbody.transform.position - transform.position;
        var sideNormal = Vector3.Cross(ramToTarget.normalized, Vector3.up);
        var explosionPosition = transform.position + sideNormal * explosionPositionSideOffset * sideSign;
        other.attachedRigidbody.AddExplosionForce(forceMultiplier, explosionPosition, explosionRadius, upwardsModifier, forceMode);
        
        OnRamCollision?.Invoke(other.attachedRigidbody);
    }
}