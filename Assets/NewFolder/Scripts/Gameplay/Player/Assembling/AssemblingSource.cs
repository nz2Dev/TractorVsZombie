using System;
using UnityEngine;

[Serializable]
public struct AssemblingSource {

    [SerializeField] private GameObject platformPreviewPrefab;
    [SerializeField] private PlatformSource initPlatformSource;

    public PlatformSource InitPlatformSource => initPlatformSource;
    
    public AssemblingPrototype Get() {
        return new AssemblingPrototype {
            platformPreviewPrefab = platformPreviewPrefab,
            pickupPlatformPrototype = initPlatformSource.GetPrototype()
        };
    }
}
