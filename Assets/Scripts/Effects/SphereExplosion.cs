using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SphereExplosion : MonoBehaviour {
    [SerializeField] private float effectRadius = 5;
    [SerializeField] private float effectForce = 15;
    [SerializeField] float effectUpwardModifier = 1;
    [SerializeField] private ForceMode effectForceMode = ForceMode.Impulse;
    [SerializeField] private GameObject explosionParticlesPrefab;
    [SerializeField] private float explosionLifetime;

    private CinemachineImpulseSource _impulseSource;

    public float EffectRadius => effectRadius;

    private void Awake() {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Explode(Action<Vector3, IEnumerable<Rigidbody>> onExplodeAffected) {
        if (explosionParticlesPrefab != null) {
            var explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
            DestructionTimer.StartOn(explosionParticles, explosionLifetime);
        }

        var affectedCollisions = Physics.SphereCastAll(transform.position, effectRadius, Vector3.up);
        var affectedRigidbodies = new HashSet<Rigidbody>();
        foreach (var collision in affectedCollisions) {
            var affectedRigidbody = collision.rigidbody;
            if (affectedRigidbody != null) {
                affectedRigidbodies.Add(affectedRigidbody);
            }
        }

        foreach (var rigidbody in affectedRigidbodies) {
            rigidbody.AddExplosionForce(
                effectForce,
                transform.position,
                effectRadius,
                effectUpwardModifier,
                effectForceMode
            );
        }

        onExplodeAffected?.Invoke(transform.position, affectedRigidbodies);

        if (_impulseSource != null) {
            _impulseSource.GenerateImpulse();
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
#endif
}
