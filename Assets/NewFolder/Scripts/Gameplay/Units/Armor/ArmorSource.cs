
using UnityEngine;

public class ArmorSource : MonoBehaviour {

    [Inline] [SerializeField] private ArmorConfig config;
    [SerializeField] private HealthSource healthSource;
    [SerializeField] private FactionSource factionSource;
    [Local] [SerializeField] private ArmorVisuals visualsPrefab;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [SerializeField] private AudioClip engineLoopSFX;
    [Local] [SerializeField] private WeaponSource weaponSource;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [SerializeField] private HitboxSource hitboxSource;
    [SerializeField] private RewardSource loadoutRewardSource;
    [NonNull, Local, SerializeField] private WorldSpaceUI worldSpaceUIPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public ArmorPrototype GetPrototype() {
        return new ArmorPrototype (
            position: transform.position,
            config: config,
            visualsPrefab: visualsPrefab,
            vehiclePrefab: vehiclePrefab,
            engineLoopSFX: engineLoopSFX,
            ramPrototype: ramSource != null ? ramSource.GetPrototype() : default,
            localWeaponPrototype: weaponSource != null ? weaponSource.GetPrototype(localTransform: true) : default,
            rewardPrototype: loadoutRewardSource.GetPrototype(),
            healthPrototype: healthSource.GetHealthPrototype(),
            factionPrototype: factionSource.GetFactionPrototype(),
            hitboxPrototype: hitboxSource.GetHitboxPrototype(),
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }
}
