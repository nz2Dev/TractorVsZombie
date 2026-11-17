using UnityEngine;

public class CanvasRig : MonoBehaviour {
    
    [SerializeField] private Transform aimTransform;
    [NonNull, SerializeField] private Canvas canvas;
    [Range(0f, 5f), SerializeField] private float canvasOffset;

    public Transform AimTransform {
        set {
            aimTransform = value;
        }
    }

    public Vector3 ForwardInput {
        set {
            var forwardFlat = Vector3.ProjectOnPlane(value, Vector3.up);
            transform.rotation = Quaternion.LookRotation(forwardFlat, Vector3.up);
            ApplyOffset();
        }
    }

    private void OnValidate() {
        if (canvas != null && aimTransform != null) {
            ApplyOffset();
            ApplyAimRoll();
        }
    }

    private void Update() {
        ApplyAimRoll();
    }

    private void ApplyOffset() {
        canvas.transform.position = transform.position + Vector3.up * 0.05f + transform.right * canvasOffset;
    }

    private void ApplyAimRoll() {
        var rollUp = Vector3.ProjectOnPlane(aimTransform.up, Vector3.up);
        canvas.transform.rotation = Quaternion.LookRotation(Vector3.down, rollUp);
    }

}