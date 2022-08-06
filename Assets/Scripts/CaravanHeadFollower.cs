using UnityEngine;

public class CaravanHeadFollower : MonoBehaviour {

    [SerializeField] private float distanceOffset = 1.5f;
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
        var headConnectPoint = _member.Head.transform.TransformPoint(Vector3.back * distanceOffset);
        var forwardVector = headConnectPoint - position;

        if (forwardVector.magnitude > minDistance) {
            transform.position = Vector3.Lerp(position, headConnectPoint, Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(forwardVector.normalized, Vector3.up);
        }
    }

}