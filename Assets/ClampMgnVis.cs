using System;
using UnityEngine;

public class ClampMgnVis : MonoBehaviour {

    [Range(0, 2f)]
    public float val = 1f;

    public Transform head;
    
    private void OnDrawGizmos() {
        var vector = head.position - transform.position;
        var clamped = Vector3.ClampMagnitude(vector, val);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + vector);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + clamped);
    }
}