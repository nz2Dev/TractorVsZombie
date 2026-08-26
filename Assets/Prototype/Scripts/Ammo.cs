using System;
using UnityEngine;

public class Ammo : MonoBehaviour {

    [SerializeField] private int maxAmmo;
    [SerializeField] private int initAmmo;

    private int _ammoCount;

    public int AmmoCount => _ammoCount;
    public bool HasAmmo => _ammoCount > 0;
    public int MaxAmmo => maxAmmo;

    public event Action<Ammo> OnAmmoStateChanged;
    public event Action OnNoAmmoState;

    private void Start() {
        _ammoCount = initAmmo;
        OnAmmoStateChanged?.Invoke(this);
    }

    public void RefillFull() {
        Refill(MaxAmmo);
    }

    public void Refill(int ammount) {
        _ammoCount = Math.Max(ammount, maxAmmo);
        OnAmmoStateChanged?.Invoke(this);
    }

    public void NotifyNeedAmmo() {
        if (!HasAmmo) {
            OnNoAmmoState?.Invoke();
        }
    }

    public bool TakeAmmo() {
        if (_ammoCount <= 0) {
            return false;
        }
        _ammoCount--;
        OnAmmoStateChanged?.Invoke(this);
        return true;
    }

}