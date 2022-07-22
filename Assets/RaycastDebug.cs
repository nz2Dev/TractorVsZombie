using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDebug : MonoBehaviour {

    [SerializeField] private int checkDistance = 3;
    [SerializeField] private LayerMask layerMask;

    void Update() {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, checkDistance, layerMask)) {
            Debug.Log("cast hit");
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.yellow);
            Debug.DrawLine(transform.position, hit.point, Color.red);
        } else {
            Debug.LogWarning("cast miss");
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.blue);
        }
    }
}
