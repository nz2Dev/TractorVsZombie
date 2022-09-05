using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tractor : MonoBehaviour {

    [SerializeField] private InputActionReference activateAction;
    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float forceMultiplier = 1;
    [SerializeField] private float acceleratedSpeedMultiplier = 2;
    [SerializeField] private float explosionRadius = 1;
    [SerializeField] private float upwardsModifier = 1;
    [SerializeField] private int ramDamage = 30;
    [SerializeField] private float explosionPositionSideOffset = 1;
    
    private Vehicle _vehicle;
    private bool _accelerated;

    private void Awake() {
        _vehicle = GetComponentInChildren<Vehicle>();
    }

    private void Update() {
        if (activateAction.action.WasPressedThisFrame()) {
            _accelerated = true;
            _vehicle.ChangeMaxSpeedMultiplier(acceleratedSpeedMultiplier);
        }
        if (activateAction.action.WasReleasedThisFrame()) {
            _accelerated = false;
            _vehicle.ChangeMaxSpeedMultiplier(1);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!_accelerated) {
            return;
        }
        if (other.attachedRigidbody == null) {
            return;
        }

        var stability = other.attachedRigidbody.GetComponent<PhysicStability>();
        if (stability != null) {
            if (!stability.IsStable) {
                return;
            }
            stability.Destabilize();
        }

        var tractorToTarget = other.attachedRigidbody.transform.position - transform.position;
        var sideNormal = Vector3.Cross(tractorToTarget.normalized, Vector3.up);
        var sideSign = Random.Range(-1, 1);
        var explosionPosition = transform.position + sideNormal * explosionPositionSideOffset * sideSign;
        other.attachedRigidbody.AddExplosionForce(forceMultiplier, explosionPosition, explosionRadius, upwardsModifier, forceMode);
        
        var health = other.attachedRigidbody.GetComponent<Health>();
        if (health != null) {
            health.TakeDamage(ramDamage);
        }
    }
}