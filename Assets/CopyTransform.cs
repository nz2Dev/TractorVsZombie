using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour {

    public Transform target;
    public bool copyPosition = true;
    public bool copyRotation = true;

    private void Update() {
        if (copyPosition) {
            transform.position = target.position;
        }
        if (copyRotation) {
            transform.rotation = target.rotation;
        }
    }

}
