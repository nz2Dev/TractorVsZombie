using UnityEngine;

[CreateAssetMenu(fileName = "PlatformConfig", menuName = "PlatformConfig", order = 0)]
public class PlatformConfig : ScriptableObject {
    public RamConfig ramConfig;
    public PlatformVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
    public Vector3 LoadoutOffset;
}