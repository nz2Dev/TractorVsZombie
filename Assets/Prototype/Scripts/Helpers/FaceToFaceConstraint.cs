using System;
using UnityEngine;

[ExecuteInEditMode]
public class FaceToFaceConstraint : MonoBehaviour {

    public Transform faceObject;
    
    private void LateUpdate() {
        if (faceObject != null) {
            transform.LookAt(transform.position + faceObject.forward);    
        }
    }
}