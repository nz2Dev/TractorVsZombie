using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class VehicleBody : MonoBehaviour {
    
    [Inline][Local][SerializeField] internal Collider baseCollider;
    
    internal Rigidbody physics;

    private void Awake() {
        physics = GetComponent<Rigidbody>();
    }

}