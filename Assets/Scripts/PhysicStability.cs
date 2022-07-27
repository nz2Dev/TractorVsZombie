using System;
using UnityEngine;

public class PhysicStability : MonoBehaviour {
    private bool _stability = true;
    private Rigidbody _rigidbody;
    public event Action<bool> OnStabilityChanged;

    public bool IsStable => _stability;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Destabilize() {
        ChangeStability(false);
    }

    private void ChangeStability(bool stability) {
        _stability = stability;
        _rigidbody.constraints = stability ? RigidbodyConstraints.FreezeRotation : 0;
        OnStabilityChanged?.Invoke(stability);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.name == "Ground") {
            ChangeStability(true);
        }
    }

}