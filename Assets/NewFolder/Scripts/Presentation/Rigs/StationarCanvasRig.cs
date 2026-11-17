using UnityEngine;

public class StationarCanvasRig : MonoBehaviour {
    
    [SerializeField] private Transform aimTransform;
    [NonNull, SerializeField] private Canvas canvas;

    public Transform AimTransform {
        set {
            aimTransform = value;
        }
    }

    private void OnValidate() {
        if (canvas != null && aimTransform != null) {
            ApplyAimRotation();
        }
    }

    private void Update() {
        ApplyAimRotation();
    }

    [ContextMenu("validate")]
    private void ApplyAimRotation() {
        canvas.transform.rotation = aimTransform.rotation;
    }
}