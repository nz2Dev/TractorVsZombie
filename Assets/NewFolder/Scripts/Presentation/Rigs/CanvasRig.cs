using UnityEngine;

public class CanvasRig : MonoBehaviour {
    
    [SerializeField] private Transform aimTransform;
    [NonNull, SerializeField] private Canvas canvas;
    [Space]
    [SerializeField] private bool localSideOffset = false;
    [SerializeField] private Vector3 sideOffsetDir = Vector3.left;
    [Range(0f, 5f), SerializeField] private float sideOffset;
    [Space]
    [SerializeField] private float upOffset = 1.0f;
    [Space]
    [SerializeField] private bool billboardRotation = false;

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
            ApplyRotation();
        }
    }

    private void Update() {
        ApplyRotation();
    }

    private void ApplyOffset() {
        var resolvedSideOffsetDir = localSideOffset ? transform.rotation * sideOffsetDir : sideOffsetDir;
        var offset = resolvedSideOffsetDir * sideOffset + Vector3.up * upOffset;
        canvas.transform.position = transform.position + offset;
    }

    private void ApplyRotation() {
        if (billboardRotation) {
            ApplyBillboardRotation();
        } else {
            ApplyAimRoll();
        }
    }

    private void ApplyAimRoll() {
        var rollUp = Vector3.ProjectOnPlane(aimTransform.up, Vector3.up);
        canvas.transform.rotation = Quaternion.LookRotation(Vector3.down, rollUp);
    }

    private void ApplyBillboardRotation() {
        canvas.transform.rotation = aimTransform.rotation;
    }

}