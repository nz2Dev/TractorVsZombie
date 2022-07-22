using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RaycastDebug : MonoBehaviour {

    [SerializeField] private int checkDistance = 10;
    [SerializeField] private float sphereRadius = 1;
    [SerializeField] private bool sphereInsteadOfRay = true;
    [SerializeField] private LayerMask layerMask;

    void Update() {
        if (sphereInsteadOfRay) {
            SphereCast();
        } else {
            RayCast();
        }
    }

    private void SphereCast() {
        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out var hit, checkDistance, layerMask)) {
            Debug.Log("cast hit");
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.yellow);
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.red);
        } else {
            Debug.LogWarning("cast miss");
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.blue);
        }
    }

    private void RayCast() {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, checkDistance, layerMask)) {
            Debug.Log("cast hit");
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.yellow);
            Debug.DrawLine(transform.position, hit.point, Color.red);
        } else {
            Debug.LogWarning("cast miss");
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.blue);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
