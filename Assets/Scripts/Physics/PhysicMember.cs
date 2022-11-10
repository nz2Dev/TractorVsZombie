using System;
using UnityEngine;

public interface IStabilityListener {
    void OnStabilityChanged(Rigidbody thisRigidbody, bool stability);
}

[RequireComponent(typeof(Rigidbody))]
public class PhysicMember : MonoBehaviour {

    [SerializeField] private LayerMask stabilizers = ~0;

    private Rigidbody _rigidbody;
    private bool _stability = true;
    private IStabilityListener[] _stabilityListenres = new IStabilityListener[0];

    public bool IsStable => _stability;
    public Rigidbody Rigidbody => _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _stabilityListenres = GetComponents<IStabilityListener>() ?? new IStabilityListener[0];
        if (_stabilityListenres.Length == 0) {
            Debug.LogWarning("No stability listeners, state change will have no effect");
        }
    }

    private void Start() {
        ChangeStability(true);
    }

    public void Destabilize() {
        ChangeStability(false);
    }

    private void ChangeStability(bool stability) {
        _stability = stability;
        
        foreach (var listener in _stabilityListenres) {
            listener.OnStabilityChanged(_rigidbody, stability);
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("On Collision enter: " + other.gameObject.name);
        if (stabilizers == (stabilizers | (1 << other.gameObject.layer))) {
            Debug.Log("On collision with stabilizer");
            ChangeStability(true);
        }
    }

}