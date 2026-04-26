using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig", order = 0)]
public class PlayerConfig : ScriptableObject {
    public int initPlatformCount;
    public PlatformSource platformSourcePrefab; // will later be replaced with source pattern itself, 
                                                // and can be referenced from scene
    public bool startOrEndCouplingOfRewards = false;
    public LoadoutSource firstLoadoutSourcePrefab;
    public LoadoutSource secondLoadoutSourcePrefab;
}