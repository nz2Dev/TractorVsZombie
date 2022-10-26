using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionRam))]
public class Ram : MonoBehaviour {

    [SerializeField] private int ramDamage = 30;

    private CollisionRam _ramEffect;

    private void Awake() {
        _ramEffect = GetComponent<CollisionRam>();
        _ramEffect.OnRamCollision += (rigidbody) => {
            var health = rigidbody.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(ramDamage);
            }
        };
    }
    
}
