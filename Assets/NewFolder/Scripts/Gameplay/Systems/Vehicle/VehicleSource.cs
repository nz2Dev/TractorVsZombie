using System;
using UnityEngine;

[Serializable]
public struct VehicleSource {
    [Local, SerializeField] private UnityVehicle prefab;
    [Inline] public VehicleConfig config;

    public VehiclePrototype GetPrototype() => new() {
        prefab = prefab,
        config = config
    };
}
