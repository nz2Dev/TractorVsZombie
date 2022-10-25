using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour {
    [SerializeField] private float effectRadius = 5;
    [SerializeField] private float effectForce = 15;
    [SerializeField] float effectUpwardModifier = 1;
    [SerializeField] private ForceMode effectForceMode = ForceMode.Impulse;
    [SerializeField] private int damage = 50;
    [SerializeField] private GameObject explosionParticlesPrefab;
    [SerializeField] private float explosionLifetime;

    public float EffectRadius => effectRadius;

    public void Explode() {
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

                var health = affectedRigidbody.GetComponent<Health>();
                var train = affectedRigidbody.GetComponent<CaravanMember>();
                if (health != null && train == null) {
                    var distanceFromEpicentr = Vector3.Distance(affectedRigidbody.transform.position, transform.position);
                    var damageDumping = (int) Utils.Map(distanceFromEpicentr, 0, effectRadius, 0, damage);
                    health.TakeDamage(damage - damageDumping);
                }
            }
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
#endif
}
