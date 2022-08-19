using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recovery : MonoBehaviour {

    private Vector3 initPosition;
    private Quaternion initRotation;
    private Vector3 initScale;

    private void Awake() {
        initPosition = transform.position;
        initRotation = transform.rotation;
        initScale = transform.localScale;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            transform.position = initPosition;
            transform.rotation = initRotation;
            transform.localScale = initScale;

            var attachedRigidbody = GetComponent<Rigidbody>();
            if (attachedRigidbody != null) {
                attachedRigidbody.velocity = Vector3.zero;
                attachedRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
