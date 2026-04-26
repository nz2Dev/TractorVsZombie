using System;

using UnityEngine;

public class HeadquarterBuildingSource : MonoBehaviour {

    [SerializeField] private HeadquarterBuildingConfig config;

    public HeadquarterBuildingPrototype GetPrototype() {
        return new HeadquarterBuildingPrototype {
            position = transform.position,
            rotation = transform.rotation,
            config = config,
        };
    }
}