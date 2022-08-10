using Unity.VisualScripting;
using UnityEngine;

public class WheelRotation : MonoBehaviour {
    
    public Transform steerTransform;

    private void Update() {
        transform.localRotation = steerTransform.localRotation;
    }

}