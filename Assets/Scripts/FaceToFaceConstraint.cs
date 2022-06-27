using System;
using UnityEngine;

public class FaceToFaceConstraint : MonoBehaviour {

    public Transform faceObject;
    
    private void LateUpdate() {
        transform.LookAt(transform.position + faceObject.forward);    
    }
}