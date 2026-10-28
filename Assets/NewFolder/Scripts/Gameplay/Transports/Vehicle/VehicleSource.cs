using System;
using UnityEngine;

[Serializable]
public struct VehicleSource {
    [Local, SerializeField] private UnityVehicle prefab;

    public VehiclePrototype GetPrototype() => new() {
        prefab = prefab
    };
}
