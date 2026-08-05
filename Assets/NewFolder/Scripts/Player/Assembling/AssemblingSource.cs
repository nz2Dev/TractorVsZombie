using System.Linq;

using UnityEngine;

public class AssemblingSource : MonoBehaviour {
    [SerializeField] private TruckSource initTruckSource;
    [SerializeField] private PlatformSource initPlatformSource;
    [SerializeField] private LoadoutSource[] initLoadoutSources;

    public int LoadoutsLength => initLoadoutSources.Length;
    public PlatformPrototype PlatformPrototype => initPlatformSource.GetPrototype();
    public LoadoutPrototype GetLoadoutPrototype(int index) => initLoadoutSources[index].GetPrototype();

    public AssemblingPrototype Get() {
        return new AssemblingPrototype {
            initTruckPrototype = initTruckSource.GetPrototype(),
            pickupPlatformPrototype = initPlatformSource.GetPrototype(),
            initLoadoutPrototypes = initLoadoutSources == null ? new LoadoutPrototype[0] : initLoadoutSources.Select(source => source.GetPrototype()).ToArray()
        };
    }
}