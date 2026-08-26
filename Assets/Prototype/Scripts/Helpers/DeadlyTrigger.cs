using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        var enteredGameObject = other.attachedRigidbody == null ? other.gameObject : other.attachedRigidbody.gameObject;
        var enteredHealth = enteredGameObject == null ? null : enteredGameObject.GetComponent<Health>();
        if (enteredHealth != null) {
            enteredHealth.TakeDamage(enteredHealth.Max);
        }
    }
}
