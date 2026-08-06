using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionMarker : MonoBehaviour {
    
    [SerializeField] private Collider thisCollider;

    public float Height;
    public float Radius;

    void OnValidate() {
        Height = thisCollider.bounds.size.y; // both are compatability
        Radius = Mathf.Max(thisCollider.bounds.extents.x, thisCollider.bounds.extents.z);
    }

    private void Awake() {
        // check out UnityVehicle for proper validation and handling of dependent component handling via params or constraints
        thisCollider.isTrigger = true;
    }
}