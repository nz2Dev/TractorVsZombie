using UnityEngine;

public class ArmorSource : MonoBehaviour {

    [Inline] [SerializeField] private ArmorConfig config;
    [Local] [SerializeField] private ArmorVisuals visualsPrefab;
    [Local] [SerializeField] private VehiclePhysics physicsPrefab;
    [SerializeField] private AudioClip engineLoopSFX;
    [Local] [SerializeField] private WeaponSource weaponSource;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [SerializeField] private RewardSource loadoutRewardSource;
    
    private void Awake() {
        gameObject.SetActive(false);
    }

    public ArmorPrototype GetPrototype() {
        return new ArmorPrototype {
            position = transform.position,
            config = config,
            visualsPrefab = visualsPrefab,
            physicsPrefab = physicsPrefab,
            engineLoopSFX = engineLoopSFX,
            ramPrototype = ramSource != null ? ramSource.GetPrototype() : default,
            localWeaponPrototype = weaponSource != null ? weaponSource.GetPrototype(localTransform: true) : default,
            rewardPrototype = loadoutRewardSource.GetPrototype(),
        };
    }
}
