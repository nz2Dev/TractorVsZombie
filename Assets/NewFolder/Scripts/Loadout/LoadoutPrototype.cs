using UnityEngine;

[System.Serializable]
public struct LoadoutPrototype {
    public Vector3 position;
    public LoadoutConfig config;
    public GameObject shellVisualsPrefab;
    public GameObject rewardVisualsPrefab;
    public WeaponPrototype localWeaponPrototype;
}
