using UnityEngine;

public class AngleRotationLimiter : MonoBehaviour {

    public float yDegreeMin = -45;
    public float yDegreeMax = 45;

    private void Update() {
        var forwrad = transform.forward;
        var zeroAngleForward = transform.parent == null ? Vector3.forward : transform.parent.forward;
        var angleBetween = Vector3.SignedAngle(zeroAngleForward, forwrad, transform.up);
        var yAngle = Mathf.Clamp(angleBetween, yDegreeMin, yDegreeMax);
        transform.localRotation = Quaternion.AngleAxis(yAngle, transform.up);
    }

}