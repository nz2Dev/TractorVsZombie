using UnityEngine;

public class PlayerPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private PlayerConfig configSource;
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private TruckSource initTruckSource;
    [SerializeField] private PlatformSource initPlatformSource;
    [SerializeField] private LoadoutSource initLoadoutSource;

    public PlayerPrototype Get() {
        return new PlayerPrototype {
            config = configSource,
            aimVisualsPrefab = aimVisualsPrefab,
            initTruckPrototype = initTruckSource.GetPrototype(),
            initLoadoutPrototype = initLoadoutSource.GetPrototype(),
            pickupPlatformPrototype = initPlatformSource.GetPrototype()
        };
    }

}