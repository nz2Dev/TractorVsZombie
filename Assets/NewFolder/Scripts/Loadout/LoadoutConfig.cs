using UnityEngine;

[CreateAssetMenu(fileName = "LoadoutConfig", menuName = "LoadoutConfig")]
public class LoadoutConfig : ScriptableObject {
    public GameObject brokenVisualsPrefab;
    public WeaponConfig weaponConfig;
    public Vector3 weaponLocalOffset;
}
