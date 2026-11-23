using UnityEngine;

[System.Serializable]
public struct WeaponPrototype {
    public Vector3 position;
    public WeaponConfig config;
    public WeaponVisuals visualsPrefab;
    public BallisticPrototype ballisticPrototype;
    public Vector3 ballisticLaunchOffset;
}
