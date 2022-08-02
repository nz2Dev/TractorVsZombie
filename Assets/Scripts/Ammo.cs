using System;
using UnityEngine;

public class Ammo : MonoBehaviour {

    [SerializeField] private int maxAmmo;
    [SerializeField] private int initAmmo;

    private int _ammoCount;

    public int AmmoCount => _ammoCount;
    public int MaxAmmo => maxAmmo;

    public event Action<Ammo> OnAmmoStateChanged;

    private void Start() {
        _ammoCount = initAmmo;
        OnAmmoStateChanged?.Invoke(this);
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