using System;
using UnityEngine;

[Serializable]
public struct MotionSource {
    [Inline, SerializeField] private MotionConfig config;

    public MotionPrototype GetPrototype() => new() {
        config = config
    };
}
