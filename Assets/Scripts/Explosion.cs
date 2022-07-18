using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    [SerializeField] private float effectRadius = 5;
    [SerializeField] private float effectForce = 15;
    [SerializeField] float effectUpwardModifier = 1;
    [SerializeField] private ForceMode effectForceMode = ForceMode.Impulse;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Perform();
        }
    }

    public void Perform() {
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
    }

}
