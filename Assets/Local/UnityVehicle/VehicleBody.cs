using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class VehicleBody : MonoBehaviour {
    
    [SerializeField] private float mass;
    [SerializeField, Tooltip("Is hiden in inspector"), Inline, ReadOnly] 
    internal Rigidbody physics;
    [Space]
    [Inline, Local, SerializeField] 
    internal Collider baseCollider;

    public Vector3 Velocity => physics.linearVelocity;

#if UNITY_EDITOR
    private void OnValidate() {
        physics.mass = mass;
    }
#endif

}