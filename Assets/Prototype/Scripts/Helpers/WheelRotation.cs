using Unity.VisualScripting;
using UnityEngine;

public class WheelRotation : MonoBehaviour {
    
    public Transform steerTransform;
    public Transform wheelTransform;

    private void Update() {
        var targetTransform = wheelTransform == null ? transform : wheelTransform;
        targetTransform.localPosition = transform.localPosition;
        targetTransform.localRotation = steerTransform.localRotation;
    }

}