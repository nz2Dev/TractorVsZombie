using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereExplosion : MonoBehaviour {
    [SerializeField] private float effectRadius = 5;
    [SerializeField] private float effectForce = 15;
    [SerializeField] float effectUpwardModifier = 1;
    [SerializeField] private ForceMode effectForceMode = ForceMode.Impulse;
    [SerializeField] private GameObject explosionParticlesPrefab;
    [SerializeField] private float explosionLifetime;

    public float EffectRadius => effectRadius;

    public void Explode(Action<Vector3, RaycastHit[]> onExplodeAffected) {
        var explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        DestructionTimer.StartOn(explosionParticles, explosionLifetime);

        var affectedCollisions = Physics.SphereCastAll(transform.position, effectRadius, Vector3.up);
        foreach (var collision in affectedCollisions) {
            var affectedRigidbody = collision.rigidbody;
            if (affectedRigidbody != null) {
                affectedRigidbody.AddExplosionForce(
                    effectForce,
                    transform.position,
                    effectRadius,
                    effectUpwardModifier,
                    effectForceMode
                );
            }
        }

        onExplodeAffected?.Invoke(transform.position, affectedCollisions);

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
#endif
}
