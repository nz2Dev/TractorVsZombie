using System;

using UnityEngine;

[Serializable]
public struct RamSource {
    
    [Inline] public RamConfig config;

    public RamPrototype GetPrototype() => new () {
        config = config
    };

}
