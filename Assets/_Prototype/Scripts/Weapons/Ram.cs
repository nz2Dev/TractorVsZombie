using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionRam))]
public class Ram : MonoBehaviour {

    private CollisionRam _ramEffect;

    public event Action<Rigidbody> OnPushed;

    private void Awake() {
        _ramEffect = GetComponent<CollisionRam>();
        _ramEffect.OnRamCollision += (rigidbody) => {
            OnPushed?.Invoke(rigidbody);
        };
    }
    
}
