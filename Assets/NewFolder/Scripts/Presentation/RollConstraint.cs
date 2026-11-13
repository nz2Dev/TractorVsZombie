using UnityEngine;

public class RollConstraint : MonoBehaviour {
    
    [SerializeField] private Transform target;

    public Transform Target { 
        set {
            target = value;
        }
    }

    private void Update() {
        CopyTargetsRoll();
    }

    [ContextMenu("Validate")]
    private void CopyTargetsRoll() {
        var rollUp = Vector3.ProjectOnPlane(target.up, Vector3.up);
        transform.rotation = Quaternion.LookRotation(Vector3.up, rollUp);
    }
}