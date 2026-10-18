
using UnityEngine;
using UnityEngine.Serialization;

public class ArmorSource : MonoBehaviour {

    [Inline] [SerializeField] private ArmorConfig config;
    [FormerlySerializedAs("combatSource"), SerializeField] private HealthFactionSource healthFactionSource;
    [Local] [SerializeField] private ArmorVisuals visualsPrefab;
    [Local] [SerializeField] private UnityVehicle vehiclePrefab;
    [SerializeField] private AudioClip engineLoopSFX;
    [Local] [SerializeField] private WeaponSource weaponSource;
    [Local] [SerializeField] private RamEffectSource ramSource;
    [Local, SerializeField] private RaycastMarker raycastMarkerPrefab;
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
            healthPrototype: healthFactionSource.GetHealthPrototype(),
            factionPrototype: healthFactionSource.GetFactionPrototype(),
            raycastMarkerPrefab: raycastMarkerPrefab,
            worldSpaceUIPrefab: worldSpaceUIPrefab
        );
    }
}
