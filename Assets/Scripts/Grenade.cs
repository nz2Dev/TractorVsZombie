using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
    [SerializeField] private float effectRadius = 5;
    [SerializeField] private float effectForce = 15;
    [SerializeField] float effectUpwardModifier = 1;
    [SerializeField] private ForceMode effectForceMode = ForceMode.Impulse;

    public float EffectRadius => effectRadius;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Explode();
        }
    }

    public void Explode() {
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

        Destroy(gameObject);
    }
}
