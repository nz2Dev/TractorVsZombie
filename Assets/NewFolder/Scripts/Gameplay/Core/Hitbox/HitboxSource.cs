using System;
using UnityEngine;

[Serializable]
public struct HitboxSource {
    [NonNull, Local, SerializeField] private RaycastMarker markerPrefab;
    [SerializeField] private ContactSurface surface;

    public HitboxPrototype GetHitboxPrototype() {
        return new HitboxPrototype {
            markerPrefab = markerPrefab,
            surface = surface,
        };
    }
}
