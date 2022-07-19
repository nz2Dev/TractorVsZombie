using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicReseter : MonoBehaviour {

    private LayerMask castLayerMask;

    public void StartCountdownReset(float time, LayerMask castLayerMask) {
        this.castLayerMask = castLayerMask;
        StartCoroutine(ResetRoutine(time));
    }

    private IEnumerator ResetRoutine(float time) {
        yield return new WaitForSeconds(time);
        ResetPhysic();
        Destroy(this);
    }

    private void ResetPhysic() {
        var up = Vector3.up * -transform.position.y;
        if (Physics.Raycast(transform.position, up, out var hitInfo, castLayerMask)) {
            transform.position = hitInfo.point;
            transform.rotation = Quaternion.identity;

            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null) {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
