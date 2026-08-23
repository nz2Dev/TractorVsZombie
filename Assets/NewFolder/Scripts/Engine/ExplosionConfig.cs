using System;

using UnityEngine;

[Serializable]
public struct ExplosionConfig {
    public float radius;
    public float force;
    public float upwardModifier;
    public ForceMode forceMode;
}
