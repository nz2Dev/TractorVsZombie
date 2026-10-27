using System;
using UnityEngine;

[Serializable]
public struct MotionSource {
    [Local, SerializeField] private RagdollBody physicsBodyPrefab;
    [Inline, SerializeField] private MotionConfig config;

    public MotionPrototype GetPrototype() => new() {
        config = config,
        physicsBodyPrefab = physicsBodyPrefab
    };
}
