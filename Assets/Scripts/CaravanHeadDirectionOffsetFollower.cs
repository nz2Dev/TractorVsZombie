using UnityEngine;

public class CaravanHeadDirectionOffsetFollower : MonoBehaviour {

    [SerializeField] private float distanceOffset = 1.5f;
    [SerializeField] private float lerpSpeedMultiplier = 2f;
    [SerializeField] private float minDistance = 0.1f;

    private CaravanMember _member;

    private void Awake() {
        _member = GetComponent<CaravanMember>();
    }

    private void Update() {
        if (_member.Head == null) {
            return;
        }

        var position = transform.position;
        var followPoint = _member.Head.transform.TransformPoint(Vector3.back * distanceOffset);
        var forwardVector = followPoint - position;

        if (forwardVector.magnitude > minDistance) {
            transform.position = Vector3.Lerp(position, followPoint, Time.deltaTime * lerpSpeedMultiplier);
            transform.rotation = Quaternion.LookRotation(forwardVector.normalized, Vector3.up);
        }
    }

}