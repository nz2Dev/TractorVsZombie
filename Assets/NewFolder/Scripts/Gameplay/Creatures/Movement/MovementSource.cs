using System;
using UnityEngine;

[Serializable]
public struct MovementSource {
    [Inline, SerializeField] private MovementConfig config;
    [SerializeField] private AgentAvoidanceConfig avoidanceConfig;

    public MovementPrototype GetPrototype() => new() {
        config = config,
        avoidanceConfig = avoidanceConfig
    };
}
