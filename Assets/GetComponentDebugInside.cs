using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetComponentDebugInside : MonoBehaviour {

    private void Awake() {
        var subject = GetComponent<GetComponentDebugSubject>();
        Debug.Log("Inside, OnAwake: Subject: " + subject);
    }

    private void Start() {
        var subject = GetComponent<GetComponentDebugSubject>();
        Debug.Log("Inside, OnStart: Subject: " + subject);
    }
}
