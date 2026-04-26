using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig", order = 0)]
public class PlayerConfig : ScriptableObject {
    public int initPlatformCount;
    public PlatformConfig platformConfig;
    public bool startOrEndCouplingOfRewards = false;
    public LoadoutConfig firstLoadoutConfig;
    public LoadoutConfig secondLoadoutConfig;
}