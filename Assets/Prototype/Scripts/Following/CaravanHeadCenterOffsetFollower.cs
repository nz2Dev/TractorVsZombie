using UnityEngine;

public class CaravanHeadCenterOffsetFollower : MonoBehaviour {
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
        var headCenterPosition = _member.Head.transform.position;
        var backwardVector = position - headCenterPosition;

        if (backwardVector.magnitude > minDistance) {
            var targetPosition = headCenterPosition + backwardVector.normalized * distanceOffset;

            var lerpAcceleration = backwardVector.magnitude * lerpSpeedMultiplier;
            transform.position = Vector3.Lerp(position, targetPosition, Time.deltaTime * lerpAcceleration);
            transform.rotation = Quaternion.LookRotation(-backwardVector.normalized, Vector3.up);
        }
    }
}