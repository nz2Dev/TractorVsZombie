using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Assertions;

public class GrenaderController : MonoBehaviour {

    private enum ReadyState {
        WaitingForAmmo,
        Preparing,
        Ready
    }

    [SerializeField] private Ammo ammo;
    [SerializeField] private Grenader grenader;
    [SerializeField] private float reloadTime = 0.3f;

    private ReadyState _state = ReadyState.Ready;
    private Coroutine _loadingCoroutine;
    private bool _fireOnLoaded;

    public bool ReadyForActivation => _state == ReadyState.Ready;
    public float TimeToReadynes => _state == ReadyState.WaitingForAmmo ? float.PositiveInfinity : _state == ReadyState.Ready ? 0 : reloadTime;
    public float ExplosionRadius => grenader.ExplosionRadius;
    public Vector3 LauncherPosition => grenader.LauncherPosition;
    public bool IsActivated => _state == ReadyState.Ready && grenader.IsInstantiated;

    public event Action<float> OnStartReload;
    public event Action OnReloaded;

    private void Awake() {
        ammo.OnAmmoStateChanged += AmmoRefillObserver;
    }

    private void OnDestroy() {
        ammo.OnAmmoStateChanged -= AmmoRefillObserver;
    }

    private void AmmoRefillObserver(Ammo ammo) {
        if (_state == ReadyState.WaitingForAmmo && ammo.HasAmmo) {
            Reload();
        }
    }

    public void Activate() {
        if (_state == ReadyState.WaitingForAmmo) {
            Assert.IsFalse(ammo.HasAmmo, "Invalid state while ammo is available");
            ammo.NotifyNeedAmmo();
            return;
        }

        if (_state == ReadyState.Preparing) {
            return;
        }

        grenader.InstatiateGrenade();
    }

    public void Aim(Vector3 point) {
        grenader.Aim(point);
    }

    public void Fire() {
        if (_state == ReadyState.Ready) {
            FireGreander();
        } else if (_state == ReadyState.Preparing) {
            _fireOnLoaded = true;
        }
    }

    private void FireGreander() {
        if (ammo.HasAmmo) {
            Assert.IsTrue(ammo.TakeAmmo());
            grenader.Fire();    
        }

        Reload();
    }

    private void Reload() {
        _state = ReadyState.WaitingForAmmo;
        if (ammo.HasAmmo) {
            _loadingCoroutine = StartCoroutine(Loading());
        } else {
            ammo.NotifyNeedAmmo();
        }
    }

    private IEnumerator Loading() {
        _state = ReadyState.Preparing;
        OnStartReload?.Invoke(reloadTime);
        yield return new WaitForSeconds(reloadTime);
        _state = ReadyState.Ready;
        OnReloaded?.Invoke();

        if (_fireOnLoaded) {
            _fireOnLoaded = false;
            FireGreander();
        }
    }

}