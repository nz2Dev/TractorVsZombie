using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class VehicleDynamics : MonoBehaviour {

    [SerializeField] private float maxTemporalLinearDrag;
    [SerializeField] private float temporalLinearDragRecoverySpeed;

    private Rigidbody physics;
    private float baseLinearDamping;
    private float temporalLinearDrag;

    public float LinearDamping => baseLinearDamping + temporalLinearDrag;

    private void Awake() {
        physics = GetComponent<Rigidbody>();
        baseLinearDamping = physics.linearDamping;
    }

    private void Update() {
        temporalLinearDrag = Mathf.MoveTowards(
            temporalLinearDrag,
            0,
            temporalLinearDragRecoverySpeed * Time.deltaTime);
    }

    public void AddTemporalDrag(float amount) {
        temporalLinearDrag = Mathf.Min(temporalLinearDrag + amount, maxTemporalLinearDrag);
    }
}
