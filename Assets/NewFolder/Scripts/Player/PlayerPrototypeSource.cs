using System.Linq;

using UnityEngine;

public class PlayerPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private PlayerConfig configSource;
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private TruckSource initTruckSource;
    [SerializeField] private PlatformSource initPlatformSource;
    [SerializeField] private LoadoutSource[] initLoadoutSources;

    public int LoadoutsLength => initLoadoutSources.Length;
    public PlatformPrototype PlatformPrototype => initPlatformSource.GetPrototype();
    public LoadoutPrototype GetLoadoutPrototype(int index) => initLoadoutSources[index].GetPrototype();

    public PlayerPrototype Get() {
        return new PlayerPrototype {
            config = configSource,
            aimVisualsPrefab = aimVisualsPrefab,
            initTruckPrototype = initTruckSource.GetPrototype(),
            initLoadoutPrototypes = initLoadoutSources == null ? new LoadoutPrototype[0] : initLoadoutSources.Select(source => source.GetPrototype()).ToArray(),
            pickupPlatformPrototype = initPlatformSource.GetPrototype()
        };
    }

}