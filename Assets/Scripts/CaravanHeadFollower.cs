using UnityEngine;

public class CaravanHeadFollower : MonoBehaviour {
    private CaravanMember _member;

    private void Awake() {
        _member = GetComponent<CaravanMember>();
    }

    private void Update() {
        if (_member.Head == null) {
            return;
        }

        var position = transform.position;
        var headConnectPoint = _member.Head.transform.TransformPoint(Vector3.back);
        var forwardVector = (headConnectPoint - position).normalized;

        transform.position = Vector3.Lerp(position, headConnectPoint, Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(forwardVector, Vector3.up);
    }

}