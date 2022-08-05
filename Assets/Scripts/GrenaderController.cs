using System;
using System.Collections;
using UnityEngine;

public class GrenaderController : MonoBehaviour {

    private enum State {
        Unloaded,
        Loading,
        Loaded
    }

    [SerializeField] private Ammo ammo;
    [SerializeField] private Grenader grenader;
    [SerializeField] private float reloadTime = 0.3f;

    private State _state = State.Unloaded;
    private bool _fireOnLoaded;

    public bool CanFire => ammo.HasAmmo;
    public float TimeToReadynes => !CanFire ? float.PositiveInfinity : _state == State.Loaded ? 0 : reloadTime;

    public event Action OnLoaded;

    public void Prepare() {
        if (_state == State.Unloaded && ammo.HasAmmo) {
            Reload();
        }
    }

    public void AimGreander(Vector3 point) {
        if (_state == State.Unloaded) {
            Reload();
        } else if (_state == State.Loaded) {
            grenader.Aim(point);
        }
    }

    public void FireGreandeAtLastAimed() {
        if (_state == State.Loaded) {
            Fire();
        } else if (_state == State.Loading) {
            _fireOnLoaded = true;
        }
    }

    private void Unload() {
        _state = State.Unloaded;
    }

    private void Reload() {
        if (_state == State.Unloaded && ammo.RequestAmmo()) {
            _state = State.Loading;
            StartCoroutine(Loading());
        }
    }

    private IEnumerator Loading() {
        yield return new WaitForSeconds(reloadTime);
        _state = State.Loaded;

        if (_fireOnLoaded) {
            _fireOnLoaded = false;
            Fire();
        } else {
            OnLoaded?.Invoke();
        }
    }

    private void Fire() {
        if (_state == State.Loaded && grenader.IsAimed && ammo.TakeAmmo()) {
            Unload();
            grenader.FireLastAimPoint();
            Reload();
        }
    }
}