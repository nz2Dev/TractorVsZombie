using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleRam : MonoBehaviour {

    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float forceMultiplier = 1;
    [SerializeField] private float explosionRadius = 1;
    [SerializeField] private float upwardsModifier = 1;
    [SerializeField] private float explosionPositionSideOffset = 1;
    [SerializeField] private int ramDamage = 30;
    
    private Vehicle _vehicle;
    private bool _ramActive;

    private void Awake() {
        _vehicle = GetComponentInChildren<Vehicle>();
    }

    public void SetRamActivation(bool activation) {
        _ramActive = activation;
    }

    // TODO make some weapone use this effect and controll the damage it deals
    // then implement tractor operator that uses this weapon to be consisten damage system

    private void OnTriggerStay(Collider other) {
        if (!_ramActive)
            return;
        
        if (other.attachedRigidbody == null)
            return;

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