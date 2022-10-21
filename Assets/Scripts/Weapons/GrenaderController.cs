using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GrenaderController : MonoBehaviour {

    private enum ReadyState {
        Invalid,
        Preparing,
        Ready
    }

    [SerializeField] private Ammo ammo;
    [SerializeField] private Grenader grenader;
    [SerializeField] private float reloadTime = 0.3f;

    private ReadyState _state = ReadyState.Ready;
    private bool _fireOnLoaded;

    public bool CanFire => ammo.HasAmmo;
    public float TimeToReadynes => !CanFire ? float.PositiveInfinity : _state == ReadyState.Ready ? 0 : reloadTime;
    public float ExplosionRadius => grenader.ExplosionRadius;
    public bool IsActivated => grenader.CanAim;

    public event Action<float> OnReload;

    private void Awake() {
        ammo.OnAmmoStateChanged += AmmoRefillObserver;
    }

    private void OnDestroy() {
        ammo.OnAmmoStateChanged -= AmmoRefillObserver;
    }

    private void AmmoRefillObserver(Ammo ammo) {
        if (_state == ReadyState.Invalid && ammo.HasAmmo) {
            Reload();
        }
    }

    public bool Activate(Vector3 point) {
        if (_state == ReadyState.Invalid) {
            Assert.IsFalse(ammo.HasAmmo, "Invalid state while ammo is available");
            ammo.NotifyNeedAmmo();
            return false;
        }

        if (_state == ReadyState.Preparing) {
            return false;
        }

        grenader.Load(point);
        return true;
    }

    public void Aim(Vector3 point) {
        if (_state != ReadyState.Invalid) {
            if (grenader.CanAim) {
                grenader.Aim(point);
            }
        }
    }

    public void Fire() {
        if (!grenader.CanFire) {
            return;
        }

        if (_state == ReadyState.Ready) {
            FireGreander();
        } else if (_state == ReadyState.Preparing) {
            _fireOnLoaded = true;
        }
    }

    private void FireGreander() {
        if (ammo.HasAmmo && grenader.CanFire) {
            Assert.IsTrue(ammo.TakeAmmo());
            grenader.Fire();    
        }

        Reload();
    }

    private void Reload() {
        _state = ReadyState.Invalid;
        if (ammo.HasAmmo) {
            StartCoroutine(Loading());
        } else {
            ammo.NotifyNeedAmmo();
        }
    }

    private IEnumerator Loading() {
        _state = ReadyState.Preparing;
        OnReload?.Invoke(reloadTime);
        yield return new WaitForSeconds(reloadTime);
        _state = ReadyState.Ready;

        if (_fireOnLoaded) {
            _fireOnLoaded = false;
            FireGreander();
        }
    }

}