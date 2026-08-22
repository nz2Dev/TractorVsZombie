using Combat;

using UnityEngine;

public class ArmorSource : MonoBehaviour {

    [Inline] [SerializeField] private ArmorConfig config;
    [Inline, SerializeField] private CombatPrototypeSource combatSource;
    [Local] [SerializeField] private ArmorVisuals visualsPrefab;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [SerializeField] private AudioClip engineLoopSFX;
    [Local] [SerializeField] private WeaponSource weaponSource;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [Local, SerializeField] private RaycastMarker raycastMarkerPrefab;
    [SerializeField] private RewardSource loadoutRewardSource;

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
            combatPrototype: combatSource.Get(),
            raycastMarkerPrefab: raycastMarkerPrefab
        );
    }
}
