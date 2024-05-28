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
    }

    private void OnEnable() {
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

        _rigidbody.constraints = stability ? RigidbodyConstraints.FreezeRotation : 0;
        if (stability) {
            _rigidbody.transform.rotation = Quaternion.identity;
        }
        
        foreach (var listener in _stabilityListenres) {
            listener.OnStabilityChanged(_rigidbody, stability);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (!_stability && stabilizers == (stabilizers | (1 << other.gameObject.layer))) {
            ChangeStability(true);
        }
    }

}