using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetComponentDebugOutside : MonoBehaviour {

    public GameObject subjectGO;

    private void Awake() {
        var subject = subjectGO.GetComponent<GetComponentDebugSubject>();
        Debug.Log("Outside, OnAwake: Subject: " + subject);
    }
    
    private void Start() {
        var subject = subjectGO.GetComponent<GetComponentDebugSubject>();
        Debug.Log("Outside, OnStart: Subject: " + subject);
    }

}
