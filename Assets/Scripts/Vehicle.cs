using UnityEngine;

class Vehicle : MonoBehaviour {
    [SerializeField] private float maxForce = 1;
    [SerializeField] private float maxSpeed = 1;
    [SerializeField] private float mass = 1;

    private Vector3 _velocity;
    private Vector3 _steeringForce;

    public Vector3 Velocity => _velocity;
    public float MaxSpeed => maxSpeed;

    public Vector3 PredictPosition(float futureTimeAmount) {
        return transform.position + _velocity * futureTimeAmount;
    }
    
    public void ApplyForce(Vector3 force) {
        _steeringForce += force;
    }

    private void Update() {
        _steeringForce = Vector3.ClampMagnitude(_steeringForce, maxForce);
        _velocity += (_steeringForce / mass);
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);

        var thisTransform = transform;
        var newPosition = thisTransform.position + _velocity * Time.deltaTime;
        if (newPosition.magnitude > 0) {
            thisTransform.LookAt(newPosition);
            thisTransform.position = newPosition;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + _velocity);
    }
}