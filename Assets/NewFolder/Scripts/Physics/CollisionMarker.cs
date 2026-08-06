using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionMarker : MonoBehaviour {
    
    [SerializeField] private Collider thisCollider;

    private void Awake() {
        // check out UnityVehicle for proper validation and handling of dependent component handling via params or constraints
        thisCollider.isTrigger = true;
    }
}