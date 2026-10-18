using System;
using UnityEngine;

[Serializable]
public struct HitboxSource {
    [NonNull, Local, SerializeField] private RaycastMarker markerPrefab;

    public HitboxPrototype GetHitboxPrototype() {
        return new HitboxPrototype {
            markerPrefab = markerPrefab,
        };
    }
}
