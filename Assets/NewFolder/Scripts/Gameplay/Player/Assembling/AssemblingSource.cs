using System;
using UnityEngine;

[Serializable]
public struct AssemblingSource {

    [SerializeField] private GameObject platformPreviewPrefab;
    [SerializeField] private TruckSource initTruckSource;
    [SerializeField] private PlatformSource initPlatformSource;
    [SerializeField] private int initialPlatformCount;
    
    public PlatformPrototype PlatformPrototype => initPlatformSource.GetPrototype();

    public AssemblingPrototype Get() {
        return new AssemblingPrototype {
            platformPreviewPrefab = platformPreviewPrefab,
            initTruckPrototype = initTruckSource.GetPrototype(),
            pickupPlatformPrototype = initPlatformSource.GetPrototype(),
            initialPlatformCount = initialPlatformCount
        };
    }
}
