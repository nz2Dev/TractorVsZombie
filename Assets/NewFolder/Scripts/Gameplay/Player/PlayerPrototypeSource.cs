using System.Linq;

using UnityEngine;

public class PlayerPrototypeSource : MonoBehaviour {
    
    [SerializeField] private AssemblingSource assemblingSource;
    [SerializeField] private LoaderSource loaderSource;
    [SerializeField] private AimingSource aimingSource;
    [SerializeField] private CollectingSource collectingSource;
    [Space]
    [SerializeField] private LoadoutSource[] initialLoadoutSources;
    [SerializeField] private TruckSource initTruckSource;
    [SerializeField] private int initialPlatformCount;

    public int LoadoutsLength => initialLoadoutSources.Length;
    public PlatformPrototype PlatformPrototype => assemblingSource.InitPlatformSource.GetPrototype();
    public LoadoutPrototype GetLoadoutPrototype(int index) => initialLoadoutSources[index].GetPrototype();

    public PlayerPrototype Get() {
        return new PlayerPrototype {
            initialLoadouts = GetLoadoutPrototypes(),
            initialPlatformCount = initialPlatformCount,
            initialTruckPrototype = initTruckSource.GetPrototype(),
            aimingPrototype = aimingSource.Get(),
            assemblingPrototype = assemblingSource.Get(),
            collectingPrototype = collectingSource.Get(),
        };
    }

    public LoadoutPrototype[] GetLoadoutPrototypes() => 
        initialLoadoutSources == null
            ? new LoadoutPrototype[0]
            : initialLoadoutSources.Select(source => source.GetPrototype()).ToArray();

}
