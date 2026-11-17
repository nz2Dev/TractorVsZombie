using UnityEngine;

public class FlatRotation : MonoBehaviour {
    public Vector3 ForwardInput {
        set {
            var forwardFlat = Vector3.ProjectOnPlane(value, Vector3.up);
            transform.rotation = Quaternion.LookRotation(forwardFlat, Vector3.up);
        }
    }
}