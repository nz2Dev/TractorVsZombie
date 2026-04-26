using UnityEngine;

[CreateAssetMenu(fileName = "LoadoutConfig", menuName = "LoadoutConfig")]
public class LoadoutConfig : ScriptableObject {
    public GameObject brokenVisualsPrefab;
    public WeaponConfig weaponConfig;
    public WeaponVisuals weaponVisualsPrefab; // currentyl duplicated manually by dicipline, in future
                                                // will have its own source authoring
                                                // and weapon prototype will be specified by holder
    public Vector3 weaponLocalOffset;
}
